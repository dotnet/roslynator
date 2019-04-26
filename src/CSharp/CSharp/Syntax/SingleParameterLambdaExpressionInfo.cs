// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about a lambda expression with a single parameter.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct SingleParameterLambdaExpressionInfo
    {
        private SingleParameterLambdaExpressionInfo(
            LambdaExpressionSyntax lambdaExpression,
            ParameterSyntax parameter,
            CSharpSyntaxNode body)
        {
            LambdaExpression = lambdaExpression;
            Parameter = parameter;
            Body = body;
        }

        /// <summary>
        /// The lambda expression.
        /// </summary>
        public LambdaExpressionSyntax LambdaExpression { get; }

        /// <summary>
        /// The parameter.
        /// </summary>
        public ParameterSyntax Parameter { get; }

        /// <summary>
        /// The body of the lambda expression.
        /// </summary>
        public CSharpSyntaxNode Body { get; }

        /// <summary>
        /// The parameter list that contains the parameter.
        /// </summary>
        public ParameterListSyntax ParameterList
        {
            get { return (IsParenthesizedLambda) ? (ParameterListSyntax)Parameter.Parent : null; }
        }

        /// <summary>
        /// True if this instance is a simple lambda expression.
        /// </summary>
        public bool IsSimpleLambda
        {
            get { return LambdaExpression?.Kind() == SyntaxKind.SimpleLambdaExpression; }
        }

        /// <summary>
        /// True if this instance is a parenthesized lambda expression.
        /// </summary>
        public bool IsParenthesizedLambda
        {
            get { return LambdaExpression?.Kind() == SyntaxKind.ParenthesizedLambdaExpression; }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return LambdaExpression != null; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return ToDebugString(Success, this, LambdaExpression); }
        }

        internal static SingleParameterLambdaExpressionInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateImpl(Walk(node, walkDownParentheses) as LambdaExpressionSyntax, allowMissing);
        }

        internal static SingleParameterLambdaExpressionInfo Create(
            LambdaExpressionSyntax lambdaExpression,
            bool allowMissing = false)
        {
            return CreateImpl(lambdaExpression, allowMissing);
        }

        private static SingleParameterLambdaExpressionInfo CreateImpl(
            LambdaExpressionSyntax lambdaExpression,
            bool allowMissing = false)
        {
            switch (lambdaExpression?.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        var simpleLambda = (SimpleLambdaExpressionSyntax)lambdaExpression;

                        ParameterSyntax parameter = simpleLambda.Parameter;

                        if (!Check(parameter, allowMissing))
                            break;

                        CSharpSyntaxNode body = simpleLambda.Body;

                        if (!Check(body, allowMissing))
                            break;

                        return new SingleParameterLambdaExpressionInfo(simpleLambda, parameter, body);
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        var parenthesizedLambda = (ParenthesizedLambdaExpressionSyntax)lambdaExpression;

                        ParameterSyntax parameter = parenthesizedLambda
                            .ParameterList?
                            .Parameters
                            .SingleOrDefault(shouldThrow: false);

                        if (!Check(parameter, allowMissing))
                            break;

                        CSharpSyntaxNode body = parenthesizedLambda.Body;

                        if (!Check(body, allowMissing))
                            break;

                        return new SingleParameterLambdaExpressionInfo(parenthesizedLambda, parameter, body);
                    }
            }

            return default;
        }
    }
}
