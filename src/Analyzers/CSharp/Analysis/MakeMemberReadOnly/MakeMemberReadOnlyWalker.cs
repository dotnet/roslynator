// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
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

        [ThreadStatic]
        private static MakeMemberReadOnlyWalker _cachedInstance;

        public SemanticModel SemanticModel { get; set; }

        public CancellationToken CancellationToken { get; set; }

        public Dictionary<string, (SyntaxNode, ISymbol)> Symbols { get; } = new Dictionary<string, (SyntaxNode, ISymbol)>();

        public static MakeMemberReadOnlyWalker GetInstance()
        {
            MakeMemberReadOnlyWalker walker = _cachedInstance;

            if (walker != null)
            {
                Debug.Assert(walker.Symbols.Count == 0);
                Debug.Assert(walker.SemanticModel == null);
                Debug.Assert(walker.CancellationToken == default);

                _cachedInstance = null;
                return walker;
            }

            return new MakeMemberReadOnlyWalker();
        }

        public static void Free(MakeMemberReadOnlyWalker walker)
        {
            walker.Reset();
            _cachedInstance = walker;
        }

        private void Reset()
        {
            Symbols.Clear();
            SemanticModel = null;
            CancellationToken = default;
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
                AnalyzeAssigned((IdentifierNameSyntax)expression);
            }
            else if (kind == SyntaxKind.SimpleMemberAccessExpression)
            {
                var memberAccessExpression = (MemberAccessExpressionSyntax)expression;

                if (memberAccessExpression.Name is IdentifierNameSyntax identifierName)
                {
                    AnalyzeAssigned(identifierName, isInInstanceConstructor: memberAccessExpression.Expression.IsKind(SyntaxKind.ThisExpression));
                }
            }
        }

        private void AnalyzeAssigned(IdentifierNameSyntax identifierName, bool isInInstanceConstructor = true)
        {
            if (Symbols.TryGetValue(identifierName.Identifier.ValueText, out (SyntaxNode node, ISymbol symbol) nodeAndSymbol))
            {
                ISymbol symbol = nodeAndSymbol.symbol;

                if (_localFunctionDepth == 0
                    && _anonymousFunctionDepth == 0
                    && ((symbol.IsStatic) ? _isInStaticConstructor : isInInstanceConstructor && _isInInstanceConstructor))
                {
                    return;
                }

                ISymbol symbol2 = SemanticModel.GetSymbol(identifierName, CancellationToken)?.OriginalDefinition;

                if (symbol.Equals(symbol2))
                    Symbols.Remove(symbol.Name);
            }
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

        public override void VisitRefExpression(RefExpressionSyntax node)
        {
            ExpressionSyntax expression = node.Expression;

            if (expression != null)
            {
                VisitAssignedExpression(expression);
            }
            else
            {
                base.VisitRefExpression(node);
            }
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
