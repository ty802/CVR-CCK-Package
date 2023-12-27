#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRAdvancedAvatarSettingsTriggerEditor
    {
        private void Draw_AreaSettings()
        {
            using (new FoldoutScope(ref _guiAreaConfigurationFoldout, "Area Configuration"))
            {
                if (!_guiAreaConfigurationFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawAreaSettings();
            }
        }

        private void DrawAreaSettings()
        {
            GUILayout.BeginVertical();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("areaSize"), new GUIContent("Area Size"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("areaOffset"), new GUIContent("Area Offset"));
            GUILayout.EndVertical();
        }
    }
}
#endif