// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.UseMethodChaining
{
    internal class MethodChainingWithAssignmentAnalysis : UseMethodChainingAnalysis
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

            SimpleAssignmentExpressionInfo assignmentInfo = SyntaxInfo.SimpleAssignmentExpressionInfo(expressionStatement.Expression);

            if (!assignmentInfo.Success)
                return false;

            if (name != (assignmentInfo.Left as IdentifierNameSyntax)?.Identifier.ValueText)
                return false;

            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(assignmentInfo.Right);

            if (!invocationInfo.Success)
                return false;

            if (!(WalkDownMethodChain(invocationInfo).Expression is IdentifierNameSyntax identifierName))
                return false;

            if (name != identifierName.Identifier.ValueText)
                return false;

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocationInfo.InvocationExpression, cancellationToken);

            if (methodSymbol== null)
                return false;

            if (!methodSymbol.ReturnType.Equals(typeSymbol))
                return false;

            if (IsReferenced(invocationInfo.InvocationExpression, identifierName, name, semanticModel, cancellationToken))
                return false;

            return true;
        }

        private static bool IsReferenced(
            SyntaxNode node,
            IdentifierNameSyntax identifierName,
            string name,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ISymbol symbol = null;

            foreach (SyntaxNode descendant in node.DescendantNodes())
            {
                if ((descendant is IdentifierNameSyntax identifierName2)
                    && identifierName != identifierName2
                    && name == identifierName2.Identifier.ValueText)
                {
                    if (symbol == null)
                        symbol = semanticModel.GetSymbol(identifierName, cancellationToken);

                    if (semanticModel.GetSymbol(identifierName2, cancellationToken)?.Equals(symbol) == true)
                        return true;
                }
            }

            return false;
        }
    }
}