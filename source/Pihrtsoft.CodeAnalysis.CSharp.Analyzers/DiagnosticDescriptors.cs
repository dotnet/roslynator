// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Analyzers
{
    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor ReplaceIsKindMethodInvocation = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.ReplaceIsKindMethodInvocation,
             title: "Replace 'IsKind(SyntaxKind.Element)' with 'IsElement'.",
             messageFormat: "Consider replacing 'IsKind(SyntaxKind.Element)' with 'IsElement'.",
             category: DiagnosticCategories.General,
             defaultSeverity: DiagnosticSeverity.Warning,
             isEnabledByDefault: true
         );

        public static readonly DiagnosticDescriptor AddCodeFileHeader = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.AddCodeFileHeader,
             title: "Add code file header",
             messageFormat: "Consider adding code file header.",
             category: DiagnosticCategories.General,
             defaultSeverity: DiagnosticSeverity.Warning,
             isEnabledByDefault: true
         );
    }
}
