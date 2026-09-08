using System;
using HarmonyLib;
using InventoryMaster.Features;
using InventoryMaster.Helpers;

namespace InventoryMaster.Patches
{
    #region [START] PATCH: TOOL BREAK AUTO-REPLACEMENT
    // ============================================================================
    // [START] PATCH: TOOL BREAK AUTO-REPLACEMENT
    // Description: Intercepts PlayerInventory.RemoveStacksFromSlot to detect when a
    //              hotbar tool runs out of durability, instantly auto-replacing it.
    // ============================================================================
    [HarmonyPatch(typeof(PlayerInventory), "RemoveStacksFromSlot")]
    public static class Patch_PlayerInventory_RemoveStacksFromSlot
    {
        private static Item_Base _itemBeforeBreak = null;
        private static bool _wasHotbarSlot = false;

        static void Prefix(PlayerInventory __instance, Slot slot)
        {
            _itemBeforeBreak = null;
            _wasHotbarSlot = false;

            if (__instance == null || slot == null || slot.IsEmpty || !slot.HasValidItemInstance()) return;

            // Check if this slot belongs to the hotbar
            if (slot.slotType == SlotType.Hotbar)
            {
                _wasHotbarSlot = true;
                _itemBeforeBreak = slot.GetItemBase();
            }
        }

        static void Postfix(PlayerInventory __instance, Slot slot, bool __result)
        {
            // If __result is true, the tool just broke and was destroyed/consumed
            if (__result && _wasHotbarSlot && _itemBeforeBreak != null && (slot == null || slot.IsEmpty))
            {
                try
                {
                    AutoToolEquipManager.TryAutoReplaceBrokenTool(__instance, slot, _itemBeforeBreak);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning($"[InventoryMaster] Auto-replace error: {ex.Message}");
                }
            }

            _itemBeforeBreak = null;
            _wasHotbarSlot = false;
        }
    }
    // ============================================================================
    // [END] PATCH: TOOL BREAK AUTO-REPLACEMENT
    // ============================================================================
    #endregion
}
