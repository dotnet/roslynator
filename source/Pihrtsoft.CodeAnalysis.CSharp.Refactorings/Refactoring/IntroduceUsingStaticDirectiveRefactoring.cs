// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class IntroduceUsingStaticDirectiveRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceUsingStaticDirective)
                && memberAccess.Expression?.IsMissing == false
                && memberAccess.Name?.IsMissing == false
                && context.SupportsSemanticModel)
            {
                memberAccess = GetTopmostMemberAccessExpression(memberAccess);

                if (memberAccess.Expression.Span.Equals(context.Span))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync();

                    var typeSymbol = semanticModel
                        .GetSymbolInfo(memberAccess.Expression, context.CancellationToken)
                        .Symbol as INamedTypeSymbol;

                    if (typeSymbol?.IsStaticClass() == true
                        && (typeSymbol.IsPublic() || typeSymbol.IsInternal())
                        && !IsUsingStaticInScope(memberAccess, typeSymbol, semanticModel, context.CancellationToken))
                    {
                        context.RegisterRefactoring($"Introduce 'using static' for '{typeSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                            cancellationToken =>
                            {
                                return RefactorAsync(
                                    context.Document,
                                    typeSymbol.ToString(),
                                    memberAccess,
                                    cancellationToken);
                            });
                    }
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            string name,
            MemberAccessExpressionSyntax memberAccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SimpleNameSyntax newNode = memberAccess.Name.WithTriviaFrom(memberAccess);

            SyntaxNode newRoot = oldRoot.ReplaceNode(memberAccess, newNode);

            newRoot = ((CompilationUnitSyntax)newRoot)
                .AddUsings(CSharpFactory.UsingStaticDirective(name));

            return document.WithSyntaxRoot(newRoot);
        }

        private static bool IsUsingStaticInScope(
            SyntaxNode node,
            INamedTypeSymbol namedTypeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (node != null)
            {
                if (node.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    var namespaceDeclaration = (NamespaceDeclarationSyntax)node;

                    if (ContainsUsingStatic(namespaceDeclaration.Usings, namedTypeSymbol, semanticModel, cancellationToken))
                        return true;
                }
                else if (node.IsKind(SyntaxKind.CompilationUnit))
                {
                    var compilationUnit = (CompilationUnitSyntax)node;

                    if (ContainsUsingStatic(compilationUnit.Usings, namedTypeSymbol, semanticModel, cancellationToken))
                        return true;
                }

                node = node.Parent;
            }

            return false;
        }

        private static bool ContainsUsingStatic(
            SyntaxList<UsingDirectiveSyntax> usingDirectives,
            INamedTypeSymbol namedTypeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (UsingDirectiveSyntax usingDirective in usingDirectives)
            {
                if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                {
                    ISymbol symbol = semanticModel
                        .GetSymbolInfo(usingDirective.Name, cancellationToken)
                        .Symbol;

                    if (symbol?.IsErrorType() == false
                        && namedTypeSymbol.Equals(symbol))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static MemberAccessExpressionSyntax GetTopmostMemberAccessExpression(MemberAccessExpressionSyntax memberAccess)
        {
            while (memberAccess.Parent?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                memberAccess = (MemberAccessExpressionSyntax)memberAccess.Parent;

            return memberAccess;
        }
    }
}
