#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.Attributes.Editor
{
    using UnityObjectEditor = UnityEditor.Editor;

    /// <summary>
    /// UI Toolkit 기반 InspectorElement에 확장 영역을 삽입하는 공통 구현입니다.
    /// Unity 내부 멤버 이름처럼 버전별로 달라지는 부분은 파생 백엔드가 제공합니다.
    /// </summary>
    internal abstract class InspectorElementInjectionBackendBase : IInspectorInjectionBackend
    {
        private const string ContainerName = "jeomseon-inspector-injection";
        private const double ScanIntervalSeconds = 0.5d;

        private readonly Dictionary<Type, EditorAccessor> _editorAccessors = new();
        private readonly List<IMGUIContainer> _containers = new();
        private Type _inspectorWindowType;
        private double _nextScanTime;
        private bool _started;
        private bool _disabled;
        private bool _attachedAny;

        public abstract string Name { get; }
        public bool IsRunning => _started && !_disabled && _attachedAny;
        protected abstract IReadOnlyList<string> EditorMemberNames { get; }
        protected virtual string InspectorElementClassName => "unity-inspector-element";

        public bool IsSupported
        {
            get
            {
                EnsureInspectorWindowType();
                return !_disabled && _inspectorWindowType != null;
            }
        }

        public void Start()
        {
            if (_started || !IsSupported)
                return;

            try
            {
                _started = true;
                EditorApplication.update += Update;
                ScanInspectors();
            }
            catch (Exception exception)
            {
                Disable(exception);
            }
        }

        public void Dispose()
        {
            EditorApplication.update -= Update;
            RemoveAttachedContainers();
            _editorAccessors.Clear();
            _started = false;
            _disabled = false;
            _attachedAny = false;
        }

        private void Update()
        {
            if (EditorApplication.timeSinceStartup < _nextScanTime)
                return;

            _nextScanTime = EditorApplication.timeSinceStartup + ScanIntervalSeconds;
            try
            {
                ScanInspectors();
            }
            catch (Exception exception)
            {
                Disable(exception);
            }
        }

        private void Disable(Exception exception)
        {
            if (_disabled)
                return;

            _disabled = true;
            EditorApplication.update -= Update;
            RemoveAttachedContainers();
            _editorAccessors.Clear();
            _started = false;
            _attachedAny = false;
            Debug.LogWarning(
                $"[Jeomseon Inspector Injection/{Name}] 내부 Inspector 접근에 실패해 " +
                "Injection 기능만 비활성화합니다. 다른 Attribute 기능은 계속 사용할 수 있습니다.\n" +
                exception);
        }

        private void ScanInspectors()
        {
            EnsureInspectorWindowType();
            if (_inspectorWindowType == null)
                return;

            IEnumerable<EditorWindow> inspectorWindows = Resources
                .FindObjectsOfTypeAll(_inspectorWindowType)
                .OfType<EditorWindow>();

            foreach (EditorWindow inspectorWindow in inspectorWindows)
            {
                VisualElement root = inspectorWindow.rootVisualElement;
                if (root == null)
                    continue;

                List<VisualElement> inspectorElements =
                    root.Query<VisualElement>(className: InspectorElementClassName).ToList();

                foreach (VisualElement inspectorElement in inspectorElements)
                    AttachOrUpdate(inspectorElement);
            }
        }

        private void AttachOrUpdate(VisualElement inspectorElement)
        {
            UnityObjectEditor editor = GetEditor(inspectorElement);
            if (editor == null || editor.target is not (MonoBehaviour or ScriptableObject))
                return;

            IMGUIContainer existing = inspectorElement.Q<IMGUIContainer>(ContainerName);
            if (existing?.userData is InjectionContext existingContext)
            {
                if (existingContext.IsDisposed)
                {
                    inspectorElement.Remove(existing);
                }
                else
                {
                    existingContext.UpdateEditor(editor);
                    return;
                }
            }

            InjectionContext context = new(editor);
            if (!context.HasDrawers)
            {
                context.Dispose();
                return;
            }

            IMGUIContainer container = new(context.OnInspectorGUI)
            {
                name = ContainerName,
                userData = context
            };

            container.RegisterCallback<DetachFromPanelEvent>(_ =>
            {
                context.Dispose();
                _containers.Remove(container);
            });
            inspectorElement.Add(container);
            _containers.Add(container);
            _attachedAny = true;
        }

        private void RemoveAttachedContainers()
        {
            foreach (IMGUIContainer container in _containers.ToArray())
            {
                if (container.userData is InjectionContext context)
                    context.Dispose();

                container.RemoveFromHierarchy();
            }

            _containers.Clear();
        }

        private UnityObjectEditor GetEditor(VisualElement inspectorElement)
        {
            Type elementType = inspectorElement.GetType();
            if (!_editorAccessors.TryGetValue(elementType, out EditorAccessor accessor))
            {
                accessor = CreateEditorAccessor(elementType);
                _editorAccessors.Add(elementType, accessor);
            }

            return accessor.Get(inspectorElement);
        }

        private EditorAccessor CreateEditorAccessor(Type elementType)
        {
            const BindingFlags Flags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (string memberName in EditorMemberNames)
            {
                PropertyInfo property = elementType.GetProperty(memberName, Flags);
                if (property?.CanRead == true &&
                    typeof(UnityObjectEditor).IsAssignableFrom(property.PropertyType))
                    return new EditorAccessor(property);

                FieldInfo field = elementType.GetField(memberName, Flags);
                if (field != null && typeof(UnityObjectEditor).IsAssignableFrom(field.FieldType))
                    return new EditorAccessor(field);
            }

            Debug.LogWarning(
                $"[Jeomseon Inspector Injection/{Name}] {elementType.FullName}에서 " +
                $"Editor 멤버를 찾지 못했습니다. 이 Unity 패치 버전의 내부 구조를 확인해야 합니다.");

            return EditorAccessor.Unsupported;
        }

        private void EnsureInspectorWindowType()
        {
            _inspectorWindowType ??=
                typeof(UnityObjectEditor).Assembly.GetType("UnityEditor.InspectorWindow");
        }

        private readonly struct EditorAccessor
        {
            public static readonly EditorAccessor Unsupported = new(null, null);

            private readonly PropertyInfo _property;
            private readonly FieldInfo _field;

            public EditorAccessor(PropertyInfo property) : this(property, null)
            {
            }

            public EditorAccessor(FieldInfo field) : this(null, field)
            {
            }

            private EditorAccessor(PropertyInfo property, FieldInfo field)
            {
                _property = property;
                _field = field;
            }

            public UnityObjectEditor Get(VisualElement element)
            {
                try
                {
                    return _property?.GetValue(element) as UnityObjectEditor ??
                           _field?.GetValue(element) as UnityObjectEditor;
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    return null;
                }
            }
        }

        private sealed class InjectionContext : IDisposable
        {
            private readonly List<IInspectorInjectedDrawer> _drawers;
            private UnityObjectEditor _editor;
            private bool _disposed;

            public InjectionContext(UnityObjectEditor editor)
            {
                _drawers = CreateDrawers();
                UpdateEditor(editor);
            }

            public bool HasDrawers => _drawers.Count > 0;
            public bool IsDisposed => _disposed;

            public void UpdateEditor(UnityObjectEditor editor)
            {
                if (_disposed || ReferenceEquals(_editor, editor))
                    return;

                _editor = editor;
                foreach (IInspectorInjectedDrawer drawer in _drawers)
                    drawer.OnEnable(_editor);
            }

            public void OnInspectorGUI()
            {
                if (_disposed || _editor == null)
                    return;

                foreach (IInspectorInjectedDrawer drawer in _drawers)
                    drawer.OnInspectorGUI(_editor);
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                foreach (IInspectorInjectedDrawer drawer in _drawers)
                    drawer.Dispose();

                _drawers.Clear();
                _editor = null;
                _disposed = true;
            }

            private static List<IInspectorInjectedDrawer> CreateDrawers()
            {
                List<IInspectorInjectedDrawer> result = new();
                foreach (Type type in TypeCache.GetTypesDerivedFrom<IInspectorInjectedDrawer>())
                {
                    if (type.IsAbstract || type.IsInterface)
                        continue;

                    try
                    {
                        if (Activator.CreateInstance(type) is IInspectorInjectedDrawer drawer)
                            result.Add(drawer);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }

                return result;
            }
        }
    }
}
#endif
