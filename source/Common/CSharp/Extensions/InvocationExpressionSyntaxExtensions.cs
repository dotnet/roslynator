// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class InvocationExpressionSyntaxExtensions
    {
        public static InvocationExpressionSyntax WithArguments(
            this InvocationExpressionSyntax invocationExpression,
            params ArgumentSyntax[] arguments)
        {
            if (invocationExpression == null)
                throw new ArgumentNullException(nameof(invocationExpression));

            return invocationExpression.WithArgumentList(ArgumentList(arguments));
        }

        public static InvocationExpressionSyntax WithArgumentList(this InvocationExpressionSyntax invocationExpression)
        {
            if (invocationExpression == null)
                throw new ArgumentNullException(nameof(invocationExpression));

            return invocationExpression.WithArgumentList(ArgumentList());
        }
    }
}
