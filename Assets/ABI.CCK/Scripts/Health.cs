using UnityEngine;
using UnityEngine.Events;

namespace ABI.CCK.Scripts
{
    [AddComponentMenu("")]
    public class Health : MonoBehaviour
    {
        public string referenceID = string.Empty;

        [Header("Health")] 
        public float healthBaseAmount = 100f;
        public float healthMaxAmount = 100f;
        [Header("Health Regeneration")] 
        public float healthRegenerationDelay;
        public float healthRegenerationRate;
        public float healthRegenerationCap;

        [Header("Armor")] 
        public float armorBaseAmount;
        public float armorMaxAmount;
        [Header("Armor Regeneration")] 
        public float armorRegenerationDelay;
        public float armorRegenerationRate;
        public float armorRegenerationCap;

        [Header("Shield")] 
        public float shieldBaseAmount;
        public float shieldMaxAmount;
        [Header("Shield Regeneration")] 
        public float shieldRegenerationDelay;
        public float shieldRegenerationRate;
        public float shieldRegenerationCap;

        [HideInInspector] 
        public UnityEvent damageReceivedEvent = new();
    }
}