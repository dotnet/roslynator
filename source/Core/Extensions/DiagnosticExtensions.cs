// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class DiagnosticExtensions
    {
        public static bool HasTag(this Diagnostic diagnostic, string tag)
        {
            return diagnostic?.Descriptor.CustomTags.Contains(tag) == true;
        }

        public static bool IsCompilerDiagnostic(this Diagnostic diagnostic)
        {
            return HasTag(diagnostic, WellKnownDiagnosticTags.Compiler);
        }
    }
}
