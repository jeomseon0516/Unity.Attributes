#if UNITY_EDITOR
namespace Jeomseon.Unity.Attributes.Editor
{
    internal static class InspectorInjectionBackendFactory
    {
        public static IInspectorInjectionBackend Create()
        {
            return new Unity6InspectorInjectionBackend();
        }
    }
}
#endif
