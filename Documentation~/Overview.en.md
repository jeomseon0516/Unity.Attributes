# Jeomseon Unity Attributes

[한국어](Overview.md) · [Installation](../README.en.md)

## Purpose

Configures Inspector display, constraints, references, invocation, and SerializeReference construction with attributes; declarations and Editor implementations are separate assemblies.

## Usage flow

Annotate fields/methods, edit or invoke in the Inspector, then apply through SerializedProperty/Undo. Confirm parameterized construction in its popup before assignment.

## Contracts and limitations

ReadOnly is not runtime access control. Change callbacks observe the Undo editing stream. Every constructor argument needs a supported input pipeline.

## Package requirements

- Package ID: `com.jeomseon.unity.attributes`
- Version: `0.4.2`
- Unity: `6000.6` / `0f1`
- `com.jeomseon.unity.editor-toolkit`: `0.7.2`
- `com.jeomseon.unity.core`: `0.3.2`

## Samples and verification

Import the following from Package Manager > Samples. Open the included Scene or Editor tool; follow any Setup step in its sample README.

- [Basic Usage](../Samples~/BasicUsage/README.md)
  - [InspectorButtonSample](../Samples~/BasicUsage/InspectorButtonSample.unity)
  - [ObjectAssignmentSample](../Samples~/BasicUsage/ObjectAssignmentSample.unity)
  - [SerializeReferenceSelectorSample](../Samples~/BasicUsage/SerializeReferenceSelectorSample.unity)

[Test sources](../Tests)

Add the package ID to manifest `testables`, then run the relevant Unity Test Runner assemblies. These links describe available tests, not a new successful test run.

## Source map

- [EditorMethodTriggerAttribute](../Runtime/EditorMethodTriggerAttribute.cs)
- [GetOrAddComponentAttribute](../Runtime/GetOrAddComponentAttribute.cs)
- [HierarchyObjectPickerAttribute](../Runtime/HierarchyObjectPickerAttribute.cs)
- [InfoBoxAttribute, InfoBoxType](../Runtime/InfoBoxAttribute.cs)
- [InspectorButtonAttribute](../Runtime/InspectorButtonAttribute.cs)
- [InvokeOnInspectorChangeAttribute](../Runtime/InvokeOnInspectorChangeAttribute.cs)
- [MaxValueAttribute](../Runtime/MaxValueAttribute.cs)
- [ReadOnlyAttribute](../Runtime/ReadOnlyAttribute.cs)
- [SerializeReferenceSelectorAttribute](../Runtime/SerializeReferenceSelectorAttribute.cs)
- [SpritePreviewAttribute](../Runtime/SpritePreviewAttribute.cs)
- [Vector2SliderAttribute](../Runtime/Vector2SliderAttribute.cs)
- [ISerializeReferenceSelectorConstructorParameterPipeline](../Editor/ConstructorPipelines/ISerializeReferenceSelectorConstructorParameterPipeline.cs)
- [SerializeReferenceSelectorConstructorPipelineNameAttribute](../Editor/ConstructorPipelines/SerializeReferenceSelectorConstructorPipelineNameAttribute.cs)
- [InspectorButtonGUI](../Editor/InspectorButtonGUI.cs)

[Usage guide](Guide.en.md)
