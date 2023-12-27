using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/Fluid Volume")]
    [HelpURL("https://developers.abinteractive.net/cck/")]
    public class FluidVolume : MonoBehaviour, ICCK_Component
    {
        public enum VolumeType
        {
            Box = 1,
            Sphere = 2
        }

        public VolumeType volumeType = VolumeType.Box;

        public Vector2 extend = new Vector2(10f, 10f);
        public float depth = 5f;
        public float density = 1f;

        public bool placeFromCenter = false;
        
        public enum StreamType
        {
            Directional = 1,
            Outwards = 2
        }

        public StreamType streamType = StreamType.Directional;

        public float streamAngle = 0f;
        public float streamStrength = 0f;

        public ParticleSystem splashParticleSystem;

        private BoxCollider _boxCollider;
        private SphereCollider _sphereCollider;
        private Vector3 _streamDirection = Vector3.zero;
        private Vector3 _streamDirectionSide = Vector3.zero;
        private float _streamTime = 0f;
        private Renderer _renderer;

        // Start is called before the first frame update
        void Start()
        {
            GameObject colliderObject = new GameObject("FluidTrigger");

            colliderObject.layer = 4;
            
            if (volumeType == VolumeType.Box)
            {
                colliderObject.transform.SetParent(transform);
                colliderObject.transform.localPosition = Vector3.zero;
                colliderObject.transform.localRotation = Quaternion.identity;
                colliderObject.transform.localScale = Vector3.one;
                _boxCollider = colliderObject.AddComponent<BoxCollider>();
                _boxCollider.isTrigger = true;
                if (placeFromCenter)
                    _boxCollider.center = Vector3.zero;
                else
                    _boxCollider.center = Vector3.down * depth * 0.5f;
                _boxCollider.size = new Vector3(extend.x, depth, extend.y);
            }

            if (volumeType == VolumeType.Sphere)
            {
                colliderObject.transform.localScale = Vector3.one * Mathf.Max(transform.lossyScale.x,
                    transform.lossyScale.y, transform.lossyScale.z);
                colliderObject.transform.SetParent(transform);
                colliderObject.transform.localPosition = Vector3.zero;
                colliderObject.transform.localRotation = Quaternion.identity;
                _sphereCollider = colliderObject.AddComponent<SphereCollider>();
                _sphereCollider.isTrigger = true;
                _sphereCollider.center = Vector3.zero;
                _sphereCollider.radius = depth;
            }

            _renderer = GetComponent<Renderer>();
            if (_renderer == null) _renderer = GetComponentInChildren<Renderer>();

            UpdateStreamDirection();
        }

        // Update is called once per frame
        void Update()
        {
            _streamTime += streamStrength * Time.deltaTime;

            UpdateStreamDirection();
            UpdateRenderer();
        }

        private void OnDrawGizmos()
        {
            UpdateStreamDirection();
            UpdateRenderer(true);

            Gizmos.color = new Color(1f, 1f, 1f, 0.4f);

            if (volumeType == VolumeType.Box)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                if(placeFromCenter)
                    Gizmos.DrawCube(Vector3.zero, new Vector3(extend.x, depth, extend.y));
                else
                    Gizmos.DrawCube(Vector3.down * depth * 0.4999999f, new Vector3(extend.x, depth, extend.y));
            }

            if (volumeType == VolumeType.Sphere)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation,
                    Vector3.one * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z));
                Gizmos.DrawSphere(Vector3.zero, depth);
                Gizmos.DrawWireSphere(Vector3.zero, depth);
            }

            if (streamStrength == 0f) return;

            var size = Mathf.Min(extend.x, extend.y) * 0.1f;
            var length = size * (Mathf.Min(Mathf.Abs(streamStrength), 5f) * 0.05f);

            var localDirection =
                transform.InverseTransformDirection(_streamDirection * (streamStrength < 0f ? -1f : 1f));
            var localDirectionSide =
                transform.InverseTransformDirection(_streamDirectionSide * (streamStrength < 0f ? -1f : 1f));
            Gizmos.DrawLine((localDirection * 2f + localDirectionSide * 2f) * -size,
                localDirection * 4f * size * length);
            Gizmos.DrawLine((localDirection * 2f + localDirectionSide * -2f) * -size,
                localDirection * 4f * size * length);
            Gizmos.DrawLine((localDirection * 2f + localDirectionSide * 2f) * -size, localDirection * -1f * size);
            Gizmos.DrawLine((localDirection * 2f + localDirectionSide * -2f) * -size, localDirection * -1f * size);
        }

        private void UpdateRenderer(bool get = false)
        {
            if (get) _renderer = GetComponent<Renderer>();

            if (_renderer != null)
            {
                _renderer.sharedMaterial.SetVector("_StreamDirection", _streamDirection);
                _renderer.sharedMaterial.SetFloat("_StreamStrength", streamStrength);
                _renderer.sharedMaterial.SetFloat("_StreamTime", _streamTime);
            }
        }

        private void UpdateStreamDirection()
        {
            Quaternion rot = Quaternion.AngleAxis(streamAngle, Vector3.up);
            Vector3 lDirection = rot * Vector3.forward;
            _streamDirection = transform.TransformDirection(lDirection);
            lDirection = rot * Vector3.right;
            _streamDirectionSide = transform.TransformDirection(lDirection);
        }
    }
}
