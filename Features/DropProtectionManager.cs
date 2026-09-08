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
        public static bool ShouldBlockDrop(Slot slot)
        {
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
