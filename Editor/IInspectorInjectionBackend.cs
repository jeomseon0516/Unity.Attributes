#if UNITY_EDITOR
using System;

namespace Jeomseon.Attribute.Editor
{
    internal interface IInspectorInjectionBackend : IDisposable
    {
        string Name { get; }
        bool IsSupported { get; }
        bool IsRunning { get; }
        void Start();
    }
}
#endif
