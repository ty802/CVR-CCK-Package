#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CVRCameraHelper))]
    public partial class CCK_CVRCameraHelperEditor : Editor
    {
        #region EditorGUI Foldouts

        private static bool _guiCameraSettingsFoldout = true;
        private static bool _guiShaderSettingsFoldout = true;

        #endregion
        
        private CVRCameraHelper _cameraHelper;

        private CVRAvatar _validAvatar;
        private CVRSpawnable _validSpawnable;
        private CVRPickupObject _validPickup;

        private SerializedProperty m_CamProp;
        private SerializedProperty m_SetAsMirroringCamera;
        
        private SerializedProperty m_SelectedShaderProp;
        private SerializedProperty m_ReplacementShadersProp;
        
        #region Unity Events

        private void OnEnable()
        {
            if (target == null) return;
            _cameraHelper = (CVRCameraHelper)target;
            
            _validAvatar = _cameraHelper.GetComponentInParent<CVRAvatar>();
            _validSpawnable = _cameraHelper.GetComponentInParent<CVRSpawnable>();
            _validPickup = _cameraHelper.GetComponentInParent<CVRPickupObject>();
            
            m_CamProp = serializedObject.FindProperty(nameof(CVRCameraHelper.cam));
            m_SetAsMirroringCamera = serializedObject.FindProperty(nameof(CVRCameraHelper.setAsMirroringCamera));
            m_SelectedShaderProp = serializedObject.FindProperty(nameof(CVRCameraHelper.selectedShader));
            m_ReplacementShadersProp = serializedObject.FindProperty(nameof(CVRCameraHelper.replacementShaders));
        }

        public override void OnInspectorGUI()
        {
            if (_cameraHelper == null)
                return;

            serializedObject.Update();
            
            Draw_Info();
            Draw_AvatarError();
            Draw_PropWarning();
            
            Draw_CameraSettings();
            Draw_ShaderSettings();
            
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Drawing Methods

        private void Draw_Info()
        {
            EditorGUILayout.HelpBox("For use with Animation Events. Allows you to call TakeScreenshot to save the Camera Render Texture output. Limited to once per second.", MessageType.Info);
        }
        
        private void Draw_AvatarError()
        {
            if (_validAvatar != null)
                EditorGUILayout.HelpBox("CVRCameraHelper is not currently allowed on Avatars!", MessageType.Error);
        }
        
        private void Draw_PropWarning()
        {
            if (_validSpawnable && !_validPickup)
                EditorGUILayout.HelpBox("CVRPickupObject is missing in hierarchy. CVRCameraHelper must be held while on a prop for TakeScreenshot to work.", MessageType.Warning);
        }

        #endregion
    }
}
#endif