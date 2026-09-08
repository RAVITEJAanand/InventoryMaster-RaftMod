using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace InventoryMaster.Patches
{
    #region [START] PATCH: TITLE SCREEN BADGE
    // ============================================================================
    // [START] PATCH: TITLE SCREEN BADGE
    // Purpose: Displays "🎒 Inventory Master Active [F2]" version watermark
    //          under Sailor's Companion on Raft's StartMenuScreen.
    //          Note: The main menu "MODS" button is unified via CanvasInstalledModsUI
    //          so it displays both mods in a clean manager dialog without clutter.
    // ============================================================================
    [HarmonyPatch(typeof(StartMenuScreen), "Start")]
    public static class StartMenuScreenStartPatch
    {
        public static void Postfix(StartMenuScreen __instance)
        {
            InjectBadge(__instance);
        }

        public static void InjectBadge(StartMenuScreen startMenu)
        {
            if (startMenu == null) return;
            try
            {
                var trav = Traverse.Create(startMenu);
                var vText = trav.Field<Text>("versionText")?.Value;
                if (vText != null && !vText.text.Contains("Inventory Master"))
                {
                    vText.text += "\n<color=#FFA726><b>🎒 Inventory Master Active [F2]</b></color>";
                }
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(StartMenuScreen), "LateStart")]
    public static class StartMenuScreenLateStartPatch
    {
        public static void Postfix(StartMenuScreen __instance)
        {
            StartMenuScreenStartPatch.InjectBadge(__instance);
        }
    }

    [HarmonyPatch(typeof(StartMenuScreen), "Update")]
    public static class StartMenuScreenUpdatePatch
    {
        private static float _lastCheck = 0f;
        public static void Postfix(StartMenuScreen __instance)
        {
            if (Time.unscaledTime - _lastCheck > 0.5f)
            {
                _lastCheck = Time.unscaledTime;
                StartMenuScreenStartPatch.InjectBadge(__instance);
            }
        }
    }
    #endregion
}
