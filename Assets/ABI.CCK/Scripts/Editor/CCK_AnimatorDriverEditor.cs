using System;
using System.Collections.Generic;
using ABI.CCK.Components;
using ABI.CCK.Scripts.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerParameter = UnityEngine.AnimatorControllerParameter;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;

[CustomEditor(typeof(AnimatorDriver))]
public class CCK_AnimatorDriverEditor : Editor
{
    private AnimatorDriver _animatorDriver;
    
    private ReorderableList _onEnterList;
    private ReorderableList _onExitList;

    private readonly List<string> _animatorParamNames = new List<string>();
    private readonly List<AnimatorDriverTask.ParameterType> _animatorParamTypes = new List<AnimatorDriverTask.ParameterType>();

    private bool _showLocalOnlyHelp;
    private bool _isInPlayMode;
    
    #region Unity Events

    private void OnEnable()
    {
        if (target == null) return;
        _animatorDriver = (AnimatorDriver)target;

        _isInPlayMode = Application.isPlaying;
        
        _animatorParamNames.Clear();
        _animatorParamTypes.Clear();
        // TODO: add parameter type display in parameter dropdown
        
        var behaviorContext = AnimatorController.FindStateMachineBehaviourContext(_animatorDriver);
        if (behaviorContext.Length > 0)
        {
            AnimatorController controller = behaviorContext[0].animatorController;
            foreach (AnimatorControllerParameter parameter in controller.parameters)
            {
                switch (parameter.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        _animatorParamNames.Add($"{parameter.name}");
                        _animatorParamTypes.Add(AnimatorDriverTask.ParameterType.Bool);
                        break;
                    case AnimatorControllerParameterType.Float:
                        _animatorParamNames.Add($"{parameter.name}");
                        _animatorParamTypes.Add(AnimatorDriverTask.ParameterType.Float);
                        break;
                    case AnimatorControllerParameterType.Int:
                        _animatorParamNames.Add($"{parameter.name}");
                        _animatorParamTypes.Add(AnimatorDriverTask.ParameterType.Int);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        _animatorParamNames.Add($"{parameter.name}");
                        _animatorParamTypes.Add(AnimatorDriverTask.ParameterType.Trigger);
                        break;
                }
            }
        }
        
        if (_onEnterList == null)
        {
            _onEnterList = new ReorderableList(_animatorDriver.EnterTasks, typeof(AnimatorDriverTask),
                true, true, !_isInPlayMode, !_isInPlayMode)
            {
                drawHeaderCallback = OnDrawHeaderTaskEnter,
                drawElementCallback = OnDrawElementTaskEnter,
                elementHeightCallback = OnHeightElementTaskEnter
            };
        }

        if (_onExitList == null)
        {
            _onExitList = new ReorderableList(_animatorDriver.ExitTasks, typeof(AnimatorDriverTask),
                true, true, !_isInPlayMode, !_isInPlayMode)
            {
                drawHeaderCallback = OnDrawHeaderTaskExit,
                drawElementCallback = OnDrawElementTaskExit,
                elementHeightCallback = OnHeightElementTaskExit
            };
        }
    }

    public override void OnInspectorGUI()
    {
        if (_animatorDriver == null)
            return;

        EditorGUI.BeginChangeCheck();
        using (new EditorGUILayout.VerticalScope(new GUIStyle() { padding = new RectOffset(10, 10, 10, 10) }))
        {
            DrawLocalOnlyToggle();
            DrawTaskLists();
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(_animatorDriver);
    }
    
    #endregion

    #region GUI Drawing
    
    private void DrawLocalOnlyToggle()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            _animatorDriver.localOnly = EditorGUILayout.Toggle("Local Only", _animatorDriver.localOnly);
            if (GUILayout.Button("?", GUILayout.Width(25)))
                _showLocalOnlyHelp = !_showLocalOnlyHelp;
            GUILayout.Space(5);
        }
        if (_showLocalOnlyHelp)
        {
            // TODO: Add to LocalizationProvider
            EditorGUILayout.HelpBox("When 'Local Only' is enabled, the animator driver is executed locally and not for remote users.", MessageType.Info);
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("Avatars: Only the wearer.\nSpawnables: Only the prop's owner.\nWorlds: This option is ignored.", MessageType.Info);
        }
        EditorGUILayout.Space();
    }

    private void DrawTaskLists()
    {
        EditorGUILayout.Space();
        _onEnterList.DoLayoutList();
        
        EditorGUILayout.Space();
        _onExitList.DoLayoutList();
    }

    #endregion

    #region ReorderableList Drawing

    private void OnDrawHeaderTaskEnter(Rect rect)
    {
        Rect labelRect = new Rect(rect.x, rect.y, rect.width - 35, EditorGUIUtility.singleLineHeight);
        GUI.Label(labelRect, "On Enter Tasks");
        EditorGUIExtensions.UtilityMenu(rect, _onEnterList, (menuBuilder, list) => {
            AppendComponentMenu(menuBuilder, list, true);
        });
    }

    private void OnDrawElementTaskEnter(Rect rect, int index, bool isactive, bool isfocused)
    {
        if (index >= _animatorDriver.EnterTasks.Count) return;
        AnimatorDriverTask element = _animatorDriver.EnterTasks[index];
        using (new EditorGUI.DisabledScope(_isInPlayMode))
            RenderTask(rect, element);
    }

    private float OnHeightElementTaskEnter(int index)
    {
        const int length = 3;
        if (index >= _animatorDriver.EnterTasks.Count) 
            return 1.25f * length * EditorGUIUtility.singleLineHeight;
        
        AnimatorDriverTask task = _animatorDriver.EnterTasks[index];
        return (length + TaskHeight(task)) * 1.25f * EditorGUIUtility.singleLineHeight;
    }
    
    private void OnDrawHeaderTaskExit(Rect rect)
    {
        Rect labelRect = new Rect(rect.x, rect.y, rect.width - 35, EditorGUIUtility.singleLineHeight);
        GUI.Label(labelRect, "On Exit Tasks");
        EditorGUIExtensions.UtilityMenu(rect, _onExitList, (menuBuilder, list) => {
            AppendComponentMenu(menuBuilder, list, false);
        });
    }

    private void OnDrawElementTaskExit(Rect rect, int index, bool isactive, bool isfocused)
    {
        if (index >= _animatorDriver.ExitTasks.Count) return;
        AnimatorDriverTask element = _animatorDriver.ExitTasks[index];
        using (new EditorGUI.DisabledScope(_isInPlayMode))
            RenderTask(rect, element);
    }

    private float OnHeightElementTaskExit(int index)
    {
        const int length = 3;
        if (index >= _animatorDriver.ExitTasks.Count) 
            return 1.25f * length * EditorGUIUtility.singleLineHeight;
        
        AnimatorDriverTask task = _animatorDriver.ExitTasks[index];
        return (length + TaskHeight(task)) * 1.25f * EditorGUIUtility.singleLineHeight;
    }
    
    #endregion

    #region AnimatorDriverTask Drawing

    private int TaskHeight(AnimatorDriverTask task)
    {
        int length = 2;
        if (task.aType == AnimatorDriverTask.SourceType.Random) 
            length += 1;
        
        if (task.op == AnimatorDriverTask.Operator.Set) 
            return length;
        
        length += task.bType == AnimatorDriverTask.SourceType.Random ? 3 : 2;
        return length;
    }

    private void RenderTask(Rect rect, AnimatorDriverTask task)
    {
        Rect _rect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
        
        float spacing = EditorGUIUtility.singleLineHeight * 1.25f;
        float originalLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 100;

        task.targetName = EditorGUIExtensions.AdvancedDropdownInput(_rect, task.targetName, _animatorParamNames,
            "Parameter", "No Parameters");
        _rect.y += spacing;
        
        var formulaDisplay = $"{task.targetName} = ";
        int parameterIndex = Math.Max(_animatorParamNames.FindIndex(m => m == task.targetName), 0);
        if (_animatorParamNames.Count != 0 && _animatorParamTypes.Count >= parameterIndex)
            task.targetType = _animatorParamTypes[parameterIndex];
    
        task.op = (AnimatorDriverTask.Operator)EditorGUI.EnumPopup(_rect, "Operation", task.op);
        _rect.y += spacing;

        task.aType = (AnimatorDriverTask.SourceType)EditorGUI.EnumPopup(_rect, "A Type", task.aType);
        _rect.y += spacing;
        
        switch (task.aType)
        {
            case AnimatorDriverTask.SourceType.Static:
                task.aValue = EditorGUI.FloatField(_rect, "A Value", task.aValue);
                _rect.y += spacing;
                formulaDisplay += $"{task.aValue} ";
                break;
            case AnimatorDriverTask.SourceType.Parameter:
                task.aName = EditorGUIExtensions.AdvancedDropdownInput(_rect, task.aName, _animatorParamNames,
                    "Parameter A", "No Parameters");
                _rect.y += spacing;
                parameterIndex = Math.Max(_animatorParamNames.FindIndex(m => m == task.aName), 0);
                task.aParamType = _animatorParamTypes[parameterIndex];
                formulaDisplay += $"{task.aName} ";
                break;
            case AnimatorDriverTask.SourceType.Random:
                task.aValue = EditorGUI.FloatField(_rect, "A Min", task.aValue);
                _rect.y += spacing;
                task.aMax = EditorGUI.FloatField(_rect, "A Max", task.aMax);
                _rect.y += spacing;
                formulaDisplay += $"Rand({task.aValue}, {task.aMax}) ";
                break;
        }

        if (task.op != AnimatorDriverTask.Operator.Set)
        {
            switch (task.op)
            {
                case AnimatorDriverTask.Operator.Addition:
                    formulaDisplay += "+ ";
                    break;
                case AnimatorDriverTask.Operator.Subtraction:
                    formulaDisplay += "- ";
                    break;
                case AnimatorDriverTask.Operator.Multiplication:
                    formulaDisplay += "* ";
                    break;
                case AnimatorDriverTask.Operator.Division:
                    formulaDisplay += "/ ";
                    break;
                case AnimatorDriverTask.Operator.Modulo:
                    formulaDisplay += "% ";
                    break;
                case AnimatorDriverTask.Operator.Power:
                    formulaDisplay += "pow ";
                    break;
                case AnimatorDriverTask.Operator.Log:
                    formulaDisplay += "log ";
                    break;
                case AnimatorDriverTask.Operator.Equal:
                    formulaDisplay += "== ";
                    break;
                case AnimatorDriverTask.Operator.LessThen:
                    formulaDisplay += "< ";
                    break;
                case AnimatorDriverTask.Operator.LessEqual:
                    formulaDisplay += "<= ";
                    break;
                case AnimatorDriverTask.Operator.MoreThen:
                    formulaDisplay += "> ";
                    break;
                case AnimatorDriverTask.Operator.MoreEqual:
                    formulaDisplay += ">= ";
                    break;
                case AnimatorDriverTask.Operator.NotEqual:
                    formulaDisplay += "!= ";
                    break;
            }
            
            task.bType = (AnimatorDriverTask.SourceType) EditorGUI.EnumPopup(_rect, "B Type", task.bType);
            _rect.y += spacing;
            
            switch (task.bType)
            {
                case AnimatorDriverTask.SourceType.Static:
                    task.bValue = EditorGUI.FloatField(_rect, "B Value", task.bValue);
                    _rect.y += spacing;
                    formulaDisplay += $"{task.bValue} ";
                    break;
                case AnimatorDriverTask.SourceType.Parameter:
                    task.bName = EditorGUIExtensions.AdvancedDropdownInput(_rect, task.bName, _animatorParamNames,
                        "Parameter B", "No Parameters");
                    _rect.y += spacing;               
                    parameterIndex = Math.Max(_animatorParamNames.FindIndex(m => m == task.bName), 0);
                    task.bParamType = _animatorParamTypes[parameterIndex];
                    formulaDisplay += $"{task.bName} ";
                    break;
                case AnimatorDriverTask.SourceType.Random:
                    task.bValue = EditorGUI.FloatField(_rect, "B Min", task.bValue);
                    _rect.y += spacing;
                    task.bMax = EditorGUI.FloatField(_rect, "B Max", task.bMax);
                    _rect.y += spacing;
                    formulaDisplay += $"Rand({task.bValue}, {task.bMax}) ";
                    break;
            }
        }

        EditorGUI.LabelField(_rect, task.targetName == "" ? "No Parameter" : formulaDisplay,
            new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        
        EditorGUIUtility.labelWidth = originalLabelWidth;
    }

    #endregion

    #region Utility
    
    private void AppendComponentMenu(GenericMenuBuilder genericMenuBuilder, ReorderableList list, bool isEnterList)
    {
        bool hasSelectedTask = list.index != -1;
        bool hasTasks = list.count > 0;
    
        if (isEnterList)
        {
            genericMenuBuilder.AddMenuItem("To Exit Task", hasTasks && hasSelectedTask, () => ConvertSelectedTask(list, _animatorDriver.EnterTasks, _animatorDriver.ExitTasks));
            genericMenuBuilder.AddMenuItem("All to Exit Task", hasTasks, ConvertAllTasksToExit);
        }
        else
        {
            genericMenuBuilder.AddMenuItem("To Enter Task", hasTasks && hasSelectedTask, () => ConvertSelectedTask(list, _animatorDriver.ExitTasks, _animatorDriver.EnterTasks));
            genericMenuBuilder.AddMenuItem("All to Enter Task", hasTasks, ConvertAllTasksToEnter);
        }
    }
    
    private void ConvertAllTasksToEnter()
    {
        _animatorDriver.EnterTasks.AddRange(_animatorDriver.ExitTasks);
        _animatorDriver.ExitTasks.Clear();
    }

    private void ConvertAllTasksToExit()
    {
        _animatorDriver.ExitTasks.AddRange(_animatorDriver.EnterTasks);
        _animatorDriver.EnterTasks.Clear();
    }
    
    private void ConvertSelectedTask(ReorderableList list, List<AnimatorDriverTask> fromTasks, List<AnimatorDriverTask> toTasks)
    {
        if (list.index == -1 || list.index >= fromTasks.Count) return;

        AnimatorDriverTask selectedTask = fromTasks[list.index];
        fromTasks.RemoveAt(list.index);
        toTasks.Add(selectedTask);
    }
    
    #endregion
}