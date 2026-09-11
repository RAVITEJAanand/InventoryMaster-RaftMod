using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InventoryMaster.Helpers
{
    #region [START] INPUT HELPER: HYBRID INPUT DETECTION
    // ============================================================================
    // [START] INPUT HELPER: HYBRID INPUT DETECTION
    // Description: Hybrid polling supporting both Unity Legacy Input and New Input System.
    // ============================================================================
    public static class InputHelper
    {
        private static bool _legacyInputAvailable = true;
        private static bool _loggedFallbackSwitch = false;
        private static bool _loggedNullKeyboard = false;

        public static bool WasKeyPressed(KeyCode legacyKey)
        {
            // 1. Try legacy Input
            if (_legacyInputAvailable)
            {
                try
                {
                    if (Input.GetKeyDown(legacyKey)) return true;
                    return false;
                }
                catch (Exception ex)
                {
                    _legacyInputAvailable = false;
                    Debug.LogWarning($"[Inventory Master] DIAGNOSTIC: Legacy Input.GetKeyDown threw, switching to New Input System fallback permanently. Exception: {ex.Message}");
                }
            }

            // 2. Try New Input System
            if (!_loggedFallbackSwitch)
            {
                _loggedFallbackSwitch = true;
                Debug.Log("[Inventory Master] DIAGNOSTIC: Now using New Input System fallback path for key checks.");
            }
            try
            {
                var kb = Keyboard.current;
                if (kb == null && !_loggedNullKeyboard)
                {
                    _loggedNullKeyboard = true;
                    Debug.LogWarning("[Inventory Master] DIAGNOSTIC: Keyboard.current is NULL - New Input System fallback cannot detect any key presses!");
                }
                if (kb != null)
                {
                    switch (legacyKey)
                    {
                        case KeyCode.F1: return kb.f1Key.wasPressedThisFrame;
                        case KeyCode.F2: return kb.f2Key.wasPressedThisFrame;
                        case KeyCode.F4: return kb.f4Key.wasPressedThisFrame;
                        case KeyCode.F5: return kb.f5Key.wasPressedThisFrame;
                        case KeyCode.Z: return kb.zKey.wasPressedThisFrame;
                        case KeyCode.K: return kb.kKey.wasPressedThisFrame;
                        case KeyCode.X: return kb.xKey.wasPressedThisFrame;
                        case KeyCode.V: return kb.vKey.wasPressedThisFrame;
                        case KeyCode.Q: return kb.qKey.wasPressedThisFrame;
                        case KeyCode.Delete: return kb.deleteKey.wasPressedThisFrame;
                        case KeyCode.Tab: return kb.tabKey.wasPressedThisFrame;
                        case KeyCode.Escape: return kb.escapeKey.wasPressedThisFrame;
                    }
                }
            }
            catch { }

            return false;
        }

        public static bool IsKeyHeld(KeyCode legacyKey)
        {
            try
            {
                if (Input.GetKey(legacyKey)) return true;
            }
            catch { }

            try
            {
                var kb = Keyboard.current;
                if (kb != null)
                {
                    switch (legacyKey)
                    {
                        case KeyCode.LeftShift: return kb.leftShiftKey.isPressed;
                        case KeyCode.RightShift: return kb.rightShiftKey.isPressed;
                        case KeyCode.LeftControl: return kb.leftCtrlKey.isPressed;
                        case KeyCode.RightControl: return kb.rightCtrlKey.isPressed;
                        case KeyCode.LeftAlt: return kb.leftAltKey.isPressed;
                        case KeyCode.RightAlt: return kb.rightAltKey.isPressed;
                    }
                }
            }
            catch { }

            return false;
        }

        public static bool IsShiftHeld()
        {
            return IsKeyHeld(KeyCode.LeftShift) || IsKeyHeld(KeyCode.RightShift);
        }

        public static bool IsCtrlHeld()
        {
            return IsKeyHeld(KeyCode.LeftControl) || IsKeyHeld(KeyCode.RightControl);
        }

        public static bool IsAltHeld()
        {
            return IsKeyHeld(KeyCode.LeftAlt) || IsKeyHeld(KeyCode.RightAlt);
        }
    }
    // ============================================================================
    // [END] INPUT HELPER: HYBRID INPUT DETECTION
    // ============================================================================
    #endregion
}
