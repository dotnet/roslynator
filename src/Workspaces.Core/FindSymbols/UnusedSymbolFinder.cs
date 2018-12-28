// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using static Roslynator.Logger;

namespace Roslynator.FindSymbols
{
    internal static class UnusedSymbolFinder
    {
        public static async Task<ImmutableArray<UnusedSymbolInfo>> FindUnusedSymbolsAsync(
            Project project,
            Func<ISymbol, bool> predicate = null,
            IImmutableSet<ISymbol> ignoredSymbols = null,
            IImmutableSet<Document> documents = null,
            CancellationToken cancellationToken = default)
        {
            Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            return await FindUnusedSymbolsAsync(
                project,
                compilation,
                predicate,
                ignoredSymbols,
                documents,
                cancellationToken).ConfigureAwait(false);
        }

        internal static async Task<ImmutableArray<UnusedSymbolInfo>> FindUnusedSymbolsAsync(
            Project project,
            Compilation compilation,
            Func<ISymbol, bool> predicate = null,
            IImmutableSet<ISymbol> ignoredSymbols = null,
            IImmutableSet<Document> documents = null,
            CancellationToken cancellationToken = default)
        {
            ImmutableArray<UnusedSymbolInfo>.Builder unusedSymbols = null;

            var namespaceOrTypeSymbols = new Stack<INamespaceOrTypeSymbol>();

            namespaceOrTypeSymbols.Push(compilation.Assembly.GlobalNamespace);

            while (namespaceOrTypeSymbols.Count > 0)
            {
                INamespaceOrTypeSymbol namespaceOrTypeSymbol = namespaceOrTypeSymbols.Pop();

                foreach (ISymbol symbol in namespaceOrTypeSymbol.GetMembers())
                {
                    bool isUnused = false;

                    if (symbol.Kind != SymbolKind.Namespace
                        && !symbol.IsImplicitlyDeclared
                        && !symbol.IsOverride
                        && (predicate == null || predicate(symbol))
                        && IsAnalyzable(symbol)
                        && ignoredSymbols?.Contains(symbol) != true)
                    {
                        isUnused = await IsUnusedSymbolAsync(symbol, project.Solution, documents, cancellationToken).ConfigureAwait(false);

                        if (isUnused)
                        {
                            string id = symbol.GetDocumentationCommentId();

                            WriteLine($"  {GetUnusedSymbolKind(symbol).ToString()} {symbol.ToDisplayString()}", ConsoleColor.Yellow, Verbosity.Normal);
                            WriteLine($"    {symbol.GetSyntax(cancellationToken).SyntaxTree.FilePath}", ConsoleColor.DarkGray, Verbosity.Detailed);
                            WriteLine($"    {id}", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                            var unusedSymbolInfo = new UnusedSymbolInfo(symbol, id, project.Id);

                            (unusedSymbols ?? (unusedSymbols = ImmutableArray.CreateBuilder<UnusedSymbolInfo>())).Add(unusedSymbolInfo);
                        }
                    }

                    if (!isUnused
                        && symbol is INamespaceOrTypeSymbol namespaceOrTypeSymbol2)
                    {
                        namespaceOrTypeSymbols.Push(namespaceOrTypeSymbol2);
                    }
                }
            }

            return unusedSymbols?.ToImmutableArray() ?? ImmutableArray<UnusedSymbolInfo>.Empty;
        }

        private static bool IsAnalyzable(ISymbol symbol)
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
                        var propertySymbol = (IPropertySymbol)symbol;

                        return propertySymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                            && !propertySymbol.ImplementsInterfaceMember(allInterfaces: true);
                    }
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        switch (methodSymbol.MethodKind)
                        {
                            case MethodKind.Ordinary:
                                {
                                    return methodSymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                                        && !CanBeEntryPoint(methodSymbol)
                                        && !methodSymbol.ImplementsInterfaceMember(allInterfaces: true);
                                }
                            case MethodKind.Constructor:
                                {
                                    return true;
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

        private static async Task<bool> IsUnusedSymbolAsync(
            ISymbol symbol,
            Solution solution,
            IImmutableSet<Document> documents,
            CancellationToken cancellationToken)
        {
            ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

            Debug.Assert(syntaxReferences.Any(), $"No syntax references for {symbol.ToDisplayString()}");

            if (!syntaxReferences.Any())
                return false;

            if (IsReferencedInDebuggerDisplayAttribute(symbol))
                return false;

            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(symbol, solution, documents, cancellationToken).ConfigureAwait(false);

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

            return true;
        }

        internal static UnusedSymbolKind GetUnusedSymbolKind(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                    {
                        var namedType = (INamedTypeSymbol)symbol;

                        switch (namedType.TypeKind)
                        {
                            case TypeKind.Class:
                                return UnusedSymbolKind.Class;
                            case TypeKind.Delegate:
                                return UnusedSymbolKind.Delegate;
                            case TypeKind.Enum:
                                return UnusedSymbolKind.Enum;
                            case TypeKind.Interface:
                                return UnusedSymbolKind.Interface;
                            case TypeKind.Struct:
                                return UnusedSymbolKind.Struct;
                        }

                        Debug.Fail(namedType.TypeKind.ToString());
                        return UnusedSymbolKind.None;
                    }
                case SymbolKind.Event:
                    {
                        return UnusedSymbolKind.Event;
                    }
                case SymbolKind.Field:
                    {
                        return UnusedSymbolKind.Field;
                    }
                case SymbolKind.Method:
                    {
                        return UnusedSymbolKind.Method;
                    }
                case SymbolKind.Property:
                    {
                        return UnusedSymbolKind.Property;
                    }
            }

            Debug.Fail(symbol.Kind.ToString());
            return UnusedSymbolKind.None;
        }

        // https://docs.microsoft.com/cs-cz/dotnet/csharp/programming-guide/main-and-command-args/
        private static bool CanBeEntryPoint(IMethodSymbol methodSymbol)
        {
            if (methodSymbol.IsStatic
                && string.Equals(methodSymbol.Name, "Main", StringComparison.Ordinal)
                && methodSymbol.ContainingType?.TypeKind.Is(TypeKind.Class, TypeKind.Struct) == true
                && !methodSymbol.TypeParameters.Any())
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                int length = parameters.Length;

                if (length == 0)
                    return true;

                if (length == 1)
                {
                    IParameterSymbol parameter = parameters[0];

                    ITypeSymbol type = parameter.Type;

                    if (type.Kind == SymbolKind.ArrayType)
                    {
                        var arrayType = (IArrayTypeSymbol)type;

                        if (arrayType.ElementType.SpecialType == SpecialType.System_String)
                            return true;
                    }
                }
            }

            return false;
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
