#if UNITY_EDITOR
using UnityEditor;

namespace ABI.CCK.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CVRSpawnable))]
    public partial class CCK_CVRSpawnableEditor : Editor
    {
        #region Editor GUI Foldouts

        private static bool _guiGeneralSettingsFoldout = true;
        private static bool _guiUseSyncedValuesFoldout = true;

        #endregion
        
        private CVRSpawnable _spawnable;
        
        private SerializedProperty m_SpawnHeightProp;
        private SerializedProperty m_PropPrivacyProp;
        
        private SerializedProperty m_SyncValuesProp;
        private SerializedProperty m_SubSyncsProp;
        
        #region Unity Events

        private void OnEnable()
        {
            if (target == null) return;
            _spawnable = (CVRSpawnable)target;
            _spawnable.Reset();
            
            m_SpawnHeightProp = serializedObject.FindProperty(nameof(CVRSpawnable.spawnHeight));
            m_PropPrivacyProp = serializedObject.FindProperty(nameof(CVRSpawnable.propPrivacy));
            m_SyncValuesProp = serializedObject.FindProperty(nameof(CVRSpawnable.syncValues));
            m_SubSyncsProp = serializedObject.FindProperty(nameof(CVRSpawnable.subSyncs));
            
            if (_syncedValueList == null) InitSyncValueList();
            if (_subSyncList == null) InitSubSyncList();
        }
        
        public override void OnInspectorGUI()
        {
            if (_spawnable == null)
                return;

            serializedObject.Update();
            
            Draw_GeneralSettings();
            Draw_SyncedValues();

            serializedObject.ApplyModifiedProperties();
        }
        
        #endregion
    }
}
#endif