#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRAvatarEditor
    {
        private void Draw_EyeBlinkSettings()
        {
            using (new FoldoutScope(ref _guiEyeBlinkSettingsFoldout, "Eye Blink Settings"))
            {
                if (!_guiEyeBlinkSettingsFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawEyeBlinkSettings();
            }
        }

        #region Drawing Methods

        private void DrawEyeBlinkSettings()
        {

            EditorGUILayout.PropertyField(m_UseBlinkBlendshapesProp, new GUIContent("Use Blink Blendshapes"));

            // TODO: TEMPORARY HIDDEN UNTIL FULLY IMPLEMENTED
            // MinMaxSliderWithFields(m_BlinkGapRangeProp, "Blink Gap", 0.1f, 10f, true);
            // EditorGUILayout.PropertyField(m_BlinkDurationProp, new GUIContent("Blink Duration"), GUILayout.MinWidth(60), GUILayout.MaxWidth(100));

            Separator();
    
            DrawBlendshapes("Blink", m_BlinkBlendshapeProp);
        }

        #endregion
    }
}
#endif