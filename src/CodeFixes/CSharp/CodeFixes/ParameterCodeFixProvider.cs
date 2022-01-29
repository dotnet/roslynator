// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParameterCodeFixProvider))]
    [Shared]
    public sealed class ParameterCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0225_ParamsParameterMustBeSingleDimensionalArray,
                    CompilerDiagnosticIdentifiers.CS1751_CannotSpecifyDefaultValueForParameterArray,
                    CompilerDiagnosticIdentifiers.CS1741_RefOrOutParameterCannotHaveDefaultValue,
                    CompilerDiagnosticIdentifiers.CS1743_CannotSpecifyDefaultValueForThisParameter);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ParameterSyntax parameter))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS0225_ParamsParameterMustBeSingleDimensionalArray:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeTypeOfParamsParameter, context.Document, root.SyntaxTree))
                                break;

                            TypeSyntax type = parameter.Type;

                            if (type?.IsMissing == false)
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                                if (typeSymbol?.Kind == SymbolKind.NamedType
                                    && typeSymbol.SupportsExplicitDeclaration())
                                {
                                    ArrayTypeSyntax newType = ArrayType(
                                        typeSymbol.ToMinimalTypeSyntax(semanticModel, parameter.SpanStart),
                                        SingletonList(ArrayRankSpecifier()));

                                    CodeAction codeAction = CodeAction.Create(
                                        $"Change parameter type to '{newType}'",
                                        ct => context.Document.ReplaceNodeAsync(type, newType.WithTriviaFrom(type), ct),
                                        GetEquivalenceKey(diagnostic));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS1751_CannotSpecifyDefaultValueForParameterArray:
                    case CompilerDiagnosticIdentifiers.CS1741_RefOrOutParameterCannotHaveDefaultValue:
                    case CompilerDiagnosticIdentifiers.CS1743_CannotSpecifyDefaultValueForThisParameter:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveDefaultValueFromParameter, context.Document, root.SyntaxTree))
                                break;

                            EqualsValueClauseSyntax defaultValue = parameter.Default;

                            CodeAction codeAction = CodeAction.Create(
                                "Remove default value from parameter",
                                ct =>
                                {
                                    ParameterSyntax newParameter = parameter
                                        .RemoveNode(defaultValue)
                                        .WithFormatterAnnotation();

                                    return context.Document.ReplaceNodeAsync(parameter, newParameter, ct);
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
