using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Erenshor_Global_Chat_Mod
{
    class EventListener
    {
        [HarmonyPatch(typeof(Stats), "DoLevelUp")]
        class LevelUpPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Stats __instance)
            {
                GameObject playerGameObject = __instance.gameObject;

                if (!IsPlayerCharacter(__instance.gameObject.GetComponent<Character>())) return;

                if(playerGameObject.GetComponent<Stats>().Level % 10 == 0)
                {
                    MessageSender.SendMilestonePackage(Mod.GetSteamUsername(), playerGameObject.GetComponent<Stats>().Level);
                }
            }
        }

        [HarmonyPatch(typeof(Inventory), nameof(Inventory.AddItemToInv), new[] { typeof(Item), typeof(int) })]
        public class PickUpItemPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Inventory __instance, Item _item, int _qual)
            {
                GameObject playerGameObject = __instance.gameObject;

                if (!IsPlayerCharacter(playerGameObject.GetComponent<Character>())) return;
                
                // Check if Item is rare
                if(IsItemSparkly(_item))
                {
                    MessageSender.SendRareItemPackage(Mod.GetSteamUsername(), _item);
                }
            }
        }

        public static bool IsPlayerCharacter(Character character)
        {
            if (character == null)
                return false;

            if (character.gameObject.tag.Equals("Character") && character.gameObject.name.Equals("Player"))
                return true;

            return false;
        }

        public static bool IsItemSparkly(Item item)
        {
            if (item == null)
                return false;

            // Prüfen, ob der Sparkler aktiv ist
            if (item.RequiredSlot != 0 && item.RequiredSlot != Item.SlotType.Aura)
            {
                // Zugriff auf den Sparkler
                Inventory playerInv = GameObject.Find("Player").GetComponent<Inventory>();
                Image sparkler = null;

                playerInv.ALLSLOTS.ForEach(slot =>
                {
                    if (slot.MyItem == item)
                    {
                        sparkler = slot.MySparkler;
                    }
                });

                if (sparkler != null && sparkler.gameObject.activeSelf)
                {
                    // Prüfen der Farbe des Sparklers
                    Color sparklerColor = sparkler.color;
                    if (sparklerColor == Color.cyan || sparklerColor == Color.magenta)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
