using System.Collections.Generic;
using Jeomseon.Unity.Attributes;
using UnityEngine;

namespace Jeomseon.Attribute.Tests
{
    public class HierarchyObjectPickerTestComponent : MonoBehaviour
    {
        [SerializeField, HierarchyObjectPicker]
        private HierarchyObjectPickerTestComponent single;

        [SerializeField, HierarchyObjectPicker]
        private HierarchyObjectPickerTestComponent[] slots = new HierarchyObjectPickerTestComponent[2];

        [SerializeField, HierarchyObjectPicker]
        private List<HierarchyObjectPickerTestComponent> list = new() { null, null };
    }
}
