// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Helpers;
using Roslynator.CSharp.Refactorings;

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
                    CompilerDiagnosticIdentifiers.ExpressionBeingAssignedMustBeConstant);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.AddComparisonWithBooleanLiteral,
                CodeFixIdentifiers.CreateSingletonArray,
                CodeFixIdentifiers.UseUncheckedExpression,
                CodeFixIdentifiers.RemoveConstModifier))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            var expression = root.FindNode(context.Span, getInnermostNodeForTie: true) as ExpressionSyntax;

            Debug.Assert(expression != null, $"{nameof(expression)} is null");

            if (expression == null)
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

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddComparisonWithBooleanLiteral)
                                && type?.IsNullableOf(SpecialType.System_Boolean) == true)
                            {
                                if (convertedType?.IsBoolean() == true
                                    || AddComparisonWithBooleanLiteralRefactoring.IsCondition(expression))
                                {
                                    CodeAction codeAction = CodeAction.Create(
                                        AddComparisonWithBooleanLiteralRefactoring.GetTitle(expression),
                                        cancellationToken => AddComparisonWithBooleanLiteralRefactoring.RefactorAsync(context.Document, expression, cancellationToken),
                                        GetEquivalenceKey(diagnostic));

                                    context.RegisterCodeFix(codeAction, diagnostic);
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
                                        GetEquivalenceKey(diagnostic));

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

                            CodeAction codeAction = CodeAction.Create(
                                "Remove 'const' modifier",
                                cancellationToken =>
                                {
                                    LocalDeclarationStatementSyntax newNode = RemoveModifierHelper.RemoveModifier(localDeclarationStatement, SyntaxKind.ConstKeyword);

                                    return context.Document.ReplaceNodeAsync(localDeclarationStatement, newNode, cancellationToken);
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

                if (parent.IsKind(SyntaxKind.VariableDeclarator))
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
