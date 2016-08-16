// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring.MissingNodeInList
{
    internal class ArgumentSyntaxRewriter : ArgumentOrParameterSyntaxRewriter<ArgumentSyntax>
    {
        public ArgumentSyntaxRewriter(
            ArgumentSyntax argument,
            ArgumentSyntax newArgument,
            SyntaxToken tokenBefore,
            SyntaxToken tokenAfter) : base(argument, newArgument, tokenBefore, tokenAfter)
        {
        }

        public override SyntaxNode VisitArgument(ArgumentSyntax node)
        {
            if (node == Node)
                return NewNode;

            return base.VisitArgument(node);
        }
    }
}
