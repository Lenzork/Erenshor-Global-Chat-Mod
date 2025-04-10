using LiteNetLib;
using Newtonsoft.Json;
using LiteNetLib.Utils;
using BepInEx.Bootstrap;
using Steamworks;

namespace Erenshor_Global_Chat_Mod
{
    public class MessageSender
    {
        public static void SendPackage(NetPeer peer, PackageData data)
        {
            var writer = new NetDataWriter();
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            writer.Put(JsonConvert.SerializeObject(data, settings));
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendChatMessage(string message)
        {
            if (Mod.GetNetworkManager().GetPeer() == null)
            {
                Mod.SendChatLogMessage("No connection to the Global Chat Server. Couldn't send message.");
                return;
            }

            if (message.Length > 255)
            {
                Mod.SendChatLogMessage("Message is too long. Couldn't send message.");
                return;
            }

            var data = new PackageData
            {
                SenderName = Mod.GetSteamUsername(),
                Type = PackageData.PackageType.ChatMessage,
                Message = message,
                ModVersion = Chainloader.PluginInfos["org.lenzork.globalchatmod"].Metadata.Version.ToString(),
                ModHash = Mod.GetModHash(),
                SteamId = SteamUser.GetSteamID().ToString()
            };
            SendPackage(Mod.GetNetworkManager().GetPeer(), data);
        }

        public static void SendRequestForOnlinePlayers()
        {
            if (Mod.GetNetworkManager().GetPeer() == null)
            {
                Mod.SendChatLogMessage("No connection to the Global Chat Server. Couldn't send message.");
                return;
            }

            var data = new PackageData
            {
                SenderName = Mod.GetSteamUsername(),
                Type = PackageData.PackageType.Information,
                Info = PackageData.InformationType.PlayersOnline,
                ModVersion = Chainloader.PluginInfos["org.lenzork.globalchatmod"].Metadata.Version.ToString(),
                ModHash = Mod.GetModHash(),
                SteamId = SteamUser.GetSteamID().ToString()
            };
            SendPackage(Mod.GetNetworkManager().GetPeer(), data);
        }

        public static void SendRequestToKickPlayer(int peerId)
        {
            if(Mod.GetNetworkManager().GetPeer() == null)
            {
                UpdateSocialLog.LogAdd("No connection to the Global Chat Server. Couldn't send command.");
                return;
            }

            var data = new PackageData
            {
                SenderName = Mod.GetSteamUsername(),
                Type = PackageData.PackageType.Information,
                Info = PackageData.InformationType.KickPlayer,
                Message = peerId.ToString(),
                ModVersion = Chainloader.PluginInfos["org.lenzork.globalchatmod"].Metadata.Version.ToString(),
                ModHash = Mod.GetModHash(),
                SteamId = SteamUser.GetSteamID().ToString()
            };
            SendPackage(Mod.GetNetworkManager().GetPeer(), data);
        }

        public static void SendRequestToBanPlayer(int peerId, int durationInSeconds)
        {
            if (Mod.GetNetworkManager().GetPeer() == null)
            {
                UpdateSocialLog.LogAdd("No connection to the Global Chat Server. Couldn't send command.");
                return;
            }

            var data = new PackageData
            {
                SenderName = Mod.GetSteamUsername(),
                Type = PackageData.PackageType.Information,
                Info = PackageData.InformationType.BanPlayer,
                Message = peerId.ToString(),
                Duration = durationInSeconds,
                ModVersion = Chainloader.PluginInfos["org.lenzork.globalchatmod"].Metadata.Version.ToString(),
                ModHash = Mod.GetModHash(),
                SteamId = SteamUser.GetSteamID().ToString()
            };
            SendPackage(Mod.GetNetworkManager().GetPeer(), data);
        }

        public static void SendRequestToBanPlayer(int peerId)
        {
            if (Mod.GetNetworkManager().GetPeer() == null)
            {
                UpdateSocialLog.LogAdd("No connection to the Global Chat Server. Couldn't send command.");
                return;
            }

            var data = new PackageData
            {
                SenderName = Mod.GetSteamUsername(),
                Type = PackageData.PackageType.Information,
                Info = PackageData.InformationType.BanPlayer,
                Message = peerId.ToString(),
                Duration = -1,
                ModVersion = Chainloader.PluginInfos["org.lenzork.globalchatmod"].Metadata.Version.ToString(),
                ModHash = Mod.GetModHash(),
                SteamId = SteamUser.GetSteamID().ToString()
            };
            SendPackage(Mod.GetNetworkManager().GetPeer(), data);
        }

        public static void SendRequestToUnbanPlayer(int peerId)
        {
            if (Mod.GetNetworkManager().GetPeer() == null)
            {
                UpdateSocialLog.LogAdd("No connection to the Global Chat Server. Couldn't send command.");
                return;
            }

            var data = new PackageData
            {
                SenderName = Mod.GetSteamUsername(),
                Type = PackageData.PackageType.Information,
                Info = PackageData.InformationType.UnbanPlayer,
                Message = peerId.ToString(),
                ModVersion = Chainloader.PluginInfos["org.lenzork.globalchatmod"].Metadata.Version.ToString(),
                ModHash = Mod.GetModHash(),
                SteamId = SteamUser.GetSteamID().ToString()
            };
            SendPackage(Mod.GetNetworkManager().GetPeer(), data);
        }
    }
}
