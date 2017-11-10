// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExpressionCodeFixProvider))]
    [Shared]
    public class ExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CannotImplicitlyConvertTypeExplicitConversionExists,
                    CompilerDiagnosticIdentifiers.ConstantValueCannotBeConverted,
                    CompilerDiagnosticIdentifiers.ExpressionBeingAssignedMustBeConstant,
                    CompilerDiagnosticIdentifiers.CannotConvertNullToTypeBecauseItIsNonNullableValueType,
                    CompilerDiagnosticIdentifiers.ResultOfExpressionIsAlwaysConstantSinceValueIsNeverEqualToNull,
                    CompilerDiagnosticIdentifiers.CannotConvertNullToTypeParameterBecauseItCouldBeNonNullableValueType,
                    CompilerDiagnosticIdentifiers.OnlyAssignmentCallIncrementDecrementAndNewObjectExpressionsCanBeUsedAsStatement,
                    CompilerDiagnosticIdentifiers.CannotImplicitlyConvertType);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddComparisonWithBooleanLiteral)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.CreateSingletonArray)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.UseUncheckedExpression)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConstModifier)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReplaceNullLiteralExpressionWithDefaultValue)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.UseCoalesceExpression)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConditionThatIsAlwaysEqualToTrueOrFalse)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.IntroduceLocalVariable)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.IntroduceField)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReplaceStringLiteralWithCharacterLiteral)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.UseYieldReturnInsteadOfReturn)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeMemberTypeAccordingToReturnExpression)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddArgumentList))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindNode(root, context.Span, out ExpressionSyntax expression))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CannotImplicitlyConvertTypeExplicitConversionExists:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            TypeInfo typeInfo = semanticModel.GetTypeInfo(expression, context.CancellationToken);

                            ITypeSymbol type = typeInfo.Type;
                            ITypeSymbol convertedType = typeInfo.ConvertedType;

                            if ((type is INamedTypeSymbol namedType)
                                && namedType.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
                            {
                                if (convertedType?.IsBoolean() == true
                                    || AddComparisonWithBooleanLiteralRefactoring.IsCondition(expression))
                                {
                                    if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddComparisonWithBooleanLiteral))
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            AddComparisonWithBooleanLiteralRefactoring.GetTitle(expression),
                                            cancellationToken => AddComparisonWithBooleanLiteralRefactoring.RefactorAsync(context.Document, expression, cancellationToken),
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.AddComparisonWithBooleanLiteral));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                                else if (namedType.TypeArguments[0].Equals(convertedType))
                                {
                                    if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.UseCoalesceExpression))
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Use coalesce expression",
                                            cancellationToken =>
                                            {
                                                ExpressionSyntax defaultValue = convertedType.ToDefaultValueSyntax(semanticModel, expression.SpanStart);

                                                ExpressionSyntax newNode = CoalesceExpression(expression.WithoutTrivia(), defaultValue)
                                                    .WithTriviaFrom(expression)
                                                    .Parenthesize()
                                                    .WithFormatterAnnotation();

                                                return context.Document.ReplaceNodeAsync(expression, newNode, cancellationToken);
                                            },
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.UseCoalesceExpression));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                            }

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeMemberTypeAccordingToReturnExpression)
                                && expression.IsParentKind(SyntaxKind.ReturnStatement, SyntaxKind.YieldReturnStatement))
                            {
                                ChangeMemberTypeRefactoring.ComputeCodeFix(context, diagnostic, expression, semanticModel);
                            }

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddCastExpression))
                                CodeFixRegistrator.AddCastExpression(context, diagnostic, expression, convertedType, semanticModel);

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.CreateSingletonArray)
                                && type?.IsErrorType() == false
                                && !type.Equals(convertedType)
                                && (convertedType is IArrayTypeSymbol arrayType)
                                && semanticModel.IsImplicitConversion(expression, arrayType.ElementType))
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    "Create singleton array",
                                    cancellationToken => CreateSingletonArrayRefactoring.RefactorAsync(context.Document, expression, arrayType.ElementType, semanticModel, cancellationToken),
                                    GetEquivalenceKey(diagnostic, CodeFixIdentifiers.CreateSingletonArray));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ConstantValueCannotBeConverted:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.UseUncheckedExpression))
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Use 'unchecked'",
                                cancellationToken =>
                                {
                                    CheckedExpressionSyntax newNode = CSharpFactory.UncheckedExpression(expression.WithoutTrivia());

                                    newNode = newNode.WithTriviaFrom(expression);

                                    return context.Document.ReplaceNodeAsync(expression, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ExpressionBeingAssignedMustBeConstant:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConstModifier))
                                break;

                            LocalDeclarationStatementSyntax localDeclarationStatement = GetLocalDeclarationStatement(expression);

                            if (localDeclarationStatement == null)
                                break;

                            SyntaxTokenList modifiers = localDeclarationStatement.Modifiers;

                            if (!modifiers.Contains(SyntaxKind.ConstKeyword))
                                break;

                            ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, localDeclarationStatement, SyntaxKind.ConstKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CannotConvertNullToTypeBecauseItIsNonNullableValueType:
                    case CompilerDiagnosticIdentifiers.CannotConvertNullToTypeParameterBecauseItCouldBeNonNullableValueType:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReplaceNullLiteralExpressionWithDefaultValue))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            CodeFixRegistrator.ReplaceNullWithDefaultValue(context, diagnostic, expression, semanticModel);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ResultOfExpressionIsAlwaysConstantSinceValueIsNeverEqualToNull:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConditionThatIsAlwaysEqualToTrueOrFalse))
                                break;

                            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(expression, allowedKinds: NullCheckKind.ComparisonToNull);

                            if (!nullCheck.Success)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Remove condition",
                                cancellationToken =>
                                {
                                    SyntaxNode newRoot = RemoveHelper.RemoveCondition(root, expression, nullCheck.Kind == NullCheckKind.NotEqualsToNull);

                                    return Task.FromResult(context.Document.WithSyntaxRoot(newRoot));
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.OnlyAssignmentCallIncrementDecrementAndNewObjectExpressionsCanBeUsedAsStatement:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            if (expression.Parent is ArrowExpressionClauseSyntax arrowExpresssionClause)
                            {
                                if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeMemberTypeAccordingToReturnExpression))
                                    break;

                                ChangeMemberTypeRefactoring.ComputeCodeFix(context, diagnostic, expression, semanticModel);
                            }
                            else if (expression.Parent is ExpressionStatementSyntax expressionStatement)
                            {
                                if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddArgumentList)
                                    && expression.IsKind(
                                        SyntaxKind.IdentifierName,
                                        SyntaxKind.SimpleMemberAccessExpression))
                                {
                                    SyntaxNode invocationExpression = SyntaxFactory.InvocationExpression(expression);

                                    if (semanticModel.GetSpeculativeMethodSymbol(expression.SpanStart, invocationExpression) != null)
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Add argument list",
                                            cancellationToken => context.Document.ReplaceNodeAsync(expression, invocationExpression, cancellationToken),
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.AddArgumentList));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }

                                if (Settings.IsAnyCodeFixEnabled(
                                    CodeFixIdentifiers.IntroduceLocalVariable,
                                    CodeFixIdentifiers.IntroduceField))
                                {
                                    if (semanticModel.GetSymbol(expression, context.CancellationToken)?.IsErrorType() != false)
                                        break;

                                    ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                                    if (typeSymbol?.IsErrorType() != false)
                                        break;

                                    if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.IntroduceLocalVariable)
                                        && !expressionStatement.IsEmbedded())
                                    {
                                        bool addAwait = typeSymbol.IsConstructedFromTaskOfT(semanticModel)
                                            && semanticModel.GetEnclosingSymbol(expressionStatement.SpanStart, context.CancellationToken).IsAsyncMethod();

                                        CodeAction codeAction = CodeAction.Create(
                                            IntroduceLocalVariableRefactoring.GetTitle(expression),
                                            cancellationToken => IntroduceLocalVariableRefactoring.RefactorAsync(context.Document, expressionStatement, typeSymbol, addAwait, semanticModel, cancellationToken),
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.IntroduceLocalVariable));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }

                                    if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.IntroduceField))
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            $"Introduce field for '{expression}'",
                                            cancellationToken => IntroduceFieldRefactoring.RefactorAsync(context.Document, expressionStatement, typeSymbol, semanticModel, cancellationToken),
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.IntroduceField));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CannotImplicitlyConvertType:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeMemberTypeAccordingToReturnExpression)
                                && expression.IsParentKind(SyntaxKind.ReturnStatement, SyntaxKind.YieldReturnStatement))
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ChangeMemberTypeRefactoring.ComputeCodeFix(context, diagnostic, expression, semanticModel);
                                break;
                            }

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReplaceStringLiteralWithCharacterLiteral)
                                && expression?.IsKind(SyntaxKind.StringLiteralExpression) == true)
                            {
                                var literalExpression = (LiteralExpressionSyntax)expression;

                                if (literalExpression.Token.ValueText.Length == 1)
                                {
                                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                    if (semanticModel.GetTypeInfo(expression, context.CancellationToken).ConvertedType?.IsChar() == true)
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Replace string literal with character literal",
                                            cancellationToken => ReplaceStringLiteralWithCharacterLiteralRefactoring.RefactorAsync(context.Document, literalExpression, cancellationToken),
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.ReplaceStringLiteralWithCharacterLiteral));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                            }

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.UseYieldReturnInsteadOfReturn)
                                && expression.IsParentKind(SyntaxKind.ReturnStatement))
                            {
                                var returnStatement = (ReturnStatementSyntax)expression.Parent;

                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ISymbol containingSymbol = semanticModel.GetEnclosingSymbol(returnStatement.SpanStart, context.CancellationToken);

                                if (containingSymbol?.IsKind(SymbolKind.Method) == true
                                    && ((IMethodSymbol)containingSymbol).ReturnType?.IsIEnumerableOrConstructedFromIEnumerableOfT() == true)
                                {
                                    CodeAction codeAction = CodeAction.Create(
                                        "Use yield return instead of return",
                                        cancellationToken => UseYieldReturnInsteadOfReturnRefactoring.RefactorAsync(context.Document, returnStatement, SyntaxKind.YieldReturnStatement, semanticModel, cancellationToken),
                                        GetEquivalenceKey(diagnostic, CodeFixIdentifiers.UseYieldReturnInsteadOfReturn));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
                            }

                            break;
                        }
                }
            }
        }

        private static LocalDeclarationStatementSyntax GetLocalDeclarationStatement(ExpressionSyntax expression)
        {
            SyntaxNode parent = expression.Parent;

            if (parent?.IsKind(SyntaxKind.EqualsValueClause) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.VariableDeclarator) == true)
                {
                    parent = parent.Parent;

                    if (parent?.IsKind(SyntaxKind.VariableDeclaration) == true)
                    {
                        parent = parent.Parent;

                        if (parent?.IsKind(SyntaxKind.LocalDeclarationStatement) == true)
                        {
                            return (LocalDeclarationStatementSyntax)parent;
                        }
                    }
                }
            }

            return null;
        }
    }
}
