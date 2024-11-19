using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using UnityEngine;
using Steamworks;
using LiteNetLib;
using Newtonsoft.Json;
using LiteNetLib.Utils;

namespace Erenshor_Global_Chat_Mod
{
    public class Mod : MelonMod
    {
        private NetManager _netManager;
        private static NetPeer _serverPeer;
        private EventBasedNetListener _listener;
        private static string steamUsername;
        private const string SERVER_IP = "127.0.0.1"; // Enter the IP of the global chat server here
        private bool wrongVersion = false;

        private static string[] ValidScenes = new string[]
        {
            "Stowaway",
            "Brake",
            "Bonepits",
            "Vitheo",
            "Krakengard",
            "FernallaField",
            "SaltedStrand",
            "Elderstone",
            "Azure",
            "Rottenfoot",
            "Braxonian",
            "Silkengrass",
            "Underspine",
            "Loomingwood",
            "Duskenlight",
            "Windwashed",
            "Blight",
            "Malaroth",
            "Braxonia",
            "Soluna",
            "Ripper",
            "Abyssal",
            "VitheosEnd",
            "Azynthi",
            "AzynthiClear",
            "DuskenPortal",
            "Rockshade",
            "ShiveringTomb",
            "Undercity",
            "Jaws"
        };

        private struct PackageData
        {
            public enum PackageType
            {
                ChatMessage,
                Information
            }

            public enum InformationType
            {
                PlayerConnected,
                PlayerDisconnected,
                VersionMismatch,
                PlayersOnline
            }

            public PackageType Type;
            public InformationType Info;
            public string SenderName;
            public string Message;
            public string ModVersion;
        }

        public override void OnLateInitializeMelon()
        {
            // Check if steam is running
            if (!SteamManager.Initialized)
            {
                MelonLogger.BigError("Erenshor_Global_Chat_Mod", "Steam is not initialized. The Global Chat Mod requires Steam to be running.");

                // Deactive/Unload the mod
                GetThisMelonMod().Unregister();
            }

            steamUsername = SteamFriends.GetPersonaName();
            MelonLogger.Msg($"Using {steamUsername} as name for the global chat.");
        }

        private MelonMod GetThisMelonMod()
        {
            return RegisteredMelons.FirstOrDefault(m => m.Equals(this));
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (ValidScenes.Contains(sceneName) && _serverPeer == null && !wrongVersion)
            {
                ConnectToGlobalServer();
            }
            else if(_serverPeer != null && !ValidScenes.Contains(sceneName))
            {
                SendDisconnectedPackage();
            }
        }

        public override void OnUpdate()
        {
            if(_serverPeer == null)
            {
                return;
            }

            // Netzwerkevents verarbeiten
            _netManager.PollEvents();

            // Verbindung zum Server überprüfen
            if (_serverPeer.ConnectionState == ConnectionState.Disconnected)
            {
                MelonLogger.Error("Lost connection to the Global Chat Server.");
                UpdateSocialLog.LogAdd($"<color=purple>[GLOBAL]</color> Lost connection to the Global Chat Server.");
                _serverPeer = null;
            }
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            // Daten vom Server empfangen
            string message = reader.GetString();

            // Beispiel: Spielerposition aktualisieren
            var data = JsonConvert.DeserializeObject<PackageData>(message);

            if (data.Type == PackageData.PackageType.ChatMessage) 
            { 
                ReceiveChatMessage(data);
            } else if(data.Type == PackageData.PackageType.Information)
            {
                switch(data.Info)
                {
                    case PackageData.InformationType.PlayerConnected:
                        UpdateSocialLog.LogAdd($"<color=purple>[GLOBAL]</color> <color=yellow>{data.SenderName} has <color=green>connected</color> to the Global Chat.</color>");
                        break;
                    case PackageData.InformationType.PlayerDisconnected:
                        UpdateSocialLog.LogAdd($"<color=purple>[GLOBAL]</color> <color=yellow>{data.SenderName} has <color=red>disconnected</color> from the Global Chat.</color>");
                        break;
                    case PackageData.InformationType.PlayersOnline:
                        UpdateSocialLog.LogAdd($"<color=purple>[GLOBAL]</color> {data.Message}");
                        break;
                    case PackageData.InformationType.VersionMismatch:
                        MelonLogger.Warning("Disconnected due to Version mismatch with the server.");
                        UpdateSocialLog.LogAdd($"<color=purple>[GLOBAL]</color> {data.Message}");
                        peer.Disconnect();
                        wrongVersion = true;
                        break;
                }
            }
        }

        public static void SendChatMessageToGlobalServer(string message, MelonInfoAttribute modInfo)
        {
            if(_serverPeer == null)
            {
                MelonLogger.Error("No connection to the Global Chat Server. Couldn't send message.");
                return;
            }

            if(message.Length > 255)
            {
                MelonLogger.Error("Message is too long. Couldn't send message.");
                UpdateSocialLog.LogAdd($"<color=purple>[GLOBAL]</color> Message is too long. Couldn't send message.");
                return;
            }

            MelonLogger.Msg($"Sending message: {message}");
            UpdateSocialLog.LogAdd($"<color=purple>[GLOBAL]</color> {Mod.GetSteamUsername()}: {message}");

            var writer = new NetDataWriter();
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            PackageData data = new PackageData
            {
                SenderName = GetSteamUsername(),
                Message = message,
                ModVersion = modInfo.Version.ToString()
            };

            writer.Put(JsonConvert.SerializeObject(data, settings));

            // Send Message to the Server
            _serverPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendRequestForOnlinePlayersToGlobalServer(MelonInfoAttribute modInfo)
        {
            if (_serverPeer == null)
            {
                MelonLogger.Error("No connection to the Global Chat Server. Couldn't send message.");
                return;
            }

            MelonLogger.Msg($"Sending request to see online players to global server");

            var writer = new NetDataWriter();
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            PackageData data = new PackageData
            {
                SenderName = GetSteamUsername(),
                Type = PackageData.PackageType.Information,
                Info = PackageData.InformationType.PlayersOnline,
                ModVersion = modInfo.Version.ToString()
            };

            writer.Put(JsonConvert.SerializeObject(data, settings));

            // Send Message to the Server
            _serverPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        private void ReceiveChatMessage(PackageData data)
        {
            UpdateSocialLog.LogAdd($"<color=purple>[GLOBAL]</color> {data.SenderName}: {data.Message}");
        }

        public static string GetSteamUsername()
        {
            return steamUsername;
        }

        public override void OnApplicationQuit()
        {
            SendDisconnectedPackage();
        }

        private void SendConnectedPackage()
        {
            var writer = new NetDataWriter();
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            PackageData data = new PackageData
            {
                Type = PackageData.PackageType.Information,
                Info = PackageData.InformationType.PlayerConnected,
                SenderName = steamUsername,
                ModVersion = GetThisMelonMod().Info.Version.ToString()
            };

            writer.Put(JsonConvert.SerializeObject(data, settings));

            // Send Message to the Server
            _serverPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        private void SendDisconnectedPackage()
        {
            var writer = new NetDataWriter();
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            PackageData data = new PackageData
            {
                Type = PackageData.PackageType.Information,
                Info = PackageData.InformationType.PlayerDisconnected,
                SenderName = steamUsername,
                ModVersion = GetThisMelonMod().Info.Version.ToString()
            };

            writer.Put(JsonConvert.SerializeObject(data, settings));

            _serverPeer.Send(writer, DeliveryMethod.ReliableOrdered);

            Task.Run(async () =>
            {
                while (_serverPeer.GetPacketsCountInReliableQueue(0, true) > 0)
                {
                    await Task.Delay(50);
                }

                // Verbindung trennen
                _serverPeer.Disconnect();
                _serverPeer = null;
            });
            
        }

        private void ConnectToGlobalServer()
        {
            _listener = new EventBasedNetListener();
            _netManager = new NetManager(_listener);

            _listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) =>
            {
                OnNetworkReceive(peer, reader, deliveryMethod);
            };
            _listener.PeerConnectedEvent += (peer) =>
            {
                SendConnectedPackage();
            };

            // Client starten  
            _netManager.Start();
            _serverPeer = _netManager.Connect(SERVER_IP, 9050, "ErenshorGlobalChat");

            int attemps = 0;

            while (_serverPeer.ConnectionState != ConnectionState.Connected && attemps < 5)
            {
                MelonLogger.Msg("Connecting to the Global Chat Server...");
                UpdateSocialLog.LogAdd($"<color=purple>[GLOBAL]</color> Connecting to the Global Chat Server... Attempt {attemps + 1}/5");
                attemps++;
                System.Threading.Thread.Sleep(1000);
            }

            if (_serverPeer.ConnectionState == ConnectionState.Connected)
            {
                MelonLogger.Msg("Successfully connected to the Global Chat Server.");
                UpdateSocialLog.LogAdd($"<color=purple>[GLOBAL]</color> Successfully connected to the Global Chat Server as {steamUsername}.");
                MelonLogger.Warning("DISCLAIMER: NO CHAT MODERATION IS IN PLACE. USE AT YOUR OWN RISK.");
            }
            else
            {
                MelonLogger.Error("Connecting to the Global Chat Server has failed.");
            }
        }
    }
}
