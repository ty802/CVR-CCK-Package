using ABI.CCK.Components;
using UnityEngine;

[AddComponentMenu("")]
[HelpURL("https://developers.abinteractive.net/cck/")]
[System.Serializable]
public class CVRWarpPoint : MonoBehaviour, ICCK_Component
{
    [Header("CVR Warp Point (Will teleport you to the position of this object on ui interaction.)")]
    public string warpPointName;
    public string warpPointDescription;
}