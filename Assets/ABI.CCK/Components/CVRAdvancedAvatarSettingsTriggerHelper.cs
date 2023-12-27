using System.Collections.Generic;
using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/CVR Advanced Avatar Trigger Helper")]
    [HelpURL("https://developers.abinteractive.net/cck/components/aas-trigger-helper/")]
    public class CVRAdvancedAvatarSettingsTriggerHelper : MonoBehaviour, ICCK_Component
    {
        public List<CVRAdvancedAvatarSettingsTrigger> triggers = new List<CVRAdvancedAvatarSettingsTrigger>();

        public void onEnter(int i) { }

        public void onExit(int i) { }
        
        public void onStay(int i) { }
    }
}