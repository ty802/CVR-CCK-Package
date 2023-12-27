using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ABI.CCK.Components;
using UnityEngine;

namespace ABI.CCK.Scripts.Editor
{
    public static class CCK_ComponentRegistry
    {
        private static readonly HashSet<Type> _monoBehaviourComponentTypes = new HashSet<Type>();
        private static readonly HashSet<Type> _stateMachineBehaviourComponentTypes = new HashSet<Type>();

        static CCK_ComponentRegistry()
        {
            PopulateCCKComponentTypes();
        }

        private static void PopulateCCKComponentTypes()
        {
            // Getting all types that are assignable to ICCK_Component
            Assembly targetAssembly = typeof(ICCK_Component).Assembly;
            var componentTypes = targetAssembly.GetTypes()
                .Where(t => typeof(ICCK_Component).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);

            foreach (Type type in componentTypes)
            {
                if (typeof(MonoBehaviour).IsAssignableFrom(type))
                {
                    _monoBehaviourComponentTypes.Add(type);
                } // StateMachineBehaviour is not a "component", but its used by the CCK
                else if (typeof(StateMachineBehaviour).IsAssignableFrom(type))
                {
                    _stateMachineBehaviourComponentTypes.Add(type);
                }
            }

            //Debug.Log($"Populated MonoBehaviour Component Types: {_monoBehaviourComponentTypes.Count}");
            //Debug.Log($"Populated StateMachineBehaviour Component Types: {_stateMachineBehaviourComponentTypes.Count}");
        }

        public static IEnumerable<Type> GetMonoBehaviourComponentTypes()
        {
            return _monoBehaviourComponentTypes;
        }

        public static IEnumerable<Type> GetStateMachineBehaviourComponentTypes()
        {
            return _stateMachineBehaviourComponentTypes;
        }
    }
}