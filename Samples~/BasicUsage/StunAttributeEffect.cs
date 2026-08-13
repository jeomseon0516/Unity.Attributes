using System;
using UnityEngine;

namespace Jeomseon.Samples.Attributes
{
    // SerializeReferenceSelector의 오버로드 선택과 기본 파이프라인을 확인하기 위한 Sample 타입입니다.
    [Serializable]
    public sealed class StunAttributeEffect : AttributeEffect
    {
        public float Duration;
        public GameObject Source;
        public StunDirectionMode DirectionMode;
        public Vector3 Direction;

        public StunAttributeEffect() { }

        public StunAttributeEffect(float duration, string description)
        {
            Duration = duration;
            Description = description;
        }

        public StunAttributeEffect(
            GameObject source,
            StunDirectionMode directionMode,
            Vector3 direction,
            float duration = 1f,
            string description = "생성자 기본 설명")
        {
            Source = source;
            DirectionMode = directionMode;
            Direction = direction;
            Duration = duration;
            Description = description;
        }
    }

    public enum StunDirectionMode
    {
        Forward,
        AwayFromSource
    }
}
