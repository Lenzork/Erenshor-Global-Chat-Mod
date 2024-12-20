﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using MelonLoader;
using UnityEngine;
using static MelonLoader.MelonLogger;

namespace Erenshor_Global_Chat_Mod
{
    internal class MessageHandler
    {
        [HarmonyPatch(typeof(TypeText), "CheckInput")]
        private static class Patch
        {
            private static bool Prefix(TypeText __instance)
            {
                string text = __instance.typed.text.ToString();

                if (text.Contains("@@"))
                {
                    Mod.SetWriteIntoGlobalByDefault(!Mod.GetWriteIntoGlobalByDefault());
                    UpdateSocialLog.LogAdd("<color=purple>[GLOBAL]</color> <color=yellow>Chatting in global chat by default is now " + (Mod.GetWriteIntoGlobalByDefault() ? "enabled" : "disabled") + "</color>");
                    
                    // Reset Player UI
                    resetPlayerUI(__instance);
                    return false;
                }
                else if (text[0] == '@')
                {
                    string message = text.Substring(1);
                    Mod.SendChatMessageToGlobalServer(message, MelonMod.FindMelon("Erenshor Global Chat Mod", "Lenzork").Info);

                    // Reset Player UI
                    resetPlayerUI(__instance);
                    return false;
                } else if (text.Contains("/@online"))
                {
                    Mod.SendRequestForOnlinePlayersToGlobalServer(MelonMod.FindMelon("Erenshor Global Chat Mod", "Lenzork").Info);

                    // Reset Player UI
                    resetPlayerUI(__instance);
                    return false;
                } else if (Mod.GetWriteIntoGlobalByDefault())
                {
                    string message = text.Substring(0);
                    Mod.SendChatMessageToGlobalServer(message, MelonMod.FindMelon("Erenshor Global Chat Mod", "Lenzork").Info);

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
