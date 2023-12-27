#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRAdvancedAvatarSettingsTriggerEditor
    {
        private void Draw_FilterSettings()
        {
            using (new FoldoutScope(ref _guiInteractionFilterFoldout, "Interaction Filter"))
            {
                if (!_guiInteractionFilterFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawFilterSettings();
            }
        }

        private void DrawFilterSettings()
        {
            GUILayout.BeginVertical();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isLocalInteractable"), new GUIContent("Local Interaction"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isNetworkInteractable"), new GUIContent("Network Interaction"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("allowParticleInteraction"), new GUIContent("Particle Interaction"));
            //EditorGUILayout.HelpBox(CCKLocalizationProvider.GetLocalizedText("ABI_UI_ADVAVTR_TRIGGER_PARTICLE_HELPBOX"), MessageType.Info);
            GUILayout.EndVertical();
        }
    }
}
#endif