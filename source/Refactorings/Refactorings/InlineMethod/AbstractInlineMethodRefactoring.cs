// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.InlineMethod
{
    internal abstract class AbstractInlineMethodRefactoring
    {
        protected AbstractInlineMethodRefactoring(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            INamedTypeSymbol invocationEnclosingType,
            IMethodSymbol methodSymbol,
            MethodDeclarationSyntax methodDeclaration,
            ImmutableArray<ParameterInfo> parameterInfos,
            SemanticModel invocationSemanticModel,
            SemanticModel declarationSemanticModel,
            CancellationToken cancellationToken)
        {
            Document = document;
            InvocationExpression = invocationExpression;
            InvocationEnclosingType = invocationEnclosingType;
            MethodSymbol = methodSymbol;
            MethodDeclaration = methodDeclaration;
            ParameterInfos = parameterInfos;
            InvocationSemanticModel = invocationSemanticModel;
            DeclarationSemanticModel = declarationSemanticModel;
            CancellationToken = cancellationToken;
        }

        public Document Document { get; }

        public InvocationExpressionSyntax InvocationExpression { get; }

        public INamedTypeSymbol InvocationEnclosingType { get; }

        public IMethodSymbol MethodSymbol { get; }

        public MethodDeclarationSyntax MethodDeclaration { get; }

        public ImmutableArray<ParameterInfo> ParameterInfos { get; }

        public SemanticModel InvocationSemanticModel { get; }

        public SemanticModel DeclarationSemanticModel { get; }

        public CancellationToken CancellationToken { get; }

        public TNode RewriteNode<TNode>(TNode node) where TNode : SyntaxNode
        {
            Dictionary<ISymbol, string> symbolMap = GetSymbolsToRename();

            Dictionary<SyntaxNode, object> replacementMap = GetReplacementMap(node, symbolMap);

            var rewriter = new InlineMethodRewriter(replacementMap);

            return (TNode)rewriter.Visit(node);
        }

        private Dictionary<SyntaxNode, object> GetReplacementMap(SyntaxNode node, Dictionary<ISymbol, string> symbolMap)
        {
            var replacementMap = new Dictionary<SyntaxNode, object>();

            foreach (SyntaxNode descendant in node.DescendantNodes(node.Span))
            {
                SyntaxKind kind = descendant.Kind();

                if (kind == SyntaxKind.IdentifierName)
                {
                    var identifierName = (IdentifierNameSyntax)descendant;

                    ISymbol symbol = DeclarationSemanticModel.GetSymbol(identifierName, CancellationToken);

                    if (symbol != null)
                    {
                        if (symbol.IsParameter())
                        {
                            foreach (ParameterInfo parameterInfo in ParameterInfos)
                            {
                                if (parameterInfo.ParameterSymbol.OriginalDefinition.Equals(symbol))
                                {
                                    replacementMap.Add(identifierName, parameterInfo.Expression);
                                    break;
                                }
                            }
                        }
                        else if (symbol.IsTypeParameter())
                        {
                            var typeParameter = (ITypeParameterSymbol)symbol;

                            ImmutableArray<ITypeSymbol> typeArguments = MethodSymbol.TypeArguments;

                            if (typeArguments.Length > typeParameter.Ordinal)
                                replacementMap.Add(identifierName, typeArguments[typeParameter.Ordinal].ToMinimalTypeSyntax(DeclarationSemanticModel, identifierName.SpanStart));
                        }
                        else if (symbol.IsStatic
                            && !identifierName.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                        {
                            INamedTypeSymbol containingType = symbol.ContainingType;

                            if (!InvocationEnclosingType
                                .BaseTypesAndSelf()
                                .Any(f => f.Equals(containingType)))
                            {
                                replacementMap.Add(identifierName, CSharpFactory.SimpleMemberAccessExpression(containingType.ToTypeSyntax().WithSimplifierAnnotation(), identifierName));
                            }
                        }

                        if (symbolMap != null
                            && symbolMap.TryGetValue(symbol, out string name))
                        {
                            replacementMap.Add(identifierName, SyntaxFactory.IdentifierName(name));
                        }
                    }
                }
                else if (symbolMap != null)
                {
                    if (kind.Is(
                        SyntaxKind.VariableDeclarator,
                        SyntaxKind.SingleVariableDesignation,
                        SyntaxKind.Parameter,
                        SyntaxKind.TypeParameter))
                    {
                        ISymbol symbol = DeclarationSemanticModel.GetDeclaredSymbol(descendant, CancellationToken);

                        if (symbolMap.TryGetValue(symbol, out string name))
                            replacementMap.Add(descendant, name);
                    }
                }
            }

            return replacementMap;
        }

        private Dictionary<ISymbol, string> GetSymbolsToRename()
        {
            ImmutableArray<ISymbol> declarationSymbols = DeclarationSemanticModel.GetDeclaredSymbols(
                MethodDeclaration.BodyOrExpressionBody(),
                excludeAnonymousTypeProperty: true,
                cancellationToken: CancellationToken);

            if (!declarationSymbols.Any())
                return null;

            declarationSymbols = declarationSymbols.RemoveAll( f => f.IsKind(SymbolKind.NamedType, SymbolKind.Field));

            if (!declarationSymbols.Any())
                return null;

            ImmutableArray<ISymbol> invocationSymbols = InvocationSemanticModel.GetSymbolsDeclaredInEnclosingSymbol(
                InvocationExpression.SpanStart,
                excludeAnonymousTypeProperty: true,
                cancellationToken: CancellationToken);

            invocationSymbols = invocationSymbols.AddRange(InvocationSemanticModel.LookupSymbols(InvocationExpression.SpanStart));

            var reservedNames = new HashSet<string>(invocationSymbols.Select(f => f.Name));

            List<ISymbol> symbols = null;

            foreach (ISymbol symbol in declarationSymbols)
            {
                if (reservedNames.Contains(symbol.Name))
                    (symbols ?? (symbols = new List<ISymbol>())).Add(symbol);
            }

            if (symbols == null)
                return null;

            reservedNames.UnionWith(declarationSymbols.Select(f => f.Name));

            var symbolMap = new Dictionary<ISymbol, string>();

            foreach (ISymbol symbol in symbols)
            {
                string newName = NameGenerator.Default.EnsureUniqueName(symbol.Name, reservedNames);

                symbolMap.Add(symbol, newName);

                reservedNames.Add(newName);
            }

            return symbolMap;
        }
    }
}
