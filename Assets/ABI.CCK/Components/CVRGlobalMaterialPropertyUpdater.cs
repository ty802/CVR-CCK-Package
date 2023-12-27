using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Animations;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/CVR Global Material Property Updater")]
    [HelpURL("https://developers.abinteractive.net/cck/components/global-material-property-updater/")]
    public class CVRGlobalMaterialPropertyUpdater : MonoBehaviour, ICCK_Component
    {

        public enum PropertyType
        {
            // For legacy reasons, this was used for Int type in shader. Needed for legacy content.
            // In reality it represents a float, and should be handled like a float.
            // Newer content will use paramFloat for Int shader types (because it's what it is)
            paramInt = 0,
            paramFloat = 1,
            paramVector4 = 2,
            // The true Integer, this was added in unity 2021. The shader type is Integer
            paramInteger = 3,
        }

        [SerializeField, NotKeyable]
        public Material material;

        [SerializeField, NotKeyable]
        public string propertyName;

        [SerializeField, NotKeyable]
        public PropertyType propertyType = PropertyType.paramFloat;

        // Variables only used to populate the cvr interactable set property value
        [NonSerialized]
        public int intValue;
        [NonSerialized]
        public float floatValue;
        [NonSerialized]
        public Vector4 vector4Value;
        [NonSerialized]
        public int integerValue;

        // Animatable parameters, value changes to these will trigger material updates
        [SerializeField, CVRInteractableActionOperation.HideFromSetPropertyByValue]
        public int intValueAnimatable;
        [SerializeField, CVRInteractableActionOperation.HideFromSetPropertyByValue]
        public float floatValueAnimatable;
        [SerializeField, CVRInteractableActionOperation.HideFromSetPropertyByValue]
        public Vector4 vector4ValueAnimatable;
        [SerializeField, CVRInteractableActionOperation.HideFromSetPropertyByValue]
        public int integerValueAnimatable;

        #if UNITY_EDITOR
        private void OnValidate() {

            if (material == null || material.shader.FindPropertyIndex(propertyName) < 0 || AnimationMode.InAnimationMode()) return;

            // Fetch the default value from the material
            switch (propertyType) {
                case PropertyType.paramInt:
                    intValueAnimatable = material!.GetInt(propertyName);
                    break;
                case PropertyType.paramFloat:
                    floatValueAnimatable = material!.GetFloat(propertyName);
                    break;
                case PropertyType.paramVector4:
                    vector4ValueAnimatable = material!.GetVector(propertyName);
                    break;
                case PropertyType.paramInteger:
                    integerValueAnimatable = material!.GetInteger(propertyName);
                    break;
            }
        }
        #endif
    }
}