using System;
using UnityEngine;
using InventoryMaster.Helpers;

namespace InventoryMaster.Features
{
    #region [START] MODULE: INVENTORY EXPANSION (FEATURE 5)
    // ============================================================================
    // [START] MODULE: INVENTORY EXPANSION (FEATURE 5)
    // Purpose: Automatically unlocks and maintains full backpack slots without
    //          requiring wearing or consuming backpack items.
    // ============================================================================
    public static class InventoryExpansionManager
    {
        private static float _lastCheckTime = 0f;

        public static void UpdateInventoryExpansion()
        {
            if (Plugin.EnableInventoryExpansion == null || !Plugin.EnableInventoryExpansion.Value) return;

            if (Time.unscaledTime - _lastCheckTime < 2.0f) return;
            _lastCheckTime = Time.unscaledTime;

            var inv = PlayerHelper.GetPlayerInventory();
            if (inv == null || inv.backpackSlots == null || inv.backpackSlots.Count == 0) return;

            // Ensure all backpack slots are activated
            try
            {
                inv.SetBackpackActiveSlots(inv.backpackSlots.Count);
            }
            catch { }
        }
    }
    // ============================================================================
    // [END] MODULE: INVENTORY EXPANSION (FEATURE 5)
    // ============================================================================
    #endregion
}
