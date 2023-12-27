#if UNITY_EDITOR
using UnityEditor;

namespace ABI.CCK.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CVRVideoPlayer))]
    public partial class CCK_CVRVideoPlayerEditor : Editor
    {
        #region Editor GUI Foldouts

        private static bool _guiGeneralSettingsFoldout = true;
        private static bool _guiAudioSettingsFoldout = false;
        private static bool _guiAudioSettingsPlaybackFoldout = false;
        private static bool _guiAudioSettingsAudioSourceFoldout = false;
        private static bool _guiPlaylistsFoldout = false;
        private static bool _guiEventsFoldout = false;

        #endregion

        #region Private Variables

        private CVRVideoPlayer _player;

        #endregion

        #region Serialized Properties

        private SerializedProperty m_SyncEnabledProp;
        private SerializedProperty m_InteractiveUIProp;
        private SerializedProperty m_VideoPlayerUIPositionProp;
        private SerializedProperty m_ProjectionTextureProp;
        
        private SerializedProperty m_AutoplayProp;
        private SerializedProperty m_LocalPlaybackSpeedProp;
        private SerializedProperty m_PlaybackVolumeProp;
        private SerializedProperty m_AudioPlaybackModeProp;
        private SerializedProperty m_CustomAudioSourceProp;
        private SerializedProperty m_RoomScaleAudioSourcesProp;
        
        private SerializedProperty m_StartedPlaybackProp;
        private SerializedProperty m_FinishedPlaybackProp;
        private SerializedProperty m_PausedPlaybackProp;
        private SerializedProperty m_SetUrlProp;

        #endregion
        
        #region Unity Events

        private void OnEnable()
        {
            if (target == null) return;
            _player = (CVRVideoPlayer)target;
            
            m_SyncEnabledProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.syncEnabled));
            m_InteractiveUIProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.interactiveUI));
            m_VideoPlayerUIPositionProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.videoPlayerUIPosition));
            m_ProjectionTextureProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.ProjectionTexture));

            m_AutoplayProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.autoplay));
            m_LocalPlaybackSpeedProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.localPlaybackSpeed));
            m_PlaybackVolumeProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.playbackVolume));
            m_AudioPlaybackModeProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.audioPlaybackMode));
            m_CustomAudioSourceProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.customAudioSource));
            m_RoomScaleAudioSourcesProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.roomScaleAudioSources));
            
            m_StartedPlaybackProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.startedPlayback));
            m_FinishedPlaybackProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.finishedPlayback));
            m_PausedPlaybackProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.pausedPlayback));
            m_SetUrlProp = serializedObject.FindProperty(nameof(CVRVideoPlayer.setUrl));
        }
        
        public override void OnInspectorGUI()
        {
            if (_player == null)
                return;

            serializedObject.Update();

            Draw_GeneralSettings();
            Draw_AudioSettings();
            Draw_Playlists();
            Draw_Events();

            serializedObject.ApplyModifiedProperties();
        }
        
        #endregion
    }
}
#endif