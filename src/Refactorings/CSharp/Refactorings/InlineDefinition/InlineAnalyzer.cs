// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Refactorings.CSharp.Refactorings.InlineDefinition;

namespace Roslynator.CSharp.Refactorings.InlineDefinition
{
    internal abstract class InlineAnalyzer<TNode, TDeclaration, TSymbol>
        where TNode : SyntaxNode
        where TDeclaration : MemberDeclarationSyntax
        where TSymbol : ISymbol
    {
        public async Task ComputeRefactoringsAsync(RefactoringContext context, TNode node)
        {
            if (!ValidateNode(node, context.Span))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            TSymbol symbol = GetMemberSymbol(node, semanticModel, context.CancellationToken);

            if (EqualityComparer<TSymbol>.Default.Equals(symbol, default(TSymbol)))
                return;

            TDeclaration declaration = await GetMemberDeclarationAsync(symbol, context.CancellationToken).ConfigureAwait(false);

            if (declaration == null)
                return;

            for (SyntaxNode parent = node.Parent; parent != null; parent = parent.Parent)
            {
                if (object.ReferenceEquals(declaration, parent))
                    return;
            }

            (ExpressionSyntax expression, SyntaxList<StatementSyntax> statements) = GetExpressionOrStatements(declaration);

            SyntaxNode nodeIncludingConditionalAccess = node.WalkUp(f => f.IsKind(SyntaxKind.ConditionalAccessExpression));

            if (expression != null
                || (statements.Any() && nodeIncludingConditionalAccess.IsParentKind(SyntaxKind.ExpressionStatement)))
            {
                ImmutableArray<ParameterInfo> parameterInfos = GetParameterInfos(node, symbol);

                if (parameterInfos.IsDefault)
                    return;

                INamedTypeSymbol enclosingType = semanticModel.GetEnclosingNamedType(node.SpanStart, context.CancellationToken);

                SemanticModel declarationSemanticModel = semanticModel;

                if (node.SyntaxTree != declaration.SyntaxTree)
                {
                    Document document = context.Solution.GetDocument(declaration.SyntaxTree);

                    // https://github.com/dotnet/roslyn/issues/5260
                    if (document == null)
                        return;

                    declarationSemanticModel = await document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
                }

                SingleInlineRefactoring<TNode, TDeclaration, TSymbol> singleRefactoring =
                    CreateSingleRefactoring(
                        context.Document,
                        nodeIncludingConditionalAccess,
                        enclosingType,
                        symbol,
                        declaration,
                        parameterInfos,
                        semanticModel,
                        declarationSemanticModel,
                        context.CancellationToken);

                AllInlineRefactoring<TNode, TDeclaration, TSymbol> allRefactoring =
                    CreateAllRefactoring(
                        symbol,
                        declaration,
                        declarationSemanticModel,
                        context.CancellationToken);

                string title = CSharpFacts.GetTitle(declaration);

                if (expression != null)
                {
                    context.RegisterRefactoring(
                        $"Inline {title}",
                        cancellationToken =>
                            singleRefactoring.InlineAsync(nodeIncludingConditionalAccess, expression, cancellationToken),
                        GetEquivalenceKey());
                }
                else
                {
                    var expressionStatement = (ExpressionStatementSyntax)nodeIncludingConditionalAccess.Parent;

                    context.RegisterRefactoring(
                        $"Inline {title}",
                        cancellationToken =>
                            singleRefactoring.InlineAsync(expressionStatement, statements, cancellationToken),
                        GetEquivalenceKey());
                }

                context.RegisterRefactoring(
                    $"Inline and remove {title}",
                    cancellationToken =>
                        allRefactoring.InlineAndRemoveAsync(context.Solution, cancellationToken),
                    EquivalenceKey.Join(GetEquivalenceKey(), "Remove"));
            }
        }

        protected abstract string GetEquivalenceKey();

        protected abstract bool ValidateNode(TNode node, TextSpan span);

        protected abstract Task<TDeclaration> GetMemberDeclarationAsync(TSymbol symbol, CancellationToken cancellationToken);

        protected abstract TSymbol GetMemberSymbol(TNode node, SemanticModel semanticModel, CancellationToken cancellationToken);

        protected abstract ImmutableArray<ParameterInfo> GetParameterInfos(TNode node, TSymbol symbol);

        protected abstract (ExpressionSyntax expression, SyntaxList<StatementSyntax> statements) GetExpressionOrStatements(TDeclaration declaration);

        protected abstract SingleInlineRefactoring<TNode, TDeclaration, TSymbol> CreateSingleRefactoring(
            Document document,
            SyntaxNode node,
            INamedTypeSymbol nodeEnclosingType,
            TSymbol symbol,
            TDeclaration declaration,
            ImmutableArray<ParameterInfo> parameterInfos,
            SemanticModel nodeSemanticModel,
            SemanticModel declarationSemanticModel,
            CancellationToken cancellationToken);

        protected abstract AllInlineRefactoring<TNode, TDeclaration, TSymbol> CreateAllRefactoring(
            TSymbol symbol,
            TDeclaration declaration,
            SemanticModel declarationSemanticModel,
            CancellationToken cancellationToken);
    }
}
