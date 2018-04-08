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
            if (memberAccess.Expression?.IsMissing != false)
                return;

            if (memberAccess.Name?.IsMissing != false)
                return;

            memberAccess = GetTopmostMemberAccessExpression(memberAccess);

            if (!context.Span.IsBetweenSpans(memberAccess.Expression))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            var typeSymbol = semanticModel.GetSymbol(memberAccess.Expression, context.CancellationToken) as INamedTypeSymbol;

            if (typeSymbol?.TypeKind != TypeKind.Class)
                return;

            if (!typeSymbol.IsStatic)
                return;

            if (!typeSymbol.DeclaredAccessibility.Is(Accessibility.Public, Accessibility.Internal))
                return;

            if (CSharpUtility.IsStaticClassInScope(memberAccess, typeSymbol, semanticModel, context.CancellationToken))
                return;

            context.RegisterRefactoring(
                $"using static {typeSymbol};",
                ct => RefactorAsync(context.Document, typeSymbol.ToString(), memberAccess, ct));
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

            newRoot = ((CompilationUnitSyntax)newRoot).AddUsings(CSharpFactory.UsingStaticDirective(SyntaxFactory.ParseName(name)));

            return document.WithSyntaxRoot(newRoot);
        }

        private static MemberAccessExpressionSyntax GetTopmostMemberAccessExpression(MemberAccessExpressionSyntax memberAccess)
        {
            while (memberAccess.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                memberAccess = (MemberAccessExpressionSyntax)memberAccess.Parent;

            return memberAccess;
        }
    }
}
