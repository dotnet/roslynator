// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParameterCodeFixProvider))]
    [Shared]
    public class ParameterCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.ParamsParameterMustBeSingleDimensionalArray,
                    CompilerDiagnosticIdentifiers.CannotSpecifyDefaultValueForParameterArray);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.ChangeTypeOfParamsParameter,
                CodeFixIdentifiers.RemoveDefaultValueFromParameter))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ParameterSyntax parameter))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.ParamsParameterMustBeSingleDimensionalArray:
                        {
                            TypeSyntax type = parameter.Type;

                            if (type?.IsMissing == false)
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                                if (typeSymbol?.Kind == SymbolKind.NamedType)
                                {
                                    ArrayTypeSyntax newType = SyntaxFactory.ArrayType(
                                        typeSymbol.ToMinimalTypeSyntax(semanticModel, parameter.SpanStart),
                                        SyntaxFactory.SingletonList(SyntaxFactory.ArrayRankSpecifier()));

                                    CodeAction codeAction = CodeAction.Create(
                                        $"Change parameter type to '{newType}'",
                                        cancellationToken => context.Document.ReplaceNodeAsync(type, newType.WithTriviaFrom(type), cancellationToken),
                                        GetEquivalenceKey(diagnostic));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CannotSpecifyDefaultValueForParameterArray:
                        {
                            EqualsValueClauseSyntax defaultValue = parameter.Default;

                            CodeAction codeAction = CodeAction.Create(
                                "Remove default value from parameter",
                                cancellationToken =>
                                {
                                    ParameterSyntax newParameter = parameter
                                        .RemoveNode(defaultValue)
                                        .WithFormatterAnnotation();

                                    return context.Document.ReplaceNodeAsync(parameter, newParameter, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
