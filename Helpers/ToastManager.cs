using System;
using UnityEngine;

namespace InventoryMaster.Helpers
{
    #region [START] TOAST NOTIFICATION ENGINE
    // ============================================================================
    // [START] TOAST NOTIFICATION ENGINE
    // Description: Provides smooth, stylized on-screen notifications for inventory events.
    // ============================================================================
    public class ToastManager : MonoBehaviour
    {
        public static ToastManager Instance { get; private set; }

        private static string _message = "";
        private static float _displayTimer = 0f;
        private const float TOAST_DURATION = 3.2f;

        private GUIStyle _boxStyle;
        private GUIStyle _textStyle;
        private Texture2D _bgTexture;

        private void Awake()
        {
            Instance = this;
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
        }

        public static void Show(string message)
        {
            _message = message;
            _displayTimer = TOAST_DURATION;
        }

        private void Update()
        {
            if (_displayTimer > 0f)
            {
                _displayTimer -= Time.unscaledDeltaTime;
                if (_displayTimer <= 0f)
                {
                    _message = "";
                }
            }
        }

        private void InitStyles()
        {
            if (_bgTexture == null)
            {
                _bgTexture = new Texture2D(1, 1);
                _bgTexture.SetPixel(0, 0, new Color(0.08f, 0.12f, 0.18f, 0.92f));
                _bgTexture.Apply();
            }

            if (_boxStyle == null)
            {
                _boxStyle = new GUIStyle();
                _boxStyle.normal.background = _bgTexture;
                _boxStyle.padding = new RectOffset(16, 16, 8, 8);
                _boxStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (_textStyle == null)
            {
                _textStyle = new GUIStyle();
                _textStyle.fontSize = 15;
                _textStyle.fontStyle = FontStyle.Bold;
                _textStyle.normal.textColor = new Color(0.92f, 0.96f, 1f, 1f);
                _textStyle.alignment = TextAnchor.MiddleCenter;
            }
        }

        private void OnGUI()
        {
            if (_displayTimer <= 0f || string.IsNullOrEmpty(_message)) return;

            InitStyles();

            float alpha = Mathf.Clamp01(_displayTimer / 0.5f);
            Color prevColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, alpha);

            GUIContent content = new GUIContent(_message);
            Vector2 size = _textStyle.CalcSize(content);
            float width = Mathf.Max(size.x + 36f, 260f);
            float height = size.y + 18f;

            float x = (Screen.width - width) * 0.5f;
            float y = Screen.height * 0.18f;

            Rect rect = new Rect(x, y, width, height);
            GUI.Box(rect, GUIContent.none, _boxStyle);
            GUI.Label(rect, _message, _textStyle);

            GUI.color = prevColor;
        }
    }
    // ============================================================================
    // [END] TOAST NOTIFICATION ENGINE
    // ============================================================================
    #endregion
}
