using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InventoryMaster.Features;

namespace InventoryMaster.UI
{
    #region [START] UI: SLOT LOCK BADGE OVERLAY (FEATURE 13 - ZERO LAG CACHING)
    // ============================================================================
    // [START] UI: SLOT LOCK BADGE OVERLAY (FEATURE 13 - ZERO LAG CACHING)
    // Purpose: Instant O(1) slot lock badge management with zero transform.Find
    //          hierarchical traversal and zero garbage allocation.
    // ============================================================================
    public static class SlotLockOverlay
    {
        private const string BADGE_NAME = "IM_LockBadge";
        private static readonly Dictionary<int, GameObject> _cachedBadges = new Dictionary<int, GameObject>();
        private static Font _cachedFont = null;

        public static void UpdateSlotVisual(Slot slot)
        {
            if (slot == null || slot.gameObject == null) return;

            int slotId = slot.GetInstanceID();
            bool isLocked = FavoriteLockManager.IsLocked(slot);

            // Fast exit if no locks exist and this slot has no badge
            if (!isLocked && !FavoriteLockManager.HasAnyLocks && !_cachedBadges.ContainsKey(slotId))
            {
                return;
            }

            _cachedBadges.TryGetValue(slotId, out GameObject badgeGO);

            if (isLocked)
            {
                if (badgeGO == null)
                {
                    if (_cachedFont == null)
                    {
                        _cachedFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    }

                    badgeGO = new GameObject(BADGE_NAME);
                    badgeGO.transform.SetParent(slot.transform, false);

                    var rect = badgeGO.AddComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0f, 1f);
                    rect.anchorMax = new Vector2(0f, 1f);
                    rect.pivot = new Vector2(0f, 1f);
                    rect.anchoredPosition = new Vector2(2f, -2f);
                    rect.sizeDelta = new Vector2(20f, 20f);

                    var text = badgeGO.AddComponent<Text>();
                    text.text = "🔒";
                    text.fontSize = 14;
                    text.alignment = TextAnchor.MiddleCenter;
                    text.color = new Color(1f, 0.85f, 0.2f, 1f);
                    text.font = _cachedFont;

                    _cachedBadges[slotId] = badgeGO;
                }
                else
                {
                    if (!badgeGO.activeSelf) badgeGO.SetActive(true);
                }
            }
            else
            {
                if (badgeGO != null && badgeGO.activeSelf)
                {
                    badgeGO.SetActive(false);
                }
            }
        }
    }
    // ============================================================================
    // [END] UI: SLOT LOCK BADGE OVERLAY (FEATURE 13 - ZERO LAG CACHING)
    // ============================================================================
    #endregion
}
