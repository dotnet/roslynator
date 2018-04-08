// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    public readonly struct CallExtensionMethodAsInstanceMethodAnalysisResult : IEquatable<CallExtensionMethodAsInstanceMethodAnalysisResult>
    {
        public CallExtensionMethodAsInstanceMethodAnalysisResult(
            InvocationExpressionSyntax invocationExpression,
            InvocationExpressionSyntax newInvocationExpression,
            IMethodSymbol methodSymbol)
        {
            InvocationExpression = invocationExpression ?? throw new ArgumentNullException(nameof(invocationExpression));
            NewInvocationExpression = newInvocationExpression ?? throw new ArgumentNullException(nameof(newInvocationExpression));
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

        public override bool Equals(object obj)
        {
            return obj is CallExtensionMethodAsInstanceMethodAnalysisResult other
                && Equals(other);
        }

        public bool Equals(CallExtensionMethodAsInstanceMethodAnalysisResult other)
        {
            return EqualityComparer<InvocationExpressionSyntax>.Default.Equals(InvocationExpression, other.InvocationExpression)
                   && EqualityComparer<InvocationExpressionSyntax>.Default.Equals(NewInvocationExpression, other.NewInvocationExpression)
                   && EqualityComparer<IMethodSymbol>.Default.Equals(MethodSymbol, other.MethodSymbol);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(InvocationExpression, Hash.Combine(NewInvocationExpression, Hash.Create(MethodSymbol)));
        }

        public static bool operator ==(CallExtensionMethodAsInstanceMethodAnalysisResult analysis1, CallExtensionMethodAsInstanceMethodAnalysisResult analysis2)
        {
            return analysis1.Equals(analysis2);
        }

        public static bool operator !=(CallExtensionMethodAsInstanceMethodAnalysisResult analysis1, CallExtensionMethodAsInstanceMethodAnalysisResult analysis2)
        {
            return !(analysis1 == analysis2);
        }
    }
}