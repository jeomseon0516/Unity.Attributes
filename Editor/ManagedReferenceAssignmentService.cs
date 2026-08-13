#if UNITY_EDITOR
using System;
using UnityEditor;

namespace Jeomseon.Unity.Attributes.Editor
{
    internal static class ManagedReferenceAssignmentService
    {
        public static bool TryAssign(
            SerializedObject serializedObject,
            string propertyPath,
            Func<object> createValue,
            string undoName,
            out string errorMessage)
        {
            if (serializedObject is null) throw new ArgumentNullException(nameof(serializedObject));
            if (createValue is null) throw new ArgumentNullException(nameof(createValue));

            errorMessage = null;
            object[] values = new object[serializedObject.targetObjects.Length];

            try
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = createValue();
                }
            }
            catch (Exception exception)
            {
                Exception cause = exception is System.Reflection.TargetInvocationException { InnerException: not null }
                    ? exception.InnerException
                    : exception;
                errorMessage = $"{cause.GetType().Name}: {cause.Message}";
                return false;
            }

            SerializedObject[] targets = new SerializedObject[serializedObject.targetObjects.Length];
            SerializedProperty[] properties = new SerializedProperty[serializedObject.targetObjects.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = new SerializedObject(serializedObject.targetObjects[i]);
                properties[i] = targets[i].FindProperty(propertyPath);
                if (properties[i] is null)
                {
                    errorMessage = $"SerializedProperty를 찾을 수 없습니다: {propertyPath}";
                    return false;
                }
            }

            Undo.RecordObjects(serializedObject.targetObjects, undoName);

            for (int i = 0; i < serializedObject.targetObjects.Length; i++)
            {
                properties[i].managedReferenceValue = values[i];
                targets[i].ApplyModifiedProperties();
            }

            serializedObject.Update();
            return true;
        }
    }
}
#endif
