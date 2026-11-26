#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using TreeView = UnityEditor.IMGUI.Controls.TreeView<int>;
using TreeViewItem = UnityEditor.IMGUI.Controls.TreeViewItem<int>;
using TreeViewState = UnityEditor.IMGUI.Controls.TreeViewState<int>;
using ElementTreeView = UnityEngine.UIElements.TreeView;
using PopupWindow = UnityEditor.PopupWindow;
using Object = UnityEngine.Object;

namespace Jeomseon.Unity.Attributes.Editor
{
    internal sealed class ComponentDropdown : TreeView
    {
        private sealed class ComponentDropdownPopupContent : PopupWindowContent
        {
            private readonly ComponentDropdown _dropdown;
            private readonly SearchField _searchField;
            private GUIStyle _labelStyle;

            // ✅ horizontal-only scroll state
            private float _hScroll;
            private bool _usesUIToolkit;

            public ComponentDropdownPopupContent(ComponentDropdown dropdown)
            {
                _dropdown = dropdown;
                _searchField = new SearchField();
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(300f, Mathf.Clamp(_dropdown.ItemCount * _dropdown.rowHeight, 200, 800));
            }

            public override VisualElement CreateGUI()
            {
                _usesUIToolkit = true;
                float lineHeight = EditorGUIUtility.singleLineHeight;

                VisualElement root = new();
                root.style.flexGrow = 1f;
                root.style.paddingTop = 3f;

                ToolbarSearchField searchField = new();
                searchField.style.width = Length.Percent(90f);
                searchField.style.alignSelf = Align.Center;
                searchField.style.flexShrink = 0f;
                searchField.SetValueWithoutNotify(_dropdown.searchString ?? string.Empty);
                root.Add(searchField);

                Label title = new("Nested Object");
                title.style.height = lineHeight;
                title.style.flexShrink = 0f;
                title.style.marginTop = lineHeight * 0.35f;
                title.style.unityFontStyleAndWeight = FontStyle.Bold;
                title.style.unityTextAlign = TextAnchor.MiddleCenter;
                root.Add(title);

                ElementTreeView treeView = new()
                {
                    fixedItemHeight = _dropdown.rowHeight,
                    horizontalScrollingEnabled = true,
                    selectionType = SelectionType.Single
                };
                treeView.style.flexGrow = 1f;
                treeView.style.marginTop = lineHeight * 0.15f;
                treeView.makeItem = () =>
                {
                    VisualElement row = new();
                    row.style.flexDirection = FlexDirection.Row;
                    row.style.alignItems = Align.Center;

                    Image icon = new() { scaleMode = ScaleMode.ScaleToFit };
                    icon.style.width = 16f;
                    icon.style.height = 16f;
                    icon.style.flexShrink = 0f;
                    icon.style.marginRight = 2f;
                    row.Add(icon);
                    row.Add(new Label());
                    return row;
                };
                treeView.bindItem = (element, index) =>
                {
                    TreeViewItem item = treeView.GetItemDataForIndex<TreeViewItem>(index);
                    Label label = element.Q<Label>();
                    label.text = item.displayName;
                    element.Q<Image>().image = _dropdown.GetIcon(item);

                    // 행을 재사용하므로 그룹 표시와 구분선을 매번 다시 지정합니다. 행 높이는 그대로 둡니다.
                    bool isGroup = _dropdown.IsGroup(item);
                    label.SetEnabled(!isGroup);
                    label.style.unityFontStyleAndWeight = isGroup
                        ? new StyleEnum<FontStyle>(FontStyle.Italic)
                        : new StyleEnum<FontStyle>(StyleKeyword.Null);

                    bool separated = _dropdown.HasSeparatorBefore(item);
                    element.style.borderTopWidth = separated ? new StyleFloat(1f) : new StyleFloat(StyleKeyword.Null);
                    element.style.borderTopColor = separated
                        ? new StyleColor(SeparatorColor)
                        : new StyleColor(StyleKeyword.Null);
                };
                // IMGUI TreeView의 DoubleClickedItem과 같은 선택 확정 시점입니다.
                treeView.itemsChosen += items =>
                {
                    foreach (object chosen in items)
                    {
                        if (chosen is not TreeViewItem item) continue;

                        _dropdown.Choose(item.id);
                        return;
                    }
                };
                root.Add(treeView);

                searchField.RegisterValueChangedCallback(change =>
                {
                    _dropdown.searchString = change.newValue;
                    Populate();
                });
                Populate();
                return root;

                void Populate()
                {
                    treeView.SetRootItems(_dropdown.CreateElementItems());
                    treeView.Rebuild();
                    treeView.ExpandAll();
                }
            }

            public override void OnGUI(Rect rect)
            {
                // UI Toolkit 내용이 만들어졌으면 IMGUI TreeView를 겹쳐 그리지 않습니다.
                if (_usesUIToolkit) return;

                _labelStyle ??= new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                };

                float voidWidth = rect.width * 0.1f;
                rect.y += 3.0f;

                Rect searchRect = new Rect(
                    rect.x + voidWidth * 0.5f,
                    rect.y,
                    rect.width - voidWidth,
                    EditorGUIUtility.singleLineHeight);

                _dropdown.searchString = _searchField.OnGUI(searchRect, _dropdown.searchString);

                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 1.35f, rect.width, EditorGUIUtility.singleLineHeight),
                    "Nested Object",
                    _labelStyle);

                Rect treeViewRect = new Rect(
                    rect.x,
                    rect.y + EditorGUIUtility.singleLineHeight * 2.5f,
                    rect.width,
                    rect.height - EditorGUIUtility.singleLineHeight * 2.5f);

                // ✅ horizontal-only outer scroll:
                // - Outer scroll handles ONLY X.
                // - TreeView keeps handling Y internally.
                float contentWidth = _dropdown.GetContentWidth(treeViewRect.width);

                // viewRect: visible area (same as treeViewRect)
                Rect viewRect = treeViewRect;

                // contentRect height == viewRect height -> prevents vertical scrollbar on outer scroll
                Rect contentRect = new Rect(0f, 0f, contentWidth, viewRect.height);

                // Horizontal scrollbar height reserved at bottom (optional but helps layout)
                // We'll use GUI.BeginGroup for positioning and a separate horizontal scrollbar control.
                // This avoids BeginScrollView creating a vertical scrollbar at all.
                drawHorizontalOnlyTree(viewRect, contentRect, contentWidth);
            }

            private void drawHorizontalOnlyTree(Rect viewRect, Rect contentRect, float contentWidth)
            {
                // We implement horizontal-only scrolling manually:
                // 1) Clip to viewRect
                // 2) Offset drawing by -_hScroll
                // 3) Draw a bottom horizontal scrollbar when needed

                const float scrollbarHeight = 16f;

                bool needH = contentWidth > viewRect.width + 0.5f;

                Rect treeArea = viewRect;
                Rect hbarRect = default;

                if (needH)
                {
                    // Reserve space for the horizontal scrollbar so it doesn't overlap rows
                    treeArea.height -= scrollbarHeight;
                    hbarRect = new Rect(viewRect.x, viewRect.yMax - scrollbarHeight, viewRect.width, scrollbarHeight);
                }

                // Clip the drawing region to the tree area
                GUI.BeginGroup(treeArea);
                {
                    // Shift content left/right
                    Rect innerRect = new Rect(-_hScroll, 0f, contentWidth, treeArea.height);

                    // Draw TreeView in the shifted coordinates.
                    _dropdown.OnGUI(innerRect);
                }
                GUI.EndGroup();

                if (needH)
                {
                    float maxScroll = Mathf.Max(0f, contentWidth - viewRect.width);
                    _hScroll = GUI.HorizontalScrollbar(hbarRect, _hScroll, viewRect.width, 0f, maxScroll + viewRect.width);
                    _hScroll = Mathf.Clamp(_hScroll, 0f, maxScroll);
                }
                else
                {
                    _hScroll = 0f;
                }
            }
        }

        public int ItemCount
        {
            get
            {
                return countItems(rootItem);

                static int countItems(TreeViewItem item)
                {
                    return item.children?.Aggregate(1, (count, child) => count + countItems(child)) ?? 1;
                }
            }
        }

        private readonly Action<Object> _onSelected;
        private readonly GameObject _rootObject;
        private readonly Type _filterType;
        private readonly Texture2D _gameObjectImage;
        private readonly Dictionary<int, Object> _itemsMap = new();
        private readonly HashSet<int> _groupIds = new();
        private readonly HashSet<int> _separatorBeforeIds = new();
        private GUIStyle _groupLabelStyle;
        private readonly ComponentDropdownPopupContent _content;

        private float _cachedContentWidth = -1f;
        private string _cachedSearchString = null;
        private int _nextId;

        public ComponentDropdown(TreeViewState state, GameObject rootObject, Type filterType, Action<Object> onSelected) : base(state)
        {
            _rootObject = rootObject;
            _filterType = filterType;
            _onSelected = onSelected;

            rowHeight *= 1.5f;
            _gameObjectImage = EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image as Texture2D;
            _content = new ComponentDropdownPopupContent(this);
        }

        protected override TreeViewItem BuildRoot()
        {
            _itemsMap.Clear();
            _groupIds.Clear();
            _separatorBeforeIds.Clear();
            _cachedContentWidth = -1f;
            _cachedSearchString = null;
            _nextId = 0;

            // EntityId 해시는 32비트로 잘려 충돌할 수 있으므로 생성 순서의 순번을 행 ID로 사용합니다.
            TreeViewItem root = new TreeViewItem(_nextId++, -1, "root");

            if (_filterType == typeof(GameObject))
            {
                addGameObjectToDropdown(root, _rootObject);
            }
            else
            {
                TreeViewItem treeRootItem = createItem(_rootObject, 0);

                Dictionary<GameObject, TreeViewItem> visited = new Dictionary<GameObject, TreeViewItem>
                {
                    { _rootObject, treeRootItem }
                };

                var candidates = _rootObject.GetComponentsInChildren(_filterType, true)
                    .Where(component => component != null)
                    .GroupBy(component => component.gameObject)
                    .ToArray();

                foreach (var group in candidates)
                    addComponentToDropdown(group.Key, visited);

                foreach (var group in candidates)
                {
                    var objectItem = visited[group.Key];
                    var components = group.ToArray();
                    if (components.Length == 1)
                    {
                        var component = components[0];
                        objectItem.displayName = $"{group.Key.name} ({component.GetType().Name})";
                        _itemsMap.Add(objectItem.id, component);
                        continue;
                    }

                    var componentsItem = new TreeViewItem(_nextId++, 0, "Components") { parent = objectItem };
                    objectItem.children ??= new List<TreeViewItem>();
                    objectItem.children.Insert(0, componentsItem);
                    _groupIds.Add(componentsItem.id);
                    if (objectItem.children.Count > 1)
                        _separatorBeforeIds.Add(objectItem.children[1].id);

                    var types = components.ToLookup(component => component.GetType());
                    var ordinals = new Dictionary<Type, int>();
                    foreach (var component in components)
                    {
                        var type = component.GetType();
                        var label = $"{group.Key.name} ({type.Name})";
                        if (types[type].Count() > 1)
                        {
                            var ordinal = ordinals.GetValueOrDefault(type) + 1;
                            ordinals[type] = ordinal;
                            label += $" #{ordinal}";
                        }

                        var componentItem = new TreeViewItem(_nextId++, 0, label);
                        componentsItem.AddChild(componentItem);
                        _itemsMap.Add(componentItem.id, component);
                    }
                }

                setDepth(treeRootItem);

                static void setDepth(TreeViewItem item, int depth = 0)
                {
                    item.depth = depth;

                    if (item.hasChildren)
                    {
                        foreach (TreeViewItem treeViewItem in item.children)
                            setDepth(treeViewItem, depth + 1);
                    }
                }

                root.AddChild(treeRootItem);
            }

            return root;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            IList<TreeViewItem> rows;
            if (string.IsNullOrEmpty(searchString))
            {
                rows = base.BuildRows(root);
            }
            else
            {
                // 기본 검색 결과는 이름순으로 정렬되므로, UI Toolkit 경로와 같은 계층 순서로 직접 수집합니다.
                var matches = new List<TreeViewItem>();
                collectMatches(root);
                rows = matches;

                void collectMatches(TreeViewItem item)
                {
                    if (item.children is null)
                        return;

                    foreach (TreeViewItem child in item.children)
                    {
                        if (child.displayName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                            matches.Add(child);

                        collectMatches(child);
                    }
                }
            }

            if (_cachedSearchString != searchString)
            {
                _cachedSearchString = searchString;
                _cachedContentWidth = -1f;
            }

            return rows;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            TreeViewItem item = args.item;

            Rect labelRect = new Rect(
                string.IsNullOrEmpty(searchString) ?
                    depthIndentWidth + args.item.depth * depthIndentWidth :
                    0f,
                args.rowRect.y + (args.rowRect.height - EditorGUIUtility.singleLineHeight) * 0.5f,
                args.rowRect.width,
                EditorGUIUtility.singleLineHeight);

            GUIContent guiContent = new GUIContent(item.displayName, getIconForItem(item));
            if (IsGroup(item))
            {
                _groupLabelStyle ??= new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Italic };
                using (new EditorGUI.DisabledScope(true))
                    EditorGUI.LabelField(labelRect, guiContent, _groupLabelStyle);
            }
            else
            {
                EditorGUI.LabelField(labelRect, guiContent);
            }

            if (HasSeparatorBefore(item))
                EditorGUI.DrawRect(new Rect(labelRect.x, args.rowRect.y, args.rowRect.xMax - labelRect.x, 1f), SeparatorColor);
        }

        // 그룹 블록 다음에 오는 첫 실제 자식 행의 위쪽 경계에 구분선을 그려, 그룹을 접어도 위치가 유지됩니다.
        internal bool HasSeparatorBefore(TreeViewItem item) =>
            string.IsNullOrEmpty(searchString) && _separatorBeforeIds.Contains(item.id);

        internal bool IsGroup(TreeViewItem item) => _groupIds.Contains(item.id);

        internal static Color SeparatorColor =>
            EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.18f) : new Color(0f, 0f, 0f, 0.22f);

        protected override void DoubleClickedItem(int id)
        {
            if (!_itemsMap.TryGetValue(id, out var selection) || selection == null) return;

            _content.editorWindow?.Close();
            _onSelected?.Invoke(selection);
        }

        internal void Choose(int id) => DoubleClickedItem(id);

        internal Texture2D GetIcon(TreeViewItem item) => getIconForItem(item);

        // UI Toolkit TreeView용 항목입니다. IMGUI BuildRows와 같이 검색어가 있으면 이름이 일치하는
        // 항목만 들여쓰기 없는 평면 목록으로 반환합니다.
        internal List<TreeViewItemData<TreeViewItem>> CreateElementItems()
        {
            List<TreeViewItemData<TreeViewItem>> result = new();
            if (rootItem?.children is null)
                return result;

            if (string.IsNullOrEmpty(searchString))
            {
                foreach (TreeViewItem child in rootItem.children)
                    result.Add(toElementItem(child));

                return result;
            }

            foreach (TreeViewItem child in rootItem.children)
                collectMatches(child);

            return result;

            static TreeViewItemData<TreeViewItem> toElementItem(TreeViewItem item)
            {
                List<TreeViewItemData<TreeViewItem>> children = item.children?
                    .Select(toElementItem)
                    .ToList();
                return new TreeViewItemData<TreeViewItem>(item.id, item, children);
            }

            void collectMatches(TreeViewItem item)
            {
                if (item.displayName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    result.Add(new TreeViewItemData<TreeViewItem>(item.id, item));

                if (item.children is null)
                    return;

                foreach (TreeViewItem child in item.children)
                    collectMatches(child);
            }
        }

        public void Show(Rect rect)
        {
            Reload();
            ExpandAll();
            PopupWindow.Show(rect, _content);
        }

        public float GetContentWidth(float minWidth)
        {
            if (_cachedContentWidth >= 0f)
                return Mathf.Max(minWidth, _cachedContentWidth);

            const float padding = 70f; // icon + spacing + safety
            float max = minWidth;

            IList<TreeViewItem> rows = GetRows();
            GUIStyle style = EditorStyles.label;

            for (int i = 0; i < rows.Count; i++)
            {
                TreeViewItem item = rows[i];

                float indent = string.IsNullOrEmpty(searchString)
                    ? (depthIndentWidth + item.depth * depthIndentWidth)
                    : 0f;

                Vector2 size = style.CalcSize(new GUIContent(item.displayName));
                float width = indent + size.x + padding;
                if (width > max) max = width;
            }

            _cachedContentWidth = max;
            return max;
        }

        private Texture2D getIconForItem(TreeViewItem item)
        {
            if (_itemsMap.TryGetValue(item.id, out var selection) && selection is Component component && component != null)
                return EditorGUIUtility.ObjectContent(component, component.GetType()).image as Texture2D ?? _gameObjectImage;

            return _gameObjectImage;
        }

        private void addComponentToDropdown(GameObject selectedObject, Dictionary<GameObject, TreeViewItem> visited, TreeViewItem prevItem = null)
        {
            while (true)
            {
                if (visited.TryGetValue(selectedObject, out TreeViewItem advancedDropdownItem))
                {
                    if (prevItem is not null)
                        advancedDropdownItem.AddChild(prevItem);

                    return;
                }

                TreeViewItem gameObjectItem = createItem(selectedObject, 0);
                visited.Add(selectedObject, gameObjectItem);

                if (prevItem is not null)
                    gameObjectItem.AddChild(prevItem);

                selectedObject = selectedObject.transform.parent.gameObject;
                prevItem = gameObjectItem;
            }
        }

        private void addGameObjectToDropdown(TreeViewItem parent, GameObject gameObject, int depth = 0)
        {
            TreeViewItem gameObjectItem = createItem(gameObject, depth);

            parent.AddChild(gameObjectItem);
            _itemsMap[gameObjectItem.id] = gameObject;

            foreach (Transform child in gameObject.transform)
                addGameObjectToDropdown(gameObjectItem, child.gameObject, depth + 1);
        }

        private TreeViewItem createItem(GameObject gameObject, int depth) =>
            new(_nextId++, depth, $"{gameObject.name} (GameObject)");
    }
}
#endif
