﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Metadata;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp;

public static class CompilerDiagnosticIdentifiersGenerator
{
    public static CompilationUnitSyntax Generate(IEnumerable<CompilerDiagnosticMetadata> compilerDiagnostics, IComparer<string> comparer)
    {
        return CompilationUnit(
            UsingDirectives(),
            NamespaceDeclaration(
                "Roslynator.CSharp",
                ClassDeclaration(
                    Modifiers.Internal_Static(),
                    "CompilerDiagnosticIdentifiers",
                    compilerDiagnostics
                        .OrderBy(f => f.Id, comparer)
                        .Select(f =>
                        {
                            return FieldDeclaration(
                                Modifiers.Public_Const(),
                                PredefinedStringType(),
                                $"{f.Id}_{f.Identifier}",
                                StringLiteralExpression(f.Id));
                        })
                        .ToSyntaxList<MemberDeclarationSyntax>())));
    }
}
