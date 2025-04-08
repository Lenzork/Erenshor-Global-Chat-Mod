namespace Erenshor_Global_Chat_Mod
{
    public static class ChatHandler
    {
        public static void HandleReceivedData(PackageData data)
        {
            if(data.Type == PackageData.PackageType.ChatMessage) {
                Mod.SendChatLogMessage($"<color=purple>[GLOBAL]</color> {data.SenderName}: {data.Message}");
            }
            else if (data.Type == PackageData.PackageType.Information)
            {
                HandleInformation(data);
            }
        }

        private static void HandleInformation(PackageData data)
        {
            switch (data.Info)
            {
                case PackageData.InformationType.PlayerConnected:
                    Mod.SendChatLogMessage($"<color=purple>[GLOBAL]</color> <color=yellow>{data.SenderName} has <color=green>connected</color> to the Global Chat.</color>");
                    break;
                case PackageData.InformationType.PlayerDisconnected:
                    Mod.SendChatLogMessage($"<color=purple>[GLOBAL]</color> <color=yellow>{data.SenderName} has <color=red>disconnected</color> from the Global Chat.</color>");
                    break;
                case PackageData.InformationType.PlayersOnline:
                    Mod.SendChatLogMessage($"<color=purple>[GLOBAL]</color> {data.Message}");
                    break;
                case PackageData.InformationType.VersionMismatch:
                    Mod.SendChatLogMessage($"<color=purple>[GLOBAL]</color> {data.Message}");
                    break;
                case PackageData.InformationType.BlacklistedMessage:
                    Mod.SendChatLogMessage($"<color=purple>[GLOBAL]</color> <color=red>{data.Message}</color>");
                    break;
                case PackageData.InformationType.MessageOfTheDay:
                    Mod.SendChatLogMessage($"<color=purple>[GLOBAL]</color> <color=yellow>{data.Message}</color>");
                    break;
            }
        }
    }
}
