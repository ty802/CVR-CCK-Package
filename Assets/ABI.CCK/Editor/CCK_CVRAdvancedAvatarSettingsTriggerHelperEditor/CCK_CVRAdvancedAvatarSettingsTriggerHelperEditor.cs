#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CVRAdvancedAvatarSettingsTriggerHelper))]
    public class CCK_CVRAdvancedAvatarSettingsTriggerHelperEditor : Editor
    {
        private CVRAdvancedAvatarSettingsTriggerHelper _triggerHelper;

        private SerializedProperty m_TriggersProp;
        
        #region Unity Events

        private void OnEnable()
        {
            if (target == null) return;
            _triggerHelper = (CVRAdvancedAvatarSettingsTriggerHelper)target;
            
            m_TriggersProp = serializedObject.FindProperty(nameof(CVRAdvancedAvatarSettingsTriggerHelper.triggers));
        }

        public override void OnInspectorGUI()
        {
            if (_triggerHelper == null)
                return;

            serializedObject.Update();
            
            Draw_Info();
            Draw_HelperTriggers();
            
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Drawing Methods

        private void Draw_Info()
        {
            EditorGUILayout.HelpBox("For use with Animation Events. Allows you to call OnEnter, OnExit, and OnStay on any trigger in this list.", MessageType.Info);
        }
        
        private void Draw_HelperTriggers()
        {
            using (new LabelScope("Trigger References"))
                DrawHelperTriggers();
        }

        private void DrawHelperTriggers()
        {
            GUILayout.BeginVertical();
            //Separator();
            using (new EditorGUI.IndentLevelScope()) EditorGUILayout.PropertyField(m_TriggersProp, new GUIContent("Triggers"));
            GUILayout.EndVertical();
        }

        #endregion
    }
}
#endif