using System;
using System.Collections.Generic;
using UnityEngine;
using InventoryMaster.Helpers;

namespace InventoryMaster.Features
{
    #region [START] MODULE: FAVORITE ITEMS LOCK (FEATURE 13)
    // ============================================================================
    // [START] MODULE: FAVORITE ITEMS LOCK (FEATURE 13)
    // Purpose: Allows locking specific slots/items to prevent accidental dropping,
    //          sorting, quick-stacking, or storage dumping.
    // ============================================================================
    public static class FavoriteLockManager
    {
        // Tracks locked slots using an internal hashset of Slot instance IDs and indices
        private static readonly HashSet<int> _lockedSlotIds = new HashSet<int>();
        private static readonly HashSet<string> _lockedItemNames = new HashSet<string>();

        public static bool HasAnyLocks => _lockedSlotIds.Count > 0;

        public static bool IsLocked(Slot slot)
        {
            if (slot == null || _lockedSlotIds.Count == 0) return false;
            return _lockedSlotIds.Contains(slot.GetInstanceID());
        }

        public static void SetLocked(Slot slot, bool locked)
        {
            if (slot == null) return;
            int id = slot.GetInstanceID();
            if (locked)
            {
                _lockedSlotIds.Add(id);
                slot.locked = true;
            }
            else
            {
                _lockedSlotIds.Remove(id);
                slot.locked = false;
            }
            slot.RefreshComponents();
        }

        public static bool ToggleLock(Slot slot)
        {
            if (slot == null || slot.IsEmpty) return false;

            bool nowLocked = !IsLocked(slot);
            SetLocked(slot, nowLocked);

            string itemName = slot.GetItemBase()?.UniqueName ?? "Item";
            if (nowLocked)
            {
                ToastManager.Show($"🔒 Locked {itemName} (Protected from Sort/Dump/Drop)");
            }
            else
            {
                ToastManager.Show($"🔓 Unlocked {itemName}");
            }

            return nowLocked;
        }

        public static void ClearAll()
        {
            _lockedSlotIds.Clear();
        }
    }
    // ============================================================================
    // [END] MODULE: FAVORITE ITEMS LOCK (FEATURE 13)
    // ============================================================================
    #endregion
}
