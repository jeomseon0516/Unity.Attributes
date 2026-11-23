using Jeomseon.Unity.Attributes;
using UnityEngine;

namespace Jeomseon.Samples.Attributes
{
    public sealed class InlineScriptableObjectSample : MonoBehaviour
    {
        [SerializeField, InlineScriptableObject] private InlineScriptableObjectSampleSettings settings;

        public InlineScriptableObjectSampleSettings Settings => settings;
    }
}
