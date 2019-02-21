// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxWalkers
{
    internal class MethodReferencedAsMethodGroupWalker : CSharpSyntaxNodeWalker
    {
        public bool Result { get; set; }

        public IMethodSymbol Symbol { get; set; }

        public SemanticModel SemanticModel { get; set; }

        public CancellationToken CancellationToken { get; set; }

        protected override bool ShouldVisit => !Result;

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            CancellationToken.ThrowIfCancellationRequested();

            if (string.Equals(Symbol.Name, node.Identifier.ValueText, StringComparison.Ordinal)
                && !IsInvoked(node)
                && SemanticModel.GetSymbol(node, CancellationToken)?.Equals(Symbol) == true)
            {
                Result = true;
            }

            bool IsInvoked(IdentifierNameSyntax identifierName)
            {
                SyntaxNode parent = identifierName.Parent;

                switch (parent.Kind())
                {
                    case SyntaxKind.InvocationExpression:
                        {
                            return true;
                        }
                    case SyntaxKind.SimpleMemberAccessExpression:
                    case SyntaxKind.MemberBindingExpression:
                        {
                            if (parent.IsParentKind(SyntaxKind.InvocationExpression))
                                return true;

                            break;
                        }
                }

                return false;
            }
        }

        public static bool IsReferencedAsMethodGroup(
            MethodDeclarationSyntax methodDeclaration,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            var typeDeclaration = (TypeDeclarationSyntax)methodDeclaration.Parent;

            return IsReferencedAsMethodGroup(typeDeclaration, methodSymbol, semanticModel, cancellationToken);
        }

        public static bool IsReferencedAsMethodGroup(
            LocalFunctionStatementSyntax localFunction,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            MemberDeclarationSyntax memberDeclaration = localFunction.FirstAncestor<MemberDeclarationSyntax>();

            return IsReferencedAsMethodGroup(memberDeclaration, methodSymbol, semanticModel, cancellationToken);
        }

        public static bool IsReferencedAsMethodGroup(
            SyntaxNode node,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            bool result = false;

            MethodReferencedAsMethodGroupWalker walker = null;

            try
            {
                walker = Cache.GetInstance();

                Debug.Assert(walker.Symbol == null, "");
                Debug.Assert(walker.SemanticModel == null, "");

                walker.Symbol = methodSymbol;
                walker.SemanticModel = semanticModel;
                walker.CancellationToken = cancellationToken;

                walker.Visit(node);
            }
            finally
            {
                if (walker != null)
                {
                    result = walker.Result;
                    Cache.Free(walker);
                }
            }

            return result;
        }

        private static class Cache
        {
            [ThreadStatic]
            private static MethodReferencedAsMethodGroupWalker _cachedInstance;

            public static MethodReferencedAsMethodGroupWalker GetInstance()
            {
                MethodReferencedAsMethodGroupWalker walker = _cachedInstance;

                if (walker != null)
                {
                    _cachedInstance = null;
                    return walker;
                }

                return new MethodReferencedAsMethodGroupWalker();
            }

            public static void Free(MethodReferencedAsMethodGroupWalker walker)
            {
                walker.Result = false;
                walker.Symbol = null;
                walker.SemanticModel = null;
                walker.CancellationToken = default;

                _cachedInstance = walker;
            }
        }
    }
}
