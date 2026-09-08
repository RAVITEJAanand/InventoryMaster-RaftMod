using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using InventoryMaster.Features;
using InventoryMaster.Helpers;
using InventoryMaster.UI;

namespace InventoryMaster.Patches
{
    #region [START] PATCH: SLOT POINTER & CLICK INTERACTIONS
    // ============================================================================
    // [START] PATCH: SLOT POINTER & CLICK INTERACTIONS
    // Description: Intercepts Slot pointer down events to handle Alt+Click (Lock),
    //              Shift/Ctrl+RightClick (Smart Split), and Double Click (Fast Transfer).
    // ============================================================================
    [HarmonyPatch(typeof(Slot), "OnPointerDown")]
    public static class Patch_Slot_OnPointerDown
    {
        static bool Prefix(Slot __instance, PointerEventData eventData)
        {
            if (__instance == null || eventData == null) return true;

            // 1. Alt + Left Click = Toggle Favorite Lock (Feature 13)
            if (InputHelper.IsAltHeld() && eventData.button == PointerEventData.InputButton.Left)
            {
                FavoriteLockManager.ToggleLock(__instance);
                SlotLockOverlay.UpdateSlotVisual(__instance);
                return false; // Suppress default click/drag
            }

            // 2. Smart Item Split (Feature 9)
            if (SmartItemSplitter.HandleSlotSplit(__instance, eventData))
            {
                return false; // Consumed
            }

            // 3. Fast Item Transfer (Feature 11)
            if (FastItemTransferManager.HandleFastTransfer(__instance, eventData))
            {
                return false; // Consumed
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Slot), nameof(Slot.RefreshComponents))]
    public static class Patch_Slot_RefreshComponents
    {
        static void Postfix(Slot __instance)
        {
            try
            {
                SlotLockOverlay.UpdateSlotVisual(__instance);
            }
            catch { }
        }
    }
    // ============================================================================
    // [END] PATCH: SLOT POINTER & CLICK INTERACTIONS
    // ============================================================================
    #endregion
}
