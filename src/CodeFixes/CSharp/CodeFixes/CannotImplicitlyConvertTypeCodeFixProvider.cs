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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CannotImplicitlyConvertTypeCodeFixProvider))]
    [Shared]
    public sealed class CannotImplicitlyConvertTypeCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS0029_CannotImplicitlyConvertType); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxNode node = root.FindNode(context.Span, getInnermostNodeForTie: true);

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS0029_CannotImplicitlyConvertType:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReplaceStringLiteralWithCharacterLiteral, context.Document, root.SyntaxTree)
                                && node.IsKind(SyntaxKind.StringLiteralExpression))
                            {
                                var literalExpression = (LiteralExpressionSyntax)node;

                                if (literalExpression.Token.ValueText.Length == 1)
                                {
                                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                    if (semanticModel.GetTypeInfo(node, context.CancellationToken).ConvertedType?.SpecialType == SpecialType.System_Char)
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Replace string literal with character literal",
                                            ct => ReplaceStringLiteralWithCharacterLiteralRefactoring.RefactorAsync(context.Document, literalExpression, ct),
                                            GetEquivalenceKey(diagnostic));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                            }

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.UseYieldReturnInsteadOfReturn, context.Document, root.SyntaxTree)
                                && node.IsParentKind(SyntaxKind.ReturnStatement))
                            {
                                var returnStatement = (ReturnStatementSyntax)node.Parent;

                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ISymbol containingSymbol = semanticModel.GetEnclosingSymbol(returnStatement.SpanStart, context.CancellationToken);

                                if (containingSymbol?.Kind == SymbolKind.Method
                                    && ((IMethodSymbol)containingSymbol).ReturnType.OriginalDefinition.IsIEnumerableOrIEnumerableOfT())
                                {
                                    CodeAction codeAction = CodeAction.Create(
                                        "Use yield return instead of return",
                                        ct => UseYieldReturnInsteadOfReturnRefactoring.RefactorAsync(context.Document, returnStatement, SyntaxKind.YieldReturnStatement, semanticModel, ct),
                                        GetEquivalenceKey(diagnostic));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
                            }

                            break;
                        }
                }
            }
        }
    }
}
