// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
        [ThreadStatic]
        private static MethodReferencedAsMethodGroupWalker _cachedInstance;

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
                && SymbolEqualityComparer.Default.Equals(SemanticModel.GetSymbol(node, CancellationToken), Symbol))
            {
                Result = true;
            }

            static bool IsInvoked(IdentifierNameSyntax identifierName)
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
            var result = false;

            MethodReferencedAsMethodGroupWalker walker = null;

            try
            {
                walker = GetInstance();

                walker.Symbol = methodSymbol;
                walker.SemanticModel = semanticModel;
                walker.CancellationToken = cancellationToken;

                walker.Visit(node);

                result = walker.Result;
            }
            finally
            {
                if (walker != null)
                    Free(walker);
            }

            return result;
        }

        private static MethodReferencedAsMethodGroupWalker GetInstance()
        {
            MethodReferencedAsMethodGroupWalker walker = _cachedInstance;

            if (walker != null)
            {
                Debug.Assert(walker.Symbol == null);
                Debug.Assert(walker.SemanticModel == null);
                Debug.Assert(walker.CancellationToken == default);

                _cachedInstance = null;
                return walker;
            }

            return new MethodReferencedAsMethodGroupWalker();
        }

        private static void Free(MethodReferencedAsMethodGroupWalker walker)
        {
            walker.Result = false;
            walker.Symbol = null;
            walker.SemanticModel = null;
            walker.CancellationToken = default;

            _cachedInstance = walker;
        }
    }
}
