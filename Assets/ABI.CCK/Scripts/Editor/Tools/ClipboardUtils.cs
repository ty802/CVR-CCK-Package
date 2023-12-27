using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

namespace ABI.CCK.Scripts.Editor.Tools
{
    public static class ClipboardUtils
    {
        private static readonly List<object> s_clipboard = new List<object>();
        
        #region Copy Methods

        public static void CopySelected<T>(List<T> source, int selectedIndex)
        {
            if (selectedIndex < 0 || selectedIndex >= source.Count)
                return;

            s_clipboard.Clear();
            s_clipboard.Add(source[selectedIndex]);
        }

        public static void CopyAll<T>(List<T> source)
        {
            s_clipboard.Clear();
            foreach (T item in source)
                s_clipboard.Add(item);
        }

        #endregion

        #region Cut Methods

        public static void CutSelected<T>(List<T> source, int selectedIndex)
        {
            if (selectedIndex < 0 || selectedIndex >= source.Count)
                return;

            s_clipboard.Clear();
            s_clipboard.Add(source[selectedIndex]);
            source.RemoveAt(selectedIndex);
        }

        public static void CutAll<T>(List<T> source)
        {
            s_clipboard.Clear();
            foreach (T item in source)
                s_clipboard.Add(item);
            
            source.Clear();
        }

        #endregion

        #region Paste Methods

        public static void Paste<T>(List<T> target)
        {
            target.AddRange(s_clipboard.Cast<T>());
        }
        
        public static bool IsValidPaste<T>(List<T> target)
        {
            return s_clipboard.Count != 0 && s_clipboard.All(item => item is T);
        }


        #endregion

        #region Reset Methods

        public static void ResetSelected<T>(List<T> source, int selectedIndex) where T : new()
        {
            if (selectedIndex < 0 || selectedIndex >= source.Count)
                return;

            if (typeof(T).IsValueType || typeof(T).GetConstructor(Type.EmptyTypes) != null)
                source[selectedIndex] = new T();
            else
                source[selectedIndex] = default(T);
        }

        public static void ResetAll<T>(List<T> source) where T : new()
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (typeof(T).IsValueType || typeof(T).GetConstructor(Type.EmptyTypes) != null)
                    source[i] = new T();
                else
                    source[i] = default(T);
            }
        }

        #endregion

        #region Delete Methods

        public static void DeleteSelected<T>(List<T> source, int selectedIndex)
        {
            if (selectedIndex < 0 || selectedIndex >= source.Count)
                return;

            source.RemoveAt(selectedIndex);
        }

        public static void DeleteAll<T>(List<T> source)
        {
            source.Clear();
        }

        #endregion

        #region Menu Builder

        public static void AppendToMenu(GenericMenuBuilder genericMenuBuilder, ReorderableList list)
            => AppendToMenu(genericMenuBuilder, list.list, list.index);
        
        public static void AppendToMenu(GenericMenuBuilder genericMenuBuilder, IList list, int index = -1)
        {
            Type itemType = GetItemTypeOfList(list);
            if (itemType == null) return;
            
            bool hasSelectedItem = index != -1;
            bool hasItems = list.Count > 0;

            genericMenuBuilder.AddMenuItem("Copy/Selected", hasSelectedItem, () => InvokeClipboardMethod("CopySelected", itemType, list, index));
            genericMenuBuilder.AddMenuItem("Copy/All", hasItems, () => InvokeClipboardMethod("CopyAll", itemType, list));
        
            //menuBuilder.AddMenuItem("Cut/Selected", hasSelectedItem, () => InvokeClipboardMethod("CutSelected", itemType, list, index));
            //menuBuilder.AddMenuItem("Cut/All", hasTasks, () => InvokeClipboardMethod("CutAll", itemType, list));
        
            genericMenuBuilder.AddMenuItem("Paste",  (bool)InvokeClipboardMethod("IsValidPaste", itemType, list), () => InvokeClipboardMethod("Paste", itemType, list));
        
            //menuBuilder.AddMenuItem("Reset/Selected", hasSelectedItem, () => InvokeClipboardMethod("ResetSelected", itemType, list, index));
            //menuBuilder.AddMenuItem("Reset/All", hasTasks, () => InvokeClipboardMethod("ResetAll", itemType, list));
        
            genericMenuBuilder.AddMenuItem("Delete/Selected", hasSelectedItem, () => InvokeClipboardMethod("DeleteSelected", itemType, list, index));
            genericMenuBuilder.AddMenuItem("Delete/All", hasItems, () => InvokeClipboardMethod("DeleteAll", itemType, list));
        }

        private static Type GetItemTypeOfList(IList list)
        {
            if (list == null) return null;
            Type listType = list.GetType();
            if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
                return listType.GetGenericArguments()[0];
            return null;
        }
        
        private static object InvokeClipboardMethod(string methodName, Type itemType, IList list, int? selectedIndex = null)
        {
            MethodInfo method = typeof(ClipboardUtils).GetMethod(methodName)?.MakeGenericMethod(itemType);
            return method?.Invoke(null, selectedIndex.HasValue ? new object[] { list, selectedIndex.Value } : new object[] { list });
        }

        #endregion
    }
}