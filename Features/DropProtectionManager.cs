using System;
using UnityEngine;
using InventoryMaster.Helpers;

namespace InventoryMaster.Features
{
    #region [START] MODULE: DROP PROTECTION (FEATURE 8)
    // ============================================================================
    // [START] MODULE: DROP PROTECTION (FEATURE 8)
    // Purpose: Prevents accidental dropping of important, locked, or high-tier items.
    // ============================================================================
    public static class DropProtectionManager
    {
        // When the player's own inventory is full, Raft's native Inventory.AddItem() silently
        // falls back to PlayerInventory.DropItem(Item_Base,int) to place the overflow on the
        // ground instead of losing it. That exact method is the one this class patches. If a mod
        // feature (Undo Trash, Fast Transfer) relies on that native overflow-to-ground fallback
        // and this manager blocks the drop for a protected category, the item vanishes entirely:
        // it never made it into the inventory (no suitable slot) and the drop got cancelled. Mod
        // features that depend on that fallback must set this flag around the call so the safety
        // net still works, without weakening protection against the player's own manual Q drops.
        public static bool SuppressForInternalTransfer = false;

        public static bool ShouldBlockDrop(Slot slot)
        {
            if (SuppressForInternalTransfer) return false;

            if (Plugin.EnableDropProtection == null || !Plugin.EnableDropProtection.Value)
            {
                return false;
            }

            // If player is intentionally holding Shift, allow drop override
            if (InputHelper.IsShiftHeld())
            {
                return false;
            }

            if (slot != null && FavoriteLockManager.IsLocked(slot))
            {
                ToastManager.Show("🔒 Item is locked! Hold Shift+Q to drop.");
                return true;
            }

            if (slot != null && !slot.IsEmpty && slot.HasValidItemInstance())
            {
                var baseItem = slot.GetItemBase();
                if (baseItem != null && Plugin.ProtectToolsAndEquipment != null && Plugin.ProtectToolsAndEquipment.Value)
                {
                    var cat = ItemCategoryHelper.GetCategory(baseItem);
                    if (cat == ItemCategory.ToolsAndWeapons || cat == ItemCategory.EquipmentAndArmor)
                    {
                        ToastManager.Show($"🔒 Protected {baseItem.UniqueName}! Hold Shift+Q to drop.");
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool ShouldBlockDrop(Item_Base item)
        {
            if (SuppressForInternalTransfer) return false;
            if (Plugin.EnableDropProtection == null || !Plugin.EnableDropProtection.Value) return false;
            if (InputHelper.IsShiftHeld()) return false;

            if (item != null && Plugin.ProtectToolsAndEquipment != null && Plugin.ProtectToolsAndEquipment.Value)
            {
                var cat = ItemCategoryHelper.GetCategory(item);
                if (cat == ItemCategory.ToolsAndWeapons || cat == ItemCategory.EquipmentAndArmor)
                {
                    ToastManager.Show($"🔒 Protected {item.UniqueName}! Hold Shift+Q to drop.");
                    return true;
                }
            }
            return false;
        }
    }
    // ============================================================================
    // [END] MODULE: DROP PROTECTION (FEATURE 8)
    // ============================================================================
    #endregion
}
