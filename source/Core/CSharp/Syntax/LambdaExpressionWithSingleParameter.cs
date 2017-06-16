// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    internal struct LambdaExpressionWithSingleParameter : IEquatable<LambdaExpressionWithSingleParameter>
    {
        internal LambdaExpressionWithSingleParameter(ParameterSyntax parameter, CSharpSyntaxNode body)
            : this()
        {
            Parameter = parameter;
            Body = body;
        }

        public ParameterSyntax Parameter { get; }
        public CSharpSyntaxNode Body { get; }

        public LambdaExpressionSyntax LambdaExpression
        {
            get { return (LambdaExpressionSyntax)Node; }
        }

        private SyntaxNode Node
        {
            get { return Body?.Parent; }
        }

        public ParameterListSyntax ParameterList
        {
            get
            {
                SyntaxNode parent = Parameter?.Parent;

                if (parent?.IsKind(SyntaxKind.ParameterList) == true)
                    return (ParameterListSyntax)parent;

                return null;
            }
        }

        public string ParameterName
        {
            get { return Parameter?.Identifier.ValueText; }
        }

        public bool IsSimpleLambda
        {
            get { return Node?.IsKind(SyntaxKind.SimpleLambdaExpression) == true; }
        }

        public bool IsParenthesizedLambda
        {
            get { return Node?.IsKind(SyntaxKind.ParenthesizedLambdaExpression) == true; }
        }

        public static LambdaExpressionWithSingleParameter Create(LambdaExpressionSyntax lambdaExpression)
        {
            if (lambdaExpression == null)
                throw new ArgumentNullException(nameof(lambdaExpression));

            switch (lambdaExpression.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        var simpleLambda = (SimpleLambdaExpressionSyntax)lambdaExpression;

                        return new LambdaExpressionWithSingleParameter(simpleLambda.Parameter, simpleLambda.Body);
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        var parenthesizedLambda = (ParenthesizedLambdaExpressionSyntax)lambdaExpression;

                        ParameterSyntax parameter = parenthesizedLambda.ParameterList?.Parameters.FirstOrDefault();

                        if (parameter == null)
                            throw new ArgumentException("", nameof(lambdaExpression));

                        return new LambdaExpressionWithSingleParameter(parameter, parenthesizedLambda.Body);
                    }
            }

            throw new ArgumentException("", nameof(lambdaExpression));
        }

        public static bool TryCreate(SyntaxNode lambdaExpression, out LambdaExpressionWithSingleParameter lambda)
        {
            if (lambdaExpression?.IsKind(SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression) == true)
                return TryCreate((LambdaExpressionSyntax)lambdaExpression, out lambda);

            lambda = default(LambdaExpressionWithSingleParameter);
            return false;
        }

        public static bool TryCreate(LambdaExpressionSyntax lambdaExpression, out LambdaExpressionWithSingleParameter result)
        {
            switch (lambdaExpression?.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        var simpleLambda = (SimpleLambdaExpressionSyntax)lambdaExpression;

                        result = new LambdaExpressionWithSingleParameter(simpleLambda.Parameter, simpleLambda.Body);
                        return true;
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        var parenthesizedLambda = (ParenthesizedLambdaExpressionSyntax)lambdaExpression;

                        ParameterListSyntax parameterList = parenthesizedLambda.ParameterList;
                        if (parameterList != null)
                        {
                            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;
                            if (parameters.Count == 1)
                            {
                                result = new LambdaExpressionWithSingleParameter(parameters[0], parenthesizedLambda.Body);
                                return true;
                            }
                        }

                        break;
                    }
            }

            result = default(LambdaExpressionWithSingleParameter);
            return false;
        }

        public override string ToString()
        {
            return Node?.ToString() ?? base.ToString();
        }

        public bool Equals(LambdaExpressionWithSingleParameter other)
        {
            return Node == other.Node;
        }

        public override bool Equals(object obj)
        {
            return obj is LambdaExpressionWithSingleParameter
                && Equals((LambdaExpressionWithSingleParameter)obj);
        }

        public override int GetHashCode()
        {
            return Node?.GetHashCode() ?? 0;
        }

        public static bool operator ==(LambdaExpressionWithSingleParameter left, LambdaExpressionWithSingleParameter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LambdaExpressionWithSingleParameter left, LambdaExpressionWithSingleParameter right)
        {
            return !left.Equals(right);
        }
    }
}
