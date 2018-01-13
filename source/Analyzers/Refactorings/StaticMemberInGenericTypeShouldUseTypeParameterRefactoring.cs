// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class StaticMemberInGenericTypeShouldUseTypeParameterRefactoring
    {
        public static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var namedType = (INamedTypeSymbol)context.Symbol;

            if (namedType.IsTypeKind(TypeKind.Class, TypeKind.Struct)
                && namedType.Arity > 0
                && !namedType.IsStatic
                && !namedType.IsImplicitClass
                && !namedType.IsImplicitlyDeclared)
            {
                foreach (ISymbol member in namedType.GetMembers())
                {
                    if (!member.IsImplicitlyDeclared
                        && member.IsStatic
                        && member.IsDeclaredAccessibility(Accessibility.Public, Accessibility.Internal, Accessibility.ProtectedOrInternal))
                    {
                        switch (member.Kind)
                        {
                            case SymbolKind.Event:
                                {
                                    var eventSymbol = (IEventSymbol)member;

                                    if (!ContainsAnyTypeParameter(namedType.TypeParameters, eventSymbol.Type))
                                        ReportDiagnostic(context, eventSymbol);

                                    break;
                                }
                            case SymbolKind.Field:
                                {
                                    var fieldSymbol = (IFieldSymbol)member;

                                    if (!ContainsAnyTypeParameter(namedType.TypeParameters, fieldSymbol.Type))
                                        ReportDiagnostic(context, fieldSymbol);

                                    break;
                                }
                            case SymbolKind.Method:
                                {
                                    var methodsymbol = (IMethodSymbol)member;

                                    if (methodsymbol.MethodKind == MethodKind.Ordinary)
                                    {
                                        ImmutableArray<ITypeParameterSymbol> typeParameters = namedType.TypeParameters;

                                        if (!ContainsAnyTypeParameter(typeParameters, methodsymbol.ReturnType)
                                            && !methodsymbol.Parameters.Any(parameter => ContainsAnyTypeParameter(typeParameters, parameter.Type)))
                                        {
                                            ReportDiagnostic(context, methodsymbol);
                                        }
                                    }

                                    break;
                                }
                            case SymbolKind.Property:
                                {
                                    var propertySymbol = (IPropertySymbol)member;

                                    if (!propertySymbol.IsIndexer
                                        && !ContainsAnyTypeParameter(namedType.TypeParameters, propertySymbol.Type))
                                    {
                                        ReportDiagnostic(context, propertySymbol);
                                    }

                                    break;
                                }
                        }
                    }
                }
            }
        }

        private static bool ContainsAnyTypeParameter(
            ImmutableArray<ITypeParameterSymbol> typeParameters,
            ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.Kind)
            {
                case SymbolKind.TypeParameter:
                    return typeParameters.Any(typeParameter => typeParameter.Equals(typeSymbol));
                case SymbolKind.ArrayType:
                    return ContainsAnyTypeParameter(typeParameters, ((IArrayTypeSymbol)typeSymbol).ElementType);
                case SymbolKind.NamedType:
                    return ContainsAnyTypeParameter(typeParameters, (INamedTypeSymbol)typeSymbol);
            }

            Debug.Assert(typeSymbol.Kind == SymbolKind.ErrorType, typeSymbol.Kind.ToString());

            return true;
        }

        private static bool ContainsAnyTypeParameter(
            ImmutableArray<ITypeParameterSymbol> typeParameters,
            INamedTypeSymbol namedTypeSymbol)
        {
            ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

            if (typeArguments.Any())
            {
                if (typeArguments.Length == 1
                    && typeArguments[0].IsTypeParameter())
                {
                    var typeArgument = (ITypeParameterSymbol)typeArguments[0];

                    return typeParameters.Any(f => f.Equals(typeArgument));
                }

                var stack = new Stack<ITypeSymbol>(typeArguments);

                while (stack.Count > 0)
                {
                    ITypeSymbol type = stack.Pop();

                    SymbolKind kind = type.Kind;

                    if (kind == SymbolKind.TypeParameter)
                    {
                        if (typeParameters.Any(f => f.Equals(type)))
                            return true;
                    }
                    else if (kind == SymbolKind.NamedType)
                    {
                        typeArguments = ((INamedTypeSymbol)type).TypeArguments;

                        for (int i = 0; i < typeArguments.Length; i++)
                            stack.Push(typeArguments[i]);
                    }
                }
            }

            return false;
        }

        private static void ReportDiagnostic(SymbolAnalysisContext context, ISymbol member)
        {
            SyntaxNode node = member.GetSyntaxOrDefault(context.CancellationToken);

            Debug.Assert(node != null, member.ToString());

            if (node != null)
            {
                SyntaxToken identifier = GetIdentifier(node);

                Debug.Assert(!identifier.IsKind(SyntaxKind.None), node.ToString());

                if (!identifier.IsKind(SyntaxKind.None))
                {
                    context.ReportDiagnostic(
                       DiagnosticDescriptors.StaticMemberInGenericTypeShouldUseTypeParameter,
                       identifier);
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

            Debug.Fail(node.Kind().ToString());
            return default(SyntaxToken);
        }
    }
}
