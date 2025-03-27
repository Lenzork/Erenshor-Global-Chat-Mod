using BepInEx;
using HarmonyLib;
using Steamworks;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Erenshor_Global_Chat_Mod
{
    [BepInPlugin("org.lenzork.globalchatmod", "Erenshor Global Chat Mod", "1.2.0")]
    public class Mod : BaseUnityPlugin
    {
        private static NetworkManager _networkManager;
        private static string steamUsername;
        private bool wrongVersion = false;
        private bool steamChecked = false;

        public void Awake()
        {
            ConfigGenerator.GenerateConfig(this);

            SceneManager.activeSceneChanged += OnSceneWasInitialized;

            // Apply all Patches
            var instance = new Harmony("org.lenzork.globalchatmod");
            instance.PatchAll();
            Logger.LogInfo("All Patches have been loaded!");

            // Print Hash
            Logger.LogInfo($"Mod Hash: {GetModHash()}");
        }

        public static string GetModHash()
        {
            // Pfad zur aktuellen DLL
            string dllPath = Assembly.GetExecutingAssembly().Location;

            if (!File.Exists(dllPath))
            {
                throw new System.Exception("DLL not found");
            }

            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(dllPath))
            {
                byte[] hash = sha256.ComputeHash(stream);
                StringBuilder sb = new StringBuilder();
                foreach (var b in hash)
                    sb.Append(b.ToString("x2")); // hex format
                return sb.ToString();
            }
        }

        public static string GetSteamUsername()
        {
            return steamUsername;
        }

        public static NetworkManager GetNetworkManager()
        {
            return _networkManager;
        }

        public void OnSceneWasInitialized(Scene current, Scene next)
        {
            if(!steamChecked)
            {
                if (!SteamManager.Initialized || SteamFriends.GetPersonaName() == null)
                {
                    Logger.LogError("Steam is not initialized. The Global Chat Mod requires Steam to be running.");
                    Destroy(this);
                    return;
                }

                steamUsername = SteamFriends.GetPersonaName();
                Logger.LogInfo($"Using {steamUsername} as name for the global chat.");

                GameObject globalChat = new GameObject("GlobalChat");
                _networkManager = globalChat.AddComponent<NetworkManager>();
                _networkManager.Init(Logger, steamUsername, this.Info);
                DontDestroyOnLoad(globalChat);

                steamChecked = true;
            }

            if (SceneValidator.IsValidScene(next.name) && !_networkManager.IsConnected && !wrongVersion)
            {
                _networkManager.ConnectToGlobalServer();
            }
            else if (_networkManager.IsConnected && !SceneValidator.IsValidScene(next.name))
            {
                _networkManager.SendDisconnectedPackage();
            }
        }

        public void OnApplicationQuit()
        {
            _networkManager.SendDisconnectedPackage();
        }
    }
}
