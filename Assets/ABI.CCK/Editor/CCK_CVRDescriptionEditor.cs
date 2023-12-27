using ABI.CCK.Components;
using UnityEditor;
using UnityEngine;

namespace ABI.CCK.Scripts.Editor
{
    [CustomEditor(typeof(CVRDescription))]
    public class CCK_CVRDescriptionEditor : UnityEditor.Editor
    {
        private CVRDescription _description;

        #region Unity Events

        private void OnEnable()
        {
            if (target == null) return;
            _description = (CVRDescription)target;
        }

        public override void OnInspectorGUI()
        {
            if (_description == null)
                return;

            EditorGUI.BeginChangeCheck();

            if (_description.locked)
                DrawLockedDescription();
            else
                DrawEditableDescription();

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(_description);
        }

        #endregion

        #region GUI Drawing

        private void DrawLockedDescription()
        {
            GUIStyle helpBoxStyle = new GUIStyle(EditorStyles.helpBox) 
            {
                padding = new RectOffset(10, 10, 10, 10)
            };

            GUIStyle descriptionStyle = new GUIStyle(EditorStyles.label) 
            {
                richText = true,
                wordWrap = true
            };

            EditorGUILayout.BeginVertical(helpBoxStyle);
            EditorGUILayout.BeginHorizontal();
        
            GUIContent iconContent = GUIContent.none;
            switch (_description.type)
            {
                case 1: // Info
                    iconContent = EditorGUIUtility.IconContent("console.infoicon");
                    break;
                case 2: // Warning
                    iconContent = EditorGUIUtility.IconContent("console.warnicon");
                    break;
                case 3: // Error
                    iconContent = EditorGUIUtility.IconContent("console.erroricon");
                    break;
            }

            if (iconContent != GUIContent.none)
            {
                GUILayout.Label(iconContent, GUILayout.Width(40), GUILayout.Height(40));
                GUILayout.Space(10);
            }

            using (new EditorGUILayout.ScrollViewScope(Vector2.zero, GUILayout.ExpandWidth(true)))
                EditorGUILayout.LabelField(_description.description, descriptionStyle); 
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (!string.IsNullOrEmpty(_description.url) && GUILayout.Button("Read more about this topic"))
                Application.OpenURL(_description.url);
        }
            
        private void DrawEditableDescription()
        {
            EditorGUILayout.LabelField("Description:", new GUIStyle(EditorStyles.label));
            _description.description = EditorGUILayout.TextArea(_description.description, 
                new GUIStyle(EditorStyles.textArea) { richText = true }, 
                GUILayout.Height(100));

            _description.url = EditorGUILayout.TextField("Documentation Url", _description.url);
            _description.type = EditorGUILayout.Popup("Icon Type", _description.type, new[] { "None", "Info", "Warning", "Error" });

            if (GUILayout.Button("Lock info"))
                _description.locked = true;
        }
        
        #endregion
    }
}