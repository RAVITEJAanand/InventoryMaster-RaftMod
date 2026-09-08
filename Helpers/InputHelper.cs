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
        public static bool WasKeyPressed(KeyCode legacyKey)
        {
            // 1. Try legacy Input
            try
            {
                if (Input.GetKeyDown(legacyKey)) return true;
            }
            catch { }

            // 2. Try New Input System
            try
            {
                var kb = Keyboard.current;
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
