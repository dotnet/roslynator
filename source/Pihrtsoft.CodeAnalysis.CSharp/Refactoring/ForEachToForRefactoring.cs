// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ForEachToForRefactoring
    {
        private const string CounterIdentifierName = "i";

        public static bool CanRefactor(
            ForEachStatementSyntax forEachStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (forEachStatement.Expression == null)
                return false;

            if (!forEachStatement.Expression.IsAnyKind(SyntaxKind.IdentifierName, SyntaxKind.SimpleMemberAccessExpression))
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(forEachStatement.Expression, cancellationToken).Type;

            if (typeSymbol == null || typeSymbol.IsKind(SymbolKind.ErrorType))
                return false;

            ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

            return HasIndexer(typeSymbol);
        }

        private static bool HasIndexer(ITypeSymbol typeSymbol)
        {
            ImmutableArray<ISymbol> members = typeSymbol.GetMembers("get_Item");

            for (int i = 0; i < members.Length; i++)
            {
                if (IsIndexer(members[i]))
                    return true;
            }

            return false;
        }

        private static bool IsIndexer(ISymbol symbol)
        {
            if (!symbol.IsKind(SymbolKind.Method))
                return false;

            if (symbol.IsStatic)
                return false;

            var methodSymbol = (IMethodSymbol)symbol;

            if (methodSymbol.Parameters.Length != 1)
                return false;

            return methodSymbol.Parameters[0].Type.SpecialType == SpecialType.System_Int32;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ForEachStatementSyntax forEachStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ForStatementSyntax forStatement = await GetForStatementAsync(
                document,
                forEachStatement,
                cancellationToken);

            forStatement = forStatement
                 .WithTriviaFrom(forEachStatement)
                 .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(forEachStatement, forStatement);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<ForStatementSyntax> GetForStatementAsync(
            Document document,
            ForEachStatementSyntax forEachStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (forEachStatement.Expression == null)
                throw new ArgumentException("foreach statement's expression cannot be null.", nameof(forEachStatement));

            SyntaxNode root = await document.GetSyntaxRootAsync();

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(
                semanticModel.GetDeclaredSymbol(forEachStatement),
                document.Project.Solution,
                cancellationToken);

            return ForStatement(
                declaration: CreateDeclaration(),
                initializers: SeparatedList<ExpressionSyntax>(),
                condition: CreateCondition(forEachStatement, semanticModel),
                incrementors: SingletonSeparatedList<ExpressionSyntax>(CreateIncrementor()),
                statement: CreateStatement(root, forEachStatement, referencedSymbols));
        }

        private static PostfixUnaryExpressionSyntax CreateIncrementor()
        {
            return PostfixUnaryExpression(
                SyntaxKind.PostIncrementExpression,
                IdentifierName(CounterIdentifierName));
        }

        private static BinaryExpressionSyntax CreateCondition(ForEachStatementSyntax forEachStatement, SemanticModel semanticModel)
        {
            return BinaryExpression(
                SyntaxKind.LessThanExpression,
                IdentifierName(CounterIdentifierName),
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(forEachStatement.Expression.ToString()),
                    IdentifierName((HasLengthProperty(forEachStatement.Expression, semanticModel)) ? "Length" : "Count")));
        }

        private static VariableDeclarationSyntax CreateDeclaration()
        {
            return VariableDeclaration(
                PredefinedType(Token(SyntaxKind.IntKeyword)),
                SingletonSeparatedList(
                    VariableDeclarator(CounterIdentifierName)
                        .WithInitializer(
                            EqualsValueClause(
                                LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0))))));
        }

        private static StatementSyntax CreateStatement(SyntaxNode root, ForEachStatementSyntax forEachStatement, IEnumerable<ReferencedSymbol> referencedSymbols)
        {
            return forEachStatement.Statement.ReplaceNodes(
                GetIdentifiers(root, referencedSymbols),
                (f, g) =>
                {
                    return ElementAccessExpression(
                        IdentifierName(forEachStatement.Expression.ToString()),
                        BracketedArgumentList(
                            SingletonSeparatedList(Argument(IdentifierName(CounterIdentifierName))))
                    ).WithTriviaFrom(f);
                });
        }

        private static IEnumerable<IdentifierNameSyntax> GetIdentifiers(SyntaxNode root, IEnumerable<ReferencedSymbol> referencedSymbols)
        {
            foreach (ReferencedSymbol referencedSymbol in referencedSymbols)
            {
                foreach (ReferenceLocation item in referencedSymbol.Locations)
                {
                    yield return root
                        .FindNode(item.Location.SourceSpan, getInnermostNodeForTie: true)
                        .FirstAncestorOrSelf<IdentifierNameSyntax>();
                }
            }
        }

        private static bool HasLengthProperty(ExpressionSyntax expression, SemanticModel semanticModel)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(expression).Type;

            if (typeSymbol == null || typeSymbol.IsKind(SymbolKind.ErrorType))
                return false;

            if (typeSymbol.BaseType?.SpecialType == SpecialType.System_Array)
                return true;

            if (typeSymbol.SpecialType == SpecialType.System_String)
                return true;

            if (typeSymbol.Kind != SymbolKind.NamedType)
                return false;

            INamedTypeSymbol immutableArraySymbol = semanticModel.Compilation.GetTypeByMetadataName("System.Collections.Immutable.ImmutableArray`1");

            if (immutableArraySymbol == null)
                return false;

            var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

            return namedTypeSymbol.ConstructedFrom.Equals(immutableArraySymbol);
        }
    }
}
