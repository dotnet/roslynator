// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ChangeMemberTypeRefactoring
    {
        public static void ComputeCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            ExpressionSyntax expression,
            SemanticModel semanticModel)
        {
            TypeInfo typeInfo = semanticModel.GetTypeInfo(expression, context.CancellationToken);

            ITypeSymbol expressionTypeSymbol = typeInfo.Type;

            if (expressionTypeSymbol == null)
                return;

            if (!expressionTypeSymbol.SupportsExplicitDeclaration())
                return;

            (ISymbol symbol, ITypeSymbol typeSymbol) = GetContainingSymbolAndType(expression, semanticModel, context.CancellationToken);

            SyntaxDebug.Assert(symbol != null, expression);

            if (symbol == null)
                return;

            if (symbol.IsOverride)
                return;

            if (symbol.ImplementsInterfaceMember())
                return;

            SyntaxNode node = symbol.GetSyntax(context.CancellationToken);

            if (node.IsKind(SyntaxKind.VariableDeclarator))
                node = node.Parent.Parent;

            TypeSyntax type = CSharpUtility.GetTypeOrReturnType(node);

            if (type == null)
                return;

            ITypeSymbol newTypeSymbol = expressionTypeSymbol;

            string additionalKey = null;

            var isAsyncMethod = false;
            var insertAwait = false;
            var isYield = false;

            if (symbol.IsAsyncMethod())
            {
                isAsyncMethod = true;

                INamedTypeSymbol taskOfT = semanticModel.GetTypeByMetadataName("System.Threading.Tasks.Task`1");

                if (taskOfT == null)
                    return;

                if (expression.Kind() == SyntaxKind.AwaitExpression)
                {
                    newTypeSymbol = taskOfT.Construct(expressionTypeSymbol);
                }
                else if (SymbolEqualityComparer.Default.Equals(expressionTypeSymbol, taskOfT))
                {
                    insertAwait = true;
                    additionalKey = "InsertAwait";
                }
                else if (expressionTypeSymbol.HasMetadataName(MetadataNames.System_Threading_Tasks_Task))
                {
                    return;
                }
            }
            else if (expression.IsParentKind(SyntaxKind.YieldReturnStatement))
            {
                isYield = true;

                newTypeSymbol = semanticModel
                    .Compilation
                    .GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T)
                    .Construct(expressionTypeSymbol);
            }

            if (!isYield
                && !isAsyncMethod
                && !typeSymbol.OriginalDefinition.IsIEnumerableOfT()
                && newTypeSymbol.OriginalDefinition.HasMetadataName(MetadataNames.System_Linq_IOrderedEnumerable_T))
            {
                INamedTypeSymbol constructedEnumerableSymbol = semanticModel
                    .Compilation
                    .GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T)
                    .Construct(((INamedTypeSymbol)newTypeSymbol).TypeArguments.ToArray());

                RegisterCodeFix(context, diagnostic, node, type, expression, constructedEnumerableSymbol, semanticModel, insertAwait: false);
                additionalKey = "IOrderedEnumerable<T>";
            }

            RegisterCodeFix(context, diagnostic, node, type, expression, newTypeSymbol, semanticModel, insertAwait: insertAwait, additionalKey: additionalKey);
        }

        private static void RegisterCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode node,
            TypeSyntax type,
            ExpressionSyntax expression,
            ITypeSymbol newTypeSymbol,
            SemanticModel semanticModel,
            bool insertAwait = false,
            string additionalKey = null)
        {
            Document document = context.Document;

            string displayName = SymbolDisplay.ToMinimalDisplayString(newTypeSymbol, semanticModel, type.SpanStart, SymbolDisplayFormats.DisplayName);

            string title = $"Change {GetText(node)} type to '{displayName}'";

            if (insertAwait)
                title += " and insert 'await'";

            CodeAction codeAction = CodeAction.Create(
                title,
                ct =>
                {
                    SyntaxNode newNode = null;

                    TypeSyntax newType = newTypeSymbol
                        .ToTypeSyntax()
                        .WithSimplifierAnnotation()
                        .WithTriviaFrom(type);

                    if (insertAwait)
                    {
                        var nodes = new SyntaxNode[] { type, expression };

                        newNode = node.ReplaceNodes(
                            nodes,
                            (f, _) =>
                            {
                                if (f == type)
                                {
                                    return newType;
                                }
                                else
                                {
                                    return AwaitExpression(
                                        Token(expression.GetLeadingTrivia(), SyntaxKind.AwaitKeyword, TriviaList(Space)),
                                        expression.WithoutLeadingTrivia());
                                }
                            });

                        return document.ReplaceNodeAsync(node, newNode, ct);
                    }
                    else
                    {
                        return document.ReplaceNodeAsync(type, newType, ct);
                    }
                },
                EquivalenceKey.Create(diagnostic, CodeFixIdentifiers.ChangeMemberTypeAccordingToReturnExpression, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static (ISymbol symbol, ITypeSymbol typeSymbol) GetContainingSymbolAndType(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            switch (semanticModel.GetEnclosingSymbol(expression.SpanStart, cancellationToken))
            {
                case IMethodSymbol methodSymbol:
                    {
                        MethodKind methodKind = methodSymbol.MethodKind;

                        if (methodKind == MethodKind.PropertyGet)
                        {
                            var propertySymbol = (IPropertySymbol)methodSymbol.AssociatedSymbol;

                            return (propertySymbol, propertySymbol.Type);
                        }

                        if (methodKind == MethodKind.Ordinary
                            && methodSymbol.PartialImplementationPart != null)
                        {
                            methodSymbol = methodSymbol.PartialImplementationPart;
                        }

                        return (methodSymbol, methodSymbol.ReturnType);
                    }
                case IFieldSymbol fieldSymbol:
                    {
                        return (fieldSymbol, fieldSymbol.Type);
                    }
            }

            SyntaxDebug.Fail(expression);

            return default((ISymbol, ITypeSymbol));
        }

        private static string GetText(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.LocalFunctionStatement:
                    return "return";
                case SyntaxKind.PropertyDeclaration:
                    return "property";
                case SyntaxKind.IndexerDeclaration:
                    return "indexer";
                case SyntaxKind.FieldDeclaration:
                    return "field";
                default:
                    return null;
            }
        }
    }
}
