#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRWorldEditor
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

            DrawReferenceCameraSetting();
            DrawSpawnBehaviourSettings();

            GUILayout.EndVertical();
        }

        #region Drawing Methods

        private void DrawReferenceCameraSetting()
        {
            EditorGUILayout.PropertyField(m_ReferenceCameraProp, new GUIContent("Reference Camera"));
        }

        private void DrawSpawnBehaviourSettings()
        {
            EditorGUILayout.PropertyField(m_SpawnsProp, new GUIContent("Spawns"), true);
            EditorGUILayout.PropertyField(m_SpawnRuleProp, new GUIContent("Spawn Rule"));
            EditorGUILayout.PropertyField(m_RespawnHeightYProp, new GUIContent("Respawn Height Y"));
            EditorGUILayout.PropertyField(m_ObjectRespawnBehaviourProp, new GUIContent("Object Respawn Behaviour"));
        }

        // private void DrawWarpPoints()
        // {
        //     _world.referenceCamera = (GameObject) EditorGUILayout.ObjectField("Reference Camera", _world.referenceCamera, typeof(GameObject), true);
        //     EditorGUILayout.PropertyField(m_WarpPointsProp, new GUIContent("Warp Points"), true);
        //     serializedObject.ApplyModifiedProperties();
        // }

        #endregion
    }
}
#endif