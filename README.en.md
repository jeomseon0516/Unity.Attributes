# Jeomseon Unity Attributes

[한국어](README.md) | English

Configures Inspector display, constraints, references, invocation, and SerializeReference construction with attributes; declarations and Editor implementations are separate assemblies.

## Quick start

Annotate fields/methods, edit or invoke in the Inspector, then apply through SerializedProperty/Undo. Confirm parameterized construction in its popup before assignment.

Complete sample: [AttributesSample.cs](Samples~/BasicUsage/AttributesSample.cs).

```csharp
using Jeomseon.Unity.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jeomseon.Samples.Attributes
{
    public sealed class AttributesSample : MonoBehaviour
    {
        [InfoBox("Attributes 패키지가 Attribute Drawer를 함께 제공합니다.")]
        [InspectorName("표시 이름")]
        [SerializeField, FormerlySerializedAs("_message")] private string message = "Hello";

        [ReadOnly]
        [SerializeField, FormerlySerializedAs("_changeCount")] private int changeCount;

        [Min(0f)]
        [SerializeField, FormerlySerializedAs("_minimumBoundedValue")] private float minimumBoundedValue;

        [MaxValue(10)]
        [SerializeField, FormerlySerializedAs("_maximumBoundedValue")] private int maximumBoundedValue;

        [InvokeOnInspectorChange(nameof(message))]
        private void OnMessageChanged()
        {
            changeCount++;
            Debug.Log($"값 변경 콜백: {message}");
        }

        [InspectorButton("메시지 출력")]
        private void PrintMessage()
        {
            Debug.Log(message);
        }
    }
}
```

## Contracts and limitations

ReadOnly is not runtime access control. Change callbacks observe the Undo editing stream. Every constructor argument needs a supported input pipeline.

## Requirements

- Unity `6000.6.0f1`
- Package: `com.jeomseon.unity.attributes` `0.4.2`
- `com.jeomseon.unity.editor-toolkit`: `0.7.2`
- `com.jeomseon.unity.core`: `0.3.2`

## Installation

Merge this into your project's `Packages/manifest.json`; preserve existing dependencies and registries.

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.jeomseon"
      ]
    }
  ],
  "dependencies": {
    "com.jeomseon.unity.attributes": "0.4.2"
  }
}
```

Or use **Install package from git URL** in Package Manager:

```text
https://github.com/jeomseon0516/Unity.Attributes.git#v0.4.2
```

## Design and behavior

### Scope and APIs

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

### Example

Import `Jeomseon.Unity.Attributes`. [AttributesSample.cs](Samples~/BasicUsage/AttributesSample.cs)
demonstrates the complete combination of display, constraints, buttons, and change callbacks.

```csharp
[InfoBox("Edit a value no larger than 10.")]
[MaxValue(10)]
[UnityEngine.SerializeField] private int count;

[InvokeOnInspectorChange(nameof(count))]
private void OnCountEdited() => UnityEngine.Debug.Log(count);
```

Import the [Basic Usage scenes](Samples~/BasicUsage/README.md) for working editor examples.

### SerializeReference flow

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

### Source and verification

Read [declarations](Runtime), [Selector](Editor/SerializeReferenceSelectorDrawer.cs),
[constructor selection](Editor/ConstructorSelectionService.cs), then
[input pipelines](Editor/ConstructorPipelines).
[Editor tests](Tests/Editor) cover construction, assignment, restore, and editor contracts.
Follow the [sample checklist](Samples~/BasicUsage/README.md) for save/reopen, independent multi-object
instances, exceptions, cancellation, Undo, and Assembly/Domain Reload button behavior.

## API usage and details

### Included APIs

- Display and editing: `InfoBox`, `ReadOnly`, `SpritePreview`, `Vector2Slider`
- Value constraint: `MaxValue` (use Unity's built-in `Min`)
- Object assignment: `GetOrAddComponent`, `HierarchyObjectPicker`
- Polymorphic references: `SerializeReferenceSelector` (concrete type selection for single
  `SerializeReference` fields and lists)
- Method invocation: `InspectorButton`, `InvokeOnInspectorChange`

### Sample verification

`Samples~/BasicUsage/AttributesSample.cs` demonstrates declarations for `InfoBox`, `MaxValue`,
and `ReadOnly`. Runnable scenes are `InspectorButtonSample`, `ObjectAssignmentSample`, and
`SerializeReferenceSelectorSample`: they cover buttons/change callbacks, assignment/Undo, and
polymorphic construction/save/restore. See the [sample checklist](Samples~/BasicUsage/README.md).

For Injection regression checks:

1. Enable Injection under `Project Settings > Jeomseon > Attributes` and confirm that one InspectorButton is shown.
2. Disable Injection and confirm that one Inspector Header fallback button is shown.
3. Repeat with Assembly Reload and Domain Reload enabled and disabled, checking for duplicate buttons, leaks, or failed reactivation.

`InvokeOnInspectorChange` delays a parameterless method call when the Inspector or an
editor tool changes a serialized field through Undo. It is not a runtime value-change event.

Use `SerializeReferenceSelector` with `[SerializeReference]`. Choosing `(None)` assigns `null`.
A concrete type is selectable when every parameter of a public constructor has an input pipeline.
For parameterized constructors, choose the overload and arguments in the popup, then create the
instance. Canceling or a constructor exception leaves the existing field value unchanged.

Built-in inputs cover `int`, `float`, `double`, `bool`, `string`, enums, `UnityEngine.Object`
subclasses, `Color`, `Vector2`, `Vector3`, and `Vector4`. Implement
`ISerializeReferenceSelectorConstructorParameterPipeline` in an Editor assembly for custom inputs.

Use Unity's built-in `MinAttribute` for a minimum-only numeric constraint. This package
provides `MaxValueAttribute` for the corresponding maximum-only constraint while keeping
the standard numeric field UI.

### Package boundaries

- This package owns both declarations and editor implementations for feature-independent inspector attributes.
- It has a one-way dependency on `com.jeomseon.unity.editor-toolkit` for shared editor APIs.
- Feature-specific attributes belong to their feature packages. For example,
  `LocalizedStringAttribute` belongs to `com.jeomseon.unity.localization`, and
  `ResetOnPoolReleaseAttribute` belongs to `com.jeomseon.unity.game-object-pooling`.
- The Runtime asmdef does not reference Editor assemblies; editor dependencies are isolated to the Editor asmdef.

## Samples

Import the desired entry from **Package Manager > Samples**. Use the included Scene/Editor tool and complete its Setup step first, if required.

- [Basic Usage](Samples~/BasicUsage/README.md)
  - [InspectorButtonSample](Samples~/BasicUsage/InspectorButtonSample.unity)
  - [ObjectAssignmentSample](Samples~/BasicUsage/ObjectAssignmentSample.unity)
  - [SerializeReferenceSelectorSample](Samples~/BasicUsage/SerializeReferenceSelectorSample.unity)

## Tests

[Test sources](Tests)

Add the package ID to manifest `testables`, then execute its relevant EditMode/PlayMode assemblies in Unity Test Runner.

## Further reading

[Complete source map and package metadata](Documentation~/Overview.en.md)
- [Changelog](CHANGELOG.md)

## License

[License](LICENSE.md)
