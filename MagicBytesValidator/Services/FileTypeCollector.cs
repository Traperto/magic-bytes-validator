using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MagicBytesValidator.Models;

namespace MagicBytesValidator.Services
{
    public static class FileTypeCollector
    {
        private static readonly IReadOnlyList<FileType> FileTypes = CollectFileTypes();

        public static IReadOnlyList<FileType> CollectFileTypes()
        {
            var assembly = typeof(FileTypeCollector).GetTypeInfo().Assembly;

            return assembly.GetTypes()
                           .Where(t => typeof(FileType).IsAssignableFrom(t))
                           .Where(t => !t.GetTypeInfo().IsAbstract)
                           .Where(t => t.GetConstructors().Any(c => c.GetParameters().Length == 0))
                           .Select(t => Activator.CreateInstance(t))
                           .OfType<FileType>()
                           .ToList();
        }
    }
}