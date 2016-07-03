// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class AttributeSyntaxExtensions
    {
        public static AttributeSyntax WithArguments(
            this AttributeSyntax attribute,
            params AttributeArgumentSyntax[] arguments)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            return attribute
                .WithArgumentList(
                    AttributeArgumentList(
                        SeparatedList(arguments)));
        }

        public static AttributeSyntax WithArgument(
            this AttributeSyntax attribute,
            AttributeArgumentSyntax argument)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            return attribute
                .WithArgumentList(
                    AttributeArgumentList(
                        SingletonSeparatedList(argument)));
        }

        public static AttributeSyntax WithArgument(
            this AttributeSyntax attribute,
            ExpressionSyntax expression)
        {
            return WithArgument(attribute, AttributeArgument(expression));
        }
    }
}
