using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using InventoryMaster.Features;
using InventoryMaster.Helpers;
using InventoryMaster.UI;
using UnityEngine;

namespace InventoryMaster
{
    #region [START] MAIN PLUGIN ENTRY: INVENTORY MASTER
    // ============================================================================
    // [START] MAIN PLUGIN ENTRY: INVENTORY MASTER
    // Description: Pure, non-overlapping QoL inventory management mod.
    //              (Zero overlap with Sailor's Companion features or hotkeys)
    // ============================================================================
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Raft.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public static GameObject ManagerGO { get; private set; }

        #region [START] CONFIGURATION DEFINITIONS
        // Hotkeys (F2 default - zero clash with Sailor's Companion F3/F4/F5/F6/F7/F8/F9/F10)
        public static ConfigEntry<KeyCode> KeyMenu;
        public static ConfigEntry<KeyCode> KeySort;
        public static ConfigEntry<KeyCode> KeyStorageDump;
        public static ConfigEntry<KeyCode> KeyHotbarSwap;

        // Feature 5: Inventory Expansion
        public static ConfigEntry<bool> EnableInventoryExpansion;

        // Feature 7 & 10: Auto Pickup & Tool Equip
        public static ConfigEntry<bool> EnableAutoPickup;
        public static ConfigEntry<float> AutoPickupRadius;
        public static ConfigEntry<bool> EnableAutoToolEquip;

        // Feature 8: Drop Protection
        public static ConfigEntry<bool> EnableDropProtection;
        public static ConfigEntry<bool> ProtectToolsAndEquipment;

        // Feature 9 & 11: Smart Split & Fast Transfer
        public static ConfigEntry<bool> EnableSmartSplit;
        public static ConfigEntry<bool> EnableFastItemTransfer;

        // Feature 15: Auto Refill Consumables
        public static ConfigEntry<bool> EnableAutoRefill;
        public static ConfigEntry<float> AutoRefillThreshold;

        // Auto Update
        public static ConfigEntry<bool> CheckForUpdates;
        #endregion

        private Harmony _harmony;

        private void Awake()
        {
            Instance = this;

            // Bind Hotkeys (F2 for Menu, Z for Sort, X for Dump, V for Hotbar Swap)
            KeyMenu = Config.Bind("General.Hotkeys", "KeyMenu", KeyCode.F2, "Hotkey to toggle the in-game Mod Menu (F2 has 0 conflict with Sailor's Companion).");
            KeySort = Config.Bind("General.Hotkeys", "KeySort", KeyCode.Z, "Hotkey to auto sort backpack or chest.");
            KeyStorageDump = Config.Bind("General.Hotkeys", "KeyStorageDump", KeyCode.X, "Hotkey to dump backpack into open chest.");
            KeyHotbarSwap = Config.Bind("General.Hotkeys", "KeyHotbarSwap", KeyCode.V, "Hotkey to swap hotbar with backpack row 1.");

            // Feature 5: Expansion
            EnableInventoryExpansion = Config.Bind("Features.Inventory", "EnableInventoryExpansion", true, "Permanently unlock and activate all 15 backpack slots.");

            // Feature 7 & 10: Automation
            EnableAutoPickup = Config.Bind("Features.Automation", "EnableAutoPickup", true, "Automatically collect loose and floating debris within 5m.");
            AutoPickupRadius = Config.Bind("Features.Automation", "AutoPickupRadius", 5.0f, "Effective radius for auto item pickup.");
            EnableAutoToolEquip = Config.Bind("Features.Automation", "EnableAutoToolEquip", true, "Context-aware tool auto-selection and replacement of broken tools.");

            // Feature 8: Protection
            EnableDropProtection = Config.Bind("Features.Safety", "EnableDropProtection", true, "Prevent accidental item drops (hold Shift+Q to drop).");
            ProtectToolsAndEquipment = Config.Bind("Features.Safety", "ProtectToolsAndEquipment", true, "Strictly protect weapons, tools, and armor from accidental drop.");

            // Feature 9 & 11: Controls
            EnableSmartSplit = Config.Bind("Features.Controls", "EnableSmartSplit", true, "Enable Shift/Ctrl+RightClick instant stack splitting.");
            EnableFastItemTransfer = Config.Bind("Features.Controls", "EnableFastItemTransfer", true, "Enable double-click bulk transfers between player and storage.");

            // Feature 15: Auto Refill
            EnableAutoRefill = Config.Bind("Features.Survival", "EnableAutoRefill", true, "Automatically consume safe food and fresh water when hunger/thirst drops below threshold.");
            AutoRefillThreshold = Config.Bind("Features.Survival", "AutoRefillThreshold", 0.25f, "Stat threshold under which consumables are automatically used (0.25 = 25%).");

            // Auto Update Checker
            CheckForUpdates = Config.Bind("General", "CheckForUpdates", true, "Check GitHub for new updates on startup.");

            // Register Harmony Patches
            RegisterHarmonyPatches();

            // Ensure persistent manager GameObject
            EnsureManager();

            Logger.LogInfo($"[{PluginInfo.PLUGIN_NAME}] v{PluginInfo.PLUGIN_VERSION} initialized successfully! Press F2 for menu, Z to Sort, X to Dump, V to Swap Hotbar.");
        }

        private void RegisterHarmonyPatches()
        {
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            try
            {
                var types = Assembly.GetExecutingAssembly().GetTypes();
                foreach (var type in types)
                {
                    if (type.GetCustomAttributes(typeof(HarmonyPatch), true).Length > 0)
                    {
                        try
                        {
                            _harmony.CreateClassProcessor(type).Patch();
                            Logger.LogInfo($"[{PluginInfo.PLUGIN_NAME}] Patched: {type.Name}");
                        }
                        catch (Exception ex)
                        {
                            Logger.LogWarning($"[{PluginInfo.PLUGIN_NAME}] Failed to patch {type.Name}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"[{PluginInfo.PLUGIN_NAME}] Error registering patches: {ex.Message}");
            }
        }

        private void EnsureManager()
        {
            if (ManagerGO == null)
            {
                ManagerGO = new GameObject("InventoryMaster_Manager");
                DontDestroyOnLoad(ManagerGO);
                ManagerGO.hideFlags = HideFlags.HideAndDontSave;

                ManagerGO.AddComponent<ToastManager>();
                ManagerGO.AddComponent<CanvasInventoryMasterUI>();
                ManagerGO.AddComponent<UpdateChecker>();
            }
        }

        private void Update()
        {
            if (!PlayerHelper.IsInGameWorld()) return;

            // Global Hotbar Swap Hotkey (V)
            if (KeyHotbarSwap != null && InputHelper.WasKeyPressed(KeyHotbarSwap.Value))
            {
                HotbarExpansionManager.SwapHotbarRow();
            }

            // Real-time automation loops
            InventoryExpansionManager.UpdateInventoryExpansion();
            AutoPickupManager.UpdateAutoPickup();
            AutoToolEquipManager.UpdateAutoToolEquip();
            AutoRefillManager.UpdateAutoRefill();
        }
    }
    // ============================================================================
    // [END] MAIN PLUGIN ENTRY: INVENTORY MASTER
    // ============================================================================
    #endregion
}
