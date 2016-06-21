// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
    public static class ForEachToForRefactoring
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

            if (forEachStatement.Expression?.IsAnyKind(
                SyntaxKind.IdentifierName,
                SyntaxKind.SimpleMemberAccessExpression) == true)
            {
                ITypeSymbol typeSymbol = semanticModel
                    .GetTypeInfo(forEachStatement.Expression, cancellationToken)
                    .Type;

                if (typeSymbol?.IsKind(SymbolKind.ErrorType) == false)
                {
                    return typeSymbol.IsKind(SymbolKind.ArrayType)
                       || typeSymbol.SpecialType == SpecialType.System_String
                       || typeSymbol.HasPublicIndexer();
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ForEachStatementSyntax forEachStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(
                semanticModel.GetDeclaredSymbol(forEachStatement),
                document.Project.Solution,
                cancellationToken);

            ForStatementSyntax forStatement = ForStatement(
                declaration: VariableDeclaration(
                    PredefinedType(Token(SyntaxKind.IntKeyword)),
                    SingletonSeparatedList(
                        VariableDeclarator(CounterIdentifierName)
                            .WithInitializer(
                                EqualsValueClause(
                                    LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        Literal(0)))))),
                initializers: SeparatedList<ExpressionSyntax>(),
                condition: BinaryExpression(
                    SyntaxKind.LessThanExpression,
                    IdentifierName(CounterIdentifierName),
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(forEachStatement.Expression.ToString()),
                        IdentifierName(GetCountOrLengthPropertyName(forEachStatement.Expression, semanticModel)))),
                incrementors: SingletonSeparatedList<ExpressionSyntax>(
                    PostfixUnaryExpression(
                        SyntaxKind.PostIncrementExpression,
                        IdentifierName(CounterIdentifierName))),
                statement: forEachStatement.Statement.ReplaceNodes(
                    GetIdentifiers(root, referencedSymbols),
                    (f, g) =>
                    {
                        return ElementAccessExpression(
                            IdentifierName(forEachStatement.Expression.ToString()),
                            BracketedArgumentList(
                                SingletonSeparatedList(Argument(IdentifierName(CounterIdentifierName))))
                        ).WithTriviaFrom(f);
                    }));

            forStatement = forStatement
                 .WithTriviaFrom(forEachStatement)
                 .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(forEachStatement, forStatement);

            return document.WithSyntaxRoot(root);
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

        private static string GetCountOrLengthPropertyName(ExpressionSyntax expression, SemanticModel semanticModel)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(expression).Type;

            if (typeSymbol?.IsKind(SymbolKind.ErrorType) == false)
            {
                if (typeSymbol.SpecialType == SpecialType.System_String)
                    return "Length";

                if (typeSymbol.BaseType?.SpecialType == SpecialType.System_Array)
                    return "Length";

                if (typeSymbol.IsKind(SymbolKind.NamedType))
                {
                    INamedTypeSymbol immutableArraySymbol = semanticModel.Compilation.GetTypeByMetadataName("System.Collections.Immutable.ImmutableArray`1");

                    if (immutableArraySymbol != null
                        && ((INamedTypeSymbol)typeSymbol).ConstructedFrom.Equals(immutableArraySymbol))
                    {
                        return "Length";
                    }
                }
            }

            return "Count";
        }
    }
}
