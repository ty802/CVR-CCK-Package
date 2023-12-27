using System;
using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/CVR Movement Parent")]
    [HelpURL("https://developers.abinteractive.net/cck/")]
    public class CVRMovementParent : MonoBehaviour, ICCK_Component
    {
        public enum VelocityInheritanceMode
        {
            None = 0,
            Parent = 1,
            Reference = 2
        }

        public enum OrientationMode
        {
            Disabled = 0,
            RotateWithParent = 1
        }
        
        [Tooltip("Controls whether the player should rotate with the moving platform.")]
        public OrientationMode orientationMode = OrientationMode.RotateWithParent;
    
        [Tooltip("Controls how the player inherits velocity from the moving platform.")]
        public VelocityInheritanceMode velocityInheritance = VelocityInheritanceMode.Parent;
        
        // to make the enabled checkbox display
        private void OnEnable(){}
    }
}