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
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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
                    CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperand,
                    CompilerDiagnosticIdentifiers.PartialModifierCanOnlyAppearImmediatelyBeforeClassStructInterfaceOrVoid,
                    CompilerDiagnosticIdentifiers.ValueCannotBeUsedAsDefaultParameter,
                    CompilerDiagnosticIdentifiers.ObjectOfTypeConvertibleToTypeIsRequired,
                    CompilerDiagnosticIdentifiers.TypeExpected,
                    CompilerDiagnosticIdentifiers.SemicolonAfterMethodOrAccessorBlockIsNotValid,
                    CompilerDiagnosticIdentifiers.CannotConvertType);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddArgumentList)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReorderModifiers)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReplaceNullLiteralExpressionWithDefaultValue)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReturnDefaultValue)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddMissingType)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveSemicolon)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConditionalAccess)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeForEachType))
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
                    case CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperand:
                        {
                            if (kind == SyntaxKind.QuestionToken
                                && token.Parent is ConditionalAccessExpressionSyntax conditionalAccess)
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(conditionalAccess.Expression, context.CancellationToken);

                                if (typeSymbol?.IsErrorType() == false
                                    && !typeSymbol.IsNullableType())
                                {
                                    if (typeSymbol.IsValueType)
                                    {
                                        if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConditionalAccess))
                                        {
                                            CodeAction codeAction = CodeAction.Create(
                                                "Remove '?' operator",
                                                cancellationToken =>
                                                {
                                                    var textChange = new TextChange(token.Span, "");
                                                    return context.Document.WithTextChangeAsync(textChange, cancellationToken);
                                                },
                                                GetEquivalenceKey(diagnostic));

                                            context.RegisterCodeFix(codeAction, diagnostic);
                                        }
                                    }
                                    else if (typeSymbol.IsReferenceType)
                                    {
                                        if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddArgumentList)
                                            && conditionalAccess.WhenNotNull is MemberBindingExpressionSyntax memberBindingExpression)
                                        {
                                            ConditionalAccessExpressionSyntax newNode = conditionalAccess.WithWhenNotNull(
                                                InvocationExpression(
                                                    memberBindingExpression.WithoutTrailingTrivia(),
                                                    ArgumentList().WithTrailingTrivia(memberBindingExpression.GetTrailingTrivia())));

                                            CodeAction codeAction = CodeAction.Create(
                                                "Add argument list",
                                                cancellationToken => context.Document.ReplaceNodeAsync(conditionalAccess, newNode, cancellationToken),
                                                GetEquivalenceKey(diagnostic));

                                            context.RegisterCodeFix(codeAction, diagnostic);
                                        }
                                    }
                                }

                                if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddArgumentList))
                                {
                                    break;
                                }

                                if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConditionalAccess))
                                {
                                }
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.PartialModifierCanOnlyAppearImmediatelyBeforeClassStructInterfaceOrVoid:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReorderModifiers))
                                break;

                            ModifiersCodeFixRegistrator.MoveModifier(context, diagnostic, token.Parent, token);
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
                                    ExpressionSyntax expression = typeSymbol.GetDefaultValueSyntax(semanticModel, returnStatement.SpanStart);

                                    ReturnStatementSyntax newNode = returnStatement.WithExpression(expression);

                                    return context.Document.ReplaceNodeAsync(returnStatement, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.TypeExpected:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddMissingType))
                                break;

                            if (token.Kind() != SyntaxKind.CloseParenToken)
                                break;

                            if (!(token.Parent is DefaultExpressionSyntax defaultExpression))
                                break;

                            if (!(defaultExpression.Type is IdentifierNameSyntax identifierName))
                                break;

                            if (!identifierName.IsMissing)
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            TypeInfo typeInfo = semanticModel.GetTypeInfo(defaultExpression, context.CancellationToken);

                            ITypeSymbol convertedType = typeInfo.ConvertedType;

                            if (convertedType?.SupportsExplicitDeclaration() != true)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                $"Add type '{SymbolDisplay.ToMinimalDisplayString(convertedType, semanticModel, defaultExpression.SpanStart, SymbolDisplayFormats.Default)}'",
                                cancellationToken =>
                                {
                                    TypeSyntax newNode = convertedType.ToMinimalTypeSyntax(semanticModel, defaultExpression.SpanStart);

                                    newNode = newNode
                                        .WithTriviaFrom(identifierName)
                                        .WithFormatterAnnotation();

                                    return context.Document.ReplaceNodeAsync(identifierName, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.SemicolonAfterMethodOrAccessorBlockIsNotValid:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveSemicolon))
                                break;

                            if (token.Kind() != SyntaxKind.SemicolonToken)
                                break;

                            switch (token.Parent)
                            {
                                case MethodDeclarationSyntax methodDeclaration:
                                    {
                                        BlockSyntax body = methodDeclaration.Body;

                                        if (body == null)
                                            break;

                                        CodeAction codeAction = CodeAction.Create(
                                            "Remove semicolon",
                                            cancellationToken =>
                                            {
                                                SyntaxTriviaList trivia = body
                                                    .GetTrailingTrivia()
                                                    .EmptyIfWhitespace()
                                                    .AddRange(token.LeadingTrivia.EmptyIfWhitespace())
                                                    .AddRange(token.TrailingTrivia);

                                                MethodDeclarationSyntax newNode = methodDeclaration
                                                    .WithBody(body.WithTrailingTrivia(trivia))
                                                    .WithSemicolonToken(default(SyntaxToken));

                                                return context.Document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken);
                                            },
                                            GetEquivalenceKey(diagnostic));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                        break;
                                    }
                                case PropertyDeclarationSyntax propertyDeclaration:
                                    {
                                        AccessorListSyntax accessorList = propertyDeclaration.AccessorList;

                                        if (accessorList == null)
                                            break;

                                        CodeAction codeAction = CodeAction.Create(
                                            "Remove semicolon",
                                            cancellationToken =>
                                            {
                                                SyntaxTriviaList trivia = accessorList
                                                    .GetTrailingTrivia()
                                                    .EmptyIfWhitespace()
                                                    .AddRange(token.LeadingTrivia.EmptyIfWhitespace())
                                                    .AddRange(token.TrailingTrivia);

                                                PropertyDeclarationSyntax newNode = propertyDeclaration
                                                    .WithAccessorList(accessorList.WithTrailingTrivia(trivia))
                                                    .WithSemicolonToken(default(SyntaxToken));

                                                return context.Document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
                                            },
                                            GetEquivalenceKey(diagnostic));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                        break;
                                    }
                                case AccessorDeclarationSyntax accessorDeclaration:
                                    {
                                        BlockSyntax body = accessorDeclaration.Body;

                                        if (body == null)
                                            break;

                                        CodeAction codeAction = CodeAction.Create(
                                            "Remove semicolon",
                                            cancellationToken =>
                                            {
                                                SyntaxTriviaList trivia = body
                                                    .GetTrailingTrivia()
                                                    .EmptyIfWhitespace()
                                                    .AddRange(token.LeadingTrivia.EmptyIfWhitespace())
                                                    .AddRange(token.TrailingTrivia);

                                                AccessorDeclarationSyntax newNode = accessorDeclaration
                                                    .WithBody(body.WithTrailingTrivia(trivia))
                                                    .WithSemicolonToken(default(SyntaxToken));

                                                return context.Document.ReplaceNodeAsync(accessorDeclaration, newNode, cancellationToken);
                                            },
                                            GetEquivalenceKey(diagnostic));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                        break;
                                    }
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CannotConvertType:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeForEachType))
                                break;

                            if (token.Kind() != SyntaxKind.ForEachKeyword)
                                break;

                            if (!(token.Parent is ForEachStatementSyntax forEachStatement))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

                            ITypeSymbol typeSymbol = info.ElementType;

                            if (typeSymbol.SupportsExplicitDeclaration())
                                CodeFixRegistrator.ChangeType(context, diagnostic, forEachStatement.Type, typeSymbol, semanticModel, CodeFixIdentifiers.ChangeForEachType);

                            CodeFixRegistrator.ChangeTypeToVar(context, diagnostic, forEachStatement.Type, CodeFixIdentifiers.ChangeTypeToVar);
                            break;
                        }
                }
            }
        }
    }
}
