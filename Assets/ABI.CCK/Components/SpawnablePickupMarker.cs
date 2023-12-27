using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/Spawnable Pickup Marker")]
    [HelpURL("https://developers.abinteractive.net/cck/")]
    public class SpawnablePickupMarker : MonoBehaviour, ICCK_Component
    {
        public string spawnableGuid;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawWireCube(new Vector3(0, 0.75f, 0), new Vector3(1f, 1.5f, 0f));
            
            Gizmos.DrawWireCube(new Vector3(0, 0.7f, 0), new Vector3(0.8f, 0.1f, 0f));
            Gizmos.DrawWireCube(new Vector3(0, 0.615f, 0), new Vector3(0.6f, 0.07f, 0f));
            Gizmos.DrawWireCube(new Vector3(0.24f, 0.28f, 0), new Vector3(0.32f, 0.42f, 0f));
            Gizmos.DrawWireCube(new Vector3(-0.24f, 0.28f, 0), new Vector3(0.32f, 0.42f, 0f));
            var scale = transform.lossyScale;
            scale.Scale(new Vector3(1f, 1f, 0f));
            rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, scale);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawWireSphere(new Vector3(0, 1.11f, 0), 0.31f);
        }
    }
}