// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.NodeInList
{
    internal class ParameterSyntaxRewriter : NodeSyntaxRewriter<ParameterSyntax>
    {
        public ParameterSyntaxRewriter(RewriterInfo<ParameterSyntax> info)
            : base(info)
        {
        }

        public override SyntaxNode VisitParameter(ParameterSyntax node)
        {
            if (node == Info.Node)
                return Info.NewNode;

            return base.VisitParameter(node);
        }
    }
}
