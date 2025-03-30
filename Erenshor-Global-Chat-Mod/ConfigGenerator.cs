using BepInEx;
using BepInEx.Configuration;

namespace Erenshor_Global_Chat_Mod
{
    public static class ConfigGenerator
    {
        public static ConfigEntry<string> _serverIp;
        public static ConfigEntry<int> _serverPort;

        public static void GenerateConfig(BaseUnityPlugin baseUnityPlugin)
        {
            _serverIp = baseUnityPlugin.Config.Bind("Connection", "ServerIP", "127.0.0.1", "The IP of the Global Chat Server.");
            _serverPort = baseUnityPlugin.Config.Bind("Connection", "ServerPort", 9050, "The port of the Global Chat Server.");
        }
    }
}
