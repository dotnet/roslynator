// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MemberDeclarationCodeFixProvider))]
    [Shared]
    public class MemberDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.FormatDeclarationBraces,
                    DiagnosticIdentifiers.MarkMemberAsStatic,
                    DiagnosticIdentifiers.RemoveRedundantOverridingMember,
                    DiagnosticIdentifiers.AddDocumentationComment,
                    DiagnosticIdentifiers.MarkContainingClassAsAbstract);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            MemberDeclarationSyntax memberDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberDeclarationSyntax>();

            Debug.Assert(memberDeclaration != null, $"{nameof(memberDeclaration)} is null");

            if (memberDeclaration == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.FormatDeclarationBraces:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Format braces",
                                cancellationToken => FormatDeclarationBracesRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.MarkMemberAsStatic:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Mark {GetMemberName(memberDeclaration)} as static",
                                cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AddDocumentationComment:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Add documentation comment",
                                cancellationToken => AddDocumentationCommentRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantOverridingMember:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Remove redundant overridding {GetMemberName(memberDeclaration)}",
                                cancellationToken => Remover.RemoveMemberAsync(context.Document, memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.MarkContainingClassAsAbstract:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Mark containing class as abstract",
                                cancellationToken => MarkContainingClassAsAbstractRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private object GetMemberName(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return "method";
                case SyntaxKind.ConstructorDeclaration:
                    return "constructor";
                case SyntaxKind.PropertyDeclaration:
                    return "property";
                case SyntaxKind.IndexerDeclaration:
                    return "indexer";
                case SyntaxKind.FieldDeclaration:
                    return "field";
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    return "event";
            }

            Debug.Assert(false, memberDeclaration.Kind().ToString());
            return "member";
        }
    }
}
