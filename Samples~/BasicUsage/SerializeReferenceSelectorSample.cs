using System.Collections.Generic;
using Jeomseon.Unity.Attributes;
using UnityEngine;

namespace Jeomseon.Samples.Attributes
{
    public sealed class SerializeReferenceSelectorSample : MonoBehaviour
    {
        [SerializeReference, SerializeReferenceSelector]
        public AttributeEffect SelectedEffect = new DamageAttributeEffect
        {
            Description = "단일 대상 피해",
            Damage = 10
        };

        [SerializeReference, SerializeReferenceSelector]
        public List<AttributeEffect> EffectSequence = new()
        {
            new DamageAttributeEffect
            {
                Description = "첫 번째 효과",
                Damage = 5
            },
            new HealAttributeEffect
            {
                Description = "두 번째 효과",
                Health = 3
            }
        };
    }
}
