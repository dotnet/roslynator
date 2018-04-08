// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class CompilerDiagnosticIdentifiersGenerator
    {
        public static CompilationUnitSyntax Generate(IEnumerable<CompilerDiagnosticDescriptor> descriptors, IComparer<string> comparer)
        {
            return CompilationUnit(
                UsingDirectives(),
                NamespaceDeclaration(
                    "Roslynator.CSharp",
                    ClassDeclaration(
                        Modifiers.InternalStatic(),
                        "CompilerDiagnosticIdentifiers",
                        descriptors
                            .OrderBy(f => f.Id, comparer)
                            .Select(f =>
                            {
                                return FieldDeclaration(
                                   Modifiers.PublicConst(),
                                   PredefinedStringType(),
                                   f.Identifier,
                                   StringLiteralExpression(f.Id));
                            })
                            .ToSyntaxList<MemberDeclarationSyntax>())));
        }
    }
}
