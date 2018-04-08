// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.AddExceptionToDocumentationComment
{
    internal abstract class ThrowInfo
    {
        protected ThrowInfo(SyntaxNode node, ExpressionSyntax expression, ITypeSymbol exceptionSymbol, ISymbol declarationSymbol)
        {
            Node = node;
            Expression = expression;
            ExceptionSymbol = exceptionSymbol;
            DeclarationSymbol = declarationSymbol;
        }

        public SyntaxNode Node { get; }

        public ITypeSymbol ExceptionSymbol { get; }

        public ExpressionSyntax Expression { get; }

        public ISymbol DeclarationSymbol { get; }

        public static ThrowInfo Create(SyntaxNode node, ITypeSymbol exceptionSymbol, ISymbol declarationSymbol)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ThrowStatement:
                    {
                        var throwStatement = (ThrowStatementSyntax)node;

                        return new ThrowStatementInfo(throwStatement, throwStatement.Expression, exceptionSymbol, declarationSymbol);
                    }
                case SyntaxKind.ThrowExpression:
                    {
                        var throwExpression = (ThrowExpressionSyntax)node;

                        return new ThrowExpressionInfo(throwExpression, throwExpression.Expression, exceptionSymbol, declarationSymbol);
                    }
                default:
                    {
                        throw new ArgumentException("", nameof(node));
                    }
            }
        }

        public IParameterSymbol GetParameterSymbol(
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            INamedTypeSymbol argumentExceptionSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_ArgumentException);

            if (ExceptionSymbol.EqualsOrInheritsFrom(argumentExceptionSymbol))
            {
                return GetParameterSymbolCore(semanticModel, cancellationToken);
            }
            else
            {
                return null;
            }
        }

        protected abstract IParameterSymbol GetParameterSymbolCore(SemanticModel semanticModel, CancellationToken cancellationToken);
    }
}