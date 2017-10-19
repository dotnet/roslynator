// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TokenCodeFixProvider))]
    [Shared]
    public class TokenCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperandOfType,
                    CompilerDiagnosticIdentifiers.PartialModifierCanOnlyAppearImmediatelyBeforeClassStructInterfaceOrVoid,
                    CompilerDiagnosticIdentifiers.ObjectOfTypeConvertibleToTypeIsRequired,
                    CompilerDiagnosticIdentifiers.ValueCannotBeUsedAsDefaultParameter);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.AddArgumentList,
                CodeFixIdentifiers.ReorderModifiers,
                CodeFixIdentifiers.ReturnDefaultValue,
                CodeFixIdentifiers.ReplaceNullLiteralExpressionWithDefaultValue))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindToken(root, context.Span.Start, out SyntaxToken token))
                return;

            SyntaxKind kind = token.Kind();

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperandOfType:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddArgumentList))
                                break;

                            if (kind != SyntaxKind.QuestionToken)
                                break;

                            if (!token.IsParentKind(SyntaxKind.ConditionalAccessExpression))
                                break;

                            var conditionalAccess = (ConditionalAccessExpressionSyntax)token.Parent;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(conditionalAccess.Expression, context.CancellationToken);

                            if (typeSymbol == null
                                || typeSymbol.IsErrorType()
                                || !typeSymbol.IsValueType
                                || typeSymbol.IsConstructedFrom(SpecialType.System_Nullable_T))
                            {
                                break;
                            }

                            CodeAction codeAction = CodeAction.Create(
                                "Remove '?' operator",
                                cancellationToken =>
                                {
                                    var textChange = new TextChange(token.Span, "");
                                    return context.Document.WithTextChangeAsync(textChange, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.PartialModifierCanOnlyAppearImmediatelyBeforeClassStructInterfaceOrVoid:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReorderModifiers))
                                break;

                            ModifiersCodeFixRegistrator.MoveModifier(context, diagnostic, token.Parent, token);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ObjectOfTypeConvertibleToTypeIsRequired:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReturnDefaultValue))
                                break;

                            if (token.Kind() != SyntaxKind.ReturnKeyword)
                                break;

                            if (!token.IsParentKind(SyntaxKind.ReturnStatement))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ISymbol symbol = semanticModel.GetEnclosingSymbol(token.SpanStart, context.CancellationToken);

                            if (symbol == null)
                                break;

                            SymbolKind symbolKind = symbol.Kind;

                            ITypeSymbol typeSymbol = null;

                            if (symbolKind == SymbolKind.Method)
                            {
                                var methodSymbol = (IMethodSymbol)symbol;

                                typeSymbol = methodSymbol.ReturnType;

                                if (methodSymbol.IsAsync
                                    && (typeSymbol is INamedTypeSymbol namedTypeSymbol))
                                {
                                    ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

                                    if (typeArguments.Any())
                                        typeSymbol = typeArguments[0];
                                }
                            }
                            else if (symbolKind == SymbolKind.Property)
                            {
                                typeSymbol = ((IPropertySymbol)symbol).Type;
                            }
                            else
                            {
                                Debug.Fail(symbolKind.ToString());
                            }

                            if (typeSymbol == null)
                                break;

                            if (typeSymbol.Kind == SymbolKind.ErrorType)
                                break;

                            if (!typeSymbol.SupportsExplicitDeclaration())
                                break;

                            var returnStatement = (ReturnStatementSyntax)token.Parent;

                            CodeAction codeAction = CodeAction.Create(
                                "Return default value",
                                cancellationToken =>
                                {
                                    ExpressionSyntax expression = typeSymbol.ToDefaultValueSyntax(semanticModel, returnStatement.SpanStart);

                                    ReturnStatementSyntax newNode = returnStatement.WithExpression(expression);

                                    return context.Document.ReplaceNodeAsync(returnStatement, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ValueCannotBeUsedAsDefaultParameter:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReplaceNullLiteralExpressionWithDefaultValue))
                                break;

                            if (!(token.Parent is ParameterSyntax parameter))
                                break;

                            ExpressionSyntax value = parameter.Default?.Value;

                            if (value == null)
                                break;

                            if (value.Kind() != SyntaxKind.NullLiteralExpression)
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            CodeFixRegistrator.ReplaceNullWithDefaultValue(context, diagnostic, value, semanticModel);
                            break;
                        }
                }
            }
        }
    }
}
