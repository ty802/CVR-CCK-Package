using UnityEngine;

namespace ABI.CCK.Components
{
    [AddComponentMenu("ChilloutVR/Damage")]
    [HelpURL("https://developers.abinteractive.net/cck/")]
    public class Damage : MonoBehaviour, ICCK_Component
    {
        public enum DamageType
        {
            Stack = 0,
            Shield = 1,
            Armor = 2,
            Health = 3,
        }

        public DamageType damageType = DamageType.Stack;

        public float damageAmount = 10f;

        [Header("Damage over time")] 
        public float damageOverTimeAmount = 0f;
        public float damageOverTimeDuration = 0f;
        public bool damageOverTimeContact = false;

        [Header("Damage Multiplier")]
        public float healthMultiplier = 1f;
        public float armorMultiplier = 1f;
        public float shieldMultiplier = 1f;
        
        [Header("Damage Falloff")]
        public bool enableFalloff = false;
        public float falloffDistance = 5f;
        public AnimationCurve falloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        public bool falloffEffectDamageOverTime;
    }
}