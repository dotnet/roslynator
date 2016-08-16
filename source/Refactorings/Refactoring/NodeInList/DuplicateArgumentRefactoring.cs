// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring.MissingNodeInList
{
    internal class DuplicateArgumentRefactoring : DuplicateArgumentOrParameterRefactoring<ArgumentSyntax, ArgumentListSyntax>
    {
        public DuplicateArgumentRefactoring(ArgumentListSyntax nodeList)
            : base(nodeList)
        {
        }

        public override SyntaxToken GetCloseParenToken(ArgumentListSyntax nodeList)
        {
            return nodeList.CloseParenToken;
        }

        public override SeparatedSyntaxList<ArgumentSyntax> GetNodes(ArgumentListSyntax nodeList)
        {
            return nodeList.Arguments;
        }

        protected override string GetTitle()
        {
            return "Duplicate argument";
        }

        public override ArgumentOrParameterSyntaxRewriter<ArgumentSyntax> GetRewriter(ArgumentSyntax node, ArgumentSyntax newNode, SyntaxToken tokenBefore, SyntaxToken tokenAfter)
        {
            return new ArgumentSyntaxRewriter(node, newNode, tokenBefore, tokenAfter);
        }
    }
}
