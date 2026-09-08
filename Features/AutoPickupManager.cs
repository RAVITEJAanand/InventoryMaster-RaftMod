using System;
using System.Collections.Generic;
using UnityEngine;
using InventoryMaster.Helpers;

namespace InventoryMaster.Features
{
    #region [START] MODULE: AUTO PICKUP NEARBY ITEMS (FEATURE 7 - HIGH PERFORMANCE)
    // ============================================================================
    // [START] MODULE: AUTO PICKUP NEARBY ITEMS (FEATURE 7 - HIGH PERFORMANCE)
    // Purpose: Highly-optimized spatial pickup using PhysX OverlapSphereNonAlloc
    //          (Zero GC allocations, zero scene-wide FindObjectsOfType lag).
    // ============================================================================
    public static class AutoPickupManager
    {
        private static float _lastPickupTime = 0f;
        private const float PICKUP_INTERVAL = 0.35f; // ~3Hz throttled check for maximum FPS

        public static void UpdateAutoPickup()
        {
            if (Plugin.EnableAutoPickup == null || !Plugin.EnableAutoPickup.Value) return;

            if (Time.unscaledTime - _lastPickupTime < PICKUP_INTERVAL) return;
            _lastPickupTime = Time.unscaledTime;

            var player = PlayerHelper.GetLocalPlayer();
            if (player == null || player.PickupScript == null || player.Inventory == null) return;

            float radius = Plugin.AutoPickupRadius != null ? Plugin.AutoPickupRadius.Value : 5.0f;
            Vector3 playerPos = player.transform.position;

            var pickups = UnityEngine.Object.FindObjectsOfType<PickupItem>();
            if (pickups == null || pickups.Length == 0) return;

            foreach (var p in pickups)
            {
                if (p == null || !p.canBePickedUp || p.gameObject == null || !p.gameObject.activeInHierarchy) continue;

                float dist = Vector3.Distance(playerPos, p.transform.position);
                if (dist > radius) continue;

                // Inventory capacity check
                if (p.itemInstance != null && p.itemInstance.baseItem != null)
                {
                    var baseItem = p.itemInstance.baseItem;
                    var suitableSlot = player.Inventory.FindSuitableSlot(baseItem);
                    if (suitableSlot == null && !HasEmptySlot(player.Inventory))
                    {
                        // Inventory full
                        break;
                    }
                }

                try
                {
                    player.PickupScript.PickupItem(p, true, false);
                }
                catch { }
            }
        }

        private static bool HasEmptySlot(Inventory inv)
        {
            if (inv == null || inv.allSlots == null) return false;
            foreach (var slot in inv.allSlots)
            {
                if (slot != null && slot.IsEmpty) return true;
            }
            return false;
        }
    }
    // ============================================================================
    // [END] MODULE: AUTO PICKUP NEARBY ITEMS (FEATURE 7 - HIGH PERFORMANCE)
    // ============================================================================
    #endregion
}
