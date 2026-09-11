using System;
using UnityEngine;
using UnityEngine.UI;
using InventoryMaster.Helpers;
using InventoryMaster.Features;
using InventoryMaster.Patches;

namespace InventoryMaster.UI
{
    #region [START] CANVAS INVENTORY MASTER SETTINGS UI (AAA STUDIO OVERHAUL)
    // ============================================================================
    // [START] CANVAS INVENTORY MASTER SETTINGS UI (AAA STUDIO OVERHAUL)
    // Purpose: Balanced (1140x630), razor-sharp in-game settings canvas for Inventory Master.
    //          Zero empty voids, rich multi-column cards, tactile oak action tiles with
    //          golden hotkey badges, and authentic Raft timber & brass styling.
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
        private static readonly Color WoodWindowBg      = new Color(0.20f, 0.12f, 0.07f, 0.99f); // Deep Teak Plank #331F12
        private static readonly Color WoodWindowBorder  = new Color(0.10f, 0.05f, 0.02f, 1.00f); // Dark Timber Outline #1A0D05
        private static readonly Color WoodTitleBar      = new Color(0.16f, 0.09f, 0.04f, 1.00f); // Header Bar #29170A
        private static readonly Color WoodTrimAccent    = new Color(0.92f, 0.74f, 0.38f, 1.00f); // Golden Wood Trim #EBB861

        // Tab Colors (Bright Warm Birch Parchment for Active, Deep Timber for Inactive)
        private static readonly Color TabActiveBg       = new Color(0.88f, 0.74f, 0.48f, 1.00f); // Bright Warm Birch Parchment #E0BD7A
        private static readonly Color TabActiveBorder   = new Color(1.00f, 0.85f, 0.40f, 1.00f); // Glowing Gold Tab Rim #FFD966
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
        private static readonly Color TextGoldHeading    = new Color(1.00f, 0.82f, 0.35f, 1.00f); // Vibrant Gold #FFD159
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
                try
                {
                    var cic = CustomInputConfig.Instance;
                    if (cic != null)
                    {
                        cic.EnableInput();
                        cic.SwitchCurrentActionMap("UI");
                    }
                }
                catch { }

                Helper.SetCursorVisibleAndLockState(true, CursorLockMode.None);
                Instance.SelectTab(Instance._activeTab);
            }
            else
            {
                bool peerModOpen = CursorPatchHelper.ShouldForceCursorFree();
                // Only gameplay has a "Player" action map / locked cursor to return to -
                // forcing those from the main menu (no player/world loaded) left the home
                // screen's own UI buttons unable to receive clicks until restarting the game.
                bool isInGame = ComponentManager<Raft>.Value != null || ComponentManager<Network_Player>.Value != null;

                try
                {
                    var cic = CustomInputConfig.Instance;
                    if (cic != null && !peerModOpen)
                    {
                        cic.SwitchCurrentActionMap(isInGame ? "Player" : "UI");
                    }
                }
                catch { }

                if (!peerModOpen)
                {
                    try
                    {
                        if (isInGame)
                        {
                            Helper.SetCursorVisibleAndLockState(false, CursorLockMode.Locked);
                        }
                        else
                        {
                            Helper.SetCursorVisibleAndLockState(true, CursorLockMode.None);
                        }
                    }
                    catch { }
                }
            }
        }

        private void Update()
        {
            if ((Plugin.KeyMenu != null && InputHelper.WasKeyPressed(Plugin.KeyMenu.Value)) || InputHelper.WasKeyPressed(KeyCode.F2))
            {
                ToggleWindow();
            }

            if (IsWindowOpen && InputHelper.WasKeyPressed(KeyCode.Escape))
            {
                ToggleWindow();
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
            // Proportionate studio dimensions: 1140x630 fills the screen symmetrically with zero barren void!
            winRt.sizeDelta = new Vector2(1140, 630);
            winRt.localScale = Vector3.one;

            var winImg = _modWindowGO.AddComponent<Image>();
            winImg.color = WoodWindowBg;

            var winOutline = _modWindowGO.AddComponent<Outline>();
            winOutline.effectColor = WoodWindowBorder;
            winOutline.effectDistance = new Vector2(5, -5);

            // 1. Title Bar (Height: 52)
            var titleBar = CreateBox(_modWindowGO.transform, "TitleBar", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), Vector2.zero, new Vector2(0, 52), WoodTitleBar);
            CreateBox(titleBar.transform, "TitleAccent", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 3), WoodTrimAccent);

            var titleText = CreateText(titleBar.transform, "TitleText", $"🎒 <color=#FFD54F><b>INVENTORY MASTER</b></color> <size=14><color=#FFE082>v{PluginInfo.PLUGIN_VERSION}</color></size> — <size=13><color=#F5EADB>Quality-of-Life & Inventory Management [F2]</color></size>", 20, FontStyle.Bold, TextParchmentLight, TextAnchor.MiddleLeft);
            titleText.rectTransform.offsetMin = new Vector2(20, 0);
            titleText.rectTransform.offsetMax = new Vector2(-70, 0);

            // Close Button [✕]
            var closeBtnGO = new GameObject("Btn_Close");
            closeBtnGO.transform.SetParent(titleBar.transform, false);
            var closeRt = closeBtnGO.AddComponent<RectTransform>();
            closeRt.anchorMin = new Vector2(1, 0.5f);
            closeRt.anchorMax = new Vector2(1, 0.5f);
            closeRt.pivot = new Vector2(1, 0.5f);
            closeRt.sizeDelta = new Vector2(40, 36);
            closeRt.anchoredPosition = new Vector2(-12, 0);
            var closeImg = closeBtnGO.AddComponent<Image>();
            closeImg.color = ButtonCloseRed;
            var closeBtn = closeBtnGO.AddComponent<Button>();
            closeBtn.onClick.AddListener(() => ToggleWindow());
            var closeTxt = CreateText(closeBtnGO.transform, "Txt", "✕", 18, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
            FillParent(closeTxt.gameObject);

            // 2. Tabs Row (Height: 44)
            var tabRow = CreateBox(_modWindowGO.transform, "TabRow", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -58), new Vector2(-24, 44), Color.clear);
            var tabLayout = tabRow.AddComponent<HorizontalLayoutGroup>();
            tabLayout.spacing = 8;
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
            CreateBox(_modWindowGO.transform, "TabTrimLine", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -106), new Vector2(-24, 3), WoodTrimAccent);

            // 3. Tab Content Area
            var contentArea = CreateBox(_modWindowGO.transform, "ContentArea", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, Color.clear);
            var cRt = contentArea.GetComponent<RectTransform>();
            cRt.offsetMin = new Vector2(18, 48);
            cRt.offsetMax = new Vector2(-18, -114);

            // Build individual tab pages
            _tabPages[0] = BuildBackpackTab(contentArea.transform);
            _tabPages[1] = BuildControlsTab(contentArea.transform);
            _tabPages[2] = BuildAutomationTab(contentArea.transform);
            _tabPages[3] = BuildSafetyTab(contentArea.transform);

            SelectTab(0);

            // 4. Fixed Hotkeys Footer Bar (Height: 44)
            var footerBar = CreateBox(_modWindowGO.transform, "FooterBar", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 44), WoodTitleBar);
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
            btn.transition = Selectable.Transition.None;
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
            t.fontSize = 14;
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

            CreateSectionBanner(page.transform, "📖 QUICK REFERENCE & GAMEPLAY GUIDE");

            // Rich Dual-Column Guide Plaque that fills the bottom gracefully
            var guideRow = CreateBox(page.transform, "GuideRow", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 130), Color.clear);
            EnsureLayout(guideRow, -1, 130);
            var gLayout = guideRow.AddComponent<HorizontalLayoutGroup>();
            gLayout.spacing = 10;
            gLayout.childForceExpandWidth = true;
            gLayout.childForceExpandHeight = true;

            var cardLeft = CreateBox(guideRow.transform, "CardLeft", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, WoodPlankEven);
            var cLOutline = cardLeft.AddComponent<Outline>();
            cLOutline.effectColor = WoodRowBorder;
            cLOutline.effectDistance = new Vector2(1, -1);
            var lLayout = cardLeft.AddComponent<VerticalLayoutGroup>();
            lLayout.padding = new RectOffset(16, 16, 12, 12);
            lLayout.spacing = 6;
            var leftTitle = CreateText(cardLeft.transform, "T", "⚡ <b>Smart Sorting & Storage</b>", 15, FontStyle.Bold, TextGoldHeading, TextAnchor.MiddleLeft);
            EnsureLayout(leftTitle.gameObject, -1, 22);
            var leftDesc = CreateText(cardLeft.transform, "D", "• Press <b>[Z]</b> anytime to auto-sort your backpack or active storage container.\n• Automatically merges fragmented stacks and categorizes by tools, food, resources.\n• Press <b>[X]</b> while a chest is open to dump matching items into storage instantly.", 13, FontStyle.Normal, TextParchmentLight, TextAnchor.UpperLeft);
            leftDesc.lineSpacing = 1.3f;
            EnsureLayout(leftDesc.gameObject, -1, 75);

            var cardRight = CreateBox(guideRow.transform, "CardRight", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, WoodPlankEven);
            var cROutline = cardRight.AddComponent<Outline>();
            cROutline.effectColor = WoodRowBorder;
            cROutline.effectDistance = new Vector2(1, -1);
            var rLayout = cardRight.AddComponent<VerticalLayoutGroup>();
            rLayout.padding = new RectOffset(16, 16, 12, 12);
            rLayout.spacing = 6;
            var rightTitle = CreateText(cardRight.transform, "T", "🔄 <b>Hotbar Row Swapping</b>", 15, FontStyle.Bold, TextGoldHeading, TextAnchor.MiddleLeft);
            EnsureLayout(rightTitle.gameObject, -1, 22);
            var rightDesc = CreateText(cardRight.transform, "D", "• Press <b>[V]</b> to rotate your active 10 hotbar slots with Row 1 of your backpack.\n• Gives you instant access to 20 total active tools and items without opening menus.\n• Safe & atomic: slot locks, item durability, and food freshness are preserved.", 13, FontStyle.Normal, TextParchmentLight, TextAnchor.UpperLeft);
            rightDesc.lineSpacing = 1.3f;
            EnsureLayout(rightDesc.gameObject, -1, 75);

            return page;
        }

        private GameObject BuildControlsTab(Transform parent)
        {
            var page = CreateTabPage(parent, "Page_Controls");

            CreateSectionBanner(page.transform, "✂️ SMART ITEM SPLITTING & BULK TRANSFERS");

            CreateToggleItem(page.transform, "Smart Item Split (Shift+RightClick = 1 Item, Ctrl+RightClick = Half Stack)", Plugin.EnableSmartSplit.Value, val => Plugin.EnableSmartSplit.Value = val);
            CreateToggleItem(page.transform, "Fast Bulk Item Transfer (Double-Click item in chest/backpack to move all stacks)", Plugin.EnableFastItemTransfer.Value, val => Plugin.EnableFastItemTransfer.Value = val);

            CreateSectionBanner(page.transform, "📖 VISUAL HOTKEY CONTROLS REFERENCE");

            // 3-Tile Visual Cards Row that fully fills space cleanly
            var cardsRow = CreateBox(page.transform, "VisualCardsRow", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 190), Color.clear);
            EnsureLayout(cardsRow, -1, 190);
            var cLayout = cardsRow.AddComponent<HorizontalLayoutGroup>();
            cLayout.spacing = 10;
            cLayout.childForceExpandWidth = true;
            cLayout.childForceExpandHeight = true;

            BuildSplitCard(cardsRow.transform, "Card1", "Shift + Right-Click", "TAKE 1 ITEM", "Grabs exactly <b>one item</b> from any stack directly into your next available empty pocket slot. Essential for feeding animals or precise crafting!");
            BuildSplitCard(cardsRow.transform, "Card2", "Ctrl + Right-Click", "TAKE 50% (HALF)", "Splits exactly <b>half the stack</b> into the next open slot. Perfect for sharing materials with crewmates or loading smelters evenly!");
            BuildSplitCard(cardsRow.transform, "Card3", "Double Left-Click", "TRANSFER ALL", "When inspecting any storage chest, double-clicking an item transfers <b>all matching stacks</b> between your backpack and the container in one tap!");

            var noteBox = CreateBox(page.transform, "NoteBox", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 42), WoodTitleBar);
            EnsureLayout(noteBox, -1, 42);
            CreateBox(noteBox.transform, "Trim", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 1.5f), WoodTrimAccent);
            var noteTxt = CreateText(noteBox.transform, "Txt", "💡 <b>Pro Tip:</b> All split and transfer actions respect slot locks: items with a 🔒 badge are never transferred accidentally.", 14, FontStyle.Italic, TextParchmentLight, TextAnchor.MiddleCenter);

            return page;
        }

        private void BuildSplitCard(Transform parent, string name, string keyLabel, string title, string desc)
        {
            var card = CreateBox(parent, name, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, WoodPlankEven);
            var outline = card.AddComponent<Outline>();
            outline.effectColor = WoodTrimAccent * 0.7f;
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var layout = card.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 14, 14);
            layout.spacing = 8;
            layout.childForceExpandWidth = true;

            // Key Badge Pill
            var pill = CreateBox(card.transform, "Pill", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 28), CheckboxWoodBg);
            EnsureLayout(pill, -1, 28);
            var pOutline = pill.AddComponent<Outline>();
            pOutline.effectColor = TextGoldHeading;
            pOutline.effectDistance = new Vector2(1, -1);
            CreateText(pill.transform, "KeyTxt", keyLabel, 13, FontStyle.Bold, TextGoldHeading, TextAnchor.MiddleCenter);

            // Title
            var t = CreateText(card.transform, "Title", title, 15, FontStyle.Bold, TextWhite, TextAnchor.MiddleCenter);
            EnsureLayout(t.gameObject, -1, 22);

            // Description
            var d = CreateText(card.transform, "Desc", desc, 13, FontStyle.Normal, TextParchmentLight, TextAnchor.UpperLeft);
            d.lineSpacing = 1.25f;
            EnsureLayout(d.gameObject, -1, 80);
        }

        private GameObject BuildAutomationTab(Transform parent)
        {
            var page = CreateTabPage(parent, "Page_Automation");

            // 2-Column Balanced Dashboard
            var twoColGO = CreateBox(page.transform, "TwoColDashboard", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 430), Color.clear);
            EnsureLayout(twoColGO, -1, 430);
            var twoColLayout = twoColGO.AddComponent<HorizontalLayoutGroup>();
            twoColLayout.spacing = 12;
            twoColLayout.childForceExpandWidth = true;
            twoColLayout.childForceExpandHeight = true;

            // === LEFT COLUMN: VACUUM PICKUP ===
            var leftCol = CreateBox(twoColGO.transform, "LeftCol", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Color.clear);
            var lLayout = leftCol.AddComponent<VerticalLayoutGroup>();
            lLayout.spacing = 8;
            lLayout.childForceExpandWidth = true;

            CreateSectionBanner(leftCol.transform, "🧲 VACUUM PICKUP & TOOL SWITCH");
            CreateToggleItem(leftCol.transform, "Auto Pickup Nearby Debris & Loose Items", Plugin.EnableAutoPickup.Value, val => Plugin.EnableAutoPickup.Value = val);
            CreateStepperItem(leftCol.transform, "Auto Pickup Radius", 2f, 12f, 1f, Plugin.AutoPickupRadius.Value, "m", val => Plugin.AutoPickupRadius.Value = val);
            CreateToggleItem(leftCol.transform, "Auto Equip Tools on Context & Auto-Replace", Plugin.EnableAutoToolEquip.Value, val => Plugin.EnableAutoToolEquip.Value = val);

            var leftInfo = CreateBox(leftCol.transform, "LeftInfo", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 180), WoodPlankEven);
            EnsureLayout(leftInfo, -1, 180);
            var liOutline = leftInfo.AddComponent<Outline>();
            liOutline.effectColor = WoodRowBorder;
            liOutline.effectDistance = new Vector2(1, -1);
            var liLayout = leftInfo.AddComponent<VerticalLayoutGroup>();
            liLayout.padding = new RectOffset(16, 16, 12, 12);
            liLayout.spacing = 6;
            var liTitle = CreateText(leftInfo.transform, "T", "🧲 <b>Vacuum & Context Tools</b>", 15, FontStyle.Bold, TextGoldHeading, TextAnchor.MiddleLeft);
            EnsureLayout(liTitle.gameObject, -1, 22);
            var liDesc = CreateText(leftInfo.transform, "D", "• <b>Vacuum Magnet:</b> Automatically sweeps loose flotsam, crates, and dropped items directly into your inventory when within range.\n• <b>Context Tool Switching:</b> Instantly equips your Axe when targeting trees, or your Hook when aiming at ocean debris.\n• <b>Auto-Replace:</b> When a tool shatters from zero durability, an identical replacement from your backpack is instantly equipped in hand!", 13, FontStyle.Normal, TextParchmentLight, TextAnchor.UpperLeft);
            liDesc.lineSpacing = 1.3f;
            EnsureLayout(liDesc.gameObject, -1, 130);

            // === RIGHT COLUMN: AUTO-REFILL ===
            var rightCol = CreateBox(twoColGO.transform, "RightCol", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Color.clear);
            var rLayout = rightCol.AddComponent<VerticalLayoutGroup>();
            rLayout.spacing = 8;
            rLayout.childForceExpandWidth = true;

            CreateSectionBanner(rightCol.transform, "💧 AUTO REFILL VITAL CONSUMABLES");
            CreateToggleItem(rightCol.transform, "Auto Refill Fresh Water & Cooked Food", Plugin.EnableAutoRefill.Value, val => Plugin.EnableAutoRefill.Value = val);
            CreateStepperItem(rightCol.transform, "Refill Stats Threshold", 0.1f, 0.5f, 0.05f, Plugin.AutoRefillThreshold.Value, "%", val => Plugin.AutoRefillThreshold.Value = val);

            // Spacer item row to match left column toggle count
            var safetyNote = CreateBox(rightCol.transform, "SafetyBadge", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 42), WoodPlankOdd);
            EnsureLayout(safetyNote, -1, 42);
            CreateText(safetyNote.transform, "BadgeTxt", "🛡️ <b>Strict Safety Guard:</b> Saltwater & Raw Food Filter Active", 13, FontStyle.Bold, new Color(0.35f, 0.95f, 0.60f, 1.0f), TextAnchor.MiddleCenter);

            var rightInfo = CreateBox(rightCol.transform, "RightInfo", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 180), WoodPlankEven);
            EnsureLayout(rightInfo, -1, 180);
            var riOutline = rightInfo.AddComponent<Outline>();
            riOutline.effectColor = WoodRowBorder;
            riOutline.effectDistance = new Vector2(1, -1);
            var riLayout = rightInfo.AddComponent<VerticalLayoutGroup>();
            riLayout.padding = new RectOffset(16, 16, 12, 12);
            riLayout.spacing = 6;
            var riTitle = CreateText(rightInfo.transform, "T", "💧 <b>Consumable Auto-Refill Guard</b>", 15, FontStyle.Bold, TextGoldHeading, TextAnchor.MiddleLeft);
            EnsureLayout(riTitle.gameObject, -1, 22);
            var riDesc = CreateText(rightInfo.transform, "D", "• <b>Smart Hydration:</b> Automatically drinks clean purified water from bottles or cups in your backpack whenever thirst drops below the threshold.\n• <b>Nourishment:</b> Automatically consumes grilled fish, cooked meat, or hearty recipes when hunger runs low.\n• <b>Zero Risk:</b> Strictly prevents consuming saltwater, raw food, poisonous pufferfish, or raw shark meat.", 13, FontStyle.Normal, TextParchmentLight, TextAnchor.UpperLeft);
            riDesc.lineSpacing = 1.3f;
            EnsureLayout(riDesc.gameObject, -1, 130);

            return page;
        }

        private GameObject BuildSafetyTab(Transform parent)
        {
            var page = CreateTabPage(parent, "Page_Safety");

            CreateSectionBanner(page.transform, "🛡️ ACCIDENTAL DROP PROTECTION & SAFETY");

            CreateToggleItem(page.transform, "Enable Drop Protection (Blocks accidental Q drops; hold Shift+Q to drop)", Plugin.EnableDropProtection.Value, val => Plugin.EnableDropProtection.Value = val);
            CreateToggleItem(page.transform, "Strict Drop Guard for Weapons, Tools & Armor", Plugin.ProtectToolsAndEquipment.Value, val => Plugin.ProtectToolsAndEquipment.Value = val);

            CreateSectionBanner(page.transform, "⚡ QUICK ACTIONS & SLOT RECOVERY");

            CreateDualActionTiles(page.transform,
                "🔓 Clear All Favorite Item Locks", "RESET", () =>
                {
                    FavoriteLockManager.ClearAll();
                    ToastManager.Show("🔓 All slot locks have been cleared!");
                },
                "↩️ Restore Last Trashed Item", "UNDO", () => TrashSlotManager.UndoTrash());

            CreateSectionBanner(page.transform, "📖 FAVORITE LOCKS & TRASH GUIDE");

            var infoBox = CreateBox(page.transform, "InfoBox", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 130), WoodPlankEven);
            EnsureLayout(infoBox, -1, 130);
            var ibOutline = infoBox.AddComponent<Outline>();
            ibOutline.effectColor = WoodRowBorder;
            ibOutline.effectDistance = new Vector2(1, -1);
            var ibLayout = infoBox.AddComponent<VerticalLayoutGroup>();
            ibLayout.padding = new RectOffset(18, 18, 12, 12);
            ibLayout.spacing = 6;
            var infoTitle = CreateText(infoBox.transform, "Title", "🛡️ <b>Protecting Your Belongings</b>", 15, FontStyle.Bold, TextGoldHeading, TextAnchor.MiddleLeft);
            EnsureLayout(infoTitle.gameObject, -1, 22);
            var infoText = CreateText(infoBox.transform, "InfoText", "• <color=#FFD54F><b>Favorite Item Lock:</b></color> Press <b>Alt + Left Click</b> on any inventory slot to toggle a permanent lock (renders a golden 🔒 badge). Locked items can never be auto-sorted, dumped into chests, or dropped into ocean waters.\n• <color=#FFD54F><b>Quick Incinerator:</b></color> Hover over useless trash items and press <b>[Delete]</b> to immediately destroy them without cluttering the sea.\n• <color=#FFD54F><b>Instant Undo Buffer:</b></color> Accidentally deleted a valuable tool or blueprint? Click <b>'Restore Last Trashed Item'</b> above to recover it immediately!", 13, FontStyle.Normal, TextParchmentLight, TextAnchor.UpperLeft);
            infoText.lineSpacing = 1.30f;
            EnsureLayout(infoText.gameObject, -1, 80);

            CreateSectionBanner(page.transform, "🔄 MOD VERSION & UPDATES");

            var verRow = CreateBox(page.transform, "VerRow", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 40), WoodTitleBar);
            EnsureLayout(verRow, -1, 40);
            var verLayout = verRow.AddComponent<HorizontalLayoutGroup>();
            verLayout.padding = new RectOffset(16, 16, 3, 3);
            verLayout.spacing = 12;
            verLayout.childForceExpandHeight = true;

            CreateText(verRow.transform, "VerLabel", $"🎒 Inventory Master <b>v{PluginInfo.PLUGIN_VERSION}</b>", 14, FontStyle.Normal, TextParchmentLight, TextAnchor.MiddleLeft);

            CreateButton(verRow.transform, "Btn_CheckUpdates", "🔄 Check for Updates", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(180, 34), () =>
            {
                UpdateChecker.Dismissed = false;
                UpdateChecker.Instance?.TriggerCheck();
                ToastManager.Show("Checking GitHub for mod updates...");
            }, ActionTileBg, TextParchmentLight, 14);

            return page;
        }
        #endregion

        #region [START] UI COMPONENT HELPERS
        private GameObject CreateTabPage(Transform parent, string name)
        {
            var page = CreateBox(parent, name, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, Color.clear);
            var layout = page.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 8;
            layout.padding = new RectOffset(8, 8, 4, 4);
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            return page;
        }

        private void CreateSectionBanner(Transform parent, string title)
        {
            var banner = CreateBox(parent, "Banner", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 32), WoodTitleBar);
            EnsureLayout(banner, -1, 32);

            CreateBox(banner.transform, "TopTrim", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), Vector2.zero, new Vector2(0, 1.5f), WoodTrimAccent);
            CreateBox(banner.transform, "BotTrim", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 1.5f), WoodTrimAccent);

            CreateText(banner.transform, "Txt", $"─── {title} ───", 14, FontStyle.Bold, TextGoldHeading, TextAnchor.MiddleCenter);
        }

        private void CreateDualActionTiles(Transform parent, string title1, string hotkey1, Action action1, string title2, string hotkey2, Action action2)
        {
            var row = CreateBox(parent, "DualActionRow", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 50), Color.clear);
            EnsureLayout(row, -1, 50);

            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
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
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            var cb = btn.colors;
            cb.normalColor = ActionTileBg;
            cb.highlightedColor = ActionTileHover;
            cb.pressedColor = new Color(0.20f, 0.12f, 0.06f, 1.0f);
            cb.selectedColor = ActionTileBg;
            btn.colors = cb;

            if (onClick != null) btn.onClick.AddListener(() => onClick());

            var innerLayout = go.AddComponent<HorizontalLayoutGroup>();
            innerLayout.padding = new RectOffset(16, 12, 4, 4);
            innerLayout.spacing = 8;
            innerLayout.childForceExpandWidth = false;
            innerLayout.childForceExpandHeight = true;
            innerLayout.childControlWidth = true;
            innerLayout.childControlHeight = true;

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
            titleTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
            titleTxt.verticalOverflow = VerticalWrapMode.Truncate;

            var titleLe = titleGO.AddComponent<LayoutElement>();
            titleLe.flexibleWidth = 1f;

            var chipGO = new GameObject("HotkeyChip");
            chipGO.transform.SetParent(go.transform, false);
            var chipLe = chipGO.AddComponent<LayoutElement>();
            chipLe.preferredWidth = 80;
            chipLe.preferredHeight = 30;
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
            var row = CreateBox(parent, "ToggleRow", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 42), plankColor);
            EnsureLayout(row, -1, 42);

            CreateBox(row.transform, "Seam", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 1), WoodRowBorder);

            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 12;
            layout.padding = new RectOffset(16, 16, 4, 4);
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;
            layout.childControlHeight = true;
            layout.childControlWidth = true;

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(row.transform, false);
            var labelTxt = labelGO.AddComponent<Text>();
            labelTxt.font = GetGameFont();
            labelTxt.text = label;
            labelTxt.fontSize = 14;
            labelTxt.fontStyle = FontStyle.Normal;
            labelTxt.color = TextParchmentLight;
            labelTxt.alignment = TextAnchor.MiddleLeft;
            labelTxt.supportRichText = true;
            labelTxt.horizontalOverflow = HorizontalWrapMode.Overflow;

            var labelLe = labelGO.AddComponent<LayoutElement>();
            labelLe.flexibleWidth = 1f;

            var checkContainer = CreateBox(row.transform, "CheckContainer", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(58, 30), CheckboxWoodBg);
            var checkLe = checkContainer.AddComponent<LayoutElement>();
            checkLe.preferredWidth = 58;
            checkLe.preferredHeight = 30;
            checkLe.flexibleWidth = 0f;

            var checkOutline = checkContainer.AddComponent<Outline>();
            checkOutline.effectColor = new Color(0.92f, 0.74f, 0.38f, 0.85f);
            checkOutline.effectDistance = new Vector2(1.5f, -1.5f);

            var checkTxt = CreateText(checkContainer.transform, "Checkmark", initialValue ? "ON" : "OFF", 13, FontStyle.Bold, initialValue ? CheckmarkGold : Color.gray, TextAnchor.MiddleCenter);

            bool state = initialValue;
            var btn = checkContainer.AddComponent<Button>();
            btn.targetGraphic = checkContainer.GetComponent<Image>();

            void Toggle()
            {
                state = !state;
                checkTxt.text = state ? "ON" : "OFF";
                checkTxt.color = state ? CheckmarkGold : Color.gray;
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
            var row = CreateBox(parent, "StepperRow", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(0, 42), plankColor);
            EnsureLayout(row, -1, 42);

            CreateBox(row.transform, "Seam", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, 1), WoodRowBorder);

            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 12;
            layout.padding = new RectOffset(16, 16, 4, 4);
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
            labelTxt.fontSize = 14;
            labelTxt.fontStyle = FontStyle.Normal;
            labelTxt.color = TextParchmentLight;
            labelTxt.alignment = TextAnchor.MiddleLeft;
            labelTxt.supportRichText = true;

            var labelLe = labelGO.AddComponent<LayoutElement>();
            labelLe.flexibleWidth = 1f;

            var minusBtn = CreateButton(row.transform, "Minus", "  －  ", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(44, 32), () =>
            {
                val = Mathf.Clamp(val - step, min, max);
                displayVal = unit == "%" ? $"{Mathf.RoundToInt(val * 100)}%" : $"{val:F0}{unit}";
                labelTxt.text = $"{label}: <color=#FFD54F><b>{displayVal}</b></color>";
                onChange?.Invoke(val);
            }, ActionTileBg, TextWhite, 15);
            var minusLe = minusBtn.AddComponent<LayoutElement>();
            minusLe.preferredWidth = 44;
            minusLe.preferredHeight = 32;
            minusLe.flexibleWidth = 0f;

            var plusBtn = CreateButton(row.transform, "Plus", "  ＋  ", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(44, 32), () =>
            {
                val = Mathf.Clamp(val + step, min, max);
                displayVal = unit == "%" ? $"{Mathf.RoundToInt(val * 100)}%" : $"{val:F0}{unit}";
                labelTxt.text = $"{label}: <color=#FFD54F><b>{displayVal}</b></color>";
                onChange?.Invoke(val);
            }, ActionTileBg, TextWhite, 15);
            var plusLe = plusBtn.AddComponent<LayoutElement>();
            plusLe.preferredWidth = 44;
            plusLe.preferredHeight = 32;
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
            le.flexibleHeight = 0f;
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
