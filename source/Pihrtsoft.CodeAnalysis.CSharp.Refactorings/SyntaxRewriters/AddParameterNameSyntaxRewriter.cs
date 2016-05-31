// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters
{
    internal class AddParameterNameSyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _semanticModel;
        private readonly ArgumentSyntax[] _arguments;

        private AddParameterNameSyntaxRewriter(ArgumentSyntax[] arguments, SemanticModel semanticModel)
        {
            _arguments = arguments;
            _semanticModel = semanticModel;
        }

        public static ArgumentListSyntax VisitNode(ArgumentListSyntax argumentList, ArgumentSyntax[] arguments, SemanticModel semanticModel)
        {
            return (ArgumentListSyntax)new AddParameterNameSyntaxRewriter(arguments, semanticModel).Visit(argumentList);
        }

        public override SyntaxNode VisitArgument(ArgumentSyntax node)
        {
            if (Array.IndexOf(_arguments, node) != -1)
                return ArgumentRefactoring.AddParameterName(node, _semanticModel);

            return base.VisitArgument(node);
        }
    }
}
