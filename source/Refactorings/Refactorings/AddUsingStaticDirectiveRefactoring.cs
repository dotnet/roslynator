// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddUsingStaticDirectiveRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddUsingStaticDirective)
                && memberAccess.Expression?.IsMissing == false
                && memberAccess.Name?.IsMissing == false
                && context.SupportsSemanticModel)
            {
                memberAccess = GetTopmostMemberAccessExpression(memberAccess);

                if (context.Span.IsBetweenSpans(memberAccess.Expression))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    var typeSymbol = semanticModel
                        .GetSymbolInfo(memberAccess.Expression, context.CancellationToken)
                        .Symbol as INamedTypeSymbol;

                    if (typeSymbol?.IsStaticClass() == true
                        && (typeSymbol.IsPublic() || typeSymbol.IsInternal())
                        && !SyntaxUtility.IsUsingStaticDirectiveInScope(memberAccess, typeSymbol, semanticModel, context.CancellationToken))
                    {
                        context.RegisterRefactoring($"using static {typeSymbol.ToString()};",
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
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SimpleNameSyntax newNode = memberAccess.Name.WithTriviaFrom(memberAccess);

            SyntaxNode newRoot = oldRoot.ReplaceNode(memberAccess, newNode);

            newRoot = ((CompilationUnitSyntax)newRoot)
                .AddUsings(CSharpFactory.UsingStaticDirective(name));

            return document.WithSyntaxRoot(newRoot);
        }

        private static MemberAccessExpressionSyntax GetTopmostMemberAccessExpression(MemberAccessExpressionSyntax memberAccess)
        {
            while (memberAccess.Parent?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                memberAccess = (MemberAccessExpressionSyntax)memberAccess.Parent;

            return memberAccess;
        }
    }
}
