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

        // Pre-allocated non-alloc buffer to prevent garbage collection stutter
        private static readonly Collider[] _hitBuffer = new Collider[48];
        private static readonly HashSet<int> _processedThisTick = new HashSet<int>();

        public static void UpdateAutoPickup()
        {
            if (Plugin.EnableAutoPickup == null || !Plugin.EnableAutoPickup.Value) return;

            if (Time.unscaledTime - _lastPickupTime < PICKUP_INTERVAL) return;
            _lastPickupTime = Time.unscaledTime;

            var player = PlayerHelper.GetLocalPlayer();
            if (player == null || player.PickupScript == null || player.Inventory == null) return;

            float radius = Plugin.AutoPickupRadius != null ? Plugin.AutoPickupRadius.Value : 5.0f;
            Vector3 playerPos = player.transform.position;

            // Use spatial physics query instead of expensive FindObjectsOfType scene traversal
            int hitCount = Physics.OverlapSphereNonAlloc(playerPos, radius, _hitBuffer);
            if (hitCount <= 0) return;

            _processedThisTick.Clear();

            for (int i = 0; i < hitCount; i++)
            {
                var col = _hitBuffer[i];
                _hitBuffer[i] = null; // Clear reference immediately

                if (col == null || col.isTrigger) continue;

                var p = col.GetComponentInParent<PickupItem>() ?? col.GetComponent<PickupItem>();
                if (p == null || !p.canBePickedUp) continue;

                int instanceId = p.GetInstanceID();
                if (_processedThisTick.Contains(instanceId)) continue;
                _processedThisTick.Add(instanceId);

                // Space check
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
