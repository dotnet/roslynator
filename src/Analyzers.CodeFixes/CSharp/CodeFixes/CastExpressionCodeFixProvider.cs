// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CastExpressionCodeFixProvider))]
[Shared]
public sealed class CastExpressionCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.UseEnumFieldExplicitly); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out CastExpressionSyntax castExpression))
            return;

        Diagnostic diagnostic = context.Diagnostics[0];
        Document document = context.Document;

        CodeAction codeAction = CodeAction.Create(
            "Use enum field explicitly",
            ct => UseEnumFieldExplicitlyAsync(castExpression, document, ct),
            GetEquivalenceKey(DiagnosticIdentifiers.UseEnumFieldExplicitly));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    private static async Task<Document> UseEnumFieldExplicitlyAsync(
        CastExpressionSyntax castExpression,
        Document document,
        CancellationToken cancellationToken)
    {
        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        Optional<object> constantValueOpt = semanticModel.GetConstantValue(castExpression.Expression, cancellationToken);

        var enumSymbol = (INamedTypeSymbol)semanticModel.GetTypeSymbol(castExpression.Type, cancellationToken);

        if (enumSymbol.HasAttribute(MetadataNames.System_FlagsAttribute))
        {
            ulong value = SymbolUtility.GetEnumValueAsUInt64(constantValueOpt.Value, enumSymbol);

            List<ulong> flags = FlagsUtility<ulong>.Instance.GetFlags(value).ToList();

            List<EnumFieldSymbolInfo> fields = EnumSymbolInfo.Create(enumSymbol).Fields
                .Where(f => flags.Contains(f.Value))
                .OrderByDescending(f => f.Value)
                .ToList();

            ExpressionSyntax newExpression = CreateEnumFieldExpression(fields[0].Symbol);

            for (int i = 1; i < fields.Count; i++)
            {
                newExpression = BitwiseOrExpression(
                    CreateEnumFieldExpression(fields[i].Symbol),
                    newExpression);
            }

            newExpression = newExpression.WithTriviaFrom(castExpression);

            return await document.ReplaceNodeAsync(castExpression, newExpression, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            IFieldSymbol symbol = enumSymbol
                .GetMembers()
                .OfType<IFieldSymbol>()
                .First(fieldSymbol =>
                {
                    return fieldSymbol.HasConstantValue
                        && constantValueOpt.Value.Equals(fieldSymbol.ConstantValue);
                });

            ExpressionSyntax newExpression = CreateEnumFieldExpression(symbol).WithTriviaFrom(castExpression);

            return await document.ReplaceNodeAsync(castExpression, newExpression, cancellationToken).ConfigureAwait(false);
        }

        static MemberAccessExpressionSyntax CreateEnumFieldExpression(IFieldSymbol symbol)
        {
            return SimpleMemberAccessExpression(
                symbol.Type.ToTypeSyntax().WithSimplifierAnnotation(),
                IdentifierName(symbol.Name));
        }
    }
}
