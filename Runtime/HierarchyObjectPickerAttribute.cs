using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jeomseon.Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false), Conditional("UNITY_EDITOR")]
    public sealed class HierarchyObjectPickerAttribute : PropertyAttribute { }
}
