using UnityEngine;

namespace ABI.CCK.Scripts.Runtime
{
    [AddComponentMenu("")]
    public class LinkOpener : MonoBehaviour
    {
        public void OpenLink(string url)
        {
            Application.OpenURL(url);
        }
    }
}
