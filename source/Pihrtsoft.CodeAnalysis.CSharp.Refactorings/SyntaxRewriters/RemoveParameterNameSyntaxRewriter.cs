using System;
// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters
{
    internal class RemoveParameterNameSyntaxRewriter : CSharpSyntaxRewriter
    {
        private static readonly RemoveParameterNameSyntaxRewriter _instance = new RemoveParameterNameSyntaxRewriter();

        private RemoveParameterNameSyntaxRewriter()
        {
        }

        public static ArgumentListSyntax VisitNode(ArgumentListSyntax argumentList)
        {
            return (ArgumentListSyntax)_instance.Visit((SyntaxNode)argumentList);
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
