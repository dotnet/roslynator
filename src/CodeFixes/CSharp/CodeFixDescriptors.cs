// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp
{
    public static partial class CodeFixDescriptors
    {
        internal static IEnumerable<string> GetCodeFixesDisabledByDefault()
        {
            return typeof(CodeFixDescriptors)
                .GetFields()
                .Where(f => f.IsPublic)
                .Select(f => (CodeFixDescriptor)f.GetValue(null))
                .Where(f => !f.IsEnabledByDefault)
                .SelectMany(f => f.FixableDiagnosticIds.Select(g => $"{g}.{f.Id}"))
                .ToList();
        }
    }
}