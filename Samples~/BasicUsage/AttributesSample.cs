using Jeomseon.Attribute;
using UnityEngine;

namespace Jeomseon.Samples.Attributes
{
    public sealed class AttributesSample : MonoBehaviour
    {
        [InfoBox("EditorToolkit을 함께 설치하면 Attribute Drawer가 표시됩니다.")]
        [DisplayAs("표시 이름")]
        [SerializeField] private string _message = "Hello";

        [ReadOnly]
        [SerializeField] private int _changeCount;

        [OnChangedValueForMethod(nameof(_message))]
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
