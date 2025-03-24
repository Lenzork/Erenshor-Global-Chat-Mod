using LiteNetLib;
using Newtonsoft.Json;
using BepInEx.Logging;
using LiteNetLib.Utils;

namespace Erenshor_Global_Chat_Mod
{
    public static class MessageSender
    {
        public static void SendPackage(NetPeer peer, PackageData data)
        {
            var writer = new NetDataWriter();
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            writer.Put(JsonConvert.SerializeObject(data, settings));
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendChatMessage(NetPeer peer, string message, string steamUsername, ManualLogSource logger)
        {
            if (peer == null)
            {
                logger.LogError("No connection to the Global Chat Server. Couldn't send message.");
                return;
            }

            if (message.Length > 255)
            {
                logger.LogError("Message is too long. Couldn't send message.");
                return;
            }

            var data = new PackageData
            {
                SenderName = steamUsername,
                Message = message,
                ModVersion = "1.2.0"
            };
            SendPackage(peer, data);
        }

        public static void SendRequestForOnlinePlayers(NetPeer peer, string steamUsername, ManualLogSource logger)
        {
            if (peer == null)
            {
                logger.LogError("No connection to the Global Chat Server. Couldn't send message.");
                return;
            }

            var data = new PackageData
            {
                SenderName = steamUsername,
                Type = PackageData.PackageType.Information,
                Info = PackageData.InformationType.PlayersOnline,
                ModVersion = "1.2.0"
            };
            SendPackage(peer, data);
        }
    }
}
