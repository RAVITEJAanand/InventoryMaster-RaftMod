using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InventoryMaster.Features;
using InventoryMaster.Helpers;

namespace InventoryMaster.UI
{
    #region [START] UI: CANVAS INVENTORY MASTER MOD MENU
    // ============================================================================
    // [START] UI: CANVAS INVENTORY MASTER MOD MENU
    // Description: Authentic Raft-styled high-DPI Unity Canvas UI with zero OnGUI
    //              overhead, matching Raft's native wooden plank aesthetics.
    //              (100% free of overlaps with Sailor's Companion features & hotkeys)
    // ============================================================================
    public class CanvasInventoryMasterUI : MonoBehaviour
    {
        public static CanvasInventoryMasterUI Instance { get; private set; }
        public static bool IsWindowOpen => Instance != null && Instance._modWindowGO != null && Instance._modWindowGO.activeSelf;

        private GameObject _canvasGO;
        private Canvas _canvas;
        private CanvasScaler _scaler;
        private GraphicRaycaster _raycaster;
        private GameObject _modWindowGO;

        // Tabs
        private const int TAB_COUNT = 4;
        private readonly GameObject[] _tabPages = new GameObject[TAB_COUNT];
        private readonly Text[] _tabButtonTexts = new Text[TAB_COUNT];
        private readonly Image[] _tabButtonImages = new Image[TAB_COUNT];
        private int _activeTab = 0;
        private int _toggleCounter = 0;

        private Font _gameFont;

        #region [START] RAFT NATIVE WOODEN PALETTE
        // ============================================================================
        // [START] RAFT NATIVE WOODEN PALETTE (Matching Raft's In-Game Settings Aesthetics)
        // ============================================================================
        private static readonly Color WoodWindowBg     = new Color(0.26f, 0.16f, 0.09f, 0.98f); // Deep Teak Plank #422917
        private static readonly Color WoodWindowBorder = new Color(0.18f, 0.10f, 0.05f, 1.00f); // Dark Outer Timber #2E1A0D
        private static readonly Color WoodTitleBar     = new Color(0.22f, 0.13f, 0.07f, 1.00f); // Dark Wood Header #382112
        private static readonly Color WoodTrimAccent   = new Color(0.78f, 0.65f, 0.44f, 1.00f); // Parchment Golden Wood Trim #C7A670

        // Tab Colors
        private static readonly Color TabActiveBg      = new Color(0.86f, 0.72f, 0.48f, 1.00f); // Warm Birch Parchment
        private static readonly Color TabActiveText    = new Color(0.18f, 0.10f, 0.05f, 1.00f); // Deep Carved Wood Font
        private static readonly Color TabInactiveBg    = new Color(0.20f, 0.12f, 0.06f, 0.96f); // Dark Inactive Wood
        private static readonly Color TabInactiveText  = new Color(0.82f, 0.72f, 0.58f, 1.00f); // Parchment Beige

        // Alternating Plank Strips
        private static readonly Color WoodPlankEven    = new Color(0.30f, 0.18f, 0.11f, 0.95f); // Plank A
        private static readonly Color WoodPlankOdd     = new Color(0.34f, 0.21f, 0.12f, 0.95f); // Plank B
        private static readonly Color WoodRowBorder    = new Color(0.20f, 0.11f, 0.06f, 0.90f); // Plank Gap Seam

        // Text Colors
        private static readonly Color TextParchmentLight = new Color(0.95f, 0.90f, 0.80f, 1.00f); // Warm Ivory
        private static readonly Color TextGoldHeading    = new Color(0.96f, 0.78f, 0.38f, 1.00f); // Gold Stencil
        private static readonly Color TextMuted          = new Color(0.68f, 0.58f, 0.45f, 1.00f); // Muted Wood

        // Checkboxes & Buttons
        private static readonly Color CheckboxWoodBg   = new Color(0.18f, 0.10f, 0.05f, 0.98f); // Recessed Box
        private static readonly Color CheckmarkGold    = new Color(0.92f, 0.78f, 0.52f, 1.00f); // Raft Golden Wood Check
        private static readonly Color WoodButtonNormal = new Color(0.38f, 0.23f, 0.14f, 0.96f); // Wood Plank Button
        private static readonly Color WoodButtonHover  = new Color(0.48f, 0.30f, 0.18f, 1.00f); // Lighter Wood Hover
        // ============================================================================
        // [END] RAFT NATIVE WOODEN PALETTE
        // ============================================================================
        #endregion

        private void Awake()
        {
            Instance = this;
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
            GetGameFont();
            BuildCanvasUI();
        }

        public Font GetGameFont()
        {
            if (_gameFont != null) return _gameFont;

            var texts = Resources.FindObjectsOfTypeAll<Text>();
            foreach (var t in texts)
            {
                if (t != null && t.font != null)
                {
                    _gameFont = t.font;
                    return _gameFont;
                }
            }

            try
            {
                _gameFont = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Segoe UI", "Tahoma" }, 14);
            }
            catch { }

            if (_gameFont == null)
            {
                _gameFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
            return _gameFont;
        }

        public static void ToggleWindow()
        {
            if (Instance == null) return;
            if (Instance._modWindowGO == null) Instance.BuildCanvasUI();

            bool newState = !Instance._modWindowGO.activeSelf;
            Instance._modWindowGO.SetActive(newState);

            if (newState)
            {
                Helper.SetCursorVisibleAndLockState(true, CursorLockMode.None);
            }
        }

        private void Update()
        {
            if (Plugin.KeyMenu != null && InputHelper.WasKeyPressed(Plugin.KeyMenu.Value))
            {
                ToggleWindow();
            }

            if (IsWindowOpen && InputHelper.WasKeyPressed(KeyCode.Escape))
            {
                _modWindowGO.SetActive(false);
            }
        }

        private void BuildCanvasUI()
        {
            if (_canvasGO == null)
            {
                _canvasGO = new GameObject("InventoryMaster_Canvas");
                _canvasGO.hideFlags = HideFlags.HideAndDontSave;
                _canvasGO.layer = LayerMask.NameToLayer("UI") >= 0 ? LayerMask.NameToLayer("UI") : 5;
                DontDestroyOnLoad(_canvasGO);

                _canvas = _canvasGO.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.overrideSorting = true;
                _canvas.sortingOrder = 32000;

                _scaler = _canvasGO.AddComponent<CanvasScaler>();
                _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                _scaler.referenceResolution = new Vector2(1920, 1080);
                _scaler.matchWidthOrHeight = 0.5f;

                _raycaster = _canvasGO.AddComponent<GraphicRaycaster>();
            }

            if (_modWindowGO == null)
            {
                BuildModWindow();
                _modWindowGO.SetActive(false);
            }
        }

        private void BuildModWindow()
        {
            _modWindowGO = new GameObject("Window_InventoryMaster");
            _modWindowGO.transform.SetParent(_canvasGO.transform, false);

            var winRt = _modWindowGO.AddComponent<RectTransform>();
            winRt.anchorMin = new Vector2(0.5f, 0.5f);
            winRt.anchorMax = new Vector2(0.5f, 0.5f);
            winRt.pivot = new Vector2(0.5f, 0.5f);
            winRt.anchoredPosition = Vector2.zero;
            winRt.sizeDelta = new Vector2(1040, 640);
            winRt.localScale = new Vector3(1.15f, 1.15f, 1.0f);

            var winImg = _modWindowGO.AddComponent<Image>();
            winImg.color = WoodWindowBg;

            // Outer Timber Border
            var outerBorder = CreateBox(_modWindowGO.transform, "WoodFrameBorder", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, WoodWindowBorder);
            var obRt = outerBorder.GetComponent<RectTransform>();
            obRt.offsetMin = new Vector2(-4, -4);
            obRt.offsetMax = new Vector2(4, 4);
            outerBorder.transform.SetAsFirstSibling();

            // Title Bar
            var titleBar = CreateBox(_modWindowGO.transform, "TitleBar", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), Vector2.zero, new Vector2(0, 50), WoodTitleBar);
            CreateBox(titleBar.transform, "TitleAccent", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 3), WoodTrimAccent);

            var titleText = CreateText(titleBar.transform, "TitleText", $"🎒 <color=#F5C761><b>INVENTORY MASTER</b></color> <size=13><color=#C7A670>v{PluginInfo.PLUGIN_VERSION}</color></size> — <size=13><color=#E6CEAC>Quality-of-Life & Inventory Management</color></size>", 18, FontStyle.Bold, TextParchmentLight, TextAnchor.MiddleLeft);
            titleText.rectTransform.offsetMin = new Vector2(18, 0);
            titleText.rectTransform.offsetMax = new Vector2(-60, 0);

            // Close Button [✕]
            CreateButton(titleBar.transform, "Btn_Close", "✕", new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-12, 0), new Vector2(32, 32), () => ToggleWindow(), CheckboxWoodBg, TextParchmentLight, 16);

            // Tabs Row
            var tabRow = CreateBox(_modWindowGO.transform, "TabRow", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -56), new Vector2(-28, 40), Color.clear);
            var tabLayout = tabRow.AddComponent<HorizontalLayoutGroup>();
            tabLayout.spacing = 8;
            tabLayout.childForceExpandWidth = true;
            tabLayout.childForceExpandHeight = true;

            string[] tabNames = { "🎒 BACKPACK & CAPACITY", "✂️ CONTROLS & TRANSFER", "🧲 AUTOMATION & REFILL", "🛡️ SAFETY & RECOVERY" };
            for (int i = 0; i < tabNames.Length; i++)
            {
                int index = i;
                var tabBtn = CreateButton(tabRow.transform, $"TabBtn_{i}", tabNames[i], Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, () => SelectTab(index), TabInactiveBg, TabInactiveText, 13);
                _tabButtonImages[i] = tabBtn.GetComponent<Image>();
                _tabButtonTexts[i] = tabBtn.GetComponentInChildren<Text>();
            }

            // Wooden Trim line separating tabs from content
            CreateBox(_modWindowGO.transform, "TabTrimLine", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -100), new Vector2(-28, 3), WoodTrimAccent);

            // Tab Content Area
            var contentArea = CreateBox(_modWindowGO.transform, "ContentArea", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, Color.clear);
            var cRt = contentArea.GetComponent<RectTransform>();
            cRt.offsetMin = new Vector2(18, 42);
            cRt.offsetMax = new Vector2(-18, -108);

            // Build individual tab pages
            _tabPages[0] = BuildBackpackTab(contentArea.transform);
            _tabPages[1] = BuildControlsTab(contentArea.transform);
            _tabPages[2] = BuildAutomationTab(contentArea.transform);
            _tabPages[3] = BuildSafetyTab(contentArea.transform);

            SelectTab(0);

            // Fixed Hotkeys Footer Bar (Showing F2 for Menu - Zero conflict with Sailor's Companion)
            var footerBar = CreateBox(_modWindowGO.transform, "FooterBar", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 36), WoodTitleBar);
            CreateBox(footerBar.transform, "FooterAccent", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), Vector2.zero, new Vector2(0, 2), WoodTrimAccent);
            CreateText(footerBar.transform, "FooterText", "<color=#C7A670>Hotkeys:</color> <color=#F5C761>[F2]</color> Menu  |  <color=#F5C761>[Z]</color> Auto Sort  |  <color=#F5C761>[X]</color> Dump to Chest  |  <color=#F5C761>[V]</color> Hotbar Swap  |  <color=#F5C761>[Alt+Click]</color> Lock Slot  |  <color=#F5C761>[Delete]</color> Trash  |  <color=#F5C761>[ESC]</color> Close", 12, FontStyle.Bold, TextParchmentLight, TextAnchor.MiddleCenter);
        }

        private void SelectTab(int index)
        {
            _activeTab = index;
            for (int i = 0; i < TAB_COUNT; i++)
            {
                if (_tabPages[i] != null) _tabPages[i].SetActive(i == index);
                if (_tabButtonImages[i] != null)
                {
                    _tabButtonImages[i].color = (i == index) ? TabActiveBg : TabInactiveBg;
                    _tabButtonTexts[i].color = (i == index) ? TabActiveText : TabInactiveText;
                    _tabButtonTexts[i].fontStyle = (i == index) ? FontStyle.Bold : FontStyle.Normal;
                }
            }
        }

        #region [START] TAB PAGES BUILDERS
        private GameObject BuildBackpackTab(Transform parent)
        {
            var page = CreateTabPage(parent, "Page_Backpack");

            CreateSectionBanner(page.transform, "✨ INVENTORY ACTIONS & MANAGEMENT");

            CreateDualActionButton(page.transform,
                "✨ Auto Sort Backpack [Z]", () => InventorySorter.SortCurrentInventory(),
                "✨ Auto Sort Open Chest [Z]", () => InventorySorter.SortCurrentInventory());

            CreateDualActionButton(page.transform,
                "📥 Dump Backpack to Chest [X]", () => StorageDumpManager.DumpBackpackToOpenStorage(),
                "🔄 Swap Hotbar with Backpack Row 1 [V]", () => HotbarExpansionManager.SwapHotbarRow());

            CreateSectionBanner(page.transform, "🎒 BACKPACK EXPANSION & CAPACITY");

            CreateToggleItem(page.transform, "Permanent Backpack Slots Unlock (All 15 slots permanently active)", Plugin.EnableInventoryExpansion.Value, val => Plugin.EnableInventoryExpansion.Value = val);

            var infoBox = CreateBox(page.transform, "BackpackInfo", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 52), WoodPlankEven);
            EnsureLayout(infoBox, -1, 52);
            var infoText = CreateText(infoBox.transform, "InfoTxt", "• <b>Hotbar Swap (V):</b> Instantly swaps your 10 active hotbar slots with Row 1 of your backpack.\n• <b>Auto Sort (Z):</b> Merges partial stacks and categorizes items by Tools, Gear, Food, and Resources.", 13, FontStyle.Normal, TextParchmentLight, TextAnchor.MiddleLeft);
            infoText.rectTransform.offsetMin = new Vector2(14, 0);

            return page;
        }

        private GameObject BuildControlsTab(Transform parent)
        {
            var page = CreateTabPage(parent, "Page_Controls");

            CreateSectionBanner(page.transform, "✂️ SMART ITEM SPLITTING & BULK TRANSFERS");

            CreateToggleItem(page.transform, "Smart Item Split (Shift+RightClick = 1, Ctrl+RightClick = Half stack)", Plugin.EnableSmartSplit.Value, val => Plugin.EnableSmartSplit.Value = val);
            CreateToggleItem(page.transform, "Fast Item Transfer (Double-Click to move all stacks of this item)", Plugin.EnableFastItemTransfer.Value, val => Plugin.EnableFastItemTransfer.Value = val);

            CreateSectionBanner(page.transform, "💡 HOW TO USE CONTROLS");

            var infoBox = CreateBox(page.transform, "ControlsInfo", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 94), WoodPlankEven);
            EnsureLayout(infoBox, -1, 94);
            var infoText = CreateText(infoBox.transform, "ControlsInfoText", "• <b>Shift + Right Click:</b> Quickly takes exactly <b>1 item</b> into the next available empty slot.\n• <b>Ctrl + Right Click:</b> Splits exactly <b>half the stack</b> into the next available empty slot.\n• <b>Double Left-Click:</b> When viewing a storage chest, double-clicking any item instantly transfers <b>all matching stacks</b> between your backpack and the chest!", 13, FontStyle.Normal, TextParchmentLight, TextAnchor.MiddleLeft);
            infoText.rectTransform.offsetMin = new Vector2(14, 0);

            return page;
        }

        private GameObject BuildAutomationTab(Transform parent)
        {
            var page = CreateTabPage(parent, "Page_Automation");

            CreateSectionBanner(page.transform, "🧲 AUTO PICKUP & CONTEXT TOOL SWITCHING");

            CreateToggleItem(page.transform, "Auto Pickup Nearby Debris & Loose Items (Collects directly into pocket)", Plugin.EnableAutoPickup.Value, val => Plugin.EnableAutoPickup.Value = val);
            CreateStepperItem(page.transform, "Auto Pickup Radius", 2f, 12f, 1f, Plugin.AutoPickupRadius.Value, "m", val => Plugin.AutoPickupRadius.Value = val);
            CreateToggleItem(page.transform, "Auto Equip Tools on Context (Axe on tree, Hook on debris) & Auto-Replace Broken", Plugin.EnableAutoToolEquip.Value, val => Plugin.EnableAutoToolEquip.Value = val);

            CreateSectionBanner(page.transform, "💧 AUTO REFILL VITAL CONSUMABLES");

            CreateToggleItem(page.transform, "Auto Refill Fresh Water & Cooked Food (Strictly avoids salt water/poison)", Plugin.EnableAutoRefill.Value, val => Plugin.EnableAutoRefill.Value = val);
            CreateStepperItem(page.transform, "Refill Stats Threshold", 0.1f, 0.5f, 0.05f, Plugin.AutoRefillThreshold.Value, "%", val => Plugin.AutoRefillThreshold.Value = val);

            return page;
        }

        private GameObject BuildSafetyTab(Transform parent)
        {
            var page = CreateTabPage(parent, "Page_Safety");

            CreateSectionBanner(page.transform, "🛡️ DROP PROTECTION & FAVORITE ITEM LOCKS");

            CreateToggleItem(page.transform, "Enable Drop Protection (Blocks accidental Q drops; hold Shift+Q to drop)", Plugin.EnableDropProtection.Value, val => Plugin.EnableDropProtection.Value = val);
            CreateToggleItem(page.transform, "Strict Drop Guard for Weapons, Tools & Armor", Plugin.ProtectToolsAndEquipment.Value, val => Plugin.ProtectToolsAndEquipment.Value = val);

            CreateDualActionButton(page.transform,
                "🔓 Clear All Favorite Item Locks", () =>
                {
                    FavoriteLockManager.ClearAll();
                    ToastManager.Show("🔓 All slot locks have been cleared!");
                },
                "↩️ Restore Last Trashed Item (Undo)", () => TrashSlotManager.UndoTrash());

            CreateSectionBanner(page.transform, "💡 QUICK HINTS & GUIDE");

            var infoBox = CreateBox(page.transform, "InfoBox", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 72), WoodPlankEven);
            EnsureLayout(infoBox, -1, 72);
            var infoText = CreateText(infoBox.transform, "InfoText", "• <b>Favorite Lock:</b> Press <b>Alt + Left Click</b> on any slot to lock/unlock. Shows 🔒 badge.\n• <b>Trash Slot:</b> Hover over unwanted items and press <b>Delete</b> to trash them safely.\n• <b>Undo Buffer:</b> Trashed by mistake? Click the Undo button above to recover your item!", 13, FontStyle.Normal, TextParchmentLight, TextAnchor.MiddleLeft);
            infoText.rectTransform.offsetMin = new Vector2(14, 0);

            return page;
        }
        #endregion

        #region [START] UI COMPONENT HELPERS
        private GameObject CreateTabPage(Transform parent, string name)
        {
            var page = CreateBox(parent, name, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, Color.clear);
            var layout = page.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 6;
            layout.padding = new RectOffset(12, 12, 6, 6);
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return page;
        }

        private void CreateSectionBanner(Transform parent, string title)
        {
            var banner = CreateBox(parent, "Banner", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 32), WoodTitleBar);
            EnsureLayout(banner, -1, 32);
            CreateText(banner.transform, "Txt", $"─── {title} ───", 13, FontStyle.Bold, TextGoldHeading, TextAnchor.MiddleCenter);
        }

        private void CreateDualActionButton(Transform parent, string label1, Action action1, string label2, Action action2)
        {
            var row = CreateBox(parent, "DualActionRow", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 38), Color.clear);
            EnsureLayout(row, -1, 38);

            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            var btn1 = CreateButton(row.transform, "Btn1", label1, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, action1, WoodButtonNormal, TextParchmentLight, 13);
            var btn2 = CreateButton(row.transform, "Btn2", label2, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, action2, WoodButtonNormal, TextParchmentLight, 13);
        }

        private void CreateToggleItem(Transform parent, string label, bool initialValue, Action<bool> onToggle)
        {
            Color plankColor = (_toggleCounter++ % 2 == 0) ? WoodPlankEven : WoodPlankOdd;
            var row = CreateBox(parent, "ToggleRow", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 34), plankColor);
            EnsureLayout(row, -1, 34);

            CreateBox(row.transform, "Seam", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 1), WoodRowBorder);

            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(14, 14, 2, 2);
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            var labelTxt = CreateText(row.transform, "Label", label, 14, FontStyle.Normal, TextParchmentLight, TextAnchor.MiddleLeft);
            EnsureLayout(labelTxt.gameObject, 860, 30, true);

            var checkContainer = CreateBox(row.transform, "CheckContainer", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(28, 28), CheckboxWoodBg);
            EnsureLayout(checkContainer, 28, 28, false);

            var checkTxt = CreateText(checkContainer.transform, "Checkmark", initialValue ? "✔" : "", 16, FontStyle.Bold, CheckmarkGold, TextAnchor.MiddleCenter);

            bool state = initialValue;
            var btn = checkContainer.AddComponent<Button>();
            btn.targetGraphic = checkContainer.GetComponent<Image>();

            void Toggle()
            {
                state = !state;
                checkTxt.text = state ? "✔" : "";
                onToggle?.Invoke(state);
            }

            btn.onClick.AddListener(Toggle);

            var rowBtn = row.AddComponent<Button>();
            rowBtn.targetGraphic = row.GetComponent<Image>();
            rowBtn.onClick.AddListener(Toggle);
        }

        private void CreateStepperItem(Transform parent, string label, float min, float max, float step, float initialValue, string unit, Action<float> onChange)
        {
            Color plankColor = (_toggleCounter++ % 2 == 0) ? WoodPlankEven : WoodPlankOdd;
            var row = CreateBox(parent, "StepperRow", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 34), plankColor);
            EnsureLayout(row, -1, 34);

            CreateBox(row.transform, "Seam", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 1), WoodRowBorder);

            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(14, 14, 2, 2);
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            float val = initialValue;
            string displayVal = unit == "%" ? $"{Mathf.RoundToInt(val * 100)}%" : $"{val:F0}{unit}";

            var labelTxt = CreateText(row.transform, "Label", $"{label}: <color=#F5C761><b>{displayVal}</b></color>", 14, FontStyle.Normal, TextParchmentLight, TextAnchor.MiddleLeft);
            EnsureLayout(labelTxt.gameObject, 800, 30, true);

            var minusBtn = CreateButton(row.transform, "Minus", "  －  ", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(44, 28), () =>
            {
                val = Mathf.Clamp(val - step, min, max);
                displayVal = unit == "%" ? $"{Mathf.RoundToInt(val * 100)}%" : $"{val:F0}{unit}";
                labelTxt.text = $"{label}: <color=#F5C761><b>{displayVal}</b></color>";
                onChange?.Invoke(val);
            }, WoodButtonNormal, TextParchmentLight, 15);
            EnsureLayout(minusBtn, 44, 28, false);

            var plusBtn = CreateButton(row.transform, "Plus", "  ＋  ", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(44, 28), () =>
            {
                val = Mathf.Clamp(val + step, min, max);
                displayVal = unit == "%" ? $"{Mathf.RoundToInt(val * 100)}%" : $"{val:F0}{unit}";
                labelTxt.text = $"{label}: <color=#F5C761><b>{displayVal}</b></color>";
                onChange?.Invoke(val);
            }, WoodButtonNormal, TextParchmentLight, 15);
            EnsureLayout(plusBtn, 44, 28, false);
        }

        private GameObject CreateBox(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            var img = go.AddComponent<Image>();
            img.color = color;
            return go;
        }

        private Text CreateText(Transform parent, string name, string content, int fontSize, FontStyle style, Color color, TextAnchor alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            var t = go.AddComponent<Text>();
            t.font = GetGameFont();
            t.text = content;
            t.fontSize = fontSize;
            t.fontStyle = style;
            t.color = color;
            t.alignment = alignment;
            t.supportRichText = true;
            return t;
        }

        private GameObject CreateButton(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta, Action onClick, Color bgColor, Color textColor, int fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            var img = go.AddComponent<Image>();
            img.color = bgColor;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            var cb = btn.colors;
            cb.normalColor = bgColor;
            cb.highlightedColor = WoodButtonHover;
            cb.pressedColor = WoodWindowBorder;
            cb.selectedColor = bgColor;
            btn.colors = cb;

            if (onClick != null) btn.onClick.AddListener(() => onClick());

            var textGO = new GameObject("Text");
            textGO.transform.SetParent(go.transform, false);
            var textRt = textGO.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.sizeDelta = Vector2.zero;

            var t = textGO.AddComponent<Text>();
            t.font = GetGameFont();
            t.text = label;
            t.fontSize = fontSize;
            t.fontStyle = FontStyle.Bold;
            t.color = textColor;
            t.alignment = TextAnchor.MiddleCenter;
            t.supportRichText = true;

            return go;
        }

        private LayoutElement EnsureLayout(GameObject go, float prefWidth, float prefHeight, bool flexibleWidth = false)
        {
            var le = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
            if (prefWidth >= 0) le.preferredWidth = prefWidth;
            if (prefHeight >= 0) le.preferredHeight = prefHeight;
            le.flexibleWidth = flexibleWidth ? 1f : 0f;
            return le;
        }
        #endregion
    }
    // ============================================================================
    // [END] UI: CANVAS INVENTORY MASTER MOD MENU
    // ============================================================================
    #endregion
}
