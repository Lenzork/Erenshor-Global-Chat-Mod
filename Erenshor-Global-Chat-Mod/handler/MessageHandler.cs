using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using BepInEx;
using UnityEngine;
using static BepInEx.Logging.Logger;
using BepInEx.Logging;

namespace Erenshor_Global_Chat_Mod
{
    internal class MessageHandler
    {
        private static bool writeIntoGlobalByDefault = false;
        public static PluginInfo plugin = null;

        [HarmonyPatch(typeof(TypeText), "CheckInput")]
        private static class Patch
        {
            private static bool Prefix(TypeText __instance)
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
                    MessageSender.SendChatMessage(message, plugin.Metadata.Version.ToString(), Steamworks.SteamFriends.GetPersonaName(), BepInEx.Logging.Logger);

                    // Reset Player UI
                    resetPlayerUI(__instance);
                    return false;
                } else if (text.Contains("/@online"))
                {
                    MessageSender.SendChatMessage(MelonMod.FindMelon("Erenshor Global Chat Mod", "Lenzork").Info);

                    // Reset Player UI
                    resetPlayerUI(__instance);
                    return false;
                } else if (writeIntoGlobalByDefault)
                {
                    string message = text.Substring(0);
                    MessageSender.SendChatMessage(message, MelonMod.FindMelon("Erenshor Global Chat Mod", "Lenzork").Info);

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
