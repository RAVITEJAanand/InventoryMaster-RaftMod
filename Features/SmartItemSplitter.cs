using System;
using UnityEngine;
using UnityEngine.EventSystems;
using InventoryMaster.Helpers;

namespace InventoryMaster.Features
{
    #region [START] MODULE: SMART ITEM SPLIT (FEATURE 9)
    // ============================================================================
    // [START] MODULE: SMART ITEM SPLIT (FEATURE 9)
    // Purpose: Enables quick stack splitting using modifier keys:
    //          - Shift + Right Click = Takes 1 item into next empty slot
    //          - Ctrl + Right Click  = Takes half stack into next empty slot
    // ============================================================================
    public static class SmartItemSplitter
    {
        public static bool HandleSlotSplit(Slot slot, PointerEventData eventData)
        {
            if (Plugin.EnableSmartSplit == null || !Plugin.EnableSmartSplit.Value) return false;
            if (slot == null || slot.IsEmpty || !slot.HasValidItemInstance()) return false;
            if (slot.itemInstance.Amount <= 1) return false;
            if (FavoriteLockManager.IsLocked(slot)) return false; // Never split a locked stack into an unlocked slot!

            // Only trigger on Right Click with Shift or Ctrl held
            if (eventData.button != PointerEventData.InputButton.Right) return false;

            bool isShift = InputHelper.IsShiftHeld();
            bool isCtrl = InputHelper.IsCtrlHeld();

            if (!isShift && !isCtrl) return false;

            var inv = slot.inventory;
            if (inv == null || inv.allSlots == null) return false;

            Slot emptySlot = null;
            foreach (var s in inv.allSlots)
            {
                if (s != null && s.IsEmpty)
                {
                    emptySlot = s;
                    break;
                }
            }

            if (emptySlot == null)
            {
                ToastManager.Show("⚠️ No empty slot available to split into!");
                return true;
            }

            int splitAmount = 1;
            if (isCtrl)
            {
                splitAmount = Mathf.FloorToInt(slot.itemInstance.Amount / 2f);
            }

            if (splitAmount <= 0) splitAmount = 1;

            var baseItem = slot.GetItemBase();
            if (baseItem == null) return false;

            slot.itemInstance.Amount -= splitAmount;
            emptySlot.SetItem(baseItem, splitAmount);

            if (slot.itemInstance.Amount <= 0)
            {
                slot.Reset();
            }

            slot.RefreshComponents();
            emptySlot.RefreshComponents();

            ToastManager.Show($"✂️ Split {splitAmount}x {baseItem.UniqueName}");
            return true; // Event consumed
        }
    }
    // ============================================================================
    // [END] MODULE: SMART ITEM SPLIT (FEATURE 9)
    // ============================================================================
    #endregion
}
