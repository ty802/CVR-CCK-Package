#if UNITY_EDITOR
using ABI.CCK.Scripts.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRAvatarEditor
    {
        private ReorderableList _advTaggingList;
        
        private void InitializeTaggingList()
        {
            if (_advTaggingList != null)
                return;
            
            _advTaggingList = new ReorderableList(_avatar.advancedTaggingList, typeof(CVRAvatarAdvancedTaggingEntry),
                false, true, true, true)
            {
                drawHeaderCallback = OnDrawHeaderTagging,
                drawElementCallback = OnDrawElementTagging,
                elementHeightCallback = OnHeightElementTagging,
                onChangedCallback = OnChangedTagging
            };
        }
        
        private void Draw_AdvancedTagging()
        {
            using (new ToggleFoldoutScope(ref _guiAdvancedTaggingFoldout, ref _avatar.enableAdvancedTagging, "Advanced Tagging"))
            {
                if (!_guiAdvancedTaggingFoldout) return;

                InitializeTaggingList();
            
                EditorGUILayout.HelpBox("If you are using the Advanced Tagging System, you do not need to Tag your Avatar appropriately if you mark all affected GameObjects here.", MessageType.Info);
                using (new EditorGUI.DisabledGroupScope(!_avatar.enableAdvancedTagging))
                    _advTaggingList.DoLayoutList();
            }
        }

        private float OnHeightElementTagging(int index)
        {
            if (index > _avatar.advancedTaggingList.Count) return EditorGUIUtility.singleLineHeight * 2.5f;
            
            return EditorGUIUtility.singleLineHeight * 
                   ((_avatar.advancedTaggingList[index].fallbackGameObject != null && 
                     _avatar.advancedTaggingList[index].fallbackGameObject.activeSelf) ? 5f : 3.75f);
        }

        private void OnDrawHeaderTagging(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "Tagged GameObjects");
            EditorGUIExtensions.UtilityMenu(_rect, _advTaggingList);
        }

        private void OnDrawElementTagging(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index > _avatar.advancedTaggingList.Count) return;
            Rect _rect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
            
            float spacing = EditorGUIUtility.singleLineHeight * 1.25f;
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;
            
            CVRAvatarAdvancedTaggingEntry tagEntry = _avatar.advancedTaggingList[index];

            tagEntry.tags = (CVRAvatarAdvancedTaggingEntry.Tags) EditorGUI.EnumFlagsField(_rect, "Tags", tagEntry.tags);
            _rect.y += spacing;
            
            tagEntry.gameObject = (GameObject) EditorGUI.ObjectField(_rect, "GameObject", tagEntry.gameObject, typeof(GameObject), true);
            _rect.y += spacing;
            
            tagEntry.fallbackGameObject = (GameObject) EditorGUI.ObjectField(_rect, "Fallback GO", tagEntry.fallbackGameObject, typeof(GameObject), true);
            _rect.y += spacing;

            if (tagEntry.fallbackGameObject != null && tagEntry.fallbackGameObject.activeSelf)
            {
                EditorGUI.HelpBox(_rect, "The Fallback GameObject needs to be disabled by default!", MessageType.Error);
                _rect.y += spacing;
            }
            
            EditorGUIUtility.labelWidth = originalLabelWidth;
        }

        private void OnChangedTagging(ReorderableList list)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
#endif