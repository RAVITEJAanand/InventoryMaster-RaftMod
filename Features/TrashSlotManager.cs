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

            // Rebuild the exact ItemInstance (including durability) that was trashed. The
            // string+amount overload of AddItem always creates a fresh instance at max Uses,
            // which would silently restore a broken/worn tool at full durability - use the
            // ItemInstance overload instead so Uses is preserved exactly as it was trashed.
            var restoreInstance = new ItemInstance(_lastTrashed.BaseItem, _lastTrashed.Amount, _lastTrashed.Uses);

            // If the backpack is full, native AddItem() auto-drops the overflow on the ground
            // instead of losing it. Suppress our own Drop Protection patch for that call so a
            // protected item (tool/weapon/armor) doesn't get silently cancelled mid-restore and
            // disappear entirely.
            DropProtectionManager.SuppressForInternalTransfer = true;
            try
            {
                inv.AddItem(restoreInstance, true);
            }
            finally
            {
                DropProtectionManager.SuppressForInternalTransfer = false;
            }

            // AddItem mutates restoreInstance in place: Amount/Uses are left at whatever
            // portion could not be placed (0 if everything was restored).
            if (restoreInstance.Amount > 0)
            {
                int recovered = _lastTrashed.Amount - restoreInstance.Amount;
                ToastManager.Show($"⚠️ Inventory full! Could only recover {recovered} items.");
                // Keep only the still-outstanding remainder so a second Undo press can't
                // re-add the portion that was already restored above (which would duplicate it).
                _lastTrashed.Amount = restoreInstance.Amount;
                _lastTrashed.Uses = restoreInstance.Uses;
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
