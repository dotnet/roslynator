// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static partial class ConfigOptions
    {
        private static readonly ImmutableDictionary<string, string> _requiredOptions = GetRequiredOptions().ToImmutableDictionary(f => f.Key, f => f.Value);

        public static string GetRequiredOptions(DiagnosticDescriptor descriptor)
        {
            Debug.Assert(_requiredOptions.ContainsKey(descriptor.Id), descriptor.Id);

            return _requiredOptions.GetValueOrDefault(descriptor.Id);
        }

        private static string JoinOptionKeys(params string[] values)
        {
            return string.Join(", ", values);
        }
    }
}