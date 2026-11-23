using UnityEngine;

namespace Jeomseon.Samples.Attributes
{
    [CreateAssetMenu(
        fileName = nameof(InlineScriptableObjectSampleSettings),
        menuName = "Jeomseon/Samples/Attributes/Inline ScriptableObject Sample Settings")]
    public sealed class InlineScriptableObjectSampleSettings : ScriptableObject
    {
        [SerializeField] private string label = "Shared";
        [SerializeField, Range(0f, 10f)] private float speed = 1f;
        [SerializeField] private int count = 1;
        [SerializeField] private Color tint = Color.white;

        public string Label => label;
        public float Speed => speed;
        public int Count => count;
        public Color Tint => tint;
    }
}
