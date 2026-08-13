using Jeomseon.Unity.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jeomseon.Samples.Attributes
{
    public sealed class ObjectAssignmentSample : MonoBehaviour
    {
        [InfoBox("Inspector를 표시하면 BoxCollider를 찾거나 Undo 가능한 방식으로 추가합니다.")]
        [GetOrAddComponent]
        [SerializeField, FormerlySerializedAs("_boxCollider")] private BoxCollider boxCollider;

        [InfoBox("드롭다운에서 현재 Scene 계층의 GameObject를 선택합니다.")]
        [HierarchyObjectPicker]
        [SerializeField, FormerlySerializedAs("_selectedGameObject")] private GameObject selectedGameObject;

        [InfoBox("드롭다운에서 선택한 GameObject의 Transform을 할당합니다.")]
        [HierarchyObjectPicker]
        [SerializeField, FormerlySerializedAs("_selectedTransform")] private Transform selectedTransform;
    }
}
