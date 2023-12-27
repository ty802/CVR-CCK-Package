using System;
using System.Collections.Generic;
using System.Linq;
using ABI.CCK.Scripts;
using UnityEngine;

namespace ABI.CCK.Components
{
    [ExecuteInEditMode]
    [AddComponentMenu("ChilloutVR/CVR Avatar")]
    [HelpURL("https://developers.abinteractive.net/cck/components/avatar/")]
    [RequireComponent(typeof(Animator))]
    public class CVRAvatar : MonoBehaviour, ICCK_Component
    {
        #region Editor Methods

        public void Reset()
        {
            if (GetComponent<CVRAssetInfo>() != null) return;
            CVRAssetInfo info = gameObject.AddComponent<CVRAssetInfo>();
            info.type = CVRAssetInfo.AssetType.Avatar;
        }

        #endregion

        #region CVRAvatarEnums
        
        public enum CVRAvatarVoiceParent { Head = 0, LeftHand = 2, RightHand = 3, Hips = 4 }
        public enum CVRAvatarVisemeMode { Visemes = 0, SingleBlendshape = 1, JawBone = 2 }
        public enum CVRAvatarEyeLookMode { Muscle = 0, Transform = 1, Blendshape = 2 }
        
        #endregion

        #region General Avatar Settings
        //[Space] [Header("General Avatar Settings")] [Space]
        
        public Vector3 viewPosition = new Vector3(0, 0.1f, 0);
        public Vector3 voicePosition = new Vector3(0, 0.15f, 0);
        public CVRAvatarVoiceParent voiceParent = CVRAvatarVoiceParent.Head;
        
        #endregion

        #region Avatar Customization
        //[Space] [Header("Avatar Customization")] [Space]
        
        public AnimatorOverrideController overrides;
        public SkinnedMeshRenderer bodyMesh;
        
        #endregion

        #region Eye Look Settings
        //[Space] [Header("Eye Look Settings")] [Space]
        
        public bool useEyeMovement = true;
        public CVRAvatarEyeLookMode eyeLookMode = CVRAvatarEyeLookMode.Muscle;
        //public Transform leftEyeTransform;
        //public Transform rightEyeTransform;
        //public string[] eyeBlendshapes = new string[4];
        
        #endregion

        #region Eye Blink Settings
        //[Space] [Header("Eye Blink Settings")] [Space]
        
        public bool useBlinkBlendshapes;
        public string[] blinkBlendshape = new string[4];
        public float blinkDuration = 0.4f;
        public Vector2 blinkGapRange = new Vector2(0.4f, 8f);
        
        #endregion

        #region Lip Sync Settings
        //[Space] [Header("Lip Sync Settings")] [Space]
        
        public bool useVisemeLipsync;
        public CVRAvatarVisemeMode visemeMode = CVRAvatarVisemeMode.Visemes;
        [Range(1, 100)] public int visemeSmoothing = 50;
        public string[] visemeBlendshapes = new string[15];
        
        #endregion

        #region Advanced Tagging
        //[Space] [Header("Advanced Tagging")] [Space]
        
        public bool enableAdvancedTagging;
        public List<CVRAvatarAdvancedTaggingEntry> advancedTaggingList = new List<CVRAvatarAdvancedTaggingEntry>();
        
        #endregion
        
        #region Advanced Settings
        //[Space] [Header("Advanced Settings")] [Space]
        
        public bool avatarUsesAdvancedSettings;
        public CVRAdvancedAvatarSettings avatarSettings;
        
        #endregion

        #region Unity Methods
        
        private void OnDrawGizmosSelected()
        {
            Vector3 scale = transform.localScale;
            scale.x = 1 / scale.x;
            scale.y = 1 / scale.y;
            scale.z = 1 / scale.z;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.TransformPoint(Vector3.Scale(viewPosition, scale)), 0.01f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.TransformPoint(Vector3.Scale(voicePosition, scale)), 0.01f);
        }
        
        #endregion

        #region Parameter Sync Usage
#if UNITY_EDITOR

        public (int, int) GetParameterSyncUsage()
        {
            if (avatarSettings?.settings == null)
                return (0, 0);

            var animatorParameterNames = new HashSet<string>();
            int syncedValuesOverrides = 0, syncedBooleansOverrides = 0;
            int syncedValuesAASAutoGen = 0, syncedBooleansAASAutoGen = 0;

            // Count override controller (real count)
            if (overrides != null && overrides.runtimeAnimatorController != null)
            {
                foreach (AnimatorControllerParameter parameter in CVRCommon.GetParametersFromController(
                             overrides.runtimeAnimatorController, CVRCommon.NonCoreFilter, CVRCommon.NonLocalFilter))
                {
                    if (!animatorParameterNames.Add(parameter.name))
                        continue;

                    if (parameter.type == AnimatorControllerParameterType.Bool)
                        syncedBooleansOverrides++;
                    else if (parameter.type != AnimatorControllerParameterType.Trigger)
                        syncedValuesOverrides++;
                }
            }
            
            animatorParameterNames.Clear();

            // Count baseController (part of autogen)
            if (avatarSettings.baseController != null)
            {
                foreach (AnimatorControllerParameter parameter in CVRCommon.GetParametersFromController(
                             avatarSettings.baseController, CVRCommon.NonCoreFilter, CVRCommon.NonLocalFilter))
                {
                    if (!animatorParameterNames.Add(parameter.name))
                        continue;

                    if (parameter.type == AnimatorControllerParameterType.Bool)
                        syncedBooleansAASAutoGen++;
                    else if (parameter.type != AnimatorControllerParameterType.Trigger)
                        syncedValuesAASAutoGen++;
                }
            }

            // Count menu entries (part of autogen, not real)
            foreach (CVRAdvancedSettingsEntry entry in avatarSettings.settings)
            {
                if (IsValidParameter(entry.machineName) && animatorParameterNames.Add(entry.machineName))
                {
                    switch (entry.type)
                    {
                        case CVRAdvancedSettingsEntry.SettingsType.GameObjectToggle:
                            if (entry.setting.usedType == CVRAdvancesAvatarSettingBase.ParameterType.GenerateBool)
                                syncedBooleansAASAutoGen += 1;
                            else
                                syncedValuesAASAutoGen += 1;
                            break;
                        case CVRAdvancedSettingsEntry.SettingsType.MaterialColor:
                            IncrementSyncValuesForEntry(entry, animatorParameterNames, ref syncedValuesAASAutoGen, "-r", "-g", "-b");
                            break;
                        case CVRAdvancedSettingsEntry.SettingsType.Joystick2D:
                        case CVRAdvancedSettingsEntry.SettingsType.InputVector2:
                            IncrementSyncValuesForEntry(entry, animatorParameterNames, ref syncedValuesAASAutoGen, "-x", "-y");
                            break;
                        case CVRAdvancedSettingsEntry.SettingsType.Joystick3D:
                        case CVRAdvancedSettingsEntry.SettingsType.InputVector3:
                            IncrementSyncValuesForEntry(entry, animatorParameterNames, ref syncedValuesAASAutoGen, "-x", "-y", "-z");
                            break;
                        case CVRAdvancedSettingsEntry.SettingsType.Slider:
                        case CVRAdvancedSettingsEntry.SettingsType.InputSingle:
                        case CVRAdvancedSettingsEntry.SettingsType.GameObjectDropdown:
                        default:
                            syncedValuesAASAutoGen += 1;
                            break;
                    }
                }
            }

            int realUsage = syncedValuesOverrides * 32 + Mathf.CeilToInt(syncedBooleansOverrides / 8f) * 8;
            int autogenUsage = syncedValuesAASAutoGen * 32 + Mathf.CeilToInt(syncedBooleansAASAutoGen / 8f) * 8;

            return (realUsage, autogenUsage);
        }

        private static bool IsValidParameter(string parameterName)
        {
            return !string.IsNullOrEmpty(parameterName) && !CVRCommon.CoreParameters.Contains(parameterName) &&
                   !parameterName.StartsWith(CVRCommon.LOCAL_PARAMETER_PREFIX);
        }

        private static void IncrementSyncValuesForEntry(CVRAdvancedSettingsEntry entry, HashSet<string> animatorParameters, ref int syncedValues, params string[] suffixes)
        {
            int newSyncedValues = suffixes.Count(suffix => animatorParameters.Add(entry.machineName + suffix));
            syncedValues += newSyncedValues;
        }
        
#endif
        #endregion
    }

    #region Advanced Tagging Class
    
    [Serializable]
    public class CVRAvatarAdvancedTaggingEntry
    {
        public enum Tags
        {
            LoudAudio = 1,
            LongRangeAudio = 2,
            ScreenFx = 4,
            FlashingColors = 8,
            FlashingLights = 16,
            Violence = 32,
            Gore = 64,
            Suggestive = 128,
            Nudity = 256,
            Horror = 512
        }
        public Tags tags = 0;
        public GameObject gameObject;
        public GameObject fallbackGameObject;
    }
    
    #endregion
}