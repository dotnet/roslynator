// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IntroduceFieldRefactoring
    {
        internal static Task<Document> RefactorAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax containingMember = expressionStatement.FirstAncestor<MemberDeclarationSyntax>();

            var containingType = (TypeDeclarationSyntax)containingMember.Parent;

            string name = NameGenerator.CreateName(typeSymbol, firstCharToLower: true) ?? DefaultNames.Variable;

            if (RefactoringSettings.Current.PrefixFieldIdentifierWithUnderscore)
                name = "_" + name;

            name = NameGenerator.Default.EnsureUniqueLocalName(
                name,
                semanticModel,
                expressionStatement.SpanStart,
                cancellationToken: cancellationToken);

            name = NameGenerator.Default.EnsureUniqueMemberName(
                name,
                semanticModel,
                containingType.OpenBraceToken.Span.End,
                cancellationToken: cancellationToken);

            ExpressionSyntax expression = expressionStatement.Expression;

            ExpressionStatementSyntax newExpressionStatement = ExpressionStatement(
                SimpleAssignmentExpression(
                    IdentifierName(Identifier(name).WithRenameAnnotation()),
                    expression.WithoutTrivia()).WithTriviaFrom(expression));

            newExpressionStatement = newExpressionStatement
                .WithTriviaFrom(expressionStatement)
                .WithFormatterAnnotation();

            FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(
                (SyntaxInfo.ModifierListInfo(containingMember).IsStatic) ? Modifiers.PrivateStatic() : Modifiers.Private(),
                typeSymbol.ToMinimalTypeSyntax(semanticModel, containingType.OpenBraceToken.Span.End),
                name);

            fieldDeclaration = fieldDeclaration.WithFormatterAnnotation();

            TypeDeclarationSyntax newNode = containingType.ReplaceNode(expressionStatement, newExpressionStatement);

            newNode = MemberDeclarationInserter.Default.Insert(newNode, fieldDeclaration);

            return document.ReplaceNodeAsync(containingType, newNode, cancellationToken);
        }
    }
}