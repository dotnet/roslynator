// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveDuplicateAttributeCodeFixProvider))]
    [Shared]
    public sealed class RemoveDuplicateAttributeCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS0579_DuplicateAttribute); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveDuplicateAttribute, context.Document, root.SyntaxTree))
                return;

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out AttributeSyntax attribute))
                return;

            SyntaxNode parent = attribute.Parent;

            if (parent is AttributeListSyntax attributeList)
            {
                CodeAction codeAction = CodeAction.Create(
                    "Remove duplicate attribute",
                    ct =>
                    {
                        SeparatedSyntaxList<AttributeSyntax> attributes = attributeList.Attributes;

                        if (attributes.Count == 1)
                        {
                            return context.Document.RemoveNodeAsync(attributeList, SyntaxRemoveOptions.KeepUnbalancedDirectives, ct);
                        }
                        else
                        {
                            return context.Document.RemoveNodeAsync(attribute, SyntaxRemoveOptions.KeepUnbalancedDirectives, ct);
                        }
                    },
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }
    }
}
