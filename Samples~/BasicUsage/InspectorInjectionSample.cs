using Jeomseon.Attribute;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jeomseon.Samples.Attributes
{
    public sealed class InspectorInjectionSample : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("_message")] private string message = "Inspector Injection";
        [ReadOnly, SerializeField] private int _changeCount;
        [ReadOnly, SerializeField] private int _buttonClickCount;

        [InvokeOnInspectorChange(nameof(message))]
        private void OnMessageChanged()
        {
            _changeCount++;
        }

        [InspectorButton("Injection 버튼 실행")]
        private void InvokeFromInspector()
        {
            Debug.Log($"{message} - 버튼 클릭 횟수: {++_buttonClickCount}", this);
        }
    }
}
