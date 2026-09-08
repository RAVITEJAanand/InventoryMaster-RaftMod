using System;
using HarmonyLib;
using InventoryMaster.Features;

namespace InventoryMaster.Patches
{
    #region [START] PATCH: DROP PROTECTION (FEATURE 8)
    // ============================================================================
    // [START] PATCH: DROP PROTECTION (FEATURE 8)
    // Description: Blocks accidental drops when item is favorited/locked or high-tier.
    // ============================================================================
    [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.DropItem), typeof(Slot))]
    public static class Patch_PlayerInventory_DropItem_Slot
    {
        static bool Prefix(Slot slot)
        {
            if (DropProtectionManager.ShouldBlockDrop(slot))
            {
                return false; // Cancel drop
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.DropItem), typeof(Item_Base), typeof(int))]
    public static class Patch_PlayerInventory_DropItem_Base
    {
        static bool Prefix(Item_Base item)
        {
            if (DropProtectionManager.ShouldBlockDrop(item))
            {
                return false; // Cancel drop
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.DropCurrentItem))]
    public static class Patch_PlayerInventory_DropCurrentItem
    {
        static bool Prefix(PlayerInventory __instance)
        {
            if (__instance != null)
            {
                var slot = __instance.GetSelectedHotbarSlot();
                if (DropProtectionManager.ShouldBlockDrop(slot))
                {
                    return false; // Cancel drop
                }
            }
            return true;
        }
    }
    // ============================================================================
    // [END] PATCH: DROP PROTECTION (FEATURE 8)
    // ============================================================================
    #endregion
}
