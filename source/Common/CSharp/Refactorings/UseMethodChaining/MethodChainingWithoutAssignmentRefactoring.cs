// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.UseMethodChaining
{
    internal class MethodChainingWithoutAssignmentRefactoring : UseMethodChainingRefactoring
    {
        protected override bool IsFixableStatement(
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

            if (!MemberInvocationExpression.TryCreate(expressionStatement.Expression, out MemberInvocationExpression memberInvocation))
                return false;

            if (!(WalkDownMethodChain(memberInvocation).Expression is IdentifierNameSyntax identifierName))
                return false;

            if (name != identifierName.Identifier.ValueText)
                return false;

            if (!semanticModel.TryGetMethodInfo(memberInvocation.InvocationExpression, out MethodInfo methodInfo, cancellationToken))
                return false;

            return !methodInfo.IsStatic
                && methodInfo.ContainingType?.Equals(typeSymbol) == true
                && methodInfo.ReturnType.Equals(typeSymbol);
        }

        protected override InvocationExpressionSyntax GetInvocationExpression(ExpressionStatementSyntax expressionStatement)
        {
            return expressionStatement.Expression as InvocationExpressionSyntax;
        }
    }
}