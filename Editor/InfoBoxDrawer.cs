#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Jeomseon.Unity.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(InfoBoxAttribute))]
    internal sealed class InfoBoxDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            InfoBoxAttribute infoBoxAttribute = (InfoBoxAttribute)attribute;
            HelpBoxMessageType messageType = infoBoxAttribute.Type switch
            {
                InfoBoxType.Warning => HelpBoxMessageType.Warning,
                InfoBoxType.Error => HelpBoxMessageType.Error,
                _ => HelpBoxMessageType.Info,
            };

            VisualElement root = new();
            root.Add(new HelpBox(infoBoxAttribute.Message, messageType));
            root.Add(new PropertyField(property));
            return root;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InfoBoxAttribute infoBoxAttribute = (attribute as InfoBoxAttribute)!;
            
            MessageType messageType = infoBoxAttribute.Type switch
            {
                InfoBoxType.Info => MessageType.Info,
                InfoBoxType.Warning => MessageType.Warning,
                InfoBoxType.Error => MessageType.Error,
                _ => MessageType.None,
            };

            float textHeight = CalculateHelpBoxHeight(infoBoxAttribute.Message, position.width);

            Rect helpBoxRect = new(
                position.x,
                position.y,
                position.width,
                textHeight);

            Rect propertyRect = new(
                position.x,
                position.y + textHeight + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                EditorGUI.GetPropertyHeight(property, label, true));

            EditorGUI.HelpBox(helpBoxRect, infoBoxAttribute.Message, messageType);
            EditorGUI.PropertyField(propertyRect, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InfoBoxAttribute infoBoxAttribute = (attribute as InfoBoxAttribute)!;
            // currentViewWidth includes the Inspector margins and indent area. Use a
            // conservative content width so wrapped text is never allocated too little height.
            float contentWidth = Mathf.Max(1f, EditorGUIUtility.currentViewWidth - 40f);
            float textHeight = CalculateHelpBoxHeight(infoBoxAttribute.Message, contentWidth);
            
            return textHeight + EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        private static float CalculateHelpBoxHeight(string message, float width)
        {
            return EditorStyles.helpBox.CalcHeight(new GUIContent(message), Mathf.Max(1f, width));
        }
    }
}
#endif
