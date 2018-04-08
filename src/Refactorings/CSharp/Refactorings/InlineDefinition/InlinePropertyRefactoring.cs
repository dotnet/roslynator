// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.InlineDefinition
{
    internal class InlinePropertyRefactoring : InlineRefactoring<IdentifierNameSyntax, PropertyDeclarationSyntax, IPropertySymbol>
    {
        public InlinePropertyRefactoring(
            Document document,
            SyntaxNode node,
            INamedTypeSymbol nodeEnclosingType,
            IPropertySymbol symbol,
            PropertyDeclarationSyntax declaration,
            ImmutableArray<ParameterInfo> parameterInfos,
            SemanticModel invocationSemanticModel,
            SemanticModel declarationSemanticModel,
            CancellationToken cancellationToken) : base(document, node, nodeEnclosingType, symbol, declaration, parameterInfos, invocationSemanticModel, declarationSemanticModel, cancellationToken)
        {
        }

        public static Task ComputeRefactoringsAsync(RefactoringContext context, IdentifierNameSyntax node)
        {
            return InlinePropertyAnalyzer.Instance.ComputeRefactoringsAsync(context, node);
        }

        public override Task<Document> InlineAsync(
            SyntaxNode node,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            node = GetNodeToReplace(node);

            return base.InlineAsync(node, expression, cancellationToken);
        }

        public override Task<Solution> InlineAndRemoveAsync(
            SyntaxNode node,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            node = GetNodeToReplace(node);

            return base.InlineAndRemoveAsync(node, expression, cancellationToken);
        }

        private static SyntaxNode GetNodeToReplace(SyntaxNode node)
        {
            if (node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var memberAccessExpression = (MemberAccessExpressionSyntax)node.Parent;

                if (object.ReferenceEquals(node, memberAccessExpression.Name))
                    node = memberAccessExpression;
            }

            return node;
        }

        public override SyntaxNode BodyOrExpressionBody
        {
            get { return Declaration.ExpressionBody ?? Declaration.AccessorList.Accessors[0].BodyOrExpressionBody(); }
        }

        public override ImmutableArray<ITypeSymbol> TypeArguments
        {
            get { return ImmutableArray<ITypeSymbol>.Empty; }
        }
    }
}
