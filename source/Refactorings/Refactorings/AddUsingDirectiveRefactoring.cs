// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class AddUsingDirectiveRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, NameSyntax name)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            name = GetFullNamespace(name, semanticModel, context.CancellationToken);

            if (name?.Parent?.IsKind(SyntaxKind.QualifiedName, SyntaxKind.SimpleMemberAccessExpression) == true)
            {
                ISymbol symbol = semanticModel
                   .GetSymbolInfo(name, context.CancellationToken)
                   .Symbol;

                if (symbol?.IsNamespace() == true
                    && IsRootNamespace(name, semanticModel, context.CancellationToken))
                {
                    var namespaceSymbol = (INamespaceSymbol)symbol;

                    if (!SyntaxUtility.IsUsingDirectiveInScope(name, namespaceSymbol, semanticModel, context.CancellationToken))
                    {
                        context.RegisterRefactoring(
                            $"using {symbol.ToString()};",
                            cancellationToken =>
                            {
                                return RefactorAsync(
                                    context.Document,
                                    name,
                                    namespaceSymbol,
                                    cancellationToken);
                            });
                    }
                }
            }
        }

        private static NameSyntax GetFullNamespace(
            NameSyntax name,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            bool isFirst = true;

            while (name.Parent?.IsKind(SyntaxKind.QualifiedName) == true)
            {
                var qualifiedName = (QualifiedNameSyntax)name.Parent;

                ISymbol symbol = semanticModel
                    .GetSymbolInfo(qualifiedName, cancellationToken)
                    .Symbol;

                if (symbol?.IsNamespace() == true)
                {
                    if (isFirst
                        && name.IsKind(SyntaxKind.IdentifierName))
                    {
                        return null;
                    }

                    name = qualifiedName;
                }
                else
                {
                    break;
                }

                isFirst = false;
            }

            return name;
        }

        private static bool IsRootNamespace(
            NameSyntax name,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            NameSyntax left = name;

            while (left?.IsKind(SyntaxKind.QualifiedName) == true)
            {
                var qualifiedName = (QualifiedNameSyntax)left;

                if (qualifiedName.Left != null)
                {
                    left = qualifiedName.Left;
                }
                else
                {
                    break;
                }
            }

            ISymbol symbol = semanticModel
                .GetSymbolInfo(left, cancellationToken)
                .Symbol;

            return symbol?.ContainingNamespace.IsGlobalNamespace == true;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            NameSyntax name,
            INamespaceSymbol namespaceSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            root = root.ReplaceNode(name.Parent, GetNewNode(name));

            root = ((CompilationUnitSyntax)root)
                .AddUsings(UsingDirective(ParseName(namespaceSymbol.ToString())));

            return document.WithSyntaxRoot(root);
        }

        private static NameSyntax GetNewNode(NameSyntax name)
        {
            switch (name.Parent.Kind())
            {
                case SyntaxKind.QualifiedName:
                    {
                        var qualifiedName = (QualifiedNameSyntax)name.Parent;

                        return qualifiedName.Right.WithLeadingTrivia(name.GetLeadingTrivia());
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)name.Parent;

                        return memberAccess.Name.WithLeadingTrivia(name.GetLeadingTrivia());
                    }
            }

            return null;
        }
    }
}
