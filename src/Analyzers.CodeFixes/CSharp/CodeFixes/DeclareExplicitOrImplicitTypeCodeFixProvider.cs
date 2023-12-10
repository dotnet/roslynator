// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp;
using static Roslynator.CSharp.CodeActionFactory;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DeclareExplicitOrImplicitTypeCodeFixProvider))]
[Shared]
public sealed class DeclareExplicitOrImplicitTypeCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.DeclareExplicitOrImplicitType); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(
            root,
            context.Span,
            out SyntaxNode node,
            predicate: f => f is TypeSyntax
                || f.IsKind(
                    SyntaxKind.VariableDeclaration,
                    SyntaxKind.DeclarationExpression,
                    SyntaxKind.ForEachStatement,
                    SyntaxKind.ForEachVariableStatement,
                    SyntaxKind.TupleExpression)))
        {
            return;
        }

        var document = context.Document;
        var diagnostic = context.Diagnostics[0];

        var semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

        if (node is TypeSyntax type)
        {
            if (type.IsVar)
            {
                Debug.Assert(node.IsParentKind(SyntaxKind.VariableDeclaration, SyntaxKind.DeclarationExpression, SyntaxKind.ForEachStatement), node.Parent.Kind().ToString());
                node = node.Parent;
            }
            else
            {
                var codeAction = ChangeTypeToVar(document, type, equivalenceKey: GetEquivalenceKey(diagnostic));
                context.RegisterCodeFix(codeAction, diagnostic);
                return;
            }
        }

        switch (node)
        {
            case TupleExpressionSyntax tupleExpression:
                {
                    var codeAction = ChangeTypeToVar(document, tupleExpression, equivalenceKey: GetEquivalenceKey(diagnostic));
                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            case VariableDeclarationSyntax variableDeclaration:
                {
                    var value = variableDeclaration.Variables[0].Initializer.Value;
                    var typeSymbol = semanticModel.GetTypeSymbol(value, context.CancellationToken);

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

                    var codeAction = UseExplicitType(document, variableDeclaration.Type, typeSymbol, semanticModel, equivalenceKey: GetEquivalenceKey(diagnostic));
                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            case DeclarationExpressionSyntax declarationExpression:
                {
                    var localSymbol = semanticModel.GetDeclaredSymbol(declarationExpression.Designation, context.CancellationToken) as ILocalSymbol;
                    var typeSymbol = (localSymbol?.Type) ?? semanticModel.GetTypeSymbol(declarationExpression, context.CancellationToken);

                    var codeAction = UseExplicitType(document, declarationExpression.Type, typeSymbol, semanticModel, equivalenceKey: GetEquivalenceKey(diagnostic));
                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            case ForEachStatementSyntax forEachStatement:
                {
                    var typeSymbol = semanticModel.GetForEachStatementInfo((CommonForEachStatementSyntax)node).ElementType;

                    var codeAction = UseExplicitType(document, forEachStatement.Type, typeSymbol, semanticModel, equivalenceKey: GetEquivalenceKey(diagnostic));
                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            case ForEachVariableStatementSyntax forEachVariableStatement:
                {
                    var declarationExpression = (DeclarationExpressionSyntax)forEachVariableStatement.Variable;
                    var typeSymbol = semanticModel.GetForEachStatementInfo((CommonForEachStatementSyntax)node).ElementType;

                    var codeAction = UseExplicitType(document, declarationExpression.Type, typeSymbol, semanticModel, equivalenceKey: GetEquivalenceKey(diagnostic));
                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
        }
    }
}
