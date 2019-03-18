// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SyntaxErrorCharExpectedCodeFixProvider))]
    [Shared]
    public class SyntaxErrorCharExpectedCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.SyntaxErrorCharExpected); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            if (!Settings.IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddMissingComma))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            ExpressionSyntax expression = root.FindNode(context.Span).FirstAncestorOrSelf<ExpressionSyntax>();

            if (expression == null)
                return;

            if (!expression.IsParentKind(SyntaxKind.ArrayInitializerExpression))
                return;

            var initializer = (InitializerExpressionSyntax)expression.Parent;

            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            int index = expressions.IndexOf(expression);

            Debug.Assert(index > 0);

            if (index <= 0)
                return;

            int newCommaIndex = expression.Span.End;

            if (expressions.GetSeparator(index - 1).IsMissing)
            {
                newCommaIndex = expressions[index - 1].Span.End;
            }
            else
            {
                Debug.Assert(index < expressions.Count - 1);

                if (index == expressions.Count - 1)
                    return;

                Debug.Assert(expressions.GetSeparator(index).IsMissing);

                if (!expressions.GetSeparator(index).IsMissing)
                    return;

                newCommaIndex = expression.Span.End;
            }

            CodeAction codeAction = CodeAction.Create(
                "Add missing comma",
                cancellationToken =>
                {
                    var textChange = new TextChange(new TextSpan(newCommaIndex, 0), ",");
                    return context.Document.WithTextChangeAsync(textChange, cancellationToken);
                },
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
