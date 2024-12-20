// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using static Roslynator.CSharp.CodeActionFactory;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseVarOrExplicitTypeCodeFixProvider))]
[Shared]
public sealed class UseVarOrExplicitTypeCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIds.UseVarOrExplicitType); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

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

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

        if (node is TypeSyntax type)
        {
            if (type.IsVar)
            {
                if (node.IsParentKind(SyntaxKind.VariableDeclaration, SyntaxKind.ForEachStatement))
                {
                    node = node.Parent;
                }
                else if (node.IsParentKind(SyntaxKind.DeclarationExpression))
                {
                    node = node.Parent;

                    if (node.IsParentKind(SyntaxKind.ForEachVariableStatement))
                        node = node.Parent;
                }
            }
            else
            {
                CodeAction codeAction = ChangeTypeToVar(document, type, equivalenceKey: GetEquivalenceKey(diagnostic, "ToImplicit"));
                context.RegisterCodeFix(codeAction, diagnostic);
                return;
            }
        }

        switch (node)
        {
            case TupleExpressionSyntax tupleExpression:
            {
                CodeAction codeAction = ChangeTypeToVar(document, tupleExpression, equivalenceKey: GetEquivalenceKey(diagnostic, "ToImplicit"));
                context.RegisterCodeFix(codeAction, diagnostic);
                break;
            }
            case VariableDeclarationSyntax variableDeclaration:
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

                CodeAction codeAction = UseExplicitType(document, variableDeclaration.Type, typeSymbol, semanticModel, equivalenceKey: GetEquivalenceKey(diagnostic, "ToExplicit"));
                context.RegisterCodeFix(codeAction, diagnostic);
                break;
            }
            case DeclarationExpressionSyntax declarationExpression:
            {
                ITypeSymbol typeSymbol = null;
                if (declarationExpression.Parent is AssignmentExpressionSyntax assignment)
                {
                    typeSymbol = semanticModel.GetTypeSymbol(assignment.Right, context.CancellationToken);
                }
                else if (declarationExpression.Parent is ArgumentSyntax argument)
                {
                    IParameterSymbol parameterSymbol = DetermineParameterHelper.DetermineParameter(argument, semanticModel, cancellationToken: context.CancellationToken);
                    typeSymbol = parameterSymbol?.Type;
                }

                if (typeSymbol is null)
                {
                    var localSymbol = semanticModel.GetDeclaredSymbol(declarationExpression.Designation, context.CancellationToken) as ILocalSymbol;
                    typeSymbol = (localSymbol?.Type) ?? semanticModel.GetTypeSymbol(declarationExpression, context.CancellationToken);
                }

                CodeAction codeAction = UseExplicitType(document, declarationExpression.Type, typeSymbol, semanticModel, equivalenceKey: GetEquivalenceKey(diagnostic, "ToExplicit"));
                context.RegisterCodeFix(codeAction, diagnostic);
                break;
            }
            case ForEachStatementSyntax forEachStatement:
            {
                ITypeSymbol typeSymbol = semanticModel.GetForEachStatementInfo((CommonForEachStatementSyntax)node).ElementType;

                CodeAction codeAction = UseExplicitType(document, forEachStatement.Type, typeSymbol, semanticModel, equivalenceKey: GetEquivalenceKey(diagnostic, "ToExplicit"));
                context.RegisterCodeFix(codeAction, diagnostic);
                break;
            }
            case ForEachVariableStatementSyntax forEachVariableStatement:
            {
                var declarationExpression = (DeclarationExpressionSyntax)forEachVariableStatement.Variable;
                ITypeSymbol typeSymbol = semanticModel.GetForEachStatementInfo((CommonForEachStatementSyntax)node).ElementType;

                CodeAction codeAction = UseExplicitType(document, declarationExpression.Type, typeSymbol, semanticModel, equivalenceKey: GetEquivalenceKey(diagnostic, "ToExplicit"));
                context.RegisterCodeFix(codeAction, diagnostic);
                break;
            }
        }
    }
}
