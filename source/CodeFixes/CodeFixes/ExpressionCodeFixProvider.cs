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
                    CompilerDiagnosticIdentifiers.CannotConvertNullToTypeParameterBecauseItCouldBeNonNullableValueType);
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
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConditionThatIsAlwaysEqualToTrueOrFalse))
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

                            if (type?.IsNamedType() == true)
                            {
                                var namedType = (INamedTypeSymbol)type;

                                if (namedType.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
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
                            }

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.CreateSingletonArray)
                                && type?.IsErrorType() == false
                                && !type.Equals(convertedType)
                                && convertedType.IsArrayType())
                            {
                                var arrayType = (IArrayTypeSymbol)convertedType;

                                if (semanticModel.IsImplicitConversion(expression, arrayType.ElementType))
                                {
                                    CodeAction codeAction = CodeAction.Create(
                                        "Create singleton array",
                                        cancellationToken => CreateSingletonArrayRefactoring.RefactorAsync(context.Document, expression, arrayType.ElementType, semanticModel, cancellationToken),
                                        GetEquivalenceKey(diagnostic, CodeFixIdentifiers.CreateSingletonArray));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
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

                            ModifiersCodeFixes.RemoveModifier(context, diagnostic, localDeclarationStatement, SyntaxKind.ConstKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CannotConvertNullToTypeBecauseItIsNonNullableValueType:
                    case CompilerDiagnosticIdentifiers.CannotConvertNullToTypeParameterBecauseItCouldBeNonNullableValueType:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReplaceNullLiteralExpressionWithDefaultValue))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(expression, context.CancellationToken).ConvertedType;

                            if (typeSymbol?.SupportsExplicitDeclaration() != true)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Replace 'null' with default value",
                                cancellationToken =>
                                {
                                    ExpressionSyntax newNode = typeSymbol.ToDefaultValueSyntax(semanticModel, expression.SpanStart);

                                    return context.Document.ReplaceNodeAsync(expression, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ResultOfExpressionIsAlwaysConstantSinceValueIsNeverEqualToNull:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveConditionThatIsAlwaysEqualToTrueOrFalse))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            if (!NullCheckExpression.TryCreate(expression, semanticModel, out NullCheckExpression nullCheck, context.CancellationToken))
                                break;

                            if (nullCheck.Kind != NullCheckKind.EqualsToNull
                                && nullCheck.Kind!= NullCheckKind.NotEqualsToNull)
                            {
                                break;
                            }

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
