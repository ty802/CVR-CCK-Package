using System.Collections.Generic;
using ABI.CCK.Components;
using UnityEditor;
using UnityEngine;

namespace ABI.CCK.Scripts.Editor
{
    [CustomEditor(typeof(PhysicsInfluencer))]
    public class CCK_PhysicsInfluencerEditor : UnityEditor.Editor
    {
        private GUIStyle _boldFoldoutStyle;
        
        private PhysicsInfluencer _influencer;
        private void OnEnable()
        {
            if (target == null) return;
            _influencer = (PhysicsInfluencer)target;
        }
        
        public override void OnInspectorGUI()
        {
            if (_influencer == null)
                return;
            
            if (_boldFoldoutStyle == null)
                _boldFoldoutStyle = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };

            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            
            using (new SharedComponentGUI.ToggleFoldoutScope(ref _influencer.guiCenterOfMassFoldout, ref _influencer.changeCenterOfMass,
                       "Change Center Of Mass", _boldFoldoutStyle))
            {
                if (_influencer.guiCenterOfMassFoldout)
                    using (new EditorGUI.IndentLevelScope())
                using (new EditorGUI.DisabledGroupScope(!_influencer.changeCenterOfMass))
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("centerOfMass"), new GUIContent("Center Of Mass"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("updateContinuously"), new GUIContent("Update Continuously"));
                }
            }
            
            EditorGUILayout.Space();
            
            using (new SharedComponentGUI.ToggleFoldoutScope(ref _influencer.guiBuoyancyFoldout, ref _influencer.enableBuoyancy,
                       "Buoyancy", _boldFoldoutStyle))
            {
                if (_influencer.guiBuoyancyFoldout)
                    using (new EditorGUI.IndentLevelScope())
                using (new EditorGUI.DisabledGroupScope(!_influencer.enableBuoyancy))
                {
                    List<MeshCollider> collider = new List<MeshCollider>(_influencer.gameObject.GetComponents<MeshCollider>());

                    if (collider.Find(m => !m.convex) != null)
                    {
                        EditorGUILayout.HelpBox("Concave Mesh colliders will not be include in the volume calculation!", MessageType.Warning);
                    }
                    
                    bool guiWasEnabled = GUI.enabled;
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("density"), new GUIContent("Density"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"), new GUIContent("Volume"));
                    GUI.enabled = guiWasEnabled;

                    Rect buttonRect = EditorGUILayout.GetControlRect();
                    buttonRect.x += 15;
                    buttonRect.width -= 15;
                    if (GUI.Button(buttonRect, "Recalculate Density and Volume"))
                        _influencer.UpdateDensity();
                    
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("airDrag"), new GUIContent("Air Drag"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("airAngularDrag"), new GUIContent("Air Angular Drag"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("fluidDrag"), new GUIContent("Fluid Drag"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("fluidAngularDrag"), new GUIContent("Fluid Angular Drag"));
                }
            }
            /*
            using (new SharedComponentGUI.ToggleFoldoutScope(ref _influencer.guiGravityFoldout, ref _influencer.enableLocalGravity,
                       "Local Gravity", _boldFoldoutStyle))
            {
                if (_influencer.guiGravityFoldout)
                    using (new EditorGUI.IndentLevelScope())
                using (new EditorGUI.DisabledGroupScope(!_influencer.enableLocalGravity))
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("forceAlignUpright"), new GUIContent("Force Align Upright"));
                }
            }
            
            using (new SharedComponentGUI.ToggleFoldoutScope(ref _influencer.guiMovementParentFoldout, ref _influencer.enableMovementParent,
                       "Movement Parent", _boldFoldoutStyle))
            {
                if (_influencer.guiMovementParentFoldout)
                    using (new EditorGUI.IndentLevelScope())
                using (new EditorGUI.DisabledGroupScope(!_influencer.enableMovementParent))
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ignoreForcesWhileParented"), new GUIContent("Ignore Forces While Parented"));
                }
            }*/
            
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(_influencer);
        }
    }
}