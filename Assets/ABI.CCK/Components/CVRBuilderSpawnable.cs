using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("")]
    [HelpURL("https://developers.abinteractive.net/cck/")]
    public class CVRBuilderSpawnable : MonoBehaviour, ICCK_Component
    {
        private void Reset()
        {
            if (GetComponent<CVRSpawnable>() != null)
                DestroyImmediate(this);
        }
    }
}