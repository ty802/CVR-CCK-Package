using ABI.CCK.Components;
using UnityEditor;
using UnityEngine;

namespace ABI.CCK.Scripts.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CVRAssetInfo))]
    public class CCK_CVRAssetInfoEditor : UnityEditor.Editor
    {
        private SerializedProperty _objectIdProperty;

        private string _newGUID;

        #region Unity Events

        private void OnEnable()
        {
            if (target == null)
                return;

            _objectIdProperty = serializedObject.FindProperty("objectId");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawHeaderInfo();

            if (!string.IsNullOrEmpty(_objectIdProperty.stringValue))
                DrawDetachGUID();
            else
                DrawAttachGUID();
            
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region GUI Drawing

        private void DrawHeaderInfo()
        {
            EditorGUILayout.HelpBox(CCKLocalizationProvider.GetLocalizedText("ABI_UI_ASSET_INFO_HEADER_INFORMATION"), MessageType.Info);
        }

        private void DrawDetachGUID()
        {
            EditorGUILayout.HelpBox(CCKLocalizationProvider.GetLocalizedText("ABI_UI_ASSET_INFO_GUID_LABEL") + _objectIdProperty.stringValue, MessageType.Info);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(CCKLocalizationProvider.GetLocalizedText("ABI_UI_ASSET_INFO_DETACH_BUTTON")))
            {
                bool shouldDetach = EditorUtility.DisplayDialog(
                    CCKLocalizationProvider.GetLocalizedText("ABI_UI_ASSET_INFO_DETACH_BUTTON_DIALOG_TITLE"),
                    CCKLocalizationProvider.GetLocalizedText("ABI_UI_ASSET_INFO_DETACH_BUTTON_DIALOG_BODY"),
                    CCKLocalizationProvider.GetLocalizedText("ABI_UI_ASSET_INFO_DETACH_BUTTON_DIALOG_ACCEPT"),
                    CCKLocalizationProvider.GetLocalizedText("ABI_UI_ASSET_INFO_DETACH_BUTTON_DIALOG_DENY"));
                
                if (shouldDetach)
                {
                    _newGUID = _objectIdProperty.stringValue;
                    _objectIdProperty.stringValue = string.Empty;
                }
            }

            if (GUILayout.Button(CCKLocalizationProvider.GetLocalizedText("ABI_UI_ASSET_INFO_COPY_BUTTON")))
            {
                if (!string.IsNullOrEmpty(_objectIdProperty.stringValue))
                    GUIUtility.systemCopyBuffer = _objectIdProperty.stringValue;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawAttachGUID()
        {
            _newGUID = EditorGUILayout.TextField(CCKLocalizationProvider.GetLocalizedText("ABI_UI_ASSET_INFO_ATTACH_LABEL"), _newGUID);
            EditorGUILayout.HelpBox(CCKLocalizationProvider.GetLocalizedText("ABI_UI_ASSET_INFO_ATTACH_INFO"), MessageType.Warning);

            if (!GUILayout.Button(CCKLocalizationProvider.GetLocalizedText("ABI_UI_ASSET_INFO_ATTACH_BUTTON"))) 
                return;
            
            if (System.Guid.TryParse(_newGUID, out _))
                _objectIdProperty.stringValue = _newGUID;
        }

        #endregion
    }
}