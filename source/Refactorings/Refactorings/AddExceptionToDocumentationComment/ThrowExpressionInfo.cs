// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings.AddExceptionToDocumentationComment
{
    internal class ThrowExpressionInfo : ThrowInfo
    {
        internal ThrowExpressionInfo(ThrowExpressionSyntax node, ExpressionSyntax expression, ITypeSymbol exceptionSymbol)
            : base(node, expression, exceptionSymbol)
        {
        }

        protected override IParameterSymbol GetParameterSymbolCore(
            ISymbol declarationSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SyntaxNode parent = Node.Parent;

            if (parent?.IsKind(SyntaxKind.CoalesceExpression) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                {
                    var assignment = (AssignmentExpressionSyntax)parent;

                    ExpressionSyntax left = assignment.Left;

                    if (left != null)
                    {
                        ISymbol leftSymbol = semanticModel.GetSymbol(left, cancellationToken);

                        if (leftSymbol?.IsParameter() == true
                            && leftSymbol.ContainingSymbol?.Equals(declarationSymbol) == true)
                        {
                            return (IParameterSymbol)leftSymbol;
                        }
                    }
                }
            }

            return null;
        }
    }
}