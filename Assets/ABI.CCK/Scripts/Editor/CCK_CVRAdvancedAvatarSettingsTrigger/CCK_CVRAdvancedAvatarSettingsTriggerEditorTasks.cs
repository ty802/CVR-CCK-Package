#if UNITY_EDITOR
using ABI.CCK.Scripts.Editor;
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRAdvancedAvatarSettingsTriggerEditor
    {
        private void Draw_SimpleTasks()
        {
            using (new FoldoutScope(ref _guiTriggerSettingsFoldout, "Trigger Settings"))
            {
                if (!_guiTriggerSettingsFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawSimpleTasks();
            }
        }

        private void DrawSimpleTasks()
        {
            GUILayout.BeginVertical();

            string newName = EditorGUIExtensions.AdvancedDropdownInput(EditorGUILayout.GetControlRect(),
                _trigger.settingName, _avatarParameterNames,
                "Setting Name", "No Parameters");
           
            if (newName != _trigger.settingName)
                serializedObject.FindProperty("settingName").stringValue = newName;
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("settingValue"), new GUIContent("Setting Value"));
            GUILayout.EndVertical();
        }
    }
}
#endif