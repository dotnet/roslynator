// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analyzers.UnnecessaryUnsafeContext
{
    internal class UnnecessaryUnsafeContextWalker : CSharpSyntaxWalker
    {
        private bool _isBeforeFirstVisit = true;

        public bool ContainsUnsafe { get; private set; }

        public void Reset()
        {
            _isBeforeFirstVisit = true;
            ContainsUnsafe = false;
        }

        public override void Visit(SyntaxNode node)
        {
            if (!ContainsUnsafe)
                base.Visit(node);
        }

        public override void VisitPointerType(PointerTypeSyntax node)
        {
            ContainsUnsafe = true;
        }

        public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            ContainsUnsafe = true;
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (node.Kind() == SyntaxKind.PointerMemberAccessExpression)
            {
                ContainsUnsafe = true;
            }
            else
            {
                base.VisitMemberAccessExpression(node);
            }
        }

        public override void VisitFixedStatement(FixedStatementSyntax node)
        {
            ContainsUnsafe = true;
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            if (node.Kind().Is(SyntaxKind.AddressOfExpression, SyntaxKind.PointerIndirectionExpression))
            {
                ContainsUnsafe = true;
            }
            else
            {
                base.VisitPrefixUnaryExpression(node);
            }
        }

        public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitLocalFunctionStatement(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitLocalFunctionStatement(node);
            }
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitClassDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitClassDeclaration(node);
            }
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitStructDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitStructDeclaration(node);
            }
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitInterfaceDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitInterfaceDeclaration(node);
            }
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitConstructorDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitConstructorDeclaration(node);
            }
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitConversionOperatorDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitConversionOperatorDeclaration(node);
            }
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitDelegateDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitDelegateDeclaration(node);
            }
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitDestructorDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitDestructorDeclaration(node);
            }
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitEventDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitEventDeclaration(node);
            }
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitEventFieldDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitEventFieldDeclaration(node);
            }
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitFieldDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitFieldDeclaration(node);
            }
        }

        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitIndexerDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitIndexerDeclaration(node);
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitMethodDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitMethodDeclaration(node);
            }
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitOperatorDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitOperatorDeclaration(node);
            }
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (_isBeforeFirstVisit)
            {
                _isBeforeFirstVisit = false;
                base.VisitPropertyDeclaration(node);
            }
            else if (!node.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
            {
                base.VisitPropertyDeclaration(node);
            }
        }
    }
}
