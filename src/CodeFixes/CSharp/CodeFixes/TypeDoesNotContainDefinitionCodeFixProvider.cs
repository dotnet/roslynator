// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeDoesNotContainDefinitionCodeFixProvider))]
    [Shared]
    public class TypeDoesNotContainDefinitionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.TypeDoesNotContainDefinitionAndNoExtensionMethodCouldBeFound); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.FixMemberAccessName,
                CodeFixIdentifiers.RemoveAwaitKeyword))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => f.IsKind(
                SyntaxKind.AwaitExpression,
                SyntaxKind.IdentifierName,
                SyntaxKind.GenericName,
                SyntaxKind.MemberBindingExpression)))
            {
                return;
            }

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.TypeDoesNotContainDefinitionAndNoExtensionMethodCouldBeFound:
                        {
                            switch (node)
                            {
                                case SimpleNameSyntax simpleName:
                                    {
                                        Debug.Assert(node.IsKind(SyntaxKind.IdentifierName, SyntaxKind.GenericName), node.Kind().ToString());

                                        if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.FixMemberAccessName))
                                            break;

                                        if (!simpleName.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                                            break;

                                        var memberAccess = (MemberAccessExpressionSyntax)simpleName.Parent;

                                        if (memberAccess.IsParentKind(SyntaxKind.InvocationExpression))
                                            break;

                                        await ComputeCodeFixAsync(context, diagnostic, memberAccess.Expression, simpleName).ConfigureAwait(false);

                                        break;
                                    }
                                case MemberBindingExpressionSyntax memberBindingExpression:
                                    {
                                        if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.FixMemberAccessName))
                                            break;

                                        if (!(memberBindingExpression.Parent is ConditionalAccessExpressionSyntax conditionalAccessExpression))
                                            break;

                                        await ComputeCodeFixAsync(context, diagnostic, conditionalAccessExpression.Expression, memberBindingExpression.Name).ConfigureAwait(false);

                                        break;
                                    }
                                case AwaitExpressionSyntax awaitExpression:
                                    {
                                        if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveAwaitKeyword))
                                            break;

                                        CodeAction codeAction = CodeAction.Create(
                                            "Remove 'await'",
                                            cancellationToken =>
                                            {
                                                ExpressionSyntax expression = awaitExpression.Expression;

                                                SyntaxTriviaList leadingTrivia = awaitExpression
                                                    .GetLeadingTrivia()
                                                    .AddRange(awaitExpression.AwaitKeyword.TrailingTrivia.EmptyIfWhitespace())
                                                    .AddRange(expression.GetLeadingTrivia().EmptyIfWhitespace());

                                                ExpressionSyntax newNode = expression.WithLeadingTrivia(leadingTrivia);

                                                return context.Document.ReplaceNodeAsync(awaitExpression, newNode, cancellationToken);
                                            },
                                            GetEquivalenceKey(diagnostic));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                        break;
                                    }
                            }

                            break;
                        }
                }
            }
        }

        private async Task ComputeCodeFixAsync(CodeFixContext context, Diagnostic diagnostic, ExpressionSyntax expression, SimpleNameSyntax simpleName)
        {
            switch (simpleName.Identifier.ValueText)
            {
                case "Count":
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ComputeCodeFix(context, diagnostic, expression, simpleName, semanticModel, "Count", "Length");
                        break;
                    }
                case "Length":
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ComputeCodeFix(context, diagnostic, expression, simpleName, semanticModel, "Length", "Count");
                        break;
                    }
            }
        }

        private void ComputeCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            SimpleNameSyntax simpleName,
            SemanticModel semanticModel,
            string name,
            string newName)
        {
            if (IsFixable(expression, newName, semanticModel, context.CancellationToken))
            {
                CodeAction codeAction = CodeAction.Create(
                    $"Use '{newName}' instead of '{name}'",
                    cancellationToken => RefactorAsync(context.Document, simpleName, newName, cancellationToken),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }

        private static bool IsFixable(
            ExpressionSyntax expression,
            string newName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            if (typeSymbol != null)
            {
                if (typeSymbol is IArrayTypeSymbol arrayType)
                    typeSymbol = arrayType.ElementType;

                foreach (ISymbol symbol in typeSymbol.GetMembers(newName))
                {
                    if (!symbol.IsStatic
                        && symbol.Kind == SymbolKind.Property)
                    {
                        var propertySymbol = (IPropertySymbol)symbol;

                        if (!propertySymbol.IsIndexer
                            && propertySymbol.IsReadOnly
                            && semanticModel.IsAccessible(expression.SpanStart, symbol))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            SimpleNameSyntax simpleName,
            string newName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SimpleNameSyntax newNode = simpleName.WithIdentifier(Identifier(newName).WithTriviaFrom(simpleName.Identifier));

            return document.ReplaceNodeAsync(simpleName, newNode, cancellationToken);
        }
    }
}
