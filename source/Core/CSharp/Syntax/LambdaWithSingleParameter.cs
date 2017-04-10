// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;

namespace Roslynator.CSharp.Syntax
{
    internal struct LambdaWithSingleParameter : IEquatable<LambdaWithSingleParameter>
    {
        internal LambdaWithSingleParameter(ParameterSyntax parameter, CSharpSyntaxNode body)
            : this()
        {
            Parameter = parameter;
            Body = body;
        }

        public ParameterSyntax Parameter { get; }
        public CSharpSyntaxNode Body { get; }

        public LambdaExpressionSyntax LambdaExpression
        {
            get { return (LambdaExpressionSyntax)Parent; }
        }

        private SyntaxNode Parent
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
            get { return Parent?.IsKind(SyntaxKind.SimpleLambdaExpression) == true; }
        }

        public bool IsParenthesizedLambda
        {
            get { return Parent?.IsKind(SyntaxKind.ParenthesizedLambdaExpression) == true; }
        }

        public static LambdaWithSingleParameter Create(LambdaExpressionSyntax lambdaExpression)
        {
            if (lambdaExpression == null)
                throw new ArgumentNullException(nameof(lambdaExpression));

            switch (lambdaExpression.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        var simpleLambda = (SimpleLambdaExpressionSyntax)lambdaExpression;

                        return new LambdaWithSingleParameter(simpleLambda.Parameter, simpleLambda.Body);
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        var parenthesizedLambda = (ParenthesizedLambdaExpressionSyntax)lambdaExpression;

                        ParameterSyntax parameter = parenthesizedLambda.ParameterList?.Parameters.FirstOrDefault();

                        if (parameter == null)
                            throw new ArgumentException("", nameof(lambdaExpression));

                        return new LambdaWithSingleParameter(parameter, parenthesizedLambda.Body);
                    }
            }

            throw new ArgumentException("", nameof(lambdaExpression));
        }

        public static bool TryCreate(SyntaxNode lambdaExpression, out LambdaWithSingleParameter lambda)
        {
            if (lambdaExpression?.IsKind(SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression) == true)
                return TryCreate((LambdaExpressionSyntax)lambdaExpression, out lambda);

            lambda = default(LambdaWithSingleParameter);
            return false;
        }

        public static bool TryCreate(LambdaExpressionSyntax lambdaExpression, out LambdaWithSingleParameter result)
        {
            switch (lambdaExpression?.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        var simpleLambda = (SimpleLambdaExpressionSyntax)lambdaExpression;

                        result = new LambdaWithSingleParameter(simpleLambda.Parameter, simpleLambda.Body);
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
                                result = new LambdaWithSingleParameter(parameters[0], parenthesizedLambda.Body);
                                return true;
                            }
                        }

                        break;
                    }
            }

            result = default(LambdaWithSingleParameter);
            return false;
        }

        public bool Equals(LambdaWithSingleParameter other)
        {
            return Parent == other.Parent;
        }

        public override bool Equals(object obj)
        {
            return obj is LambdaWithSingleParameter
                && Equals((LambdaWithSingleParameter)obj);
        }

        public override int GetHashCode()
        {
            return Parent?.GetHashCode() ?? 0;
        }

        public static bool operator ==(LambdaWithSingleParameter left, LambdaWithSingleParameter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LambdaWithSingleParameter left, LambdaWithSingleParameter right)
        {
            return !left.Equals(right);
        }
    }
}
