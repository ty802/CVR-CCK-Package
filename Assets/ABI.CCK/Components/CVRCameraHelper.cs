using System.Collections.Generic;
using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/CVR Camera Helper")]
    [HelpURL("https://developers.abinteractive.net/cck/")]
    public class CVRCameraHelper : MonoBehaviour, ICCK_Component
    {
        public Camera cam;
        public bool setAsMirroringCamera;

        public List<Shader> replacementShaders = new List<Shader>();
        public int selectedShader = -1;
        
        public void TakeScreenshot()
        {
            
        }
    }
}