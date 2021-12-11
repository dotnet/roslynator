// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.CSharpTypeFactory;

namespace Roslynator.CSharp.Refactorings.ReplaceEqualsExpression
{
    internal abstract class ReplaceEqualityOperatorRefactoring
    {
        public abstract RefactoringDescriptor GetDescriptor();

        public abstract string MethodName { get; }

        public static async Task ComputeRefactoringsAsync(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(binaryExpression, NullCheckStyles.ComparisonToNull);

            if (!nullCheck.Success)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            if (semanticModel
                .GetTypeInfo(nullCheck.Expression, context.CancellationToken)
                .ConvertedType?
                .SpecialType != SpecialType.System_String)
            {
                return;
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ReplaceEqualityOperatorWithStringIsNullOrEmpty))
                ReplaceEqualityOperatorWithStringIsNullOrEmptyRefactoring.Instance.RegisterRefactoring(context, nullCheck);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ReplaceEqualityOperatorWithStringIsNullOrWhiteSpace))
                ReplaceEqualityOperatorWithStringIsNullOrWhiteSpaceRefactoring.Instance.RegisterRefactoring(context, nullCheck);
        }

        private void RegisterRefactoring(RefactoringContext context, NullCheckExpressionInfo nullCheck)
        {
            string title = (nullCheck.Style == NullCheckStyles.EqualsToNull)
                ? $"Replace '{nullCheck.NullCheckExpression}' with 'string.{MethodName}({nullCheck.Expression})'"
                : $"Replace '{nullCheck.NullCheckExpression}' with '!string.{MethodName}({nullCheck.Expression})'";

            context.RegisterRefactoring(
                title,
                ct => RefactorAsync(context.Document, nullCheck, ct),
                GetDescriptor());
        }

        private Task<Document> RefactorAsync(
            Document document,
            in NullCheckExpressionInfo nullCheck,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax newNode = SimpleMemberInvocationExpression(
                StringType(),
                IdentifierName(MethodName),
                Argument(nullCheck.Expression));

            if (nullCheck.Style == NullCheckStyles.NotEqualsToNull)
                newNode = LogicalNotExpression(newNode);

            newNode = newNode
                .WithTriviaFrom(nullCheck.NullCheckExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(nullCheck.NullCheckExpression, newNode, cancellationToken);
        }
    }
}
