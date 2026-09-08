using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InventoryMaster.UI;

namespace InventoryMaster.Patches
{
    #region [START] PATCH: TITLE SCREEN BUTTON & BADGE INJECTION
    // ============================================================================
    // [START] PATCH: TITLE SCREEN BUTTON & BADGE INJECTION
    // Purpose: Injects "🎒 Inventory Master Active [F2]" badge and menu button
    //          into Raft's StartMenuScreen and PauseMenu for intuitive accessibility.
    // ============================================================================
    [HarmonyPatch(typeof(StartMenuScreen), "Start")]
    public static class StartMenuScreenStartPatch
    {
        public static void Postfix(StartMenuScreen __instance)
        {
            InjectModsButton(__instance);
        }

        public static void InjectModsButton(StartMenuScreen startMenu)
        {
            if (startMenu == null) return;
            try
            {
                var trav = Traverse.Create(startMenu);

                // 1. Add version watermark badge
                var vText = trav.Field<Text>("versionText")?.Value;
                if (vText != null && !vText.text.Contains("Inventory Master"))
                {
                    vText.text += "\n<color=#FFA726><b>🎒 Inventory Master Active [F2]</b></color>";
                }

                // 2. Add title menu button
                var settingsSel = trav.Field<Selectable>("settingsButton")?.Value;
                var menuButtonsGO = trav.Field<GameObject>("menuButtons")?.Value 
                    ?? (settingsSel != null ? settingsSel.transform.parent?.gameObject : GameObject.Find("MenuButtons"));

                if (menuButtonsGO == null) return;
                if (menuButtonsGO.transform.Find("Button_InventoryMaster_Mods") != null) return;

                // Find a template button
                GameObject templateGO = null;
                var sailorsBtn = menuButtonsGO.transform.Find("Button_SailorsCompanion_Mods");
                if (sailorsBtn != null)
                {
                    templateGO = sailorsBtn.gameObject;
                }
                else if (settingsSel != null)
                {
                    templateGO = settingsSel.gameObject;
                }
                else
                {
                    var anyBtn = menuButtonsGO.GetComponentInChildren<Button>(true);
                    if (anyBtn != null) templateGO = anyBtn.gameObject;
                }

                if (templateGO != null)
                {
                    var newBtnGO = GameObject.Instantiate(templateGO, menuButtonsGO.transform);
                    newBtnGO.name = "Button_InventoryMaster_Mods";
                    newBtnGO.transform.SetSiblingIndex(templateGO.transform.GetSiblingIndex() + 1);

                    // Update Text
                    var tmp = newBtnGO.GetComponentInChildren<TMP_Text>(true);
                    if (tmp != null)
                    {
                        tmp.text = "INVENTORY MASTER";
                        tmp.color = new Color(1.0f, 0.65f, 0.15f); // Amber gold
                    }
                    var legacyText = newBtnGO.GetComponentInChildren<Text>(true);
                    if (legacyText != null)
                    {
                        legacyText.text = "INVENTORY MASTER";
                        legacyText.color = new Color(1.0f, 0.65f, 0.15f);
                    }

                    var btn = newBtnGO.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick = new Button.ButtonClickedEvent();
                        btn.onClick.AddListener(() =>
                        {
                            Debug.Log("[Inventory Master] Main Menu INVENTORY MASTER button clicked!");
                            CanvasInventoryMasterUI.ToggleWindow();
                        });
                    }

                    foreach (var comp in newBtnGO.GetComponents<MonoBehaviour>())
                    {
                        if (comp != null && comp != btn && !(comp is TMP_Text) && !(comp is Text) && !(comp is Graphic))
                        {
                            if (comp.GetType().Name.Contains("Setting") || comp is UnityEngine.EventSystems.IPointerClickHandler)
                            {
                                GameObject.Destroy(comp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[Inventory Master] Error injecting StartMenu badge/button: " + ex.Message);
            }
        }
    }

    [HarmonyPatch(typeof(StartMenuScreen), "LateStart")]
    public static class StartMenuScreenLateStartPatch
    {
        public static void Postfix(StartMenuScreen __instance)
        {
            StartMenuScreenStartPatch.InjectModsButton(__instance);
        }
    }

    [HarmonyPatch(typeof(StartMenuScreen), "Update")]
    public static class StartMenuScreenUpdatePatch
    {
        private static float _lastCheck = 0f;
        public static void Postfix(StartMenuScreen __instance)
        {
            if (Time.unscaledTime - _lastCheck > 0.5f)
            {
                _lastCheck = Time.unscaledTime;
                StartMenuScreenStartPatch.InjectModsButton(__instance);
            }
        }
    }
    #endregion

    #region [START] PATCH: PAUSE MENU BUTTON INJECTION
    // ============================================================================
    // [START] PATCH: PAUSE MENU BUTTON INJECTION
    // Purpose: Injects "INVENTORY MASTER" button into the ESC Pause Menu.
    // ============================================================================
    [HarmonyPatch(typeof(PauseMenu), "Start")]
    public static class PauseMenuStartPatch
    {
        public static void Postfix(PauseMenu __instance)
        {
            InjectPauseModsButton(__instance);
        }

        public static void InjectPauseModsButton(PauseMenu pauseMenu)
        {
            if (pauseMenu == null) return;
            try
            {
                var holder = pauseMenu.buttonHolderPanel ?? (pauseMenu.transform.Find("ButtonHolder")?.gameObject);
                if (holder == null) return;

                var parent = holder.transform;
                if (parent.Find("Button_InventoryMaster_PauseMods") != null) return;

                // Find a template button
                var sailorsBtn = parent.Find("Button_SailorsCompanion_PauseMods");
                GameObject templateGO = sailorsBtn != null ? sailorsBtn.gameObject : null;
                if (templateGO == null)
                {
                    var targetBtn = parent.GetComponentInChildren<Button>(true);
                    if (targetBtn != null) templateGO = targetBtn.gameObject;
                }

                if (templateGO != null)
                {
                    var newBtnGO = GameObject.Instantiate(templateGO, parent);
                    newBtnGO.name = "Button_InventoryMaster_PauseMods";
                    newBtnGO.transform.SetSiblingIndex(templateGO.transform.GetSiblingIndex() + 1);

                    var tmp = newBtnGO.GetComponentInChildren<TMP_Text>(true);
                    if (tmp != null)
                    {
                        tmp.text = "INVENTORY MASTER";
                        tmp.color = new Color(1.0f, 0.65f, 0.15f);
                    }
                    var txt = newBtnGO.GetComponentInChildren<Text>(true);
                    if (txt != null)
                    {
                        txt.text = "INVENTORY MASTER";
                        txt.color = new Color(1.0f, 0.65f, 0.15f);
                    }

                    var btn = newBtnGO.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick = new Button.ButtonClickedEvent();
                        btn.onClick.AddListener(() =>
                        {
                            Debug.Log("[Inventory Master] Pause Menu INVENTORY MASTER button clicked!");
                            CanvasInventoryMasterUI.ToggleWindow();
                        });
                    }

                    foreach (var comp in newBtnGO.GetComponents<MonoBehaviour>())
                    {
                        if (comp != null && comp != btn && !(comp is TMP_Text) && !(comp is Text) && !(comp is Graphic))
                        {
                            if (comp.GetType().Name.Contains("Setting") || comp is UnityEngine.EventSystems.IPointerClickHandler)
                            {
                                GameObject.Destroy(comp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[Inventory Master] Error injecting PauseMenu button: " + ex.Message);
            }
        }
    }

    [HarmonyPatch(typeof(PauseMenu), "Update")]
    public static class PauseMenuUpdatePatch
    {
        private static float _lastPauseCheck = 0f;
        public static void Postfix(PauseMenu __instance)
        {
            if (Time.unscaledTime - _lastPauseCheck > 0.5f)
            {
                _lastPauseCheck = Time.unscaledTime;
                PauseMenuStartPatch.InjectPauseModsButton(__instance);
            }
        }
    }
    #endregion
}
