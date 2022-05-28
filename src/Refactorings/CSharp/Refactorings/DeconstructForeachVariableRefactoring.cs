// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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

            IEnumerable<ISymbol> parameters = null;

            if (typeSymbol.IsTupleType)
            {
                var tupleType = (INamedTypeSymbol)typeSymbol;
                parameters = tupleType.TupleElements;
            }
            else
            {
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

                parameters = deconstructSymbol.Parameters;
            }

            ISymbol foreachSymbol = semanticModel.GetDeclaredSymbol(forEachStatement, context.CancellationToken);

            if (foreachSymbol?.IsKind(SymbolKind.Local) != true)
                return;

            var walker = new DeconstructForeachVariableWalker(
                parameters,
                foreachSymbol,
                forEachStatement.Identifier.ValueText,
                semanticModel,
                context.CancellationToken);

            walker.Visit(forEachStatement.Statement);

            if (walker.Success)
            {
                context.RegisterRefactoring(
                    "Deconstruct foreach variable",
                    ct => RefactorAsync(context.Document, forEachStatement, parameters, foreachSymbol, semanticModel, ct),
                    RefactoringDescriptors.DeconstructForeachVariable);
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ForEachStatementSyntax forEachStatement,
            IEnumerable<ISymbol> deconstructSymbols,
            ISymbol identifierSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            int position = forEachStatement.SpanStart;
            ITypeSymbol elementType = semanticModel.GetForEachStatementInfo(forEachStatement).ElementType;
            SyntaxNode enclosingSymbolSyntax = semanticModel.GetEnclosingSymbolSyntax(position, cancellationToken);

            ImmutableArray<ISymbol> declaredSymbols = semanticModel.GetDeclaredSymbols(enclosingSymbolSyntax, excludeAnonymousTypeProperty: true, cancellationToken);

            ImmutableArray<ISymbol> symbols = declaredSymbols
                .Concat(semanticModel.LookupSymbols(position))
                .Distinct()
                .Except(deconstructSymbols)
                .ToImmutableArray();

            Dictionary<string, string> newNames = deconstructSymbols
                .Select(parameter =>
                {
                    string name = StringUtility.FirstCharToLower(parameter.Name);
                    string newName = NameGenerator.Default.EnsureUniqueName(name, symbols);

                    return (name: parameter.Name, newName);
                })
                .ToDictionary(f => f.name, f => f.newName);

            var rewriter = new DeconstructForeachVariableRewriter(identifierSymbol, newNames, semanticModel, cancellationToken);

            var newStatement = (StatementSyntax)rewriter.Visit(forEachStatement.Statement);

            DeclarationExpressionSyntax variableExpression = DeclarationExpression(
                CSharpFactory.VarType().WithTriviaFrom(forEachStatement.Type),
                ParenthesizedVariableDesignation(
                    deconstructSymbols.Select(parameter =>
                    {
                        return SingleVariableDesignation(
                            Identifier(SyntaxTriviaList.Empty, newNames[parameter.Name], SyntaxTriviaList.Empty));
                    })
                        .ToSeparatedSyntaxList<VariableDesignationSyntax>())
                    .WithTriviaFrom(forEachStatement.Identifier))
                .WithFormatterAnnotation();

            ForEachVariableStatementSyntax forEachVariableStatement = ForEachVariableStatement(
                forEachStatement.AttributeLists,
                forEachStatement.AwaitKeyword,
                forEachStatement.ForEachKeyword,
                forEachStatement.OpenParenToken,
                variableExpression,
                forEachStatement.InKeyword,
                forEachStatement.Expression,
                forEachStatement.CloseParenToken,
                newStatement);

            return await document.ReplaceNodeAsync(forEachStatement, forEachVariableStatement, cancellationToken).ConfigureAwait(false);
        }

        private class DeconstructForeachVariableWalker : CSharpSyntaxWalker
        {
            public DeconstructForeachVariableWalker(
                IEnumerable<ISymbol> parameters,
                ISymbol identifierSymbol,
                string identifier,
                SemanticModel semanticModel,
                CancellationToken cancellationToken)
            {
                Parameters = parameters;
                IdentifierSymbol = identifierSymbol;
                Identifier = identifier;
                SemanticModel = semanticModel;
                CancellationToken = cancellationToken;
            }

            public IEnumerable<ISymbol> Parameters { get; }

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
                            foreach (ISymbol parameter in Parameters)
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
                Dictionary<string, string> names,
                SemanticModel semanticModel,
                CancellationToken cancellationToken)
            {
                IdentifierSymbol = identifierSymbol;
                Names = names;
                SemanticModel = semanticModel;
                CancellationToken = cancellationToken;
            }

            public ISymbol IdentifierSymbol { get; }

            public Dictionary<string, string> Names { get; }

            public SemanticModel SemanticModel { get; }

            public CancellationToken CancellationToken { get; }

            public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
            {
                if (node.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                    && node.Expression is IdentifierNameSyntax identifierName
                    && identifierName.Identifier.ValueText == IdentifierSymbol.Name
                    && SymbolEqualityComparer.Default.Equals(SemanticModel.GetSymbol(identifierName, CancellationToken), IdentifierSymbol))
                {
                    string name = node.Name.Identifier.ValueText;

                    if (!Names.TryGetValue(name, out string newName))
                        newName = StringUtility.FirstCharToLower(name);

                    return IdentifierName(newName).WithTriviaFrom(identifierName);
                }

                return base.VisitMemberAccessExpression(node);
            }
        }
    }
}
