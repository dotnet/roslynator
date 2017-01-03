// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.AddExceptionToDocumentationComment
{
    internal abstract class ThrowInfo
    {
        protected ThrowInfo(SyntaxNode node, ExpressionSyntax expression, ITypeSymbol exceptionSymbol)
        {
            Node = node;
            Expression = expression;
            ExceptionSymbol = exceptionSymbol;
        }

        public SyntaxNode Node { get; }

        public ITypeSymbol ExceptionSymbol { get; }

        public ExpressionSyntax Expression { get; }

        public static ThrowInfo Create(SyntaxNode node, ITypeSymbol exceptionSymbol)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ThrowStatement:
                    {
                        var throwStatement = (ThrowStatementSyntax)node;

                        return new ThrowStatementInfo(throwStatement, throwStatement.Expression, exceptionSymbol);
                    }
                case SyntaxKind.ThrowExpression:
                    {
                        var throwExpression = (ThrowExpressionSyntax)node;

                        return new ThrowExpressionInfo(throwExpression, throwExpression.Expression, exceptionSymbol);
                    }
                default:
                    {
                        throw new ArgumentException("", nameof(node));
                    }
            }
        }

        public IParameterSymbol GetParameterSymbol(
            ISymbol declarationSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            INamedTypeSymbol argumentExceptionSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_ArgumentException);

            if (ExceptionSymbol.EqualsOrDerivedFrom(argumentExceptionSymbol))
            {
                return GetParameterSymbolCore(declarationSymbol, semanticModel, cancellationToken);
            }
            else
            {
                return null;
            }
        }

        protected abstract IParameterSymbol GetParameterSymbolCore(
            ISymbol declarationSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken);
    }
}