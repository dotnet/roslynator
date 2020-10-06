// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal readonly struct MethodChain : IEnumerable<SyntaxNode>
    {
        public MethodChain(ExpressionSyntax expression)
        {
            Expression = expression;
        }

        public ExpressionSyntax Expression { get; }

        IEnumerator<SyntaxNode> IEnumerable<SyntaxNode>.GetEnumerator()
        {
            if (Expression != null)
                return new EnumeratorImpl(this);

            return Empty.Enumerator<SyntaxNode>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (Expression != null)
                return new EnumeratorImpl(this);

            return Empty.Enumerator<SyntaxNode>();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator
        {
            private readonly MethodChain _chain;
            private SyntaxNode _current;

            internal Enumerator(MethodChain chain)
            {
                _chain = chain;
                _current = null;
            }

            public bool MoveNext()
            {
                if (_current == null)
                {
                    _current = _chain.Expression;

                    return _current != null;
                }

                ExpressionSyntax last = GetLastChild(_current);

                if (last != null)
                {
                    _current = last;
                }
                else
                {
                    while (_current != _chain.Expression
                        && IsFirstChild(_current))
                    {
                        _current = _current.Parent;
                    }

                    if (_current == _chain.Expression)
                    {
                        _current = null;
                        return false;
                    }

                    _current = GetPreviousSibling(_current);
                }

                return true;
            }

            private static ExpressionSyntax GetLastChild(SyntaxNode node)
            {
                switch (node?.Kind())
                {
                    case SyntaxKind.ConditionalAccessExpression:
                        return ((ConditionalAccessExpressionSyntax)node).WhenNotNull;
                    case SyntaxKind.MemberBindingExpression:
                        return ((MemberBindingExpressionSyntax)node).Name;
                    case SyntaxKind.SimpleMemberAccessExpression:
                        return ((MemberAccessExpressionSyntax)node).Name;
                    case SyntaxKind.ElementAccessExpression:
                        return ((ElementAccessExpressionSyntax)node).Expression;
                    case SyntaxKind.InvocationExpression:
                        return ((InvocationExpressionSyntax)node).Expression;
                }

                return null;
            }

            private static SyntaxNode GetPreviousSibling(SyntaxNode node)
            {
                SyntaxNode parent = node.Parent;

                switch (parent.Kind())
                {
                    case SyntaxKind.ConditionalAccessExpression:
                        {
                            var conditionalAccess = (ConditionalAccessExpressionSyntax)parent;

                            if (conditionalAccess.WhenNotNull == node)
                                return conditionalAccess.Expression;

                            break;
                        }
                    case SyntaxKind.SimpleMemberAccessExpression:
                        {
                            var memberAccess = (MemberAccessExpressionSyntax)parent;

                            if (memberAccess.Name == node)
                                return memberAccess.Expression;

                            break;
                        }
                }

                return null;
            }

            private static bool IsFirstChild(SyntaxNode node)
            {
                SyntaxNode parent = node.Parent;

                switch (parent.Kind())
                {
                    case SyntaxKind.ConditionalAccessExpression:
                        return ((ConditionalAccessExpressionSyntax)parent).Expression == node;
                    case SyntaxKind.SimpleMemberAccessExpression:
                        return ((MemberAccessExpressionSyntax)parent).Expression == node;
                }

                return true;
            }

            public SyntaxNode Current => _current ?? throw new InvalidOperationException();

            public void Reset()
            {
                _current = null;
            }

            public override bool Equals(object obj) => throw new NotSupportedException();

            public override int GetHashCode() => throw new NotSupportedException();
        }

        private class EnumeratorImpl : IEnumerator<SyntaxNode>
        {
            private Enumerator _en;

            internal EnumeratorImpl(in MethodChain methodChain)
            {
                _en = new Enumerator(methodChain);
            }

            public SyntaxNode Current => _en.Current;

            object IEnumerator.Current => _en.Current;

            public bool MoveNext() => _en.MoveNext();

            void IEnumerator.Reset() => _en.Reset();

            void IDisposable.Dispose()
            {
            }
        }
    }
}
