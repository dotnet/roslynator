// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters
{
    internal class AddParameterNameSyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _semanticModel;

        private AddParameterNameSyntaxRewriter(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        public static ArgumentListSyntax VisitNode(ArgumentListSyntax argumentList, SemanticModel semanticModel)
        {
            return (ArgumentListSyntax)new AddParameterNameSyntaxRewriter(semanticModel).Visit(argumentList);
        }

        public override SyntaxNode VisitArgument(ArgumentSyntax node)
        {
            return ArgumentRefactoring.AddParameterName(node, _semanticModel);
        }
    }
}
