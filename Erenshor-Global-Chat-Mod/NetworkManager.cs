using LiteNetLib;
using BepInEx.Logging;
using Newtonsoft.Json;

namespace Erenshor_Global_Chat_Mod
{
    public class NetworkManager
    {
        private NetManager _netManager;
        private NetPeer _serverPeer;
        private EventBasedNetListener _listener;
        private ManualLogSource _logger;
        private string _steamUsername;

        public bool IsConnected => _serverPeer != null && _serverPeer.ConnectionState == ConnectionState.Connected;

        public NetworkManager(ManualLogSource logger, string steamUsername)
        {
            _logger = logger;
            _steamUsername = steamUsername;
            _listener = new EventBasedNetListener();
            _netManager = new NetManager(_listener);

            _listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) => OnNetworkReceive(peer, reader, deliveryMethod); // Fix the delegate signature
            _listener.PeerConnectedEvent += OnPeerConnected;
        }

        public void ConnectToGlobalServer()
        {
            _netManager.Start();
            _serverPeer = _netManager.Connect("127.0.0.1", 9050, "ErenshorGlobalChat");

            int attempts = 0;
            while (_serverPeer.ConnectionState != ConnectionState.Connected && attempts < 5)
            {
                _logger.LogMessage("Connecting to the Global Chat Server...");
                attempts++;
                System.Threading.Thread.Sleep(1000);
            }

            if (_serverPeer.ConnectionState == ConnectionState.Connected)
            {
                _logger.LogMessage("Successfully connected to the Global Chat Server.");
            }
            else
            {
                _logger.LogMessage("Connecting to the Global Chat Server has failed.");
            }
        }

        public void Update()
        {
            if (_serverPeer == null) return;
            _netManager.PollEvents();

            if (_serverPeer.ConnectionState == ConnectionState.Disconnected)
            {
                _logger.LogError("Lost connection to the Global Chat Server.");
                _serverPeer = null;
            }
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            string message = reader.GetString();
            var data = JsonConvert.DeserializeObject<PackageData>(message);
            ChatHandler.HandleReceivedData(data, _logger);
        }

        private void OnPeerConnected(NetPeer peer)
        {
            SendConnectedPackage();
        }

        public void SendConnectedPackage()
        {
            var data = new PackageData
            {
                Type = PackageData.PackageType.Information,
                Info = PackageData.InformationType.PlayerConnected,
                SenderName = _steamUsername,
                ModVersion = "1.0.0"
            };
            MessageSender.SendPackage(_serverPeer, data);
        }

        public void SendDisconnectedPackage()
        {
            var data = new PackageData
            {
                Type = PackageData.PackageType.Information,
                Info = PackageData.InformationType.PlayerDisconnected,
                SenderName = _steamUsername,
                ModVersion = "1.0.0"
            };
            MessageSender.SendPackage(_serverPeer, data);
            _serverPeer.Disconnect();
            _serverPeer = null;
        }
    }
}
