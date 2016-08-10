// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis
{
    public static class NamingHelper
    {
        public static string CreateIdentifierName(TypeSyntax type, SemanticModel semanticModel, bool firstCharToLower = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(type).Type;

            if (typeSymbol == null)
                return null;

            return CreateIdentifierName(typeSymbol, firstCharToLower);
        }

        public static string CreateIdentifierName(ITypeSymbol typeSymbol, bool firstCharToLower = false)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ITypeSymbol typeSymbol2 = ExtractFromNullableType(typeSymbol);

            ITypeSymbol typeSymbol3 = ExtractFromArrayOrGenericCollection(typeSymbol2);

            string name = GetName(typeSymbol3);

            if (name == null)
                return null;

            if (typeSymbol3.TypeKind == TypeKind.Interface
                && name.Length > 1
                && name[0] == 'I')
            {
                name = name.Substring(1);
            }

            if (UsePlural(typeSymbol2)
                && typeSymbol2.Implements(SpecialType.System_Collections_IEnumerable))
            {
                if (name.EndsWith("s", StringComparison.Ordinal) || name.EndsWith("x", StringComparison.Ordinal))
                    name += "es";
                else if (name.EndsWith("y", StringComparison.Ordinal))
                    name += "ies";
                else
                    name += "s";
            }

            if (firstCharToLower)
                name = TextUtility.FirstCharToLowerInvariant(name);

            return name;
        }

        private static ITypeSymbol ExtractFromNullableType(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.IsNamedType())
            {
                var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
                    return namedTypeSymbol.TypeArguments[0];
            }

            return typeSymbol;
        }

        private static ITypeSymbol ExtractFromArrayOrGenericCollection(ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.Kind)
            {
                case SymbolKind.ArrayType:
                    {
                        return ((IArrayTypeSymbol)typeSymbol).ElementType;
                    }
                case SymbolKind.NamedType:
                    {
                        var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                        if (namedTypeSymbol.TypeArguments.Length == 1
                            && namedTypeSymbol.Implements(SpecialType.System_Collections_IEnumerable))
                        {
                            return namedTypeSymbol.TypeArguments[0];
                        }

                        break;
                    }
            }

            return typeSymbol;
        }

        private static bool UsePlural(ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.Kind)
            {
                case SymbolKind.ArrayType:
                    return true;
                case SymbolKind.NamedType:
                    return ((INamedTypeSymbol)typeSymbol).TypeArguments.Length == 1;
                default:
                    return false;
            }
        }

        private static string GetName(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.IsTypeParameter())
            {
                if (typeSymbol.Name.Length > 1
                    && typeSymbol.Name[0] == 'T')
                {
                    return typeSymbol.Name.Substring(1);
                }
            }
            else if (typeSymbol.IsPredefinedType())
            {
                return null;
            }

            return typeSymbol.Name;
        }

        public static string ToCamelCaseWithUnderscore(string value)
        {
            return CreateName(value, "_");
        }

        public static string ToCamelCase(string value)
        {
            return CreateName(value, "");
        }

        public static string ToCamelCase(string value, bool prefixWithUnderscore = false)
        {
            return CreateName(value, (prefixWithUnderscore) ? "_" : "");
        }

        private static string CreateName(string value, string prefix)
        {
            var sb = new StringBuilder(prefix, value.Length + prefix.Length);

            int i = 0;

            while (i < value.Length && value[i] == '_')
                i++;

            if (char.IsUpper(value[i]))
                sb.Append(char.ToLower(value[i]));
            else
                sb.Append(value[i]);

            i++;

            sb.Append(value, i, value.Length - i);

            return sb.ToString();
        }

        public static bool IsValidCamelCaseWithUnderscore(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value[0] == '_')
            {
                if (value.Length > 1)
                {
                    if (value[1] == '_')
                        return false;

                    if (char.IsUpper(value[1]))
                        return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool IsValidCamelCaseWithoutUnderscore(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value[0] == '_')
                return false;

            if (char.IsUpper(value[0]))
                return false;

            return true;
        }

        public static string RemoveUnderscoreFromIdentifier(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '_')
                {
                    var sb = new StringBuilder(value, 0, i, value.Length);

                    int prevIndex = 0;

                    do
                    {
                        i++;

                        if (i < value.Length && char.IsLower(value[i]))
                        {
                            sb.Append(char.ToUpperInvariant(value[i]));
                            i++;
                        }

                        prevIndex = i;

                        while (i < value.Length)
                        {
                            if (value[i] == '_')
                                break;

                            i++;
                        }

                        sb.Append(value, prevIndex, i - prevIndex);
                    } while (i < value.Length);

                    return sb.ToString();
                }
            }

            return value;
        }
    }
}
