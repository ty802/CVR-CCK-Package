#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRSpawnableEditor
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

            EditorGUILayout.PropertyField(m_SpawnHeightProp);
            EditorGUILayout.PropertyField(m_PropPrivacyProp);

            GUILayout.EndVertical();
        }
        
        #endregion
    }
}
#endif