using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;

namespace Erenshor_Global_Chat_Mod
{
    public static class ConfigGenerator
    {
        public static ConfigEntry<string> _serverIp;
        public static ConfigEntry<int> _serverPort;
        public static ConfigEntry<bool> _enablePrintInGlobalChat;
        public static ConfigEntry<List<string>> _ignoredPlayers;

        public static void GenerateConfig(BaseUnityPlugin baseUnityPlugin)
        {
            _serverIp = baseUnityPlugin.Config.Bind("Connection", "ServerIP", "127.0.0.1", "The IP of the Global Chat Server.");
            _serverPort = baseUnityPlugin.Config.Bind("Connection", "ServerPort", 9050, "The port of the Global Chat Server.");
            _enablePrintInGlobalChat = baseUnityPlugin.Config.Bind("Preferences", "EnablePrintInGlobalChat", false, "Will also print all Messages of the mod into the games Global Chat Tab and not only the local Chat Tab.");
            _ignoredPlayers = baseUnityPlugin.Config.Bind("Preferences", "IgnoredPlayers", new List<string>(), "List of players to ignore. Use SteamID64 format. Example: 76561198000000000");
        }
    }
}
