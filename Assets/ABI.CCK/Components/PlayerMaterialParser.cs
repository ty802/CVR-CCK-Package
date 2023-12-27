using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/Player Material Parser")]
    [HelpURL("https://developers.abinteractive.net/cck/")]
    public class PlayerMaterialParser : MonoBehaviour, ICCK_Component
    {
        public Material targetMaterial;

        public string playerRootPositions = "_PlayerRootPositions";
        public string playerHipPositions = "_PlayerHipPositions";
        public string playerHeadPositions = "_PlayerHeadPositions";
        public string playerLeftHandPositions = "_PlayerLeftHandPositions";
        public string playerRightHandPositions = "_PlayerRightHandPositions";
        public string playerChestPositions = "_PlayerChestPositions";
        public string playerLeftFootPositions = "_PlayerLeftFootPositions";
        public string playerRightFootPositions = "_PlayerRightFootPositions";
    }
}
