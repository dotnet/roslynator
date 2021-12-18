// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.NodeInList
{
    internal class CopyParameterRefactoring : CopyArgumentOrParameterRefactoring<ParameterSyntax, ParameterListSyntax>
    {
        public CopyParameterRefactoring(ParameterListSyntax listSyntax)
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
            return "Copy parameter";
        }

        protected override NodeSyntaxRewriter<ParameterSyntax> GetRewriter(RewriterInfo<ParameterSyntax> info)
        {
            return new ParameterSyntaxRewriter(info);
        }
    }
}
