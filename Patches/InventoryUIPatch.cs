using System;
using HarmonyLib;
using UnityEngine;
using InventoryMaster.Features;
using InventoryMaster.Helpers;

namespace InventoryMaster.Patches
{
    #region [START] PATCH: INVENTORY SHORTCUTS & ACTION HOOKS
    // ============================================================================
    // [START] PATCH: INVENTORY SHORTCUTS & ACTION HOOKS
    // Description: Hooks inventory update loop to detect action hotkeys:
    //              - Delete: Trashes currently hovered slot
    //              - Z: Sorts inventory / chest
    //              - K: Quick Stacks to nearby chests
    //              - X: Dumps backpack into open chest
    // ============================================================================
    [HarmonyPatch(typeof(Inventory), "Update")]
    public static class Patch_Inventory_Update
    {
        static void Postfix(Inventory __instance)
        {
            if (__instance == null || !PlayerHelper.IsInGameWorld()) return;

            // 1. Delete key over hovered slot -> Send to Trash Slot (Feature 14)
            if (InputHelper.WasKeyPressed(KeyCode.Delete))
            {
                if (Inventory.hoverSlot != null && !Inventory.hoverSlot.IsEmpty)
                {
                    TrashSlotManager.TrashSlot(Inventory.hoverSlot);
                }
                else if (TrashSlotManager.HasTrashedItem)
                {
                    TrashSlotManager.UndoTrash();
                }
            }

            // 2. Hotkey: Sort Inventory (Feature 3)
            if (Plugin.KeySort != null && InputHelper.WasKeyPressed(Plugin.KeySort.Value))
            {
                InventorySorter.SortCurrentInventory();
            }

            // 3. Hotkey: Storage Dump (Feature 12)
            if (Plugin.KeyStorageDump != null && InputHelper.WasKeyPressed(Plugin.KeyStorageDump.Value))
            {
                StorageDumpManager.DumpBackpackToOpenStorage();
            }
        }
    }
    // ============================================================================
    // [END] PATCH: INVENTORY SHORTCUTS & ACTION HOOKS
    // ============================================================================
    #endregion
}
