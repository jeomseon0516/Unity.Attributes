#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Jeomseon.Unity.Attributes.Editor.ConstructorPipelines;

namespace Jeomseon.Unity.Attributes.Editor
{
    internal static class ConstructorSelectionService
    {
        public static void ChooseConstructor(PendingConstructorSelection pending, int index)
        {
            if (pending is null) throw new ArgumentNullException(nameof(pending));
            if ((uint)index >= pending.ConstructibleConstructors.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            pending.ChosenConstructorIndex = index;
            pending.ParameterPipelineChoice.Clear();
            pending.ErrorMessage = null;

            ParameterInfo[] parameters = pending.ConstructibleConstructors[index].GetParameters();
            object[] values = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].HasDefaultValue)
                {
                    values[i] = parameters[i].DefaultValue;
                    continue;
                }

                IReadOnlyList<ISerializeReferenceSelectorConstructorParameterPipeline> candidates =
                    ConstructorParameterPipelineRegistry.GetCandidates(parameters[i].ParameterType);
                values[i] = candidates[0].GetDefaultValue(parameters[i].ParameterType);
            }

            pending.ParameterValues = values;
        }

        public static bool TryCreateInstance(PendingConstructorSelection pending, out object instance, out string errorMessage)
        {
            instance = null;
            errorMessage = null;

            if (pending?.ChosenConstructor is null)
            {
                errorMessage = "생성자를 선택하세요.";
                return false;
            }

            try
            {
                instance = pending.ChosenConstructor.Invoke(pending.ParameterValues);
                return true;
            }
            catch (Exception exception)
            {
                Exception cause = exception is TargetInvocationException { InnerException: not null }
                    ? exception.InnerException
                    : exception;
                errorMessage = $"{cause.GetType().Name}: {cause.Message}";
                return false;
            }
        }
    }
}
#endif
