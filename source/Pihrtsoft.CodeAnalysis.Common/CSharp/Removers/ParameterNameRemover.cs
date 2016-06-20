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

        private readonly ArgumentSyntax[] _arguments;

        private ParameterNameRemover(ArgumentSyntax[] arguments = null)
        {
            _arguments = arguments;
        }

        public static ArgumentListSyntax VisitNode(ArgumentListSyntax argumentList, ArgumentSyntax[] arguments = null)
        {
            if (argumentList == null)
                throw new ArgumentNullException(nameof(argumentList));

            if (arguments == null)
            {
                return (ArgumentListSyntax)_instance.Visit(argumentList);
            }
            else
            {
                var instance = new ParameterNameRemover(arguments);
                return (ArgumentListSyntax)instance.Visit(argumentList);
            }
        }

        public override SyntaxNode VisitArgument(ArgumentSyntax node)
        {
            if (_arguments == null || Array.IndexOf(_arguments, node) != -1)
            {
                return node
                    .WithNameColon(null)
                    .WithTriviaFrom(node);
            }

            return base.VisitArgument(node);
        }
    }
}
