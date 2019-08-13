// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class CodeFixIdentifiersGenerator
    {
        public static CompilationUnitSyntax Generate(IEnumerable<CodeFixMetadata> codeFixes, IComparer<string> comparer)
        {
            return CompilationUnit(
                UsingDirectives("Roslynator.CodeFixes"),
                NamespaceDeclaration(
                    "Roslynator.CSharp",
                    ClassDeclaration(
                        Modifiers.Public_Static_Partial(),
                        "CodeFixIdentifiers",
                        codeFixes
                            .Where(f => !f.IsObsolete)
                            .OrderBy(f => f.Id, comparer)
                            .Select(f =>
                            {
                                return FieldDeclaration(
                                   Modifiers.Public_Const(),
                                   PredefinedStringType(),
                                   f.Identifier,
                                   AddExpression(SimpleMemberAccessExpression(IdentifierName("CodeFixIdentifier"), IdentifierName("CodeFixIdPrefix")), StringLiteralExpression(f.Id.Substring(CodeFixIdentifier.CodeFixIdPrefix.Length))));
                            })
                            .ToSyntaxList<MemberDeclarationSyntax>())));
        }
    }
}
