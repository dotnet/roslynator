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

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeCodeFixProvider))]
    [Shared]
    public class TypeCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.StaticTypesCannotBeUsedAsTypeArguments); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.MakeClassNonStatic))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            TypeSyntax type = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<TypeSyntax>();

            Debug.Assert(type != null, $"{nameof(type)} is null");

            if (type == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.StaticTypesCannotBeUsedAsTypeArguments:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ISymbol symbol = semanticModel.GetSymbol(type, context.CancellationToken)?.OriginalDefinition;

                            if (symbol?.IsNamedType() == true
                                && ((INamedTypeSymbol)symbol).IsClass())
                            {
                                ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

                                if (syntaxReferences.Length == 1)
                                {
                                    CodeAction codeAction = CodeAction.Create(
                                        GetTitle(symbol, semanticModel, type.SpanStart),
                                        cancellationToken => context.Document.RemoveModifierAsync(syntaxReferences[0].GetSyntax(cancellationToken), SyntaxKind.StaticKeyword, cancellationToken),
                                        GetEquivalenceKey(diagnostic));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
                                else if (syntaxReferences.Length > 1)
                                {
                                    CodeAction codeAction = CodeAction.Create(
                                        GetTitle(symbol, semanticModel, type.SpanStart),
                                        cancellationToken =>
                                        {
                                            return context.Document.Solution().ReplaceNodesAsync(
                                                syntaxReferences.Select(f => (ClassDeclarationSyntax)f.GetSyntax(cancellationToken)),
                                                (f, g) => f.RemoveModifier(SyntaxKind.StaticKeyword),
                                                cancellationToken);
                                        },
                                        GetEquivalenceKey(diagnostic));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
                            }

                            break;
                        }
                }
            }
        }

        private static string GetTitle(ISymbol symbol, SemanticModel semanticModel, int position)
        {
            return $"Make '{SymbolDisplay.GetMinimalString(symbol, semanticModel, position)}' non-static";
        }
    }
}
