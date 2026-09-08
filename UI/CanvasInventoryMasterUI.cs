using System;
using UnityEngine;
using UnityEngine.UI;
using InventoryMaster.Helpers;
using InventoryMaster.Features;

namespace InventoryMaster.UI
{
    #region [START] CANVAS INVENTORY MASTER SETTINGS UI (AAA STUDIO OVERHAUL)
    // ============================================================================
    // [START] CANVAS INVENTORY MASTER SETTINGS UI (AAA STUDIO OVERHAUL)
    // Purpose: Spacious (1200x720), razor-sharp in-game settings canvas for Inventory Master.
    //          Zero font squishing, bright active tab indicator, fixed single-line titles,
    //          right-aligned custom checkboxes, and sculpted 54px action tiles with hotkey badges.
    // ============================================================================
    public class CanvasInventoryMasterUI : MonoBehaviour
    {
        public static CanvasInventoryMasterUI Instance { get; private set; }

        private GameObject _canvasGO;
        private Canvas _canvas;
        private CanvasScaler _scaler;
        private GraphicRaycaster _raycaster;
        private GameObject _modWindowGO;
        private Font _gameFont;

        public static bool IsWindowOpen => Instance != null && Instance._modWindowGO != null && Instance._modWindowGO.activeSelf;

        #region [START] RAFT NATIVE HIGH-CONTRAST TIMBER PALETTE
        // ============================================================================
        // [START] RAFT NATIVE HIGH-CONTRAST TIMBER PALETTE
        // ============================================================================
        private static readonly Color WoodWindowBg      = new Color(0.20f, 0.12f, 0.07f, 0.99f); // Deep Teak Plank
        private static readonly Color WoodWindowBorder  = new Color(0.10f, 0.05f, 0.02f, 1.00f); // Dark Timber Outline
        private static readonly Color WoodTitleBar      = new Color(0.16f, 0.09f, 0.04f, 1.00f); // Header Bar
        private static readonly Color WoodTrimAccent    = new Color(0.92f, 0.74f, 0.38f, 1.00f); // Golden Wood Trim #EBB861

        // Tab Colors (Bright Warm Birch Parchment for Active, Deep Timber for Inactive)
        private static readonly Color TabActiveBg       = new Color(0.88f, 0.74f, 0.48f, 1.00f); // Bright Warm Birch Parchment #E0BD7A
        private static readonly Color TabActiveBorder   = new Color(1.00f, 0.85f, 0.40f, 1.00f); // Glowing Gold Tab Rim
        private static readonly Color TabActiveText     = new Color(0.18f, 0.10f, 0.05f, 1.00f); // Deep Carved Timber Font #2E1A0D
        private static readonly Color TabInactiveBg     = new Color(0.18f, 0.11f, 0.06f, 0.96f); // Dark Wood Plank
        private static readonly Color TabInactiveBorder = new Color(0.28f, 0.18f, 0.10f, 0.60f); // Dark Inactive Rim
        private static readonly Color TabInactiveText   = new Color(0.86f, 0.78f, 0.68f, 1.00f); // Soft Parchment Text

        // Alternating Plank Strips
        private static readonly Color WoodPlankEven     = new Color(0.25f, 0.15f, 0.09f, 0.98f); // Plank A
        private static readonly Color WoodPlankOdd      = new Color(0.28f, 0.17f, 0.10f, 0.98f); // Plank B
        private static readonly Color WoodRowBorder     = new Color(0.12f, 0.06f, 0.03f, 0.90f); // Plank Seam

        // High-Contrast Text Colors
        private static readonly Color TextWhite          = new Color(1.00f, 1.00f, 1.00f, 1.00f); // Pure Crisp White
        private static readonly Color TextParchmentLight = new Color(0.96f, 0.93f, 0.87f, 1.00f); // Warm Ivory
        private static readonly Color TextGoldHeading    = new Color(1.00f, 0.82f, 0.35f, 1.00f); // Vibrant Gold
        private static readonly Color TextMuted          = new Color(0.80f, 0.72f, 0.60f, 1.00f); // Soft Timber

        // Action Buttons & Checkboxes
        private static readonly Color ActionTileBg      = new Color(0.32f, 0.19f, 0.11f, 1.00f); // Carved Oak Button
        private static readonly Color ActionTileHover   = new Color(0.46f, 0.28f, 0.16f, 1.00f); // Bright Polished Hover
        private static readonly Color ActionTileBorder  = new Color(0.90f, 0.72f, 0.36f, 0.95f); // Golden Brass Rim
        private static readonly Color CheckboxWoodBg    = new Color(0.12f, 0.07f, 0.03f, 0.98f); // Inset Box
        private static readonly Color CheckmarkGold     = new Color(1.00f, 0.82f, 0.35f, 1.00f); // Vibrant Gold Check
        private static readonly Color ButtonCloseRed    = new Color(0.70f, 0.16f, 0.14f, 0.98f); // Close Red
        // ============================================================================
        // [END] RAFT NATIVE HIGH-CONTRAST TIMBER PALETTE
        // ============================================================================
        #endregion

        // Tabs Management
        private const int TAB_COUNT = 4;
        private GameObject[] _tabPages = new GameObject[TAB_COUNT];
        private Image[] _tabButtonImages = new Image[TAB_COUNT];
        private Outline[] _tabButtonOutlines = new Outline[TAB_COUNT];
        private Text[] _tabButtonTexts = new Text[TAB_COUNT];
        private int _activeTab = 0;
        private int _toggleCounter = 0;

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

            try
            {
                _gameFont = Font.CreateDynamicFontFromOSFont(new[] { "Segoe UI Semibold", "Segoe UI", "Arial", "Tahoma" }, 24);
            }
            catch { }

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

            _gameFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
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
                Instance.SelectTab(Instance._activeTab); // Ensure active tab highlights brightly!
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
            winRt.sizeDelta = new Vector2(1280, 760); // Spacious, expansive, breathing room!
            winRt.localScale = Vector3.one;          // 1:1 Pixel Sharpness!

            var winImg = _modWindowGO.AddComponent<Image>();
            winImg.color = WoodWindowBg;

            var winOutline = _modWindowGO.AddComponent<Outline>();
            winOutline.effectColor = WoodWindowBorder;
            winOutline.effectDistance = new Vector2(5, -5);

            // 1. Title Bar (Height: 58)
            var titleBar = CreateBox(_modWindowGO.transform, "TitleBar", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), Vector2.zero, new Vector2(0, 58), WoodTitleBar);
            CreateBox(titleBar.transform, "TitleAccent", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 3), WoodTrimAccent);

            var titleText = CreateText(titleBar.transform, "TitleText", $"🎒 <color=#FFD54F><b>INVENTORY MASTER</b></color> <size=15><color=#FFE082>v{PluginInfo.PLUGIN_VERSION}</color></size> — <size=15><color=#F5EADB>Quality-of-Life & Inventory Management</color></size>", 21, FontStyle.Bold, TextParchmentLight, TextAnchor.MiddleLeft);
            titleText.rectTransform.offsetMin = new Vector2(22, 0);
            titleText.rectTransform.offsetMax = new Vector2(-70, 0);

            // Close Button [✕]
            var closeBtnGO = new GameObject("Btn_Close");
            closeBtnGO.transform.SetParent(titleBar.transform, false);
            var closeRt = closeBtnGO.AddComponent<RectTransform>();
            closeRt.anchorMin = new Vector2(1, 0.5f);
            closeRt.anchorMax = new Vector2(1, 0.5f);
            closeRt.pivot = new Vector2(1, 0.5f);
            closeRt.sizeDelta = new Vector2(44, 38);
            closeRt.anchoredPosition = new Vector2(-12, 0);
            var closeImg = closeBtnGO.AddComponent<Image>();
            closeImg.color = ButtonCloseRed;
            var closeBtn = closeBtnGO.AddComponent<Button>();
            closeBtn.onClick.AddListener(() => ToggleWindow());
            var closeTxt = CreateText(closeBtnGO.transform, "Txt", "✕", 20, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
            FillParent(closeTxt.gameObject);

            // 2. Tabs Row (Height: 48)
            var tabRow = CreateBox(_modWindowGO.transform, "TabRow", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -64), new Vector2(-28, 48), Color.clear);
            var tabLayout = tabRow.AddComponent<HorizontalLayoutGroup>();
            tabLayout.spacing = 10;
            tabLayout.childForceExpandWidth = true;
            tabLayout.childForceExpandHeight = true;

            string[] tabNames = { "🎒 BACKPACK & ACTIONS", "✂️ SMART SPLIT & TRANSFER", "🧲 VACUUM PICKUP & REFILL", "🛡️ SAFETY & PROTECTION" };
            for (int i = 0; i < tabNames.Length; i++)
            {
                int index = i;
                var tabBtn = CreateTabButton(tabRow.transform, $"TabBtn_{i}", tabNames[i], () => SelectTab(index));
                _tabButtonImages[i] = tabBtn.GetComponent<Image>();
                _tabButtonOutlines[i] = tabBtn.GetComponent<Outline>();
                _tabButtonTexts[i] = tabBtn.GetComponentInChildren<Text>();
            }

            // Trim line under tabs
            CreateBox(_modWindowGO.transform, "TabTrimLine", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -116), new Vector2(-28, 3), WoodTrimAccent);

            // 3. Tab Content Area
            var contentArea = CreateBox(_modWindowGO.transform, "ContentArea", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, Color.clear);
            var cRt = contentArea.GetComponent<RectTransform>();
            cRt.offsetMin = new Vector2(20, 52);
            cRt.offsetMax = new Vector2(-20, -126);

            // Build individual tab pages
            _tabPages[0] = BuildBackpackTab(contentArea.transform);
            _tabPages[1] = BuildControlsTab(contentArea.transform);
            _tabPages[2] = BuildAutomationTab(contentArea.transform);
            _tabPages[3] = BuildSafetyTab(contentArea.transform);

            SelectTab(0);

            // 4. Fixed Hotkeys Footer Bar (Height: 48)
            var footerBar = CreateBox(_modWindowGO.transform, "FooterBar", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 48), WoodTitleBar);
            CreateBox(footerBar.transform, "FooterAccent", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), Vector2.zero, new Vector2(0, 2), WoodTrimAccent);
            CreateText(footerBar.transform, "FooterText", "<color=#FFD54F><b>Hotkeys:</b></color> <color=#FFFFFF>[F2]</color> Menu   |   <color=#FFFFFF>[Z]</color> Auto Sort   |   <color=#FFFFFF>[X]</color> Dump to Chest   |   <color=#FFFFFF>[V]</color> Hotbar Swap   |   <color=#FFFFFF>[Alt+Click]</color> Lock Slot   |   <color=#FFFFFF>[ESC]</color> Close", 14, FontStyle.Normal, TextParchmentLight, TextAnchor.MiddleCenter);
        }

        private GameObject CreateTabButton(Transform parent, string name, string label, Action onClick)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var img = go.AddComponent<Image>();
            img.color = TabInactiveBg;

            var outline = go.AddComponent<Outline>();
            outline.effectColor = TabInactiveBorder;
            outline.effectDistance = new Vector2(2, -2);

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.transition = Selectable.Transition.None; // Prevent Unity EventSystem from breaking tab colors!
            if (onClick != null) btn.onClick.AddListener(() => onClick());

            var textGO = new GameObject("Text");
            textGO.transform.SetParent(go.transform, false);
            var textRt = textGO.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            var t = textGO.AddComponent<Text>();
            t.font = GetGameFont();
            t.text = label;
            t.fontSize = 15;
            t.fontStyle = FontStyle.Normal;
            t.color = TabInactiveText;
            t.alignment = TextAnchor.MiddleCenter;
            t.supportRichText = true;

            return go;
        }

        private void SelectTab(int index)
        {
            _activeTab = index;
            for (int i = 0; i < TAB_COUNT; i++)
            {
                bool isActive = (i == index);
                if (_tabPages[i] != null) _tabPages[i].SetActive(isActive);

                if (_tabButtonImages[i] != null)
                {
                    _tabButtonImages[i].color = isActive ? TabActiveBg : TabInactiveBg;
                }
                if (_tabButtonOutlines[i] != null)
                {
                    _tabButtonOutlines[i].effectColor = isActive ? TabActiveBorder : TabInactiveBorder;
                    _tabButtonOutlines[i].effectDistance = isActive ? new Vector2(2.5f, -2.5f) : new Vector2(1.5f, -1.5f);
                }
                if (_tabButtonTexts[i] != null)
                {
                    // ACTIVE TAB GLOWS IN BRIGHT GOLD WITH BOLD TEXT!
                    _tabButtonTexts[i].color = isActive ? TabActiveText : TabInactiveText;
                    _tabButtonTexts[i].fontStyle = isActive ? FontStyle.Bold : FontStyle.Normal;
                }
            }
        }

        #region [START] TAB PAGES BUILDERS
        private GameObject BuildBackpackTab(Transform parent)
        {
            var page = CreateTabPage(parent, "Page_Backpack");

            CreateSectionBanner(page.transform, "⚡ INSTANT INVENTORY ACTIONS");

            // Row 1: Sort Backpack & Sort Chest
            CreateDualActionTiles(page.transform,
                "⚡ Auto Sort Backpack", "[Z] KEY", () => InventorySorter.SortCurrentInventory(),
                "⚡ Auto Sort Open Chest", "[Z] KEY", () => InventorySorter.SortCurrentInventory());

            // Row 2: Dump Backpack & Swap Hotbar
            CreateDualActionTiles(page.transform,
                "📥 Dump Backpack to Chest", "[X] KEY", () => StorageDumpManager.DumpBackpackToOpenStorage(),
                "🔄 Swap Hotbar with Row 1", "[V] KEY", () => HotbarExpansionManager.SwapHotbarRow());

            CreateSectionBanner(page.transform, "🎒 BACKPACK EXPANSION & CAPACITY");

            CreateToggleItem(page.transform, "Permanent Backpack Slots Unlock (All 15 slots permanently active)", Plugin.EnableInventoryExpansion.Value, val => Plugin.EnableInventoryExpansion.Value = val);

            // Info Plaque with ample breathing room
            var infoBox = CreateBox(page.transform, "BackpackInfo", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 80), WoodPlankEven);
            EnsureLayout(infoBox, -1, 80);
            var infoText = CreateText(infoBox.transform, "InfoTxt", "• <color=#FFD54F><b>Hotbar Row Swap [V]:</b></color> Instantly swaps your 10 active hotbar slots with Row 1 of your backpack.\n• <color=#FFD54F><b>Auto Sort [Z]:</b></color> Automatically merges partial stacks and neatly categorizes items by Tools, Equipment, Food, and Resources.", 14, FontStyle.Normal, TextParchmentLight, TextAnchor.MiddleLeft);
            infoText.lineSpacing = 1.30f;
            infoText.rectTransform.offsetMin = new Vector2(18, 0);

            return page;
        }

        private GameObject BuildControlsTab(Transform parent)
        {
            var page = CreateTabPage(parent, "Page_Controls");

            CreateSectionBanner(page.transform, "✂️ SMART ITEM SPLITTING & BULK TRANSFERS");

            CreateToggleItem(page.transform, "Smart Item Split (Shift+RightClick = 1 Item, Ctrl+RightClick = Half Stack)", Plugin.EnableSmartSplit.Value, val => Plugin.EnableSmartSplit.Value = val);
            CreateToggleItem(page.transform, "Fast Bulk Item Transfer (Double-Click item in chest/backpack to move all stacks)", Plugin.EnableFastItemTransfer.Value, val => Plugin.EnableFastItemTransfer.Value = val);

            CreateSectionBanner(page.transform, "📖 HOW TO USE INVENTORY CONTROLS");

            var infoBox = CreateBox(page.transform, "ControlsInfo", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 115), WoodPlankEven);
            EnsureLayout(infoBox, -1, 115);
            var infoText = CreateText(infoBox.transform, "ControlsInfoText", "• <color=#FFD54F><b>Shift + Right Click:</b></color> Takes exactly <b>1 item</b> from stack into next available empty slot.\n• <color=#FFD54F><b>Ctrl + Right Click:</b></color> Splits exactly <b>half the stack</b> into next available empty slot.\n• <color=#FFD54F><b>Double Left-Click:</b></color> When interacting with a storage chest, double-clicking any item instantly transfers <b>all matching stacks</b> between your backpack and chest!", 14, FontStyle.Normal, TextParchmentLight, TextAnchor.MiddleLeft);
            infoText.lineSpacing = 1.30f;
            infoText.rectTransform.offsetMin = new Vector2(18, 0);

            return page;
        }

        private GameObject BuildAutomationTab(Transform parent)
        {
            var page = CreateTabPage(parent, "Page_Automation");

            CreateSectionBanner(page.transform, "🧲 VACUUM PICKUP & CONTEXT TOOL SWITCHING");

            CreateToggleItem(page.transform, "Auto Pickup Nearby Debris & Loose Items (Collects directly into pocket)", Plugin.EnableAutoPickup.Value, val => Plugin.EnableAutoPickup.Value = val);
            CreateStepperItem(page.transform, "Auto Pickup Radius", 2f, 12f, 1f, Plugin.AutoPickupRadius.Value, "m", val => Plugin.AutoPickupRadius.Value = val);
            CreateToggleItem(page.transform, "Auto Equip Tools on Context (Axe on tree, Hook on debris) & Auto-Replace Broken", Plugin.EnableAutoToolEquip.Value, val => Plugin.EnableAutoToolEquip.Value = val);

            CreateSectionBanner(page.transform, "💧 AUTO REFILL VITAL CONSUMABLES");

            CreateToggleItem(page.transform, "Auto Refill Fresh Water & Cooked Food (Strictly avoids saltwater & raw food)", Plugin.EnableAutoRefill.Value, val => Plugin.EnableAutoRefill.Value = val);
            CreateStepperItem(page.transform, "Refill Stats Threshold", 0.1f, 0.5f, 0.05f, Plugin.AutoRefillThreshold.Value, "%", val => Plugin.AutoRefillThreshold.Value = val);

            return page;
        }

        private GameObject BuildSafetyTab(Transform parent)
        {
            var page = CreateTabPage(parent, "Page_Safety");

            CreateSectionBanner(page.transform, "🛡️ ACCIDENTAL DROP PROTECTION & SAFETY");

            CreateToggleItem(page.transform, "Enable Drop Protection (Blocks accidental Q drops; hold Shift+Q to drop)", Plugin.EnableDropProtection.Value, val => Plugin.EnableDropProtection.Value = val);
            CreateToggleItem(page.transform, "Strict Drop Guard for Weapons, Tools & Armor", Plugin.ProtectToolsAndEquipment.Value, val => Plugin.ProtectToolsAndEquipment.Value = val);

            CreateDualActionTiles(page.transform,
                "🔓 Clear All Favorite Item Locks", "RESET", () =>
                {
                    FavoriteLockManager.ClearAll();
                    ToastManager.Show("🔓 All slot locks have been cleared!");
                },
                "↩️ Restore Last Trashed Item", "UNDO", () => TrashSlotManager.UndoTrash());

            CreateSectionBanner(page.transform, "📖 FAVORITE LOCKS & TRASH GUIDE");

            var infoBox = CreateBox(page.transform, "InfoBox", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 95), WoodPlankEven);
            EnsureLayout(infoBox, -1, 95);
            var infoText = CreateText(infoBox.transform, "InfoText", "• <color=#FFD54F><b>Favorite Item Lock:</b></color> Press <b>Alt + Left Click</b> on any slot to toggle lock (renders 🔒 badge; immune to sort/dump/drop).\n• <color=#FFD54F><b>Trash Slot:</b></color> Hover over unwanted items and press <b>Delete</b> to safely incinerate them.\n• <color=#FFD54F><b>Undo Buffer:</b></color> Accidentally trashed an item? Click the Undo button above to immediately recover it!", 14, FontStyle.Normal, TextParchmentLight, TextAnchor.MiddleLeft);
            infoText.lineSpacing = 1.30f;
            infoText.rectTransform.offsetMin = new Vector2(18, 0);

            return page;
        }
        #endregion

        #region [START] UI COMPONENT HELPERS
        private GameObject CreateTabPage(Transform parent, string name)
        {
            var page = CreateBox(parent, name, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, Color.clear);
            var layout = page.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10; // Comfortable 10px gap between sections!
            layout.padding = new RectOffset(12, 12, 8, 8);
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = true;
            layout.childControlHeight = true; // Control child heights cleanly!
            return page;
        }

        private void CreateSectionBanner(Transform parent, string title)
        {
            var banner = CreateBox(parent, "Banner", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 34), WoodTitleBar);
            EnsureLayout(banner, -1, 34);

            CreateBox(banner.transform, "TopTrim", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), Vector2.zero, new Vector2(0, 1.5f), WoodTrimAccent);
            CreateBox(banner.transform, "BotTrim", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 1.5f), WoodTrimAccent);

            CreateText(banner.transform, "Txt", $"─── {title} ───", 14, FontStyle.Bold, TextGoldHeading, TextAnchor.MiddleCenter);
        }

        private void CreateDualActionTiles(Transform parent, string title1, string hotkey1, Action action1, string title2, string hotkey2, Action action2)
        {
            var row = CreateBox(parent, "DualActionRow", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 54), Color.clear);
            EnsureLayout(row, -1, 54);

            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 14;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            layout.childControlWidth = true;
            layout.childControlHeight = true;

            CreateActionTile(row.transform, "Tile1", title1, hotkey1, action1);
            CreateActionTile(row.transform, "Tile2", title2, hotkey2, action2);
        }

        private GameObject CreateActionTile(Transform parent, string name, string title, string hotkey, Action onClick)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var img = go.AddComponent<Image>();
            img.color = ActionTileBg;

            var outline = go.AddComponent<Outline>();
            outline.effectColor = ActionTileBorder;
            outline.effectDistance = new Vector2(2, -2);

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            var cb = btn.colors;
            cb.normalColor = ActionTileBg;
            cb.highlightedColor = ActionTileHover;
            cb.pressedColor = new Color(0.20f, 0.12f, 0.06f, 1.0f);
            cb.selectedColor = ActionTileBg;
            btn.colors = cb;

            if (onClick != null) btn.onClick.AddListener(() => onClick());

            // Inner Layout: Title on Left, Hotkey Badge on Right
            var innerLayout = go.AddComponent<HorizontalLayoutGroup>();
            innerLayout.padding = new RectOffset(18, 14, 4, 4);
            innerLayout.spacing = 10;
            innerLayout.childForceExpandWidth = false;
            innerLayout.childForceExpandHeight = true;
            innerLayout.childControlWidth = true;  // Ensure title gets controlled width!
            innerLayout.childControlHeight = true;

            // Title Text (Full width, single line, no wrapping!)
            var titleGO = new GameObject("Title");
            titleGO.transform.SetParent(go.transform, false);
            var titleTxt = titleGO.AddComponent<Text>();
            titleTxt.font = GetGameFont();
            titleTxt.text = title;
            titleTxt.fontSize = 15;
            titleTxt.fontStyle = FontStyle.Bold;
            titleTxt.color = TextWhite;
            titleTxt.alignment = TextAnchor.MiddleLeft;
            titleTxt.supportRichText = true;
            titleTxt.horizontalOverflow = HorizontalWrapMode.Overflow; // No wrapping!
            titleTxt.verticalOverflow = VerticalWrapMode.Truncate;

            var titleLe = titleGO.AddComponent<LayoutElement>();
            titleLe.flexibleWidth = 1f;

            // Hotkey Badge Chip
            var chipGO = new GameObject("HotkeyChip");
            chipGO.transform.SetParent(go.transform, false);
            var chipLe = chipGO.AddComponent<LayoutElement>();
            chipLe.preferredWidth = 82;
            chipLe.preferredHeight = 34;
            chipLe.flexibleWidth = 0f;

            var chipImg = chipGO.AddComponent<Image>();
            chipImg.color = new Color(0.12f, 0.07f, 0.03f, 0.95f);
            var chipOutline = chipGO.AddComponent<Outline>();
            chipOutline.effectColor = new Color(1.0f, 0.82f, 0.35f, 0.8f);
            chipOutline.effectDistance = new Vector2(1, -1);

            var chipTxtGO = new GameObject("ChipText");
            chipTxtGO.transform.SetParent(chipGO.transform, false);
            var chipTxtRt = chipTxtGO.AddComponent<RectTransform>();
            chipTxtRt.anchorMin = Vector2.zero;
            chipTxtRt.anchorMax = Vector2.one;
            chipTxtRt.offsetMin = Vector2.zero;
            chipTxtRt.offsetMax = Vector2.zero;

            var chipTxt = chipTxtGO.AddComponent<Text>();
            chipTxt.font = GetGameFont();
            chipTxt.text = hotkey;
            chipTxt.fontSize = 12;
            chipTxt.fontStyle = FontStyle.Bold;
            chipTxt.color = TextGoldHeading;
            chipTxt.alignment = TextAnchor.MiddleCenter;

            return go;
        }

        private void CreateToggleItem(Transform parent, string label, bool initialValue, Action<bool> onToggle)
        {
            Color plankColor = (_toggleCounter++ % 2 == 0) ? WoodPlankEven : WoodPlankOdd;
            var row = CreateBox(parent, "ToggleRow", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 44), plankColor);
            EnsureLayout(row, -1, 44);

            CreateBox(row.transform, "Seam", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 1), WoodRowBorder);

            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 14;
            layout.padding = new RectOffset(18, 18, 4, 4);
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;
            layout.childControlHeight = true;
            layout.childControlWidth = true; // Control width so label stretches and pushes checkbox to the right!

            // Label on the Left taking all available width
            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(row.transform, false);
            var labelTxt = labelGO.AddComponent<Text>();
            labelTxt.font = GetGameFont();
            labelTxt.text = label;
            labelTxt.fontSize = 15;
            labelTxt.fontStyle = FontStyle.Normal;
            labelTxt.color = TextParchmentLight;
            labelTxt.alignment = TextAnchor.MiddleLeft;
            labelTxt.supportRichText = true;
            labelTxt.horizontalOverflow = HorizontalWrapMode.Overflow;

            var labelLe = labelGO.AddComponent<LayoutElement>();
            labelLe.flexibleWidth = 1f;

            // Checkbox on the Far Right
            var checkContainer = CreateBox(row.transform, "CheckContainer", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(32, 32), CheckboxWoodBg);
            var checkLe = checkContainer.AddComponent<LayoutElement>();
            checkLe.preferredWidth = 32;
            checkLe.preferredHeight = 32;
            checkLe.flexibleWidth = 0f;

            var checkOutline = checkContainer.AddComponent<Outline>();
            checkOutline.effectColor = new Color(0.92f, 0.74f, 0.38f, 0.85f);
            checkOutline.effectDistance = new Vector2(1.5f, -1.5f);

            var checkTxt = CreateText(checkContainer.transform, "Checkmark", initialValue ? "✔" : "", 19, FontStyle.Bold, CheckmarkGold, TextAnchor.MiddleCenter);

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
            var row = CreateBox(parent, "StepperRow", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 44), plankColor);
            EnsureLayout(row, -1, 44);

            CreateBox(row.transform, "Seam", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 1), WoodRowBorder);

            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 14;
            layout.padding = new RectOffset(18, 18, 4, 4);
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;
            layout.childControlHeight = true;
            layout.childControlWidth = true;

            float val = initialValue;
            string displayVal = unit == "%" ? $"{Mathf.RoundToInt(val * 100)}%" : $"{val:F0}{unit}";

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(row.transform, false);
            var labelTxt = labelGO.AddComponent<Text>();
            labelTxt.font = GetGameFont();
            labelTxt.text = $"{label}: <color=#FFD54F><b>{displayVal}</b></color>";
            labelTxt.fontSize = 15;
            labelTxt.fontStyle = FontStyle.Normal;
            labelTxt.color = TextParchmentLight;
            labelTxt.alignment = TextAnchor.MiddleLeft;
            labelTxt.supportRichText = true;

            var labelLe = labelGO.AddComponent<LayoutElement>();
            labelLe.flexibleWidth = 1f;

            var minusBtn = CreateButton(row.transform, "Minus", "  －  ", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(48, 34), () =>
            {
                val = Mathf.Clamp(val - step, min, max);
                displayVal = unit == "%" ? $"{Mathf.RoundToInt(val * 100)}%" : $"{val:F0}{unit}";
                labelTxt.text = $"{label}: <color=#FFD54F><b>{displayVal}</b></color>";
                onChange?.Invoke(val);
            }, ActionTileBg, TextWhite, 15);
            var minusLe = minusBtn.AddComponent<LayoutElement>();
            minusLe.preferredWidth = 48;
            minusLe.preferredHeight = 34;
            minusLe.flexibleWidth = 0f;

            var plusBtn = CreateButton(row.transform, "Plus", "  ＋  ", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(48, 34), () =>
            {
                val = Mathf.Clamp(val + step, min, max);
                displayVal = unit == "%" ? $"{Mathf.RoundToInt(val * 100)}%" : $"{val:F0}{unit}";
                labelTxt.text = $"{label}: <color=#FFD54F><b>{displayVal}</b></color>";
                onChange?.Invoke(val);
            }, ActionTileBg, TextWhite, 15);
            var plusLe = plusBtn.AddComponent<LayoutElement>();
            plusLe.preferredWidth = 48;
            plusLe.preferredHeight = 34;
            plusLe.flexibleWidth = 0f;
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

            var outline = go.AddComponent<Outline>();
            outline.effectColor = ActionTileBorder * 0.7f;
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            var cb = btn.colors;
            cb.normalColor = bgColor;
            cb.highlightedColor = ActionTileHover;
            cb.pressedColor = WoodWindowBorder;
            cb.selectedColor = bgColor;
            btn.colors = cb;

            if (onClick != null) btn.onClick.AddListener(() => onClick());

            var textGO = new GameObject("Text");
            textGO.transform.SetParent(go.transform, false);
            var textRt = textGO.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

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
            le.flexibleHeight = 0f; // Strictly prevent vertical stretching into giant voids!
            return le;
        }

        private void FillParent(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        #endregion
    }
    // ============================================================================
    // [END] CANVAS INVENTORY MASTER SETTINGS UI
    // ============================================================================
    #endregion
}
