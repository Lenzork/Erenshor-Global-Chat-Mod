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
                    Mod.SendChatMessageToGlobalServer(message);

                    // Reset Player UI
                    __instance.typed.text = "";
                    __instance.CDFrames = 10f;
                    __instance.InputBox.SetActive(value: false);
                    GameData.PlayerTyping = false;
                    return false;
                }
                return true;
            }
        }
    }
}
