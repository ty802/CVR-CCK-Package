#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ABI.CCK.Scripts.Editor
{
    // Shared between all Custom Editors for CCK Components specifically
    public static class SharedComponentGUI
    {
        public static readonly GUIStyle s_BoldLabelStyle;
        public static readonly GUIStyle s_BoldFoldoutStyle;

        static SharedComponentGUI()
        {
            // Foldout scope styles
            s_BoldLabelStyle = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };
            s_BoldFoldoutStyle = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
        }
        
        public static void Separator()
        {
            GUILayout.Space(2);
            GUILayout.Box("", EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.Height(2));
            GUILayout.Space(2);
        }

        public static bool InnerFoldout(ref bool foldoutState, string label, GUIStyle style = null)
        {
            return foldoutState = EditorGUILayout.Foldout(foldoutState, label, true, style ?? EditorStyles.foldout);
        }
        
        public static bool InnerFoldout(ref bool foldoutState, GUIContent label, GUIStyle style = null)
        {
            return foldoutState = EditorGUILayout.Foldout(foldoutState, label, true, style ?? EditorStyles.foldout);
        }
        
        public class LabelScope : GUI.Scope
        {
            // Magic numbers
            private const float LabelHeight = 24.0f;
            private const float IndentX = 8.0f;
            private const float VerticalSpacingAdjust = -26.0f;

            public LabelScope(string label, GUIStyle style = null)
            {
                InitLabelScope(new GUIContent(label), style ?? s_BoldFoldoutStyle);
            }

            public LabelScope(GUIContent label, GUIStyle style = null)
            {
                InitLabelScope(label, style ?? s_BoldFoldoutStyle);
            }
            
            private void InitLabelScope(GUIContent label, GUIStyle style)
            {
                // Label
                Rect labelRect = GUILayoutUtility.GetRect(1.0f, LabelHeight);
                EditorGUI.HelpBox(labelRect, "", MessageType.None);
                labelRect.x += IndentX;
                labelRect.width -= IndentX;
                EditorGUI.LabelField(labelRect, label, style);

                // Content
                GUILayout.Space(VerticalSpacingAdjust);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(LabelHeight);
            }

            protected override void CloseScope()
            {
                GUILayout.Space(3f);
                GUILayout.EndVertical();
            }
        }

        public class FoldoutScope : GUI.Scope
        {
            // Magic numbers
            private const float FoldoutHeight = 24.0f;
            private const float IndentX = 18.0f;
            private const float VerticalSpacingAdjust = -26.0f;

            public FoldoutScope(ref bool foldoutState, string label, GUIStyle style = null)
            {
                InitFoldoutScope(ref foldoutState, new GUIContent(label), style ?? s_BoldFoldoutStyle);
            }

            public FoldoutScope(ref bool foldoutState, GUIContent label, GUIStyle style = null)
            {
                InitFoldoutScope(ref foldoutState, label, style ?? s_BoldFoldoutStyle);
            }

            private void InitFoldoutScope(ref bool foldoutState, GUIContent label, GUIStyle style)
            {
                // Foldout
                Rect foldoutRect = GUILayoutUtility.GetRect(1.0f, FoldoutHeight);
                EditorGUI.HelpBox(foldoutRect, "", MessageType.None);
                foldoutRect.x += IndentX;
                foldoutRect.width -= IndentX;
                foldoutState = EditorGUI.Foldout(foldoutRect, foldoutState, label, true, style);

                // Content
                GUILayout.Space(VerticalSpacingAdjust);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(foldoutState ? FoldoutHeight : 21);
            }

            protected override void CloseScope()
            {
                GUILayout.Space(3f);
                GUILayout.EndVertical();
            }
        }

        public class ToggleFoldoutScope : GUI.Scope
        {
            // Magic numbers 2
            private const float FoldoutHeight = 24.0f;
            private const float IndentX = 18.0f;
            private const float VerticalSpacingAdjust = -26.0f;
            private const float ToggleWidth = 16.0f;

            public ToggleFoldoutScope(ref bool foldoutState, ref bool toggleState, string label, GUIStyle style = null)
            {
                InitToggleFoldoutScope(ref foldoutState, ref toggleState, new GUIContent(label), style ?? s_BoldFoldoutStyle);
            }

            public ToggleFoldoutScope(ref bool foldoutState, ref bool toggleState, GUIContent label, GUIStyle style = null)
            {
                InitToggleFoldoutScope(ref foldoutState, ref toggleState, label, style ?? s_BoldFoldoutStyle);
            }
            
            private void InitToggleFoldoutScope(ref bool foldoutState, ref bool toggleState, GUIContent label, GUIStyle style)
            {
                // Foldout
                Rect fullRect = GUILayoutUtility.GetRect(1.0f, FoldoutHeight);
                EditorGUI.HelpBox(fullRect, "", MessageType.None);

                // Toggle
                Rect toggleRect = new Rect(fullRect.x + IndentX + 1f, fullRect.y + 1f, ToggleWidth, FoldoutHeight);
                toggleState = EditorGUI.Toggle(toggleRect, toggleState);

                // Foldout
                Rect foldoutRect = new Rect(fullRect.x + IndentX, fullRect.y, fullRect.width - IndentX, FoldoutHeight);
                foldoutState = EditorGUI.Foldout(foldoutRect, foldoutState, "      " + label, true, style);

                // Content
                GUILayout.Space(VerticalSpacingAdjust);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(foldoutState ? FoldoutHeight : 21);
            }

            protected override void CloseScope()
            {
                GUILayout.Space(3f);
                GUILayout.EndVertical();
            }
        }
        
        public class SetIndentLevelScope : GUI.Scope
        {
            private readonly int originalIndentLevel;
            
            public SetIndentLevelScope(int indentLevel)
            {
                originalIndentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indentLevel;
            }

            protected override void CloseScope()
            {
                 EditorGUI.indentLevel = originalIndentLevel;
            }
        }
    }
}
#endif