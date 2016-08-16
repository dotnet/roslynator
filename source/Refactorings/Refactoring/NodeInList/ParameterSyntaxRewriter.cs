// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring.MissingNodeInList
{
    internal class ParameterSyntaxRewriter : ArgumentOrParameterSyntaxRewriter<ParameterSyntax>
    {
        public ParameterSyntaxRewriter(
            ParameterSyntax parameter,
            ParameterSyntax newParameter,
            SyntaxToken tokenBefore,
            SyntaxToken tokenAfter) : base(parameter, newParameter, tokenBefore, tokenAfter)
        {
        }

        public override SyntaxNode VisitParameter(ParameterSyntax node)
        {
            if (node == Node)
                return NewNode;

            return base.VisitParameter(node);
        }
    }
}
