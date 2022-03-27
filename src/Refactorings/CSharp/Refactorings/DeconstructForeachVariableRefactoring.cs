// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DeconstructForeachVariableRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            ForEachStatementSyntax forEachStatement,
            SemanticModel semanticModel)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(forEachStatement.Type);

            IMethodSymbol deconstructSymbol = typeSymbol.FindMember<IMethodSymbol>(
                "Deconstruct",
                symbol =>
                {
                    if (symbol.DeclaredAccessibility == Accessibility.Public)
                    {
                        ImmutableArray<IParameterSymbol> parameters = symbol.Parameters;

                        return parameters.Any()
                            && parameters.All(f => f.RefKind == RefKind.Out);
                    }

                    return false;
                });

            if (deconstructSymbol is null)
                return;

            ISymbol foreachSymbol = semanticModel.GetDeclaredSymbol(forEachStatement, context.CancellationToken);

            if (foreachSymbol?.IsKind(SymbolKind.Local) != true)
                return;

            var walker = new DeconstructForeachVariableWalker(
                deconstructSymbol,
                foreachSymbol,
                forEachStatement.Identifier.ValueText,
                semanticModel,
                context.CancellationToken);

            walker.Visit(forEachStatement.Statement);

            if (!walker.Success)
                return;

            context.RegisterRefactoring(
                "Deconstruct foreach variable",
                ct => RefactorAsync(context.Document, forEachStatement, deconstructSymbol, foreachSymbol, semanticModel, ct),
                RefactoringDescriptors.DeconstructForeachVariable);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ForEachStatementSyntax forEachStatement,
            IMethodSymbol deconstructSymbol,
            ISymbol identifierSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            DeclarationExpressionSyntax variableExpression = DeclarationExpression(
                CSharpFactory.VarType().WithTriviaFrom(forEachStatement.Type),
                ParenthesizedVariableDesignation(
                    deconstructSymbol.Parameters.Select(parameter =>
                    {
                        return (VariableDesignationSyntax)SingleVariableDesignation(
                            Identifier(
                                SyntaxTriviaList.Empty,
                                parameter.Name,
                                SyntaxTriviaList.Empty));
                    })
                        .ToSeparatedSyntaxList())
                    .WithTriviaFrom(forEachStatement.Identifier))
                .WithFormatterAnnotation();

            var rewriter = new DeconstructForeachVariableRewriter(identifierSymbol, semanticModel, cancellationToken);

            var newStatement = (StatementSyntax)rewriter.Visit(forEachStatement.Statement);

            ForEachVariableStatementSyntax newForEachStatement = ForEachVariableStatement(
                forEachStatement.AttributeLists,
                forEachStatement.AwaitKeyword,
                forEachStatement.ForEachKeyword,
                forEachStatement.OpenParenToken,
                variableExpression.WithFormatterAnnotation(),
                forEachStatement.InKeyword,
                forEachStatement.Expression,
                forEachStatement.CloseParenToken,
                newStatement);

            return await document.ReplaceNodeAsync(forEachStatement, newForEachStatement, cancellationToken).ConfigureAwait(false);
        }

        private class DeconstructForeachVariableWalker : CSharpSyntaxWalker
        {
            public DeconstructForeachVariableWalker(
                IMethodSymbol deconstructMethod,
                ISymbol identifierSymbol,
                string identifier,
                SemanticModel semanticModel,
                CancellationToken cancellationToken)
            {
                DeconstructMethod = deconstructMethod;
                IdentifierSymbol = identifierSymbol;
                Identifier = identifier;
                SemanticModel = semanticModel;
                CancellationToken = cancellationToken;
            }

            public IMethodSymbol DeconstructMethod { get; }

            public ISymbol IdentifierSymbol { get; }

            public string Identifier { get; }

            public SemanticModel SemanticModel { get; }

            public CancellationToken CancellationToken { get; }

            public bool Success { get; private set; } = true;

            public override void DefaultVisit(SyntaxNode node)
            {
                if (Success)
                    base.DefaultVisit(node);
            }

            public override void VisitIdentifierName(IdentifierNameSyntax node)
            {
                if (node.Identifier.ValueText == IdentifierSymbol.Name
                    && SymbolEqualityComparer.Default.Equals(SemanticModel.GetSymbol(node, CancellationToken), IdentifierSymbol)
                    && !IsFixable(node))
                {
                    Success = false;
                }

                base.VisitIdentifierName(node);

                bool IsFixable(IdentifierNameSyntax node)
                {
                    if (node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)node.Parent;
                        if (object.ReferenceEquals(memberAccess.Expression, node))
                        {
                            foreach (IParameterSymbol parameter in DeconstructMethod.Parameters)
                            {
                                if (string.Equals(parameter.Name, memberAccess.Name.Identifier.ValueText, StringComparison.OrdinalIgnoreCase))
                                    return true;
                            }
                        }
                    }

                    return false;
                }
            }
        }

        private class DeconstructForeachVariableRewriter : CSharpSyntaxRewriter
        {
            public DeconstructForeachVariableRewriter(
                ISymbol identifierSymbol,
                SemanticModel semanticModel,
                CancellationToken cancellationToken)
            {
                IdentifierSymbol = identifierSymbol;
                SemanticModel = semanticModel;
                CancellationToken = cancellationToken;
            }

            public ISymbol IdentifierSymbol { get; }

            public SemanticModel SemanticModel { get; }

            public CancellationToken CancellationToken { get; }

            public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
            {
                if (node.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                    && node.Expression is IdentifierNameSyntax identifierName
                    && identifierName.Identifier.ValueText == IdentifierSymbol.Name
                    && SymbolEqualityComparer.Default.Equals(SemanticModel.GetSymbol(identifierName, CancellationToken), IdentifierSymbol))
                {
                    return IdentifierName(StringUtility.FirstCharToLower(node.Name.Identifier.ValueText))
                        .WithTriviaFrom(identifierName);
                }

                return base.VisitMemberAccessExpression(node);
            }
        }
    }
}
