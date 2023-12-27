using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/CVR Material Updater")]
    [HelpURL("https://developers.abinteractive.net/cck/components/material-updater/")]
    public class CVRMaterialUpdater : MonoBehaviour, ICCK_Component
    {
        public enum UpdateType
        {
            Update = 0,
            FixedUpdate = 1,
        }

        public UpdateType updateType = UpdateType.Update;

        private Renderer render;
        private Vector3 lastPos;
        private Vector3 velocity;
        private Vector3 lastRot;  
        private Vector3 angularVelocity;
        private static readonly int _cvrVelocity = Shader.PropertyToID("_CVR_Velocity");
        private static readonly int _cvrAngularVelocity = Shader.PropertyToID("_CVR_Angular_Velocity");

        private void Start()
        {
            render = GetComponent<Renderer>();
        }

        private void Update()
        {
            if (updateType == UpdateType.FixedUpdate || render == null) return;
            ProcessUpdate();
        }

        private void FixedUpdate()
        {
            if (updateType == UpdateType.Update || render == null) return;
            ProcessUpdate();
        }

        private void ProcessUpdate()
        {
            velocity = (lastPos - transform.position) / (updateType == UpdateType.Update?Time.deltaTime:Time.fixedDeltaTime);
            Quaternion rotation = transform.rotation;
            angularVelocity = rotation.eulerAngles - lastRot;
            
            render.material.SetVector(_cvrVelocity, velocity);
            render.material.SetVector(_cvrAngularVelocity, angularVelocity);
            
            lastPos = transform.position;
            lastRot = rotation.eulerAngles;
        }
    }
}