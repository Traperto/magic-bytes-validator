using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MagicBytesValidator.Models;

namespace MagicBytesValidator.Services
{
    public static class FileTypeCollector
    {
        public static IEnumerable<FileType> CollectFileTypes(Assembly? assembly = null)
        {
            assembly ??= typeof(FileTypeCollector).GetTypeInfo().Assembly;

            return assembly.GetTypes()
                           .Where(t => typeof(FileType).IsAssignableFrom(t))
                           .Where(t => !t.GetTypeInfo().IsAbstract)
                           .Where(t => t.GetConstructors().Any(c => c.GetParameters().Length == 0))
                           .Select(Activator.CreateInstance)
                           .OfType<FileType>()
                           .ToList();
        }
    }
}