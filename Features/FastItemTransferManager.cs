using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using InventoryMaster.Helpers;

namespace InventoryMaster.Features
{
    #region [START] MODULE: FAST ITEM TRANSFER (FEATURE 11)
    // ============================================================================
    // [START] MODULE: FAST ITEM TRANSFER (FEATURE 11)
    // Purpose: Enables ultra-fast bulk transfer between player and open storage
    //          when double-clicking an item.
    // ============================================================================
    public static class FastItemTransferManager
    {
        public static bool HandleFastTransfer(Slot clickedSlot, PointerEventData eventData)
        {
            if (Plugin.EnableFastItemTransfer == null || !Plugin.EnableFastItemTransfer.Value) return false;
            if (clickedSlot == null || clickedSlot.IsEmpty || !clickedSlot.HasValidItemInstance()) return false;
            if (eventData.button != PointerEventData.InputButton.Left) return false;

            // Check if double click
            if (eventData.clickCount < 2) return false;

            var playerInv = PlayerHelper.GetPlayerInventory();
            if (playerInv == null || playerInv.secondInventory == null) return false; // Only when chest is open

            var sourceInv = clickedSlot.inventory;
            var targetInv = sourceInv == playerInv ? playerInv.secondInventory : playerInv;
            if (sourceInv == null || targetInv == null) return false;

            var baseItem = clickedSlot.GetItemBase();
            if (baseItem == null) return false;

            int totalTransferred = 0;
            var sourceSlots = sourceInv == playerInv 
                ? PlayerHelper.GetPlayerInventorySlots(playerInv) 
                : sourceInv.allSlots;

            foreach (var s in sourceSlots)
            {
                if (s == null || s.IsEmpty || !s.HasValidItemInstance()) continue;
                if (FavoriteLockManager.IsLocked(s)) continue; // Never move locked items!
                if (s.itemInstance.baseItem.UniqueIndex != baseItem.UniqueIndex) continue;

                int amount = s.itemInstance.Amount;

                // If the target is the player's own inventory and it's full, native AddItem()
                // auto-drops the overflow on the ground instead of losing it. Suppress Drop
                // Protection for that internal call so a protected item isn't silently cancelled
                // mid-transfer and lost (it would already be deducted from the source slot below).
                bool targetIsPlayerInv = targetInv == playerInv;
                int remaining;
                if (targetIsPlayerInv) DropProtectionManager.SuppressForInternalTransfer = true;
                try
                {
                    remaining = targetInv.AddItem(baseItem.UniqueName, amount);
                }
                finally
                {
                    if (targetIsPlayerInv) DropProtectionManager.SuppressForInternalTransfer = false;
                }
                int transferred = amount - remaining;

                if (transferred > 0)
                {
                    totalTransferred += transferred;
                    s.itemInstance.Amount -= transferred;
                    if (s.itemInstance.Amount <= 0)
                    {
                        s.Reset();
                    }
                    s.RefreshComponents();
                }

                if (remaining > 0)
                {
                    // Target inventory full
                    break;
                }
            }

            if (totalTransferred > 0)
            {
                if (targetInv.allSlots != null)
                {
                    foreach (var ts in targetInv.allSlots)
                    {
                        if (ts != null) ts.RefreshComponents();
                    }
                }
                string targetName = targetInv == playerInv ? "Backpack" : "Chest";
                ToastManager.Show($"⚡ Transferred {totalTransferred}x {baseItem.UniqueName} to {targetName}");
                return true; // Consumed
            }

            return false;
        }
    }
    // ============================================================================
    // [END] MODULE: FAST ITEM TRANSFER (FEATURE 11)
    // ============================================================================
    #endregion
}
