using System.Collections.Generic;
using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/CVR Object Sync")]
    [HelpURL("https://developers.abinteractive.net/cck/components/object-sync/")]
    public class CVRObjectSync : MonoBehaviour, ICCK_Component
    {

        [HideInInspector] 
        public string syncOwner;
        
        [HideInInspector]
        public string guid = "";

        
    }
}
