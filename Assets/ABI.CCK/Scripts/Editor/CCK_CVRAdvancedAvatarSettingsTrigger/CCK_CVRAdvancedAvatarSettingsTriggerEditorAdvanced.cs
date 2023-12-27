#if UNITY_EDITOR
using System.Linq;
using ABI.CCK.Scripts;
using ABI.CCK.Scripts.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRAdvancedAvatarSettingsTriggerEditor
    {
        // Allow
        private ReorderableList _allowedTypesList;
        private ReorderableList _allowedPointersList;

        // Tasks
        private ReorderableList _onEnterList;
        private ReorderableList _onExitList;
        private ReorderableList _onStayList;
        
        private void Draw_AllowedFilterSettings()
        {
            using (new FoldoutScope(ref _guiAllowedFilterFoldout, "Allowed Filter"))
            {
                if (!_guiAllowedFilterFoldout) return;
                DrawAllowedFilterSettings();
            }
        }

        private void Draw_AdvancedTasks()
        {
            using (new FoldoutScope(ref _guiTriggerSettingsFoldout, "Trigger Tasks"))
            {
                if (!_guiTriggerSettingsFoldout) return;
                DrawAdvancedTasks();
            }
        }
        
        #region Drawing Methods

        private void DrawAllowedFilterSettings()
        {
            if (_allowedPointersList == null)
            {
                _allowedPointersList = new ReorderableList(serializedObject,
                    serializedObject.FindProperty("allowedPointer"), false, true, true, false)
                {
                    drawHeaderCallback = OnDrawHeaderAllowedPointers,
                    drawElementCallback = OnDrawElementAllowedPointers,
                    elementHeightCallback = OnElementHeight,
                    onChangedCallback = OnChanged
                };
            }

            if (_allowedTypesList == null)
            {
                _allowedTypesList = new ReorderableList(serializedObject,
                    serializedObject.FindProperty("allowedTypes"), false, true, true, false)
                {
                    drawHeaderCallback = OnDrawHeaderAllowedTypes,
                    drawElementCallback = OnDrawElementAllowedTypes,
                    elementHeightCallback = OnElementHeight,
                    onChangedCallback = OnChanged
                };
            }

            int newSelectedIndex;
            int currentMode = (_allowedPointersList.count > 0) ? 1 : 0;
            
            using (new EditorGUI.IndentLevelScope())
                newSelectedIndex = EditorGUILayout.Popup("Allow Filter Mode", currentMode, new[] { "Type", "Reference" });
            
            // This is so jank... Why are they one or the other???
            if (newSelectedIndex != currentMode) {
                if (newSelectedIndex == 0) {
                    _allowedPointersList.serializedProperty.arraySize = 0;
                } else {
                    _allowedPointersList.serializedProperty.arraySize = 1;
                    _allowedPointersList.serializedProperty.GetArrayElementAtIndex(0).objectReferenceValue = null;
                }
            }

            Separator();
            
            if (newSelectedIndex == 0) {
                EditorGUILayout.HelpBox(CCKLocalizationProvider.GetLocalizedText("ABI_UI_ADVAVTR_TRIGGER_ALLOWED_TYPES_HELPBOX"), MessageType.Info);
                _allowedTypesList.DoLayoutList();
            } else {
                EditorGUILayout.HelpBox(CCKLocalizationProvider.GetLocalizedText("ABI_UI_ADVAVTR_TRIGGER_ALLOWED_POINTERS_HELPBOX"), MessageType.Info);
                _allowedPointersList.DoLayoutList();
            }
        }

        private void DrawAdvancedTasks()
        {
            if (_onEnterList == null)
            {
                _onEnterList = new ReorderableList(_trigger.enterTasks,
                    typeof(CVRAdvancedAvatarSettingsTriggerTask),
                    false, true, true, true)
                {
                    drawHeaderCallback = OnDrawHeaderEnter,
                    drawElementCallback = OnDrawElementEnter,
                    elementHeightCallback = OnHeightElementEnter,
                    onChangedCallback = OnChangedEnter
                };
            }

            if (_onExitList == null)
            {
                _onExitList = new ReorderableList(_trigger.exitTasks, typeof(CVRAdvancedAvatarSettingsTriggerTask),
                    false, true, true, true)
                {
                    drawHeaderCallback = OnDrawHeaderExit,
                    drawElementCallback = OnDrawElementExit,
                    elementHeightCallback = OnHeightElementExit,
                    onChangedCallback = OnChangedExit
                };
            }

            if (_onStayList == null)
            {
                _onStayList = new ReorderableList(_trigger.stayTasks,
                    typeof(CVRAdvancedAvatarSettingsTriggerTaskStay),
                    false, true, true, true)
                {
                    drawHeaderCallback = OnDrawHeaderStay,
                    drawElementCallback = OnDrawElementStay,
                    elementHeightCallback = OnHeightElementStay,
                    onChangedCallback = OnChangedStay
                };
            }

            _onEnterList.DoLayoutList();
            EditorGUILayout.Space();

            _onExitList.DoLayoutList();
            EditorGUILayout.Space();

            _onStayList.DoLayoutList();
            EditorGUILayout.Space();

            if (_trigger.stayTasks.Count > 0)
            {
                _trigger.sampleDirection = (CVRAdvancedAvatarSettingsTrigger.SampleDirection)
                    EditorGUILayout.EnumPopup("Sample Direction", _trigger.sampleDirection);
            }
        }

        #endregion

        #region ReorderableList AllowLists

        private void OnDrawHeaderAllowedTypes(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "Allowed Types");
        }

        private void OnDrawHeaderAllowedPointers(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "Allowed Pointers");
        }

        private float OnElementHeight(int index)
        {
            return 4 + EditorGUIUtility.singleLineHeight;
        }

        private void OnChanged(ReorderableList list)
        {
            EditorUtility.SetDirty(_trigger);
        }

        private void OnDrawElementAllowedTypes(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= _allowedTypesList.serializedProperty.arraySize) return;
            SerializedProperty element = _allowedTypesList.serializedProperty.GetArrayElementAtIndex(index);
            
            GUIContent buttonContent = CVRCommon.DefaultPointerTypes.Contains(element.stringValue)
                ? EditorGUIExtensions.GetCachedIconContent("Favorite")
                : EditorGUIExtensions.GetCachedIconContent("d_editicon.sml");

            var newValue = EditorGUIExtensions.AdvancedDropdownInput(
                new Rect(rect.x, rect.y + 2, rect.width - 20, EditorGUIUtility.singleLineHeight), element.stringValue,
                CVRCommon.DefaultPointerTypes, "Default Pointer Types", buttonContent);
            
            if (newValue != element.stringValue)
            {
                element.stringValue = newValue;
                _allowedTypesList.serializedProperty.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            if (GUI.Button(new Rect(rect.x + rect.width - 18, rect.y + 2, 18, EditorGUIUtility.singleLineHeight), "X"))
            {
                _allowedTypesList.serializedProperty.DeleteArrayElementAtIndex(index);
                _allowedTypesList.serializedProperty.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_trigger);
            }
        }

        private void OnDrawElementAllowedPointers(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= _allowedPointersList.serializedProperty.arraySize) return;
            SerializedProperty element = _allowedPointersList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.ObjectField(new Rect(rect.x, rect.y + 2, rect.width - 20, EditorGUIUtility.singleLineHeight),
                element, typeof(CVRPointer), GUIContent.none);

            if (GUI.Button(new Rect(rect.x + rect.width - 18, rect.y + 2, 18, EditorGUIUtility.singleLineHeight), "X"))
            {
                _allowedPointersList.serializedProperty.DeleteArrayElementAtIndex(index);
                _allowedPointersList.serializedProperty.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_trigger);
            }
        }

        #endregion

        #region ReorderableList OnEnterTasks

        private void OnDrawHeaderEnter(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "On Enter Trigger");
            EditorGUIExtensions.UtilityMenu(_rect, _onEnterList);
        }

        private void OnDrawElementEnter(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index >= _trigger.enterTasks.Count) return;
            CVRAdvancedAvatarSettingsTriggerTask enterTask = _trigger.enterTasks[index];

            Rect _rect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
            float spacing = EditorGUIUtility.singleLineHeight * 1.25f;
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;
            
            enterTask.settingName = EditorGUIExtensions.AdvancedDropdownInput(_rect, enterTask.settingName, _avatarParameterNames,
                "Setting Name", "No Parameters");
            _rect.y += spacing;
            
            enterTask.settingValue = EditorGUI.FloatField(_rect, "Setting Value", enterTask.settingValue);
            _rect.y += spacing;
            
            enterTask.delay = EditorGUI.FloatField(_rect, "Delay", enterTask.delay);
            _rect.y += spacing;
            
            enterTask.holdTime = EditorGUI.FloatField(_rect,"Hold Time", enterTask.holdTime);
            _rect.y += spacing;
            
            enterTask.updateMethod =
                (CVRAdvancedAvatarSettingsTriggerTask.UpdateMethod)EditorGUI.EnumPopup(_rect, "Update Method", enterTask.updateMethod);

            EditorGUIUtility.labelWidth = originalLabelWidth;
        }

        private float OnHeightElementEnter(int index)
        {
            return EditorGUIUtility.singleLineHeight * 6.25f;
        }

        private void OnChangedEnter(ReorderableList list)
        {
            EditorUtility.SetDirty(_trigger);
        }

        #endregion

        #region ReorderableList OnExitTasks

        private void OnDrawHeaderExit(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "On Exit Trigger");
            EditorGUIExtensions.UtilityMenu(_rect, _onExitList);
        }

        private void OnDrawElementExit(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index >= _trigger.exitTasks.Count) return;
            CVRAdvancedAvatarSettingsTriggerTask exitTask = _trigger.exitTasks[index];

            Rect _rect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
            float spacing = EditorGUIUtility.singleLineHeight * 1.25f;
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;

            exitTask.settingName = EditorGUIExtensions.AdvancedDropdownInput(_rect, exitTask.settingName, _avatarParameterNames,
                "Setting Name", "No Parameters");
            _rect.y += spacing;
            
            exitTask.settingValue = EditorGUI.FloatField(_rect, "Setting Value", exitTask.settingValue);
            _rect.y += spacing;
            
            exitTask.delay = EditorGUI.FloatField(_rect, "Delay", exitTask.delay);
            _rect.y += spacing;

            exitTask.updateMethod =
                (CVRAdvancedAvatarSettingsTriggerTask.UpdateMethod)EditorGUI.EnumPopup(_rect, "Update Method", exitTask.updateMethod);

            EditorGUIUtility.labelWidth = originalLabelWidth;
        }

        private float OnHeightElementExit(int index)
        {
            return EditorGUIUtility.singleLineHeight * 5f;
        }

        private void OnChangedExit(ReorderableList list)
        {
            EditorUtility.SetDirty(_trigger);
        }

        #endregion

        #region ReorderableList OnStayTasks

        private void OnDrawHeaderStay(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "On Stay Trigger");
            EditorGUIExtensions.UtilityMenu(_rect, _onStayList);
        }

        private void OnDrawElementStay(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index >= _trigger.stayTasks.Count) return;
            CVRAdvancedAvatarSettingsTriggerTaskStay stayTask = _trigger.stayTasks[index];

            Rect _rect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
            float spacing = EditorGUIUtility.singleLineHeight * 1.25f;
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;

            stayTask.settingName = EditorGUIExtensions.AdvancedDropdownInput(_rect, stayTask.settingName, _avatarParameterNames,
                "Setting Name", "No Parameters");
            _rect.y += spacing;

            stayTask.updateMethod =
                (CVRAdvancedAvatarSettingsTriggerTaskStay.UpdateMethod)EditorGUI.EnumPopup(_rect,
                    "Update Method", stayTask.updateMethod);
            _rect.y += spacing;

            if (stayTask.updateMethod == CVRAdvancedAvatarSettingsTriggerTaskStay.UpdateMethod.SetFromPosition ||
                stayTask.updateMethod == CVRAdvancedAvatarSettingsTriggerTaskStay.UpdateMethod.SetFromDistance)
            {
                stayTask.minValue = EditorGUI.FloatField(_rect, "Min Value", stayTask.minValue);
                _rect.y += spacing;
                
                stayTask.maxValue = EditorGUI.FloatField(_rect, "Max Value", stayTask.maxValue);
                _rect.y += spacing;
            }
            else
            {
                stayTask.minValue = EditorGUI.FloatField(_rect, "Change per sec", stayTask.minValue);
                _rect.y += spacing;
            }

            EditorGUIUtility.labelWidth = originalLabelWidth;
        }

        private float OnHeightElementStay(int index)
        {
            if (index >= _trigger.stayTasks.Count) return EditorGUIUtility.singleLineHeight * 3.75f;
            CVRAdvancedAvatarSettingsTriggerTaskStay stayTask = _trigger.stayTasks[index];

            if (stayTask.updateMethod == CVRAdvancedAvatarSettingsTriggerTaskStay.UpdateMethod.SetFromPosition ||
                stayTask.updateMethod == CVRAdvancedAvatarSettingsTriggerTaskStay.UpdateMethod.SetFromDistance)
                return EditorGUIUtility.singleLineHeight * 5f;

            return EditorGUIUtility.singleLineHeight * 3.75f;
        }

        private void OnChangedStay(ReorderableList list)
        {
            EditorUtility.SetDirty(_trigger);
        }

        #endregion
    }
}
#endif