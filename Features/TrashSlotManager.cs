using System;
using UnityEngine;
using InventoryMaster.Helpers;

namespace InventoryMaster.Features
{
    #region [START] MODULE: TRASH SLOT WITH SAFE UNDO (FEATURE 14)
    // ============================================================================
    // [START] MODULE: TRASH SLOT WITH SAFE UNDO (FEATURE 14)
    // Purpose: Safely trashes unwanted items with an instant 1-step undo buffer
    //          to prevent accidental permanent item loss.
    // ============================================================================
    public static class TrashSlotManager
    {
        private class TrashedItemData
        {
            public Item_Base BaseItem;
            public int Amount;
            public int Uses;
        }

        private static TrashedItemData _lastTrashed = null;

        public static bool HasTrashedItem => _lastTrashed != null;

        public static void TrashSlot(Slot slot)
        {
            if (slot == null || slot.IsEmpty || !slot.HasValidItemInstance()) return;

            if (FavoriteLockManager.IsLocked(slot))
            {
                ToastManager.Show("🔒 Cannot trash locked item! Unlock it first.");
                return;
            }

            var baseItem = slot.GetItemBase();
            if (baseItem == null) return;

            _lastTrashed = new TrashedItemData
            {
                BaseItem = baseItem,
                Amount = slot.itemInstance.Amount,
                Uses = slot.itemInstance.Uses
            };

            string name = baseItem.UniqueName;
            int count = slot.itemInstance.Amount;

            slot.Reset();
            slot.RefreshComponents();

            ToastManager.Show($"🗑️ Trashed {count}x {name} (Press Undo to recover)");
        }

        public static void UndoTrash()
        {
            if (_lastTrashed == null)
            {
                ToastManager.Show("⚠️ No recently trashed item to restore.");
                return;
            }

            var inv = PlayerHelper.GetPlayerInventory();
            if (inv == null) return;

            int remaining = inv.AddItem(_lastTrashed.BaseItem.UniqueName, _lastTrashed.Amount);
            if (remaining > 0)
            {
                ToastManager.Show($"⚠️ Inventory full! Could only recover {_lastTrashed.Amount - remaining} items.");
            }
            else
            {
                ToastManager.Show($"↩️ Restored {_lastTrashed.Amount}x {_lastTrashed.BaseItem.UniqueName}!");
                _lastTrashed = null;
            }
        }
    }
    // ============================================================================
    // [END] MODULE: TRASH SLOT WITH SAFE UNDO (FEATURE 14)
    // ============================================================================
    #endregion
}
