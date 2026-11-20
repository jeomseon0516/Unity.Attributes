#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jeomseon.Unity.Attributes.Editor
{
    internal static class InspectorInjectionSettings
    {
        private const string EnabledKey =
            "Jeomseon.Unity.Attributes.InspectorInjection.Enabled";

        public static bool Enabled
        {
            get => EditorUserSettings.GetConfigValue(EnabledKey) != "false";
            set
            {
                EditorUserSettings.SetConfigValue(EnabledKey, value ? "true" : "false");
                InspectorButtonFallback.Refresh();
                InspectorInjectionService.Refresh();
            }
        }

        [SettingsProvider]
        private static SettingsProvider CreateProvider()
        {
            return new SettingsProvider(
                "Project/Jeomseon/Attributes",
                SettingsScope.Project)
            {
                label = "Attributes",
                activateHandler = (_, rootElement) => rootElement.Add(CreateSettingsGUI("Attributes"))
            };
        }

        private static VisualElement CreateSettingsGUI(string title)
        {
            // UI Toolkit SettingsProvider는 창이 제목을 그려 주지 않으므로 IMGUI 화면과 같은 제목을 직접 둡니다.
            VisualElement root = new();
            root.style.paddingLeft = 10f;
            root.style.paddingRight = 10f;

            Label titleLabel = new(title);
            titleLabel.AddToClassList("title");
            titleLabel.style.fontSize = 19;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginTop = 1f;
            titleLabel.style.marginBottom = 10f;
            root.Add(titleLabel);

            Toggle enabledToggle = new("Enable Inspector Injection")
            {
                value = Enabled
            };
            enabledToggle.RegisterValueChangedCallback(change => Enabled = change.newValue);
            root.Add(enabledToggle);

            root.Add(new HelpBox(
                "Experimental: injects Attribute UI into the default Inspector. " +
                "When disabled, InspectorButton uses the Inspector header fallback.",
                HelpBoxMessageType.Info));
            return root;
        }
    }
}
#endif
