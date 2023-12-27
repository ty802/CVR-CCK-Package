#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRVideoPlayerEditor
    {
        private ReorderableList _roomScaleAudioSourcesList;
        
        private void Draw_AudioSettings()
        {
            using (new FoldoutScope(ref _guiAudioSettingsFoldout, "Audio Settings"))
            {
                if (!_guiAudioSettingsFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawAudioSettings();
            }
        }
        
        private void DrawAudioSettings()
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
            
            EditorGUILayout.PropertyField(m_AutoplayProp, new GUIContent("Play On Awake"));
            EditorGUILayout.Slider(m_LocalPlaybackSpeedProp, 0.0f, 1.0f, new GUIContent("Playback Speed"));
            EditorGUILayout.PropertyField(m_PlaybackVolumeProp, new GUIContent("Playback Volume"), true);
            
            EditorGUILayout.PropertyField(m_AudioPlaybackModeProp, new GUIContent("Audio Playback Mode"), true);
            switch (_player.audioPlaybackMode)
            {
                default:
                case CVRVideoPlayer.AudioMode.Direct:
                    EditorGUILayout.HelpBox("This will output the audio in a direct way using 2D Audio Sources. No special setup required, works out of the box.", MessageType.Info);
                    break;
                case CVRVideoPlayer.AudioMode.AudioSource:
                    EditorGUILayout.HelpBox("This will use the provided Custom Audio Source to determine its settings such as fall of range (min, max) and more.", MessageType.Info);
                    break;
                case CVRVideoPlayer.AudioMode.RoomScale:
                    EditorGUILayout.HelpBox("The Room Scale Audio Playback Mode should only be used when playing 5.1 or 7.1 audio. As well as a specific setup of such speakers, audio sources, is required for it to output good results. Stereo Audio will render degraded when using the room scale output mode. ", MessageType.Info);
                    break;
            }
        }

        private void DrawAudioSources()
        {
            if (!InnerFoldout(ref _guiAudioSettingsAudioSourceFoldout, "Audio Sources")) 
                return;
            
            EditorGUILayout.PropertyField(m_CustomAudioSourceProp, new GUIContent("Custom Audio Source"), true);
            
            if (_roomScaleAudioSourcesList == null)
            {
                _roomScaleAudioSourcesList = new ReorderableList(
                    serializedObject,
                    m_RoomScaleAudioSourcesProp,
                    true, true, true, true);

                _roomScaleAudioSourcesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    Rect _rect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
                    SerializedProperty element = _roomScaleAudioSourcesList.serializedProperty.GetArrayElementAtIndex(index);
                    
                    float halfWidth = _rect.width * 0.5f;
                    EditorGUI.PropertyField(
                        new Rect(_rect.x, _rect.y, halfWidth, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("type"), GUIContent.none);
                    EditorGUI.PropertyField(
                        new Rect(_rect.x + halfWidth, _rect.y, halfWidth, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("audioSource"), GUIContent.none);
                };

                _roomScaleAudioSourcesList.elementHeight = EditorGUIUtility.singleLineHeight + 4f;
                _roomScaleAudioSourcesList.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Room Scale Audio Sources");
                    //EditorGUIExtensions.UtilityMenu(rect, _roomScaleAudioSourcesList); // TODO: Make utility menu work with serialized properties
                };
            }

            using (new SetIndentLevelScope(0))
            {
                using (new EditorGUILayout.VerticalScope(new GUIStyle() { padding = new RectOffset(15, 0, 5, 5) }))
                    _roomScaleAudioSourcesList.DoLayoutList();
            }
        }
        
        #endregion
    }
}
#endif
