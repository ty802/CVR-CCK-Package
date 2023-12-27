#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRMirrorEditor
    {
        private void Draw_AdvancedSettings()
        {
            using (new FoldoutScope(ref _guiAdvancedSettingsFoldout, "Advanced Settings"))
            {
                if (!_guiAdvancedSettingsFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawAdvancedSettings();
            }
        }

        private void DrawAdvancedSettings()
        {
            GUILayout.BeginVertical();
            
            EditorGUILayout.PropertyField(m_ClearFlagsProp, new GUIContent("Clear Flags"));
            
            if (_mirror.m_ClearFlags == CVRMirror.MirrorClearFlags.Skybox)
                EditorGUILayout.PropertyField(m_CustomSkyboxProp, new GUIContent("Custom Skybox"));
            else
                EditorGUILayout.PropertyField(m_CustomColorProp, new GUIContent("Custom Color"));

            EditorGUILayout.PropertyField(m_ClipPlaneOffsetProp, new GUIContent("Clip Plane Offset"));
            EditorGUILayout.PropertyField(m_framesNeededToUpdateProp, new GUIContent("Frames Needed To Update"));
            
            GUILayout.EndVertical();
        }
    }
}
#endif