// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodDeclarationOrLocalFunctionStatementCodeFixProvider))]
[Shared]
public sealed class MethodDeclarationOrLocalFunctionStatementCodeFixProvider : CompilerDiagnosticCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS1624_BodyCannotBeIteratorBlockBecauseTypeIsNotIteratorInterfaceType); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];

        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeMethodReturnType, context.Document, root.SyntaxTree))
            return;

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => f.IsKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement)))
            return;

        BlockSyntax body = (node is MethodDeclarationSyntax methodDeclaration)
            ? methodDeclaration.Body
            : ((LocalFunctionStatementSyntax)node).Body;

        SyntaxDebug.Assert(body is not null, node);

        if (body is null)
            return;

        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

        ITypeSymbol typeSymbol = null;
        HashSet<ITypeSymbol> typeSymbols = null;
        INamedTypeSymbol ienumerableOfTSymbol = null;

        foreach (SyntaxNode descendant in body.DescendantNodes(descendIntoChildren: f => !CSharpFacts.IsFunction(f.Kind())))
        {
            if (!descendant.IsKind(SyntaxKind.YieldReturnStatement))
                continue;

            var yieldReturn = (YieldStatementSyntax)descendant;

            ExpressionSyntax expression = yieldReturn.Expression;

            if (expression is null)
                continue;

            var namedTypeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken) as INamedTypeSymbol;

            if (namedTypeSymbol?.IsErrorType() != false)
                continue;

            if (typeSymbol is null)
            {
                typeSymbol = namedTypeSymbol;
            }
            else
            {
                if (typeSymbols is null)
                {
                    typeSymbols = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default) { typeSymbol };
                }

                if (!typeSymbols.Add(namedTypeSymbol))
                    continue;
            }

            if (ienumerableOfTSymbol is null)
                ienumerableOfTSymbol = semanticModel.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1");

            CodeFixRegistrator.ChangeTypeOrReturnType(
                context,
                diagnostic,
                node,
                ienumerableOfTSymbol.Construct(namedTypeSymbol),
                semanticModel,
                additionalKey: SymbolDisplay.ToMinimalDisplayString(namedTypeSymbol, semanticModel, node.SpanStart, SymbolDisplayFormats.DisplayName));
        }
    }
}
