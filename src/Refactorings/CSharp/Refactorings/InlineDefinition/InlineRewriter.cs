// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.InlineDefinition
{
    internal class InlineRewriter : CSharpSyntaxRewriter
    {
        private readonly Dictionary<SyntaxNode, object> _replacementMap;

        public InlineRewriter(Dictionary<SyntaxNode, object> replacementMap)
        {
            _replacementMap = replacementMap;
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (_replacementMap.TryGetValue(node, out object newValue))
            {
                var newNode = (ExpressionSyntax)newValue;

                if (!newNode.IsKind(SyntaxKind.IdentifierName))
                    newNode = newNode.Parenthesize();

                return newNode;
            }
            else
            {
                return base.VisitIdentifierName(node);
            }
        }

        public override SyntaxNode VisitParameter(ParameterSyntax node)
        {
            var newNode = (ParameterSyntax)base.VisitParameter(node);

            if (_replacementMap.TryGetValue(node, out object newValue))
            {
                return newNode.WithIdentifier(SyntaxFactory.Identifier(newValue.ToString()));
            }
            else
            {
                return newNode;
            }
        }

        public override SyntaxNode VisitTypeParameter(TypeParameterSyntax node)
        {
            var newNode = (TypeParameterSyntax)base.VisitTypeParameter(node);

            if (_replacementMap.TryGetValue(node, out object newValue))
            {
                return newNode.WithIdentifier(SyntaxFactory.Identifier(newValue.ToString()));
            }
            else
            {
                return newNode;
            }
        }

        public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            var newNode = (VariableDeclaratorSyntax)base.VisitVariableDeclarator(node);

            if (_replacementMap.TryGetValue(node, out object newValue))
            {
                return newNode.WithIdentifier(SyntaxFactory.Identifier(newValue.ToString()));
            }
            else
            {
                return newNode;
            }
        }

        public override SyntaxNode VisitSingleVariableDesignation(SingleVariableDesignationSyntax node)
        {
            var newNode = (SingleVariableDesignationSyntax)base.VisitSingleVariableDesignation(node);

            if (_replacementMap.TryGetValue(node, out object newValue))
            {
                return newNode.WithIdentifier(SyntaxFactory.Identifier(newValue.ToString()));
            }
            else
            {
                return newNode;
            }
        }

        public override SyntaxNode VisitForEachStatement(ForEachStatementSyntax node)
        {
            var newNode = (ForEachStatementSyntax)base.VisitForEachStatement(node);

            if (_replacementMap.TryGetValue(node, out object newValue))
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
