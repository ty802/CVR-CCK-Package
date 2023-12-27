using ABI.CCK.Components;
using UnityEngine;

[AddComponentMenu("ChilloutVR/CVR Blitter")]
[HelpURL("https://developers.abinteractive.net/cck/components/blitter/")]
public class CVRBlitter : MonoBehaviour, ICCK_Component
{
#pragma warning disable 414
    [SerializeField] RenderTexture originTexture;
    [SerializeField] RenderTexture destinationTexture;
    [SerializeField] Material blitMaterial;
#pragma warning restore 414
    public bool clearEveryFrame;
}