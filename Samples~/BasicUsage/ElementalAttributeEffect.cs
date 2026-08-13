using System;
using UnityEngine;

namespace Jeomseon.Samples.Attributes
{
    // StunAttributeEffect가 다루지 않는 나머지 매개변수 파이프라인(int/bool/double/Color/Vector2/
    // Vector4)과 생성자 예외 표시 흐름을 확인하기 위한 Sample 타입입니다.
    [Serializable]
    public sealed class ElementalAttributeEffect : AttributeEffect
    {
        public int Stacks;
        public bool IsCritical;
        public double Multiplier;
        public Color EffectColor;
        public Vector2 AreaSize;
        public Vector4 FalloffCurve;

        public ElementalAttributeEffect() { }

        public ElementalAttributeEffect(int stacks, bool isCritical)
        {
            Stacks = stacks;
            IsCritical = isCritical;
        }

        public ElementalAttributeEffect(double multiplier, Color effectColor)
        {
            Multiplier = multiplier;
            EffectColor = effectColor;
        }

        public ElementalAttributeEffect(
            Vector2 areaSize,
            Vector4 falloffCurve,
            double multiplier = 1.0,
            int stacks = 1)
        {
            AreaSize = areaSize;
            FalloffCurve = falloffCurve;
            Multiplier = multiplier;
            Stacks = stacks;
        }

        public ElementalAttributeEffect(string invalidConfiguration)
        {
            throw new InvalidOperationException($"잘못된 설정: {invalidConfiguration}");
        }
    }
}
