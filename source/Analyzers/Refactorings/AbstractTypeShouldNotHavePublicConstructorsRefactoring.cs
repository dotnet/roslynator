// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AbstractTypeShouldNotHavePublicConstructorsRefactoring
    {
        public static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            if (constructorDeclaration.Modifiers.Contains(SyntaxKind.PublicKeyword))
            {
                SyntaxNode parent = constructorDeclaration.Parent;

                if (parent?.IsKind(SyntaxKind.ClassDeclaration) == true)
                {
                    var classDeclaration = (ClassDeclarationSyntax)parent;

                    SyntaxTokenList modifiers = classDeclaration.Modifiers;

                    bool isAbstract = modifiers.Contains(SyntaxKind.AbstractKeyword);

                    if (!isAbstract
                        && modifiers.Contains(SyntaxKind.PartialKeyword))
                    {
                        INamedTypeSymbol classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);

                        if (classSymbol != null)
                            isAbstract = classSymbol.IsAbstract;
                    }

                    if (isAbstract)
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.AbstractTypeShouldNotHavePublicConstructors,
                            constructorDeclaration.Identifier);
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ConstructorDeclarationSyntax constructor,
            CancellationToken cancellationToken)
        {
            SyntaxTokenList modifiers = constructor.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.PublicKeyword);

            Debug.Assert(index != -1, modifiers.ToString());

            if (index != -1)
            {
                SyntaxToken modifier = modifiers[index];

                SyntaxToken newModifier = CSharpFactory.ProtectedKeyword().WithTriviaFrom(modifier);

                return await document.ReplaceTokenAsync(modifier, newModifier, cancellationToken).ConfigureAwait(false);
            }

            return document;
        }
    }
}
