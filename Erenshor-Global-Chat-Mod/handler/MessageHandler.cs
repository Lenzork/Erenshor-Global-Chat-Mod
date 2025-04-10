using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Erenshor_Global_Chat_Mod
{
    internal class MessageHandler
    {
        private static bool writeIntoGlobalByDefault = false;

        [HarmonyPatch(typeof(TypeText), "CheckInput")]
        public static class Patch
        {
            private static readonly Dictionary<Func<string, bool>, Action<TypeText>> CommandHandlers = new Dictionary<Func<string, bool>, Action<TypeText>>
                {
                    { text => text.Contains("@@"), HandleToggleGlobalChat },
                    { text => text.StartsWith("@"), HandleSendGlobalMessage },
                    { text => text.Contains("/@online"), HandleRequestOnlinePlayers },
                    { text => text.StartsWith("/@connect"), HandleConnectToServer },
                    { text => text.StartsWith("/@kick"), HandleKickPlayer },
                    { text => text.StartsWith("/@ban"), HandleBanPlayer },
                    { text => text.StartsWith("/@unban"), HandleUnbanPlayer }
                };

            [HarmonyPrefix]
            public static bool Prefix(TypeText __instance)
            {
                string text = __instance.typed.text.ToString();

                foreach (var handler in CommandHandlers)
                {
                    if (handler.Key(text))
                    {
                        handler.Value(__instance);
                        ResetPlayerUI(__instance);
                        return false;
                    }
                }

                if (writeIntoGlobalByDefault)
                {
                    HandleSendGlobalMessage(__instance);
                    ResetPlayerUI(__instance);
                    return false;
                }

                return true;
            }

            private static void HandleToggleGlobalChat(TypeText __instance)
            {
                writeIntoGlobalByDefault = !writeIntoGlobalByDefault;
                Mod.SendChatLogMessage("<color=purple>[GLOBAL]</color> <color=yellow>Chatting in global chat by default is now " + (writeIntoGlobalByDefault ? "enabled" : "disabled") + "</color>");
            }

            private static void HandleSendGlobalMessage(TypeText __instance)
            {
                string message = __instance.typed.text.StartsWith("@") ? __instance.typed.text.Substring(1) : __instance.typed.text;
                MessageSender.SendChatMessage(message);
            }

            private static void HandleRequestOnlinePlayers(TypeText __instance)
            {
                MessageSender.SendRequestForOnlinePlayers();
            }

            private static void HandleConnectToServer(TypeText __instance)
            {
                Mod.GetNetworkManager().ConnectToGlobalServer();
            }

            private static void HandleKickPlayer(TypeText __instance)
            {
                try
                {
                    int peerId = int.Parse(__instance.typed.text.Substring(7));
                    MessageSender.SendRequestToKickPlayer(peerId);
                }
                catch (FormatException)
                {
                    Mod.SendChatLogMessage("<color=purple>[GLOBAL]</color> <color=red>Invalid player ID format. Please use a valid number.</color>");
                }
            }

            private static void HandleBanPlayer(TypeText __instance)
            {
                try
                {
                    int peerId = int.Parse(__instance.typed.text.Substring(6));
                    MessageSender.SendRequestToBanPlayer(peerId);
                }
                catch (FormatException)
                {
                    Mod.SendChatLogMessage("<color=purple>[GLOBAL]</color> <color=red>Invalid player ID format. Please use a valid number.</color>");
                }
            }

            private static void HandleUnbanPlayer(TypeText __instance)
            {
                try
                {
                    int peerId = int.Parse(__instance.typed.text.Substring(8));
                    MessageSender.SendRequestToUnbanPlayer(peerId);
                }
                catch (FormatException)
                {
                    Mod.SendChatLogMessage("<color=purple>[GLOBAL]</color> <color=red>Invalid player ID format. Please use a valid number.</color>");
                }
            }

            private static void ResetPlayerUI(TypeText __instance)
            {
                __instance.typed.text = "";
                __instance.CDFrames = 10f;
                __instance.InputBox.SetActive(value: false);
                GameData.PlayerTyping = false;
            }
        }
    }
}
