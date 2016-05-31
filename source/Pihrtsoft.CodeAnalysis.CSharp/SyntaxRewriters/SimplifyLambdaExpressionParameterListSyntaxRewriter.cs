// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters
{
    internal class SimplifyLambdaExpressionParameterListSyntaxRewriter : CSharpSyntaxRewriter
    {
        private static readonly SimplifyLambdaExpressionParameterListSyntaxRewriter _instance = new SimplifyLambdaExpressionParameterListSyntaxRewriter();

        private SimplifyLambdaExpressionParameterListSyntaxRewriter()
        {
        }

        public static ParenthesizedLambdaExpressionSyntax VisitNode(ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
        {
            return (ParenthesizedLambdaExpressionSyntax)_instance.Visit(parenthesizedLambda);
        }

        public override SyntaxNode VisitParameter(ParameterSyntax node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTriviaList list = SyntaxFactory.TriviaList(node.Type.GetLeadingTrivia())
                .AddRange(node.Type.GetTrailingTrivia())
                .AddRange(node.Identifier.LeadingTrivia);

            return node
                .WithType(null)
                .WithIdentifier(node.Identifier.WithLeadingTrivia(list));
        }
    }
}
