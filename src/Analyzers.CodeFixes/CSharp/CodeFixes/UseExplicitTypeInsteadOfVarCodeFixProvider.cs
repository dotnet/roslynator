// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseExplicitTypeInsteadOfVarCodeFixProvider))]
[Shared]
public sealed class UseExplicitTypeInsteadOfVarCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious,
                DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarWhenTypeIsObvious);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => f.IsKind(SyntaxKind.VariableDeclaration, SyntaxKind.DeclarationExpression)))
            return;

        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

        if (node is VariableDeclarationSyntax variableDeclaration)
        {
            ExpressionSyntax value = variableDeclaration.Variables[0].Initializer.Value;
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(value, context.CancellationToken);

            if (typeSymbol is null)
            {
                var localSymbol = semanticModel.GetDeclaredSymbol(variableDeclaration.Variables[0], context.CancellationToken) as ILocalSymbol;

                if (localSymbol is not null)
                {
                    typeSymbol = localSymbol.Type;

                    value = value.WalkDownParentheses();

                    Debug.Assert(
                        value.IsKind(SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression),
                        value.Kind().ToString());

                    if (value.IsKind(SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression))
                        typeSymbol = typeSymbol.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
                }
                else
                {
                    SyntaxDebug.Fail(variableDeclaration.Variables[0]);
                    return;
                }
            }

            RegisterCodeFix(context, variableDeclaration.Type, typeSymbol, semanticModel);
        }
        else
        {
            var declarationExpression = (DeclarationExpressionSyntax)node;

            TypeSyntax type = declarationExpression.Type;

            var localSymbol = semanticModel.GetDeclaredSymbol(declarationExpression.Designation, context.CancellationToken) as ILocalSymbol;

            ITypeSymbol typeSymbol = (localSymbol?.Type) ?? semanticModel.GetTypeSymbol(declarationExpression, context.CancellationToken);

            RegisterCodeFix(context, type, typeSymbol, semanticModel);
        }
    }

    private static void RegisterCodeFix(CodeFixContext context, TypeSyntax type, ITypeSymbol typeSymbol, SemanticModel semanticModel)
    {
        foreach (Diagnostic diagnostic in context.Diagnostics)
        {
            CodeAction codeAction = CodeActionFactory.UseExplicitType(context.Document, type, typeSymbol, semanticModel, equivalenceKey: GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
