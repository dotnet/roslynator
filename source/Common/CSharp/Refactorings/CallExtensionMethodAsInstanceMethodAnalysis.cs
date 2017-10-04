// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    public struct CallExtensionMethodAsInstanceMethodAnalysis
    {
        public CallExtensionMethodAsInstanceMethodAnalysis(
            InvocationExpressionSyntax invocationExpression,
            InvocationExpressionSyntax newInvocationExpression,
            IMethodSymbol methodSymbol)
        {
            if (invocationExpression == null)
                throw new ArgumentNullException(nameof(invocationExpression));

            if (newInvocationExpression == null)
                throw new ArgumentNullException(nameof(newInvocationExpression));

            InvocationExpression = invocationExpression;
            NewInvocationExpression = newInvocationExpression;
            MethodSymbol = methodSymbol;
        }

        public InvocationExpressionSyntax InvocationExpression { get; }

        public InvocationExpressionSyntax NewInvocationExpression { get; }

        public IMethodSymbol MethodSymbol { get; }

        public bool Success
        {
            get
            {
                return InvocationExpression != null
                    && NewInvocationExpression != null;
            }
        }
    }
}