// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Helpers;

namespace Roslynator.CSharp.Refactorings.InlineDefinition
{
    internal class InlineMethodAnalyzer : InlineAnalyzer<InvocationExpressionSyntax, MethodDeclarationSyntax, IMethodSymbol>
    {
        public static InlineMethodAnalyzer Instance { get; } = new InlineMethodAnalyzer();

        protected override bool ValidateNode(InvocationExpressionSyntax node, TextSpan span)
        {
            ExpressionSyntax expression = node.Expression;

            if (expression == null)
                return false;

            ArgumentListSyntax argumentList = node.ArgumentList;

            if (argumentList == null)
                return false;

            if (expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
            {
                return ((MemberAccessExpressionSyntax)expression).Name?.Span.Contains(span) == true;
            }
            else
            {
                return expression.Span.Contains(span);
            }
        }

        protected override IMethodSymbol GetMemberSymbol(
            InvocationExpressionSyntax node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var methodSymbol = semanticModel.GetSymbol(node, cancellationToken) as IMethodSymbol;

            if (methodSymbol == null)
                return null;

            if (methodSymbol.Language != LanguageNames.CSharp)
                return null;

            MethodKind methodKind = methodSymbol.MethodKind;

            if (methodKind == MethodKind.Ordinary)
            {
                if (methodSymbol.IsStatic)
                    return methodSymbol;

                INamedTypeSymbol enclosingType = semanticModel.GetEnclosingNamedType(node.SpanStart, cancellationToken);

                if (methodSymbol.ContainingType?.Equals(enclosingType) == true)
                {
                    ExpressionSyntax expression = node.Expression;

                    if (expression != null)
                    {
                        if (!expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                            || ((MemberAccessExpressionSyntax)expression).Expression.IsKind(SyntaxKind.ThisExpression))
                        {
                            return methodSymbol;
                        }
                    }
                }
            }
            else if (methodKind == MethodKind.ReducedExtension
                && node.Expression?.Kind() == SyntaxKind.SimpleMemberAccessExpression)
            {
                return methodSymbol;
            }

            return null;
        }

        protected override async Task<MethodDeclarationSyntax> GetMemberDeclarationAsync(
            IMethodSymbol symbol,
            CancellationToken cancellationToken)
        {
            foreach (SyntaxReference reference in symbol.DeclaringSyntaxReferences)
            {
                SyntaxNode node = await reference.GetSyntaxAsync(cancellationToken).ConfigureAwait(false);

                if ((node is MethodDeclarationSyntax methodDeclaration)
                    && methodDeclaration.BodyOrExpressionBody() != null)
                {
                    return methodDeclaration;
                }
            }

            return null;
        }

        protected override ImmutableArray<ParameterInfo> GetParameterInfos(
            InvocationExpressionSyntax node,
            IMethodSymbol symbol)
        {
            bool isReduced = symbol.MethodKind == MethodKind.ReducedExtension;

            ImmutableArray<IParameterSymbol> parameters = symbol.Parameters;

            List<ParameterInfo> parameterInfos = null;

            SeparatedSyntaxList<ArgumentSyntax> arguments = node.ArgumentList.Arguments;

            foreach (ArgumentSyntax argument in arguments)
            {
                IParameterSymbol parameterSymbol = DetermineParameterHelper.DetermineParameter(argument, arguments, parameters);

                if (parameterSymbol != null)
                {
                    var parameterInfo = new ParameterInfo(parameterSymbol, argument.Expression);

                    (parameterInfos ?? (parameterInfos = new List<ParameterInfo>())).Add(parameterInfo);
                }
                else
                {
                    return default(ImmutableArray<ParameterInfo>);
                }
            }

            foreach (IParameterSymbol parameterSymbol in parameters)
            {
                if (parameterInfos == null
                    || parameterInfos.FindIndex(f => f.ParameterSymbol.Equals(parameterSymbol)) == -1)
                {
                    if (parameterSymbol.HasExplicitDefaultValue)
                    {
                        var parameterInfo = new ParameterInfo(parameterSymbol, null);

                        (parameterInfos ?? (parameterInfos = new List<ParameterInfo>())).Add(parameterInfo);
                    }
                    else
                    {
                        return default(ImmutableArray<ParameterInfo>);
                    }
                }
            }

            if (isReduced)
            {
                var memberAccess = (MemberAccessExpressionSyntax)node.Expression;

                ExpressionSyntax expression = memberAccess.Expression;

                SyntaxNode nodeIncludingConditionalAccess = node.WalkUp(SyntaxKind.ConditionalAccessExpression);

                if (nodeIncludingConditionalAccess != node)
                {
                    int startIndex = expression.Span.End - nodeIncludingConditionalAccess.SpanStart;
                    expression = SyntaxFactory.ParseExpression(nodeIncludingConditionalAccess.ToString().Remove(startIndex));
                }

                var parameterInfo = new ParameterInfo(symbol.ReducedFrom.Parameters[0], expression.TrimTrivia(), isThis: true);

                (parameterInfos ?? (parameterInfos = new List<ParameterInfo>())).Add(parameterInfo);
            }

            return (parameterInfos != null)
                ? parameterInfos.ToImmutableArray()
                : ImmutableArray<ParameterInfo>.Empty;
        }

        protected override (ExpressionSyntax expression, SyntaxList<StatementSyntax> statements) GetExpressionOrStatements(MethodDeclarationSyntax declaration)
        {
            BlockSyntax body = declaration.Body;

            if (body == null)
                return (declaration.ExpressionBody?.Expression, default(SyntaxList<StatementSyntax>));

            SyntaxList<StatementSyntax> statements = body.Statements;

            if (!statements.Any())
                return (null, default(SyntaxList<StatementSyntax>));

            switch (statements.SingleOrDefault(shouldThrow: false))
            {
                case ReturnStatementSyntax returnStatement:
                    return (returnStatement.Expression, default(SyntaxList<StatementSyntax>));
                case ExpressionStatementSyntax expressionStatement:
                    return (expressionStatement.Expression, default(SyntaxList<StatementSyntax>));
            }

            if (!declaration.ReturnsVoid())
                return (null, default(SyntaxList<StatementSyntax>));

            return (null, statements);
        }

        protected override InlineRefactoring<InvocationExpressionSyntax, MethodDeclarationSyntax, IMethodSymbol> CreateRefactoring(
            Document document,
            SyntaxNode node,
            INamedTypeSymbol nodeEnclosingType,
            IMethodSymbol symbol,
            MethodDeclarationSyntax declaration,
            ImmutableArray<ParameterInfo> parameterInfos,
            SemanticModel nodeSemanticModel,
            SemanticModel declarationSemanticModel,
            CancellationToken cancellationToken)
        {
            return new InlineMethodRefactoring(document, node, nodeEnclosingType, symbol, declaration, parameterInfos, nodeSemanticModel, declarationSemanticModel, cancellationToken);
        }
    }
}
