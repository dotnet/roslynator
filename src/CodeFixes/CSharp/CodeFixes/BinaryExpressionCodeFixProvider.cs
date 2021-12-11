// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BinaryExpressionCodeFixProvider))]
    [Shared]
    public sealed class BinaryExpressionCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS0077_AsOperatorMustBeUsedWithReferenceTypeOrNullableType); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReplaceAsExpressionWithCastExpression, context.Document, root.SyntaxTree))
                return;

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out BinaryExpressionSyntax binaryExpression))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            if (!ReplaceAsWithCastAnalysis.IsFixable(binaryExpression, semanticModel, context.CancellationToken))
                return;

            CodeAction codeAction = CodeAction.Create(
                ReplaceAsExpressionWithExplicitCastRefactoring.Title,
                ct => ReplaceAsExpressionWithExplicitCastRefactoring.RefactorAsync(context.Document, binaryExpression, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
