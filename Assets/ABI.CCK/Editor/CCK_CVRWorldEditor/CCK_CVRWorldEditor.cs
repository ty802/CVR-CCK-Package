#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace ABI.CCK.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CVRWorld))]
    public partial class CCK_CVRWorldEditor : Editor
    {
        #region Editor GUI Foldouts

        private static bool _guiGeneralSettingsFoldout = true;
        
        private static bool _guiAdvSettingsFoldout;
        private static bool _guiWorldRulesFoldout;
        private static bool _guiMovementConfigurationFoldout;
        private static bool _guiWorldGraphicsFoldout;
        private static bool _guiWorldCollisionMatrixFoldout;
        
        private static bool _guiObjectLibraryFoldout;

        #endregion

        #region Private Variables

        private CVRWorld _world;

        #endregion

        #region Serialized Properties

        private SerializedProperty m_ReferenceCameraProp;
        private SerializedProperty m_SpawnsProp;
        private SerializedProperty m_SpawnRuleProp;
        private SerializedProperty m_RespawnHeightYProp;
        private SerializedProperty m_ObjectRespawnBehaviourProp;
        //private SerializedProperty m_WarpPointsProp;

        private SerializedProperty m_AllowSpawnablesProp;
        private SerializedProperty m_AllowPortalsProp;
        private SerializedProperty m_AllowFlyingProp;
        private SerializedProperty m_EnableZoomProp;
        private SerializedProperty m_ShowNamePlatesProp;
        private SerializedProperty m_HighlightProfileProp;
        private SerializedProperty m_FovProp;
        private SerializedProperty m_EnableDepthNormalsProp;
        private SerializedProperty m_AllowExtremeFarClippingPlaneProp;
        private SerializedProperty m_BaseMovementSpeedProp;
        private SerializedProperty m_SprintMultiplierProp;
        private SerializedProperty m_StrafeMultiplierProp;
        private SerializedProperty m_CrouchMultiplierProp;
        private SerializedProperty m_ProneMultiplierProp;
        private SerializedProperty m_FlyMultiplierProp;
        private SerializedProperty m_InAirMovementMultiplierProp;
        private SerializedProperty m_GravityProp;
        private SerializedProperty m_ObjectGravityProp;
        private SerializedProperty m_JumpHeightProp;
        private SerializedProperty m_UseCustomCollisionMatrixProp;

        #endregion

        #region Unity Events

        private void OnEnable()
        {
            if (target == null) return;
            _world = (CVRWorld)target;
            _world.Reset();
            
            m_ReferenceCameraProp = serializedObject.FindProperty(nameof(CVRWorld.referenceCamera));
            m_SpawnsProp = serializedObject.FindProperty(nameof(CVRWorld.spawns));
            m_SpawnRuleProp = serializedObject.FindProperty(nameof(CVRWorld.spawnRule));
            m_RespawnHeightYProp = serializedObject.FindProperty(nameof(CVRWorld.respawnHeightY));
            m_ObjectRespawnBehaviourProp = serializedObject.FindProperty(nameof(CVRWorld.objectRespawnBehaviour));
            // m_WarpPointsProp = serializedObject.FindProperty(nameof(CVRWorld.warpPoints));
            
            m_AllowSpawnablesProp = serializedObject.FindProperty(nameof(CVRWorld.allowSpawnables));
            m_AllowPortalsProp = serializedObject.FindProperty(nameof(CVRWorld.allowPortals));
            m_AllowFlyingProp = serializedObject.FindProperty(nameof(CVRWorld.allowFlying));
            m_EnableZoomProp = serializedObject.FindProperty(nameof(CVRWorld.enableZoom));
            m_ShowNamePlatesProp = serializedObject.FindProperty(nameof(CVRWorld.showNamePlates));
#if CCK_ADDIN_HIGHLIGHT_PLUS
            m_HighlightProfileProp = serializedObject.FindProperty(nameof(CVRWorld.highlightProfile));
#endif
            m_FovProp = serializedObject.FindProperty(nameof(CVRWorld.fov));
            m_EnableDepthNormalsProp = serializedObject.FindProperty(nameof(CVRWorld.enableDepthNormals));
            m_AllowExtremeFarClippingPlaneProp = serializedObject.FindProperty(nameof(CVRWorld.allowExtremeFarClippingPlane));
            m_BaseMovementSpeedProp = serializedObject.FindProperty(nameof(CVRWorld.baseMovementSpeed));
            m_SprintMultiplierProp = serializedObject.FindProperty(nameof(CVRWorld.sprintMultiplier));
            m_StrafeMultiplierProp = serializedObject.FindProperty(nameof(CVRWorld.strafeMultiplier));
            m_CrouchMultiplierProp = serializedObject.FindProperty(nameof(CVRWorld.crouchMultiplier));
            m_ProneMultiplierProp = serializedObject.FindProperty(nameof(CVRWorld.proneMultiplier));
            m_FlyMultiplierProp = serializedObject.FindProperty(nameof(CVRWorld.flyMultiplier));
            m_InAirMovementMultiplierProp = serializedObject.FindProperty(nameof(CVRWorld.inAirMovementMultiplier));
            m_GravityProp = serializedObject.FindProperty(nameof(CVRWorld.gravity));
            m_ObjectGravityProp = serializedObject.FindProperty(nameof(CVRWorld.objectGravity));
            m_JumpHeightProp = serializedObject.FindProperty(nameof(CVRWorld.jumpHeight));
            m_UseCustomCollisionMatrixProp = serializedObject.FindProperty(nameof(CVRWorld.useCustomCollisionMatrix));
        }

        public override void OnInspectorGUI()
        {
            if (_world == null)
                return;

            serializedObject.Update();

            EditorGUIUtility.labelWidth *= 1.25f;
            
            Draw_GeneralSettings();
            Draw_AdvancedSettings();
            //Draw_ObjectLibrary();
            
            EditorGUIUtility.labelWidth *= 0.8f;

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Gizmos Drawing

        [DrawGizmo(GizmoType.Selected)]
        private static void OnGizmosSelected(CVRWorld world, GizmoType _) => DrawGizmos(world, true);

        [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
        private static void OnGizmoNotSelected(CVRWorld world, GizmoType _) => DrawGizmos(world, false);

        private static void DrawGizmos(CVRWorld world, bool isSelected)
        {
            Gizmos.color = new Color(1, 1, 1, isSelected ? 0.8f : 0.25f);
            DrawSpawns(world);
            DrawPanoFrustum(world);
            if (isSelected) DrawEditorLabel(world);
        }

        private static void DrawSpawns(CVRWorld world)
        {
            foreach (GameObject spawn in world.spawns.Length == 0 ? new [] { world.gameObject } : world.spawns)
                if (spawn) DrawArrow(spawn.transform.position, new Vector3(0, spawn.transform.eulerAngles.y, 0), 1);
        }

        private static void DrawPanoFrustum(Component world)
        {
            Vector3 pos = world.transform.position + Vector3.up;
            for (int i = 0; i < 4; i++)
            {
                Gizmos.matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0, 90 * i, 0), Vector3.one);
                Gizmos.DrawFrustum(Vector3.zero, 85f, 1f, 0.1f, 1f);
            }
        }

        private static void DrawArrow(Vector3 position, Vector3 angle, float size)
        {
            Vector3 a1 = position + new Vector3(0, 0.1f * size, 0);
            Vector3 a2 = RotatePointAroundPivot(position + new Vector3(0.1f * size, 0, 0), position, angle);
            Vector3 a3 = position + new Vector3(0, -0.1f * size, 0);
            Vector3 a4 = RotatePointAroundPivot(position + new Vector3(-0.1f * size, 0, 0), position, angle);

            Vector3 b1 = RotatePointAroundPivot(position + new Vector3(0, 0.1f * size, 0.3f * size), position, angle);
            Vector3 b2 = RotatePointAroundPivot(position + new Vector3(0.1f * size, 0, 0.3f * size), position, angle);
            Vector3 b3 = RotatePointAroundPivot(position + new Vector3(0, -0.1f * size, 0.3f * size), position, angle);
            Vector3 b4 = RotatePointAroundPivot(position + new Vector3(-0.1f * size, 0, 0.3f * size), position, angle);

            Vector3 c1 = RotatePointAroundPivot(position + new Vector3(0, 0.2f * size, 0.3f * size), position, angle);
            Vector3 c2 = RotatePointAroundPivot(position + new Vector3(0.2f * size, 0, 0.3f * size), position, angle);
            Vector3 c3 = RotatePointAroundPivot(position + new Vector3(0, -0.2f * size, 0.3f * size), position, angle);
            Vector3 c4 = RotatePointAroundPivot(position + new Vector3(-0.2f * size, 0, 0.3f * size), position, angle);

            Vector3 d = RotatePointAroundPivot(position + new Vector3(0, 0, 0.5f * size), position, angle);

            Gizmos.DrawLine(position, a1);
            Gizmos.DrawLine(position, a2);
            Gizmos.DrawLine(position, a3);
            Gizmos.DrawLine(position, a4);

            Gizmos.DrawLine(a1, b1);
            Gizmos.DrawLine(a2, b2);
            Gizmos.DrawLine(a3, b3);
            Gizmos.DrawLine(a4, b4);

            Gizmos.DrawLine(b1, c1);
            Gizmos.DrawLine(b2, c2);
            Gizmos.DrawLine(b3, c3);
            Gizmos.DrawLine(b4, c4);

            Gizmos.DrawLine(c1, d);
            Gizmos.DrawLine(c2, d);
            Gizmos.DrawLine(c3, d);
            Gizmos.DrawLine(c4, d);
        }

        private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) =>
            Quaternion.Euler(angles) * (point - pivot) + pivot;

        private static void DrawEditorLabel(Component world)
        {
            Vector3 pos = world.transform.TransformPoint(Vector3.up);
            Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);
            GUIStyle style = new GUIStyle { normal = { textColor = Color.white }, fontSize = 10 };
            Handles.BeginGUI();
            GUI.Label(new Rect(pos2D.x + 20, pos2D.y - 10, 100, 20), "Portal Image will be taken from here", style);
            Handles.EndGUI();
        }

        #endregion
    }
}
#endif