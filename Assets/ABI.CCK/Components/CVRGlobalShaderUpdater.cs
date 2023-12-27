using System;
using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/CVR Global Shader Updater")]
    [HelpURL("https://developers.abinteractive.net/cck/components/global-shader-updater/")]
    public class CVRGlobalShaderUpdater : MonoBehaviour, ICCK_Component
    {
        public bool updateValues = true;
        
        public Vector4 CVR_CCK_Global_1 = Vector4.zero;
        public Vector4 CVR_CCK_Global_2 = Vector4.zero;
        public Vector4 CVR_CCK_Global_3 = Vector4.zero;
        public Vector4 CVR_CCK_Global_4 = Vector4.zero;

        public bool updateTexture = false;
        
        public RenderTexture renderTexture;
        public string propertyName;

        private void Update()
        {
            
        }
    }
}