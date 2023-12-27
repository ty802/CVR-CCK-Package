using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/CVR GI Material Updater")]
    [HelpURL("https://developers.abinteractive.net/cck/components/gi-material-updater/")]
    [RequireComponent(typeof(Renderer))]
    public class CVRGIMaterialUpdater : MonoBehaviour, ICCK_Component
    {
#pragma warning disable 649
        [SerializeField] bool updateEveryFrame;
#pragma warning restore 649
        private Renderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            if (_renderer == null || !updateEveryFrame) return;
            _renderer.UpdateGIMaterials();
        }
    }
}