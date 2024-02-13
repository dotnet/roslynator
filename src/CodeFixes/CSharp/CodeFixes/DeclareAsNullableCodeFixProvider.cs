// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DeclareAsNullableCodeFixProvider))]
[Shared]
public sealed class DeclareAsNullableCodeFixProvider : CompilerDiagnosticCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS8600_ConvertingNullLiteralOrPossibleNullValueToNonNullableType); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];

        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddNullableAnnotation, context.Document, root.SyntaxTree))
            return;

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => f.IsKind(SyntaxKind.EqualsValueClause, SyntaxKind.DeclarationExpression, SyntaxKind.SimpleAssignmentExpression, SyntaxKind.CastExpression)))
            return;

        if (node is EqualsValueClauseSyntax equalsValueClause)
        {
            ExpressionSyntax expression = equalsValueClause.Value;

            if (expression.Span == context.Span
                && equalsValueClause.IsParentKind(SyntaxKind.VariableDeclarator)
                && equalsValueClause.Parent.Parent is VariableDeclarationSyntax variableDeclaration
                && variableDeclaration.Variables.Count == 1)
            {
                TryRegisterCodeFix(context, diagnostic, variableDeclaration.Type);
            }
        }
        else if (node is DeclarationExpressionSyntax declarationExpression)
        {
            TryRegisterCodeFix(context, diagnostic, declarationExpression.Type);
        }
        else if (node is AssignmentExpressionSyntax assignmentExpression)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            var localSymbol = semanticModel.GetSymbol(assignmentExpression.Left, context.CancellationToken) as ILocalSymbol;

            if (localSymbol is not null)
            {
                SyntaxNode declarator = await localSymbol.GetSyntaxAsync(context.CancellationToken).ConfigureAwait(false);

                if (declarator.IsKind(SyntaxKind.VariableDeclarator)
                    && declarator.Parent is VariableDeclarationSyntax variableDeclaration
                    && variableDeclaration.Variables.Count == 1)
                {
                    TryRegisterCodeFix(context, diagnostic, variableDeclaration.Type);
                }
            }
        }
        else if (node is CastExpressionSyntax castExpression)
        {
            TryRegisterCodeFixForCast(context, diagnostic, castExpression.Type);
        }
    }

    private static void TryRegisterCodeFix(CodeFixContext context, Diagnostic diagnostic, TypeSyntax type)
    {
        if (type.IsKind(SyntaxKind.NullableType))
            return;

        CodeAction codeAction = CodeAction.Create(
            "Declare as nullable",
            ct =>
            {
                NullableTypeSyntax newType = SyntaxFactory.NullableType(type.WithoutTrivia()).WithTriviaFrom(type);
                return context.Document.ReplaceNodeAsync(type, newType, ct);
            },
            GetEquivalenceKey(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    private static void TryRegisterCodeFixForCast(CodeFixContext context, Diagnostic diagnostic, TypeSyntax type)
    {
        if (type.IsKind(SyntaxKind.NullableType))
            return;

        CodeAction codeAction = CodeAction.Create(
            "Declare as nullable",
            async ct =>
            {
                NullableTypeSyntax newType = SyntaxFactory.NullableType(type.WithoutTrivia()).WithTriviaFrom(type);
                Document changedDocument = await context.Document.ReplaceNodeAsync(type, newType, ct).ConfigureAwait(false);

                // This could be in a variable declaration, so grab the new syntax root and find the newly-replaced type node
                SyntaxNode root = await changedDocument.GetSyntaxRootAsync(ct).ConfigureAwait(false);
                SyntaxNode insertedNewType = root.FindNode(type.Span);

                // Try finding a surrounding variable declaration whose type we also have to change
                if (insertedNewType.AncestorsAndSelf().FirstOrDefault(a => a.IsKind(SyntaxKind.EqualsValueClause)) is EqualsValueClauseSyntax equalsValueClause)
                {
                    ExpressionSyntax expression = equalsValueClause.Value;

                    if (equalsValueClause.IsParentKind(SyntaxKind.VariableDeclarator)
                        && equalsValueClause.Parent.Parent is VariableDeclarationSyntax variableDeclaration
                        && variableDeclaration.Variables.Count == 1
                        && !variableDeclaration.Type.IsKind(SyntaxKind.NullableType)
                        && variableDeclaration.Type is not IdentifierNameSyntax { Identifier.Text: "var" })
                    {
                        NullableTypeSyntax newDeclarationType = SyntaxFactory.NullableType(variableDeclaration.Type.WithoutTrivia()).WithTriviaFrom(variableDeclaration.Type);
                        changedDocument = await changedDocument.ReplaceNodeAsync(variableDeclaration.Type, newDeclarationType, ct).ConfigureAwait(false);
                    }
                }
                return changedDocument;
            },
            GetEquivalenceKey(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }
}
