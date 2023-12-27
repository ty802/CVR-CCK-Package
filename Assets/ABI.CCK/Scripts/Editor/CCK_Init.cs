using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

#pragma warning disable

[InitializeOnLoad]
public class CCK_Init
{
    static CCK_Init()
    {
        const string cckSymbol = "CVR_CCK_EXISTS";
        BuildTargetGroup selectedTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(selectedTargetGroup);

        if (!defines.Contains(cckSymbol))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(selectedTargetGroup, defines + ";" + cckSymbol);
            Debug.Log("[CCK:Init] Added CVR_CCK_EXISTS Scripting Symbol.");
        }

        bool shouldRecreateTagManager = LayerMask.LayerToName(10) != "PlayerNetwork" 
                                  || LayerMask.LayerToName(15) != "CVRReserved3" 
                                  || LayerMask.LayerToName(6) != "PassPlayer";

        if (shouldRecreateTagManager)
        {
            Debug.Log("[CCK:Init] TagManager asset has to be recreated. Now recreating.");
            ResetTagManager();
        }

#if UNITY_2021_1_OR_NEWER
        if (true)
#else
        if (!PlayerSettings.virtualRealitySupported)
#endif
        {
            Debug.Log("[CCK:Init] XR and render settings have to be changed. Now changing.");

    #if PLATFORM_ANDROID
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.Android);
    #else
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
    #endif
            
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_4_6;
            PlayerSettings.legacyClampBlendShapeWeights = true;

    #if UNITY_2021_1_OR_NEWER
            PlayerSettings.virtualRealitySupported = true;
            PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;
            XRSettings.enabled = false;
    #else
            PlayerSettings.virtualRealitySupported = true;
            PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Standalone, new[] { "None", "Oculus", "OpenVR", "MockHMD" });
            PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
    #endif
        }

        if (!shouldRecreateTagManager && PlayerSettings.virtualRealitySupported)
            Debug.Log("[CCK:Init] Verified TagManager and ProjectSettings. No need to readjust.");
    }

    [MenuItem("Alpha Blend Interactive/Utilities/Reset Layer Names", false, 260)]
    private static void ForceResetTagManager()
    {
        bool proceed = EditorUtility.DisplayDialog(
            "CCK :: Reset Layer Names",
            "Are you sure you want to reset all layer names? This will reset all custom layer names to the CCK defaults.",
            "Yes",
            "No");

        if (!proceed)
            return;
        
        ResetTagManager(true);
    }
    
    private static void ResetTagManager(bool forceReset = false)
    {
        SerializedObject tagManager =
            new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layerProperty = tagManager.FindProperty("layers");

        var layerList = new (int Index, string Name)[]
        {
            // game layers, can be force renamed
            (6, "PassPlayer"), (7, "BlockPlayer"),
            (8, "PlayerLocal"), (9, "PlayerClone"), (10, "PlayerNetwork"),
            (11, "MirrorReflection"), (12, "Camera Only"),
            (13, "CVRReserved1"), (14, "CVRReserved2"), (15, "CVRReserved3"),
            // User Content world layers, do not normally force rename
            (16, "CVRContent1 (can be renamed)"), (17, "CVRContent2 (can be renamed)"),
            (18, "CVRContent3 (can be renamed)"), (19, "CVRContent4 (can be renamed)"),
            (20, "CVRContent5 (can be renamed)"), (21, "CVRContent6 (can be renamed)"),
            (22, "CVRContent7 (can be renamed)"), (23, "CVRContent8 (can be renamed)"),
            (24, "CVRContent9 (can be renamed)"), (25, "CVRContent10 (can be renamed)"),
            (26, "CVRContent11 (can be renamed)"), (27, "CVRContent12 (can be renamed)"),
            (28, "CVRContent13 (can be renamed)"), (29, "CVRContent14 (can be renamed)"),
            (30, "CVRContent15 (can be renamed)"), (31, "CVRContent16 (can be renamed)")
        };

        foreach (var (Index, Name) in layerList)
        {
            SerializedProperty sp = layerProperty.GetArrayElementAtIndex(Index);

            if (sp == null) continue;
 
            // always rename
            if (Index >= 6 && Index <= 15)
            {
                sp.stringValue = Name;
            }
            // only rename if legacy or forced
            else if (forceReset || 
                     sp.stringValue == "" || 
                     sp.stringValue == "PostProcessing" || // :)
                     sp.stringValue == "CVRPickup" || 
                     sp.stringValue == "CVRInteractable" || 
                     sp.stringValue.StartsWith("RCC_"))
            {
                sp.stringValue = Name;
            }
        }

        tagManager.ApplyModifiedProperties();
    }
}