// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Helpers;

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
                    CompilerDiagnosticIdentifiers.ArgumentMustBePassedWithOutKeyword,
                    CompilerDiagnosticIdentifiers.ArgumentMayNotBePassedWithRefKeyword,
                    CompilerDiagnosticIdentifiers.CannotConvertArgumentType);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.AddOutModifierToArgument,
                CodeFixIdentifiers.RemoveRefModifier,
                CodeFixIdentifiers.CreateSingletonArray))
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
                    case CompilerDiagnosticIdentifiers.ArgumentMustBePassedWithOutKeyword:
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
                    case CompilerDiagnosticIdentifiers.ArgumentMayNotBePassedWithRefKeyword:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveRefModifier))
                                return;

                            CodeAction codeAction = CodeAction.Create(
                                "Remove 'ref' modifier",
                                cancellationToken =>
                                {
                                    ArgumentSyntax newArgument = argument
                                        .WithRefOrOutKeyword(default(SyntaxToken))
                                        .PrependToLeadingTrivia(argument.RefOrOutKeyword.GetLeadingAndTrailingTrivia())
                                        .WithFormatterAnnotation();

                                    return context.Document.ReplaceNodeAsync(argument, newArgument, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CannotConvertArgumentType:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddOutModifierToArgument))
                                return;

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
                                            && typeSymbol2.IsArrayType())
                                        {
                                            var arrayType = (IArrayTypeSymbol)typeSymbol2;

                                            if (semanticModel.IsImplicitConversion(expression, arrayType.ElementType))
                                            {
                                                CodeAction codeAction = CodeAction.Create(
                                                    "Create singleton array",
                                                    cancellationToken => CreateSingletonArrayRefactoring.RefactorAsync(context.Document, expression, arrayType.ElementType, semanticModel, cancellationToken),
                                                    GetEquivalenceKey(diagnostic));

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
    }
}
