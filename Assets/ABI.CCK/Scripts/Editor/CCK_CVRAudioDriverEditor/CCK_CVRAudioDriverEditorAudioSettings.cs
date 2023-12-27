#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRAudioDriverEditor
    {
        private void Draw_DriverSettings()
        {
            using (new LabelScope("Driver Settings"))
            {
                using (new EditorGUI.IndentLevelScope())
                    DrawDriverSettings();
            }
        }

        private void DrawDriverSettings()
        {
            GUILayout.BeginVertical();
            
            DrawPlaybackSettings();
            
            EditorGUILayout.Space();
            DrawAudioSources();
            
            GUILayout.EndVertical();
        }

        #region Drawing Methods

        private void DrawPlaybackSettings()
        {
            if (!InnerFoldout(ref _guiAudioSettingsPlaybackFoldout, "Playback"))
                return;

            EditorGUILayout.PropertyField(m_PlayOnSwitchProp, new GUIContent("Play On Switch"));
            EditorGUILayout.PropertyField(m_SelectedAudioClipProp, new GUIContent("Selected Audio Clip"));
        }

        private void DrawAudioSources()
        {
            if (!InnerFoldout(ref _guiAudioSettingsAudioSourceFoldout, "Audio"))
                return;

            EditorGUILayout.PropertyField(m_AudioSourceProp, new GUIContent("Audio Source"));
            EditorGUILayout.PropertyField(m_AudioClipsProp, new GUIContent("Audio Clips"), true);
        }

        #endregion
    }
}
#endif