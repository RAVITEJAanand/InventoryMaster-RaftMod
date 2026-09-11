using System;
using UnityEngine;
using InventoryMaster.Helpers;

namespace InventoryMaster.Features
{
    #region [START] MODULE: HOTBAR EXPANSION & ROW SWAP (FEATURE 6)
    // ============================================================================
    // [START] MODULE: HOTBAR EXPANSION & ROW SWAP (FEATURE 6)
    // Purpose: Instantly swaps Hotbar slots with the first row of the backpack,
    //          granting immediate access to 20 hotbar-ready items on the fly.
    // ============================================================================
    public static class HotbarExpansionManager
    {
        private static bool _isSecondaryRow = false;

        public static void SwapHotbarRow()
        {
            var inv = PlayerHelper.GetPlayerInventory();
            if (inv == null || inv.allSlots == null || inv.backpackSlots == null)
            {
                ToastManager.Show("⚠️ Cannot swap hotbar outside of game world.");
                return;
            }

            int hotCount = Mathf.Min(inv.hotslotCount, 10);
            int backCount = inv.backpackSlots.Count;
            if (hotCount <= 0 || backCount <= 0) return;

            int countToSwap = Mathf.Min(hotCount, backCount);

            for (int i = 0; i < countToSwap; i++)
            {
                Slot hotSlot = inv.GetSlot(i);
                Slot backSlot = inv.backpackSlots[i];

                if (hotSlot == null || backSlot == null) continue;

                // Never move locked/favorited items out of their slot.
                if (FavoriteLockManager.IsLocked(hotSlot) || FavoriteLockManager.IsLocked(backSlot)) continue;

                // Temporary copy of hotSlot item
                var hotItem = hotSlot.itemInstance;
                var backItem = backSlot.itemInstance;

                // Swap
                if (backItem != null)
                {
                    hotSlot.SetItem(backItem);
                }
                else
                {
                    hotSlot.Reset();
                }

                if (hotItem != null)
                {
                    backSlot.SetItem(hotItem);
                }
                else
                {
                    backSlot.Reset();
                }

                hotSlot.RefreshComponents();
                backSlot.RefreshComponents();
            }

            _isSecondaryRow = !_isSecondaryRow;
            string rowName = _isSecondaryRow ? "Backpack Row (Secondary)" : "Primary Hotbar";
            ToastManager.Show($"🔄 Active Hotbar: {rowName}");
        }
    }
    // ============================================================================
    // [END] MODULE: HOTBAR EXPANSION & ROW SWAP (FEATURE 6)
    // ============================================================================
    #endregion
}
