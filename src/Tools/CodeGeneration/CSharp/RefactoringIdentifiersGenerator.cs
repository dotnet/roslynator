// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.CSharp.Refactorings;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class RefactoringIdentifiersGenerator
    {
        public static CompilationUnitSyntax Generate(IEnumerable<RefactoringDescriptor> refactorings, bool obsolete, IComparer<string> comparer)
        {
            return CompilationUnit(
                UsingDirectives("System"),
                NamespaceDeclaration("Roslynator.CSharp.Refactorings",
                    ClassDeclaration(
                        Modifiers.PublicStaticPartial(),
                        "RefactoringIdentifiers",
                        refactorings
                            .Where(f => f.IsObsolete == obsolete)
                            .OrderBy(f => f.Identifier, comparer)
                            .Select(f =>
                            {
                                FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(
                                    Modifiers.PublicConst(),
                                    PredefinedStringType(),
                                    f.Identifier,
                                    AddExpression(IdentifierName("Prefix"), StringLiteralExpression(f.Id.Substring(RefactoringIdentifiers.Prefix.Length))));

                                return fieldDeclaration.AddObsoleteAttributeIf(f.IsObsolete);
                            })
                            .ToSyntaxList<MemberDeclarationSyntax>())));
        }
    }
}
