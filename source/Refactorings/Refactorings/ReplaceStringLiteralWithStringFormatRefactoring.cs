// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceStringLiteralWithStringFormatRefactoring
    {
        private static readonly Regex _compositeFormatRegex = new Regex(@"{(\d+)(,-?\d+)?(:\w+)?}", RegexOptions.Compiled);

        public static async Task ComputeRefactoringsAsync(RefactoringContext context, LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression.IsKind(SyntaxKind.StringLiteralExpression))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                var parentInvocationExpression = literalExpression.FirstAncestor<InvocationExpressionSyntax>();

                if (parentInvocationExpression != null)
                {
                    MethodInfo info = semanticModel.GetMethodInfo(parentInvocationExpression, context.CancellationToken);

                    if (info.IsValid
                        && info.HasName("Format")
                        && info.IsContainingType(SpecialType.System_String))
                    {
                        // If the format string is already within a string.Format call then stop
                        return;
                    }
                }

                string text = literalExpression.Token.ValueText;

                IEnumerable<int> formatItems = GetFormatItems(text);

                if (formatItems.Any())
                {
                    var numberOfArgs = formatItems.First() + 1;

                    context.RegisterRefactoring(
                        "Replace string with string.Format invocation",
                        cancellationToken =>
                        {
                            return ReplaceWithStringFormatAsync(
                                context.Document,
                                literalExpression,
                                numberOfArgs,
                                cancellationToken);
                        });
                }
            }
        }

        private static IEnumerable<int> GetFormatItems(string text)
        {
            return _compositeFormatRegex.Matches(text).OfType<Match>()
                .Select(m => int.Parse(m.Groups[1].Value))
                .OrderByDescending(m => m)
                .Distinct();
        }

        public static Task<Document> ReplaceWithStringFormatAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            int numberOfArgs,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stringIdentifier = IdentifierName("string");
            var format = IdentifierName("Format");
            var memberAccess = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, stringIdentifier, format);
            var argument = Argument(literalExpression);
            var argumentSyntax = new List<ArgumentSyntax>();

            argumentSyntax.Add(argument);

            for (int i = 0; i < numberOfArgs; i++)
            {
                argumentSyntax.Add(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal($"ARG{i}"))));
            }

            var argumentList = SeparatedList(argumentSyntax);

            var stringFormatCall =
                ExpressionStatement(
                    InvocationExpression(
                        memberAccess,
                        ArgumentList(argumentList)));

            return document.ReplaceNodeAsync(literalExpression, stringFormatCall.Expression, cancellationToken);
        }
    }
}