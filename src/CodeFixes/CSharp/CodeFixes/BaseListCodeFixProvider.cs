// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BaseListCodeFixProvider))]
    [Shared]
    public class BaseListCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.BaseClassMustComeBeforeAnyInterface); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.MoveBaseClassBeforeAnyInterface))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out BaseListSyntax baseList))
                return;

            if (baseList.ContainsDiagnostics)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.BaseClassMustComeBeforeAnyInterface:
                        {
                            SeparatedSyntaxList<BaseTypeSyntax> types = baseList.Types;

                            if (types.Count > 1)
                            {
                                BaseTypeSyntax baseType = types.First(f => context.Span.Contains(f.Span));

                                CodeAction codeAction = CodeAction.Create(
                                    $"Move '{baseType.Type}' before any interface",
                                    cancellationToken =>
                                    {
                                        BaseTypeSyntax firstType = types[0];

                                        SeparatedSyntaxList<BaseTypeSyntax> newTypes = types
                                            .Replace(baseType, firstType.WithTriviaFrom(baseType))
                                            .ReplaceAt(0, baseType.WithTriviaFrom(firstType));

                                        BaseListSyntax newBaseList = baseList.WithTypes(newTypes);

                                        return context.Document.ReplaceNodeAsync(baseList, newBaseList, cancellationToken);
                                    },
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                }
            }
        }
    }
}
