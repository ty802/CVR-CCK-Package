#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using ABI.CCK.Scripts.Editor;
using UnityEditor;
using UnityEngine;

namespace ABI.CCK.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CVRAvatar))]
    public partial class CCK_CVRAvatarEditor : Editor
    {
        #region Editor GUI Foldouts
        
        private static bool _guiAvatarSettingsFoldout = true;
        private static bool _guiAvatarCustomizationFoldout = true;
        private static bool _guiEyeLookSettingsFoldout = false;
        private static bool _guiEyeLookMuscleInfoFoldout = false;
        private static bool _guiEyeBlinkSettingsFoldout = false;
        private static bool _guiLipSyncSettingsFoldout = false;
        private static bool _guiAdvancedTaggingFoldout = false;
        private static bool _guiAdvancedSettingsFoldout = false;
        
        #endregion

        #region Private Variables

        // Common
        private static readonly string[] _visemeNames =
            { "sil", "PP", "FF", "TH", "DD", "kk", "CH", "SS", "nn", "RR", "aa", "E", "ih", "oh", "ou" };
        
        // Avatar
        private CVRAvatar _avatar;
        private Animator _animator;

        // Avatar Info
        private bool _isHumanoid;
        private List<string> _blendShapeNames;

        #endregion
        
        #region Serialized Properties
        
        private SerializedProperty m_ViewPositionProp;
        private SerializedProperty m_VoiceParentProp;
        private SerializedProperty m_VoicePositionProp;
        private SerializedProperty m_OverridesProp;
        private SerializedProperty m_BodyMeshProp;
        private SerializedProperty m_UseEyeMovementProp;
        private SerializedProperty m_UseBlinkBlendshapesProp;
        private SerializedProperty m_BlinkBlendshapeProp;
        private SerializedProperty m_UseVisemeLipsyncProp;
        private SerializedProperty m_VisemeModeProp;
        private SerializedProperty m_VisemeSmoothingProp;
        private SerializedProperty m_VisemeBlendshapesProp;
        
        #endregion

        #region Unity Events

        private void OnEnable()
        {
            if (target == null) return;
            _avatar = (CVRAvatar)target;
            _avatar.Reset();

            m_ViewPositionProp = serializedObject.FindProperty(nameof(CVRAvatar.viewPosition));
            m_VoiceParentProp = serializedObject.FindProperty(nameof(CVRAvatar.voiceParent));
            m_VoicePositionProp = serializedObject.FindProperty(nameof(CVRAvatar.voicePosition));
            m_OverridesProp = serializedObject.FindProperty(nameof(CVRAvatar.overrides));
            m_BodyMeshProp = serializedObject.FindProperty(nameof(CVRAvatar.bodyMesh));
            m_UseEyeMovementProp = serializedObject.FindProperty(nameof(CVRAvatar.useEyeMovement));
            m_UseBlinkBlendshapesProp = serializedObject.FindProperty(nameof(CVRAvatar.useBlinkBlendshapes));
            m_BlinkBlendshapeProp = serializedObject.FindProperty(nameof(CVRAvatar.blinkBlendshape));
            m_UseVisemeLipsyncProp = serializedObject.FindProperty(nameof(CVRAvatar.useVisemeLipsync));
            m_VisemeModeProp = serializedObject.FindProperty(nameof(CVRAvatar.visemeMode));
            m_VisemeSmoothingProp = serializedObject.FindProperty(nameof(CVRAvatar.visemeSmoothing));
            m_VisemeBlendshapesProp = serializedObject.FindProperty(nameof(CVRAvatar.visemeBlendshapes));
            
            GetBlendshapeNames();

            _animator = _avatar.GetComponent<Animator>();
            if (_animator != null && _animator.isHuman)
            {
                _isHumanoid = true;
                GetHumanoidEyeMuscleLimits();
            }
        }
        
        public override void OnInspectorGUI()
        {
            if (_avatar == null)
                return;

            // TODO: USE SERIALIZED PROPERTIES!
            serializedObject.Update();
            
            Draw_GeneralAvatarSettings();
            Draw_AvatarCustomization();
            Draw_EyeLookSettings();
            Draw_EyeBlinkSettings();
            Draw_LipSyncSettings();

            Draw_AdvancedTagging();
            Draw_AdvancedSettings();

            serializedObject.ApplyModifiedProperties();
        }
        
        public virtual void OnSceneGUI()
        {
            if (_avatar == null) 
                return;
            
            Transform avatarTransform = _avatar.transform;
            Vector3 scale = avatarTransform.localScale;
            Vector3 inverseScale = new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z);

            //View Position
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.green;
            style.fontSize = 20;
            Handles.BeginGUI();
            Vector3 pos = avatarTransform.TransformPoint(Vector3.Scale(_avatar.viewPosition, inverseScale));
            Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);
            GUI.Label(new Rect(pos2D.x + 20, pos2D.y - 10, 100, 20), "View Position", style);
            Handles.EndGUI();

            EditorGUI.BeginChangeCheck();
            Vector3 viewPos = Handles.PositionHandle(pos, avatarTransform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_avatar, "CVR View Position Change");
                _avatar.viewPosition = Vector3.Scale(avatarTransform.InverseTransformPoint(viewPos), scale);
            }

            //Voice Position
            style.normal.textColor = Color.red;
            Handles.BeginGUI();
            pos = avatarTransform.TransformPoint(Vector3.Scale(_avatar.voicePosition, inverseScale));
            pos2D = HandleUtility.WorldToGUIPoint(pos);
            GUI.Label(new Rect(pos2D.x + 20, pos2D.y - 10, 100, 20), "Voice Position", style);
            Handles.EndGUI();

            EditorGUI.BeginChangeCheck();
            Vector3 voicePos = Handles.PositionHandle(pos, avatarTransform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_avatar, "CVR Voice Position Change");
                _avatar.voicePosition = Vector3.Scale(avatarTransform.InverseTransformPoint(voicePos), scale);
            }
        }

        #endregion
        
        #region BlendshapeDropdowns

        private void DrawBlendshape(string label, ref string blendshape)
        {
            if (_avatar.bodyMesh != null)
            {
                if (string.IsNullOrEmpty(blendshape))
                    blendshape = "-none-";
                
                blendshape = EditorGUIExtensions.CustomPopup(
                    GUILayoutUtility.GetRect(new GUIContent(label), EditorStyles.popup),
                    label, 
                    blendshape, 
                    _blendShapeNames.ToArray(),
                    blendshape
                );
            }
            else
            {
                EditorGUILayout.HelpBox("Avatar does not have a Face Mesh specified.", MessageType.Warning);
            }
        }
        
        private void DrawBlendshape(string label, SerializedProperty blendshapesProp)
        {
            if (_avatar.bodyMesh != null)
            {
                if (!blendshapesProp.isArray)
                    return;

                if (blendshapesProp.arraySize != 1)
                    blendshapesProp.arraySize = 1;
                
                SerializedProperty blendshapeElementProp = blendshapesProp.GetArrayElementAtIndex(0);
                
                string currentValue = blendshapeElementProp.stringValue;
                if (string.IsNullOrEmpty(currentValue))
                    currentValue = "-none-";
                
                string newValue = EditorGUIExtensions.CustomPopup(
                    GUILayoutUtility.GetRect(new GUIContent(label), EditorStyles.popup),
                    label,
                    currentValue,
                    _blendShapeNames.ToArray(),
                    currentValue);

                if (newValue != currentValue)
                    blendshapeElementProp.stringValue = newValue;
            }
            else
            {
                EditorGUILayout.HelpBox("Avatar does not have a Face Mesh specified.", 
                    MessageType.Warning);
            }
        }
        
        private void DrawBlendshapes(string label, ref string[] blendshapes, IReadOnlyList<string> names = null)
        {
            if (_avatar.bodyMesh != null)
            {
                for (var i = 0; i < blendshapes.Length; i++)
                {
                    var currentLabel = names == null ? label + " " + (i + 1) : label + names[i];
                    if (string.IsNullOrEmpty(blendshapes[i]))
                        blendshapes[i] = "-none-";
                    
                    blendshapes[i] = EditorGUIExtensions.CustomPopup(
                        GUILayoutUtility.GetRect(new GUIContent(currentLabel), EditorStyles.popup), 
                        currentLabel, 
                        blendshapes[i], 
                        _blendShapeNames.ToArray(),
                        blendshapes[i]);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Avatar does not have a Face Mesh specified.",
                    MessageType.Warning);
            }
        }
        
        private void DrawBlendshapes(string label, SerializedProperty blendshapesProp, IReadOnlyList<string> names = null)
        {
            if (_avatar.bodyMesh != null)
            {
                if (!blendshapesProp.isArray)
                    return;

                if (names != null && blendshapesProp.arraySize != names.Count)
                    blendshapesProp.arraySize = names.Count;

                for (int i = 0; i < blendshapesProp.arraySize; i++)
                {
                    SerializedProperty blendshapeElementProp = blendshapesProp.GetArrayElementAtIndex(i);
                    var currentLabel = (names == null) ? label + " " + (i + 1) : label + names[i];
                    
                    string currentValue = blendshapeElementProp.stringValue;
                    if (string.IsNullOrEmpty(currentValue))
                    {
                        currentValue = "-none-";
                    }
                    
                    string newValue = EditorGUIExtensions.CustomPopup(
                        GUILayoutUtility.GetRect(new GUIContent(currentLabel), EditorStyles.popup),
                        currentLabel,
                        currentValue,
                        _blendShapeNames.ToArray(),
                        currentValue);

                    if (newValue != currentValue)
                        blendshapeElementProp.stringValue = newValue;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Avatar does not have a Face Mesh specified.", 
                    MessageType.Warning);
            }
        }

        #endregion
        
        #region Private Methods

        // this is probably overkill
        private void AutoSetViewPosition()
        {
            if (_animator == null || !_isHumanoid)
                return;
            
            Transform leftEye = _animator.GetBoneTransform(HumanBodyBones.LeftEye);
            Transform rightEye = _animator.GetBoneTransform(HumanBodyBones.RightEye);
            Transform head = _animator.GetBoneTransform(HumanBodyBones.Head);

            if (leftEye && rightEye)
            {
                Undo.RecordObject(_avatar, "CVR View Position Change");
                _avatar.viewPosition = RoundNearZero((leftEye.position + rightEye.position) / 2);
                return;
            }

            if (leftEye || rightEye)
            {
                Undo.RecordObject(_avatar, "CVR View Position Change");
                _avatar.viewPosition = ProjectSingleEyePosition(leftEye ? leftEye : rightEye);
                return;
            }

            if (head)
            {
                string[] leftEyeNames = { "LeftEye", "Left_Eye", "EyeLeft", "Eye_Left" };
                string[] rightEyeNames = { "RightEye", "Right_Eye", "EyeRight", "Eye_Right" };
                leftEye = FindChildByNameVariants(head, leftEyeNames);
                rightEye = FindChildByNameVariants(head, rightEyeNames);
                
                if (leftEye && rightEye) 
                {
                    Undo.RecordObject(_avatar, "CVR View Position Change");
                    _avatar.viewPosition = RoundNearZero((leftEye.position + rightEye.position) / 2);
                    return;
                }

                if (leftEye || rightEye) 
                {
                    Undo.RecordObject(_avatar, "CVR View Position Change");
                    _avatar.viewPosition = ProjectSingleEyePosition(leftEye ? leftEye : rightEye);
                    return;
                }

                Transform root = _animator.GetBoneTransform(HumanBodyBones.Hips);
                float headBoneHeight = Vector3.Distance(root.position, head.position);
                Vector3 localOffset = new Vector3(0f, -0.1f * headBoneHeight, 0.1f * headBoneHeight);
                Undo.RecordObject(_avatar, "CVR View Position Change");
                _avatar.viewPosition = RoundNearZero(head.TransformPoint(localOffset));
                return;
            }

            Debug.LogWarning("Could not find suitable bone for view position. We really tried...");
        }

        private Vector3 ProjectSingleEyePosition(Transform singleEye)
        {
            Vector3 eyePosition = singleEye.position;
            Transform avatarRoot = _animator.transform;
            Vector3 toEyeDirection = (eyePosition - avatarRoot.position).normalized;

            float dotForward = Vector3.Dot(toEyeDirection, avatarRoot.forward);
            float dotUp = Vector3.Dot(toEyeDirection, avatarRoot.up);
            float dotRight = Vector3.Dot(toEyeDirection, avatarRoot.right);

            if (Mathf.Abs(dotForward) > Mathf.Abs(dotUp) && Mathf.Abs(dotForward) > Mathf.Abs(dotRight))
                return RoundNearZero(eyePosition - Vector3.Project(eyePosition - _animator.transform.position, avatarRoot.forward));
            
            return Mathf.Abs(dotUp) > Mathf.Abs(dotRight) ? RoundNearZero(singleEye.position - Vector3.Project(eyePosition - avatarRoot.position, avatarRoot.up)) : RoundNearZero(singleEye.position - Vector3.Project(eyePosition - avatarRoot.position, avatarRoot.right));
        }
        
        private void AutoSetVoicePosition()
        {
            Transform jaw = _animator.GetBoneTransform(HumanBodyBones.Jaw);
            Transform head = _animator.GetBoneTransform(HumanBodyBones.Head);

            if (jaw)
            {
                Undo.RecordObject(_avatar, "CVR Voice Position Change");
                _avatar.voicePosition = jaw.position;
            }
            else if (head)
            {
                Vector3 localOffset = new Vector3(0f, 0.005f, 0.06f);
                Undo.RecordObject(_avatar, "CVR Voice Position Change");
                _avatar.voicePosition = head.TransformPoint(localOffset);
            }
        }
        
        private static Transform FindChildByNameVariants(Transform parent, string[] nameVariants)
        {
            foreach (string potentialName in nameVariants)
            {
                Transform child = parent.Find(potentialName);
                if (child) return child;

                child = parent.Cast<Transform>().FirstOrDefault(t => string.Equals(t.name, potentialName, StringComparison.OrdinalIgnoreCase));
                if (child) return child;
            }

            return null;
        }
        
        private static Vector3 RoundNearZero(Vector3 position)
        {
            const float tolerance = 0.01f;
            return new Vector3(
                Mathf.Abs(position.x) < tolerance ? 0 : position.x,
                Mathf.Abs(position.y) < tolerance ? 0 : position.y,
                Mathf.Abs(position.z) < tolerance ? 0 : position.z
            );
        }

        private void GetBlendshapeNames()
        {
            _blendShapeNames = new List<string> { "-none-" };
            if (_avatar.bodyMesh == null) return;
            
            for (var i = 0; i < _avatar.bodyMesh.sharedMesh.blendShapeCount; i++)
                _blendShapeNames.Add(_avatar.bodyMesh.sharedMesh.GetBlendShapeName(i));
        }

        private void AutoSelectVisemeBlendshapes()
        {
            Undo.RecordObject(_avatar, "CVR Auto Select Visemes");
            for (var i = 0; i < _visemeNames.Length; i++)
            {
                var vPrefix = "v_" + _visemeNames[i];
                var visemePrefix = "viseme_" + _visemeNames[i];

                foreach (var blendShapeName in _blendShapeNames)
                {
                    if (blendShapeName.IndexOf(vPrefix, StringComparison.OrdinalIgnoreCase) < 0 &&
                        blendShapeName.IndexOf(visemePrefix, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                    _avatar.visemeBlendshapes[i] = blendShapeName;
                    break;
                }
            }
        }

        // Failed... Reset is called on component
        // but editor doesnt exist yet...
        // private void AutoSetup(CVRAvatar avatar)
        // {
        //     if (_avatar != avatar)
        //         return;
        //     
        //     // find face mesh first
        //     AutoSetViewPosition();
        //     AutoSetVoicePosition();
        //     AutoSelectVisemeBlendshapes();
        // }
        
        #endregion
    }
}
#endif