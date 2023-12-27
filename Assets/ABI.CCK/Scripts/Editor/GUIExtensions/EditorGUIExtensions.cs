#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ABI.CCK.Scripts.Editor.Tools;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace ABI.CCK.Scripts.Editor
{
    public static partial class EditorGUIExtensions
    {
        private const float DEFAULT_DROPDOWN_BUTTON_WIDTH = 35;
        private static readonly Dictionary<string, GUIContent> s_iconContentCache = new Dictionary<string, GUIContent>();
        private static string s_lastSelectedValue = "";

        #region Specialized EditorGUI
        
        public delegate void AppendAdditionalMenuItemsDelegate(GenericMenuBuilder genericMenuBuilder, ReorderableList list);
        public static void UtilityMenu(Rect rect, ReorderableList list, AppendAdditionalMenuItemsDelegate appendAdditionalMenuItems = null)
        {
            Rect dropdownButtonRect = new Rect(rect.x + rect.width - 21, rect.y, 20, EditorGUIUtility.singleLineHeight);
            if (EditorGUI.DropdownButton(dropdownButtonRect, GUIContent.none, FocusType.Passive))
            {
                GenericMenuBuilder genericMenuBuilder = new GenericMenuBuilder();
                ClipboardUtils.AppendToMenu(genericMenuBuilder, list); // default for all uses atm
                if (appendAdditionalMenuItems != null)
                {
                    genericMenuBuilder.AddSeparator();
                    appendAdditionalMenuItems.Invoke(genericMenuBuilder, list);
                }
                genericMenuBuilder.DropDown(dropdownButtonRect);
            }
        }
        
        public static string AdvancedDropdownInput(
            Rect rect, 
            string currentValue, 
            List<string> dropdownItems, 
            string label, 
            string defaultTitle,
            GUIContent buttonContent = null)
        {
            if (buttonContent == null)
            {
                buttonContent = dropdownItems.Contains(currentValue)
                    ? GetCachedIconContent("d_Search Icon")
                    : GetCachedIconContent("console.erroricon.sml");
            }

            string title = (dropdownItems.Count == 0 ? defaultTitle : currentValue);
            return CustomDropdown(rect, label, currentValue, dropdownItems.ToArray(), buttonContent, DEFAULT_DROPDOWN_BUTTON_WIDTH, title);
        }
        
        public static string AdvancedDropdownInput(
            Rect rect, 
            string currentValue, 
            List<string> dropdownItems, 
            string title,
            GUIContent buttonContent = null)
        {
            if (buttonContent == null)
            {
                buttonContent = dropdownItems.Contains(currentValue)
                    ? GetCachedIconContent("d_Search Icon")
                    : GetCachedIconContent("console.erroricon.sml");
            }

            return CustomDropdown(rect, currentValue, dropdownItems.ToArray(), buttonContent, DEFAULT_DROPDOWN_BUTTON_WIDTH, title);
        }
        
        public static string AdvancedDropdownInput(
            Rect rect, 
            string currentValue, 
            string[] dropdownItems, 
            string title,
            GUIContent buttonContent = null)
        {
            if (buttonContent == null)
            {
                buttonContent = dropdownItems.Contains(currentValue)
                    ? GetCachedIconContent("d_Search Icon")
                    : GetCachedIconContent("console.erroricon.sml");
            }

            return CustomDropdown(rect, currentValue, dropdownItems, buttonContent, DEFAULT_DROPDOWN_BUTTON_WIDTH, title);
        }

        // bit lazy ig
        public static void LimitSliderSided(string label, ref Vector2 limits)
        {
            var originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 150;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(originalLabelWidth - 16));
            limits.x = EditorGUILayout.FloatField(limits.x, GUILayout.Width(50));
            GUILayout.Space(-13);
            EditorGUILayout.MinMaxSlider(ref limits.x, ref limits.y, -180f, 180f);
            GUILayout.Space(-13);
            limits.y = EditorGUILayout.FloatField(limits.y, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = originalLabelWidth;
        }
        
        #endregion
        
        #region CustomPopup

        public static string CustomPopup(Rect rect, string label, string currentItem, string[] items, string title = null)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            // Use label width to position the dropdown appropriately
            float labelWidth = EditorGUIUtility.labelWidth;

            Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
            Rect buttonRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);

            EditorGUI.LabelField(labelRect, label);

            if (EditorGUI.DropdownButton(buttonRect, new GUIContent(currentItem), FocusType.Passive))
            {
                GUIUtility.keyboardControl = controlId;
                new CustomAdvancedDropdown(new AdvancedDropdownState(), items, title, selected =>
                {
                    s_lastSelectedValue = selected;
                }).Show(buttonRect); // We'll show the dropdown right where our buttonRect is
            }

            if (controlId == GUIUtility.keyboardControl && !string.IsNullOrEmpty(s_lastSelectedValue))
            {
                currentItem = s_lastSelectedValue;
                s_lastSelectedValue = "";
            }

            return currentItem;
        }

        #endregion

        #region CustomDropdown

        public static string CustomDropdown(
            Rect rect, 
            string label, 
            string currentItem, 
            string[] items, 
            string title = null)
        {
            GUIContent defaultButtonContent = items.Contains(currentItem)
                ? GetCachedIconContent("d_Search Icon")
                : GetCachedIconContent("console.erroricon.sml");
            
            return CustomDropdown(rect, label, currentItem, items, defaultButtonContent, DEFAULT_DROPDOWN_BUTTON_WIDTH, title);
        }

        public static string CustomDropdown(
            Rect rect, 
            string label, 
            string currentItem, 
            string[] items, 
            GUIContent buttonContent, 
            string title = null)
        {
            return CustomDropdown(rect, label, currentItem, items, buttonContent, DEFAULT_DROPDOWN_BUTTON_WIDTH, title);
        }

        private static string CustomDropdown(
            Rect rect, 
            string label, 
            string currentItem, 
            string[] items, 
            GUIContent buttonContent, 
            float dropdownButtonWidth, 
            string title)
        {
            float labelWidth = 2f + EditorGUIUtility.labelWidth;
            float indentOffset = 15 * EditorGUI.indentLevel;

            Rect textFieldRect = new Rect(rect.x + labelWidth - indentOffset, rect.y, rect.width - labelWidth + indentOffset - dropdownButtonWidth, rect.height);
            Rect buttonRect = new Rect(rect.x + rect.width - dropdownButtonWidth, rect.y, dropdownButtonWidth, rect.height);
            Rect dropdownRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);

            EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, rect.height), label);
            currentItem = EditorGUI.TextField(textFieldRect, currentItem);

            return CustomDropdown(currentItem, items, title, buttonRect, dropdownRect, buttonContent);
        }
        
        private static string CustomDropdown(
            Rect rect, 
            string currentItem, 
            string[] items, 
            GUIContent buttonContent, 
            float dropdownButtonWidth, 
            string title)
        {
            float indentOffset = 15 * EditorGUI.indentLevel;
            Rect textFieldRect = new Rect(rect.x - indentOffset, rect.y, rect.width + indentOffset - dropdownButtonWidth, rect.height);
            Rect buttonRect = new Rect(rect.x + rect.width - dropdownButtonWidth, rect.y, dropdownButtonWidth, rect.height);
            Rect dropdownRect = new Rect(rect.x, rect.y, rect.width, rect.height);

            currentItem = EditorGUI.TextField(textFieldRect, currentItem);
            return CustomDropdown(currentItem, items, title, buttonRect, dropdownRect, buttonContent);
        }
        
        private static string CustomDropdown(
            string currentItem, 
            string[] items, 
            string title, 
            Rect buttonRect, 
            Rect dropdownRect, 
            GUIContent buttonContent)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            if (EditorGUI.DropdownButton(buttonRect, buttonContent, FocusType.Passive))
            {
                GUIUtility.keyboardControl = controlId;
                new CustomAdvancedDropdown(new AdvancedDropdownState(), items, title, selected =>
                {
                    s_lastSelectedValue = selected;
                }).Show(dropdownRect);
            }

            if (controlId == GUIUtility.keyboardControl && !string.IsNullOrEmpty(s_lastSelectedValue))
            {
                currentItem = s_lastSelectedValue;
                s_lastSelectedValue = "";
            }

            return currentItem;
        }

        #endregion

        #region Private Methods

        public static GUIContent GetCachedIconContent(string iconName)
        {
            if (s_iconContentCache.TryGetValue(iconName, out GUIContent iconContent)) 
                return iconContent;
            
            iconContent = EditorGUIUtility.IconContent(iconName);
            s_iconContentCache[iconName] = iconContent;
            return iconContent;
        }

        #endregion

        #region Custom GUI

        private class CustomAdvancedDropdown : AdvancedDropdown
        {
            private readonly Action<string> _onItemSelected;
            private readonly string[] _items;
            private readonly string _title;
    
            internal CustomAdvancedDropdown(AdvancedDropdownState state, string[] items, string title, Action<string> onItemSelected) : base(state)
            {
                _items = items ?? Array.Empty<string>();
                _title = title ?? "Items";
                _onItemSelected = onItemSelected;
            }
    
            internal new void Show(Rect rect)
            {
                minimumSize = new Vector2(rect.width, 200f);
                
                // We have to set the maximumSize through reflection, cause unity is cool
                typeof(AdvancedDropdown).GetProperty("maximumSize", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.SetValue(this, new Vector2(rect.width, 400f));

                base.Show(rect);
            }
    
            protected override AdvancedDropdownItem BuildRoot()
            {
                AdvancedDropdownItem root = new AdvancedDropdownItem(_title);
    
                foreach (var item in _items)
                {
                    AdvancedDropdownItem current = root;
                    string[] parts = item.Split('/');
    
                    for (int i = 0; i < parts.Length; i++)
                    {
                        string name = (i == parts.Length - 1) ? item : parts[i];
                        AdvancedDropdownItem child = current.children.FirstOrDefault(c => c.name == name);
    
                        if (child == null)
                        {
                            child = new AdvancedDropdownItem(name);
                            if (name == item && name == _title)
                                child.icon = GetCachedIconContent("d_FilterSelectedOnly").image as Texture2D;
    
                            current.AddChild(child);
                        }
                
                        current = child;
                    }
                }
    
                return root;
            }
    
            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                base.ItemSelected(item);
                _onItemSelected?.Invoke(item.name);
            }
        }

        #endregion
    }
}
#endif