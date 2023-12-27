#if UNITY_EDITOR
using ABI.CCK.Scripts.Editor;
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRAvatarEditor
    {
        // Muscle Limits
        private Vector2 _leftEyeUpDown, _leftEyeInOut, _rightEyeUpDown, _rightEyeInOut;
        
        private void Draw_EyeLookSettings()
        {
            using (new FoldoutScope(ref _guiEyeLookSettingsFoldout, "Eye Look Settings"))
            {
                if (!_guiEyeLookSettingsFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawEyeLookSettings();
            }
        }
        
        #region Drawing Methods
        
        private void DrawEyeLookSettings()
        {
            EditorGUILayout.PropertyField(m_UseEyeMovementProp, new GUIContent("Use Eye Movement"));
                    
            // TODO: TEMPORARY HIDDEN UNTIL FULLY IMPLEMENTED
            // EditorGUILayout.PropertyField(m_EyeLookModeProp, new GUIContent("Eye Look Mode"));
            
            Separator();
                    
            // switch (_avatar.eyeLookMode)
            // {
            //     default: // legacy avatars likely use Muscle
            //     case CVRAvatar.CVRAvatarEyeLookMode.Muscle:
            //         DrawMuscleEyeSettings();
            //         break;
            //     case CVRAvatar.CVRAvatarEyeLookMode.Transform:
            //         //DrawTransformEyeSettings();
            //         break;
            //     case CVRAvatar.CVRAvatarEyeLookMode.Blendshape:
            //         //DrawBlendshapeEyeSettings();
            //         break;
            // }
        }

        private void DrawMuscleEyeSettings()
        {
            if (!_isHumanoid)
            {
                EditorGUILayout.HelpBox("Avatar is not configured as humanoid or lacks an Animator component. Muscle eye look mode is not supported!",
                    MessageType.Error);
                return;
            }
            
            EditorGUILayout.HelpBox("Using the eye transforms and muscle limits set in the humanoid configuration.",
                MessageType.Info);

            if (!InnerFoldout(ref _guiEyeLookMuscleInfoFoldout, "Eye Limits")) 
                return;

            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.ObjectField("Left Eye", _animator.GetBoneTransform(HumanBodyBones.LeftEye),
                typeof(Transform), true);
            EditorGUILayout.ObjectField("Right Eye", _animator.GetBoneTransform(HumanBodyBones.RightEye),
                typeof(Transform), true);

            EditorGUILayout.LabelField("Left Eye Limits", EditorStyles.boldLabel);
            EditorGUIExtensions.LimitSliderSided("Eye Down/Up", ref _leftEyeUpDown);
            EditorGUIExtensions.LimitSliderSided("Eye In/Out", ref _leftEyeInOut);

            EditorGUILayout.LabelField("Right Eye Limits", EditorStyles.boldLabel);
            EditorGUIExtensions.LimitSliderSided("Eye Down/Up", ref _rightEyeUpDown);
            EditorGUIExtensions.LimitSliderSided("Eye In/Out", ref _rightEyeInOut);

            EditorGUI.EndDisabledGroup();
        }

        #endregion

        #region Private Methods

        private void GetHumanoidEyeMuscleLimits()
        {
            Vector2 defaultUpDown = new Vector2(-10f, 15f);
            Vector2 defaultInOut = new Vector2(-20f, 20f);

            foreach (HumanBone humanBone in _animator.avatar.humanDescription.human)
                if (humanBone.humanName == HumanBodyBones.LeftEye.ToString())
                {
                    _leftEyeUpDown = humanBone.limit.useDefaultValues
                        ? defaultUpDown
                        : new Vector2(humanBone.limit.min.z, humanBone.limit.max.z);
                    _leftEyeInOut = humanBone.limit.useDefaultValues
                        ? defaultInOut
                        : new Vector2(-humanBone.limit.max.y, -humanBone.limit.min.y);
                }
                else if (humanBone.humanName == HumanBodyBones.RightEye.ToString())
                {
                    _rightEyeUpDown = humanBone.limit.useDefaultValues
                        ? defaultUpDown
                        : new Vector2(humanBone.limit.min.z, humanBone.limit.max.z);
                    _rightEyeInOut = humanBone.limit.useDefaultValues
                        ? defaultInOut
                        : new Vector2(humanBone.limit.min.y, humanBone.limit.max.y);
                }
        }

        #endregion
    }
}
#endif