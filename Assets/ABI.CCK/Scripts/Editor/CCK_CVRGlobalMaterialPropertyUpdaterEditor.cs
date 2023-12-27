using System;
using ABI.CCK.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ABI.CCK.Scripts.Editor
{
    [CustomEditor(typeof(CVRGlobalMaterialPropertyUpdater))]
    public class CCK_CVRGlobalMaterialPropertyUpdaterEditor : UnityEditor.Editor
    {
        private SerializedProperty _materialProp;
        private SerializedProperty _propertyNameProp;
        private SerializedProperty _propertyTypeProp;
        private SerializedProperty _intValueAnimatableProp;
        private SerializedProperty _floatValueAnimatableProp;
        private SerializedProperty _vector4ValueAnimatableProp;
        private SerializedProperty _integerValueAnimatableProp;

        private string[] _materialPropertyNames;
        private int _selectedPropertyIndex = -1;

        private void OnEnable()
        {
            // Cache the SerializedProperties
            _materialProp = serializedObject.FindProperty("material");
            _propertyNameProp = serializedObject.FindProperty("propertyName");
            _propertyTypeProp = serializedObject.FindProperty("propertyType");
            _intValueAnimatableProp = serializedObject.FindProperty("intValueAnimatable");
            _floatValueAnimatableProp = serializedObject.FindProperty("floatValueAnimatable");
            _vector4ValueAnimatableProp = serializedObject.FindProperty("vector4ValueAnimatable");
            _integerValueAnimatableProp = serializedObject.FindProperty("integerValueAnimatable");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw the material field
            EditorGUILayout.PropertyField(_materialProp);

            if (_materialProp.objectReferenceValue != null)
            {
                Material material = _materialProp.objectReferenceValue as Material;
                Shader shader = material!.shader;
                _materialPropertyNames = GetFilteredMaterialPropertyNames(shader);
                _selectedPropertyIndex = Mathf.Max(0, Array.IndexOf(_materialPropertyNames, _propertyNameProp.stringValue));

                if (_materialPropertyNames.Length > 0)
                {
                    _selectedPropertyIndex = EditorGUILayout.Popup("Property Name", _selectedPropertyIndex, _materialPropertyNames);
                    _propertyNameProp.stringValue = _materialPropertyNames[_selectedPropertyIndex];
                    _propertyTypeProp.enumValueIndex = (int)GetPropertyType(shader, _propertyNameProp.stringValue);

                    // Display the property type as a label
                    string propertyTypeLabel = ((CVRGlobalMaterialPropertyUpdater.PropertyType)_propertyTypeProp.enumValueIndex) switch
                    {
                        CVRGlobalMaterialPropertyUpdater.PropertyType.paramInt => "Int (actually a float)",
                        CVRGlobalMaterialPropertyUpdater.PropertyType.paramFloat => "Float",
                        CVRGlobalMaterialPropertyUpdater.PropertyType.paramVector4 => "Vector",
                        CVRGlobalMaterialPropertyUpdater.PropertyType.paramInteger => "Integer",
                        _ => "Unknown"
                    };
                    EditorGUILayout.LabelField("Property Type", propertyTypeLabel);

                    switch ((CVRGlobalMaterialPropertyUpdater.PropertyType)_propertyTypeProp.enumValueIndex)
                    {
                        case CVRGlobalMaterialPropertyUpdater.PropertyType.paramInt:
                            EditorGUILayout.PropertyField(_intValueAnimatableProp, new GUIContent("Property Fake Integer (Float) Value"));
                            break;
                        case CVRGlobalMaterialPropertyUpdater.PropertyType.paramFloat:
                            EditorGUILayout.PropertyField(_floatValueAnimatableProp, new GUIContent("Property Float Value"));
                            break;
                        case CVRGlobalMaterialPropertyUpdater.PropertyType.paramVector4:
                            EditorGUILayout.PropertyField(_vector4ValueAnimatableProp, new GUIContent("Property Vector Value"));
                            break;
                        case CVRGlobalMaterialPropertyUpdater.PropertyType.paramInteger:
                            EditorGUILayout.PropertyField(_integerValueAnimatableProp, new GUIContent("Property Integer Value"));
                            break;
                    }

                    EditorGUILayout.HelpBox("The material property initial value is used as the default value.", MessageType.Info);
                    EditorGUILayout.HelpBox("You can animate the Value via animation clips to change it. It will detect when the property is animated, and only then it update it. It can NOT be used to set a default value!", MessageType.Info);
                    EditorGUILayout.HelpBox("In worlds you can also changed the Value using the component CVR Interactable Action: \"Set Property By Value\"", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("No compatible shader properties found.", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Please assign a Material to use this component.", MessageType.Error);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static string[] GetFilteredMaterialPropertyNames(Shader shader)
        {
            var props = new System.Collections.Generic.List<string>();
            for (int i = 0; i < shader.GetPropertyCount(); i++)
            {
                var propType = shader.GetPropertyType(i);
                if (propType
                    is ShaderPropertyType.Float
                    or ShaderPropertyType.Range
                    or ShaderPropertyType.Vector
                    or ShaderPropertyType.Int)
                {
                    props.Add(shader.GetPropertyName(i));
                }
            }
            return props.ToArray();
        }

        private static CVRGlobalMaterialPropertyUpdater.PropertyType GetPropertyType(Shader shader, string propertyName)
        {
            int index = shader.FindPropertyIndex(propertyName);
            if (index < 0) return CVRGlobalMaterialPropertyUpdater.PropertyType.paramFloat;

            ShaderPropertyType shaderPropertyType = shader.GetPropertyType(index);
            return shaderPropertyType switch
            {
                ShaderPropertyType.Float or ShaderPropertyType.Range => CVRGlobalMaterialPropertyUpdater.PropertyType.paramFloat,
                ShaderPropertyType.Vector => CVRGlobalMaterialPropertyUpdater.PropertyType.paramVector4,
                ShaderPropertyType.Int => CVRGlobalMaterialPropertyUpdater.PropertyType.paramInteger,
                _ => CVRGlobalMaterialPropertyUpdater.PropertyType.paramFloat
            };
        }
    }
}