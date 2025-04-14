using BepInEx;
using BepInEx.Configuration;
using System;

namespace Erenshor_Global_Chat_Mod
{
    public static class ConfigGenerator
    {
        public static ConfigEntry<string> _serverIp;
        public static ConfigEntry<int> _serverPort;
        public static ConfigEntry<bool> _enablePrintInGlobalChat;

        public static void GenerateConfig(BaseUnityPlugin baseUnityPlugin)
        {
            _serverIp = baseUnityPlugin.Config.Bind("Connection", "ServerIP", "45.131.109.73", "The IP of the Global Chat Server.");
            _serverPort = baseUnityPlugin.Config.Bind("Connection", "ServerPort", 9050, "The port of the Global Chat Server.");
            _enablePrintInGlobalChat = baseUnityPlugin.Config.Bind("Preferences", "EnablePrintInGlobalChat", false, "Will also print all Messages of the mod into the games Global Chat Tab and not only the local Chat Tab.");
        }
    }
}
