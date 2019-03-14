// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Roslynator
{
    internal static class UnusedSymbolUtility
    {
        public static bool CanBeUnusedSymbol(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Namespace:
                    {
                        return false;
                    }
                case SymbolKind.NamedType:
                    {
                        var namedType = (INamedTypeSymbol)symbol;

                        return namedType.TypeKind != TypeKind.Class
                            || !namedType.IsStatic;
                    }
                case SymbolKind.Event:
                    {
                        if (symbol.IsOverride)
                            return false;

                        var eventSymbol = (IEventSymbol)symbol;

                        return eventSymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                            && !eventSymbol.ImplementsInterfaceMember(allInterfaces: true);
                    }
                case SymbolKind.Field:
                    {
                        var fieldSymbol = (IFieldSymbol)symbol;

                        return !fieldSymbol.ImplementsInterfaceMember(allInterfaces: true);
                    }
                case SymbolKind.Property:
                    {
                        if (symbol.IsOverride)
                            return false;

                        var propertySymbol = (IPropertySymbol)symbol;

                        return propertySymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                            && !propertySymbol.ImplementsInterfaceMember(allInterfaces: true);
                    }
                case SymbolKind.Method:
                    {
                        if (symbol.IsOverride)
                            return false;

                        var methodSymbol = (IMethodSymbol)symbol;

                        switch (methodSymbol.MethodKind)
                        {
                            case MethodKind.Ordinary:
                                {
                                    return methodSymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                                        && !SymbolUtility.CanBeEntryPoint(methodSymbol)
                                        && !methodSymbol.ImplementsInterfaceMember(allInterfaces: true);
                                }
                            case MethodKind.Constructor:
                                {
                                    return methodSymbol.Parameters.Any()
                                        || !methodSymbol.ContainingType.IsAbstract;
                                }
                        }

                        return false;
                    }
                default:
                    {
                        Debug.Fail(symbol.Kind.ToString());
                        return false;
                    }
            }
        }

        public static async Task<bool> IsUnusedSymbolAsync(
            ISymbol symbol,
            Solution solution,
            CancellationToken cancellationToken = default)
        {
            ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

            Debug.Assert(syntaxReferences.Any(), $"No syntax references for {symbol.ToDisplayString()}");

            if (!syntaxReferences.Any())
                return false;

            if (IsReferencedInDebuggerDisplayAttribute(symbol))
                return false;

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(symbol, solution, cancellationToken).ConfigureAwait(false);

            foreach (ReferencedSymbol referencedSymbol in referencedSymbols)
            {
                foreach (ReferenceLocation referenceLocation in referencedSymbol.Locations)
                {
                    if (referenceLocation.IsImplicit)
                        continue;

                    if (referenceLocation.IsCandidateLocation)
                        return false;

                    Location location = referenceLocation.Location;

                    if (!location.IsInSource)
                        continue;

                    foreach (SyntaxReference syntaxReference in syntaxReferences)
                    {
                        if (syntaxReference.SyntaxTree != location.SourceTree
                            || !syntaxReference.Span.Contains(location.SourceSpan))
                        {
                            return false;
                        }
                    }
                }
            }

            if (symbol.Kind == SymbolKind.Field)
            {
                INamedTypeSymbol containingType = symbol.ContainingType;

                if (containingType.TypeKind == TypeKind.Enum
                    && containingType.HasAttribute(MetadataNames.System_FlagsAttribute))
                {
                    var fieldSymbol = (IFieldSymbol)symbol;

                    if (fieldSymbol.HasConstantValue)
                    {
                        ulong value = SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, containingType);

                        if (value == 0)
                            return false;
                    }
                }
            }

            return true;
        }

        private static bool IsReferencedInDebuggerDisplayAttribute(ISymbol symbol)
        {
            if (symbol.DeclaredAccessibility == Accessibility.Private
                && CanBeReferencedInDebuggerDisplayAttribute())
            {
                string value = symbol.ContainingType
                    .GetAttribute(MetadataNames.System_Diagnostics_DebuggerDisplayAttribute)?
                    .ConstructorArguments
                    .SingleOrDefault(shouldThrow: false)
                    .Value?
                    .ToString();

                return value != null
                    && IsReferencedInDebuggerDisplayAttribute(value);
            }

            return false;

            bool CanBeReferencedInDebuggerDisplayAttribute()
            {
                switch (symbol.Kind)
                {
                    case SymbolKind.Field:
                        {
                            return true;
                        }
                    case SymbolKind.Method:
                        {
                            return !((IMethodSymbol)symbol).Parameters.Any();
                        }
                    case SymbolKind.Property:
                        {
                            return !((IPropertySymbol)symbol).IsIndexer;
                        }
                }

                return false;
            }

            bool IsReferencedInDebuggerDisplayAttribute(string value)
            {
                int length = value.Length;

                for (int i = 0; i < length; i++)
                {
                    switch (value[i])
                    {
                        case '{':
                            {
                                i++;

                                int startIndex = i;

                                while (i < length)
                                {
                                    char ch = value[i];

                                    if (ch == '}'
                                        || ch == ','
                                        || ch == '(')
                                    {
                                        int nameLength = i - startIndex;

                                        if (nameLength > 0
                                            && string.CompareOrdinal(symbol.Name, 0, value, startIndex, nameLength) == 0)
                                        {
                                            return true;
                                        }

                                        if (ch != '}')
                                        {
                                            i++;

                                            while (i < length
                                                && value[i] != '}')
                                            {
                                                i++;
                                            }
                                        }

                                        break;
                                    }

                                    i++;
                                }

                                break;
                            }
                        case '}':
                            {
                                return false;
                            }
                        case '\\':
                            {
                                i++;
                                break;
                            }
                    }
                }

                return false;
            }
        }
    }
}
