// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis.UsePatternMatching
{
    internal class UsePatternMatchingWalker : CSharpSyntaxNodeWalker
    {
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

        public void Clear()
        {
            SetValues(default(IdentifierNameSyntax), default(SemanticModel), default(CancellationToken));
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

                if (_symbol.Equals(_semanticModel.GetSymbol(node, _cancellationToken)))
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
    }
}

