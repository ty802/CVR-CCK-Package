using System.Reflection;
using UnityEditor;
using System;

namespace ABI.CCK.Scripts.Editor
{
    public static class CCK_GizmoFix
    {
        // TODO: Unity 2022.1 will have a new API for this, so we can remove this reflection hack.
        // https://docs.unity3d.com/2022.2/Documentation/ScriptReference/GizmoUtility.SetGizmoEnabled.html
        
        #region SetGizmoIconEnabled Reflection Hack

        private static MethodInfo _setIconEnabledMethod;
    
        private static MethodInfo SetIconEnabledMethod
        {
            get
            {
                if (_setIconEnabledMethod != null)
                    return _setIconEnabledMethod;
            
                _setIconEnabledMethod = Assembly.GetAssembly(typeof(UnityEditor.Editor))
                    ?.GetType("UnityEditor.AnnotationUtility")
                    ?.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);

                return _setIconEnabledMethod;
            }
        }

        private static void SetGizmoIconEnabled(Type type, bool on)
        {
            if (SetIconEnabledMethod == null) return;

            const int MONO_BEHAVIOR_CLASS_ID = 114; // https://docs.unity3d.com/Manual/ClassIDReference.html
            // "Annotation not found" - type has no custom gizmo icon set.
            SetIconEnabledMethod.Invoke(null, new object[] { MONO_BEHAVIOR_CLASS_ID, type.Name, on ? 1 : 0 });
        }

        #endregion

        [MenuItem("Alpha Blend Interactive/Utilities/Gizmo Icons/Show All", false, 250)]
        private static void ShowAllGizmos() => SetGizmoIcons(true);
        
        [MenuItem("Alpha Blend Interactive/Utilities/Gizmo Icons/Hide All", false, 250)]
        private static void HideAllGizmos() => SetGizmoIcons(false);
        
        private static void SetGizmoIcons(bool show)
        {
            bool proceed = EditorUtility.DisplayDialog(
                "CCK :: Gizmo Fix",
                $"Are you sure you want to {(show ? "enable" : "disable")} all Gizmo icons? This will {(show ? "show" : "hide")} all `CVR` component icons in the scene view.",
                "Yes", "No");

            if (!proceed)
                return;
             
            foreach (Type type in CCK_ComponentRegistry.GetMonoBehaviourComponentTypes())
                SetGizmoIconEnabled(type, show);
        }
    }
}