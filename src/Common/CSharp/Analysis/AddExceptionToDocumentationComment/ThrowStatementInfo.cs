// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.AddExceptionToDocumentationComment
{
    internal class ThrowStatementInfo : ThrowInfo
    {
        internal ThrowStatementInfo(ThrowStatementSyntax node, ExpressionSyntax expression, ITypeSymbol exceptionSymbol, ISymbol declarationSymbol)
            : base(node, expression, exceptionSymbol, declarationSymbol)
        {
        }

        protected override IParameterSymbol GetParameterSymbolCore(
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SyntaxNode parent = Node.Parent;

            if (parent == null)
                return null;

            if (parent.IsKind(SyntaxKind.Block))
                parent = parent.Parent;

            if (parent is not IfStatementSyntax ifStatement)
                return null;

            ExpressionSyntax condition = ifStatement.Condition;

            if (condition?.Kind() != SyntaxKind.EqualsExpression)
                return null;

            var equalsExpression = (BinaryExpressionSyntax)condition;

            ExpressionSyntax left = equalsExpression.Left;

            if (left == null)
                return null;

            ISymbol leftSymbol = semanticModel.GetSymbol(left, cancellationToken);

            if (leftSymbol?.Kind != SymbolKind.Parameter)
                return null;

            if (!SymbolEqualityComparer.Default.Equals(leftSymbol.ContainingSymbol, DeclarationSymbol))
                return null;

            return (IParameterSymbol)leftSymbol;
        }
    }
}
