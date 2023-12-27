#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRAvatarEditor
    {
        private void Draw_LipSyncSettings()
        {
            using (new FoldoutScope(ref _guiLipSyncSettingsFoldout, "Lip Sync Settings"))
            {
                if (!_guiLipSyncSettingsFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawLipSyncSettings();
            }
        }

        #region Drawing Methods
        
        private void DrawLipSyncSettings()
        {
            EditorGUILayout.PropertyField(m_UseVisemeLipsyncProp, new GUIContent("Use Lip Sync"));
            EditorGUILayout.PropertyField(m_VisemeModeProp, new GUIContent("Lip Sync Mode"));
                    
            Separator();
            
            switch (_avatar.visemeMode)
            {
                default: // not really sure what the default is?
                case CVRAvatar.CVRAvatarVisemeMode.Visemes:
                {
                    DrawLipSyncVisemeMode();
                    break;
                }
                case CVRAvatar.CVRAvatarVisemeMode.SingleBlendshape:
                    DrawBlendshape("Single Blendshape: ", m_VisemeBlendshapesProp);
                    break;
                case CVRAvatar.CVRAvatarVisemeMode.JawBone:
                    DrawLipSyncJawBoneMode();
                    break;
            }
        }

        private void DrawLipSyncVisemeMode()
        {
            EditorGUILayout.PropertyField(m_VisemeSmoothingProp, new GUIContent("Viseme Smoothing"), GUILayout.MinWidth(10));
            DrawBlendshapes("Viseme: ", m_VisemeBlendshapesProp, _visemeNames);

            EditorGUILayout.Space();
            
            if (_avatar.bodyMesh != null && GUILayout.Button("Auto Select Visemes"))
            {
                m_UseVisemeLipsyncProp.boolValue = true;
                AutoSelectVisemeBlendshapes();
            }

            //EditorGUILayout.HelpBox(CCKLocalizationProvider.GetLocalizedText("ABI_UI_AVATAR_INFO_EYE_VISEMES"), MessageType.Info);
        }

        private void DrawLipSyncJawBoneMode()
        {
            if (!_isHumanoid)
            {
                EditorGUILayout.HelpBox("Avatar is not configured as humanoid or lacks an Animator component. Jaw bone lipsync mode is not supported!",
                    MessageType.Error);
                return;
            }
            
            EditorGUILayout.HelpBox("Using the jaw transform set in the humanoid configuration.", MessageType.Info);
            
            using (new EditorGUI.DisabledGroupScope(true))
                EditorGUILayout.ObjectField("Jaw", _animator.GetBoneTransform(HumanBodyBones.Jaw), typeof(Transform), true);
        }
        
        #endregion
    }
}
#endif 