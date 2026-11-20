#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Jeomseon.Unity.Attributes;

namespace Jeomseon.Unity.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
    internal sealed class SpritePreviewDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (attribute is not SpritePreviewAttribute spritePreviewAttribute)
                return new VisualElement();

            VisualElement root = new();
            root.Add(new PropertyField(property));

            Image preview = new()
            {
                scaleMode = ScaleMode.ScaleToFit
            };
            preview.style.width = spritePreviewAttribute.Size;
            preview.style.height = spritePreviewAttribute.Size;
            preview.style.marginBottom = EditorGUIUtility.standardVerticalSpacing;
            root.Add(preview);

            root.TrackPropertyValue(property, Refresh);
            Refresh(property);
            return root;

            void Refresh(SerializedProperty current)
            {
                Sprite sprite = current.objectReferenceValue as Sprite;
                preview.image = sprite ? sprite.texture : null;
                preview.style.display = sprite ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is not SpritePreviewAttribute spritePreviewAttribute) return;

            EditorGUI.PropertyField(
                new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property,
                label);

            if (property.objectReferenceValue && property.objectReferenceValue is Sprite sprite)
            {
                Rect previewRect = new(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight,
                    spritePreviewAttribute.Size,
                    spritePreviewAttribute.Size);

                EditorGUI.DrawPreviewTexture(
                    previewRect,
                    sprite.texture,
                    null,
                    ScaleMode.ScaleToFit);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SpritePreviewAttribute spritePreviewAttribute = attribute as SpritePreviewAttribute;
            float totalHeight = EditorGUIUtility.singleLineHeight;

            if (property.objectReferenceValue && property.objectReferenceValue is Sprite)
            {
                totalHeight += spritePreviewAttribute.Size + EditorGUIUtility.standardVerticalSpacing;
            }

            return totalHeight;
        }
    }
}
#endif
