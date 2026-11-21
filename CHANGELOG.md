# Changelog

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
