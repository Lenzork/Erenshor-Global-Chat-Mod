using BepInEx.Logging;

namespace Erenshor_Global_Chat_Mod
{
    public static class ChatHandler
    {
        public static void HandleReceivedData(PackageData data, ManualLogSource logger)
        {
            switch (data.Type)
            {
                case PackageData.PackageType.ChatMessage:
                    UpdateSocialLog.LogAdd($"<color=purple>[GLOBAL]</color> {data.SenderName}: {data.Message}");
                    break;
                case PackageData.PackageType.Information:
                    HandleInformation(data, logger);
                    break;
            }
        }

        private static void HandleInformation(PackageData data, ManualLogSource logger)
        {
            switch (data.Info)
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
                    logger.LogWarning("Disconnected due to Version mismatch with the server.");
                    UpdateSocialLog.LogAdd($"<color=purple>[GLOBAL]</color> {data.Message}");
                    break;
            }
        }
    }
}
