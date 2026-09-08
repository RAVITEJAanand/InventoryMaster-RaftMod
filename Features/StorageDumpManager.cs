using System;
using System.Collections.Generic;
using UnityEngine;
using InventoryMaster.Helpers;

namespace InventoryMaster.Features
{
    #region [START] MODULE: ONE-CLICK STORAGE DUMP (FEATURE 12)
    // ============================================================================
    // [START] MODULE: ONE-CLICK STORAGE DUMP (FEATURE 12)
    // Purpose: Deposits all non-locked backpack items into the currently open chest
    //          with a single keystroke or button press.
    // ============================================================================
    public static class StorageDumpManager
    {
        public static void DumpBackpackToOpenStorage()
        {
            var playerInv = PlayerHelper.GetPlayerInventory();
            if (playerInv == null)
            {
                ToastManager.Show("⚠️ Enter a game world to dump items.");
                return;
            }

            var chestInv = playerInv.secondInventory;
            if (chestInv == null || chestInv.allSlots == null)
            {
                ToastManager.Show("📦 Open a storage chest first to dump items!");
                return;
            }

            var backpackSlots = playerInv.backpackSlots != null && playerInv.backpackSlots.Count > 0
                ? playerInv.backpackSlots
                : playerInv.allSlots;

            int totalDumped = 0;

            foreach (var slot in backpackSlots)
            {
                if (slot == null || slot.IsEmpty || !slot.HasValidItemInstance()) continue;
                if (FavoriteLockManager.IsLocked(slot)) continue; // Strictly protect locked items!

                var baseItem = slot.GetItemBase();
                if (baseItem == null) continue;

                int initialAmount = slot.itemInstance.Amount;
                int remaining = chestInv.AddItem(baseItem.UniqueName, initialAmount);
                int transferred = initialAmount - remaining;

                if (transferred > 0)
                {
                    totalDumped += transferred;
                    slot.itemInstance.Amount -= transferred;
                    if (slot.itemInstance.Amount <= 0)
                    {
                        slot.Reset();
                    }
                    slot.RefreshComponents();
                }

                if (remaining > 0)
                {
                    // Chest is full
                    break;
                }
            }

            if (totalDumped > 0)
            {
                ToastManager.Show($"📥 Dumped {totalDumped} items into chest!");
            }
            else
            {
                ToastManager.Show("📦 Chest is full or no eligible items to dump.");
            }
        }
    }
    // ============================================================================
    // [END] MODULE: ONE-CLICK STORAGE DUMP (FEATURE 12)
    // ============================================================================
    #endregion
}
