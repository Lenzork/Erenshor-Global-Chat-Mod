using LiteNetLib;
using Newtonsoft.Json;
using BepInEx.Logging;
using LiteNetLib.Utils;
using BepInEx;
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

        public static void SendChatMessage(string message, string steamUsername)
        {
            if (Mod.GetNetworkManager().GetPeer() == null)
            {
                UpdateSocialLog.LogAdd("No connection to the Global Chat Server. Couldn't send message.");
                return;
            }

            if (message.Length > 255)
            {
                UpdateSocialLog.LogAdd("Message is too long. Couldn't send message.");
                return;
            }

            var data = new PackageData
            {
                SenderName = steamUsername,
                Type = PackageData.PackageType.ChatMessage,
                Message = message,
                ModVersion = Chainloader.PluginInfos["org.lenzork.globalchatmod"].Metadata.Version.ToString(),
                ModHash = Mod.GetModHash(),
                SteamId = SteamUser.GetSteamID().ToString()
            };
            SendPackage(Mod.GetNetworkManager().GetPeer(), data);
        }

        public static void SendRequestForOnlinePlayers(string steamUsername)
        {
            if (Mod.GetNetworkManager().GetPeer() == null)
            {
                UpdateSocialLog.LogAdd("No connection to the Global Chat Server. Couldn't send message.");
                return;
            }

            var data = new PackageData
            {
                SenderName = steamUsername,
                Type = PackageData.PackageType.Information,
                Info = PackageData.InformationType.PlayersOnline,
                ModVersion = Chainloader.PluginInfos["org.lenzork.globalchatmod"].Metadata.Version.ToString(),
                ModHash = Mod.GetModHash(),
                SteamId = SteamUser.GetSteamID().ToString()
            };
            SendPackage(Mod.GetNetworkManager().GetPeer(), data);
        }
    }
}
