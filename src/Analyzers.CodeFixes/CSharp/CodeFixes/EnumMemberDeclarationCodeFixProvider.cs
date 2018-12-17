// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumMemberDeclarationCodeFixProvider))]
    [Shared]
    public class EnumMemberDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.DeclareEnumValueAsCombinationOfNames); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out EnumMemberDeclarationSyntax enumMemberDeclaration))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.DeclareEnumValueAsCombinationOfNames:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Declare value as combination of names",
                                cancellationToken => DeclareEnumValueAsCombinationOfNamesAsync(context.Document, enumMemberDeclaration, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static async Task<Document> DeclareEnumValueAsCombinationOfNamesAsync(
            Document document,
            EnumMemberDeclarationSyntax enumMemberDeclaration,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IFieldSymbol fieldSymbol = semanticModel.GetDeclaredSymbol(enumMemberDeclaration, cancellationToken);

            EnumSymbolInfo enumInfo = EnumSymbolInfo.Create(fieldSymbol.ContainingType);

            EnumFieldSymbolInfo fieldInfo = EnumFieldSymbolInfo.Create(fieldSymbol);

            List<EnumFieldSymbolInfo> values = enumInfo.Decompose(fieldInfo);

            values.Sort((f, g) =>
            {
                if (f.HasCompositeValue())
                {
                    if (g.HasCompositeValue())
                    {
                        return f.Value.CompareTo(g.Value);
                    }
                    else
                    {
                        return -1;
                    }
                }
                else if (g.HasCompositeValue())
                {
                    return 1;
                }

                return f.Value.CompareTo(g.Value);
            });

            ExpressionSyntax oldValue = enumMemberDeclaration.EqualsValue.Value;

            BinaryExpressionSyntax newValue = BitwiseOrExpression(CreateIdentifierName(values[0]), CreateIdentifierName(values[1]));

            for (int i = 2; i < values.Count; i++)
                newValue = BitwiseOrExpression(newValue, CreateIdentifierName(values[i]));

            newValue = newValue.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(oldValue, newValue, cancellationToken).ConfigureAwait(false);
        }

        private static IdentifierNameSyntax CreateIdentifierName(in EnumFieldSymbolInfo fieldInfo)
        {
            return SyntaxFactory.IdentifierName(fieldInfo.Name);
        }
    }
}
