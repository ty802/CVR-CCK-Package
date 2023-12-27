using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/CVR Description")]
    [HelpURL("https://developers.abinteractive.net/cck/")]
    public class CVRDescription : MonoBehaviour, ICCK_Component
    {
        public string description;
        public string url;
        public bool locked = false;
        public int type = 0;
    }
}