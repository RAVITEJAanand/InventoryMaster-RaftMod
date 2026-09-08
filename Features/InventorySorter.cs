using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InventoryMaster.Helpers;

namespace InventoryMaster.Features
{
    #region [START] MODULE: AUTO SORT INVENTORY (FEATURE 3)
    // ============================================================================
    // [START] MODULE: AUTO SORT INVENTORY (FEATURE 3)
    // Purpose: Automatically stacks partial stacks and neatly reorganizes inventory
    //          and chest slots by logical category and item name, protecting locked slots.
    // ============================================================================
    public static class InventorySorter
    {
        private class SlotItemData
        {
            public Item_Base BaseItem;
            public int Amount;
            public int Uses;
            public ItemCategory Category;
            public string Name;
        }

        public static void SortCurrentInventory()
        {
            var player = PlayerHelper.GetLocalPlayer();
            if (player == null || player.Inventory == null)
            {
                ToastManager.Show("⚠️ Enter a game world to sort inventory!");
                return;
            }

            var playerInv = player.Inventory;
            bool chestOpen = playerInv.secondInventory != null && playerInv.secondInventory.allSlots != null && playerInv.secondInventory.allSlots.Count > 0;

            // Check if a secondary chest inventory is currently open
            if (chestOpen)
            {
                SortInventorySlots(playerInv.secondInventory.allSlots, "Chest", false);
            }

            // Sort player carried inventory slots (all non-hotbar, non-equipment slots)
            var carriedSlots = PlayerHelper.GetPlayerInventorySlots(playerInv);
            SortInventorySlots(carriedSlots, "Backpack", chestOpen);
        }

        public static void SortInventorySlots(List<Slot> slots, string inventoryName, bool suppressEmptyToast = false)
        {
            if (slots == null || slots.Count == 0) return;

            // 1. First Pass: Merge partial stacks across non-locked slots
            MergePartialStacks(slots);

            // 2. Second Pass: Collect unlocked slots and their items
            var targetSlots = new List<Slot>();
            var itemsToSort = new List<SlotItemData>();

            foreach (var slot in slots)
            {
                if (slot == null) continue;
                if (FavoriteLockManager.IsLocked(slot)) continue; // Never move locked slots!

                targetSlots.Add(slot);

                if (!slot.IsEmpty && slot.HasValidItemInstance())
                {
                    var baseItem = slot.GetItemBase();
                    if (baseItem != null)
                    {
                        itemsToSort.Add(new SlotItemData
                        {
                            BaseItem = baseItem,
                            Amount = slot.itemInstance.Amount,
                            Uses = slot.itemInstance.Uses,
                            Category = ItemCategoryHelper.GetCategory(baseItem),
                            Name = baseItem.UniqueName ?? ""
                        });
                    }
                }
            }

            if (itemsToSort.Count == 0)
            {
                if (!suppressEmptyToast)
                {
                    ToastManager.Show($"✨ {inventoryName} is already empty or sorted.");
                }
                return;
            }

            // 3. Sort collected items: Category -> Name -> Amount descending
            var sortedItems = itemsToSort
                .OrderBy(x => (int)x.Category)
                .ThenBy(x => x.Name)
                .ThenByDescending(x => x.Amount)
                .ToList();

            // 4. Clear the target slots
            foreach (var slot in targetSlots)
            {
                slot.Reset();
            }

            // 5. Place sorted items back into target slots
            for (int i = 0; i < sortedItems.Count && i < targetSlots.Count; i++)
            {
                var itemData = sortedItems[i];
                var slot = targetSlots[i];

                slot.SetItem(itemData.BaseItem, itemData.Amount);
                if (slot.itemInstance != null && itemData.Uses > 0)
                {
                    slot.itemInstance.Uses = itemData.Uses;
                }
                slot.RefreshComponents();
            }

            // Refresh all remaining empty slots
            for (int i = sortedItems.Count; i < targetSlots.Count; i++)
            {
                targetSlots[i].RefreshComponents();
            }

            ToastManager.Show($"✨ {inventoryName} sorted by categories!");
        }

        private static void MergePartialStacks(List<Slot> slots)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var s1 = slots[i];
                if (s1 == null || s1.IsEmpty || !s1.HasValidItemInstance()) continue;
                if (FavoriteLockManager.IsLocked(s1)) continue;

                var baseItem = s1.GetItemBase();
                if (baseItem == null) continue;
                int maxStack = baseItem.settings_Inventory != null ? baseItem.settings_Inventory.StackSize : 20;
                if (maxStack <= 1) continue; // Single tools don't stack

                for (int j = i + 1; j < slots.Count; j++)
                {
                    var s2 = slots[j];
                    if (s2 == null || s2.IsEmpty || !s2.HasValidItemInstance()) continue;
                    if (FavoriteLockManager.IsLocked(s2)) continue;

                    if (s2.itemInstance.baseItem.UniqueIndex == baseItem.UniqueIndex)
                    {
                        int space = maxStack - s1.itemInstance.Amount;
                        if (space > 0)
                        {
                            int transfer = Mathf.Min(space, s2.itemInstance.Amount);
                            s1.itemInstance.Amount += transfer;
                            s2.itemInstance.Amount -= transfer;

                            if (s2.itemInstance.Amount <= 0)
                            {
                                s2.Reset();
                            }
                            s1.RefreshComponents();
                            s2.RefreshComponents();
                        }
                    }
                    if (s1.itemInstance.Amount >= maxStack) break;
                }
            }
        }
    }
    // ============================================================================
    // [END] MODULE: AUTO SORT INVENTORY (FEATURE 3)
    // ============================================================================
    #endregion
}
