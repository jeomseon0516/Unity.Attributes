using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jeomseon.Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true), Conditional("UNITY_EDITOR")]
    public sealed class SpritePreviewAttribute : PropertyAttribute
    {
        public float Size { get; } = 0f;

        public SpritePreviewAttribute(float size = 64f)
        {
            Size = Mathf.Max(0f, size);
        }
    }
}
