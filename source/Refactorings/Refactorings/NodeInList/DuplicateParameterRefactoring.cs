// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.NodeInList
{
    internal class DuplicateParameterRefactoring : DuplicateArgumentOrParameterRefactoring<ParameterSyntax, ParameterListSyntax>
    {
        public DuplicateParameterRefactoring(ParameterListSyntax listSyntax)
            : base(listSyntax, listSyntax.Parameters)
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

        protected override string GetTitle(params string[] args)
        {
            return "Duplicate parameter";
        }

        protected override NodeSyntaxRewriter<ParameterSyntax> GetRewriter(RewriterInfo<ParameterSyntax> info)
        {
            return new ParameterSyntaxRewriter(info);
        }
    }
}
