#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jeomseon.Unity.EditorToolkit.Editor;

namespace Jeomseon.Unity.Attributes.Editor.ConstructorPipelines
{
    /// <summary>
    /// .. TypeCache로 검색한 매개변수 파이프라인 구현체를 보관하고, 생성자 매개변수 타입에 맞는
    /// 파이프라인 후보와 "생성 가능한(파이프라인이 모든 매개변수를 지원하는) 생성자"를
    /// SerializeReferenceSelectorDrawer에 제공합니다.
    /// </summary>
    internal static class ConstructorParameterPipelineRegistry
    {
        private static readonly List<ISerializeReferenceSelectorConstructorParameterPipeline> Pipelines =
            EditorTypeDiscovery.GetConcreteTypesDerivedFrom<ISerializeReferenceSelectorConstructorParameterPipeline>()
                .Where(type => type.GetConstructor(Type.EmptyTypes) is not null)
                .Select(type => (ISerializeReferenceSelectorConstructorParameterPipeline)Activator.CreateInstance(type))
                .ToList();

        public static IReadOnlyList<ISerializeReferenceSelectorConstructorParameterPipeline> GetCandidates(Type parameterType)
        {
            return Pipelines.Where(pipeline => pipeline.CanHandle(parameterType)).ToList();
        }

        public static bool IsConstructible(Type parameterType) => GetCandidates(parameterType).Count > 0;

        public static bool HasConstructibleConstructor(Type type)
        {
            return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Any(IsConstructible);
        }

        public static ConstructorInfo[] GetConstructibleConstructors(Type type)
        {
            return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .Where(IsConstructible)
                .ToArray();
        }

        public static string GetDisplayName(ISerializeReferenceSelectorConstructorParameterPipeline pipeline)
        {
            Type pipelineType = pipeline.GetType();
            SerializeReferenceSelectorConstructorPipelineNameAttribute nameAttribute =
                pipelineType.GetCustomAttribute<SerializeReferenceSelectorConstructorPipelineNameAttribute>();
            return nameAttribute?.DisplayName ?? pipelineType.Name;
        }

        private static bool IsConstructible(ConstructorInfo constructor)
        {
            return constructor.GetParameters().All(parameter => IsConstructible(parameter.ParameterType));
        }
    }
}
#endif
