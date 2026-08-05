using Jeomseon.Attribute;
using UnityEngine;

namespace Jeomseon.Samples.Attributes
{
    public sealed class AttributesSample : MonoBehaviour
    {
        [InfoBox("Attributes 패키지가 Attribute Drawer를 함께 제공합니다.")]
        [InspectorName("표시 이름")]
        [SerializeField] private string _message = "Hello";

        [ReadOnly]
        [SerializeField] private int _changeCount;

        [Min(0f)]
        [SerializeField] private float _minimumBoundedValue;

        [MaxValue(10)]
        [SerializeField] private int _maximumBoundedValue;

        [InvokeOnInspectorChange(nameof(_message))]
        private void OnMessageChanged()
        {
            _changeCount++;
            Debug.Log($"값 변경 콜백: {_message}");
        }

        [InspectorButton("메시지 출력")]
        private void PrintMessage()
        {
            Debug.Log(_message);
        }
    }
}
