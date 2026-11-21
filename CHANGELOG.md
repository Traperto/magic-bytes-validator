# Changelog

## 3.0.0

This release contains breaking changes. Update to 2.6.0 first and fix all obsolete warnings: 2.6.0 already
contains all replacements, so most of the update is then done. The sections below describe what changed compared
to 2.6.0 and how to update your code.

### Removed

The APIs deprecated in 2.6.0 were removed:

| Removed                                               | Replacement                                                                     |
| ----------------------------------------------------- | ------------------------------------------------------------------------------- |
| `FormFileTypeProvider.FindFileTypeForFormFile`        | `FindValidatedTypeAsync`                                                        |
| `StreamFileTypeProvider.FindByMagicByteSequenceAsync` | `TryFindUnambiguousAsync`                                                       |
| `FileTypeCollector.CollectFileTypes`                  | `FileTypeCollector.CollectFileTypesForAssembly` or `Mapping.Register(assembly)` |
| `EnumerableExtensions.AsIndexed`                      | `Enumerable.Select((value, index) => (value, index))`                           |
| `FileByteFilter.ByteCheck.ByteArray`                  | `FileByteFilter.ByteCheck.Bytes`                                                |

### Changed APIs

- The `Mapping` properties and constructor parameters of `Validator` and `FormFileTypeProvider` (and their
  interfaces) are typed as `IMapping` instead of `Mapping`.
- `IFileType.MimeTypes` and `IFileType.Extensions` are `IReadOnlyList<string>` instead of `string[]`.
  Custom implementations of `IFileType` have to be adjusted.
- `IStreamFileTypeProvider` has a `Mapping` property. `FindAllMatchesAsync`, `FindCloseMatchesAsync` and
  `TryFindUnambiguousAsync` take `validationType` as optional parameter instead of offering overloads, and
  `FindAllMatchesAsync` and `FindCloseMatchesAsync` return `IReadOnlyList<IFileType>` instead of
  `IEnumerable<IFileType>`. Callers usually don't need to change anything, custom implementations of the
  interface have to be adjusted.
- `FileByteFilter.ByteCheck` is sealed and `Offset` is a read-only property instead of a field. Recompile code
  that uses it.
- `ArgumentEmptyException` derives from `ArgumentException` instead of `ArgumentNullException` and sets
  `ParamName`. `null` arguments throw an `ArgumentNullException` instead. If you catch `ArgumentNullException`
  to handle empty values, catch `ArgumentEmptyException` (or `ArgumentException`) as well.

### Changed behaviour

- A `FileByteFilter` is frozen automatically when it is used for the first time (in addition to calling
  `Freeze()`). Afterwards its checks can't be changed anymore and further configuration throws an
  `InvalidOperationException`. Configure custom file types completely in their constructor.
- MIME types, extensions and byte patterns are copied when a file type is created or a check is configured, so
  changing the original arrays afterwards has no effect.

## 2.6.0

This release is fully compatible with 2.5.0. It adds the APIs that replace the ones that will be removed or changed
in 3.0 and marks the old ones as obsolete. Fix all obsolete warnings to prepare for the update to 3.0.

### Added

- `services.AddMagicBytesValidator()` registers `IMapping`, `IValidator`, `IStreamFileTypeProvider` and
  `IFormFileTypeProvider` as singletons that share the same mapping. Custom file types can be registered via
  `services.AddMagicBytesValidator(mapping => mapping.Register(new CustomType()))`.
- `FileByteFilter.Freeze()` and `FileByteFilter.IsFrozen`: a frozen filter rejects further configuration with an
  `InvalidOperationException` and can safely be shared between threads.
- `FileByteFilter.ByteCheck.Bytes` as read-only replacement for `ByteArray`.
- `StreamFileTypeProvider` exposes its mapping via a `Mapping` property. The mapping constructor parameter is
  optional now and defaults to a new `Mapping`.
- Overloads of `FindAllMatchesAsync`, `FindCloseMatchesAsync` and `TryFindUnambiguousAsync` with a
  `validationType` parameter. `IStreamFileTypeProvider` provides default implementations, so existing
  implementations of the interface keep working (they support `FileByteType.Strict` only).

### Deprecated

These APIs will be removed in 3.0:

| Deprecated                                            | Replacement                                                                     |
| ----------------------------------------------------- | ------------------------------------------------------------------------------- |
| `FormFileTypeProvider.FindFileTypeForFormFile`        | `FindValidatedTypeAsync`                                                        |
| `StreamFileTypeProvider.FindByMagicByteSequenceAsync` | `TryFindUnambiguousAsync`                                                       |
| `FileTypeCollector.CollectFileTypes`                  | `FileTypeCollector.CollectFileTypesForAssembly` or `Mapping.Register(assembly)` |
| `EnumerableExtensions.AsIndexed`                      | `Enumerable.Select((value, index) => (value, index))`                           |
| `FileByteFilter.ByteCheck.ByteArray`                  | `FileByteFilter.ByteCheck.Bytes`                                                |

### Upcoming changes in 3.0

The following changes can't be made without breaking compatibility and are therefore postponed to 3.0:

- The `Mapping` properties and constructor parameters of `Validator` and `FormFileTypeProvider` (and their
  interfaces) will be typed as `IMapping`. Only use members of `IMapping` to be prepared.
- `IFileType.MimeTypes` and `IFileType.Extensions` will be `IReadOnlyList<string>` instead of `string[]`.
- A `FileByteFilter` will be frozen automatically when it is used for the first time. Configure custom file types
  completely in their constructor.
- `ArgumentEmptyException` will derive from `ArgumentException` instead of `ArgumentNullException`.
- `IStreamFileTypeProvider` will get a `Mapping` property, and its methods will return `IReadOnlyList<IFileType>`
  and take `validationType` as optional parameter instead of offering overloads.
- `FileByteFilter.ByteCheck` will be sealed and `Offset` will become a read-only property instead of a field.
