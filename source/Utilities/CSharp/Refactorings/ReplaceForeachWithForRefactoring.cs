// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class ReplaceForEachWithForRefactoring
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

            ITypeSymbol typeSymbol = semanticModel
                .GetTypeInfo(forEachStatement.Expression, cancellationToken)
                .Type;

            if (typeSymbol?.IsErrorType() == false)
            {
                return typeSymbol.IsArrayType()
                   || typeSymbol.IsString()
                   || typeSymbol.HasPublicIndexer();
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

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(
                semanticModel.GetDeclaredSymbol(forEachStatement),
                document.Project.Solution,
                cancellationToken).ConfigureAwait(false);

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
                 .WithFormatterAnnotation();

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

            if (typeSymbol?.IsErrorType() == false)
            {
                if (typeSymbol.IsString())
                    return "Length";

                if (typeSymbol.BaseType?.SpecialType == SpecialType.System_Array)
                    return "Length";

                if (typeSymbol.IsNamedType())
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
