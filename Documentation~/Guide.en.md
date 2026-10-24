# Attributes: usage and design

[한국어](Guide.md) · [Installation](../README.en.md)

## Scope and APIs

Attributes configure repeated Inspector display, reference assignment, method invocation, and
polymorphic construction. Declarations belong to Runtime; drawers, discovery, reflection, and Undo
belong to Editor. Domain-specific attributes belong to their feature packages.

| Task | API | Contract |
| --- | --- | --- |
| Messages, previews, read-only display | `InfoBox`, `SpritePreview`, `ReadOnly` | Inspector presentation, not runtime access control |
| Numeric editing | `MaxValue`, `Vector2Slider` | Use Unity Min for minimum-only constraints |
| Reference assignment | `GetOrAddComponent`, `HierarchyObjectPicker` | Check Scene/Prefab references and Undo |
| Buttons | `InspectorButton` | Distinguish editor actions from runtime initialization |
| Editor change callbacks | `InvokeOnInspectorChange` | Undo change stream; parameterless methods |
| Polymorphic construction | `SerializeReferenceSelector` | Requires Unity SerializeReference |

ReadOnly does not prevent code mutation. InvokeOnInspectorChange does not replace runtime setters
or events. Inspector Injection and Header fallback are alternative paths that avoid duplicate buttons.

## Example

Import `Jeomseon.Unity.Attributes`. [AttributesSample.cs](../Samples~/BasicUsage/AttributesSample.cs)
demonstrates the complete combination of display, constraints, buttons, and change callbacks.

```csharp
[InfoBox("Edit a value no larger than 10.")]
[MaxValue(10)]
[UnityEngine.SerializeField] private int count;

[InvokeOnInspectorChange(nameof(count))]
private void OnCountEdited() => UnityEngine.Debug.Log(count);
```

Import the [Basic Usage scenes](../Samples~/BasicUsage/README.md) for working editor examples.

## SerializeReference flow

The drawer resolves the declared base type and discovers constructible concrete types. Simple
parameterless creation can assign directly; parameterized creation opens a constructor popup.
Every parameter needs a supported input pipeline. On confirmation, each selected target receives
its own new instance through Undo-aware assignment. Errors and cancellation preserve the old value;
`(None)` explicitly assigns null.

Built-in inputs cover int, float, double, bool, string, enum, Unity Object, Color, and Vector2/3/4.
Constructors with unsupported parameters are excluded. Extend
`ISerializeReferenceSelectorConstructorParameterPipeline` in an Editor assembly: concrete implementations
with public parameterless constructors are discovered using TypeCache. Implement compatibility,
default values, and field drawing; the optional naming attribute supplies the display name.

## Source and verification

Read [declarations](../Runtime), [Selector](../Editor/SerializeReferenceSelectorDrawer.cs),
[constructor selection](../Editor/ConstructorSelectionService.cs), then
[input pipelines](../Editor/ConstructorPipelines).
[Editor tests](../Tests/Editor) cover construction, assignment, restore, and editor contracts.
Follow the [sample checklist](../Samples~/BasicUsage/README.md) for save/reopen, independent multi-object
instances, exceptions, cancellation, Undo, and Assembly/Domain Reload button behavior.
