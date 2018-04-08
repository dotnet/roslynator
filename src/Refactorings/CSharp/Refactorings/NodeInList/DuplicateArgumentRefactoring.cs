// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.NodeInList
{
    internal class DuplicateArgumentRefactoring : DuplicateArgumentOrParameterRefactoring<ArgumentSyntax, ArgumentListSyntax>
    {
        public DuplicateArgumentRefactoring(ArgumentListSyntax listSyntax)
            : base(listSyntax, listSyntax.Arguments)
        {
        }

        public override SyntaxToken GetOpenParenToken()
        {
            return ListSyntax.OpenParenToken;
        }

        public override SyntaxToken GetCloseParenToken()
        {
            return ListSyntax.CloseParenToken;
        }

        protected override NodeSyntaxRewriter<ArgumentSyntax> GetRewriter(RewriterInfo<ArgumentSyntax> info)
        {
            return new ArgumentSyntaxRewriter(info);
        }

        protected override string GetTitle(params string[] args)
        {
            return "Duplicate argument";
        }
    }
}
