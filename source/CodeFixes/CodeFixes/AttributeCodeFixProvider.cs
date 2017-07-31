// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AttributeCodeFixProvider))]
    [Shared]
    public class AttributeCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.AttributeIsNotValidOnThisDeclarationType,
                    CompilerDiagnosticIdentifiers.AttributeIsOnlyValidOnMethodsOrAttributeClasses);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveAttribute))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            //TODO: TryFindFirstAncestorOrSelf
            AttributeSyntax attribute = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<AttributeSyntax>();

            if (attribute == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.AttributeIsNotValidOnThisDeclarationType:
                    case CompilerDiagnosticIdentifiers.AttributeIsOnlyValidOnMethodsOrAttributeClasses:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Remove attribute '{attribute.Name}'",
                                cancellationToken =>
                                {
                                    var attributeList = (AttributeListSyntax)attribute.Parent;

                                    SeparatedSyntaxList<AttributeSyntax> attributes = attributeList.Attributes;

                                    if (attributes.Count == 1)
                                    {
                                        return context.Document.RemoveNodeAsync(attributeList, RemoveHelper.GetRemoveOptions(attributeList), cancellationToken);
                                    }
                                    else
                                    {
                                        return context.Document.RemoveNodeAsync(attribute, RemoveHelper.GetRemoveOptions(attributeList), cancellationToken);
                                    }
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
