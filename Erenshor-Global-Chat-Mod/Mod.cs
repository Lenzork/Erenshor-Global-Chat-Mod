using BepInEx;
using Steamworks;
using UnityEngine.SceneManagement;

namespace Erenshor_Global_Chat_Mod
{
    [BepInPlugin("org.lenzork.globalchatmod", "Erenshor Global Chat Mod", "1.0.0")]
    public class Mod : BaseUnityPlugin
    {
        private NetworkManager _networkManager;
        private static string steamUsername;
        private bool wrongVersion = false;

        public void Awake()
        {
            ConfigGenerator.GenerateConfig(this);
            MessageHandler.plugin = this.Info;

            if (!SteamManager.Initialized)
            {
                Logger.LogError("Steam is not initialized. The Global Chat Mod requires Steam to be running.");
                Destroy(this);
                return;
            }

            steamUsername = SteamFriends.GetPersonaName();
            Logger.LogInfo($"Using {steamUsername} as name for the global chat.");
            SceneManager.activeSceneChanged += OnSceneWasInitialized;

            _networkManager = new NetworkManager(Logger, steamUsername);
        }

        public void OnSceneWasInitialized(Scene current, Scene next)
        {
            if (SceneValidator.IsValidScene(next.name) && !_networkManager.IsConnected && !wrongVersion)
            {
                _networkManager.ConnectToGlobalServer();
            }
            else if (_networkManager.IsConnected && !SceneValidator.IsValidScene(next.name))
            {
                _networkManager.SendDisconnectedPackage();
            }
        }

        public void Update()
        {
            _networkManager.Update();
        }

        public void OnApplicationQuit()
        {
            _networkManager.SendDisconnectedPackage();
        }
    }
}
