#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ABI.CCK.Scripts.Editor
{
    public class GenericMenuBuilder
    {
        private readonly GenericMenu _menu = new GenericMenu();

        public void AddMenuItem(string itemName, bool condition, GenericMenu.MenuFunction callback)
        {
            if (condition)
                _menu.AddItem(new GUIContent(itemName), false, callback);
            else
                _menu.AddDisabledItem(new GUIContent(itemName));
        }
        
        public void AddSeparator(string path = "")
        {
            _menu.AddSeparator(path);
        }

        public void DropDown(Rect position)
        {
            _menu.DropDown(position);
        }
    }
}
#endif