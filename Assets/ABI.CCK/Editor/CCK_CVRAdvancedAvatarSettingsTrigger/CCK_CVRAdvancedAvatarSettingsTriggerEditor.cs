#if UNITY_EDITOR
using System.Collections.Generic;
using ABI.CCK.Scripts;
using UnityEditor;

// TODO: Rename parent folder to CCK_CVRAdvancedAvatarSettingsTriggerEditor
// I fucked up and forgot the Editor postfix, its driving me mad.
// We cannot fix it without requiring clean reimport of the CCK, so it should wait for huge refactor?

namespace ABI.CCK.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CVRAdvancedAvatarSettingsTrigger))]
    public partial class CCK_CVRAdvancedAvatarSettingsTriggerEditor : Editor
    {
        #region EditorGUI Foldouts

        private static bool _guiAreaConfigurationFoldout = true;
        private static bool _guiInteractionFilterFoldout = true;
        private static bool _guiAllowedFilterFoldout;
        private static bool _guiTriggerSettingsFoldout;

        #endregion
        
        private CVRAdvancedAvatarSettingsTrigger _trigger;
        private List<string> _avatarParameterNames;

        #region Unity Events

        private void OnEnable()
        {
            if (target == null) return;
            _trigger = (CVRAdvancedAvatarSettingsTrigger)target;
            
            CVRAvatar avatar = _trigger.GetComponentInParent<CVRAvatar>();
            if (avatar != null && avatar.overrides != null)
                _avatarParameterNames = CVRCommon.GetParametersFromControllerAsString(avatar.overrides, CVRCommon.NonCoreFilter);
            else
                _avatarParameterNames = new List<string>();
        }

        public override void OnInspectorGUI()
        {
            if (_trigger == null)
                return;

            serializedObject.Update();
            
            Draw_TriggerMode();
            
            Draw_AreaSettings();
            Draw_FilterSettings();

            if (!_trigger.useAdvancedTrigger)
            {
                Draw_SimpleTasks();
            }
            else
            {
                Draw_AllowedFilterSettings();
                Draw_AdvancedTasks();
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Drawing Methods

        private void Draw_TriggerMode()
        {
            int newSelectedIndex = EditorGUILayout.Popup("Trigger Mode", _trigger.useAdvancedTrigger ? 1 : 0,
                new[] { "Simple", "Advanced" });
            _trigger.useAdvancedTrigger = (newSelectedIndex == 1);
        }

        #endregion
    }
}
#endif