using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jeomseon.Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true), Conditional("UNITY_EDITOR")]
    public sealed class ReadOnlyAttribute : PropertyAttribute { }
}