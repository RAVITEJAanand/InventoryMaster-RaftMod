using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using InventoryMaster.UI;

namespace InventoryMaster.Patches
{
    #region [START] PATCH: CURSOR UNLOCKING & VISIBILITY
    // ============================================================================
    // [START] PATCH: CURSOR UNLOCKING & VISIBILITY
    // Description: Ensures mouse cursor is freed and interactable when Mod UI is active.
    // ============================================================================
    public static class CursorPatchHelper
    {
        private static PropertyInfo _scWindowProp;
        private static PropertyInfo _fcWindowProp;
        private static bool _typesResolved = false;

        public static bool ShouldForceCursorFree()
        {
            // 1. Inventory Master UI
            if (CanvasInventoryMasterUI.IsWindowOpen) return true;

            // 2. Peer Mods (Sailor's Companion & Farmer's Companion)
            if (!_typesResolved) ResolvePeerTypes();

            if (_scWindowProp != null)
            {
                try { if ((bool)_scWindowProp.GetValue(null)) return true; } catch { }
            }
            if (_fcWindowProp != null)
            {
                try { if ((bool)_fcWindowProp.GetValue(null)) return true; } catch { }
            }

            return false;
        }

        private static void ResolvePeerTypes()
        {
            try
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (_scWindowProp == null)
                    {
                        var scType = asm.GetType("SailorsCompanion.UI.CanvasModUI");
                        if (scType != null)
                            _scWindowProp = scType.GetProperty("IsWindowOpen", BindingFlags.Public | BindingFlags.Static);
                    }
                    if (_fcWindowProp == null)
                    {
                        var fcType = asm.GetType("FarmersCompanion.UI.CanvasFarmersCompanionUI");
                        if (fcType != null)
                            _fcWindowProp = fcType.GetProperty("IsWindowOpen", BindingFlags.Public | BindingFlags.Static);
                    }
                }
                if (_scWindowProp != null && _fcWindowProp != null) _typesResolved = true;
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(MouseLook), "Update")]
    public static class MouseLookUpdatePatch
    {
        public static bool Prefix()
        {
            if (CursorPatchHelper.ShouldForceCursorFree())
            {
                return false; // Freeze camera rotation while Inventory Master UI is open!
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Helper), "SetCursorVisibleAndLockState")]
    public static class HelperSetCursorVisibleAndLockStatePatch
    {
        public static void Prefix(ref bool state, ref CursorLockMode mode)
        {
            if (CursorPatchHelper.ShouldForceCursorFree())
            {
                state = true;
                mode = CursorLockMode.None;
            }
        }
    }

    [HarmonyPatch(typeof(Helper), "SetCursorLockState")]
    public static class HelperSetCursorLockStatePatch
    {
        public static void Prefix(ref CursorLockMode mode)
        {
            if (CursorPatchHelper.ShouldForceCursorFree())
            {
                mode = CursorLockMode.None;
            }
        }
    }

    [HarmonyPatch(typeof(Helper), "SetCursorVisible")]
    public static class HelperSetCursorVisiblePatch
    {
        public static void Prefix(ref bool state)
        {
            if (CursorPatchHelper.ShouldForceCursorFree())
            {
                state = true;
            }
        }
    }
    // ============================================================================
    // [END] PATCH: CURSOR UNLOCKING & VISIBILITY
    // ============================================================================
    #endregion
}
