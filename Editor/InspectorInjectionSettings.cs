#if UNITY_EDITOR
using UnityEditor;

namespace Jeomseon.Attribute.Editor
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
                guiHandler = _ =>
                {
                    EditorGUI.BeginChangeCheck();
                    bool enabled = EditorGUILayout.Toggle(
                        "Enable Inspector Injection",
                        Enabled);
                    if (EditorGUI.EndChangeCheck())
                        Enabled = enabled;

                    EditorGUILayout.HelpBox(
                        "Experimental: injects Attribute UI into the default Inspector. " +
                        "When disabled, InspectorButton uses the Inspector header fallback.",
                        MessageType.Info);
                }
            };
        }
    }
}
#endif
