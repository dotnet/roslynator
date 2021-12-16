// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using Roslynator.CSharp.Refactorings;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CannotConvertArgumentTypeCodeFixProvider))]
    [Shared]
    public sealed class CannotConvertArgumentTypeCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS1503_CannotConvertArgumentType); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ExpressionSyntax expression, getInnermostNodeForTie: false))
                return;

            if (expression.Parent is not ArgumentSyntax argument
                || argument.Expression != expression)
            {
                return;
            }

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case CompilerDiagnosticIdentifiers.CS1503_CannotConvertArgumentType:
                    {
                        if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReplaceNullLiteralExpressionWithDefaultValue, document, root.SyntaxTree)
                            && expression.Kind() == SyntaxKind.NullLiteralExpression
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
                                        CodeFixIdentifiers.ReplaceNullLiteralExpressionWithDefaultValue);
                                }
                            }
                        }

                        if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddArgumentList, document, root.SyntaxTree)
                            && expression.IsKind(
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
                                    ct =>
                                    {
                                        ArgumentSyntax newNode = argument.WithExpression(invocationExpression);

                                        return document.ReplaceNodeAsync(argument, newNode, ct);
                                    },
                                    GetEquivalenceKey(diagnostic, CodeFixIdentifiers.AddArgumentList));

                                context.RegisterCodeFix(codeAction, diagnostic);
                                break;
                            }
                        }

                        if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.CreateSingletonArray, document, root.SyntaxTree)
                            && expression?.IsMissing == false)
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression);

                            if (typeSymbol?.IsErrorType() == false)
                            {
                                foreach (ITypeSymbol typeSymbol2 in DetermineParameterTypeHelper.DetermineParameterTypes(argument, semanticModel, context.CancellationToken))
                                {
                                    if (!SymbolEqualityComparer.Default.Equals(typeSymbol, typeSymbol2)
                                        && typeSymbol2 is IArrayTypeSymbol arrayType
                                        && semanticModel.IsImplicitConversion(expression, arrayType.ElementType))
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Create singleton array",
                                            ct => CreateSingletonArrayRefactoring.RefactorAsync(document, expression, arrayType.ElementType, semanticModel, ct),
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.CreateSingletonArray));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                        break;
                                    }
                                }
                            }
                        }

                        break;
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
                return default;

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
                        return default;

                    parameters = parameters2;
                }
            }

            return parameters;
        }
    }
}
