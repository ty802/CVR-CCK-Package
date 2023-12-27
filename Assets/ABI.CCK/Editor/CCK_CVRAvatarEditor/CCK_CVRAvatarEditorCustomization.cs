#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRAvatarEditor
    {
        private void Draw_AvatarCustomization()
        {
            using (new FoldoutScope(ref _guiAvatarCustomizationFoldout, "Avatar Customization"))
            {
                if (!_guiAvatarCustomizationFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawAvatarCustomization();
            }
        }

        #region Drawing Methods

        private void DrawAvatarCustomization()
        {
            EditorGUILayout.PropertyField(m_OverridesProp, new GUIContent("Animation Overrides"));

            EditorGUILayout.HelpBox(CCKLocalizationProvider.GetLocalizedText("ABI_UI_AVATAR_INFO_OVERRIDE_CONTROLLER"), MessageType.Info);

            Object previousBodyMesh = m_BodyMeshProp.objectReferenceValue;
            EditorGUILayout.PropertyField(m_BodyMeshProp, new GUIContent("Face Mesh"));
            if (m_BodyMeshProp.objectReferenceValue != previousBodyMesh)
                GetBlendshapeNames();
        }

        #endregion
    }
}
#endif