#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CVRMovementParent))]
    public class CCK_CVRMovementParentEditor : Editor
    {
        private CVRMovementParent _movementParent;
        
        private SerializedProperty m_OrientationModeProp;
        private SerializedProperty m_VelocityInheritanceProp;
        
        #region Unity Events

        private void OnEnable()
        {
            if (target == null) return;
            _movementParent = (CVRMovementParent)target;
            
            m_OrientationModeProp = serializedObject.FindProperty(nameof(CVRMovementParent.orientationMode));
            m_VelocityInheritanceProp = serializedObject.FindProperty(nameof(CVRMovementParent.velocityInheritance));
        }
        
        public override void OnInspectorGUI()
        {
            if (_movementParent == null)
                return;

            serializedObject.Update();

            Draw_ColliderWarning();
            Draw_ParentingSettings();

            serializedObject.ApplyModifiedProperties();
        }
        
        #endregion

        #region Drawing Methods
        
        private void Draw_ColliderWarning()
        {
            if (!_movementParent.TryGetComponent(out Collider _))
                EditorGUILayout.HelpBox("A Collider is required for Movement Parent to function.", MessageType.Warning);
        }
        
        private void Draw_ParentingSettings()
        {
            using (new LabelScope("Parenting Settings"))
            {
                using (new EditorGUI.IndentLevelScope())
                    DrawParentingSettings();
            }
        }

        private void DrawParentingSettings()
        {
            GUILayout.BeginVertical();

            EditorGUILayout.PropertyField(m_OrientationModeProp, new GUIContent("Orientation Mode"));
            EditorGUILayout.PropertyField(m_VelocityInheritanceProp, new GUIContent("Velocity Inheritance"));

            GUILayout.EndVertical();
        }

        #endregion
    }
}
#endif