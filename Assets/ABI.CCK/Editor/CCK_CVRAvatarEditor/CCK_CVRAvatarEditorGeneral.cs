#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRAvatarEditor
    {
        private void Draw_GeneralAvatarSettings()
        {
            using (new FoldoutScope(ref _guiAvatarSettingsFoldout, "General Avatar Settings"))
            {
                if (!_guiAvatarSettingsFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawGeneralSettings();
            }
        }

        #region Drawing Methods
        
        private void DrawGeneralSettings()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(m_ViewPositionProp, new GUIContent("View Position"));
                if (_isHumanoid && GUILayout.Button("Auto", GUILayout.Width(40))) AutoSetViewPosition();
            }
            
            EditorGUILayout.Space();
            if (_isHumanoid) EditorGUILayout.PropertyField(m_VoiceParentProp, new GUIContent("Voice Parent"));
            
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(m_VoicePositionProp, new GUIContent("Voice Position"));
                if (_isHumanoid && GUILayout.Button("Auto", GUILayout.Width(40))) AutoSetVoicePosition();
            }
        }
        
        #endregion
    }
}
#endif