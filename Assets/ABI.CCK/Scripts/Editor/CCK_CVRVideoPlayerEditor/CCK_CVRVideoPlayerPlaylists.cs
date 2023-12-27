#if UNITY_EDITOR
using ABI.CCK.Scripts.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static ABI.CCK.Scripts.Editor.SharedComponentGUI;

namespace ABI.CCK.Components
{
    public partial class CCK_CVRVideoPlayerEditor
    {
        private const string TypeLabel = "Playlists";
        private ReorderableList _playlistList;
        
        private void Draw_Playlists()
        {
            using (new FoldoutScope(ref _guiPlaylistsFoldout, "Playlists"))
            {
                if (!_guiPlaylistsFoldout) return;
                DrawPlaylists();
            }
        }

        #region Drawing Methods
        
        private void DrawPlaylists()
        {
            if (_playlistList == null)
            {
                _playlistList = new ReorderableList(_player.entities, typeof(CVRVideoPlayerPlaylist), true, true, true, true)
                {
                    drawHeaderCallback = OnDrawHeader,
                    drawElementCallback = OnDrawElement,
                    elementHeightCallback = OnHeightElement,
                    onAddCallback = OnAdd,
                    onChangedCallback = OnChanged
                };
            }
            
            GUILayout.BeginVertical();
            
            DrawDefaultVideoSelection();
            EditorGUILayout.Space();
            
            _playlistList.DoLayoutList();
            EditorGUILayout.Space();
        
            GUILayout.EndVertical();
        }

        private void DrawDefaultVideoSelection()
        {
            EditorGUILayout.LabelField(new GUIContent("Play On Awake Object", "Default video to play on start/awake"), new GUIContent(_player.playOnAwakeObject?.videoTitle));
            if (GUILayout.Button("Remove Play on Awake Object")) _player.playOnAwakeObject = null;
        }
        
        #endregion

        #region Playlist ReorderableList

        private float OnHeightElement(int index)
        {
            var height = 0f;

            if (!_player.entities[index].isCollapsed) return EditorGUIUtility.singleLineHeight * 1.25f;

            height += EditorGUIUtility.singleLineHeight * (3f + 2.5f);

            if (_player.entities[index].playlistVideos.Count == 0) height += 1.25f * EditorGUIUtility.singleLineHeight;

            foreach (var entry in _player.entities[index].playlistVideos)
            {
                if (entry == null)
                {
                    height += 1.25f * EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    height += (entry.isCollapsed ? 7.5f : 1.25f) * EditorGUIUtility.singleLineHeight;
                }
            }

            return height;
        }

        private void OnDrawHeader(Rect rect)
        {
            Rect rectA = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(rectA, TypeLabel);
            EditorGUIExtensions.UtilityMenu(rectA, _playlistList);
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index > _player.entities.Count) return;

            rect.y += 2;
            rect.x += 12;
            rect.width -= 12;
            Rect rectA = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();

            bool collapse = EditorGUI.Foldout(rectA, _player.entities[index].isCollapsed, "Playlist Title", true);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Playlist Expand");
                _player.entities[index].isCollapsed = collapse;
            }

            rectA.x += 80;
            rectA.width = rect.width - 80;

            EditorGUI.BeginChangeCheck();

            string playlistTitle = EditorGUI.TextField(rectA, _player.entities[index].playlistTitle);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Playlist Title");
                _player.entities[index].playlistTitle = playlistTitle;
            }

            if (!_player.entities[index].isCollapsed) return;

            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            rectA = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(rectA, "Thumbnail Url");
            rectA.x += 80;
            rectA.width = rect.width - 80;

            EditorGUI.BeginChangeCheck();

            string playlistThumbnailUrl = EditorGUI.TextField(rectA, _player.entities[index].playlistThumbnailUrl);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Playlist Thumbnail Url");
                _player.entities[index].playlistThumbnailUrl = playlistThumbnailUrl;
            }

            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            //_rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            var videoList = _player.entities[index].GetReorderableList(_player);
            videoList.DoList(new Rect(rect.x, rect.y, rect.width, 20f));
        }

        private void OnAdd(ReorderableList list)
        {
            Undo.RecordObject(target, "Add Playlist Entry");
            _player.entities.Add(null);
        }

        private void OnChanged(ReorderableList list)
        {
            Undo.RecordObject(target, "Playlist List changed");
        }

        #endregion
    }
}
#endif