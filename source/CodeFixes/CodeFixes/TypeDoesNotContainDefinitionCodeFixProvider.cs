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

            SyntaxNode node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(SyntaxKind.AwaitExpression, SyntaxKind.IdentifierName, SyntaxKind.GenericName);

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.TypeDoesNotContainDefinitionAndNoExtensionMethodCouldBeFound:
                        {
                            switch (node.Kind())
                            {
                                case SyntaxKind.IdentifierName:
                                case SyntaxKind.GenericName:
                                    {
                                        if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.FixMemberAccessName))
                                            break;

                                        var simpleName = (SimpleNameSyntax)node;

                                        if (!simpleName.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                                            break;

                                        var memberAccess = (MemberAccessExpressionSyntax)simpleName.Parent;

                                        if (memberAccess.IsParentKind(SyntaxKind.InvocationExpression))
                                            break;

                                        switch (simpleName.Identifier.ValueText)
                                        {
                                            case "Count":
                                                {
                                                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                                    ComputeCodeFix(context, diagnostic, memberAccess, semanticModel, "Count", "Length");
                                                    break;
                                                }
                                            case "Length":
                                                {
                                                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                                    ComputeCodeFix(context, diagnostic, memberAccess, semanticModel, "Length", "Count");
                                                    break;
                                                }
                                        }

                                        break;
                                    }
                                case SyntaxKind.AwaitExpression:
                                    {
                                        if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveAwaitKeyword))
                                            break;

                                        var awaitExpression = (AwaitExpressionSyntax)node;

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

        private void ComputeCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            MemberAccessExpressionSyntax memberAccess,
            SemanticModel semanticModel,
            string name,
            string newName)
        {
            if (IsFixable(memberAccess, newName, semanticModel, context.CancellationToken))
            {
                CodeAction codeAction = CodeAction.Create(
                    $"Use '{newName}' instead of '{name}'",
                    cancellationToken => RefactorAsync(context.Document, memberAccess, newName, cancellationToken),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }

        private static bool IsFixable(
            MemberAccessExpressionSyntax memberAccess,
            string newName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(memberAccess.Expression, cancellationToken);

            if (typeSymbol != null)
            {
                if (typeSymbol.IsArrayType())
                    typeSymbol = ((IArrayTypeSymbol)typeSymbol).ElementType;

                foreach (ISymbol symbol in typeSymbol.GetMembers(newName))
                {
                    if (!symbol.IsStatic
                        && symbol.IsProperty())
                    {
                        var propertySymbol = (IPropertySymbol)symbol;

                        if (!propertySymbol.IsIndexer
                            && propertySymbol.IsReadOnly
                            && semanticModel.IsAccessible(memberAccess.SpanStart, symbol))
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
            MemberAccessExpressionSyntax memberAccess,
            string newName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MemberAccessExpressionSyntax newNode = memberAccess
                .WithName(IdentifierName(newName))
                .WithTriviaFrom(memberAccess.Name);

            return document.ReplaceNodeAsync(memberAccess, newNode, cancellationToken);
        }
    }
}
