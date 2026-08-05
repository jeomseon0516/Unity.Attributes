using System;
using System.Diagnostics;
using UnityEngine;

namespace Jeomseon.Attribute
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false), Conditional("UNITY_EDITOR")]
    public sealed class Vector2SliderAttribute : PropertyAttribute
    {
        public float Min { get; }
        public float Max { get; }

        public Vector2SliderAttribute(float min, float max)
        {
            Min = Mathf.Min(min, max);
            Max = Mathf.Max(min, max);
        }
    }
}
