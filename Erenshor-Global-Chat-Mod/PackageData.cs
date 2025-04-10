namespace Erenshor_Global_Chat_Mod
{
    public struct PackageData
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
            PlayersOnline,
            BlacklistedMessage,
            MessageOfTheDay,
            KickPlayer,
            BanPlayer,
            UnbanPlayer
        }

        public PackageType Type;
        public InformationType Info;
        public string SenderName;
        public string Message;
        public int Duration;
        public string ModVersion;
        public string ModHash;
        public string SteamId;
    }
}
