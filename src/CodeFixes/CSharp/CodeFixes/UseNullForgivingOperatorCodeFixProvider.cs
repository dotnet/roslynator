// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseNullForgivingOperatorCodeFixProvider))]
    [Shared]
    public sealed class UseNullForgivingOperatorCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS8625_CannotConvertNullLiteralToNonNullableReferenceType); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindNode(
                root,
                context.Span,
                out ExpressionSyntax expression))
            {
                return;
            }

            SyntaxDebug.Assert(expression.IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultLiteralExpression, SyntaxKind.DefaultExpression), expression);

            if (!expression.IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultLiteralExpression, SyntaxKind.DefaultExpression))
                return;

            Diagnostic diagnostic = context.Diagnostics[0];
            Document document = context.Document;

            switch (diagnostic.Id)
            {
                case CompilerDiagnosticIdentifiers.CS8625_CannotConvertNullLiteralToNonNullableReferenceType:
                    {
                        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.UseNullForgivingOperator, context.Document, root.SyntaxTree))
                            break;

                        CodeAction codeAction = CodeAction.Create(
                            "Use null-forgiving operator",
                            ct =>
                            {
                                PostfixUnaryExpressionSyntax newExpression = SuppressNullableWarningExpression(expression.WithoutTrivia())
                                    .WithTriviaFrom(expression);

                                return document.ReplaceNodeAsync(expression, newExpression, ct);
                            },
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);

                        break;
                    }
            }
        }
    }
}
