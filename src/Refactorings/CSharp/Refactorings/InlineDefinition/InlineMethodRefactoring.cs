// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.InlineDefinition
{
    internal class InlineMethodRefactoring : InlineRefactoring<InvocationExpressionSyntax, MethodDeclarationSyntax, IMethodSymbol>
    {
        public InlineMethodRefactoring(
            Document document,
            SyntaxNode node,
            INamedTypeSymbol nodeEnclosingType,
            IMethodSymbol symbol,
            MethodDeclarationSyntax declaration,
            ImmutableArray<ParameterInfo> parameterInfos,
            SemanticModel nodeSemanticModel,
            SemanticModel declarationSemanticModel,
            CancellationToken cancellationToken) : base(document, node, nodeEnclosingType, symbol, declaration, parameterInfos, nodeSemanticModel, declarationSemanticModel, cancellationToken)
        {
        }

        public static Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax node)
        {
            return InlineMethodAnalyzer.Instance.ComputeRefactoringsAsync(context, node);
        }

        public override SyntaxNode BodyOrExpressionBody => Declaration.BodyOrExpressionBody();

        public override ImmutableArray<ITypeSymbol> TypeArguments => Symbol.TypeArguments;
    }
}
