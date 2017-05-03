// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings.MakeMemberReadOnly
{
    internal abstract class MakeMemberReadOnlyRefactoring
    {
        public abstract HashSet<ISymbol> GetAnalyzableSymbols(
            SymbolAnalysisContext context,
            INamedTypeSymbol containingType);

        public abstract void ReportFixableSymbols(
            SymbolAnalysisContext context,
            INamedTypeSymbol containingType,
            HashSet<ISymbol> symbols);

        public virtual HashSet<ISymbol> GetFixableSymbols(
            SymbolAnalysisContext context,
            INamedTypeSymbol containingType,
            HashSet<ISymbol> symbols)
        {
            CancellationToken cancellationToken = context.CancellationToken;
            ImmutableArray<SyntaxReference> syntaxReferences = containingType.DeclaringSyntaxReferences;

            for (int i = 0; i < syntaxReferences.Length; i++)
            {
                SyntaxNode syntax = syntaxReferences[i].GetSyntax(cancellationToken);

                SemanticModel semanticModel = context.Compilation.GetSemanticModel(syntaxReferences[i].SyntaxTree);

                foreach (SyntaxNode descendant in syntax.DescendantNodes())
                {
                    if (descendant.IsKind(SyntaxKind.IdentifierName))
                    {
                        var identifierName = (IdentifierNameSyntax)descendant;

                        ISymbol symbol = semanticModel.GetSymbol(identifierName, cancellationToken);

                        if (ValidateSymbol(symbol)
                            && symbols.Contains(symbol.OriginalDefinition))
                        {
                            ExpressionSyntax assignedExpression = GetAssignedExpression(descendant);

                            if (assignedExpression != null
                                && semanticModel.GetSymbol(assignedExpression, cancellationToken)?.Equals(symbol) == true
                                && !IsAssignmentThasIsAllowedForReadOnlyMember(assignedExpression, containingType, symbol.IsStatic, semanticModel, cancellationToken))
                            {
                                symbols.Remove(symbol.OriginalDefinition);
                            }
                        }
                    }
                }
            }

            return symbols;
        }

        protected virtual bool ValidateSymbol(ISymbol symbol)
        {
            return symbol != null;
        }

        public void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var typeSymbol = (INamedTypeSymbol)context.Symbol;

            if (typeSymbol.IsTypeKind(TypeKind.Class, TypeKind.Struct))
            {
                HashSet<ISymbol> symbols = GetAnalyzableSymbols(context, typeSymbol);

                if (symbols != null)
                {
                    symbols = GetFixableSymbols(context, typeSymbol, symbols);

                    if (symbols.Count > 0)
                        ReportFixableSymbols(context, typeSymbol, symbols);
                }
            }
        }

        protected virtual bool IsAssignmentThasIsAllowedForReadOnlyMember(
            SyntaxNode node,
            INamedTypeSymbol containingType,
            bool isStatic,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            while (node != null)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.ConstructorDeclaration:
                        {
                            ISymbol symbol = semanticModel.GetDeclaredSymbol(node, cancellationToken);

                            return symbol?.ContainingType == containingType
                                && symbol?.IsStatic == isStatic;
                        }
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                    case SyntaxKind.AnonymousMethodExpression:
                    case SyntaxKind.LocalFunctionStatement:
                        return false;
                }

                if (node is MemberDeclarationSyntax)
                    return false;

                node = node.Parent;
            }

            return false;
        }

        protected virtual ExpressionSyntax GetAssignedExpression(SyntaxNode node)
        {
            for (node = node.Parent; node != null; node = node.Parent)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.SimpleAssignmentExpression:
                    case SyntaxKind.AddAssignmentExpression:
                    case SyntaxKind.SubtractAssignmentExpression:
                    case SyntaxKind.MultiplyAssignmentExpression:
                    case SyntaxKind.DivideAssignmentExpression:
                    case SyntaxKind.ModuloAssignmentExpression:
                    case SyntaxKind.AndAssignmentExpression:
                    case SyntaxKind.ExclusiveOrAssignmentExpression:
                    case SyntaxKind.OrAssignmentExpression:
                    case SyntaxKind.LeftShiftAssignmentExpression:
                    case SyntaxKind.RightShiftAssignmentExpression:
                        return ((AssignmentExpressionSyntax)node).Left;
                    case SyntaxKind.PreIncrementExpression:
                    case SyntaxKind.PreDecrementExpression:
                        return ((PrefixUnaryExpressionSyntax)node).Operand;
                    case SyntaxKind.PostIncrementExpression:
                    case SyntaxKind.PostDecrementExpression:
                        return ((PostfixUnaryExpressionSyntax)node).Operand;
                    case SyntaxKind.Argument:
                        {
                            var argument = (ArgumentSyntax)node;

                            if (argument.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword, SyntaxKind.OutKeyword))
                                return argument.Expression;

                            break;
                        }
                    case SyntaxKind.Block:
                    case SyntaxKind.ArrowExpressionClause:
                        return null;
                }
            }

            return null;
        }
    }
}
