#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ABI.CCK.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CVRMirror))]
    public partial class CCK_CVRMirrorEditor : Editor
    {
        #region Editor GUI Foldouts

        private static bool _guiGeneralSettingsFoldout = true;
        private static bool _guiOptimizationSettingsFoldout;
        private static bool _guiAdvancedSettingsFoldout ;

        #endregion

        #region Private Variables

        private CVRMirror _mirror;

        #endregion

        #region Serialized Properties

        private SerializedProperty m_DisablePixelLightsProp;
        private SerializedProperty m_TextureSizeProp;
        private SerializedProperty m_ClipPlaneOffsetProp;
        private SerializedProperty m_framesNeededToUpdateProp;
        private SerializedProperty m_ReflectLayersProp;
        private SerializedProperty m_UseOcclusionCullingProp;
        private SerializedProperty m_ClearFlagsProp;
        private SerializedProperty m_CustomSkyboxProp;
        private SerializedProperty m_CustomColorProp;
        private SerializedProperty m_ignoreLegacyBehaviourProp;

        #endregion
     
        #region Unity Events

        private void OnEnable()
        {
            if (target == null) return;
            _mirror = (CVRMirror)target;
            
            m_DisablePixelLightsProp = serializedObject.FindProperty(nameof(CVRMirror.m_DisablePixelLights));
            m_TextureSizeProp = serializedObject.FindProperty(nameof(CVRMirror.m_TextureSize));
            m_ClipPlaneOffsetProp = serializedObject.FindProperty(nameof(CVRMirror.m_ClipPlaneOffset));
            m_framesNeededToUpdateProp = serializedObject.FindProperty(nameof(CVRMirror.m_framesNeededToUpdate));
            m_ReflectLayersProp = serializedObject.FindProperty(nameof(CVRMirror.m_ReflectLayers));

            m_UseOcclusionCullingProp = serializedObject.FindProperty(nameof(CVRMirror.m_UseOcclusionCulling));

            m_ClearFlagsProp = serializedObject.FindProperty(nameof(CVRMirror.m_ClearFlags));
            m_CustomSkyboxProp = serializedObject.FindProperty(nameof(CVRMirror.m_CustomSkybox));
            m_CustomColorProp = serializedObject.FindProperty(nameof(CVRMirror.m_CustomColor));

            m_ignoreLegacyBehaviourProp = serializedObject.FindProperty(nameof(CVRMirror.m_ignoreLegacyBehaviour));
        }
        
        public override void OnInspectorGUI()
        {
            if (_mirror == null)
                return;

            serializedObject.Update();

            Draw_SetupButton();
            Draw_UpgradeButton();
            
            Draw_GeneralSettings();
            // Draw_OptimizationSettings(); // TODO: Revisit optimization settings
            Draw_AdvancedSettings();

            serializedObject.ApplyModifiedProperties();
        }
        
        #endregion

        #region Drawing Methods

        private void Draw_UpgradeButton()
        {
            if (m_ignoreLegacyBehaviourProp.boolValue)
                return;
            
            EditorGUILayout.HelpBox("Mirror is in 'Legacy' state. Must be upgraded to customize the reflect layers properly.", MessageType.Warning);
            if (!GUILayout.Button("Upgrade")) 
                return;
            
            m_ignoreLegacyBehaviourProp.boolValue = true;

            m_ClipPlaneOffsetProp.floatValue = 0.001f;
            m_ReflectLayersProp.intValue &= ~(1 << 5);
            m_ReflectLayersProp.intValue &= ~(1 << 15);
            m_ReflectLayersProp.intValue |= 1 << 8;
            m_ReflectLayersProp.intValue |= 1 << 9;
            m_ReflectLayersProp.intValue |= 1 << 10;
        }

        private void Draw_SetupButton()
        {
            // Check if the necessary components are present.
            if (_mirror.TryGetComponent(out MeshRenderer _) && _mirror.TryGetComponent(out MeshFilter _))
                return;

            _mirror.enabled = false;
            
            EditorGUILayout.HelpBox("CVRMirror is not yet configured. Please use the Mirror prefabs in 'ABI.CCK/Prefabs' or click Setup Mirror.", MessageType.Warning);
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Setup Mirror", GUILayout.ExpandWidth(true)))
                    SetupMirror(false);
                if (GUILayout.Button("Setup Cutout Mirror", GUILayout.ExpandWidth(true)))
                    SetupMirror(true);
            }
        }
        
        #endregion

        #region Private Methods

        private void SetupMirror(bool isCutout)
        {
            string materialPath = isCutout
                ? "Assets/ABI.CCK/Materials/Mirror_Cutout.mat"
                : "Assets/ABI.CCK/Materials/Mirror.mat";
            
            Undo.SetCurrentGroupName("Auto Mirror Setup");
            int undoGroup = Undo.GetCurrentGroup();
            
            if (!_mirror.TryGetComponent(out MeshFilter meshFilter))
            {
                meshFilter = Undo.AddComponent<MeshFilter>(_mirror.gameObject);
                meshFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            }

            if (!_mirror.TryGetComponent(out MeshRenderer meshRenderer))
                meshRenderer = Undo.AddComponent<MeshRenderer>(_mirror.gameObject);
            
            Undo.RecordObject(meshRenderer, "Change Mirror Material");
            meshRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            
            if (_mirror.transform.localScale == Vector3.one)
            {
                Undo.RecordObject(_mirror.transform, "Change Mirror Scale");
                _mirror.transform.localScale = new Vector3(3, 2, 1);
            }

            // ignore custom reflection layers, if set
            if (_mirror.m_ReflectLayers == -1 
                || _mirror.m_ReflectLayers == 3840 
                || _mirror.m_ReflectLayers == 3841)
            {
                Undo.RecordObject(_mirror, "Change Mirror Reflect Layers");
                _mirror.m_ReflectLayers = isCutout ? 3840 : 3841; // Reflect Players &|| World
            }
            
            Undo.RecordObject(_mirror, "Configured Mirror Component");
            _mirror.m_CustomColor = isCutout ? Color.clear : Color.white;
            _mirror.m_ClearFlags = isCutout ? CVRMirror.MirrorClearFlags.Color : CVRMirror.MirrorClearFlags.Skybox;
            _mirror.enabled = true;
            _mirror.OnValidate();
            
            Undo.CollapseUndoOperations(undoGroup);
        }

        #endregion
    }
}
#endif