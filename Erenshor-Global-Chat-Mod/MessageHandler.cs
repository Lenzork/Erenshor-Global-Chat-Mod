using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using MelonLoader;
using UnityEngine;

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

                if (text[0] == '@')
                {
                    string message = text.Substring(1);
                    Mod.SendChatMessageToGlobalServer(message, MelonMod.FindMelon("Erenshor Global Chat Mod", "Lenzork").Info);

                    // Reset Player UI
                    __instance.typed.text = "";
                    __instance.CDFrames = 10f;
                    __instance.InputBox.SetActive(value: false);
                    GameData.PlayerTyping = false;
                    return false;
                } else if (text.Contains("@@"))
                {
                    Mod.writeIntoGlobalByDefault = !Mod.writeIntoGlobalByDefault;
                    UpdateSocialLog.LogAdd("Chatting in global chat by default is now " + (Mod.writeIntoGlobalByDefault ? "enabled" : "disabled"));
                }
                // When the player types a message, it will be sent to the global chat by default
                if (Mod.writeIntoGlobalByDefault)
                {
                    string message = text.Substring(0);
                    Mod.SendChatMessageToGlobalServer(message, MelonMod.FindMelon("Erenshor Global Chat Mod", "Lenzork").Info);
                    return false;
                }
                return true;
            }
        }
    }
}
