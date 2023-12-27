using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("")]
    [HelpURL("https://developers.abinteractive.net/cck/components/pointer/")]
    public class CVRAdvancedAvatarSettingsPointer : CVRPointer
    {
        private void OnDrawGizmos()
        {
            if (!isActiveAndEnabled) 
                return;
            
            Gizmos.color = Color.cyan;
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawSphere(Vector3.zero, 0.015f);
        }
    }
}