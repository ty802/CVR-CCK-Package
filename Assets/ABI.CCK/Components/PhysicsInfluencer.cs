using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/Physics Influencer")]
    [HelpURL("https://developers.abinteractive.net/cck/")]
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class PhysicsInfluencer : MonoBehaviour, ICCK_Component
    {
        public bool guiCenterOfMassFoldout;
        public bool changeCenterOfMass;

        public Transform centerOfMass;
        public bool updateContinuously;


        public bool guiBuoyancyFoldout;
        public bool enableBuoyancy = false;

        public float density;
        public float volume;

        public float airDrag = 0f;
        public float airAngularDrag = 0.05f;

        public float fluidDrag = 3f;
        public float fluidAngularDrag = 1f;


        public bool guiGravityFoldout;
        public bool enableLocalGravity;

        public bool forceAlignUpright;


        public bool guiMovementParentFoldout;
        public bool enableMovementParent;

        public bool ignoreForcesWhileParented;

        private Rigidbody _rigidbody;
        private Collider[] _colliders;

        private void Reset()
        {
            UpdateDensity();
        }

        public void UpdateDensity()
        {
            _rigidbody = GetComponent<Rigidbody>();

            float mass = _rigidbody.mass;
            _rigidbody.SetDensity(1f);
            volume = _rigidbody.mass;
            density = (mass / 1000f) / volume;

            _rigidbody.mass = mass;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
