// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal static class SyntaxHelper
    {
        public static string GetCountOrLengthPropertyName(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            bool allowImmutableArray = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            if (typeSymbol?.IsErrorType() == false
                && !typeSymbol.IsConstructedFromIEnumerableOfT())
            {
                if (typeSymbol.BaseType?.SpecialType == SpecialType.System_Array)
                    return "Length";

                if (allowImmutableArray
                    && SymbolAnalyzer.IsConstructedFromImmutableArrayOfT(typeSymbol, semanticModel))
                {
                    return "Length";
                }

                ImmutableArray<INamedTypeSymbol> allInterfaces = typeSymbol.AllInterfaces;

                for (int i = 0; i < allInterfaces.Length; i++)
                {
                    if (allInterfaces[i].ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_ICollection_T)
                    {
                        foreach (ISymbol members in typeSymbol.GetMembers("Count"))
                        {
                            if (members.IsProperty()
                                && members.IsPublic())
                            {
                                return "Count";
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static string GetNodeTitle(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.IfStatement:
                    return "if statement";
                case SyntaxKind.ElseClause:
                    return "else clause";
                case SyntaxKind.DoStatement:
                    return "do statement";
                case SyntaxKind.ForEachStatement:
                    return "foreach statement";
                case SyntaxKind.ForStatement:
                    return "for statement";
                case SyntaxKind.UsingStatement:
                    return "using statement";
                case SyntaxKind.WhileStatement:
                    return "while statement";
                case SyntaxKind.LockStatement:
                    return "lock statement";
                case SyntaxKind.FixedStatement:
                    return "fixed statement";
            }

            Debug.Assert(false, node.Kind().ToString());
            return "";
        }
    }
}
