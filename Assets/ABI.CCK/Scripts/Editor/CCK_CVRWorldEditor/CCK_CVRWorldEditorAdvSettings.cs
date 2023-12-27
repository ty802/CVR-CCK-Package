#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

#if CCK_ADDIN_HIGHLIGHT_PLUS
using HighlightPlus;
#endif

namespace ABI.CCK.Components
{
    public partial class CCK_CVRWorldEditor
    {
        private void Draw_AdvancedSettings()
        {
            using (new ToggleFoldoutScope(ref _guiAdvSettingsFoldout, ref _world.useAdvancedSettings, "Advanced Settings"))
            {
                if (!_guiAdvSettingsFoldout) return;
                using (new EditorGUI.IndentLevelScope())
                using (new EditorGUI.DisabledGroupScope(!_world.useAdvancedSettings))
                    DrawAdvancedSettings();
            }
        }

        private void DrawAdvancedSettings()
        {
            GUILayout.BeginVertical();

            DrawWorldRules();
            
            EditorGUILayout.Space();
            DrawWorldGraphicsSettings();
            
            EditorGUILayout.Space();
            DrawWorldMovementConfiguration();
            
            EditorGUILayout.Space();
            DrawWorldCollisionMatrixSettings();

            GUILayout.EndVertical();
        }

        #region Drawing Methods

        private void DrawWorldRules()
        {
            if (!InnerFoldout(ref _guiWorldRulesFoldout, "World Rules")) 
                return;

            EditorGUILayout.PropertyField(m_AllowSpawnablesProp, new GUIContent("Allow Spawnables"));
            EditorGUILayout.PropertyField(m_AllowPortalsProp, new GUIContent("Allow Portals"));
            EditorGUILayout.PropertyField(m_AllowFlyingProp, new GUIContent("Allow Flying"));
            EditorGUILayout.PropertyField(m_EnableZoomProp, new GUIContent("Allow Zoom"));
            EditorGUILayout.PropertyField(m_ShowNamePlatesProp, new GUIContent("Show Nameplates"));
        }

        private void DrawWorldGraphicsSettings()
        {
            if (!InnerFoldout(ref _guiWorldGraphicsFoldout, "World Graphics")) 
                return;

#if CCK_ADDIN_HIGHLIGHT_PLUS
            EditorGUILayout.PropertyField(m_HighlightProfileProp, new GUIContent("Highlighting Profile"));
#endif

            EditorGUILayout.PropertyField(m_FovProp, new GUIContent("Fov"));
            EditorGUILayout.PropertyField(m_EnableDepthNormalsProp, new GUIContent("Enable Depth Normals"));
            EditorGUILayout.PropertyField(m_AllowExtremeFarClippingPlaneProp, new GUIContent("Allow Extreme Far Clip Plane"));
        }

        private void DrawWorldMovementConfiguration()
        {
            if (!InnerFoldout(ref _guiMovementConfigurationFoldout, "Movement Settings")) 
                return;

            EditorGUILayout.HelpBox("Changing these values can lead to an undesirable experience.", MessageType.Warning);

            EditorGUILayout.PropertyField(m_BaseMovementSpeedProp, new GUIContent("Base Movement Speed"));
            EditorGUILayout.PropertyField(m_SprintMultiplierProp, new GUIContent("Sprint Multiplier"));
            EditorGUILayout.PropertyField(m_StrafeMultiplierProp, new GUIContent("Strafe Multiplier"));
            EditorGUILayout.PropertyField(m_CrouchMultiplierProp, new GUIContent("Crouch Multiplier"));
            EditorGUILayout.PropertyField(m_ProneMultiplierProp, new GUIContent("Prone Multiplier"));
            EditorGUILayout.PropertyField(m_FlyMultiplierProp, new GUIContent("Fly Multiplier"));
            EditorGUILayout.PropertyField(m_InAirMovementMultiplierProp, new GUIContent("In-Air Movement Multiplier"));
            EditorGUILayout.PropertyField(m_GravityProp, new GUIContent("Player Gravity"));
            EditorGUILayout.PropertyField(m_JumpHeightProp, new GUIContent("Jump Height"));
            EditorGUILayout.PropertyField(m_ObjectGravityProp, new GUIContent("Object Gravity"));
        }
        
        private void DrawWorldCollisionMatrixSettings()
        {
            if (!InnerFoldout(ref _guiWorldCollisionMatrixFoldout, "World Collision Matrix")) 
                return;
            
            EditorGUILayout.PropertyField(m_UseCustomCollisionMatrixProp, new GUIContent("Use Custom Collision Matrix"));
                
            Separator();

            using (new EditorGUI.DisabledGroupScope(!_world.useCustomCollisionMatrix))
            {
                GUILayout.BeginVertical("HelpBox");
                int numExceptions = 0;
                var exceptionList = new List<string>();

                for (int i = 0; i < _world.collisionMatrix.Count; i++)
                {
                    for (int j = 0; j < _world.collisionMatrix[i].Count; j++)
                    {
                        if (_world.collisionMatrix[i][j]) 
                            continue;
                            
                        numExceptions++;
                        exceptionList.Add(
                            $"Ignore collision between \"{LayerMask.LayerToName(i)}\" and \"{LayerMask.LayerToName(31 - j)}\"");
                    }
                }

                GUIStyle textStyle = EditorStyles.label;
                textStyle.wordWrap = true;
                EditorGUILayout.LabelField(
                    $"Number of Collision Exceptions: {numExceptions}\n{string.Join("\n", exceptionList)}", textStyle);

                GUILayout.EndVertical();
                    
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Capture Collision Matrix"))
                    _world.CaptureCollisionMatrix();
            }
        }

        #endregion
    }
}
#endif