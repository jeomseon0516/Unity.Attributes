using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Jeomseon.Attribute
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true), Conditional("UNITY_EDITOR")]
    public sealed class InfoBoxAttribute : PropertyAttribute
    {
        public string Message { get; }
        public InfoBoxType Type { get; }

        public InfoBoxAttribute(string message, InfoBoxType type = InfoBoxType.Info)
        {
            Message = message ?? string.Empty;
            Type = type;
        }
    }

    public enum InfoBoxType
    {
        Info,
        Warning,
        Error
    }
}
