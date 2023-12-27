#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ABI.CCK.Scripts.Editor
{
    public static partial class EditorGUIExtensions
    {
        private static readonly int s_ProgressBarHashCode = "s_ProgressBarHash".GetHashCode();
        private static readonly GUIStyle s_BarBack = new GUIStyle("ProgressBarBack");
        private static readonly GUIStyle s_BarBarBlue = new GUIStyle("ProgressBarBar");
        private static readonly GUIStyle s_BarBarYellow;
        private static readonly GUIStyle s_BarBarRed;
        private static readonly GUIStyle s_BarText = new GUIStyle("ProgressBarText");

        static EditorGUIExtensions()
        {
            // Custom progress bar
            s_BarBarYellow = new GUIStyle(s_BarBarBlue);
            InitTextureForStyle(s_BarBarYellow, Color.yellow);
            s_BarBarRed = new GUIStyle(s_BarBarBlue);
            InitTextureForStyle(s_BarBarRed, Color.red);
        }

        public static void MultiProgressBar(Rect position, float value1, float value2, string text)
        {
            if (Event.current.GetTypeForControl(GUIUtility.GetControlID(s_ProgressBarHashCode, FocusType.Keyboard,
                    position)) != EventType.Repaint)
                return;

            s_BarBack.Draw(position, false, false, false, false);

            GUIStyle styleForValue1 = value1 > 1f ? s_BarBarRed : s_BarBarBlue;
            GUIStyle styleForValue2 = value2 > 1f ? s_BarBarRed : s_BarBarYellow;

            value1 = Mathf.Clamp01(value1);
            value2 = Mathf.Clamp01(value2);

            Rect position1 = new Rect(position);
            Rect position2 = new Rect(position);
            position1.width *= value1;
            position2.width *= value2;

            if (value1 > value2)
            {
                if (value1 > 0f) styleForValue1.Draw(position1, false, false, false, false);
                if (value2 > 0f) styleForValue2.Draw(position2, false, false, false, false);
            }
            else
            {
                if (value2 > 0f) styleForValue2.Draw(position2, false, false, false, false);
                if (value1 > 0f) styleForValue1.Draw(position1, false, false, false, false);
            }

            s_BarText.Draw(position, text, false, false, false, false);
        }

        #region Private Methods

        private static void InitTextureForStyle(GUIStyle style, Color color)
        {
            Texture2D texture = new Texture2D(1, 3);
            texture.SetPixel(0, 0, color + new Color(0.1f, 0.1f, 0f, 0f));
            texture.SetPixel(0, 1, color);
            texture.SetPixel(0, 2, color - new Color(0.1f, 0.1f, 0f, 0f));
            texture.Apply();
            style.normal.background = texture;
        }

        #endregion
    }
}
#endif