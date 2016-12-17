// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxRewriters
{
    internal class NameColonRemover : CSharpSyntaxRewriter
    {
        private readonly HashSet<ArgumentSyntax> _arguments;

        private NameColonRemover()
        {
        }

        private NameColonRemover(IEnumerable<ArgumentSyntax> arguments)
        {
            _arguments = new HashSet<ArgumentSyntax>(arguments);
        }

        public static TNode RemoveNameColons<TNode>(TNode node) where TNode : SyntaxNode
        {
            var remover = new NameColonRemover();

            return (TNode)remover.Visit(node);
        }

        public static TNode RemoveNameColons<TNode>(TNode node, IEnumerable<ArgumentSyntax> arguments) where TNode : SyntaxNode
        {
            var remover = new NameColonRemover(arguments);

            return (TNode)remover.Visit(node);
        }

        public override SyntaxNode VisitArgument(ArgumentSyntax node)
        {
            if (_arguments == null || _arguments.Contains(node))
            {
                return node
                    .WithNameColon(null)
                    .WithTriviaFrom(node);
            }
            else
            {
                return base.VisitArgument(node);
            }
        }
    }
}
