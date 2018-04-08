// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.AddExceptionToDocumentationComment
{
    internal class ThrowExpressionInfo : ThrowInfo
    {
        internal ThrowExpressionInfo(ThrowExpressionSyntax node, ExpressionSyntax expression, ITypeSymbol exceptionSymbol, ISymbol declarationSymbol)
            : base(node, expression, exceptionSymbol, declarationSymbol)
        {
        }

        protected override IParameterSymbol GetParameterSymbolCore(SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            SyntaxNode parent = Node.Parent;

            if (parent?.Kind() != SyntaxKind.CoalesceExpression)
                return null;

            SimpleAssignmentExpressionInfo simpleAssignment = SyntaxInfo.SimpleAssignmentExpressionInfo(parent.Parent);

            ISymbol leftSymbol = semanticModel.GetSymbol(simpleAssignment.Left, cancellationToken);

            if (leftSymbol?.Kind != SymbolKind.Parameter)
                return null;

            if (leftSymbol.ContainingSymbol?.Equals(DeclarationSymbol) != true)
                return null;

            return (IParameterSymbol)leftSymbol;
        }
    }
}