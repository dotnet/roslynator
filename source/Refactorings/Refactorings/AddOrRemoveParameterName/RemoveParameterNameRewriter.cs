// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.AddOrRemoveParameterName
{
    internal class RemoveParameterNameRewriter : CSharpSyntaxRewriter
    {
        private readonly ImmutableArray<ArgumentSyntax> _arguments;

        public RemoveParameterNameRewriter(ImmutableArray<ArgumentSyntax> arguments)
        {
            _arguments = arguments;
        }

        public override SyntaxNode VisitArgument(ArgumentSyntax node)
        {
            if (_arguments.Contains(node))
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
