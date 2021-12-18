// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ArgumentCodeFixProvider))]
    [Shared]
    public sealed class ArgumentCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS1620_ArgumentMustBePassedWithRefOrOutKeyword,
                    CompilerDiagnosticIdentifiers.CS1615_ArgumentShouldNotBePassedWithRefOrOutKeyword,
                    CompilerDiagnosticIdentifiers.CS0192_ReadOnlyFieldCannotBePassedAsRefOrOutValue);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ArgumentSyntax argument))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS1620_ArgumentMustBePassedWithRefOrOutKeyword:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddOutModifierToArgument, context.Document, root.SyntaxTree))
                                return;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            IParameterSymbol parameter = semanticModel.DetermineParameter(argument, allowCandidate: true, cancellationToken: context.CancellationToken);

                            if (parameter == null)
                                return;

                            SyntaxToken refOrOutKeyword = default;

                            if (parameter.RefKind == RefKind.Out)
                            {
                                refOrOutKeyword = Token(SyntaxKind.OutKeyword);
                            }
                            else if (parameter.RefKind == RefKind.Ref)
                            {
                                refOrOutKeyword = Token(SyntaxKind.RefKeyword);
                            }
                            else
                            {
                                return;
                            }

                            CodeAction codeAction = CodeAction.Create(
                                $"Add '{SyntaxFacts.GetText(refOrOutKeyword.Kind())}' modifier",
                                ct =>
                                {
                                    ArgumentSyntax newArgument = argument
                                        .WithRefOrOutKeyword(refOrOutKeyword)
                                        .WithFormatterAnnotation();

                                    return context.Document.ReplaceNodeAsync(argument, newArgument, ct);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS1615_ArgumentShouldNotBePassedWithRefOrOutKeyword:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveRefModifier, context.Document, root.SyntaxTree))
                                return;

                            CodeAction codeAction = CodeAction.Create(
                                "Remove 'ref' modifier",
                                ct =>
                                {
                                    ArgumentSyntax newArgument = argument
                                        .WithRefOrOutKeyword(default(SyntaxToken))
                                        .PrependToLeadingTrivia(argument.RefOrOutKeyword.GetAllTrivia())
                                        .WithFormatterAnnotation();

                                    return context.Document.ReplaceNodeAsync(argument, newArgument, ct);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0192_ReadOnlyFieldCannotBePassedAsRefOrOutValue:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.MakeFieldWritable, context.Document, root.SyntaxTree))
                                return;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(argument.Expression, context.CancellationToken);

                            if (symbolInfo.CandidateReason != CandidateReason.NotAVariable)
                                return;

                            if (symbolInfo.CandidateSymbols.SingleOrDefault(shouldThrow: false) is not IFieldSymbol fieldSymbol)
                                return;

                            if (fieldSymbol.DeclaredAccessibility != Accessibility.Private)
                                return;

                            if (fieldSymbol.GetSyntax().Parent.Parent is not FieldDeclarationSyntax fieldDeclaration)
                                return;

                            TypeDeclarationSyntax containingTypeDeclaration = fieldDeclaration.FirstAncestor<TypeDeclarationSyntax>();

                            if (!argument.Ancestors().Any(f => f == containingTypeDeclaration))
                                return;

                            ModifiersCodeFixRegistrator.RemoveModifier(
                                context,
                                diagnostic,
                                fieldDeclaration,
                                SyntaxKind.ReadOnlyKeyword,
                                title: $"Make '{fieldSymbol.Name}' writable",
                                additionalKey: CodeFixIdentifiers.MakeFieldWritable);

                            break;
                        }
                }
            }
        }
    }
}
