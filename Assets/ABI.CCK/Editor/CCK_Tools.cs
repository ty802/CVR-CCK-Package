using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using System.Xml.XPath;
using ABI.CCK.Components;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace ABI.CCK.Scripts.Editor
{
    public class CCK_Tools
    {
        public enum SearchType
        {
            Scene = 1, 
            Selection = 2
        }
        
        public static int CleanMissingScripts(SearchType searchType, bool shouldRemove, GameObject givenObject)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            GameObject[] rootObjects = (searchType == SearchType.Scene)
                ? activeScene.GetRootGameObjects()
                : new GameObject[] { givenObject };

            List<GameObject> allFoundObjects = rootObjects
                .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                .Select(t => t.gameObject)
                .ToList();

            int scriptCount = 0;
            int goCount = 0;

            foreach (var go in allFoundObjects)
            {
                int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (count <= 0) continue;

                if (shouldRemove)
                {
                    Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                    if (PrefabUtility.IsPartOfAnyPrefab(go))
                    {
                        GameObject prefabInstance = PrefabUtility.GetNearestPrefabInstanceRoot(go);
                        if (prefabInstance != null)
                            PrefabUtility.ApplyPrefabInstance(prefabInstance, InteractionMode.AutomatedAction);
                    }
                }

                scriptCount += count;
                goCount++;
            }

            if (shouldRemove)
            {
                EditorSceneManager.MarkSceneDirty(activeScene);
                EditorSceneManager.SaveScene(activeScene);
                Debug.Log($"[CCK:Tools] Found and removed {scriptCount} missing scripts from {goCount} GameObjects");
            }

            return scriptCount;
        }
        
        public static void CleanEditorOnlyGameObjects(GameObject gameObject)
        {
            var objectsToDestroy = (from child in gameObject.GetComponentsInChildren<Transform>(true)
                where child.CompareTag("EditorOnly")
                select child.gameObject).ToList();

            for (int i = objectsToDestroy.Count - 1; i >= 0; i--)
                UnityEngine.Object.DestroyImmediate(objectsToDestroy[i]);
        }
        
        [MenuItem("Assets/Create/CVR Override Controller")]
        private static void CreateCVROverrideController()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject) + "/New Override Controller.overrideController";
            string[] guids = AssetDatabase.FindAssets("AvatarAnimator t:animatorController", null);

            if (guids.Length < 1)
            {
                Debug.LogError("No Animator controller with the name \"AvatarAnimator\" was found. Please make sure that you CCK is installed properly.");
                return;
            }
            var overrideController = new AnimatorOverrideController();
            overrideController.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(guids[0]));
            
            AssetDatabase.CreateAsset (overrideController, path);
        }
    }
}
