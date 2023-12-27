#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ABI.CCK.Scripts;
using ABI.CCK.Scripts.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerParameter = UnityEngine.AnimatorControllerParameter;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRAvatarEditor
    {
        private ReorderableList _advSettingsList;
        
        private int _syncedValues;
        private int _syncedBooleans;
        private readonly List<string> _parameters = new List<string>();
        
        private bool _definitionContainsError;

        private void InitializeSettingsList()
        {
            if (_advSettingsList != null)
                return;
            
            if (_avatar.avatarSettings == null || !_avatar.avatarSettings.initialized)
                CreateAvatarSettings(_avatar);
            
            if (_avatar.avatarSettings == null) 
                return;

            _advSettingsList = new ReorderableList(_avatar.avatarSettings.settings, typeof(CVRAdvancedSettingsEntry),
                true, true, true, true)
            {
                drawHeaderCallback = OnDrawHeaderAAS,
                drawElementCallback = OnDrawElementAAS,
                elementHeightCallback = OnHeightElementAAS,
                onChangedCallback = OnChangedAAS
            };
        }

        private void Draw_AdvancedSettings()
        {
            using (new ToggleFoldoutScope(ref _guiAdvancedSettingsFoldout, ref _avatar.avatarUsesAdvancedSettings, "Advanced Settings"))
            {
                if (!_guiAdvancedSettingsFoldout) return;

                InitializeSettingsList();

                using (new EditorGUI.DisabledGroupScope(!_avatar.avatarUsesAdvancedSettings))
                    DrawAvatarAdvancedSettings();
            }
        }
        
        // TODO: Cleanup all of this...
        // Decouple AutoGen from GUI
        // Add simplified menu entry mode (no AutoGen)

        private void DrawAvatarAdvancedSettings()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            if (_avatar.avatarSettings != null)
            {
                _avatar.avatarSettings.baseController = EditorGUILayout.ObjectField("Base Controller",
                    _avatar.avatarSettings.baseController, typeof(RuntimeAnimatorController),
                    false) as RuntimeAnimatorController;

                if (_avatar.avatarSettings.baseController is AnimatorOverrideController)
                    _avatar.avatarSettings.baseController = null;

                EditorGUILayout.HelpBox(
                    "This is the Base Controller that is extended for the creation of your Advanced Avatar Settings. " +
                    "If you do not want to extend a specific Animator Controller, make sure that the Default Avatar Animator " +
                    "From the Directory \"ABI.CCK/Animations\" is used here.", MessageType.Info);

                _avatar.avatarSettings.baseOverrideController = EditorGUILayout.ObjectField("Override Controller",
                    _avatar.avatarSettings.baseOverrideController, typeof(RuntimeAnimatorController),
                    false) as RuntimeAnimatorController;

                if (_avatar.avatarSettings.baseOverrideController is AnimatorController)
                    _avatar.avatarSettings.baseOverrideController = null;

                EditorGUILayout.HelpBox(
                    "You can Put your previous Override Controller here in order to put your overrides in " +
                    "the newly created Override Controller.", MessageType.Info);
                
                (int, int) current = _avatar.GetParameterSyncUsage();
                EditorGUIExtensions.MultiProgressBar(
                    EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight),
                    current.Item1 / CVRCommon.AVATAR_BIT_LIMIT,
                    (current.Item2) / CVRCommon.AVATAR_BIT_LIMIT,
                    $"({current.Item1}, {current.Item2}) of 3200 Synced Bits used"
                );
                
                _advSettingsList.DoLayoutList();

                if (current.Item2 <= CVRCommon.AVATAR_BIT_LIMIT)
                {
                    if (GUILayout.Button("Create Controller"))
                        CreateAnimator();
                }
                else
                {
                    using (new EditorGUI.DisabledScope(true))
                        GUILayout.Button("Create Controller");
                    EditorGUILayout.HelpBox(
                        "Cannot create controller. You are over the Synced Bit Limit!", MessageType.Warning);
                    GUILayout.Space(5);
                }
                
                if (_avatar.avatarSettings.overrides != null && _avatar.avatarSettings.overrides != _avatar.overrides)
                {
                    if (GUILayout.Button("Attach created Override to Avatar"))
                        _avatar.overrides = _avatar.avatarSettings.overrides;
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        #region ReorderableListDrawing AAS

        private void OnDrawHeaderAAS(Rect rect)
        {
            Rect labelRect = new Rect(rect.x, rect.y, rect.width - 35, EditorGUIUtility.singleLineHeight);
            GUI.Label(labelRect, "Inputs");
            EditorGUIExtensions.UtilityMenu(rect, _advSettingsList, (menuBuilder, list) => {
                AppendComponentMenu(menuBuilder, _advSettingsList);
            });
        }

        private void OnChangedAAS(ReorderableList list)
        {
            EditorUtility.SetDirty(target);
        }
        
        private float OnHeightElementAAS(int index)
        {
            if (index >= _avatar.avatarSettings.settings.Count) return EditorGUIUtility.singleLineHeight * 1f;
            CVRAdvancedSettingsEntry advSettingEntry = _avatar.avatarSettings.settings[index];

            // When collapsed only return one line height
            if (!advSettingEntry.setting.isCollapsed) return EditorGUIUtility.singleLineHeight * 1.25f;

            if (string.IsNullOrWhiteSpace(advSettingEntry.name))
                return EditorGUIUtility.singleLineHeight * 3.75f;
            
            switch (advSettingEntry.type)
            {
                case CVRAdvancedSettingsEntry.SettingsType.GameObjectToggle:
                {
                    CVRAdvancesAvatarSettingGameObjectToggle gameObjectToggle = (CVRAdvancesAvatarSettingGameObjectToggle)advSettingEntry.setting;
                    if (gameObjectToggle == null || gameObjectToggle.gameObjectTargets == null)
                        return EditorGUIUtility.singleLineHeight * 11.50f;
                    float height = 10.50f;
                    if (gameObjectToggle.useAnimationClip)
                    {
                        //height -= 1.25f;
                    }
                    else
                    {
                        foreach (CVRAdvancedSettingsTargetEntryGameObject gameobjectTarget in gameObjectToggle.gameObjectTargets)
                        {
                            if (!gameobjectTarget.isCollapsed)
                            {
                                height += 1.25f;
                            }
                            else
                            {
                                height += 3.75f;
                            }
                        }

                        if (gameObjectToggle.gameObjectTargets.Count == 0)
                        {
                            height += 1f;
                        }
                    }

                    return EditorGUIUtility.singleLineHeight * height;
                }
                case CVRAdvancedSettingsEntry.SettingsType.GameObjectDropdown:
                {
                    CVRAdvancesAvatarSettingGameObjectDropdown gameObjectDropdown = (CVRAdvancesAvatarSettingGameObjectDropdown)advSettingEntry.setting;
                    if (gameObjectDropdown == null || gameObjectDropdown.options == null)
                        return EditorGUIUtility.singleLineHeight * 9.25f;
                    float height = 8.25f;
                    foreach (CVRAdvancedSettingsDropDownEntry option in gameObjectDropdown.options)
                    {
                        height += 1;
                        if (option.isCollapsed)
                        {
                            height += 4;
                            if (option.useAnimationClip)
                            {
                                height -= 1.5f;
                            }
                            else
                            {
                                if (option.gameObjectTargets.Count != 0)
                                {
                                    foreach (CVRAdvancedSettingsTargetEntryGameObject gameObj in option.gameObjectTargets)
                                    {
                                        if (!gameObj.isCollapsed)
                                        {
                                            height += 1;
                                        }
                                        else
                                        {
                                            height += 3;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return EditorGUIUtility.singleLineHeight * height * 1.25f;
                }
                case CVRAdvancedSettingsEntry.SettingsType.MaterialColor:
                {
                    CVRAdvancedAvatarSettingMaterialColor materialColor = (CVRAdvancedAvatarSettingMaterialColor)advSettingEntry.setting;
                    if (materialColor == null || materialColor.materialColorTargets == null)
                        return EditorGUIUtility.singleLineHeight * 8f;
                    float height = 8f;
                    foreach (CVRAdvancedSettingsTargetEntryMaterialColor option in materialColor.materialColorTargets)
                    {
                        if (!option.isCollapsed)
                        {
                            height += 1.25f;
                        }
                        else
                        {
                            height += 3.75f;
                        }
                    }

                    if (materialColor.materialColorTargets.Count == 0)
                    {
                        height += 1f;
                    }

                    return EditorGUIUtility.singleLineHeight * height;
                }
                case CVRAdvancedSettingsEntry.SettingsType.Slider:
                {
                    CVRAdvancesAvatarSettingSlider slider = (CVRAdvancesAvatarSettingSlider)advSettingEntry.setting;
                    if (slider == null || slider.materialPropertyTargets == null)
                        return EditorGUIUtility.singleLineHeight * 8f;
                    float height = slider.useAnimationClip ? 8.75f : 12.5f;
                    if (slider.useAnimationClip) return height * EditorGUIUtility.singleLineHeight;
                    foreach (CVRAdvancedSettingsTargetEntryMaterialProperty option in slider.materialPropertyTargets)
                    {
                        if (!option.isCollapsed)
                        {
                            height += 1.25f;
                        }
                        else
                        {
                            height += 5 * 1.25f;
                        }
                    }

                    if (slider.materialPropertyTargets.Count == 0)
                    {
                        height += 1.25f;
                    }

                    return EditorGUIUtility.singleLineHeight * height;
                }
                case CVRAdvancedSettingsEntry.SettingsType.Joystick2D:
                case CVRAdvancedSettingsEntry.SettingsType.Joystick3D:
                    return EditorGUIUtility.singleLineHeight * 11.25f;
            }

            return EditorGUIUtility.singleLineHeight * 8.75f;
        }

        private void OnDrawElementAAS(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= _avatar.avatarSettings.settings.Count) return;
            CVRAdvancedSettingsEntry advSettingEntry = _avatar.avatarSettings.settings[index];
            rect.y += 2;
            rect.x += 12;
            rect.width -= 12;
            Rect _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            advSettingEntry.setting.isCollapsed = EditorGUI.Foldout(_rect, advSettingEntry.setting.isCollapsed, "Name", true);
            _rect.x += 100;
            _rect.width = rect.width - 100;
            var menuName = EditorGUI.TextField(_rect, advSettingEntry.name);
            
            // only change when menu name changes
            // dont want to overwrite existing parameter names :)
            if (menuName != advSettingEntry.name)
            {
                advSettingEntry.name = menuName;
                advSettingEntry.machineName = Regex.Replace(advSettingEntry.name, @"[^a-zA-Z0-9/.\-_#]", "");
            }

            // when collapsed skip rest of UI drawing
            if (!advSettingEntry.setting.isCollapsed)
            {
                advSettingEntry.RunCollapsedSetup();
                return;
            }

            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            if (string.IsNullOrWhiteSpace(advSettingEntry.name))
            {
                _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2f);
                EditorGUI.HelpBox(_rect, "Name cannot be empty", MessageType.Error);
                _definitionContainsError = true;
                return;
            }

            if (advSettingEntry.name == null || (advSettingEntry.name != null && advSettingEntry.machineName == null))
            {
                _definitionContainsError = true;
            }

            EditorGUI.LabelField(_rect, "Parameter");
            _rect.x += 100;
            _rect.width = rect.width - 100;

            Rect warningRect = new Rect(rect.x + 60, rect.y, 20, EditorGUIUtility.singleLineHeight);

            switch (advSettingEntry.type)
            {
                case CVRAdvancedSettingsEntry.SettingsType.GameObjectToggle:
                    EditorGUI.LabelField(_rect, advSettingEntry.machineName);
                    if (_parameters.Contains(advSettingEntry.machineName))
                    {
                        EditorGUI.HelpBox(warningRect, "", MessageType.Warning);
                        EditorGUI.LabelField(warningRect,
                            new GUIContent("", "This Layer already exists and will not be regenerated."));
                    }

                    break;
                case CVRAdvancedSettingsEntry.SettingsType.GameObjectDropdown:
                    EditorGUI.LabelField(_rect, advSettingEntry.machineName);
                    if (_parameters.Contains(advSettingEntry.machineName))
                    {
                        EditorGUI.HelpBox(warningRect, "", MessageType.Warning);
                        EditorGUI.LabelField(warningRect,
                            new GUIContent("", "This Layer already exists and will not be regenerated."));
                    }

                    break;
                case CVRAdvancedSettingsEntry.SettingsType.MaterialColor:
                    EditorGUI.LabelField(_rect, $"{advSettingEntry.machineName}-r, {advSettingEntry.machineName}-g, {advSettingEntry.machineName}-b");
                    if (_parameters.Contains(advSettingEntry.machineName + "-r") ||
                        _parameters.Contains(advSettingEntry.machineName + "-g") ||
                        _parameters.Contains(advSettingEntry.machineName + "-b"))
                    {
                        EditorGUI.HelpBox(warningRect, "", MessageType.Warning);
                        EditorGUI.LabelField(warningRect,
                            new GUIContent("", "This Layer already exists and will not be regenerated."));
                    }

                    break;
                case CVRAdvancedSettingsEntry.SettingsType.Slider:
                    EditorGUI.LabelField(_rect, advSettingEntry.machineName);
                    if (_parameters.Contains(advSettingEntry.machineName))
                    {
                        EditorGUI.HelpBox(warningRect, "", MessageType.Warning);
                        EditorGUI.LabelField(warningRect,
                            new GUIContent("", "This Layer already exists and will not be regenerated."));
                    }

                    break;
                case CVRAdvancedSettingsEntry.SettingsType.Joystick2D:
                    EditorGUI.LabelField(_rect, $"{advSettingEntry.machineName}-x, {advSettingEntry.machineName}-y");
                    if (_parameters.Contains(advSettingEntry.machineName + "-x") ||
                        _parameters.Contains(advSettingEntry.machineName + "-y"))
                    {
                        EditorGUI.HelpBox(warningRect, "", MessageType.Warning);
                        EditorGUI.LabelField(warningRect,
                            new GUIContent("", "This Layer already exists and will not be regenerated."));
                    }

                    break;
                case CVRAdvancedSettingsEntry.SettingsType.Joystick3D:
                    EditorGUI.LabelField(_rect, $"{advSettingEntry.machineName}-x, {advSettingEntry.machineName}-y, {advSettingEntry.machineName}-z");
                    if (_parameters.Contains(advSettingEntry.machineName + "-x") ||
                        _parameters.Contains(advSettingEntry.machineName + "-y") ||
                        _parameters.Contains(advSettingEntry.machineName + "-z"))
                    {
                        EditorGUI.HelpBox(warningRect, "", MessageType.Warning);
                        EditorGUI.LabelField(warningRect,
                            new GUIContent("", "This Layer already exists and will not be regenerated."));
                    }

                    break;
                case CVRAdvancedSettingsEntry.SettingsType.InputSingle:
                    EditorGUI.LabelField(_rect, advSettingEntry.machineName);
                    if (_parameters.Contains(advSettingEntry.machineName))
                    {
                        EditorGUI.HelpBox(warningRect, "", MessageType.Warning);
                        EditorGUI.LabelField(warningRect,
                            new GUIContent("", "This Layer already exists and will not be regenerated."));
                    }

                    break;
                case CVRAdvancedSettingsEntry.SettingsType.InputVector2:
                    EditorGUI.LabelField(_rect, $"{advSettingEntry.machineName}-x, {advSettingEntry.machineName}-y");
                    if (_parameters.Contains(advSettingEntry.machineName + "-x") ||
                        _parameters.Contains(advSettingEntry.machineName + "-y"))
                    {
                        EditorGUI.HelpBox(warningRect, "", MessageType.Warning);
                        EditorGUI.LabelField(warningRect,
                            new GUIContent("", "This Layer already exists and will not be regenerated."));
                    }

                    break;
                case CVRAdvancedSettingsEntry.SettingsType.InputVector3:
                    EditorGUI.LabelField(_rect, $"{advSettingEntry.machineName}-x, {advSettingEntry.machineName}-y, {advSettingEntry.machineName}-z");
                    if (_parameters.Contains(advSettingEntry.machineName + "-x") ||
                        _parameters.Contains(advSettingEntry.machineName + "-y") ||
                        _parameters.Contains(advSettingEntry.machineName + "-z"))
                    {
                        EditorGUI.HelpBox(warningRect, "", MessageType.Warning);
                        EditorGUI.LabelField(warningRect,
                            new GUIContent("", "This Layer already exists and will not be regenerated."));
                    }

                    break;
            }

            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(_rect, "Type");
            _rect.x += 100;
            _rect.width = rect.width - 100;
            CVRAdvancedSettingsEntry.SettingsType type = (CVRAdvancedSettingsEntry.SettingsType)EditorGUI.EnumPopup(_rect, advSettingEntry.type);

            if (type != advSettingEntry.type)
            {
                advSettingEntry.type = type;

                switch (type)
                {
                    case CVRAdvancedSettingsEntry.SettingsType.GameObjectToggle:
                        advSettingEntry.setting = new CVRAdvancesAvatarSettingGameObjectToggle();
                        break;
                    case CVRAdvancedSettingsEntry.SettingsType.GameObjectDropdown:
                        advSettingEntry.setting = new CVRAdvancesAvatarSettingGameObjectDropdown();
                        break;
                    case CVRAdvancedSettingsEntry.SettingsType.MaterialColor:
                        advSettingEntry.setting = new CVRAdvancedAvatarSettingMaterialColor();
                        break;
                    case CVRAdvancedSettingsEntry.SettingsType.Slider:
                        advSettingEntry.setting = new CVRAdvancesAvatarSettingSlider();
                        break;
                    case CVRAdvancedSettingsEntry.SettingsType.Joystick2D:
                        advSettingEntry.setting = new CVRAdvancesAvatarSettingJoystick2D();
                        break;
                    case CVRAdvancedSettingsEntry.SettingsType.Joystick3D:
                        advSettingEntry.setting = new CVRAdvancesAvatarSettingJoystick3D();
                        break;
                    case CVRAdvancedSettingsEntry.SettingsType.InputSingle:
                        advSettingEntry.setting = new CVRAdvancesAvatarSettingInputSingle();
                        break;
                    case CVRAdvancedSettingsEntry.SettingsType.InputVector2:
                        advSettingEntry.setting = new CVRAdvancesAvatarSettingInputVector2();
                        break;
                    case CVRAdvancedSettingsEntry.SettingsType.InputVector3:
                        advSettingEntry.setting = new CVRAdvancesAvatarSettingInputVector3();
                        break;
                }
            }

            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            if (advSettingEntry.type == CVRAdvancedSettingsEntry.SettingsType.GameObjectToggle)
            {
                EditorGUI.LabelField(_rect, "Generate Type");
                _rect.x += 100;
                _rect.width = rect.width - 100;
                advSettingEntry.setting.usedType =
                    (CVRAdvancesAvatarSettingBase.ParameterType)EditorGUI.EnumPopup(_rect, advSettingEntry.setting.usedType);

                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);
            }

            if (advSettingEntry.type == CVRAdvancedSettingsEntry.SettingsType.GameObjectDropdown)
            {
                EditorGUI.LabelField(_rect, "Generate Type");
                _rect.x += 100;
                _rect.width = rect.width - 100;
                advSettingEntry.setting.usedType = (CVRAdvancesAvatarSettingBase.ParameterType)EditorGUI.IntPopup(
                    _rect,
                    (int)advSettingEntry.setting.usedType,
                    new[] { "Generate Float", "Generate Int" },
                    new[] { 1, 2 });

                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);
            }

            switch (advSettingEntry.type)
            {
                case CVRAdvancedSettingsEntry.SettingsType.GameObjectToggle:

                    _parameters.Add(advSettingEntry.machineName);

                    CVRAdvancesAvatarSettingGameObjectToggle gameObjectToggle = (CVRAdvancesAvatarSettingGameObjectToggle)advSettingEntry.setting;

                    // Default State
                    EditorGUI.LabelField(_rect, "Default");
                    _rect.x += 100;
                    _rect.width = rect.width - 100;
                    gameObjectToggle.defaultValue = EditorGUI.Toggle(_rect, gameObjectToggle.defaultValue);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

                    // Use Animation Clip
                    EditorGUI.LabelField(_rect, "Use Animation");
                    _rect.x += 100;
                    _rect.width = rect.width - 100;
                    gameObjectToggle.useAnimationClip = EditorGUI.Toggle(_rect, gameObjectToggle.useAnimationClip);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    if (gameObjectToggle.useAnimationClip)
                    {
                        _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

                        // Animation Clip Slot
                        EditorGUI.LabelField(_rect, "Clip");
                        _rect.x += 100;
                        _rect.width = rect.width - 100;
                        gameObjectToggle.animationClip = (AnimationClip)EditorGUI.ObjectField(_rect,
                            gameObjectToggle.animationClip, typeof(AnimationClip), false);

                        rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                        _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

                        // Animation Clip Slot
                        EditorGUI.LabelField(_rect, "Off Clip");
                        _rect.x += 100;
                        _rect.width = rect.width - 100;
                        gameObjectToggle.offAnimationClip = (AnimationClip)EditorGUI.ObjectField(_rect,
                            gameObjectToggle.offAnimationClip, typeof(AnimationClip), false);
                    }
                    else
                    {
                        ReorderableList gameObjectList = gameObjectToggle.GetReorderableList(_avatar);
                        gameObjectList.DoList(new Rect(rect.x, rect.y, rect.width, 20f));
                    }

                    break;
                case CVRAdvancedSettingsEntry.SettingsType.GameObjectDropdown:

                    _parameters.Add(advSettingEntry.machineName);

                    CVRAdvancesAvatarSettingGameObjectDropdown gameObjectDropdown = (CVRAdvancesAvatarSettingGameObjectDropdown)advSettingEntry.setting;

                    EditorGUI.LabelField(_rect, "Default");
                    _rect.x += 100;
                    _rect.width = rect.width - 100;
                    gameObjectDropdown.defaultValue = EditorGUI.Popup(_rect, gameObjectDropdown.defaultValue,
                        gameObjectDropdown.getOptionsList());

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                    ReorderableList options = gameObjectDropdown.GetReorderableList(_avatar);
                    options.DoList(new Rect(rect.x, rect.y, rect.width, 20f));
                    break;
                case CVRAdvancedSettingsEntry.SettingsType.MaterialColor:

                    _parameters.Add(advSettingEntry.machineName + "-r");
                    _parameters.Add(advSettingEntry.machineName + "-g");
                    _parameters.Add(advSettingEntry.machineName + "-b");

                    CVRAdvancedAvatarSettingMaterialColor materialColor = (CVRAdvancedAvatarSettingMaterialColor)advSettingEntry.setting;

                    EditorGUI.LabelField(_rect, "Default");
                    _rect.x += 100;
                    _rect.width = rect.width - 100;
                    materialColor.defaultValue = EditorGUI.ColorField(_rect, new GUIContent(), materialColor.defaultValue,
                        true, false, false);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                    ReorderableList materialColorList = materialColor.GetReorderableList(_avatar);
                    materialColorList.DoList(new Rect(rect.x, rect.y, rect.width, 20f));
                    break;
                case CVRAdvancedSettingsEntry.SettingsType.Slider:

                    _parameters.Add(advSettingEntry.machineName);

                    CVRAdvancesAvatarSettingSlider slider = (CVRAdvancesAvatarSettingSlider)advSettingEntry.setting;

                    EditorGUI.LabelField(_rect, "Default");
                    _rect.x += 100;
                    _rect.width = rect.width - 100;
                    slider.defaultValue = EditorGUI.Slider(_rect, slider.defaultValue, 0f, 1f);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

                    // Use Animation Clip
                    EditorGUI.LabelField(_rect, "Use Animation");
                    _rect.x += 100;
                    _rect.width = rect.width - 100;
                    slider.useAnimationClip = EditorGUI.Toggle(_rect, slider.useAnimationClip);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    if (slider.useAnimationClip)
                    {
                        _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

                        //Min Animation Clip Slot
                        EditorGUI.LabelField(_rect, "Min Clip");
                        _rect.x += 100;
                        _rect.width = rect.width - 100;
                        slider.minAnimationClip = (AnimationClip)EditorGUI.ObjectField(_rect, slider.minAnimationClip,
                            typeof(AnimationClip), false);

                        rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                        _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

                        //Max Animation Clip Slot
                        EditorGUI.LabelField(_rect, "Max Clip");
                        _rect.x += 100;
                        _rect.width = rect.width - 100;
                        slider.maxAnimationClip = (AnimationClip)EditorGUI.ObjectField(_rect, slider.maxAnimationClip,
                            typeof(AnimationClip), false);
                    }
                    else
                    {
                        ReorderableList materialPropertyList = slider.GetReorderableList(_avatar);
                        materialPropertyList.DoList(new Rect(rect.x, rect.y, rect.width, 20f));
                    }

                    foreach (CVRAdvancedSettingsTargetEntryMaterialProperty materialProperty in slider.materialPropertyTargets)
                    {
                        rect.y += EditorGUIUtility.singleLineHeight * 1.25f * (!materialProperty.isCollapsed ? 1 : 5);
                    }

                    rect.y += EditorGUIUtility.singleLineHeight *
                              (3f + (slider.materialPropertyTargets.Count == 0 ? 1.25f : 0));
                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);

                    if (!slider.useAnimationClip)
                    {
                        EditorGUI.HelpBox(_rect,
                            "The Setup Utility will help you create a slider for Material properties " +
                            "If you want to bind other properties you can edit the animation files generated " +
                            "by the System after the animator was created.", MessageType.Info);
                    }

                    break;
                case CVRAdvancedSettingsEntry.SettingsType.Joystick2D:

                    _parameters.Add(advSettingEntry.machineName + "-x");
                    _parameters.Add(advSettingEntry.machineName + "-y");

                    CVRAdvancesAvatarSettingJoystick2D joystick = (CVRAdvancesAvatarSettingJoystick2D)advSettingEntry.setting;

                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);
                    joystick.defaultValue = EditorGUI.Vector2Field(_rect, "Default", joystick.defaultValue);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);
                    joystick.rangeMin = EditorGUI.Vector2Field(_rect, "Range Min", joystick.rangeMin);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);
                    joystick.rangeMax = EditorGUI.Vector2Field(_rect, "Range Max", joystick.rangeMax);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);

                    EditorGUI.HelpBox(_rect, "This Settings does not provide a Setup Utility. " +
                                             "But it will create the necessary Animator Layers, Parameters and Animations. " +
                                             "So you can edit them to your liking after the animator was created.",
                        MessageType.Info);
                    break;
                case CVRAdvancedSettingsEntry.SettingsType.Joystick3D:

                    _parameters.Add(advSettingEntry.machineName + "-x");
                    _parameters.Add(advSettingEntry.machineName + "-y");
                    _parameters.Add(advSettingEntry.machineName + "-z");

                    CVRAdvancesAvatarSettingJoystick3D joystick3D = (CVRAdvancesAvatarSettingJoystick3D)advSettingEntry.setting;

                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);
                    joystick3D.defaultValue = EditorGUI.Vector3Field(_rect, "Default", joystick3D.defaultValue);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);
                    joystick3D.rangeMin = EditorGUI.Vector3Field(_rect, "Range Min", joystick3D.rangeMin);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);
                    joystick3D.rangeMax = EditorGUI.Vector3Field(_rect, "Range Max", joystick3D.rangeMax);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);

                    EditorGUI.HelpBox(_rect, "This Settings does not provide a Setup Utility. " +
                                             "But it will create the necessary Animator Layers, Parameters and Animations. " +
                                             "So you can edit them to your liking after the animator was created.",
                        MessageType.Info);
                    break;
                case CVRAdvancedSettingsEntry.SettingsType.InputSingle:

                    _parameters.Add(advSettingEntry.machineName);

                    CVRAdvancesAvatarSettingInputSingle inputSingle = (CVRAdvancesAvatarSettingInputSingle)advSettingEntry.setting;

                    EditorGUI.LabelField(_rect, "Default");
                    _rect.x += 100;
                    _rect.width = rect.width - 100;
                    inputSingle.defaultValue = EditorGUI.FloatField(_rect, inputSingle.defaultValue);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);

                    EditorGUI.HelpBox(_rect, "This Settings does not provide a Setup Utility. " +
                                             "But it will create the necessary Animator Layers, Parameters and Animations. " +
                                             "So you can edit them to your liking after the animator was created.",
                        MessageType.Info);
                    break;
                case CVRAdvancedSettingsEntry.SettingsType.InputVector2:

                    _parameters.Add(advSettingEntry.machineName + "-x");
                    _parameters.Add(advSettingEntry.machineName + "-y");

                    CVRAdvancesAvatarSettingInputVector2 inputVector2 = (CVRAdvancesAvatarSettingInputVector2)advSettingEntry.setting;

                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);
                    inputVector2.defaultValue = EditorGUI.Vector2Field(_rect, "Default", inputVector2.defaultValue);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);

                    EditorGUI.HelpBox(_rect, "This Settings does not provide a Setup Utility. " +
                                             "But it will create the necessary Animator Layers, Parameters and Animations. " +
                                             "So you can edit them to your liking after the animator was created.",
                        MessageType.Info);
                    break;
                case CVRAdvancedSettingsEntry.SettingsType.InputVector3:

                    _parameters.Add(advSettingEntry.machineName + "-x");
                    _parameters.Add(advSettingEntry.machineName + "-y");
                    _parameters.Add(advSettingEntry.machineName + "-z");

                    CVRAdvancesAvatarSettingInputVector3 inputVector3 = (CVRAdvancesAvatarSettingInputVector3)advSettingEntry.setting;

                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);
                    inputVector3.defaultValue = EditorGUI.Vector3Field(_rect, "Default", inputVector3.defaultValue);

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);

                    EditorGUI.HelpBox(_rect, "This Settings does not provide a Setup Utility. " +
                                             "But it will create the necessary Animator Layers, Parameters and Animations. " +
                                             "So you can edit them to your liking after the animator was created.",
                        MessageType.Info);
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void CreateAnimator()
        {
            if (_avatar.avatarSettings.baseController == null)
            {
                EditorUtility.DisplayDialog("Animator Error",
                    "The Base Animator was not selected. No new Animator Controller was created.", "OK");
                return;
            }

            if (_avatar.avatarSettings.animator != null)
            {
                if (!EditorUtility.DisplayDialog("Animator already created",
                        "There is Animator already created for this avatar.", "Override", "Cancel"))
                {
                    return;
                }
            }

            string pathToCurrentFolder = "Assets/AdvancedSettings.Generated";
            if (!AssetDatabase.IsValidFolder(pathToCurrentFolder))
                AssetDatabase.CreateFolder("Assets", "AdvancedSettings.Generated");

            string folderPath = pathToCurrentFolder + "/" + _avatar.name + "_AAS";
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder(pathToCurrentFolder, _avatar.name + "_AAS");
            string animatorPath = pathToCurrentFolder + "/" + _avatar.name + "_AAS/" + _avatar.name + "_aas.controller";
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(_avatar.avatarSettings.baseController.GetInstanceID()),
                animatorPath);

            _avatar.avatarSettings.animator = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorPath);

            _parameters.Clear();

            if (_avatar.avatarSettings.baseController != null)
            {
                AnimatorController animator = (AnimatorController)_avatar.avatarSettings.baseController;

                foreach (AnimatorControllerParameter parameter in animator.parameters)
                {
                    if (parameter.type == AnimatorControllerParameterType.Float && parameter.name.Length > 0 &&
                        !CVRCommon.CoreParameters.Contains(parameter.name) && parameter.name.Substring(0, 1) != "#")
                    {
                        _parameters.Add(parameter.name);
                    }

                    if (parameter.type == AnimatorControllerParameterType.Int && parameter.name.Length > 0 &&
                        !CVRCommon.CoreParameters.Contains(parameter.name) && parameter.name.Substring(0, 1) != "#")
                    {
                        _parameters.Add(parameter.name);
                    }

                    if (parameter.type == AnimatorControllerParameterType.Bool && parameter.name.Length > 0 &&
                        !CVRCommon.CoreParameters.Contains(parameter.name) && parameter.name.Substring(0, 1) != "#")
                    {
                        _parameters.Add(parameter.name);
                    }
                }
            }

            foreach (CVRAdvancedSettingsEntry entry in _avatar.avatarSettings.settings)
            {
                switch (entry.type)
                {
                    default:
                        if (_parameters.Contains(entry.machineName)) continue;
                        break;
                    case CVRAdvancedSettingsEntry.SettingsType.MaterialColor:
                        if (_parameters.Contains(entry.machineName + "-r") ||
                            _parameters.Contains(entry.machineName + "-g") ||
                            _parameters.Contains(entry.machineName + "-b")) continue;
                        break;
                    case CVRAdvancedSettingsEntry.SettingsType.Joystick2D:
                    case CVRAdvancedSettingsEntry.SettingsType.InputVector2:
                        if (_parameters.Contains(entry.machineName + "-x") ||
                            _parameters.Contains(entry.machineName + "-y")) continue;
                        break;
                    case CVRAdvancedSettingsEntry.SettingsType.Joystick3D:
                    case CVRAdvancedSettingsEntry.SettingsType.InputVector3:
                        if (_parameters.Contains(entry.machineName + "-x") ||
                            _parameters.Contains(entry.machineName + "-y") ||
                            _parameters.Contains(entry.machineName + "-z")) continue;
                        break;
                }

                entry.setting.SetupAnimator(ref _avatar.avatarSettings.animator, entry.machineName, folderPath);
            }

            if (_avatar.avatarSettings.baseOverrideController != null)
            {
                string overridePath = pathToCurrentFolder + "/" + _avatar.name + "_AAS/" + _avatar.name +
                                      "_aas_overrides.overrideController";
                AssetDatabase.CopyAsset(
                    AssetDatabase.GetAssetPath(_avatar.avatarSettings.baseOverrideController.GetInstanceID()),
                    overridePath);
                _avatar.avatarSettings.overrides = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(overridePath);
                _avatar.avatarSettings.overrides.runtimeAnimatorController = _avatar.avatarSettings.animator;
            }
            else
            {
                _avatar.avatarSettings.overrides = new AnimatorOverrideController(_avatar.avatarSettings.animator);
                AssetDatabase.CreateAsset(_avatar.avatarSettings.overrides,
                    pathToCurrentFolder + "/" + _avatar.name + "_AAS/" + _avatar.name +
                    "_aas_overrides.overrideController");
            }

            AssetDatabase.SaveAssets();
        }

        private static void CreateAvatarSettings(CVRAvatar avatar)
        {
            string[] guids = AssetDatabase.FindAssets("AvatarAnimator t:animatorController", null);

            if (guids.Length < 1)
            {
                Debug.LogError(
                    "No Animator controller with the name \"AvatarAnimator\" was found. Please make sure that you CCK is installed properly.");
                return;
            }

            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath =
                projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            if (getActiveFolderPath != null) getActiveFolderPath.Invoke(null, Array.Empty<object>());

            avatar.avatarSettings = new CVRAdvancedAvatarSettings
            {
                baseController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(guids[0])),
                settings = new List<CVRAdvancedSettingsEntry>(),
                initialized = true
            };
        }

        #endregion

        #region Utility

        private void AppendComponentMenu(GenericMenuBuilder genericMenuBuilder, ReorderableList list)
        {
            bool hasSettings = _avatar != null &&
                              _avatar.avatarSettings != null &&
                              _avatar.avatarSettings.settings != null;

            genericMenuBuilder.AddMenuItem("Collapse All", hasSettings,
                () => CollapseAllEntries(_avatar.avatarSettings.settings));
        }

        private static void CollapseAllEntries(List<CVRAdvancedSettingsEntry> settingsEntries)
        {
            foreach (CVRAdvancedSettingsEntry advSettingEntry in settingsEntries)
                advSettingEntry.setting.isCollapsed = false;
        }

        #endregion

    }
}
#endif