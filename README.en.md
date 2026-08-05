# Jeomseon Unity Attributes

Reusable inspector attributes with their editor implementations for Unity projects.

## Installation

```json
{
  "dependencies": {
    "com.jeomseon.unity.attributes": "0.3.0"
  }
}
```

## Included APIs

- Display and editing: `InfoBox`, `ReadOnly`, `SpritePreview`, `Vector2Slider`
- Value constraint: `MaxValue` (use Unity's built-in `Min`)
- Object assignment: `GetOrAddComponent`, `HierarchyObjectPicker`
- Method invocation: `InspectorButton`, `InvokeOnInspectorChange`

`InvokeOnInspectorChange` delays a parameterless method call when the Inspector or an
editor tool changes a serialized field through Undo. It is not a runtime value-change event.

Use Unity's built-in `MinAttribute` for a minimum-only numeric constraint. This package
provides `MaxValueAttribute` for the corresponding maximum-only constraint while keeping
the standard numeric field UI.

## Package boundaries

- This package owns both declarations and editor implementations for feature-independent inspector attributes.
- It has a one-way dependency on `com.jeomseon.unity.editor-toolkit` for shared editor APIs.
- Feature-specific attributes belong to their feature packages. For example,
  `LocalizedStringAttribute` belongs to `com.jeomseon.unity.localization`, and
  `ResetOnPoolReleaseAttribute` belongs to `com.jeomseon.unity.game-object-pooling`.
- The Runtime asmdef does not reference Editor assemblies; editor dependencies are isolated to the Editor asmdef.
