using System;
using UnityEngine;

namespace Jeomseon.Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ShowIfAttribute : PropertyAttribute
    {
        public string MemberName { get; }
        public int ExpectedValue { get; }

        public ShowIfAttribute(string memberName, int expectedValue)
        {
            MemberName = memberName;
            ExpectedValue = expectedValue;
        }

        public ShowIfAttribute(string memberName, bool expectedValue = true)
            : this(memberName, expectedValue ? 1 : 0) { }
    }
}
