using System;
using ABI.CCK.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/Combat System")]
    [HelpURL("https://developers.abinteractive.net/cck/")]
    public class CombatSystem : Health
    {
        [Header("General settings")]
        public bool friendlyFire = false;
        
        public enum RespawnBehavior
        {
            RespawnOnWorld = 0,
            RespawnOnPoint = 1,
            RespawnInPlace = 2,
            None = 3,
        }
        [Header("Respawn Behavior")]
        public RespawnBehavior respawnBehavior = RespawnBehavior.RespawnOnWorld;
        public Transform respawnPoint;
        public float respawnTime = 10f;

        [Header("Events")]
        public UnityEvent playerDownEvent = new UnityEvent();
        public UnityEvent playerHitEvent = new UnityEvent();
        public UnityEvent playerRespawnEvent = new UnityEvent();
        public UnityEvent playerRevitalizeEvent = new UnityEvent();
        
        [Header("PVP Events")]
        public UnityEvent playerDownedEvent = new UnityEvent();
        public UnityEvent downedAnotherPlayerEvent = new UnityEvent();
        public UnityEvent playerGotHitEvent = new UnityEvent();
        public UnityEvent hitAnotherPlayerEvent = new UnityEvent();

        private void Reset()
        {
            referenceID = Guid.NewGuid().ToString();
        }
    }
}