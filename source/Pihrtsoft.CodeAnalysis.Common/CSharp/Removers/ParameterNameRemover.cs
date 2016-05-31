// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Removers
{
    public sealed class ParameterNameRemover : CSharpSyntaxRewriter
    {
        private static readonly ParameterNameRemover _instance = new ParameterNameRemover();

        private ParameterNameRemover()
        {
        }

        public static ArgumentListSyntax VisitNode(ArgumentListSyntax argumentList)
        {
            if (argumentList == null)
                throw new ArgumentNullException(nameof(argumentList));

            return (ArgumentListSyntax)_instance.Visit(argumentList);
        }

        public override SyntaxNode VisitArgument(ArgumentSyntax node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node
                .WithNameColon(null)
                .WithTriviaFrom(node);
        }
    }
}
