// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseStringComparisonCodeFixProvider))]
    [Shared]
    public class UseStringComparisonCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseStringComparison); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => f.IsKind(
                SyntaxKind.EqualsExpression,
                SyntaxKind.NotEqualsExpression,
                SyntaxKind.InvocationExpression)))
            {
                return;
            }

            Diagnostic diagnostic = context.Diagnostics[0];

            switch (node.Kind())
            {
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)node;

                        SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(binaryExpression.Left.WalkDownParentheses());

                        if (!invocationInfo.Success)
                            invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo((InvocationExpressionSyntax)binaryExpression.Right.WalkDownParentheses());

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        INamedTypeSymbol comparisonSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_StringComparison);

                        if (!invocationInfo.NameText.EndsWith("Invariant", StringComparison.Ordinal)
                            || !RegisterCodeFix(context, diagnostic, binaryExpression, comparisonSymbol, "InvariantCultureIgnoreCase"))
                        {
                            RegisterCodeFix(context, diagnostic, binaryExpression, comparisonSymbol, "OrdinalIgnoreCase");
                            RegisterCodeFix(context, diagnostic, binaryExpression, comparisonSymbol, "CurrentCultureIgnoreCase");
                        }

                        break;
                    }
                case SyntaxKind.InvocationExpression:
                    {
                        var invocationExpression = (InvocationExpressionSyntax)node;

                        SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

                        SeparatedSyntaxList<ArgumentSyntax> arguments = invocationInfo.Arguments;

                        InvocationExpressionSyntax invocationExpression2;

                        if (arguments.Count == 1)
                        {
                            invocationExpression2 = (InvocationExpressionSyntax)invocationInfo.Expression;
                        }
                        else
                        {
                            invocationExpression2 = (arguments[0].Expression.WalkDownParentheses() as InvocationExpressionSyntax)
                                ?? (InvocationExpressionSyntax)arguments[1].Expression.WalkDownParentheses();
                        }

                        SimpleMemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression2);

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        INamedTypeSymbol comparisonSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_StringComparison);

                        if (!invocationInfo2.NameText.EndsWith("Invariant", StringComparison.Ordinal)
                            || !RegisterCodeFix(context, diagnostic, invocationInfo, comparisonSymbol, "InvariantCultureIgnoreCase"))
                        {
                            RegisterCodeFix(context, diagnostic, invocationInfo, comparisonSymbol, "OrdinalIgnoreCase");
                            RegisterCodeFix(context, diagnostic, invocationInfo, comparisonSymbol, "CurrentCultureIgnoreCase");
                        }

                        break;
                    }
            }
        }

        private bool RegisterCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            BinaryExpressionSyntax binaryExpression,
            INamedTypeSymbol comparisonSymbol,
            string comparisonName)
        {
            if (!comparisonSymbol.ContainsMember<IFieldSymbol>(comparisonName))
                return false;

            CodeAction codeAction = CodeAction.Create(
                GetTitle(comparisonName),
                cancellationToken => UseStringComparisonRefactoring.RefactorAsync(context.Document, binaryExpression, comparisonName, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.UseStringComparison, comparisonName));

            context.RegisterCodeFix(codeAction, diagnostic);
            return true;
        }

        private bool RegisterCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            SimpleMemberInvocationExpressionInfo invocationInfo,
            INamedTypeSymbol comparisonSymbol,
            string comparisonName)
        {
            if (!comparisonSymbol.ContainsMember<IFieldSymbol>(comparisonName))
                return false;

            CodeAction codeAction = CodeAction.Create(
                GetTitle(comparisonName),
                cancellationToken => UseStringComparisonRefactoring.RefactorAsync(context.Document, invocationInfo, comparisonName, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.UseStringComparison, comparisonName));

            context.RegisterCodeFix(codeAction, diagnostic);
            return true;
        }

        private static string GetTitle(string stringComparison)
        {
            return $"Use 'StringComparison.{stringComparison}'";
        }
    }
}