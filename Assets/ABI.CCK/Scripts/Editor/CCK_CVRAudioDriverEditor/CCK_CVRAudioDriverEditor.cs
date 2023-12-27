#if UNITY_EDITOR
using UnityEditor;

// I likely overcomplicated this GUI. I'm not sure if I'll keep it this way or not.

namespace ABI.CCK.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CVRAudioDriver))]
    public partial class CCK_CVRAudioDriverEditor : Editor
    {
        #region EditorGUI Foldouts

        private static bool _guiAudioSettingsPlaybackFoldout;
        private static bool _guiAudioSettingsAudioSourceFoldout;

        #endregion

        #region Private Variables

        private CVRAudioDriver _audio;

        #endregion

        #region Serialized Properties

        private SerializedProperty m_AudioSourceProp;
        private SerializedProperty m_AudioClipsProp;
        
        private SerializedProperty m_SelectedAudioClipProp;
        private SerializedProperty m_PlayOnSwitchProp;

        #endregion

        #region Unity Events

        private void OnEnable()
        {
            if (target == null) return;
            _audio = (CVRAudioDriver)target;
            
            m_AudioSourceProp = serializedObject.FindProperty(nameof(CVRAudioDriver.audioSource));
            m_AudioClipsProp = serializedObject.FindProperty(nameof(CVRAudioDriver.audioClips));
            m_SelectedAudioClipProp = serializedObject.FindProperty(nameof(CVRAudioDriver.selectedAudioClip));
            m_PlayOnSwitchProp = serializedObject.FindProperty(nameof(CVRAudioDriver.playOnSwitch));
        }

        public override void OnInspectorGUI()
        {
            if (_audio == null)
                return;

            serializedObject.Update();
            
            Draw_DriverSettings();
            
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}
#endif