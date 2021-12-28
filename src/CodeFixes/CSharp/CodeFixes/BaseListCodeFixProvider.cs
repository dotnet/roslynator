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

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BaseListCodeFixProvider))]
    [Shared]
    public sealed class BaseListCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS1722_BaseClassMustComeBeforeAnyInterface,
                    CompilerDiagnosticIdentifiers.CS0713_StaticClassCannotDeriveFromType,
                    CompilerDiagnosticIdentifiers.CS0714_StaticClassCannotImplementInterfaces);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out BaseListSyntax baseList))
                return;

            if (baseList.ContainsDiagnostics)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS1722_BaseClassMustComeBeforeAnyInterface:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.MoveBaseClassBeforeAnyInterface, context.Document, root.SyntaxTree))
                                return;

                            SeparatedSyntaxList<BaseTypeSyntax> types = baseList.Types;

                            if (types.Count > 1)
                            {
                                BaseTypeSyntax baseType = types.First(f => context.Span.Contains(f.Span));

                                CodeAction codeAction = CodeAction.Create(
                                    $"Move '{baseType.Type}' before any interface",
                                    ct =>
                                    {
                                        BaseTypeSyntax firstType = types[0];

                                        SeparatedSyntaxList<BaseTypeSyntax> newTypes = types
                                            .Replace(baseType, firstType.WithTriviaFrom(baseType))
                                            .ReplaceAt(0, baseType.WithTriviaFrom(firstType));

                                        BaseListSyntax newBaseList = baseList.WithTypes(newTypes);

                                        return context.Document.ReplaceNodeAsync(baseList, newBaseList, ct);
                                    },
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0713_StaticClassCannotDeriveFromType:
                    case CompilerDiagnosticIdentifiers.CS0714_StaticClassCannotImplementInterfaces:
                        {
                            if (baseList.Parent is not ClassDeclarationSyntax classDeclaration)
                                break;

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.MakeClassNonStatic, context.Document, root.SyntaxTree))
                            {
                                ModifiersCodeFixRegistrator.RemoveModifier(
                                    context,
                                    diagnostic,
                                    classDeclaration,
                                    SyntaxKind.StaticKeyword,
                                    title: "Make class non-static",
                                    additionalKey: CodeFixIdentifiers.MakeClassNonStatic);
                            }

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveBaseList, context.Document, root.SyntaxTree))
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    "Remove base list",
                                    ct =>
                                    {
                                        SyntaxToken token = baseList.GetFirstToken().GetPreviousToken();

                                        SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                                            .AddRange(baseList.GetLeadingTrivia().EmptyIfWhitespace())
                                            .AddRange(baseList.GetTrailingTrivia());

                                        ClassDeclarationSyntax newNode = classDeclaration
                                            .ReplaceToken(token, token.WithTrailingTrivia(trivia))
                                            .WithBaseList(null);

                                        return context.Document.ReplaceNodeAsync(classDeclaration, newNode, ct);
                                    },
                                    GetEquivalenceKey(diagnostic, CodeFixIdentifiers.RemoveBaseList));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                }
            }
        }
    }
}
