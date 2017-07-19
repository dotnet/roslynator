// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConditionalExpressionClauseCodeFixProvider))]
    [Shared]
    public class ConditionalExpressionClauseCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.TypeOfConditionalExpressionCannotBeDetermined); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddCastExpression))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            ConditionalExpressionSyntax conditionalExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ConditionalExpressionSyntax>();

            Debug.Assert(conditionalExpression != null, $"{nameof(conditionalExpression)} is null");

            if (conditionalExpression == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.TypeOfConditionalExpressionCannotBeDetermined:
                        {
                            ExpressionSyntax whenTrue = conditionalExpression.WhenTrue;

                            ExpressionSyntax whenFalse = conditionalExpression.WhenFalse;

                            if (whenTrue?.IsMissing == false
                                && whenFalse?.IsMissing == false)
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ITypeSymbol falseType = semanticModel.GetTypeSymbol(whenFalse, context.CancellationToken);

                                if (falseType?.IsErrorType() == false)
                                {
                                    ITypeSymbol destinationType = FindDestinationType(whenTrue, falseType.BaseType, semanticModel);

                                    if (destinationType != null)
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            $"Cast to '{SymbolDisplay.GetMinimalString(destinationType, semanticModel, whenTrue.SpanStart)}'",
                                            cancellationToken => AddCastExpressionRefactoring.RefactorAsync(context.Document, whenTrue, destinationType, semanticModel, cancellationToken),
                                            GetEquivalenceKey(diagnostic));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                            }

                            break;
                        }
                }
            }
        }

        private static ITypeSymbol FindDestinationType(ExpressionSyntax expression, ITypeSymbol type, SemanticModel semanticModel)
        {
            while (type != null)
            {
                if (semanticModel.IsImplicitConversion(expression, type))
                    return type;

                type = type.BaseType;
            }

            return null;
        }
    }
}
