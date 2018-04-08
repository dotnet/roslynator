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
    public static class DiagnosticIdentifiersGenerator
    {
        public static CompilationUnitSyntax Generate(IEnumerable<AnalyzerDescriptor> analyzers, bool obsolete, IComparer<string> comparer)
        {
            return CompilationUnit(
                UsingDirectives("System"),
                NamespaceDeclaration("Roslynator.CSharp",
                    ClassDeclaration(
                        Modifiers.PublicStaticPartial(),
                        "DiagnosticIdentifiers",
                        analyzers
                            .Where(f => f.IsObsolete == obsolete)
                            .OrderBy(f => f.Id, comparer)
                            .Select(f =>
                            {
                                FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(
                                    Modifiers.PublicConst(),
                                    PredefinedStringType(),
                                    f.Identifier,
                                    StringLiteralExpression(f.Id));

                                if (f.IsObsolete)
                                    fieldDeclaration = fieldDeclaration.AddObsoleteAttributeIf(f.IsObsolete, error: true);

                                return fieldDeclaration;
                            })
                            .ToSyntaxList<MemberDeclarationSyntax>())));
        }
    }
}
