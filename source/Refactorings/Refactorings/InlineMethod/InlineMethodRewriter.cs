// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.InlineMethod
{
    internal class InlineMethodRewriter : CSharpSyntaxRewriter
    {
        private readonly Dictionary<SyntaxNode, object> _replacementMap;

        public InlineMethodRewriter(Dictionary<SyntaxNode, object> replacementMap)
        {
            _replacementMap = replacementMap;
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            object newValue;
            if (_replacementMap.TryGetValue(node, out newValue))
            {
                return ((ExpressionSyntax)newValue).Parenthesize(moveTrivia: true).WithSimplifierAnnotation();
            }
            else
            {
                return base.VisitIdentifierName(node);
            }
        }

        public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            var newNode = (VariableDeclaratorSyntax)base.VisitVariableDeclarator(node);

            object newValue;
            if (_replacementMap.TryGetValue(node, out newValue))
            {
                return newNode.WithIdentifier(SyntaxFactory.Identifier(newValue.ToString()));
            }
            else
            {
                return newNode;
            }
        }
    }
}
