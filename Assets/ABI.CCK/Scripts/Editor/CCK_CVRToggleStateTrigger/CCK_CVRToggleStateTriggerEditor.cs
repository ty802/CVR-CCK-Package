#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CVRToggleStateTrigger))]
    public partial class CCK_CVRToggleStateTriggerEditor : Editor
    {
        #region EditorGUI Foldouts

        private static bool _guiAreaConfigurationFoldout = true;
        private static bool _guiToggleTaskFoldout = true;

        #endregion
        
        private CVRToggleStateTrigger _trigger;

        private SerializedProperty m_AreaSizeProp;
        private SerializedProperty m_AreaOffsetProp;
        
        private SerializedProperty m_ToggleStateIDProp;
        
        #region Unity Events

        private void OnEnable()
        {
            if (target == null) return;
            _trigger = (CVRToggleStateTrigger)target;
            
            m_AreaSizeProp = serializedObject.FindProperty(nameof(CVRToggleStateTrigger.areaSize));
            m_AreaOffsetProp = serializedObject.FindProperty(nameof(CVRToggleStateTrigger.areaOffset));
            m_ToggleStateIDProp = serializedObject.FindProperty(nameof(CVRToggleStateTrigger.toggleStateID));
        }

        public override void OnInspectorGUI()
        {
            if (_trigger == null)
                return;

            serializedObject.Update();
            
            Draw_AreaSettings();
            Draw_ToggleTask();
            
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Drawing Methods
        
        private void Draw_ToggleTask()
        {
            using (new FoldoutScope(ref _guiToggleTaskFoldout, "Toggle State Task"))
            {
                if (!_guiToggleTaskFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawToggleTask();
            }
        }

        private void DrawToggleTask()
        {
            GUILayout.BeginVertical();
            EditorGUILayout.PropertyField(m_ToggleStateIDProp, new GUIContent("Toggle State ID"));
            //Separator();
            EditorGUILayout.HelpBox("Sets the toggle state of your avatar when a CVRPointer enters the indicated trigger area.", MessageType.Info);
            GUILayout.EndVertical();
        }

        #endregion
    }
}
#endif