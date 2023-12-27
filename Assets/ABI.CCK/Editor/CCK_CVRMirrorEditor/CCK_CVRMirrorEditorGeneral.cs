#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRMirrorEditor
    {
        private void Draw_GeneralSettings()
        {
            using (new FoldoutScope(ref _guiGeneralSettingsFoldout, "General Settings"))
            {
                if (!_guiGeneralSettingsFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawGeneralSettings();
            }
        }

        #region Drawing Methods
        
        private void DrawGeneralSettings()
        {
            GUILayout.BeginVertical();

            EditorGUILayout.PropertyField(m_DisablePixelLightsProp, new GUIContent("Disable Pixel Lights"));
            EditorGUILayout.PropertyField(m_TextureSizeProp, new GUIContent("Texture Size"));
            EditorGUILayout.PropertyField(m_ReflectLayersProp, new GUIContent("Reflect Layers"));
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Reflect Players", GUILayout.ExpandWidth(true)))
                m_ReflectLayersProp.intValue = 3840;

            if (GUILayout.Button("Reflect Players & World", GUILayout.ExpandWidth(true)))
                m_ReflectLayersProp.intValue = 3841;
            
            EditorGUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
        
        #endregion
    }
}
#endif