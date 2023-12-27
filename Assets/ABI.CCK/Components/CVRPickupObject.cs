using System;
using System.Collections.Generic;
using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/CVR Pickup Object")]
    [HelpURL("https://developers.abinteractive.net/cck/components/pickup-object/")]
    public class CVRPickupObject : MonoBehaviour, ICCK_Component
    {
        public enum GripType
        {
            Free = 1,
            Origin = 2
        }
        
        public GripType gripType = GripType.Free;
        public Transform gripOrigin;

        public bool disallowTheft;
        
        public float maximumGrabDistance = 0f;
        
        public List<SnappingReference> snappingReferences = new List<SnappingReference>();

        public bool autoHold = false;
        public bool updateWithPhysics = true;

        public Transform ikReference;
        
        private void OnDrawGizmos()
        {
            if (gripType == GripType.Origin && gripOrigin != null)
            {
                var t = gripOrigin.transform;
                var s = t.lossyScale;
                
                Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, Vector3.one);
                Gizmos.color = Color.white;
                
                Gizmos.DrawWireCube(new Vector3(0f, 0f, -0.15f), new Vector3(0.1f, 0.1f, 0.2f));
                Gizmos.DrawWireCube(new Vector3(-0.075f, 0f, 0f), new Vector3(0.05f, 0.2f, 0.2f));
                Gizmos.DrawWireCube(new Vector3(0f, 0f, 0.1f), new Vector3(0.1f, 0.2f, 0.05f));
                Gizmos.DrawWireCube(new Vector3(0.07f, 0.05f, 0f), new Vector3(0.05f, 0.05f, 0.15f));
            }
        }
    }
    
    [System.Serializable]
    public class SnappingReference
    {
        public Transform referencePoint;
        public string allowedType;
    }
}