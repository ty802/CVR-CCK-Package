#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRCameraHelperEditor
    {
        private void Draw_CameraSettings()
        {
            using (new FoldoutScope(ref _guiCameraSettingsFoldout, "Camera Settings"))
            {
                if (!_guiCameraSettingsFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawCameraSettings();
            }
        }

        private void DrawCameraSettings()
        {
            GUILayout.BeginVertical();

            EditorGUILayout.PropertyField(m_CamProp, new GUIContent("Camera"));
            
            // This option is not implemented. Leaving in GUI though as it fills the space nicely. :)
            EditorGUILayout.PropertyField(m_SetAsMirroringCamera, new GUIContent("Mirroring Camera"));

            GUILayout.EndVertical();
        }
    }
}
#endif