// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddAllPropertiesToInitializerRefactoring
    {
        public static bool IsApplicableSpan(InitializerExpressionSyntax initializer, TextSpan span)
        {
            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;
            if (!expressions.Any())
            {
                return true;
            }

            if (span.IsEmpty)
            {
                if (expressions.Count == expressions.SeparatorCount
                    && TextSpan.FromBounds(expressions.GetSeparator(expressions.Count - 1).Span.End, initializer.CloseBraceToken.SpanStart).Contains(span))
                {
                    return true;
                }

                TextSpan span2 = TextSpan.FromBounds(expressions.Last().Span.End, initializer.CloseBraceToken.SpanStart);

                if (span2.Length > 0)
                {
                    span2 = new TextSpan(span2.Start + 1, span2.Length - 1);

                    if (span2.Contains(span))
                        return true;
                }
            }

            return false;
        }

        public static void ComputeRefactorings(
            RefactoringContext context,
            InitializerExpressionSyntax initializer,
            SemanticModel semanticModel)
        {
            Debug.Assert(initializer.IsKind(SyntaxKind.ObjectInitializerExpression, SyntaxKind.WithInitializerExpression), initializer.Kind().ToString());

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(initializer.Parent, context.CancellationToken);

            if (typeSymbol?.IsErrorType() != false)
                return;

            if (typeSymbol.IsAnonymousType)
                return;

            int position = initializer.OpenBraceToken.Span.End;

            ImmutableArray<ISymbol> symbols = semanticModel.LookupSymbols(position, typeSymbol);

            if (!symbols.Any())
                return;

            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            if (expressions.Any())
            {
                var initializedPropertyNames = new HashSet<string>();

                foreach (ExpressionSyntax expression in expressions)
                {
                    Debug.Assert(expression.IsKind(SyntaxKind.SimpleAssignmentExpression), expression.Kind().ToString());

                    if (expression.IsKind(SyntaxKind.SimpleAssignmentExpression))
                    {
                        SimpleAssignmentExpressionInfo assignmentInfo = SyntaxInfo.SimpleAssignmentExpressionInfo((AssignmentExpressionSyntax)expression);

                        if (assignmentInfo.Success)
                        {
                            ExpressionSyntax left = assignmentInfo.Left;

                            Debug.Assert(left.IsKind(SyntaxKind.IdentifierName), left.Kind().ToString());

                            if (left is IdentifierNameSyntax identifierName)
                            {
                                initializedPropertyNames.Add(identifierName.Identifier.ValueText);
                            }
                        }
                    }
                }

                Dictionary<string, IPropertySymbol> namesToProperties = null;

                foreach (ISymbol symbol in symbols)
                {
                    if (initializedPropertyNames.Contains(symbol.Name))
                        continue;

                    IPropertySymbol propertySymbol = GetInitializableProperty(symbol, position, semanticModel);

                    if (propertySymbol == null)
                        continue;

                    if (namesToProperties != null)
                    {
                        if (namesToProperties.ContainsKey(propertySymbol.Name))
                            continue;
                    }
                    else
                    {
                        namesToProperties = new Dictionary<string, IPropertySymbol>();
                    }

                    namesToProperties.Add(propertySymbol.Name, propertySymbol);
                }

                if (namesToProperties != null)
                {
                    Document document = context.Document;

                    context.RegisterRefactoring(
                        "Initialize all properties",
                        ct =>
                        {
                            IEnumerable<IPropertySymbol> propertySymbols = namesToProperties.Select(f => f.Value);
                            return RefactorAsync(document, initializer, propertySymbols, initializeToDefault: false, ct);
                        },
                        RefactoringDescriptors.AddAllPropertiesToInitializer);
                }
            }
            else if (HasAccessiblePropertySetter())
            {
                Document document = context.Document;

                context.RegisterRefactoring(
                    "Initialize all properties",
                    ct =>
                    {
                        IEnumerable<IPropertySymbol> propertySymbols = GetInitializableProperties(initializer, symbols, semanticModel);
                        return RefactorAsync(document, initializer, propertySymbols, initializeToDefault: false, ct);
                    },
                    RefactoringDescriptors.AddAllPropertiesToInitializer);
            }

            bool HasAccessiblePropertySetter()
            {
                foreach (ISymbol symbol in symbols)
                {
                    if (GetInitializableProperty(symbol, position, semanticModel) != null)
                        return true;
                }

                return false;
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            IEnumerable<IPropertySymbol> propertySymbols,
            bool initializeToDefault,
            CancellationToken cancellationToken)
        {
            IdentifierNameSyntax missingIdentifierName = (initializeToDefault)
                ? null
                : IdentifierName(MissingToken(TriviaList(), SyntaxKind.IdentifierToken, TriviaList(Space)));

            IEnumerable<AssignmentExpressionSyntax> newExpressions = propertySymbols
                .OrderBy(f => f.Name)
                .Select(propertySymbol =>
                {
                    return SimpleAssignmentExpression(
                        IdentifierName(propertySymbol.Name),
                        (initializeToDefault)
                            ? propertySymbol.Type.GetDefaultValueSyntax(document.GetDefaultSyntaxOptions())
                            : missingIdentifierName);
                });

            InitializerExpressionSyntax newInitializer = initializer
                .WithExpressions(initializer.Expressions.AddRange(newExpressions));

            if (initializer.IsMultiLine())
            {
                newInitializer = newInitializer.ReplaceNode(
                    newInitializer.Expressions.Last(),
                    newInitializer.Expressions.Last().AppendToTrailingTrivia(NewLine()));
            }

            newInitializer = newInitializer.WithFormatterAnnotation();

            return document.ReplaceNodeAsync(initializer, newInitializer, cancellationToken);
        }

        private static IPropertySymbol GetInitializableProperty(ISymbol symbol, int position, SemanticModel semanticModel)
        {
            if (!symbol.IsStatic
                && symbol.IsKind(SymbolKind.Property))
            {
                var propertySymbol = (IPropertySymbol)symbol;

                if (!propertySymbol.IsIndexer)
                {
                    IMethodSymbol setter = propertySymbol.SetMethod;

                    if (setter != null
                        && semanticModel.IsAccessible(position, setter))
                    {
                        return propertySymbol;
                    }
                }
            }

            return null;
        }

        private static IEnumerable<IPropertySymbol> GetInitializableProperties(
            InitializerExpressionSyntax initializer,
            IEnumerable<ISymbol> symbols,
            SemanticModel semanticModel)
        {
            int position = initializer.OpenBraceToken.Span.End;

            foreach (ISymbol symbol in symbols)
            {
                IPropertySymbol propertySymbol = GetInitializableProperty(symbol, position, semanticModel);

                if (propertySymbol != null)
                    yield return propertySymbol;
            }
        }
    }
}
