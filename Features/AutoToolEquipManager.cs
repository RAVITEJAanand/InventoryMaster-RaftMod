using System;
using System.Collections.Generic;
using UnityEngine;
using InventoryMaster.Helpers;

namespace InventoryMaster.Features
{
    #region [START] MODULE: AUTO EQUIP TOOLS & AUTO-REPLACE (FEATURE 10 - OPTIMIZED)
    // ============================================================================
    // [START] MODULE: AUTO EQUIP TOOLS & AUTO-REPLACE (FEATURE 10 - OPTIMIZED)
    // Purpose: Throttled raycast detection of resources on aim with cached camera,
    //          and instant auto-replacement of tools that shatter from zero durability.
    // ============================================================================
    public static class AutoToolEquipManager
    {
        private static float _lastRaycastTime = 0f;
        private static string _lastEquippedToolType = "";
        private static Camera _cachedCamera = null;

        public static void UpdateAutoToolEquip()
        {
            if (Plugin.EnableAutoToolEquip == null || !Plugin.EnableAutoToolEquip.Value) return;

            if (Time.unscaledTime - _lastRaycastTime < 0.35f) return;
            _lastRaycastTime = Time.unscaledTime;

            var player = PlayerHelper.GetLocalPlayer();
            if (player == null) return;

            if (_cachedCamera == null)
            {
                _cachedCamera = Camera.main;
                if (_cachedCamera == null) return;
            }

            var inv = player.Inventory;
            if (inv == null || inv.hotbar == null) return;

            // Raycast forward from camera, reaching up to 15m (hook distance) and hitting triggers
            Ray ray = new Ray(_cachedCamera.transform.position, _cachedCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 15.0f, ~0, QueryTriggerInteraction.Collide))
            {
                var hitCol = hit.collider;
                if (hitCol == null) return;
                var hitGO = hitCol.gameObject;
                if (hitGO == null) return;

                string goName = hitGO.name;
                string desiredToolKeyword = null;

                // 1. Axe Context (Trees, Palms, Wood)
                if (hitGO.GetComponentInParent<HarvestableTree>() != null ||
                    goName.IndexOf("tree", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    goName.IndexOf("palm", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    goName.IndexOf("pine", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    goName.IndexOf("birch", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    goName.IndexOf("wood", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    goName.IndexOf("trunk", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    desiredToolKeyword = "axe";
                }
                // 2. Hook Context (Ocean flotsam, debris, floating crates/barrels, reef nodes)
                else if (IsEligibleDebrisOrReef(hitGO, goName))
                {
                    desiredToolKeyword = "hook";
                }
                // 3. Spear / Weapon Context (Predators, sharks, boars, bears, hostile entities)
                else if (hitGO.GetComponentInParent<AI_NetworkBehaviour>() != null ||
                         goName.IndexOf("shark", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("bear", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("boar", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("bird", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("screecher", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("lurker", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("hyena", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("rat", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    desiredToolKeyword = "spear";
                }

                if (!string.IsNullOrEmpty(desiredToolKeyword) && desiredToolKeyword != _lastEquippedToolType)
                {
                    // Check if current active slot already holds this tool
                    var curSlot = inv.GetSelectedHotbarSlot();
                    if (curSlot != null && !curSlot.IsEmpty && curSlot.GetItemBase() != null)
                    {
                        if (curSlot.GetItemBase().UniqueName.IndexOf(desiredToolKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            _lastEquippedToolType = desiredToolKeyword;
                            return;
                        }
                    }

                    // Look for desired tool in hotbar slots (0 to hotslotCount-1)
                    int hotCount = Mathf.Min(inv.hotslotCount, 10);
                    for (int i = 0; i < hotCount; i++)
                    {
                        var slot = inv.GetSlot(i);
                        if (slot != null && !slot.IsEmpty && slot.GetItemBase() != null)
                        {
                            if (slot.GetItemBase().UniqueName.IndexOf(desiredToolKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                inv.hotbar.SetSelectedSlotIndex(i);
                                _lastEquippedToolType = desiredToolKeyword;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Automatically replaces a broken tool in the hotbar with an identical or compatible
        /// replacement tool found in the player's carried inventory.
        /// </summary>
        public static void TryAutoReplaceBrokenTool(PlayerInventory playerInv, Slot hotSlot, Item_Base brokenItem)
        {
            if (Plugin.EnableAutoToolEquip == null || !Plugin.EnableAutoToolEquip.Value) return;
            if (playerInv == null || hotSlot == null || brokenItem == null) return;

            var carriedSlots = PlayerHelper.GetPlayerInventorySlots(playerInv);
            Slot replacementSlot = null;

            // 1. Priority: Find identical tool (same UniqueIndex)
            foreach (var s in carriedSlots)
            {
                if (s == null || s.IsEmpty || !s.HasValidItemInstance()) continue;
                if (FavoriteLockManager.IsLocked(s)) continue; // Never auto-consume locked slots

                if (s.itemInstance.baseItem.UniqueIndex == brokenItem.UniqueIndex)
                {
                    replacementSlot = s;
                    break;
                }
            }

            // 2. Secondary fallback: Find same tool category (e.g. any axe, hook, or spear)
            if (replacementSlot == null)
            {
                string brokenName = brokenItem.UniqueName;
                string keyword = null;
                if (brokenName.IndexOf("axe", StringComparison.OrdinalIgnoreCase) >= 0) keyword = "axe";
                else if (brokenName.IndexOf("hook", StringComparison.OrdinalIgnoreCase) >= 0) keyword = "hook";
                else if (brokenName.IndexOf("spear", StringComparison.OrdinalIgnoreCase) >= 0) keyword = "spear";
                else if (brokenName.IndexOf("bow", StringComparison.OrdinalIgnoreCase) >= 0) keyword = "bow";

                if (!string.IsNullOrEmpty(keyword))
                {
                    foreach (var s in carriedSlots)
                    {
                        if (s == null || s.IsEmpty || !s.HasValidItemInstance()) continue;
                        if (FavoriteLockManager.IsLocked(s)) continue;

                        if (s.itemInstance.baseItem.UniqueName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            replacementSlot = s;
                            break;
                        }
                    }
                }
            }

            if (replacementSlot != null && replacementSlot.itemInstance != null)
            {
                var replBase = replacementSlot.GetItemBase();
                int amount = replacementSlot.itemInstance.Amount;
                int uses = replacementSlot.itemInstance.Uses;

                hotSlot.SetItem(replBase, amount);
                if (hotSlot.itemInstance != null && uses > 0)
                {
                    hotSlot.itemInstance.Uses = uses;
                }

                replacementSlot.Reset();

                hotSlot.RefreshComponents();
                replacementSlot.RefreshComponents();

                // NOTE: Do NOT call hotbar.ReselectCurrentSlot() here.
                // That triggers UseItemController.Deselect() → ThrowableComponent.OnDeSelect()
                // → ChargeMeter.Reset() which crashes if player holds a throwable when tool breaks.

                ToastManager.Show($"🔄 Auto-equipped replacement {replBase.UniqueName}!");
            }
        }

        private static bool IsEligibleDebrisOrReef(GameObject hitGO, string goName)
        {
            var pickup = hitGO.GetComponentInParent<PickupItem>();
            if (pickup != null)
            {
                if (pickup is ItemNet || pickup.pickupItemType != PickupItemType.Default) return false;
                if (pickup.GetComponentInParent<Block>() != null) return false;
                if (pickup.GetComponent<ItemCollector>() != null) return false;
                if (pickup.isDropped) return false;
                return true;
            }

            return goName.IndexOf("flotsam", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   goName.IndexOf("barrel", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   goName.IndexOf("crate", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   goName.IndexOf("plank", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   goName.IndexOf("debris", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   goName.IndexOf("plastic", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   goName.IndexOf("reef", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   goName.IndexOf("clay", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   goName.IndexOf("sand", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   goName.IndexOf("scrap", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   goName.IndexOf("ore", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
    // ============================================================================
    // [END] MODULE: AUTO EQUIP TOOLS & AUTO-REPLACE (FEATURE 10 - OPTIMIZED)
    // ============================================================================
    #endregion
}
