// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddStaticModifierToAllPartialClassDeclarationsRefactoring
    {
        public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol symbol)
        {
            if (symbol.IsClass()
                && symbol.IsStatic
                && !symbol.IsImplicitClass
                && !symbol.IsImplicitlyDeclared)
            {
                ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

                if (syntaxReferences.Length > 1)
                {
                    bool isStatic = false;
                    List<ClassDeclarationSyntax> classDeclarations = null;

                    foreach (SyntaxReference syntaxReference in syntaxReferences)
                    {
                        SyntaxNode node = syntaxReference.GetSyntax(context.CancellationToken);

                        Debug.Assert(node.IsKind(SyntaxKind.ClassDeclaration), node.Kind().ToString());

                        if (node.IsKind(SyntaxKind.ClassDeclaration))
                        {
                            var classDeclaration = (ClassDeclarationSyntax)node;
                            SyntaxTokenList modifiers = classDeclaration.Modifiers;

                            if (modifiers.Contains(SyntaxKind.StaticKeyword))
                            {
                                isStatic = true;
                            }
                            else if (!classDeclaration.ContainsDirectives(modifiers.Span))
                            {
                                if (classDeclarations == null)
                                    classDeclarations = new List<ClassDeclarationSyntax>();

                                classDeclarations.Add(classDeclaration);
                            }
                        }
                    }

                    if (isStatic
                        && classDeclarations != null)
                    {
                        foreach (ClassDeclarationSyntax classDeclaration in classDeclarations)
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.AddStaticModifierToAllPartialClassDeclarations,
                                classDeclaration.Identifier.GetLocation());
                        }
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ClassDeclarationSyntax classDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await InsertModifierRefactoring.RefactorAsync(
                document,
                classDeclaration,
                SyntaxKind.StaticKeyword,
                cancellationToken).ConfigureAwait(false);
        }
    }
}
