// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AttributeCodeFixProvider))]
    [Shared]
    public sealed class AttributeCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0592_AttributeIsNotValidOnThisDeclarationType,
                    CompilerDiagnosticIdentifiers.CS1689_AttributeIsOnlyValidOnMethodsOrAttributeClasses);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveAttribute, context.Document, root.SyntaxTree))
                return;

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out AttributeSyntax attribute))
                return;

            CodeAction codeAction = CodeAction.Create(
                $"Remove attribute '{attribute.Name}'",
                ct =>
                {
                    var attributeList = (AttributeListSyntax)attribute.Parent;

                    SeparatedSyntaxList<AttributeSyntax> attributes = attributeList.Attributes;

                    if (attributes.Count == 1)
                    {
                        return context.Document.RemoveNodeAsync(attributeList, ct);
                    }
                    else
                    {
                        return context.Document.RemoveNodeAsync(attribute, ct);
                    }
                },
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
