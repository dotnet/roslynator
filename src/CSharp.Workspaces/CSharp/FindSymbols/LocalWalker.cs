// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.FindSymbols
{
    internal class LocalWalker : CSharpSyntaxWalker
    {
        public LocalWalker()
            : base(SyntaxWalkerDepth.Node)
        {
        }

        public virtual void VisitLocal(SyntaxNode node)
        {
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            VisitLocal(node);
            base.VisitSimpleLambdaExpression(node);
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            VisitLocal(node);
            base.VisitParenthesizedLambdaExpression(node);
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            VisitLocal(node);
            base.VisitAnonymousMethodExpression(node);
        }

        //TODO: VisitAnonymousObjectMemberDeclarator
        //public override void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        //{
        //    if (node.NameEquals != null)
        //        VisitLocal(node);

        //    base.VisitAnonymousObjectMemberDeclarator(node);
        //}

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            VisitLocal(node);
            base.VisitLocalFunctionStatement(node);
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            VisitLocal(node);
            base.VisitVariableDeclarator(node);
        }

        public override void VisitSingleVariableDesignation(SingleVariableDesignationSyntax node)
        {
            VisitLocal(node);
            base.VisitSingleVariableDesignation(node);
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            if (node.Identifier != default)
                VisitLocal(node);

            base.VisitCatchDeclaration(node);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            VisitLocal(node);
            base.VisitForEachStatement(node);
        }
    }
}
