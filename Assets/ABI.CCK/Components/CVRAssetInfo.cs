using System;
using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [HelpURL("https://developers.abinteractive.net/cck/components/asset-info/")]
    public class CVRAssetInfo : MonoBehaviour, ICCK_Component
    {
        public enum AssetType
        {
            Avatar = 1,
            World = 2,
            Spawnable = 3
        }
        
        public AssetType type;
        public string objectId;

        [HideInInspector]
        public string randomNum;

        // just to make sure
        private void OnValidate() => Reset();
        private void Reset()
        {
            if (TryGetComponent(out CVRAvatar _))
                type = AssetType.Avatar;
            if (TryGetComponent(out CVRWorld _))
                type = AssetType.World;
            if (TryGetComponent(out CVRSpawnable _))
                type = AssetType.Spawnable;
        }
    }
}
