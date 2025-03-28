using LiteNetLib;
using BepInEx.Logging;
using Newtonsoft.Json;
using BepInEx;
using BepInEx.Bootstrap;
using UnityEngine;
using Steamworks;

namespace Erenshor_Global_Chat_Mod
{
    public class NetworkManager : MonoBehaviour
    {
        private NetManager _netManager;
        private static NetPeer _serverPeer;
        private EventBasedNetListener _listener;
        private ManualLogSource _logger;
        private string _steamUsername;

        public bool IsConnected => _serverPeer != null && _serverPeer.ConnectionState == ConnectionState.Connected;

        public NetPeer GetPeer()
        {
            return _serverPeer;
        }

        public void Init(ManualLogSource logger, string steamUsername, PluginInfo pluginInfo)
        {
            _logger = logger;
            _steamUsername = steamUsername;
            _listener = new EventBasedNetListener();
            _netManager = new NetManager(_listener);

            _listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) =>
            {
                OnNetworkReceive(peer, reader, deliveryMethod);
            };
            _listener.PeerConnectedEvent += OnPeerConnected;
        }

        public void ConnectToGlobalServer()
        {
            _netManager.Start();
            _serverPeer = _netManager.Connect(ConfigGenerator._serverIp.Value, ConfigGenerator._serverPort.Value, "ErenshorGlobalChat");

            int attempts = 0;
            while (_serverPeer.ConnectionState != ConnectionState.Connected && attempts < 5)
            {
                _logger.LogMessage("Connecting to the Global Chat Server...");
                attempts++;
                System.Threading.Thread.Sleep(1000);
            }

            if (_serverPeer.ConnectionState == ConnectionState.Connected)
            {
                UpdateSocialLog.LogAdd("<color=purple>[GLOBAL]</color> <color=green>Successfully connected to the Global Chat Server.</color>");
                _logger.LogMessage("Successfully connected to the Global Chat Server.");
            }
            else
            {
                _logger.LogMessage("Connecting to the Global Chat Server has failed.");
            }
        }

        public void Update()
        {
            if (_serverPeer == null || this._netManager == null) return;
            _netManager.PollEvents();

            if (_serverPeer.ConnectionState == ConnectionState.Disconnected)
            {
                _logger.LogError("Lost connection to the Global Chat Server.");
                UpdateSocialLog.LogAdd("<color=purple>[GLOBAL]</color> <color=red>Lost connection to the Global Chat Server.</color>");
                _serverPeer = null;
            }
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            string message = reader.GetString();
            var data = JsonConvert.DeserializeObject<PackageData>(message);
            ChatHandler.HandleReceivedData(data);
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
                ModVersion = Chainloader.PluginInfos["org.lenzork.globalchatmod"].Metadata.Version.ToString(),
                ModHash = Mod.GetModHash(),
                SteamId = SteamUser.GetSteamID().ToString()
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
                ModVersion = Chainloader.PluginInfos["org.lenzork.globalchatmod"].Metadata.Version.ToString(),
                ModHash = Mod.GetModHash(),
                SteamId = SteamUser.GetSteamID().ToString()
            };
            MessageSender.SendPackage(_serverPeer, data);
            _serverPeer.Disconnect();
            _serverPeer = null;
        }
    }
}
