// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring.MissingNodeInList
{
    internal class DuplicateParameterRefactoring : DuplicateArgumentOrParameterRefactoring<ParameterSyntax, ParameterListSyntax>
    {
        public DuplicateParameterRefactoring(ParameterListSyntax nodeList)
            : base(nodeList)
        {
        }

        public override SeparatedSyntaxList<ParameterSyntax> GetNodes(ParameterListSyntax nodeList)
        {
            return nodeList.Parameters;
        }

        public override SyntaxToken GetCloseParenToken(ParameterListSyntax nodeList)
        {
            return nodeList.CloseParenToken;
        }

        protected override string GetTitle()
        {
            return "Duplicate parameter";
        }

        public override ArgumentOrParameterSyntaxRewriter<ParameterSyntax> GetRewriter(ParameterSyntax node, ParameterSyntax newNode, SyntaxToken tokenBefore, SyntaxToken tokenAfter)
        {
            return new ParameterSyntaxRewriter(node, newNode, tokenBefore, tokenAfter);
        }
    }
}
