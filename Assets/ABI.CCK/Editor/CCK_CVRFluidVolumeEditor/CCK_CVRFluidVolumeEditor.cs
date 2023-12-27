#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FluidVolume))]
    public class CCK_CVRFluidVolumeEditor : Editor
    {
        private FluidVolume _fluidVolume;
        
        private SerializedProperty m_VolumeTypeProp;
        private SerializedProperty m_ExtendProp;
        private SerializedProperty m_DepthProp;
        private SerializedProperty m_DensityProp;
        private SerializedProperty m_PlaceFromCenterProp;
        private SerializedProperty m_StreamTypeProp;
        private SerializedProperty m_StreamAngleProp;
        private SerializedProperty m_StreamStrengthProp;
        private SerializedProperty m_SplashParticleSystem;
        
        #region Unity Events

        private void OnEnable()
        {
            if (target == null) return;
            _fluidVolume = (FluidVolume) target;
            
            m_VolumeTypeProp = serializedObject.FindProperty(nameof(FluidVolume.volumeType));
            m_ExtendProp = serializedObject.FindProperty(nameof(FluidVolume.extend));
            m_DepthProp = serializedObject.FindProperty(nameof(FluidVolume.depth));
            m_DensityProp = serializedObject.FindProperty(nameof(FluidVolume.density));
            m_PlaceFromCenterProp = serializedObject.FindProperty(nameof(FluidVolume.placeFromCenter));
            m_StreamTypeProp = serializedObject.FindProperty(nameof(FluidVolume.streamType));
            m_StreamAngleProp = serializedObject.FindProperty(nameof(FluidVolume.streamAngle));
            m_StreamStrengthProp = serializedObject.FindProperty(nameof(FluidVolume.streamStrength));
            m_SplashParticleSystem = serializedObject.FindProperty(nameof(FluidVolume.splashParticleSystem));
        }
        
        public override void OnInspectorGUI()
        {
            if (_fluidVolume == null)
                return;

            serializedObject.Update();

            Draw_Editor();

            serializedObject.ApplyModifiedProperties();
        }
        
        #endregion

        #region Drawing Methods

        private void Draw_Editor()
        {
            EditorGUILayout.PropertyField(m_VolumeTypeProp, new GUIContent("Volume Type"));
            EditorGUILayout.PropertyField(m_ExtendProp, new GUIContent("Width/Length"));
            EditorGUILayout.PropertyField(m_DepthProp, new GUIContent("Depth"));
            if (_fluidVolume.volumeType == FluidVolume.VolumeType.Box)
                EditorGUILayout.PropertyField(m_PlaceFromCenterProp, new GUIContent("Place from center"));
            EditorGUILayout.Space(15f);
            EditorGUILayout.PropertyField(m_StreamTypeProp, new GUIContent("Stream Type"));
            EditorGUILayout.PropertyField(m_StreamAngleProp, new GUIContent("Stream Angle"));
            EditorGUILayout.PropertyField(m_StreamStrengthProp, new GUIContent("Stream Strength"));
            EditorGUILayout.Space(15f);
            EditorGUILayout.PropertyField(m_SplashParticleSystem, new GUIContent("Splash Particle System"));
        }

        #endregion
    }
}
#endif