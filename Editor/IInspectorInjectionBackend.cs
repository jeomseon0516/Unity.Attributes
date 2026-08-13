#if UNITY_EDITOR
using System;

namespace Jeomseon.Unity.Attributes.Editor
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
