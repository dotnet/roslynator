// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis.UsePatternMatching
{
    internal class UsePatternMatchingWalker : CSharpSyntaxNodeWalker
    {
        [ThreadStatic]
        private static UsePatternMatchingWalker _cachedInstance;

        private ISymbol _symbol;
        private IdentifierNameSyntax _identifierName;
        private string _name;
        private SemanticModel _semanticModel;
        private CancellationToken _cancellationToken;

        public bool? IsFixable { get; private set; }

        protected override bool ShouldVisit
        {
            get { return IsFixable != false; }
        }

        public void SetValues(
            IdentifierNameSyntax identifierName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            IsFixable = null;

            _symbol = null;
            _name = identifierName?.Identifier.ValueText;
            _identifierName = identifierName;
            _semanticModel = semanticModel;
            _cancellationToken = cancellationToken;
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            if (string.Equals(node.Identifier.ValueText, _name))
            {
                if (_symbol == null)
                {
                    _symbol = _semanticModel.GetSymbol(_identifierName, _cancellationToken);

                    if (_symbol?.IsErrorType() != false)
                    {
                        IsFixable = false;
                        return;
                    }
                }

                if (SymbolEqualityComparer.Default.Equals(_symbol, _semanticModel.GetSymbol(node, _cancellationToken)))
                {
                    ExpressionSyntax n = node;

                    if (n.IsParentKind(SyntaxKind.SimpleMemberAccessExpression)
                        && ((MemberAccessExpressionSyntax)n.Parent).Expression.IsKind(SyntaxKind.ThisExpression))
                    {
                        n = (ExpressionSyntax)n.Parent;
                    }

                    if (!n.WalkUpParentheses().IsParentKind(SyntaxKind.CastExpression))
                    {
                        IsFixable = false;
                        return;
                    }

                    IsFixable = true;
                }
            }
        }

        public static UsePatternMatchingWalker GetInstance()
        {
            UsePatternMatchingWalker walker = _cachedInstance;

            if (walker != null)
            {
                Debug.Assert(walker._symbol == null);
                Debug.Assert(walker._identifierName == null);
                Debug.Assert(walker._semanticModel == null);
                Debug.Assert(walker._cancellationToken == default);

                _cachedInstance = null;
                return walker;
            }

            return new UsePatternMatchingWalker();
        }

        public static void Free(UsePatternMatchingWalker walker)
        {
            walker.SetValues(default(IdentifierNameSyntax), default(SemanticModel), default(CancellationToken));

            _cachedInstance = walker;
        }
    }
}
