// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis.MakeMemberReadOnly
{
    internal class MakeMemberReadOnlyWalker : AssignedExpressionWalker
    {
        private int _classOrStructDepth;
        private int _localFunctionDepth;
        private int _anonymousFunctionDepth;
        private bool _isInInstanceConstructor;
        private bool _isInStaticConstructor;

        public HashSet<AssignedInfo> Assigned { get; private set; }

        public void Clear()
        {
            Assigned?.Clear();
            _classOrStructDepth = 0;
            _localFunctionDepth = 0;
            _anonymousFunctionDepth = 0;
            _isInInstanceConstructor = false;
            _isInStaticConstructor = false;
        }

        public override void VisitAssignedExpression(ExpressionSyntax expression)
        {
            SyntaxKind kind = expression.Kind();

            if (kind == SyntaxKind.IdentifierName)
            {
                AddAssigned((IdentifierNameSyntax)expression);
            }
            else if (kind == SyntaxKind.SimpleMemberAccessExpression)
            {
                var memberAccessExpression = (MemberAccessExpressionSyntax)expression;

                if (memberAccessExpression.Name is IdentifierNameSyntax identifierName)
                    AddAssigned(identifierName);
            }
        }

        private void AddAssigned(IdentifierNameSyntax identifierName)
        {
            AssignedInfo info;

            if (_localFunctionDepth == 0
                && _anonymousFunctionDepth == 0)
            {
                info = new AssignedInfo(identifierName, _isInInstanceConstructor, _isInStaticConstructor);
            }
            else
            {
                info = new AssignedInfo(identifierName);
            }

            (Assigned ?? (Assigned = new HashSet<AssignedInfo>())).Add(info);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            if (_classOrStructDepth == 0)
            {
                if (node.Modifiers.Contains(SyntaxKind.StaticKeyword))
                {
                    _isInStaticConstructor = true;
                }
                else
                {
                    _isInInstanceConstructor = true;
                }
            }

            base.VisitConstructorDeclaration(node);

            _isInInstanceConstructor = false;
            _isInStaticConstructor = false;
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            _localFunctionDepth++;
            base.VisitLocalFunctionStatement(node);
            _localFunctionDepth--;
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            _anonymousFunctionDepth++;
            base.VisitSimpleLambdaExpression(node);
            _anonymousFunctionDepth--;
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            _anonymousFunctionDepth++;
            base.VisitParenthesizedLambdaExpression(node);
            _anonymousFunctionDepth--;
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            _anonymousFunctionDepth++;
            base.VisitAnonymousMethodExpression(node);
            _anonymousFunctionDepth--;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            _classOrStructDepth++;
            base.VisitClassDeclaration(node);
            _classOrStructDepth--;
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            _classOrStructDepth++;
            base.VisitStructDeclaration(node);
            _classOrStructDepth--;
        }
    }
}
