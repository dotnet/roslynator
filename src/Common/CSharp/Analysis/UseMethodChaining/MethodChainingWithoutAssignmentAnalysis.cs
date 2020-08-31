// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.UseMethodChaining
{
    internal class MethodChainingWithoutAssignmentAnalysis : UseMethodChainingAnalysis
    {
        public override bool IsFixableStatement(
            StatementSyntax statement,
            string name,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (statement.SpanOrLeadingTriviaContainsDirectives())
                return false;

            if (!(statement is ExpressionStatementSyntax expressionStatement))
                return false;

            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(expressionStatement.Expression);

            if (!invocationInfo.Success)
                return false;

            SimpleMemberInvocationExpressionInfo topInvocationInfo = WalkDownMethodChain(invocationInfo);

            if (!(topInvocationInfo.Expression is IdentifierNameSyntax identifierName))
                return false;

            if (name != identifierName.Identifier.ValueText)
                return false;

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(topInvocationInfo.InvocationExpression, cancellationToken);

            return methodSymbol?.IsStatic == false
                && SymbolEqualityComparer.Default.Equals(methodSymbol.ContainingType, typeSymbol)
                && SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, typeSymbol);
        }
    }
}