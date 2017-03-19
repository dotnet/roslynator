// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Diagnostics.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidStaticMemberInGenericTypesRefactoring
    {
        public static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var namedType = (INamedTypeSymbol)context.Symbol;

            if (namedType.IsClass()
                && namedType.Arity > 0
                && !namedType.IsStatic
                && !namedType.IsImplicitClass
                && !namedType.IsImplicitlyDeclared)
            {
                foreach (ISymbol member in namedType.GetMembers())
                {
                    switch (member.Kind)
                    {
                        case SymbolKind.Event:
                        case SymbolKind.Field:
                            {
                                Analyze(context, member);
                                break;
                            }
                        case SymbolKind.Method:
                            {
                                var methodsymbol = (IMethodSymbol)member;

                                if (methodsymbol.MethodKind == MethodKind.Ordinary)
                                    Analyze(context, member);

                                break;
                            }
                        case SymbolKind.Property:
                            {
                                var propertySymbol = (IPropertySymbol)member;

                                if (!propertySymbol.IsIndexer)
                                    Analyze(context, member);

                                break;
                            }
                    }
                }
            }
        }

        private static void Analyze(SymbolAnalysisContext context, ISymbol member)
        {
            if (!member.IsImplicitlyDeclared
                && member.IsStatic
                && member.IsPubliclyVisible())
            {
                SyntaxNode node = member
                    .DeclaringSyntaxReferences
                    .FirstOrDefault()?
                    .GetSyntax(context.CancellationToken);

                Debug.Assert(node != null, member.ToString());

                if (node != null)
                {
                    SyntaxToken identifier = GetIdentifier(node);

                    Debug.Assert(!identifier.IsKind(SyntaxKind.None), "");

                    if (!identifier.IsKind(SyntaxKind.None))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.AvoidStaticMembersInGenericTypes,
                            identifier);
                    }
                }
            }
        }

        private static SyntaxToken GetIdentifier(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.VariableDeclarator:
                    return ((VariableDeclaratorSyntax)node).Identifier;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).Identifier;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).Identifier;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).Identifier;
            }

            Debug.Assert(false, node.Kind().ToString());
            return default(SyntaxToken);
        }
    }
}
