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
using Roslynator.CodeFixes;
using Roslynator.CSharp.Helpers;
using Roslynator.CSharp.Refactorings;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ArgumentCodeFixProvider))]
    [Shared]
    public class ArgumentCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.ArgumentMustBePassedWithRefOrOutKeyword,
                    CompilerDiagnosticIdentifiers.ArgumentShouldNotBePassedWithRefOrOutKeyword,
                    CompilerDiagnosticIdentifiers.CannotConvertArgumentType);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.AddOutModifierToArgument,
                CodeFixIdentifiers.RemoveRefModifier,
                CodeFixIdentifiers.CreateSingletonArray,
                CodeFixIdentifiers.AddArgumentList,
                CodeFixIdentifiers.ReplaceNullLiteralExpressionWithDefaultValue))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ArgumentSyntax argument))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.ArgumentMustBePassedWithRefOrOutKeyword:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddOutModifierToArgument))
                                return;

                            CodeAction codeAction = CodeAction.Create(
                           "Add 'out' modifier",
                           cancellationToken =>
                           {
                               ArgumentSyntax newArgument = argument
                                   .WithRefOrOutKeyword(CSharpFactory.OutKeyword())
                                   .WithFormatterAnnotation();

                               return context.Document.ReplaceNodeAsync(argument, newArgument, cancellationToken);
                           },
                           GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ArgumentShouldNotBePassedWithRefOrOutKeyword:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveRefModifier))
                                return;

                            CodeAction codeAction = CodeAction.Create(
                                "Remove 'ref' modifier",
                                cancellationToken =>
                                {
                                    ArgumentSyntax newArgument = argument
                                        .WithRefOrOutKeyword(default(SyntaxToken))
                                        .PrependToLeadingTrivia(argument.RefOrOutKeyword.GetAllTrivia())
                                        .WithFormatterAnnotation();

                                    return context.Document.ReplaceNodeAsync(argument, newArgument, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CannotConvertArgumentType:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReplaceNullLiteralExpressionWithDefaultValue))
                            {
                                ExpressionSyntax expression = argument.Expression;

                                if (expression.Kind() == SyntaxKind.NullLiteralExpression
                                    && argument.Parent is ArgumentListSyntax argumentList)
                                {
                                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                    ImmutableArray<IParameterSymbol> parameterSymbols = FindParameters(argumentList, semanticModel, context.CancellationToken);

                                    if (!parameterSymbols.IsDefault)
                                    {
                                        int index = argumentList.Arguments.IndexOf(argument);

                                        IParameterSymbol parameterSymbol = parameterSymbols[index];

                                        ITypeSymbol typeSymbol = parameterSymbol.Type;

                                        if (typeSymbol.IsValueType)
                                        {
                                            CodeFixRegistrator.ReplaceNullWithDefaultValue(
                                                context,
                                                diagnostic,
                                                expression,
                                                typeSymbol,
                                                semanticModel,
                                                CodeFixIdentifiers.ReplaceNullLiteralExpressionWithDefaultValue);
                                        }
                                    }
                                }
                            }

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddArgumentList))
                            {
                                ExpressionSyntax expression = argument.Expression;

                                if (expression.IsKind(
                                    SyntaxKind.IdentifierName,
                                    SyntaxKind.GenericName,
                                    SyntaxKind.SimpleMemberAccessExpression))
                                {
                                    InvocationExpressionSyntax invocationExpression = InvocationExpression(
                                        expression.WithoutTrailingTrivia(),
                                        ArgumentList().WithTrailingTrivia(expression.GetTrailingTrivia()));

                                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                    if (semanticModel.GetSpeculativeMethodSymbol(expression.SpanStart, invocationExpression) != null)
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Add argument list",
                                            cancellationToken =>
                                            {
                                                ArgumentSyntax newNode = argument.WithExpression(invocationExpression);

                                                return context.Document.ReplaceNodeAsync(argument, newNode, cancellationToken);
                                            },
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.AddArgumentList));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                        break;
                                    }
                                }
                            }

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.CreateSingletonArray))
                            {
                                ExpressionSyntax expression = argument.Expression;

                                if (expression?.IsMissing == false)
                                {
                                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                    ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression);

                                    if (typeSymbol?.IsErrorType() == false)
                                    {
                                        foreach (ITypeSymbol typeSymbol2 in DetermineParameterTypeHelper.DetermineParameterTypes(argument, semanticModel, context.CancellationToken))
                                        {
                                            if (!typeSymbol.Equals(typeSymbol2)
                                                && typeSymbol2 is IArrayTypeSymbol arrayType
                                                && semanticModel.IsImplicitConversion(expression, arrayType.ElementType))
                                            {
                                                CodeAction codeAction = CodeAction.Create(
                                                    "Create singleton array",
                                                    cancellationToken => CreateSingletonArrayRefactoring.RefactorAsync(context.Document, expression, arrayType.ElementType, semanticModel, cancellationToken),
                                                    GetEquivalenceKey(diagnostic, CodeFixIdentifiers.CreateSingletonArray));

                                                context.RegisterCodeFix(codeAction, diagnostic);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        }
                }
            }
        }

        private static ImmutableArray<IParameterSymbol> FindParameters(
            ArgumentListSyntax argumentList,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(argumentList.Parent, cancellationToken);

            ImmutableArray<ISymbol> candidateSymbols = symbolInfo.CandidateSymbols;

            if (candidateSymbols.IsEmpty)
                return default(ImmutableArray<IParameterSymbol>);

            int argumentCount = argumentList.Arguments.Count;

            var parameters = default(ImmutableArray<IParameterSymbol>);

            foreach (ISymbol symbol in candidateSymbols)
            {
                ImmutableArray<IParameterSymbol> parameters2 = symbol.ParametersOrDefault();

                Debug.Assert(!parameters2.IsDefault, symbol.Kind.ToString());

                if (!parameters2.IsDefault
                    && parameters2.Length == argumentCount)
                {
                    if (!parameters.IsDefault)
                        return default(ImmutableArray<IParameterSymbol>);

                    parameters = parameters2;
                }
            }

            return parameters;
        }
    }
}
