using UnityEngine;

namespace Jeomseon.Attribute.Tests
{
    public sealed class PlainManagedReferenceTestComponent : MonoBehaviour
    {
        [SerializeReference] public ManagedReferenceTestPayload Plain;
    }
}
