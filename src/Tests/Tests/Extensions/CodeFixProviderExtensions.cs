// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator
{
    internal static class CodeFixProviderExtensions
    {
        public static bool CanFixAny(this CodeFixProvider fixProvider, ImmutableArray<DiagnosticDescriptor> diagnosticDecriptors)
        {
            foreach (string diagnosticId in fixProvider.FixableDiagnosticIds)
            {
                foreach (DiagnosticDescriptor diagnosticDescriptor in diagnosticDecriptors)
                {
                    if (string.Equals(diagnosticId, diagnosticDescriptor.Id, StringComparison.Ordinal))
                        return true;
                }
            }

            return false;
        }
    }
}
