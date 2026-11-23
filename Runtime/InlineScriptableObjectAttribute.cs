using System;
using System.Diagnostics;
using UnityEngine;

namespace Jeomseon.Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Field), Conditional("UNITY_EDITOR")]
    public sealed class InlineScriptableObjectAttribute : PropertyAttribute
    {
    }
}
