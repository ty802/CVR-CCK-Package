#if UNITY_EDITOR
using ABI.CCK.Scripts;
using ABI.CCK.Scripts.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRSpawnableEditor
    {
        private ReorderableList _syncedValueList;
        private CVRSpawnableValue _syncedValueEntry;

        private ReorderableList _subSyncList;
        private CVRSpawnableSubSync _subSyncEntry;

        private int _syncedValueCost;
        private int _subSyncCost;
        private int _totalCost;

        private bool _outOfBoundsError;

        private void Draw_SyncedValues()
        {
            using (new ToggleFoldoutScope(ref _guiUseSyncedValuesFoldout, ref _spawnable.useAdditionalValues,
                       "Use Synced Values"))
            {
                if (!_guiUseSyncedValuesFoldout) return;
                //using (new EditorGUI.IndentLevelScope())
                using (new EditorGUI.DisabledGroupScope(!_spawnable.useAdditionalValues))
                {
                    DrawSyncedValues();
                }
            }
        }

        #region Drawing Methods

        private void DrawSyncedValues()
        {
            GUILayout.BeginVertical();

            DrawSyncedBitUsageBar();
            EditorGUILayout.Space();

            _syncedValueList.displayAdd = _totalCost <= 39f;
            _syncedValueList.DoLayoutList();

            EditorGUILayout.Space();

            _subSyncList.displayAdd = _totalCost <= 39f;
            _subSyncList.DoLayoutList();

            GUILayout.EndVertical();
        }

        private void DrawSyncedBitUsageBar()
        {
            int subSyncCost = Mathf.CeilToInt(GetSubSyncCost());
            _totalCost = _spawnable.syncValues.Count + subSyncCost;
            Rect _rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
            EditorGUI.ProgressBar(_rect, _totalCost / CVRCommon.SPAWNABLE_BIT_LIMIT, _totalCost + " of 40 Synced Values used");
            
            if (subSyncCost > 0f)
                EditorGUILayout.HelpBox(subSyncCost.ToString("F2")+" Values are used for Sub Sync Transforms", MessageType.Info);
        }
        
        #endregion
        
        #region Private Methods
        
        private float GetSubSyncCost()
        {
            var _subSyncCosts = 0f;
            
            foreach (CVRSpawnableSubSync subSync in _spawnable.subSyncs)
            {
                if (subSync.syncedValues.HasFlag(CVRSpawnableSubSync.SyncFlags.TransformX))
                    _subSyncCosts += (int) subSync.precision / 4f;
                if (subSync.syncedValues.HasFlag(CVRSpawnableSubSync.SyncFlags.TransformY))
                    _subSyncCosts += (int) subSync.precision / 4f;
                if (subSync.syncedValues.HasFlag(CVRSpawnableSubSync.SyncFlags.TransformZ))
                    _subSyncCosts += (int) subSync.precision / 4f;
                if (subSync.syncedValues.HasFlag(CVRSpawnableSubSync.SyncFlags.RotationX))
                    _subSyncCosts += (int) subSync.precision / 4f;
                if (subSync.syncedValues.HasFlag(CVRSpawnableSubSync.SyncFlags.RotationY))
                    _subSyncCosts += (int) subSync.precision / 4f;
                if (subSync.syncedValues.HasFlag(CVRSpawnableSubSync.SyncFlags.RotationZ))
                    _subSyncCosts += (int) subSync.precision / 4f;
            }

            return _subSyncCosts;
        }
        
        #endregion

        #region ReorderableList SyncedValue

        private void InitSyncValueList()
        {
            _syncedValueList = new ReorderableList(serializedObject, m_SyncValuesProp, true, true, true, true)
            {
                drawHeaderCallback = OnDrawHeaderSyncedValue,
                drawElementCallback = OnDrawElementSyncedValue,
                elementHeightCallback = OnHeightElementSyncedValue,
                onAddCallback = OnAddSyncedValue,
                onChangedCallback = OnChangedSyncedValue,
                list = _spawnable.syncValues
            };
        }

        private void OnChangedSyncedValue(ReorderableList list)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private void OnAddSyncedValue(ReorderableList list)
        {
            if (m_SyncValuesProp.arraySize >= 40) return;

            m_SyncValuesProp.arraySize++;
            serializedObject.ApplyModifiedProperties();
            Repaint();
        }

        private float OnHeightElementSyncedValue(int index)
        {
            return EditorGUIUtility.singleLineHeight * 7.5f;
        }

        private void OnDrawElementSyncedValue(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty syncedValueEntry = m_SyncValuesProp.GetArrayElementAtIndex(index);

            rect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);

            var spacing = EditorGUIUtility.singleLineHeight * 1.25f;
            var originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 120;

            EditorGUI.PropertyField(rect, syncedValueEntry.FindPropertyRelative("name"), new GUIContent("Name"));
            rect.y += spacing;
            EditorGUI.PropertyField(rect, syncedValueEntry.FindPropertyRelative("startValue"),
                new GUIContent("Start Value"));
            rect.y += spacing;
            EditorGUI.PropertyField(rect, syncedValueEntry.FindPropertyRelative("updatedBy"),
                new GUIContent("Update Type"));
            rect.y += spacing;
            EditorGUI.PropertyField(rect, syncedValueEntry.FindPropertyRelative("updateMethod"),
                new GUIContent("Update Method"));
            rect.y += spacing;
            EditorGUI.PropertyField(rect, syncedValueEntry.FindPropertyRelative("animator"),
                new GUIContent("Connected Animator"));
            rect.y += spacing;

            SerializedProperty animatorParameterNameProp =
                syncedValueEntry.FindPropertyRelative("animatorParameterName");
            SerializedProperty animatorProp = syncedValueEntry.FindPropertyRelative("animator");
            Animator animator = animatorProp.objectReferenceValue as Animator;

            animatorParameterNameProp.stringValue = EditorGUIExtensions.AdvancedDropdownInput(
                rect,
                animatorParameterNameProp.stringValue,
                CVRCommon.GetParametersFromAnimatorAsString(animator),
                "Animator Parameter",
                "No Parameters"
            );

            EditorGUIUtility.labelWidth = originalLabelWidth;
        }

        private void OnDrawHeaderSyncedValue(Rect rect)
        {
            Rect labelRect = new Rect(rect.x, rect.y, rect.width - 35, EditorGUIUtility.singleLineHeight);
            GUI.Label(labelRect, "Synced Values");
            EditorGUIExtensions.UtilityMenu(rect, _syncedValueList);
        }

        #endregion
        
        #region ReorderableList SubSync
        
        private void InitSubSyncList()
        {
            _subSyncList = new ReorderableList(serializedObject, m_SubSyncsProp, false, true, true, true)
            {
                drawHeaderCallback = OnDrawHeaderSubSync,
                drawElementCallback = OnDrawElementSubSync,
                elementHeightCallback = OnHeightElementSubSync,
                onAddCallback = OnAddSubSync,
                onChangedCallback = OnChangedSubSync,
                list = _spawnable.subSyncs
            };
        }

        private void OnDrawHeaderSubSync(Rect rect)
        {
            Rect labelRect = new Rect(rect.x, rect.y, rect.width - 35, EditorGUIUtility.singleLineHeight);
            GUI.Label(labelRect, "Sub Sync Transforms");
            EditorGUIExtensions.UtilityMenu(rect, _subSyncList);
        }

        private void OnDrawElementSubSync(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty subSyncEntry = m_SubSyncsProp.GetArrayElementAtIndex(index);
            SerializedProperty transformProperty = subSyncEntry.FindPropertyRelative("transform");
            SerializedProperty syncedValuesProperty = subSyncEntry.FindPropertyRelative("syncedValues");
            SerializedProperty precisionProperty = subSyncEntry.FindPropertyRelative("precision");
            SerializedProperty syncBoundaryProperty = subSyncEntry.FindPropertyRelative("syncBoundary");

            rect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
            
            float spacing = EditorGUIUtility.singleLineHeight + 2f;
            EditorGUI.PropertyField(rect, transformProperty, new GUIContent("Transform"));
            rect.y += spacing;
            EditorGUI.PropertyField(rect, syncedValuesProperty, new GUIContent("Synced Properties"));
            rect.y += spacing;
            EditorGUI.PropertyField(rect, precisionProperty, new GUIContent("Sync Precision"));

            if (precisionProperty.enumValueFlag != (int)CVRSpawnableSubSync.SyncPrecision.Full)
            {
                rect.y += spacing;
                EditorGUI.PropertyField(rect, syncBoundaryProperty, new GUIContent("Sync Boundary"));
            }
        }

        private float OnHeightElementSubSync(int index)
        {
            SerializedProperty subSyncEntry = m_SubSyncsProp.GetArrayElementAtIndex(index);
            SerializedProperty precisionProperty = subSyncEntry.FindPropertyRelative("precision");

            if (precisionProperty.enumValueFlag == (int)CVRSpawnableSubSync.SyncPrecision.Full)
                return EditorGUIUtility.singleLineHeight * 3.25f;
            
            return EditorGUIUtility.singleLineHeight * 4.5f;
        }

        private void OnAddSubSync(ReorderableList list)
        {
            if (m_SubSyncsProp.arraySize >= 40) return;
            m_SubSyncsProp.InsertArrayElementAtIndex(m_SubSyncsProp.arraySize);
            serializedObject.ApplyModifiedProperties();
        }

        private void OnChangedSubSync(ReorderableList list)
        {
        }

        #endregion
    }
}
#endif