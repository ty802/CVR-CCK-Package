using ABI.CCK.Components;
using ABI.CCK.Scripts.Editor;
using ABI.CCK.Scripts.Editor.Tools;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ABI.CCK.Scripts.Editor
{
    //[CustomEditor(typeof(CVRSpawnable), true)]
    public class CCK_CVRSpawnableEditor : UnityEditor.Editor
    {
        private CVRSpawnable _spawnable;
        
        private ReorderableList _syncedValueList;
        private CVRSpawnableValue _syncedValueEntry;
        
        private ReorderableList _subSyncList;
        private CVRSpawnableSubSync _subSyncEntry;

        private float _subSyncCosts;
        private bool _outOfBoundsError;

        #region Unity Events

        public void OnEnable()
        {
            // Occurs on recompile
            if (target == null)
                return; 
            
            _spawnable = (CVRSpawnable)target;
            _spawnable.Reset();
            
            if (_syncedValueList == null) 
                InitializeList();
            
            if (_subSyncList == null) 
                InitializeSubSyncList();
        }
        
        public override void OnInspectorGUI()
        {
            if (_spawnable == null)
                return;

            EditorGUI.BeginChangeCheck();
            
            _spawnable.spawnHeight = EditorGUILayout.FloatField("Spawn Height", _spawnable.spawnHeight);

            _spawnable.propPrivacy = (CVRSpawnable.PropPrivacy) EditorGUILayout.EnumPopup("Prop Usage", _spawnable.propPrivacy);
            
            GUILayout.BeginVertical("HelpBox");
            
            GUILayout.BeginHorizontal ();
            _spawnable.useAdditionalValues = EditorGUILayout.Toggle (_spawnable.useAdditionalValues, GUILayout.Width(16));
            EditorGUILayout.LabelField ("Enable Sync Values", GUILayout.Width(250));
            GUILayout.EndHorizontal ();

            if (_spawnable.useAdditionalValues)
            {
                UpdateSubSyncCosts();
                
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical("GroupBox");

                var SyncCost = _spawnable.syncValues.Count + _subSyncCosts;
                
                Rect _rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                EditorGUI.ProgressBar(_rect, SyncCost / 40f, Mathf.CeilToInt(SyncCost) + " of 40 Synced Values used");
                EditorGUILayout.Space();

                if (_subSyncCosts > 0f)
                {
                    EditorGUILayout.HelpBox(_subSyncCosts.ToString("F2")+" Values are used for Sub Sync Transforms", MessageType.Info);
                    EditorGUILayout.Space();
                }
                
                _syncedValueList.displayAdd = SyncCost <= 39f;
                _syncedValueList.DoLayoutList();
                
                EditorGUILayout.Space();
                
                _subSyncList.displayAdd = SyncCost <= 39f;
                _subSyncList.DoLayoutList();
                
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            
            GUILayout.EndVertical();

            if (_outOfBoundsError)
            {
                EditorGUILayout.HelpBox("A Sub Sync Transform is out of bounds by default. This object will snap to its bounds, when it is being synced.", MessageType.Error);
                _outOfBoundsError = false;
            }

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(_spawnable);
        }

        #endregion

        #region ReorderableList SyncedValue

        private void InitializeList()
        {
            _syncedValueList = new ReorderableList(_spawnable.syncValues, typeof(CVRAdvancedSettingsEntry), true, true, true, true)
                {
                    drawHeaderCallback = OnDrawHeaderSyncedValue,
                    drawElementCallback = OnDrawElementSyncedValue,
                    elementHeightCallback = OnHeightElementSyncedValue,
                    onAddCallback = OnAddSyncedValue,
                    onChangedCallback = OnChangedSyncedValue
                };
        }
        
        private void OnChangedSyncedValue(ReorderableList list)
        {
            //EditorUtility.SetDirty(target);
        }

        private void OnAddSyncedValue(ReorderableList list)
        {
            if (_spawnable.syncValues.Count >= 40) return;
            _spawnable.syncValues.Add(new CVRSpawnableValue());
            Repaint();
        }

        private float OnHeightElementSyncedValue(int index)
        {
            return EditorGUIUtility.singleLineHeight * 7.5f;
        }

        private void OnDrawElementSyncedValue(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index > _spawnable.syncValues.Count) return;
            _syncedValueEntry = _spawnable.syncValues[index];
            
            rect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
            
            float spacing = EditorGUIUtility.singleLineHeight * 1.25f;
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 120;

            _syncedValueEntry.name = EditorGUI.TextField(rect, "Name", _syncedValueEntry.name);
            rect.y += spacing;
            _syncedValueEntry.startValue = EditorGUI.FloatField(rect, "Start Value", _syncedValueEntry.startValue);
            rect.y += spacing;
            _syncedValueEntry.updatedBy = (CVRSpawnableValue.UpdatedBy) EditorGUI.EnumPopup(rect, "Update Type", _syncedValueEntry.updatedBy);
            rect.y += spacing;
            _syncedValueEntry.updateMethod = (CVRSpawnableValue.UpdateMethod) EditorGUI.EnumPopup(rect, "Update Method", _syncedValueEntry.updateMethod);
            rect.y += spacing;
            _syncedValueEntry.animator = (Animator)EditorGUI.ObjectField(rect, "Connected Animator", _syncedValueEntry.animator, typeof(Animator), true);
            rect.y += spacing;
            
            _syncedValueEntry.animatorParameterName = EditorGUIExtensions.AdvancedDropdownInput(rect, _syncedValueEntry.animatorParameterName,
                CVRCommon.GetParametersFromAnimatorAsString(_syncedValueEntry.animator), "Animator Parameter","No Parameters");

            EditorGUIUtility.labelWidth = originalLabelWidth;
        }
        
        private void OnDrawHeaderSyncedValue(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "Values");
            EditorGUIExtensions.UtilityMenu(_rect, _syncedValueList);
        }

        #endregion

        #region ReorderableList SubSync

        private void InitializeSubSyncList()
        {
            _subSyncList = new ReorderableList(_spawnable.subSyncs, typeof(CVRSpawnableSubSync), false, true, true, true)
                {
                    drawHeaderCallback = OnDrawHeaderSubSync,
                    drawElementCallback = OnDrawElementSubSync,
                    elementHeightCallback = OnHeightElementSubSync,
                    onAddCallback = OnAddSubSync,
                    onChangedCallback = OnChangedSubSync
                };
        }

        private void OnDrawHeaderSubSync(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "Sub Sync Transforms");
            EditorGUIExtensions.UtilityMenu(_rect, _subSyncList);
        }

        private void OnDrawElementSubSync(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index > _spawnable.subSyncs.Count) return;
            _subSyncEntry = _spawnable.subSyncs[index];
    
            rect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
            
            float spacing = EditorGUIUtility.singleLineHeight * 1.25f;
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 120;

            _subSyncEntry.transform = (Transform)EditorGUI.ObjectField(rect, "Transform", _subSyncEntry.transform, typeof(Transform), true);
            rect.y += spacing;
            _subSyncEntry.syncedValues = (CVRSpawnableSubSync.SyncFlags) EditorGUI.EnumFlagsField(rect, "Synced Properties", _subSyncEntry.syncedValues);
            rect.y += spacing;
            _subSyncEntry.precision = (CVRSpawnableSubSync.SyncPrecision) EditorGUI.EnumPopup(rect, "Sync Precision", _subSyncEntry.precision);
            rect.y += spacing;

            if (_subSyncEntry.precision != CVRSpawnableSubSync.SyncPrecision.Full)
            {
                _subSyncEntry.syncBoundary = EditorGUI.FloatField(rect, "Sync Boundary", _subSyncEntry.syncBoundary);
                rect.y += spacing;
                if (_subSyncEntry.transform != null)
                {
                    Vector3 localPosition = _subSyncEntry.transform.localPosition;
                    _outOfBoundsError = Mathf.Abs(localPosition.x) > _subSyncEntry.syncBoundary ||
                                       Mathf.Abs(localPosition.y) > _subSyncEntry.syncBoundary ||
                                       Mathf.Abs(localPosition.z) > _subSyncEntry.syncBoundary;
                }
            }

            EditorGUIUtility.labelWidth = originalLabelWidth;
        }

        private float OnHeightElementSubSync(int index)
        {
            if (_spawnable.subSyncs[index].precision == CVRSpawnableSubSync.SyncPrecision.Full) 
                return EditorGUIUtility.singleLineHeight * 3.75f;
            return EditorGUIUtility.singleLineHeight * 5f;
        }

        private void OnAddSubSync(ReorderableList list)
        {
            if (_spawnable.subSyncs.Count >= 40) return;
            _spawnable.subSyncs.Add(new CVRSpawnableSubSync());
            Repaint();
        }

        private void OnChangedSubSync(ReorderableList list)
        {
            //EditorUtility.SetDirty(target);
        }

        #endregion
        
        #region Private Methods

        private void UpdateSubSyncCosts()
        {
            _subSyncCosts = 0f;
            
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
        }

        #endregion
    }
}