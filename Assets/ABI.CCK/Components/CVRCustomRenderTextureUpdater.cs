using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/CVR Custom Render Texture Updater")]
    [HelpURL("https://developers.abinteractive.net/cck/components/custom-render-texture-updater/")]
    public class CVRCustomRenderTextureUpdater : MonoBehaviour, ICCK_Component
    {
        public CustomRenderTexture customRenderTexture;

        private void Update()
        {
            customRenderTexture.Update();
        }
    }
}