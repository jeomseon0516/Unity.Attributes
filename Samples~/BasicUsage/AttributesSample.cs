using Jeomseon.Attribute;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jeomseon.Samples.Attributes
{
    public sealed class AttributesSample : MonoBehaviour
    {
        [InfoBox("Attributes 패키지가 Attribute Drawer를 함께 제공합니다.")]
        [InspectorName("표시 이름")]
        [SerializeField, FormerlySerializedAs("_message")] private string message = "Hello";

        [ReadOnly]
        [SerializeField, FormerlySerializedAs("_changeCount")] private int changeCount;

        [Min(0f)]
        [SerializeField, FormerlySerializedAs("_minimumBoundedValue")] private float minimumBoundedValue;

        [MaxValue(10)]
        [SerializeField, FormerlySerializedAs("_maximumBoundedValue")] private int maximumBoundedValue;

        [InvokeOnInspectorChange(nameof(message))]
        private void OnMessageChanged()
        {
            changeCount++;
            Debug.Log($"값 변경 콜백: {message}");
        }

        [InspectorButton("메시지 출력")]
        private void PrintMessage()
        {
            Debug.Log(message);
        }
    }
}
