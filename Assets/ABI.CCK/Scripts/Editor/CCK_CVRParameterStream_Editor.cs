using System.Collections.Generic;
using ABI.CCK.Components;
using ABI.CCK.Scripts.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;

namespace ABI.CCK.Scripts.Editor
{
    [CustomEditor(typeof(CVRParameterStream))]
    public class CCK_CVRParameterStream_Editor : UnityEditor.Editor
    {
        private CVRAvatar avatar;
        private CVRSpawnable spawnable;
        
        private CVRParameterStream stream;
        private ReorderableList list;

        public enum TargetTypeSpawnable
        {
            Animator = 0,
            CustomFloat = 3,
        }
        
        public void OnEnable()
        {
            // Occurs on recompile
            if (target == null)
                return; 
            
            stream = (CVRParameterStream)target;
            avatar = stream.transform.GetComponentInParent<CVRAvatar>();
            spawnable = stream.transform.GetComponentInParent<CVRSpawnable>();
            
            if (avatar != null)
                stream.referenceType = CVRParameterStream.ReferenceType.Avatar;
            else if (spawnable != null)
                stream.referenceType = CVRParameterStream.ReferenceType.Spawnable;
            else
                stream.referenceType = CVRParameterStream.ReferenceType.World;
            
            if (list == null)
            {
                list = new ReorderableList(stream.entries, typeof(CVRParameterStreamEntry), 
                    true, true, true, true)
                {
                    drawHeaderCallback = DrawHeaderCallback,
                    drawElementCallback = DrawElementCallback,
                    elementHeightCallback = ElementHeightCallback
                };
            }
        }

        public override void OnInspectorGUI()
        {
            if (stream == null)
                return;
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.LabelField($"Type: {stream.referenceType}");
            
            if (stream.referenceType != CVRParameterStream.ReferenceType.Avatar)
            {
                stream.onlyUpdateWhenHeld = EditorGUILayout.Toggle("only update when held", stream.onlyUpdateWhenHeld);
                stream.onlyUpdateWhenAttached = EditorGUILayout.Toggle("only update when attached", stream.onlyUpdateWhenAttached);
                stream.onlyUpdateWhenControlled = EditorGUILayout.Toggle("only update when controlled", stream.onlyUpdateWhenControlled);
            }
            
            list.DoLayoutList();
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        private float ElementHeightCallback(int index)
        {
            return EditorGUIUtility.singleLineHeight * 1.25f * (((int) stream.entries[index].applicationType % 5 == 1?5f:4f) + 
                   (stream.entries[index].targetType == CVRParameterStreamEntry.TargetType.Animator ? 1f : 0f));
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index >= stream.entries.Count) return;
        
            rect.y += 2f;

            switch (stream.referenceType)
            {
                case CVRParameterStream.ReferenceType.World:
                    stream.entries[index].type = (CVRParameterStreamEntry.Type) EditorGUI.EnumPopup(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                        "Type",
                        stream.entries[index].type
                    );

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                
                    stream.entries[index].targetType = (CVRParameterStreamEntry.TargetType) EditorGUI.Popup(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                        "Output Type",
                        (int) stream.entries[index].targetType,
                        new []{"Animator", "VariableBuffer"}
                    );

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                
                    stream.entries[index].target = (GameObject) EditorGUI.ObjectField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                        "Target",
                        stream.entries[index].target,
                        typeof(GameObject),
                        true
                    );

                    if (stream.entries[index].target)
                    {
                        switch (stream.entries[index].targetType)
                        {
                            case CVRParameterStreamEntry.TargetType.Animator:
                                var animator = stream.entries[index].target.GetComponent<Animator>();
                                if (animator == null) stream.entries[index].target = null;
                                break;
                            case CVRParameterStreamEntry.TargetType.VariableBuffer:
                                var varBuffer = stream.entries[index].target.GetComponent<CVRVariableBuffer>();
                                if (varBuffer == null) stream.entries[index].target = null;
                                break;
                        }
                    }

                    if (stream.entries[index].target != null && stream.entries[index].targetType == CVRParameterStreamEntry.TargetType.Animator)
                    {
                        rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                        Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                        
                        Animator animator = stream.entries[index].target.GetComponent<Animator>();
                        
                        stream.entries[index].parameterName = EditorGUIExtensions.AdvancedDropdownInput(_rect,
                            stream.entries[index].parameterName, CVRCommon.GetParametersFromAnimatorAsString(animator), 
                            "Parameter","No Parameters");
                    }
                    break;
                
                case CVRParameterStream.ReferenceType.Avatar:
                    stream.entries[index].type = (CVRParameterStreamEntry.Type) EditorGUI.EnumPopup(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                        "Type",
                        stream.entries[index].type
                    );

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    
                    var _rectA = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    
                    EditorGUI.LabelField(_rectA, "Output Type", "AdvancedAvatarAnimator");
                    
                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                    stream.entries[index].targetType = CVRParameterStreamEntry.TargetType.AvatarAnimator;
                    
                    _rectA = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    
                    var avatarParameterNames = new List<string>();
                    if (avatar.overrides != null && avatar.overrides is AnimatorOverrideController overrideController &&
                        overrideController.runtimeAnimatorController is AnimatorController animatorController)
                        avatarParameterNames.AddRange(CVRCommon.GetParametersFromControllerAsString(animatorController, CVRCommon.NonCoreFilter));

                    stream.entries[index].parameterName = EditorGUIExtensions.AdvancedDropdownInput(_rectA,
                        stream.entries[index].parameterName, avatarParameterNames, 
                        "Parameter","No Parameters");
                    
                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                    stream.entries[index].applicationType = (CVRParameterStreamEntry.ApplicationType) EditorGUI.EnumPopup(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                        "Value Application",
                        stream.entries[index].applicationType
                    );

                    if ((int) stream.entries[index].applicationType % 5 == 1)
                    {
                        rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                        stream.entries[index].staticValue = EditorGUI.FloatField(
                            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            "Static Value",
                            stream.entries[index].staticValue
                        );
                    }
                    
                    break;
                
                case CVRParameterStream.ReferenceType.Spawnable:
                    stream.entries[index].type = (CVRParameterStreamEntry.Type)EditorGUI.EnumPopup(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        "Type",
                        stream.entries[index].type
                    );

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                    var _rectB = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    stream.entries[index].targetType = (CVRParameterStreamEntry.TargetType) EditorGUI.EnumPopup(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                        "Output Type", 
                        (TargetTypeSpawnable) stream.entries[index].targetType
                    );
                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                    _rectB = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

                    if (stream.entries[index].targetType == CVRParameterStreamEntry.TargetType.CustomFloat)
                    {
                        var spawnableParameterNames = new List<string>();
                        if (spawnable != null)
                        {
                            foreach (CVRSpawnableValue parameter in spawnable.syncValues)
                            {
                                if (!string.IsNullOrWhiteSpace(parameter.name))
                                    spawnableParameterNames.Add(parameter.name);
                            }
                        }

                        stream.entries[index].parameterName = EditorGUIExtensions.AdvancedDropdownInput(_rectB,
                            stream.entries[index].parameterName, spawnableParameterNames,
                            "Synced Value", "No Synced Values");
                    }
                    else if (stream.entries[index].targetType == CVRParameterStreamEntry.TargetType.Animator)
                    {
                        stream.entries[index].target = (GameObject) EditorGUI.ObjectField(
                            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                            "Target",
                            stream.entries[index].target,
                            typeof(GameObject),
                            true
                        );

                        if (stream.entries[index].target == null) return;
                        
                        var animator = stream.entries[index].target.GetComponent<Animator>();
                        if (animator == null) stream.entries[index].target = null;
                        
                        rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                        Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

                        if (animator == null) return;
                        
                        stream.entries[index].parameterName = EditorGUIExtensions.AdvancedDropdownInput(_rect,
                            stream.entries[index].parameterName, CVRCommon.GetParametersFromAnimatorAsString(animator), 
                            "Parameter","No Parameters");
                    }

                    break;
            }
        }

        private void DrawHeaderCallback(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "Entries");
            EditorGUIExtensions.UtilityMenu(_rect, list);
        }
    }
}