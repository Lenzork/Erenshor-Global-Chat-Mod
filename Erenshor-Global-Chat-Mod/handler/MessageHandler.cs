using HarmonyLib;

namespace Erenshor_Global_Chat_Mod
{
    internal class MessageHandler
    {
        private static bool writeIntoGlobalByDefault = false;

        [HarmonyPatch(typeof(TypeText), "CheckInput")]
        public static class Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(TypeText __instance)
            {
                string text = __instance.typed.text.ToString();

                if (text.Contains("@@"))
                {
                    writeIntoGlobalByDefault = !writeIntoGlobalByDefault;
                    UpdateSocialLog.LogAdd("<color=purple>[GLOBAL]</color> <color=yellow>Chatting in global chat by default is now " + (writeIntoGlobalByDefault ? "enabled" : "disabled") + "</color>");

                    // Reset Player UI
                    resetPlayerUI(__instance);
                    return false;
                }
                else if (text[0] == '@')
                {
                    string message = text.Substring(1);
                    MessageSender.SendChatMessage(message, Mod.GetSteamUsername());

                    // Reset Player UI
                    resetPlayerUI(__instance);
                    return false;
                } else if (text.Contains("/@online"))
                {
                    MessageSender.SendRequestForOnlinePlayers(Mod.GetSteamUsername());

                    // Reset Player UI
                    resetPlayerUI(__instance);
                    return false;
                } else if (text.StartsWith("/@connect")) {
                    Mod.GetNetworkManager().ConnectToGlobalServer();

                    // Reset Player UI
                    resetPlayerUI(__instance);
                    return false;
                } else if (writeIntoGlobalByDefault)
                {
                    string message = text.Substring(0);
                    MessageSender.SendChatMessage(message, Mod.GetSteamUsername());

                    // Reset Player UI
                    resetPlayerUI(__instance);
                    return false;
                }
                return true;
            }

            private static void resetPlayerUI(TypeText __instance)
            {
                // Reset Player UI
                __instance.typed.text = "";
                __instance.CDFrames = 10f;
                __instance.InputBox.SetActive(value: false);
                GameData.PlayerTyping = false;
            }
        }
    }
}
