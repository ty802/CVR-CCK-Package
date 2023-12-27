#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRVideoPlayerEditor
    {
        private void Draw_GeneralSettings()
        {
            using (new FoldoutScope(ref _guiGeneralSettingsFoldout, "General Settings"))
            {
                if (!_guiGeneralSettingsFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawGeneralSettings();
            }
        }

        private void DrawGeneralSettings()
        {
            GUILayout.BeginVertical();
            
            EditorGUILayout.PropertyField(m_SyncEnabledProp, new GUIContent("Network Sync"));
            EditorGUILayout.PropertyField(m_InteractiveUIProp, new GUIContent("Use Interactive Library UI"));
            EditorGUILayout.PropertyField(m_VideoPlayerUIPositionProp, new GUIContent("UI Position/Parent"));
            
            //EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_ProjectionTextureProp, new GUIContent("Projection Texture"));
            if (m_ProjectionTextureProp.objectReferenceValue == null)
            {
                Separator();
                using (new SetIndentLevelScope(0))
                {
                    EditorGUILayout.HelpBox("The video player output texture is empty, please fill it or no video will be drawn.", MessageType.Warning);
                    if (GUILayout.Button("Auto Create Render Texture"))
                    {
                        RenderTexture tex = new RenderTexture(1920, 1080, 24);
                        if (!AssetDatabase.IsValidFolder("Assets/ABI.Generated"))
                            AssetDatabase.CreateFolder("Assets", "ABI.Generated");
                        if (!AssetDatabase.IsValidFolder("Assets/ABI.Generated/VideoPlayer"))
                            AssetDatabase.CreateFolder("Assets/ABI.Generated", "VideoPlayer");
                        if (!AssetDatabase.IsValidFolder("Assets/ABI.Generated/VideoPlayer/RenderTextures"))
                            AssetDatabase.CreateFolder("Assets/ABI.Generated/VideoPlayer", "RenderTextures");
                        AssetDatabase.CreateAsset(tex,
                            "Assets/ABI.Generated/VideoPlayer/RenderTextures/" + ((MonoBehaviour)serializedObject.targetObject).gameObject.GetInstanceID() + ".renderTexture");
                        m_ProjectionTextureProp.objectReferenceValue = tex;
                    }
                }
            }
            
            GUILayout.EndVertical();
        }
    }
}
#endif