// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExpressionCodeFixProvider))]
    [Shared]
    public sealed class ExpressionCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0266_CannotImplicitlyConvertTypeExplicitConversionExists,
                    CompilerDiagnosticIdentifiers.CS0221_ConstantValueCannotBeConverted,
                    CompilerDiagnosticIdentifiers.CS0133_ExpressionBeingAssignedMustBeConstant,
                    CompilerDiagnosticIdentifiers.CS0037_CannotConvertNullToTypeBecauseItIsNonNullableValueType,
                    CompilerDiagnosticIdentifiers.CS0472_ResultOfExpressionIsAlwaysConstantSinceValueIsNeverEqualToNull,
                    CompilerDiagnosticIdentifiers.CS0403_CannotConvertNullToTypeParameterBecauseItCouldBeNonNullableValueType,
                    CompilerDiagnosticIdentifiers.CS0201_OnlyAssignmentCallIncrementDecrementAndNewObjectExpressionsCanBeUsedAsStatement,
                    CompilerDiagnosticIdentifiers.CS0029_CannotImplicitlyConvertType,
                    CompilerDiagnosticIdentifiers.CS0131_LeftHandSideOfAssignmentMustBeVariablePropertyOrIndexer,
                    CompilerDiagnosticIdentifiers.CS0191_ReadOnlyFieldCannotBeAssignedTo);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindNode(root, context.Span, out ExpressionSyntax expression))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS0266_CannotImplicitlyConvertTypeExplicitConversionExists:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            TypeInfo typeInfo = semanticModel.GetTypeInfo(expression, context.CancellationToken);

                            ITypeSymbol type = typeInfo.Type;
                            ITypeSymbol convertedType = typeInfo.ConvertedType;

                            if ((type is INamedTypeSymbol namedType)
                                && namedType.IsNullableType())
                            {
                                if (convertedType?.SpecialType == SpecialType.System_Boolean
                                    || AddComparisonWithBooleanLiteralRefactoring.IsCondition(expression))
                                {
                                    if (!IsRightExpressionOfAssignment(expression)
                                        && IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddComparisonWithBooleanLiteral, context.Document, root.SyntaxTree))
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            AddComparisonWithBooleanLiteralRefactoring.GetTitle(expression),
                                            ct => AddComparisonWithBooleanLiteralRefactoring.RefactorAsync(context.Document, expression, ct),
                                            GetEquivalenceKey(
                                                diagnostic,
                                                CodeFixIdentifiers.AddComparisonWithBooleanLiteral));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }

                                    static bool IsRightExpressionOfAssignment(ExpressionSyntax expression)
                                    {
                                        return expression.IsParentKind(SyntaxKind.SimpleAssignmentExpression)
                                            && ((AssignmentExpressionSyntax)expression.Parent).Right == expression;
                                    }
                                }
                                else if (SymbolEqualityComparer.Default.Equals(namedType.TypeArguments[0], convertedType))
                                {
                                    if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.UseCoalesceExpression, context.Document, root.SyntaxTree))
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Use coalesce expression",
                                            ct =>
                                            {
                                                ExpressionSyntax defaultValue = convertedType.GetDefaultValueSyntax(context.Document.GetDefaultSyntaxOptions());

                                                ExpressionSyntax newNode = CoalesceExpression(expression.WithoutTrivia(), defaultValue)
                                                    .WithTriviaFrom(expression)
                                                    .Parenthesize()
                                                    .WithFormatterAnnotation();

                                                return context.Document.ReplaceNodeAsync(expression, newNode, ct);
                                            },
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.UseCoalesceExpression));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                            }

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeMemberTypeAccordingToReturnExpression, context.Document, root.SyntaxTree)
                                && expression.IsParentKind(SyntaxKind.ReturnStatement, SyntaxKind.YieldReturnStatement))
                            {
                                ChangeMemberTypeRefactoring.ComputeCodeFix(context, diagnostic, expression, semanticModel);
                            }

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddExplicitCast, context.Document, root.SyntaxTree))
                                CodeFixRegistrator.AddExplicitCast(context, diagnostic, expression, convertedType, semanticModel);

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeTypeAccordingToInitializer, context.Document, root.SyntaxTree))
                                ChangeTypeAccordingToInitializerRefactoring.ComputeCodeFix(context, diagnostic, expression, semanticModel);

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.CreateSingletonArray, context.Document, root.SyntaxTree)
                                && type?.IsErrorType() == false
                                && !SymbolEqualityComparer.Default.Equals(type, convertedType)
                                && (convertedType is IArrayTypeSymbol arrayType)
                                && semanticModel.IsImplicitConversion(expression, arrayType.ElementType))
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    "Create singleton array",
                                    ct => CreateSingletonArrayRefactoring.RefactorAsync(context.Document, expression, arrayType.ElementType, semanticModel, ct),
                                    GetEquivalenceKey(diagnostic, CodeFixIdentifiers.CreateSingletonArray));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0221_ConstantValueCannotBeConverted:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.UseUncheckedExpression, context.Document, root.SyntaxTree))
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Use 'unchecked'",
                                ct =>
                                {
                                    CheckedExpressionSyntax newNode = CSharpFactory.UncheckedExpression(expression.WithoutTrivia());

                                    newNode = newNode.WithTriviaFrom(expression);

                                    return context.Document.ReplaceNodeAsync(expression, newNode, ct);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0133_ExpressionBeingAssignedMustBeConstant:
                        {
                            SyntaxNode parent = expression.Parent;

                            if (parent?.IsKind(SyntaxKind.EqualsValueClause) != true)
                                break;

                            parent = parent.Parent;

                            if (parent?.IsKind(SyntaxKind.VariableDeclarator) != true)
                                break;

                            parent = parent.Parent;

                            if (parent is not VariableDeclarationSyntax variableDeclaration)
                                break;

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveConstModifier, context.Document, root.SyntaxTree)
                                && variableDeclaration.Parent is LocalDeclarationStatementSyntax localDeclarationStatement)
                            {
                                SyntaxTokenList modifiers = localDeclarationStatement.Modifiers;

                                if (!modifiers.Contains(SyntaxKind.ConstKeyword))
                                    break;

                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, localDeclarationStatement, SyntaxKind.ConstKeyword);
                            }
                            else if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReplaceConstantWithField, context.Document, root.SyntaxTree)
                                && variableDeclaration.Variables.Count == 1
                                && (variableDeclaration.Parent is FieldDeclarationSyntax fieldDeclaration)
                                && fieldDeclaration.Modifiers.Contains(SyntaxKind.ConstKeyword))
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    UseReadOnlyFieldInsteadOfConstantRefactoring.Title,
                                    ct => UseReadOnlyFieldInsteadOfConstantRefactoring.RefactorAsync(context.Document, fieldDeclaration, ct),
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                                break;
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0037_CannotConvertNullToTypeBecauseItIsNonNullableValueType:
                    case CompilerDiagnosticIdentifiers.CS0403_CannotConvertNullToTypeParameterBecauseItCouldBeNonNullableValueType:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReplaceNullLiteralExpressionWithDefaultValue, context.Document, root.SyntaxTree))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            CodeFixRegistrator.ReplaceNullWithDefaultValue(context, diagnostic, expression, semanticModel);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0472_ResultOfExpressionIsAlwaysConstantSinceValueIsNeverEqualToNull:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveConditionThatIsAlwaysEqualToTrueOrFalse, context.Document, root.SyntaxTree))
                                break;

                            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(expression, allowedStyles: NullCheckStyles.ComparisonToNull);

                            if (!nullCheck.Success)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Remove condition",
                                ct =>
                                {
                                    ct.ThrowIfCancellationRequested();

                                    SyntaxNode newRoot = RemoveCondition(root, expression, nullCheck.Style == NullCheckStyles.NotEqualsToNull);

                                    ct.ThrowIfCancellationRequested();

                                    return Task.FromResult(context.Document.WithSyntaxRoot(newRoot));
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0201_OnlyAssignmentCallIncrementDecrementAndNewObjectExpressionsCanBeUsedAsStatement:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveParentheses, context.Document, root.SyntaxTree)
                                && expression is ParenthesizedExpressionSyntax parenthesizedExpression
                                && parenthesizedExpression?.IsMissing == false)
                            {
                                CodeAction codeAction = CodeActionFactory.RemoveParentheses(context.Document, parenthesizedExpression, equivalenceKey: GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                                break;
                            }

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            if (expression.IsParentKind(SyntaxKind.ArrowExpressionClause))
                            {
                                if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeMemberTypeAccordingToReturnExpression, context.Document, root.SyntaxTree))
                                    break;

                                ChangeMemberTypeRefactoring.ComputeCodeFix(context, diagnostic, expression, semanticModel);
                            }
                            else if (expression.Parent is ExpressionStatementSyntax expressionStatement)
                            {
                                if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddArgumentList, context.Document, root.SyntaxTree)
                                    && expression.IsKind(
                                        SyntaxKind.IdentifierName,
                                        SyntaxKind.SimpleMemberAccessExpression))
                                {
                                    SyntaxNode invocationExpression = InvocationExpression(expression);

                                    if (semanticModel.GetSpeculativeMethodSymbol(expression.SpanStart, invocationExpression) != null)
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Add argument list",
                                            ct => context.Document.ReplaceNodeAsync(expression, invocationExpression, ct),
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.AddArgumentList));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }

                                if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReplaceComparisonWithAssignment, context.Document, root.SyntaxTree)
                                    && expression.IsKind(SyntaxKind.EqualsExpression))
                                {
                                    BinaryExpressionInfo info = SyntaxInfo.BinaryExpressionInfo(expression);

                                    if (!info.Success)
                                        break;

                                    ITypeSymbol leftTypeSymbol = semanticModel.GetTypeSymbol(info.Left, context.CancellationToken);

                                    if (leftTypeSymbol?.IsErrorType() != false)
                                        break;

                                    if (!semanticModel.IsImplicitConversion(info.Right, leftTypeSymbol))
                                        break;

                                    CodeAction codeAction = CodeAction.Create(
                                        "Replace comparison with assignment",
                                        ct =>
                                        {
                                            AssignmentExpressionSyntax simpleAssignment = SimpleAssignmentExpression(info.Left, info.Right).WithTriviaFrom(expression);
                                            return context.Document.ReplaceNodeAsync(expression, simpleAssignment, ct);
                                        },
                                        GetEquivalenceKey(diagnostic, CodeFixIdentifiers.ReplaceComparisonWithAssignment));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }

                                if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReplaceConditionalExpressionWithIfElse, context.Document, root.SyntaxTree)
                                    && (expression is ConditionalExpressionSyntax conditionalExpression)
                                    && conditionalExpression.Condition != null)
                                {
                                    ExpressionSyntax whenTrue = conditionalExpression.WhenTrue;
                                    ExpressionSyntax whenFalse = conditionalExpression.WhenFalse;

                                    if (whenTrue != null
                                        && whenFalse != null
                                        && semanticModel.GetTypeSymbol(whenTrue, context.CancellationToken)?.SpecialType == SpecialType.System_Void
                                        && semanticModel.GetTypeSymbol(whenFalse, context.CancellationToken)?.SpecialType == SpecialType.System_Void)
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Replace ?: with if-else",
                                            ct =>
                                            {
                                                IfStatementSyntax newNode = IfStatement(
                                                    conditionalExpression.Condition.WalkDownParentheses(),
                                                    Block(ExpressionStatement(whenTrue)),
                                                    ElseClause(Block(ExpressionStatement(whenFalse))));

                                                newNode = newNode
                                                    .WithTriviaFrom(expressionStatement)
                                                    .WithFormatterAnnotation();

                                                return context.Document.ReplaceNodeAsync(expressionStatement, newNode, ct);
                                            },
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.ReplaceConditionalExpressionWithIfElse));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }

                                if (semanticModel.GetSymbol(expression, context.CancellationToken)?.IsErrorType() != false)
                                    break;

                                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                                if (typeSymbol?.IsErrorType() != false)
                                    break;

                                if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.IntroduceLocalVariable, context.Document, root.SyntaxTree)
                                    && !expressionStatement.IsEmbedded())
                                {
                                    bool addAwait = typeSymbol.OriginalDefinition.EqualsOrInheritsFromTaskOfT()
                                        && semanticModel.GetEnclosingSymbol(expressionStatement.SpanStart, context.CancellationToken).IsAsyncMethod();

                                    CodeAction codeAction = CodeAction.Create(
                                        IntroduceLocalVariableRefactoring.GetTitle(expression),
                                        ct => IntroduceLocalVariableRefactoring.RefactorAsync(context.Document, expressionStatement, typeSymbol, addAwait, semanticModel, ct),
                                        GetEquivalenceKey(diagnostic, CodeFixIdentifiers.IntroduceLocalVariable));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }

                                if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.IntroduceField, context.Document, root.SyntaxTree))
                                {
                                    CodeAction codeAction = CodeAction.Create(
                                        $"Introduce field for '{expression}'",
                                        ct => IntroduceFieldRefactoring.RefactorAsync(context.Document, expressionStatement, typeSymbol, semanticModel, ct),
                                        GetEquivalenceKey(diagnostic, CodeFixIdentifiers.IntroduceField));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0029_CannotImplicitlyConvertType:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReplaceYieldReturnWithForEach, context.Document, root.SyntaxTree)
                                && expression.IsParentKind(SyntaxKind.YieldReturnStatement))
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);
                                ReplaceYieldReturnWithForEachRefactoring.ComputeCodeFix(context, diagnostic, expression, semanticModel);
                                break;
                            }

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeMemberTypeAccordingToReturnExpression, context.Document, root.SyntaxTree)
                                && expression.IsParentKind(SyntaxKind.ReturnStatement, SyntaxKind.YieldReturnStatement, SyntaxKind.ArrowExpressionClause))
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ChangeMemberTypeRefactoring.ComputeCodeFix(context, diagnostic, expression, semanticModel);
                                break;
                            }

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeTypeAccordingToInitializer, context.Document, root.SyntaxTree))
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                CodeFixRegistrationResult result = ChangeTypeAccordingToInitializerRefactoring.ComputeCodeFix(context, diagnostic, expression, semanticModel);

                                if (!result.Success)
                                    RemoveAssignmentOfVoidExpression(context, diagnostic, expression, semanticModel);

                                break;
                            }

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReplaceStringLiteralWithCharacterLiteral, context.Document, root.SyntaxTree)
                                && expression?.Kind() == SyntaxKind.StringLiteralExpression)
                            {
                                var literalExpression = (LiteralExpressionSyntax)expression;

                                if (literalExpression.Token.ValueText.Length == 1)
                                {
                                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                    if (semanticModel.GetTypeInfo(expression, context.CancellationToken).ConvertedType?.SpecialType == SpecialType.System_Char)
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Replace string literal with character literal",
                                            ct => ReplaceStringLiteralWithCharacterLiteralRefactoring.RefactorAsync(context.Document, literalExpression, ct),
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.ReplaceStringLiteralWithCharacterLiteral));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                            }

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.UseYieldReturnInsteadOfReturn, context.Document, root.SyntaxTree)
                                && expression.IsParentKind(SyntaxKind.ReturnStatement))
                            {
                                var returnStatement = (ReturnStatementSyntax)expression.Parent;

                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ISymbol containingSymbol = semanticModel.GetEnclosingSymbol(returnStatement.SpanStart, context.CancellationToken);

                                if (containingSymbol?.Kind == SymbolKind.Method
                                    && ((IMethodSymbol)containingSymbol).ReturnType.OriginalDefinition.IsIEnumerableOrIEnumerableOfT())
                                {
                                    CodeAction codeAction = CodeAction.Create(
                                        "Use yield return instead of return",
                                        ct => UseYieldReturnInsteadOfReturnRefactoring.RefactorAsync(context.Document, returnStatement, SyntaxKind.YieldReturnStatement, semanticModel, ct),
                                        GetEquivalenceKey(diagnostic, CodeFixIdentifiers.UseYieldReturnInsteadOfReturn));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0131_LeftHandSideOfAssignmentMustBeVariablePropertyOrIndexer:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveConstModifier, context.Document, root.SyntaxTree))
                                return;

                            if (!expression.IsKind(SyntaxKind.IdentifierName))
                                return;

                            if (!expression.IsParentKind(SyntaxKind.SimpleAssignmentExpression))
                                return;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(expression, context.CancellationToken);

                            if (symbolInfo.CandidateReason != CandidateReason.NotAVariable)
                                return;

                            if (symbolInfo.CandidateSymbols.SingleOrDefault(shouldThrow: false) is not ILocalSymbol localSymbol)
                                return;

                            if (!localSymbol.IsConst)
                                return;

                            SyntaxNode node = localSymbol.GetSyntaxOrDefault(context.CancellationToken);

                            if (!node.IsKind(SyntaxKind.VariableDeclarator))
                                return;

                            node = node.Parent;

                            if (!node.IsKind(SyntaxKind.VariableDeclaration))
                                return;

                            node = node.Parent;

                            if (node is not LocalDeclarationStatementSyntax localDeclaration)
                                return;

                            SyntaxToken constModifier = localDeclaration.Modifiers.Find(SyntaxKind.ConstKeyword);

                            if (!constModifier.IsKind(SyntaxKind.ConstKeyword))
                                return;

                            ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, localDeclaration, constModifier);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0191_ReadOnlyFieldCannotBeAssignedTo:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.MakeFieldWritable, context.Document, root.SyntaxTree))
                                break;

                            SimpleAssignmentExpressionInfo simpleAssignment = SyntaxInfo.SimpleAssignmentExpressionInfo(expression.Parent);

                            if (!simpleAssignment.Success)
                                return;

                            if (simpleAssignment.Left != expression)
                                return;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(expression, context.CancellationToken);

                            if (symbolInfo.CandidateReason != CandidateReason.NotAVariable)
                                return;

                            if (symbolInfo.CandidateSymbols.SingleOrDefault(shouldThrow: false) is not IFieldSymbol fieldSymbol)
                                return;

                            if (fieldSymbol.DeclaredAccessibility != Accessibility.Private)
                                return;

                            if (fieldSymbol.GetSyntax().Parent.Parent is not FieldDeclarationSyntax fieldDeclaration)
                                return;

                            TypeDeclarationSyntax containingTypeDeclaration = fieldDeclaration.FirstAncestor<TypeDeclarationSyntax>();

                            if (!expression.Ancestors().Any(f => f == containingTypeDeclaration))
                                return;

                            ModifiersCodeFixRegistrator.RemoveModifier(
                                context,
                                diagnostic,
                                fieldDeclaration,
                                SyntaxKind.ReadOnlyKeyword,
                                title: $"Make '{fieldSymbol.Name}' writable");

                            break;
                        }
                }
            }
        }

        private static TNode RemoveCondition<TNode>(
            TNode node,
            ExpressionSyntax expression,
            bool isTrue) where TNode : SyntaxNode
        {
            expression = expression.WalkUpParentheses();

            SyntaxNode parent = expression.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.IfStatement:
                    {
                        var ifStatement = (IfStatementSyntax)parent;

                        if (ifStatement.Condition == expression)
                        {
                            if (isTrue)
                            {
                                return RemoveOrReplaceNode(node, ifStatement, ifStatement.Statement);
                            }
                            else
                            {
                                return RemoveOrReplaceNode(node, ifStatement, ifStatement.Else?.Statement);
                            }
                        }

                        break;
                    }
                case SyntaxKind.DoStatement:
                    {
                        var doStatement = (DoStatementSyntax)parent;

                        if (doStatement.Condition == expression
                            && !isTrue)
                        {
                            return RemoveOrReplaceNode(node, doStatement, doStatement.Statement);
                        }

                        break;
                    }
                case SyntaxKind.WhileStatement:
                    {
                        var whileStatement = (WhileStatementSyntax)parent;

                        if (whileStatement.Condition == expression
                            && !isTrue)
                        {
                            return node.RemoveStatement(whileStatement);
                        }

                        break;
                    }
                case SyntaxKind.ForStatement:
                    {
                        var forStatement = (ForStatementSyntax)parent;

                        if (forStatement.Condition == expression)
                        {
                            if (isTrue)
                            {
                                return node.RemoveNode(expression);
                            }
                            else
                            {
                                return node.RemoveStatement(forStatement);
                            }
                        }

                        break;
                    }
                case SyntaxKind.ConditionalExpression:
                    {
                        var conditionalExpression = (ConditionalExpressionSyntax)parent;

                        if (conditionalExpression.Condition == expression)
                        {
                            if (isTrue)
                            {
                                return RemoveOrReplaceNode(node, conditionalExpression, conditionalExpression.WhenTrue);
                            }
                            else
                            {
                                return RemoveOrReplaceNode(node, conditionalExpression, conditionalExpression.WhenFalse);
                            }
                        }

                        break;
                    }
                case SyntaxKind.LogicalAndExpression:
                    {
                        var logicalAnd = (BinaryExpressionSyntax)parent;

                        ExpressionSyntax left = logicalAnd.Left;
                        ExpressionSyntax right = logicalAnd.Right;

                        ExpressionSyntax other = (left == expression) ? right : left;

                        if (isTrue)
                        {
                            return RemoveOrReplaceNode(node, logicalAnd, other);
                        }
                        else
                        {
                            return RemoveCondition(node, logicalAnd, isTrue);
                        }
                    }
                case SyntaxKind.LogicalOrExpression:
                    {
                        var logicalOr = (BinaryExpressionSyntax)parent;

                        ExpressionSyntax left = logicalOr.Left;
                        ExpressionSyntax right = logicalOr.Right;

                        ExpressionSyntax other = (left == expression) ? right : left;

                        if (isTrue)
                        {
                            return RemoveCondition(node, logicalOr, isTrue);
                        }
                        else
                        {
                            return RemoveOrReplaceNode(node, logicalOr, other);
                        }
                    }
            }

            return node.ReplaceNode(expression, BooleanLiteralExpression(isTrue));

            static TRoot RemoveOrReplaceNode<TRoot>(
                TRoot root,
                SyntaxNode nodeToRemove,
                SyntaxNode newNode) where TRoot : SyntaxNode
            {
                if (newNode == null)
                {
                    if (nodeToRemove == null)
                        return root;

                    if (nodeToRemove is StatementSyntax statement)
                        root.RemoveStatement(statement);

                    return root.RemoveNode(nodeToRemove);
                }

                if (newNode is BlockSyntax block)
                    return root.ReplaceNode(nodeToRemove, block.Statements);

                return root.ReplaceNode(nodeToRemove, newNode);
            }
        }

        private static void RemoveAssignmentOfVoidExpression(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            SemanticModel semanticModel)
        {
            if (expression.Parent is not AssignmentExpressionSyntax assignmentExpression)
                return;

            if (expression != assignmentExpression.Right)
                return;

            if (assignmentExpression.Parent is not ExpressionStatementSyntax expressionStatement)
                return;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if (typeSymbol?.SpecialType != SpecialType.System_Void)
                return;

            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                "Remove assignment",
                ct =>
                {
                    ExpressionStatementSyntax newNode = expressionStatement
                        .WithExpression(expression)
                        .PrependToLeadingTrivia(expressionStatement.GetLeadingTrivia())
                        .WithFormatterAnnotation();

                    return document.ReplaceNodeAsync(expressionStatement, newNode, ct);
                },
                EquivalenceKey.Create(diagnostic, "RemoveAssignment"));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
