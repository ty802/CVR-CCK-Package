#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRMirrorEditor
    {
        private void Draw_OptimizationSettings()
        {
            if (!_mirror.m_ignoreLegacyBehaviour)
                return;
            
            using (new FoldoutScope(ref _guiOptimizationSettingsFoldout, "Optimization Settings"))
            {
                if (!_guiOptimizationSettingsFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawOptimizationSettings();
            }
        }

        #region Drawing Methods
        
        private void DrawOptimizationSettings()
        {
            GUILayout.BeginVertical();
            
            EditorGUILayout.PropertyField(m_UseOcclusionCullingProp, new GUIContent("Use Occlusion Culling"));
            // EditorGUILayout.PropertyField(m_UseOcclusionCullingProp, new GUIContent("Cull Backside"));
            
            GUILayout.EndVertical();
        }
        
        #endregion
    }
}
#endif