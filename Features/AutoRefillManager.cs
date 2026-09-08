using System;
using UnityEngine;
using InventoryMaster.Helpers;

namespace InventoryMaster.Features
{
    #region [START] MODULE: AUTO REFILL WATER & FOOD (FEATURE 15 - OPTIMIZED)
    // ============================================================================
    // [START] MODULE: AUTO REFILL WATER & FOOD (FEATURE 15 - OPTIMIZED)
    // Purpose: Throttled 1Hz vital stats monitoring with zero frame-by-frame overhead
    //          even when inventory contains no food or drinks.
    // ============================================================================
    public static class AutoRefillManager
    {
        private static float _lastCheckTime = 0f;
        private static float _lastConsumeTime = 0f;
        private const float CHECK_INTERVAL = 1.0f;   // Check at most 1 time per second (1Hz)
        private const float REFILL_COOLDOWN = 6.0f;  // 6s cooldown between actual consumptions

        public static void UpdateAutoRefill()
        {
            if (Plugin.EnableAutoRefill == null || !Plugin.EnableAutoRefill.Value) return;

            // Throttle to 1Hz check rate - zero frame-by-frame CPU waste
            if (Time.unscaledTime - _lastCheckTime < CHECK_INTERVAL) return;
            _lastCheckTime = Time.unscaledTime;

            // Enforce consumption cooldown
            if (Time.unscaledTime - _lastConsumeTime < REFILL_COOLDOWN) return;

            var player = PlayerHelper.GetLocalPlayer();
            if (player == null || player.Stats == null || player.Inventory == null) return;

            float threshold = Plugin.AutoRefillThreshold != null ? Plugin.AutoRefillThreshold.Value : 0.25f;

            var stats = player.Stats;
            float thirstNorm = stats.stat_thirst != null ? stats.stat_thirst.Normal.Value / 100f : 1f;
            float hungerNorm = stats.stat_hunger != null ? stats.stat_hunger.Normal.Value / 100f : 1f;

            // 1. Thirst check
            if (thirstNorm < threshold)
            {
                if (TryConsumeDrink(player))
                {
                    _lastConsumeTime = Time.unscaledTime;
                    return;
                }
            }

            // 2. Hunger check
            if (hungerNorm < threshold)
            {
                if (TryConsumeFood(player))
                {
                    _lastConsumeTime = Time.unscaledTime;
                    return;
                }
            }
        }

        private static bool TryConsumeDrink(Network_Player player)
        {
            var inv = player.Inventory;
            if (inv.allSlots == null) return false;

            foreach (var slot in inv.allSlots)
            {
                if (slot == null || slot.IsEmpty || !slot.HasValidItemInstance()) continue;
                if (FavoriteLockManager.IsLocked(slot)) continue;

                var baseItem = slot.GetItemBase();
                if (baseItem == null) continue;

                string name = baseItem.UniqueName;
                if (string.IsNullOrEmpty(name)) continue;

                // Check for fresh water / drinks; strictly avoid saltwater
                if ((name.IndexOf("water_fresh", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("bottle_water", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("coconut", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("smoothie", StringComparison.OrdinalIgnoreCase) >= 0) &&
                    name.IndexOf("salt", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    player.Stats.Consume(baseItem);
                    slot.RemoveItem(1);
                    slot.RefreshComponents();
                    ToastManager.Show($"💧 Auto-drank {baseItem.UniqueName}!");
                    return true;
                }
            }
            return false;
        }

        private static bool TryConsumeFood(Network_Player player)
        {
            var inv = player.Inventory;
            if (inv.allSlots == null) return false;

            foreach (var slot in inv.allSlots)
            {
                if (slot == null || slot.IsEmpty || !slot.HasValidItemInstance()) continue;
                if (FavoriteLockManager.IsLocked(slot)) continue;

                var baseItem = slot.GetItemBase();
                if (baseItem == null) continue;

                string name = baseItem.UniqueName;
                if (string.IsNullOrEmpty(name)) continue;

                // Consume cooked food, fruits, or meals; strictly exclude raw poisonous items
                if ((name.IndexOf("cooked", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("stew", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("soup", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("pie", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("berry", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("mango", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("banana", StringComparison.OrdinalIgnoreCase) >= 0) &&
                    name.IndexOf("poison", StringComparison.OrdinalIgnoreCase) < 0 &&
                    name.IndexOf("puffer", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    player.Stats.Consume(baseItem);
                    slot.RemoveItem(1);
                    slot.RefreshComponents();
                    ToastManager.Show($"🍖 Auto-ate {baseItem.UniqueName}!");
                    return true;
                }
            }
            return false;
        }
    }
    // ============================================================================
    // [END] MODULE: AUTO REFILL WATER & FOOD (FEATURE 15 - OPTIMIZED)
    // ============================================================================
    #endregion
}
