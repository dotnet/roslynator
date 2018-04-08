// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceYieldReturnWithForEachRefactoring
    {
        public static void ComputeCodeFix(
             CodeFixContext context,
             Diagnostic diagnostic,
             ExpressionSyntax expression,
             SemanticModel semanticModel)
        {
            TypeInfo typeInfo = semanticModel.GetTypeInfo(expression, context.CancellationToken);

            ITypeSymbol type = typeInfo.Type;

            if (!(type is INamedTypeSymbol namedTypeSymbol))
                return;

            ITypeSymbol convertedType = typeInfo.ConvertedType;

            if (type == convertedType)
                return;

            if (namedTypeSymbol.ConstructedFrom.SpecialType != SpecialType.System_Collections_Generic_IEnumerable_T)
                return;

            if (!namedTypeSymbol.TypeArguments[0].Equals(convertedType))
                return;

            CodeAction codeAction = CodeAction.Create(
                "Replace yield return with foreach",
                cancellationToken => RefactorAsync(context.Document, expression, semanticModel, cancellationToken),
                EquivalenceKey.Create(diagnostic, CodeFixIdentifiers.ReplaceYieldReturnWithForEach));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            System.Threading.CancellationToken cancellationToken)
        {
            string name = DefaultNames.ForEachVariable;

            name = NameGenerator.Default.EnsureUniqueLocalName(name, semanticModel, expression.SpanStart, cancellationToken: cancellationToken);

            ForEachStatementSyntax forEachStatement = ForEachStatement(
                type: VarType(),
                identifier: Identifier(name),
                expression: expression.TrimTrivia(),
                statement: YieldReturnStatement(IdentifierName(name)));

            SyntaxNode yieldStatement = expression.Parent;

            forEachStatement = forEachStatement.WithTriviaFrom(yieldStatement);

            return document.ReplaceNodeAsync(yieldStatement, forEachStatement, cancellationToken);
        }
    }
}