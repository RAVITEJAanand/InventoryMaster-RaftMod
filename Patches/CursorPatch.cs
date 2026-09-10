using System;
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
        public static bool ShouldForceCursorFree()
        {
            return CanvasInventoryMasterUI.IsWindowOpen;
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
