// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation
{
    internal static class SymbolDefinitionWriterHelpers
    {
        public static ImmutableArray<SymbolDisplayPart> RemoveAttributeSuffix(ISymbol symbol, ImmutableArray<SymbolDisplayPart> parts)
        {
            if (symbol.Name.EndsWith("Attribute", StringComparison.Ordinal)
                && symbol.IsKind(SymbolKind.NamedType)
                && ((INamedTypeSymbol)symbol).InheritsFrom(MetadataNames.System_Attribute))
            {
                return RemoveAttributeSuffix(parts);
            }

            return parts;
        }

        public static ImmutableArray<SymbolDisplayPart> RemoveAttributeSuffix(ImmutableArray<SymbolDisplayPart> parts)
        {
            SymbolDisplayPart part = parts.FirstOrDefault(f => f.Kind == SymbolDisplayPartKind.ClassName);

            Debug.Assert(part.Kind == SymbolDisplayPartKind.ClassName, part.Kind.ToString());

            if (part.Kind == SymbolDisplayPartKind.ClassName)
            {
                const string attributeSuffix = "Attribute";

                string text = part.ToString();

                if (text.EndsWith(attributeSuffix, StringComparison.Ordinal))
                {
                    parts = parts.Replace(part, part.WithText(text.Remove(text.Length - attributeSuffix.Length)));
                }
            }

            return parts;
        }

        public static string GetAccessorName(IMethodSymbol accessorSymbol)
        {
            switch (accessorSymbol.MethodKind)
            {
                case MethodKind.EventAdd:
                    return "add";
                case MethodKind.EventRemove:
                    return "remove";
                case MethodKind.PropertyGet:
                    return "get";
                case MethodKind.PropertySet:
                    return "set";
                default:
                    return null;
            }
        }

        public static bool HasAttributes(ISymbol symbol, SymbolFilterOptions filter)
        {
            if (IsMatch(symbol))
                return true;

            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                    {
                        var typeSymbol = (INamedTypeSymbol)symbol;

                        if (typeSymbol.TypeKind == TypeKind.Delegate
                            && HasAttributes(typeSymbol.DelegateInvokeMethod?.Parameters ?? ImmutableArray<IParameterSymbol>.Empty))
                        {
                            return true;
                        }

                        break;
                    }
                case SymbolKind.Event:
                    {
                        var eventSymbol = (IEventSymbol)symbol;

                        if (IsMatch(eventSymbol.AddMethod))
                            return true;

                        if (IsMatch(eventSymbol.RemoveMethod))
                            return true;

                        break;
                    }
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        if (HasAttributes(methodSymbol.Parameters))
                            return true;

                        break;
                    }
                case SymbolKind.Property:
                    {
                        var propertySymbol = (IPropertySymbol)symbol;

                        if (HasAttributes(propertySymbol.Parameters))
                            return true;

                        if (IsMatch(propertySymbol.GetMethod))
                            return true;

                        if (IsMatch(propertySymbol.SetMethod))
                            return true;

                        break;
                    }
            }

            return false;

            bool HasAttributes(ImmutableArray<IParameterSymbol> parameters)
            {
                return parameters.Any(f => IsMatch(f));
            }

            bool IsMatch(ISymbol s)
            {
                if (s != null)
                {
                    foreach (AttributeData attribute in s.GetAttributes())
                    {
                        if (filter.IsMatch(s, attribute))
                            return true;
                    }
                }

                return false;
            }
        }

        public static ImmutableArray<IParameterSymbol> GetParameters(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                    return GetParameters((INamedTypeSymbol)symbol);
                case SymbolKind.Method:
                    return ((IMethodSymbol)symbol).Parameters;
                case SymbolKind.Property:
                    return ((IPropertySymbol)symbol).Parameters;
            }

            return ImmutableArray<IParameterSymbol>.Empty;
        }

        public static ImmutableArray<IParameterSymbol> GetParameters(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeKind == TypeKind.Delegate)
            {
                IMethodSymbol delegateInvokeMethod = typeSymbol.DelegateInvokeMethod;

                if (delegateInvokeMethod != null)
                    return delegateInvokeMethod.Parameters;
            }

            return ImmutableArray<IParameterSymbol>.Empty;
        }

        public static (IMethodSymbol accessor1, IMethodSymbol accessor2) GetAccessors(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Event:
                    {
                        var eventSymbol = (IEventSymbol)symbol;

                        return (eventSymbol.AddMethod, eventSymbol.RemoveMethod);
                    }
                case SymbolKind.Property:
                    {
                        var propertySymbol = (IPropertySymbol)symbol;

                        return (propertySymbol.GetMethod, propertySymbol.SetMethod);
                    }
            }

            return default;
        }

        public static (int start, int end, ISymbol symbol) FindDefinitionName(
            ISymbol symbol,
            ImmutableArray<SymbolDisplayPart> parts)
        {
            ISymbol s = null;
            int j = 0;

            for (int i = 0; i < parts.Length; i++)
            {
                if (!IsGlobalPrefix(i))
                    continue;

                j = i + 1;

                if (!ReadNamespaces())
                    continue;

                if (symbol.IsKind(SymbolKind.Namespace))
                    return (i, j, parts[j].Symbol);

                if (!ReadTypeNames())
                    continue;

                if (symbol.IsKind(SymbolKind.NamedType))
                {
                    if (s == symbol)
                    {
                        if (((INamedTypeSymbol)s).TypeKind != TypeKind.Delegate
                            || Peek(j).IsPunctuation("("))
                        {
                            return (i, j, s);
                        }
                    }
                }
                else
                {
                    if (Peek(j).IsPunctuation("~"))
                        j++;

                    if (Peek(j).IsMemberName())
                    {
                        j++;

                        s = parts[j].Symbol;

                        if (ReadTypeParameterList())
                            return (i, j, s);
                    }
                    else if (Peek(j).Kind == SymbolDisplayPartKind.Keyword)
                    {
                        switch (Peek(j).ToString())
                        {
                            case "operator":
                                {
                                    if (Peek(j + 1).IsSpace()
                                        && Peek(j + 2).Kind == SymbolDisplayPartKind.MethodName)
                                    {
                                        j += 3;
                                        return (i, j, parts[j].Symbol);
                                    }

                                    break;
                                }
                            case "this":
                                {
                                    j++;
                                    return (i, j, symbol);
                                }
                            case "implicit":
                            case "explicit":
                                {
                                    if (Peek(j + 1).IsSpace()
                                        && Peek(j + 2).IsKeyword("operator")
                                        && Peek(j + 3).IsSpace())
                                    {
                                        j += 4;

                                        if (IsGlobalPrefix(j + 1))
                                        {
                                            j += 2;

                                            if (ReadNamespaces()
                                                && ReadTypeNames())
                                            {
                                                return (i, j, symbol);
                                            }
                                        }
                                    }

                                    break;
                                }
                        }
                    }
                }
            }

            Debug.Fail(parts.ToDisplayString());

            return (-1, -1, null);

            bool IsGlobalPrefix(int i)
            {
                return parts[i].IsGlobalNamespace()
                    && Peek(i).IsPunctuation("::");
            }

            SymbolDisplayPart Peek(int i)
            {
                if (i + 1 < parts.Length)
                    return parts[i + 1];

                return default;
            }

            bool ReadNamespaces()
            {
                if (Peek(j).Kind != SymbolDisplayPartKind.NamespaceName)
                    return false;

                j++;

                while (Peek(j).IsPunctuation("."))
                {
                    j++;

                    if (Peek(j).Kind != SymbolDisplayPartKind.NamespaceName)
                        break;

                    j++;
                }

                return true;
            }

            bool ReadTypeNames()
            {
                if (!Peek(j).IsTypeName())
                    return false;

                j++;

                s = parts[j].Symbol;

                if (!ReadTypeParameterList())
                    return false;

                while (Peek(j).IsPunctuation("."))
                {
                    j++;

                    if (!Peek(j).IsTypeName())
                        break;

                    j++;

                    s = parts[j].Symbol;

                    if (!ReadTypeParameterList())
                        return false;
                }

                return true;
            }

            bool ReadTypeParameterList()
            {
                if (!Peek(j).IsPunctuation("<"))
                    return true;

                j++;

                int depth = 1;

                while (j < parts.Length)
                {
                    if (Peek(j).Kind == SymbolDisplayPartKind.Punctuation)
                    {
                        switch (Peek(j).ToString())
                        {
                            case "<":
                                {
                                    depth++;
                                    break;
                                }
                            case ">":
                                {
                                    Debug.Assert(depth > 0);

                                    depth--;

                                    if (depth == 0)
                                    {
                                        j++;
                                        return true;
                                    }

                                    break;
                                }
                        }
                    }

                    j++;
                }

                return false;
            }
        }
    }
}
