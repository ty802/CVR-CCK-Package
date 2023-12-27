#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRVideoPlayerEditor
    {
        private void Draw_Events()
        {
            using (new FoldoutScope(ref _guiEventsFoldout, "Events"))
            {
                if (!_guiEventsFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                    DrawEvents();
            }
        }

        private void DrawEvents()
        {
            GUILayout.BeginVertical();
        
            EditorGUILayout.PropertyField(m_StartedPlaybackProp, true);
            EditorGUILayout.PropertyField(m_FinishedPlaybackProp, true);
            EditorGUILayout.PropertyField(m_PausedPlaybackProp, true);
            EditorGUILayout.PropertyField(m_SetUrlProp, true);
            
            GUILayout.EndVertical();
        }
    }
}
#endif