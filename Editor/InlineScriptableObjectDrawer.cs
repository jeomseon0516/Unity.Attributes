using Jeomseon.Unity.EditorToolkit.Editor.Window;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(InlineScriptableObjectAttribute))]
    internal sealed class InlineScriptableObjectDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference ||
                !typeof(ScriptableObject).IsAssignableFrom(fieldInfo.FieldType))
                return new PropertyField(property);

            var root = new VisualElement();
            var reference = new ObjectField(property.displayName)
            {
                objectType = fieldInfo.FieldType,
                allowSceneObjects = false,
                bindingPath = property.propertyPath
            };
            var foldout = new Foldout { text = "Asset Properties", value = false };
            var inspector = new ScriptableObjectInspector();
            foldout.Add(inspector);
            root.Add(reference);
            root.Add(foldout);

            root.TrackPropertyValue(property, Refresh);
            foldout.RegisterValueChangedCallback(_ => Refresh(property));
            Refresh(property);
            return root;

            void Refresh(SerializedProperty current)
            {
                var asset = current.hasMultipleDifferentValues
                    ? null
                    : current.objectReferenceValue as ScriptableObject;
                foldout.style.display = asset != null ? DisplayStyle.Flex : DisplayStyle.None;
                inspector.SetTarget(foldout.value ? asset : null);
            }
        }
    }
}
