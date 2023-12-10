// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Documentation;
using Roslynator.CSharp.Refactorings;
using Roslynator.Documentation;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp;

internal static class DocumentRefactorings
{
    public static async Task<Document> ChangeTypeAsync(
        Document document,
        TypeSyntax type,
        ITypeSymbol typeSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        if (type.IsVar
            && type.Parent is DeclarationExpressionSyntax declarationExpression
            && declarationExpression.Designation is ParenthesizedVariableDesignationSyntax designation)
        {
#if DEBUG
            SyntaxNode parent = declarationExpression.Parent;

            switch (parent.Kind())
            {
                case SyntaxKind.SimpleAssignmentExpression:
                    {
                        var assignmentExpression = (AssignmentExpressionSyntax)parent;
                        Debug.Assert(object.ReferenceEquals(assignmentExpression.Left, declarationExpression));
                        break;
                    }
                case SyntaxKind.ForEachVariableStatement:
                    {
                        var forEachStatement = (ForEachVariableStatementSyntax)parent;
                        Debug.Assert(object.ReferenceEquals(forEachStatement.Variable, declarationExpression));
                        break;
                    }
                default:
                    {
                        SyntaxDebug.Fail(parent);
                        break;
                    }
            }
#endif
            TupleExpressionSyntax tupleExpression = CreateTupleExpression(typeSymbol, designation, GetSymbolDisplayFormat(semanticModel, type.SpanStart))
                .WithTriviaFrom(declarationExpression)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(declarationExpression, tupleExpression, cancellationToken).ConfigureAwait(false);
        }

        TypeSyntax newType = ChangeType(type, typeSymbol, semanticModel);

        return await document.ReplaceNodeAsync(type, newType, cancellationToken).ConfigureAwait(false);
    }

    private static SymbolDisplayFormat GetSymbolDisplayFormat(SemanticModel semanticModel, int position)
    {
        NullableContext context = semanticModel.GetNullableContext(position);

        return ((context & NullableContext.AnnotationsEnabled) != 0)
                ? SymbolDisplayFormats.FullName
                : SymbolDisplayFormats.FullName_WithoutNullableReferenceTypeModifier;
    }

    private static TypeSyntax ChangeType(TypeSyntax type, ITypeSymbol typeSymbol, SemanticModel semanticModel)
    {
        NullableContext context = semanticModel.GetNullableContext(type.SpanStart);

        TypeSyntax newType = typeSymbol
            .ToTypeSyntax(((context & NullableContext.AnnotationsEnabled) != 0)
                ? SymbolDisplayFormats.FullName
                : SymbolDisplayFormats.FullName_WithoutNullableReferenceTypeModifier)
            .WithTriviaFrom(type);

        if (newType is TupleTypeSyntax tupleType)
        {
            SeparatedSyntaxList<TupleElementSyntax> newElements = tupleType
                .Elements
                .Select(tupleElement => tupleElement.WithType(tupleElement.Type.WithSimplifierAnnotation()))
                .ToSeparatedSyntaxList();

            return tupleType.WithElements(newElements);
        }
        else
        {
            return newType.WithSimplifierAnnotation();
        }
    }

    private static TupleExpressionSyntax CreateTupleExpression(
        ITypeSymbol typeSymbol,
        ParenthesizedVariableDesignationSyntax designation,
        SymbolDisplayFormat format)
    {
        if (!typeSymbol.SupportsExplicitDeclaration())
            throw new ArgumentException($"Type '{typeSymbol.ToDisplayString()}' does not support explicit declaration.", nameof(typeSymbol));

        var tupleExpression = (TupleExpressionSyntax)ParseExpression(typeSymbol.ToDisplayString(format));

        SeparatedSyntaxList<VariableDesignationSyntax> variables = designation.Variables;
        SeparatedSyntaxList<ArgumentSyntax> arguments = tupleExpression.Arguments;
        SeparatedSyntaxList<ArgumentSyntax> newArguments = arguments.ForEach(argument =>
        {
            if (argument.Expression is DeclarationExpressionSyntax declarationExpression)
                return argument.WithExpression(declarationExpression.WithType(declarationExpression.Type.WithSimplifierAnnotation()));

            if (argument.Expression is PredefinedTypeSyntax or MemberAccessExpressionSyntax)
            {
                return argument.WithExpression(
                    DeclarationExpression(
                        ParseTypeName(argument.Expression.ToString()).WithSimplifierAnnotation(),
                        variables[arguments.IndexOf(argument)]));
            }

            SyntaxDebug.Fail(argument.Expression);

            return argument;
        });

        //SeparatedSyntaxList<ArgumentSyntax> newArguments = tupleExpression
        //    .Arguments
        //    .Select(argument =>
        //    {
        //        if (argument.Expression is DeclarationExpressionSyntax declarationExpression)
        //            return argument.WithExpression(declarationExpression.WithType(declarationExpression.Type.WithSimplifierAnnotation()));

        //        if (argument.Expression is PredefinedTypeSyntax or MemberAccessExpressionSyntax)
        //        {
        //            return argument.WithExpression(DeclarationExpression(ParseTypeName(argument.Expression.ToString()).WithSimplifierAnnotation(), DiscardDesignation()));
        //        }

        //        SyntaxDebug.Fail(argument.Expression);

        //        return argument;
        //    })
        //    .ToSeparatedSyntaxList();

        return tupleExpression.WithArguments(newArguments);
    }

    public static Task<Document> ChangeTypeToVarAsync(
        Document document,
        TypeSyntax type,
        CancellationToken cancellationToken = default)
    {
        IdentifierNameSyntax newType = VarType().WithTriviaFrom(type);

        return document.ReplaceNodeAsync(type, newType, cancellationToken);
    }

    public static Task<Document> ChangeTypeToVarAsync(
        Document document,
        TupleExpressionSyntax tupleExpression,
        CancellationToken cancellationToken = default)
    {
        SeparatedSyntaxList<VariableDesignationSyntax> variables = tupleExpression.Arguments
            .Select(f => f.Expression)
            .Cast<DeclarationExpressionSyntax>()
            .Select(f => f.Designation)
            .ToSeparatedSyntaxList();

        DeclarationExpressionSyntax declarationExpression = DeclarationExpression(
            VarType(),
            ParenthesizedVariableDesignation(variables))
            .WithTriviaFrom(tupleExpression)
            .WithFormatterAnnotation();

        return document.ReplaceNodeAsync(tupleExpression, declarationExpression, cancellationToken);
    }

    public static Task<Document> ChangeTypeAndAddAwaitAsync(
        Document document,
        VariableDeclarationSyntax variableDeclaration,
        VariableDeclaratorSyntax variableDeclarator,
        SyntaxNode containingDeclaration,
        ITypeSymbol newTypeSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        TypeSyntax type = variableDeclaration.Type;

        ExpressionSyntax value = variableDeclarator.Initializer.Value;

        AwaitExpressionSyntax newValue = AwaitExpression(value.WithoutTrivia()).WithTriviaFrom(value);

        TypeSyntax newType = ChangeType(type, newTypeSymbol, semanticModel);

        VariableDeclarationSyntax newVariableDeclaration = variableDeclaration
            .ReplaceNode(value, newValue)
            .WithType(newType);

        if (!SyntaxInfo.ModifierListInfo(containingDeclaration).IsAsync)
        {
            SyntaxNode newDeclaration = containingDeclaration
                .ReplaceNode(variableDeclaration, newVariableDeclaration)
                .InsertModifier(SyntaxKind.AsyncKeyword);

            return document.ReplaceNodeAsync(containingDeclaration, newDeclaration, cancellationToken);
        }

        return document.ReplaceNodeAsync(variableDeclaration, newVariableDeclaration, cancellationToken);
    }

    public static Task<Document> AddExplicitCastAsync(
        Document document,
        ExpressionSyntax expression,
        ITypeSymbol destinationType,
        CancellationToken cancellationToken = default)
    {
        TypeSyntax type = destinationType.ToTypeSyntax().WithSimplifierAnnotation();

        return AddExplicitCastAsync(document, expression, type, cancellationToken);
    }

    public static Task<Document> AddExplicitCastAsync(
        Document document,
        ExpressionSyntax expression,
        TypeSyntax destinationType,
        CancellationToken cancellationToken = default)
    {
        ExpressionSyntax newExpression = expression
            .WithoutTrivia()
            .Parenthesize();

        ExpressionSyntax newNode = CastExpression(destinationType, newExpression)
            .WithTriviaFrom(expression)
            .Parenthesize();

        return document.ReplaceNodeAsync(expression, newNode, cancellationToken);
    }

    public static Task<Document> RemoveAsyncAwaitAsync(
        Document document,
        SyntaxToken asyncKeyword,
        CancellationToken cancellationToken = default)
    {
        return RemoveAsyncAwait.RefactorAsync(document, asyncKeyword, cancellationToken);
    }

    public static async Task<Document> AddNewDocumentationCommentsAsync(
        Document document,
        DocumentationCommentGeneratorSettings settings = null,
        bool skipNamespaceDeclaration = true,
        CancellationToken cancellationToken = default)
    {
        SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        var rewriter = new AddNewDocumentationCommentRewriter(settings, skipNamespaceDeclaration);

        SyntaxNode newRoot = rewriter.Visit(root);

        return document.WithSyntaxRoot(newRoot);
    }

    public static async Task<Document> AddBaseOrNewDocumentationCommentsAsync(
        Document document,
        SemanticModel semanticModel,
        DocumentationCommentGeneratorSettings settings = null,
        bool skipNamespaceDeclaration = true,
        CancellationToken cancellationToken = default)
    {
        SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        var rewriter = new AddBaseOrNewDocumentationCommentRewriter(semanticModel, settings, skipNamespaceDeclaration, cancellationToken);

        SyntaxNode newRoot = rewriter.Visit(root);

        return document.WithSyntaxRoot(newRoot);
    }

    public static Task<Document> RemoveParenthesesAsync(
        Document document,
        ParenthesizedExpressionSyntax parenthesizedExpression,
        CancellationToken cancellationToken = default)
    {
        ExpressionSyntax expression = parenthesizedExpression.Expression;

        SyntaxTriviaList leading = parenthesizedExpression.GetLeadingTrivia()
            .Concat(parenthesizedExpression.OpenParenToken.TrailingTrivia)
            .Concat(expression.GetLeadingTrivia())
            .ToSyntaxTriviaList();

        SyntaxTriviaList trailing = expression.GetTrailingTrivia()
            .Concat(parenthesizedExpression.CloseParenToken.LeadingTrivia)
            .Concat(parenthesizedExpression.GetTrailingTrivia())
            .ToSyntaxTriviaList();

        ExpressionSyntax newExpression = expression
            .WithLeadingTrivia(leading)
            .WithTrailingTrivia(trailing)
            .WithFormatterAnnotation();

        if (!leading.Any())
        {
            SyntaxNode parent = parenthesizedExpression.Parent;

            switch (parent.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    {
                        var returnStatement = (ReturnStatementSyntax)parent;

                        SyntaxToken returnKeyword = returnStatement.ReturnKeyword;

                        if (!returnKeyword.TrailingTrivia.Any())
                        {
                            ReturnStatementSyntax newNode = returnStatement.Update(returnKeyword.WithTrailingTrivia(Space), newExpression, returnStatement.SemicolonToken);

                            return document.ReplaceNodeAsync(returnStatement, newNode, cancellationToken);
                        }

                        break;
                    }
                case SyntaxKind.YieldReturnStatement:
                    {
                        var yieldReturn = (YieldStatementSyntax)parent;

                        SyntaxToken returnKeyword = yieldReturn.ReturnOrBreakKeyword;

                        if (!returnKeyword.TrailingTrivia.Any())
                        {
                            YieldStatementSyntax newNode = yieldReturn.Update(yieldReturn.YieldKeyword, returnKeyword.WithTrailingTrivia(Space), newExpression, yieldReturn.SemicolonToken);

                            return document.ReplaceNodeAsync(yieldReturn, newNode, cancellationToken);
                        }

                        break;
                    }
                case SyntaxKind.AwaitExpression:
                    {
                        var awaitExpression = (AwaitExpressionSyntax)parent;

                        SyntaxToken awaitKeyword = awaitExpression.AwaitKeyword;

                        if (!awaitKeyword.TrailingTrivia.Any())
                        {
                            AwaitExpressionSyntax newNode = awaitExpression.Update(awaitKeyword.WithTrailingTrivia(Space), newExpression);

                            return document.ReplaceNodeAsync(awaitExpression, newNode, cancellationToken);
                        }

                        break;
                    }
            }
        }

        return document.ReplaceNodeAsync(parenthesizedExpression, newExpression, cancellationToken);
    }
}
