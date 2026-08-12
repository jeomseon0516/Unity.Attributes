using UnityEngine;

namespace Jeomseon.Attribute.Tests
{
    public sealed class SelectorManagedReferenceTestComponent : MonoBehaviour
    {
        [SerializeReference, SerializeReferenceSelector] public ManagedReferenceTestPayload WithSelector;
    }
}
