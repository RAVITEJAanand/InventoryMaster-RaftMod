using System;
using UnityEngine;
using InventoryMaster.Helpers;

namespace InventoryMaster.Features
{
    #region [START] MODULE: AUTO EQUIP TOOLS (FEATURE 10 - OPTIMIZED)
    // ============================================================================
    // [START] MODULE: AUTO EQUIP TOOLS (FEATURE 10 - OPTIMIZED)
    // Purpose: Throttled raycast detection of resources on aim with cached camera
    //          and zero garbage allocation.
    // ============================================================================
    public static class AutoToolEquipManager
    {
        private static float _lastRaycastTime = 0f;
        private static string _lastEquippedToolType = "";
        private static Camera _cachedCamera = null;

        public static void UpdateAutoToolEquip()
        {
            if (Plugin.EnableAutoToolEquip == null || !Plugin.EnableAutoToolEquip.Value) return;

            if (Time.unscaledTime - _lastRaycastTime < 0.4f) return;
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

            // Raycast forward from camera, ignoring triggers
            Ray ray = new Ray(_cachedCamera.transform.position, _cachedCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 4.5f, ~0, QueryTriggerInteraction.Ignore))
            {
                var hitGO = hit.collider.gameObject;
                if (hitGO == null) return;

                string goName = hitGO.name;
                string desiredToolKeyword = null;

                if (goName.IndexOf("tree", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    goName.IndexOf("palm", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    goName.IndexOf("wood", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    goName.IndexOf("trunk", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    desiredToolKeyword = "axe";
                }
                else if (goName.IndexOf("debris", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("barrel", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("reef", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("clay", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("sand", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    desiredToolKeyword = "hook";
                }
                else if (goName.IndexOf("shark", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("bear", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("boar", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("bird", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("screecher", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         goName.IndexOf("lurker", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    desiredToolKeyword = "spear";
                }

                if (!string.IsNullOrEmpty(desiredToolKeyword) && desiredToolKeyword != _lastEquippedToolType)
                {
                    // Check if active slot already holds this tool
                    var curSlot = inv.GetSelectedHotbarSlot();
                    if (curSlot != null && !curSlot.IsEmpty && curSlot.GetItemBase() != null)
                    {
                        if (curSlot.GetItemBase().UniqueName.IndexOf(desiredToolKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            _lastEquippedToolType = desiredToolKeyword;
                            return;
                        }
                    }

                    // Look for desired tool in hotbar slots
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
    }
    // ============================================================================
    // [END] MODULE: AUTO EQUIP TOOLS (FEATURE 10 - OPTIMIZED)
    // ============================================================================
    #endregion
}
