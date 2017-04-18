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
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceForEachWithForRefactoring
    {
        public static bool CanRefactor(
            ForEachStatementSyntax forEachStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(forEachStatement.Expression, cancellationToken);

            if (typeSymbol?.IsErrorType() == false)
            {
                return typeSymbol.IsString()
                   || typeSymbol.IsArrayType()
                   || typeSymbol.Implements(SpecialType.System_Collections_Generic_IList_T)
                   || (HasApplicableIndexer(typeSymbol, forEachStatement, semanticModel)
                        && typeSymbol.Implements(semanticModel.GetTypeByMetadataName(MetadataNames.System_Collections_ICollection)));
            }

            return false;
        }

        private static bool HasApplicableIndexer(
            ITypeSymbol containingType,
            ForEachStatementSyntax forEachStatement,
            SemanticModel semanticModel)
        {
            ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

            foreach (ISymbol member in containingType.GetMembers("this[]"))
            {
                var propertySymbol = (IPropertySymbol)member;

                if (!propertySymbol.IsWriteOnly
                    && semanticModel.IsAccessible(forEachStatement.SpanStart, propertySymbol.GetMethod)
                    && propertySymbol.Type.Equals(info.ElementType)
                    && propertySymbol.SingleParameterOrDefault()?.Type.IsInt() == true)
                {
                    return true;
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

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(
                semanticModel.GetDeclaredSymbol(forEachStatement),
                document.Project.Solution,
                cancellationToken).ConfigureAwait(false);

            string name = NameGenerator.Default.EnsureUniqueLocalName(
                DefaultNames.ForVariable,
                semanticModel,
                forEachStatement.Statement.SpanStart,
                cancellationToken: cancellationToken);

            SyntaxToken identifier = Identifier(name);

            if (name != DefaultNames.ForVariable)
                identifier = identifier.WithRenameAnnotation();

            ForStatementSyntax forStatement = ForStatement(
                declaration: VariableDeclaration(
                    IntType(),
                    identifier,
                    EqualsValueClause(NumericLiteralExpression(0))),
                initializers: default(SeparatedSyntaxList<ExpressionSyntax>),
                condition: LessThanExpression(
                    IdentifierName(name),
                    SimpleMemberAccessExpression(
                        IdentifierName(forEachStatement.Expression.ToString()),
                        IdentifierName(GetCountOrLengthPropertyName(forEachStatement.Expression, semanticModel, cancellationToken)))),
                incrementors: SingletonSeparatedList<ExpressionSyntax>(
                    PostIncrementExpression(
                        IdentifierName(name))),
                statement: forEachStatement.Statement.ReplaceNodes(
                    GetIdentifiers(root, referencedSymbols),
                    (f, g) =>
                    {
                        return ElementAccessExpression(
                            IdentifierName(forEachStatement.Expression.ToString()),
                            BracketedArgumentList(
                                SingletonSeparatedList(Argument(IdentifierName(name))))
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

        private static string GetCountOrLengthPropertyName(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            if (typeSymbol?.IsErrorType() == false)
            {
                if (typeSymbol.IsString()
                    || typeSymbol.IsArrayType()
                    || typeSymbol.IsConstructedFromImmutableArrayOfT(semanticModel))
                {
                    return "Length";
                }
            }

            return "Count";
        }
    }
}
