#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Jeomseon.Unity.Attributes.Editor
{
    /// <summary>
    /// .. baseType을 구현/상속하는 구체 타입들을 검색 가능한 트리로 보여줍니다. 클래스
    /// 상속 체인이 있으면(추상 클래스 등) 같은 조상을 공유하는 타입끼리 폴더처럼 묶고,
    /// 인터페이스를 바로 구현한 타입은 최상위에 나열합니다.
    /// </summary>
    internal sealed class TypeSelectorAdvancedDropdown : AdvancedDropdown
    {
        private sealed class TypeItem : AdvancedDropdownItem
        {
            public Type Type { get; }
            public TypeItem(string name, Type type) : base(name) => Type = type;
        }

        public event Action<Type> OnTypeSelected;
        public event Action OnCleared;

        private readonly Type _baseType;
        private readonly IReadOnlyList<Type> _concreteTypes;

        public TypeSelectorAdvancedDropdown(AdvancedDropdownState state, Type baseType, IReadOnlyList<Type> concreteTypes) : base(state)
        {
            _baseType = baseType;
            _concreteTypes = concreteTypes;
            minimumSize = new Vector2(260, 300);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new(_baseType.Name);

            root.AddChild(new AdvancedDropdownItem("(None)"));
            root.AddSeparator();

            foreach (Type concreteType in _concreteTypes.OrderBy(type => type.Name))
            {
                AdvancedDropdownItem group = GetOrCreateAncestorGroup(root, concreteType);
                group.AddChild(new TypeItem(concreteType.Name, concreteType));
            }

            return root;
        }

        private AdvancedDropdownItem GetOrCreateAncestorGroup(AdvancedDropdownItem root, Type concreteType)
        {
            List<Type> chain = new();
            for (Type ancestor = concreteType.BaseType;
                 ancestor != null && ancestor != typeof(object) && _baseType.IsAssignableFrom(ancestor);
                 ancestor = ancestor.BaseType)
            {
                chain.Add(ancestor);
            }
            chain.Reverse();

            AdvancedDropdownItem current = root;
            foreach (Type ancestor in chain)
            {
                AdvancedDropdownItem existingGroup = current.childList
                    .FirstOrDefault(item => item is not TypeItem && item.name == ancestor.Name);

                if (existingGroup is null)
                {
                    existingGroup = new AdvancedDropdownItem(ancestor.Name);
                    current.AddChild(existingGroup);
                }

                current = existingGroup;
            }

            return current;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item is TypeItem typeItem)
            {
                OnTypeSelected?.Invoke(typeItem.Type);
            }
            else
            {
                OnCleared?.Invoke();
            }
        }
    }
}
#endif
