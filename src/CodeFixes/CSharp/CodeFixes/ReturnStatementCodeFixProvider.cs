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
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ReturnStatementCodeFixProvider))]
    [Shared]
    public sealed class ReturnStatementCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS1622_CannotReturnValueFromIterator,
                    CompilerDiagnosticIdentifiers.CS0127_SinceMethodReturnsVoidReturnKeywordMustNotBeFollowedByObjectExpression,
                    CompilerDiagnosticIdentifiers.CS1997_SinceMethodIsAsyncMethodThatReturnsTaskReturnKeywordMustNotBeFollowedByObjectExpression);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ReturnStatementSyntax returnStatement))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS1622_CannotReturnValueFromIterator:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.UseYieldReturnInsteadOfReturn, context.Document, root.SyntaxTree))
                                break;

                            ExpressionSyntax expression = returnStatement.Expression;

                            if (expression != null)
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ISymbol containingSymbol = semanticModel.GetEnclosingSymbol(returnStatement.SpanStart, context.CancellationToken);

                                if (containingSymbol?.Kind == SymbolKind.Method)
                                {
                                    var methodSymbol = (IMethodSymbol)containingSymbol;

                                    ITypeSymbol returnType = methodSymbol.ReturnType;

                                    var replacementKind = SyntaxKind.None;

                                    if (returnType.SpecialType == SpecialType.System_Collections_IEnumerable)
                                    {
                                        if (semanticModel
                                            .GetTypeSymbol(expression, context.CancellationToken)
                                            .OriginalDefinition
                                            .IsIEnumerableOrIEnumerableOfT())
                                        {
                                            replacementKind = SyntaxKind.ForEachStatement;
                                        }
                                        else
                                        {
                                            replacementKind = SyntaxKind.YieldReturnStatement;
                                        }
                                    }
                                    else if (returnType.Kind == SymbolKind.NamedType)
                                    {
                                        var namedTypeSymbol = (INamedTypeSymbol)returnType;

                                        if (namedTypeSymbol.OriginalDefinition.IsIEnumerableOfT())
                                        {
                                            if (semanticModel.IsImplicitConversion(expression, namedTypeSymbol.TypeArguments[0]))
                                            {
                                                replacementKind = SyntaxKind.YieldReturnStatement;
                                            }
                                            else
                                            {
                                                replacementKind = SyntaxKind.ForEachStatement;
                                            }
                                        }
                                    }

                                    if (replacementKind == SyntaxKind.YieldReturnStatement
                                        || (replacementKind == SyntaxKind.ForEachStatement && !returnStatement.SpanContainsDirectives()))
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Use yield return instead of return",
                                            ct => UseYieldReturnInsteadOfReturnRefactoring.RefactorAsync(context.Document, returnStatement, replacementKind, semanticModel, ct),
                                            GetEquivalenceKey(diagnostic));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0127_SinceMethodReturnsVoidReturnKeywordMustNotBeFollowedByObjectExpression:
                    case CompilerDiagnosticIdentifiers.CS1997_SinceMethodIsAsyncMethodThatReturnsTaskReturnKeywordMustNotBeFollowedByObjectExpression:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeMemberTypeAccordingToReturnExpression, context.Document, root.SyntaxTree))
                            {
                                ChangeMemberTypeRefactoring.ComputeCodeFix(context, diagnostic, returnStatement.Expression, semanticModel);
                            }

                            break;
                        }
                }
            }
        }
    }
}
