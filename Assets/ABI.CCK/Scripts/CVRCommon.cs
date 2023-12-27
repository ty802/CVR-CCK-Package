using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ABI.CCK.Scripts
{
    public static class CVRCommon
    {
        public const string LOCAL_PARAMETER_PREFIX = "#";
        public const string LOCOMOTION_EMOTES_LAYER_NAME = "Locomotion/Emotes";
        public const string HAND_LEFT_LAYER_NAME = "LeftHand";
        public const string HAND_RIGHT_LAYER_NAME = "RightHand";
        public const string TOGGLES_LAYER_NAME = "Toggles";

        public const float AVATAR_BIT_LIMIT = 3200f;
        public const float SPAWNABLE_BIT_LIMIT = 40f;

        // Core parameters used by Avatars
        public static readonly string[] CoreParameters =
        {
            "MovementX", "MovementY", "Grounded", "Emote", "GestureLeft",
            "GestureRight", "Toggle", "Sitting", "Crouching",
            "CancelEmote", "Prone", "Flying", "Swimming"
        };
        
        // Default pointer types used by the Pointers & Triggers system
        public static readonly string[] DefaultPointerTypes =
        {
            "mouth", "index", "grab" // generic is no type, but cant display *nothing* in the advdropdown
        };

        // -1f to 6f, 0-1f is Fist weight
        public enum GestureIndex
        {
            HandOpen,
            Neutral,
            Fist,
            ThumbsUp,
            HandGun,
            Fingerpoint,
            Victory,
            RockNRoll
        }

#if UNITY_EDITOR

        // This is all jank :>

        public static bool NonCoreFilter(AnimatorControllerParameter param)
        {
            return !CoreParameters.Contains(param.name);
        }

        public static bool NonNullFilter(AnimatorControllerParameter param)
        {
            return !string.IsNullOrWhiteSpace(param.name);
        }

        public static bool NonLocalFilter(AnimatorControllerParameter param)
        {
            return !param.name.StartsWith(LOCAL_PARAMETER_PREFIX);
        }

        private static IEnumerable<T> FilterParameters<T>(AnimatorControllerParameter[] parameters,
            Func<AnimatorControllerParameter, T> selector, params Predicate<AnimatorControllerParameter>[] filters)
        {
            return parameters.Where(p =>
                {
                    if (!NonNullFilter(p))
                        return false;

                    foreach (var filter in filters)
                        if (filter != null && !filter(p))
                            return false;

                    return true;
                })
                .Select(selector);
        }

        public static List<AnimatorControllerParameter> GetParametersFromController(
            RuntimeAnimatorController controller, params Predicate<AnimatorControllerParameter>[] filters)
        {
            UnityEditor.Animations.AnimatorController animatorController;

            if (controller is AnimatorOverrideController overrideController)
                animatorController = overrideController.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            else
                animatorController = controller as UnityEditor.Animations.AnimatorController;

            return animatorController == null
                ? new List<AnimatorControllerParameter>()
                : FilterParameters(animatorController.parameters, p => p, filters).ToList();
        }

        public static List<string> GetParametersFromControllerAsString(RuntimeAnimatorController controller,
            params Predicate<AnimatorControllerParameter>[] filters)
        {
            UnityEditor.Animations.AnimatorController animatorController;

            if (controller is AnimatorOverrideController overrideController)
                animatorController = overrideController.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            else
                animatorController = controller as UnityEditor.Animations.AnimatorController;

            return animatorController == null
                ? new List<string>()
                : FilterParameters(animatorController.parameters, p => p.name, filters).ToList();
        }

        public static List<AnimatorControllerParameter> GetParametersFromAnimator(Animator animator,
            params Predicate<AnimatorControllerParameter>[] filters)
        {
            if (animator == null || animator.runtimeAnimatorController == null)
                return new List<AnimatorControllerParameter>();

            UnityEditor.Animations.AnimatorController animatorController = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            if (animator.runtimeAnimatorController is AnimatorOverrideController overrideController)
                animatorController = overrideController.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

            return animatorController == null
                ? new List<AnimatorControllerParameter>()
                : FilterParameters(animatorController.parameters, p => p, filters).ToList();
        }

        public static List<string> GetParametersFromAnimatorAsString(Animator animator,
            params Predicate<AnimatorControllerParameter>[] filters)
        {
            if (animator == null || animator.runtimeAnimatorController == null)
                return new List<string>();

            UnityEditor.Animations.AnimatorController animatorController = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            if (animator.runtimeAnimatorController is AnimatorOverrideController overrideController)
                animatorController = overrideController.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

            return animatorController == null
                ? new List<string>()
                : FilterParameters(animatorController.parameters, p => p.name, filters).ToList();
        }
#endif
    }
}