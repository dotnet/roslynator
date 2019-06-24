// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.InlineDefinition
{
    internal abstract class SingleInlineRefactoring<TNode, TDeclaration, TSymbol>
        where TNode : SyntaxNode
        where TDeclaration : MemberDeclarationSyntax
        where TSymbol : ISymbol
    {
        protected SingleInlineRefactoring(
            Document document,
            SyntaxNode node,
            INamedTypeSymbol nodeEnclosingType,
            TSymbol symbol,
            TDeclaration declaration,
            ImmutableArray<ParameterInfo> parameterInfos,
            SemanticModel invocationSemanticModel,
            SemanticModel declarationSemanticModel,
            CancellationToken cancellationToken)
        {
            Document = document;
            Node = node;
            NodeEnclosingType = nodeEnclosingType;
            Symbol = symbol;
            Declaration = declaration;
            ParameterInfos = parameterInfos;
            InvocationSemanticModel = invocationSemanticModel;
            DeclarationSemanticModel = declarationSemanticModel;
            CancellationToken = cancellationToken;
        }

        public abstract SyntaxNode BodyOrExpressionBody { get; }

        public abstract ImmutableArray<ITypeSymbol> TypeArguments { get; }

        public Document Document { get; }

        public SyntaxNode Node { get; }

        public INamedTypeSymbol NodeEnclosingType { get; }

        public TSymbol Symbol { get; }

        public TDeclaration Declaration { get; }

        public ImmutableArray<ParameterInfo> ParameterInfos { get; }

        public SemanticModel InvocationSemanticModel { get; }

        public SemanticModel DeclarationSemanticModel { get; }

        public CancellationToken CancellationToken { get; }

        public virtual Task<Document> InlineAsync(
            SyntaxNode node,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax newExpression = RewriteExpression(node, expression);

            return Document.ReplaceNodeAsync(node, newExpression, cancellationToken);
        }

        public ParenthesizedExpressionSyntax RewriteExpression(SyntaxNode node, ExpressionSyntax expression)
        {
            return RewriteNode(expression)
                .WithTriviaFrom(node)
                .Parenthesize()
                .WithFormatterAnnotation();
        }

        public virtual Task<Document> InlineAsync(
            ExpressionStatementSyntax expressionStatement,
            SyntaxList<StatementSyntax> statements,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            StatementSyntax[] newStatements = RewriteStatements(statements, expressionStatement.GetLeadingTrivia(), expressionStatement.GetTrailingTrivia());

            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(expressionStatement);

            if (statementsInfo.Success)
            {
                StatementListInfo newInfo = statementsInfo.WithStatements(statementsInfo.Statements.ReplaceRange(expressionStatement, newStatements));

                return Document.ReplaceNodeAsync(statementsInfo.Parent, newInfo.Parent, cancellationToken);
            }
            else
            {
                return Document.ReplaceNodeAsync(expressionStatement, Block(newStatements), cancellationToken);
            }
        }

        public StatementSyntax[] RewriteStatements(SyntaxList<StatementSyntax> statements, SyntaxTriviaList leadingTrivia, SyntaxTriviaList trailingTrivia)
        {
            var newStatements = new StatementSyntax[statements.Count];

            for (int i = 0; i < statements.Count; i++)
                newStatements[i] = RewriteNode(statements[i]).WithFormatterAnnotation();

            newStatements[0] = newStatements[0].WithLeadingTrivia(leadingTrivia);
            newStatements[statements.Count - 1] = newStatements[statements.Count - 1].WithTrailingTrivia(trailingTrivia);
            return newStatements;
        }

        private T RewriteNode<T>(T node) where T : SyntaxNode
        {
            Dictionary<ISymbol, string> symbolMap = GetSymbolsToRename();

            Dictionary<SyntaxNode, object> replacementMap = GetReplacementMap(node, symbolMap);

            var rewriter = new InlineRewriter(replacementMap);

            return (T)rewriter.Visit(node);
        }

        private Dictionary<SyntaxNode, object> GetReplacementMap(SyntaxNode node, Dictionary<ISymbol, string> symbolMap)
        {
            var replacementMap = new Dictionary<SyntaxNode, object>();

            foreach (SyntaxNode descendant in node.DescendantNodesAndSelf(node.Span))
            {
                SyntaxKind kind = descendant.Kind();

                if (kind == SyntaxKind.IdentifierName)
                {
                    var identifierName = (IdentifierNameSyntax)descendant;

                    ISymbol symbol = DeclarationSemanticModel.GetSymbol(identifierName, CancellationToken);

                    if (symbol != null)
                    {
                        if (symbol is IParameterSymbol parameterSymbol)
                        {
                            foreach (ParameterInfo parameterInfo in ParameterInfos)
                            {
                                if (ParameterEquals(parameterInfo, parameterSymbol))
                                {
                                    ExpressionSyntax expression = parameterInfo.Expression;

                                    if (expression == null
                                        && parameterInfo.ParameterSymbol.HasExplicitDefaultValue)
                                    {
                                        expression = parameterInfo.ParameterSymbol.GetDefaultValueMinimalSyntax(InvocationSemanticModel, Node.SpanStart);
                                    }
                                    else if (parameterInfo.ParameterSymbol.Type.TypeKind == TypeKind.Delegate
                                        && expression is AnonymousFunctionExpressionSyntax)
                                    {
                                        TypeSyntax parameterType =
                                            parameterInfo.ParameterSymbol.Type.ToMinimalTypeSyntax(
                                                InvocationSemanticModel,
                                                Node.SpanStart);
                                        expression =
                                            CastExpression(parameterType, ParenthesizedExpression(expression));
                                    }

                                    replacementMap.Add(identifierName, expression);

                                    break;
                                }
                            }
                        }
                        else if (symbol.Kind == SymbolKind.TypeParameter)
                        {
                            var typeParameter = (ITypeParameterSymbol)symbol;

                            ImmutableArray<ITypeSymbol> typeArguments = TypeArguments;

                            if (typeArguments.Length > typeParameter.Ordinal)
                                replacementMap.Add(identifierName, typeArguments[typeParameter.Ordinal].ToMinimalTypeSyntax(InvocationSemanticModel, Node.SpanStart));
                        }
                        else if (symbol.IsStatic
                            && !identifierName.IsParentKind(SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.QualifiedName))
                        {
                            INamedTypeSymbol containingType = symbol.ContainingType;

                            if (containingType != null)
                            {
                                if (!NodeEnclosingType
                                    .BaseTypesAndSelf()
                                    .Any(f => f.Equals(containingType)))
                                {
                                    replacementMap.Add(identifierName, CSharpFactory.SimpleMemberAccessExpression(containingType.ToTypeSyntax().WithSimplifierAnnotation(), identifierName));
                                }
                            }
                            else if (symbol is ITypeSymbol typeSymbol)
                            {
                                replacementMap.Add(identifierName, typeSymbol.ToMinimalTypeSyntax(InvocationSemanticModel, Node.SpanStart));
                            }
                        }

                        if (symbolMap != null
                            && symbolMap.TryGetValue(symbol, out string name))
                        {
                            replacementMap.Add(identifierName, IdentifierName(name));
                        }
                    }
                }
                else if (symbolMap != null)
                {
                    switch (kind)
                    {
                        case SyntaxKind.VariableDeclarator:
                        case SyntaxKind.SingleVariableDesignation:
                        case SyntaxKind.Parameter:
                        case SyntaxKind.TypeParameter:
                        case SyntaxKind.ForEachStatement:
                        case SyntaxKind.ForEachVariableStatement:
                            {
                                ISymbol symbol = DeclarationSemanticModel.GetDeclaredSymbol(descendant, CancellationToken);

                                Debug.Assert(symbol != null || (descendant as ForEachVariableStatementSyntax)?.Variable?.Kind() == SyntaxKind.TupleExpression, kind.ToString());

                                if (symbol != null
                                    && symbolMap.TryGetValue(symbol, out string name))
                                {
                                    replacementMap.Add(descendant, name);
                                }

                                break;
                            }
                    }
                }
            }

            return replacementMap;

            bool ParameterEquals(in ParameterInfo parameterInfo, IParameterSymbol parameterSymbol2)
            {
                IParameterSymbol parameterSymbol = parameterInfo.ParameterSymbol;

                if (parameterSymbol.ContainingSymbol is IMethodSymbol methodSymbol)
                {
                    if (parameterInfo.IsThis
                        || methodSymbol.MethodKind == MethodKind.ReducedExtension)
                    {
                        int ordinal = parameterSymbol.Ordinal;

                        if (methodSymbol.MethodKind == MethodKind.ReducedExtension)
                            ordinal++;

                        return ordinal == parameterSymbol2.Ordinal
                            && string.Equals(parameterSymbol.Name, parameterSymbol2.Name, StringComparison.Ordinal);
                    }
                }

                return parameterSymbol.OriginalDefinition.Equals(parameterSymbol2);
            }
        }

        private Dictionary<ISymbol, string> GetSymbolsToRename()
        {
            ImmutableArray<ISymbol> declarationSymbols = DeclarationSemanticModel.GetDeclaredSymbols(
                BodyOrExpressionBody,
                excludeAnonymousTypeProperty: true,
                cancellationToken: CancellationToken);

            if (!declarationSymbols.Any())
                return null;

            declarationSymbols = declarationSymbols.RemoveAll(f => f.IsKind(SymbolKind.NamedType, SymbolKind.Field));

            if (!declarationSymbols.Any())
                return null;

            int position = Node.SpanStart;

            ImmutableArray<ISymbol> invocationSymbols = InvocationSemanticModel.GetSymbolsDeclaredInEnclosingSymbol(
                position,
                excludeAnonymousTypeProperty: true,
                cancellationToken: CancellationToken);

            invocationSymbols = invocationSymbols.AddRange(InvocationSemanticModel.LookupSymbols(position));

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
