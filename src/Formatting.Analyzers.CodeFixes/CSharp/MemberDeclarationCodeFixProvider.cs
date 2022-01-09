// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MemberDeclarationCodeFixProvider))]
    [Shared]
    public sealed class MemberDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.FormatTypeDeclarationBraces,
                    DiagnosticIdentifiers.PutConstructorInitializerOnItsOwnLine);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out MemberDeclarationSyntax memberDeclaration))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.FormatTypeDeclarationBraces:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Format braces on multiple lines",
                            ct => FormatTypeDeclarationBracesOnMultipleLinesAsync(document, memberDeclaration, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.PutConstructorInitializerOnItsOwnLine:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Put constructor initializer on its own line",
                            ct =>
                            {
                                return CodeFixHelpers.AddNewLineBeforeAndIncreaseIndentationAsync(
                                    document,
                                    ((ConstructorDeclarationSyntax)memberDeclaration).Initializer.ColonToken,
                                    SyntaxTriviaAnalysis.AnalyzeIndentation(memberDeclaration, ct),
                                    ct);
                            },
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> FormatTypeDeclarationBracesOnMultipleLinesAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newDeclaration = GetNewDeclaration().WithFormatterAnnotation();

            return document.ReplaceNodeAsync(declaration, newDeclaration, cancellationToken);

            MemberDeclarationSyntax GetNewDeclaration()
            {
                switch (declaration)
                {
                    case ClassDeclarationSyntax classDeclaration:
                        return classDeclaration.WithCloseBraceToken(classDeclaration.CloseBraceToken.AppendEndOfLineToLeadingTrivia());
                    case StructDeclarationSyntax structDeclaration:
                        return structDeclaration.WithCloseBraceToken(structDeclaration.CloseBraceToken.AppendEndOfLineToLeadingTrivia());
                    case InterfaceDeclarationSyntax interfaceDeclaration:
                        return interfaceDeclaration.WithCloseBraceToken(interfaceDeclaration.CloseBraceToken.AppendEndOfLineToLeadingTrivia());
                    case RecordDeclarationSyntax recordDeclaration:
                        return recordDeclaration.WithCloseBraceToken(recordDeclaration.CloseBraceToken.AppendEndOfLineToLeadingTrivia());
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
