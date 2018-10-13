// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.Documentation
{
    internal static class SymbolDeclarationBuilder
    {
        public static ImmutableArray<SymbolDisplayPart> GetDisplayParts(
            ISymbol symbol,
            SymbolDisplayFormat format,
            SymbolDisplayTypeDeclarationOptions typeDeclarationOptions = SymbolDisplayTypeDeclarationOptions.None,
            Func<INamedTypeSymbol, bool> isVisibleAttribute = null,
            bool formatBaseList = false,
            bool formatConstraints = false,
            bool formatParameters = false,
            bool splitAttributes = true,
            bool includeAttributeArguments = false,
            bool omitIEnumerable = false,
            bool useNameOnlyIfPossible = false)
        {
            ImmutableArray<SymbolDisplayPart> parts;

            if (symbol is INamedTypeSymbol typeSymbol)
            {
                parts = typeSymbol.ToDisplayParts(format, typeDeclarationOptions);
            }
            else
            {
                parts = symbol.ToDisplayParts(format);
                typeSymbol = null;
            }

            ImmutableArray<AttributeData> attributes = ImmutableArray<AttributeData>.Empty;
            bool hasAttributes = false;

            if (isVisibleAttribute != null)
            {
                attributes = symbol.GetAttributes();

                hasAttributes = attributes.Any(f => isVisibleAttribute(f.AttributeClass));
            }

            int baseListCount = 0;
            INamedTypeSymbol baseType = null;
            ImmutableArray<INamedTypeSymbol> interfaces = default;

            if (typeSymbol != null)
            {
                if (typeSymbol.TypeKind.Is(TypeKind.Class, TypeKind.Interface))
                {
                    baseType = typeSymbol.BaseType;

                    if (baseType?.SpecialType == SpecialType.System_Object)
                        baseType = null;
                }

                interfaces = typeSymbol.Interfaces;

                if (omitIEnumerable
                    && interfaces.Any(f => f.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T))
                {
                    interfaces = interfaces.RemoveAll(f => f.SpecialType == SpecialType.System_Collections_IEnumerable);
                }

                baseListCount = interfaces.Length;

                if (baseType != null)
                    baseListCount++;
            }

            int constraintCount = 0;
            int whereIndex = -1;

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].IsKeyword("where"))
                {
                    if (whereIndex == -1)
                        whereIndex = i;

                    constraintCount++;
                }
            }

            if (!hasAttributes
                && baseListCount == 0
                && constraintCount == 0
                && (!formatParameters || symbol.GetParameters().Length <= 1))
            {
                return parts;
            }

            INamespaceSymbol containingNamespace = (useNameOnlyIfPossible) ? symbol.ContainingNamespace : null;

            ImmutableArray<SymbolDisplayPart>.Builder builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>(parts.Length);

            AddAttributes(builder, attributes, isVisibleAttribute, containingNamespace, splitAttributes: splitAttributes, includeAttributeArguments: includeAttributeArguments);

            if (baseListCount > 0)
            {
                if (whereIndex != -1)
                {
                    builder.AddRange(parts, whereIndex);
                }
                else
                {
                    builder.AddRange(parts);
                    builder.AddSpace();
                }

                builder.AddPunctuation(":");
                builder.AddSpace();

                if (baseType != null)
                {
                    builder.AddDisplayParts(baseType, containingNamespace);

                    if (interfaces.Any())
                    {
                        builder.AddPunctuation(",");

                        if (formatBaseList)
                        {
                            builder.AddLineBreak();
                            builder.AddIndentation();
                        }
                        else
                        {
                            builder.AddSpace();
                        }
                    }
                }

                interfaces = interfaces.Sort((x, y) =>
                {
                    INamespaceSymbol n1 = x.ContainingNamespace;
                    INamespaceSymbol n2 = y.ContainingNamespace;

                    if (!MetadataNameEqualityComparer<INamespaceSymbol>.Instance.Equals(n1, n2))
                    {
                        return string.CompareOrdinal(
                            n1.ToDisplayString(SymbolDisplayFormats.TypeNameAndContainingTypesAndNamespaces),
                            n2.ToDisplayString(SymbolDisplayFormats.TypeNameAndContainingTypesAndNamespaces));
                    }

                    return string.CompareOrdinal(
                        ToDisplayString(x, containingNamespace),
                        ToDisplayString(y, containingNamespace));
                });

                ImmutableArray<INamedTypeSymbol>.Enumerator en = interfaces.GetEnumerator();

                if (en.MoveNext())
                {
                    while (true)
                    {
                        builder.AddDisplayParts(en.Current, containingNamespace);

                        if (en.MoveNext())
                        {
                            builder.AddPunctuation(",");

                            if (formatBaseList)
                            {
                                builder.AddLineBreak();
                                builder.AddIndentation();
                            }
                            else
                            {
                                builder.AddSpace();
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (whereIndex != -1)
                {
                    if (!formatConstraints
                        || (baseListCount == 1 && constraintCount == 1))
                    {
                        builder.AddSpace();
                    }
                }
            }
            else if (whereIndex != -1)
            {
                builder.AddRange(parts, whereIndex);
            }
            else
            {
                builder.AddRange(parts);
            }

            if (whereIndex != -1)
            {
                for (int i = whereIndex; i < parts.Length; i++)
                {
                    if (parts[i].IsKeyword("where"))
                    {
                        if (formatConstraints
                            && (baseListCount > 1 || constraintCount > 1))
                        {
                            builder.AddLineBreak();
                            builder.AddIndentation();
                        }

                        builder.Add(parts[i]);
                    }
                    else if (parts[i].IsTypeName()
                        && parts[i].Symbol is INamedTypeSymbol namedTypeSymbol)
                    {
                        builder.AddDisplayParts(namedTypeSymbol, containingNamespace);
                    }
                    else
                    {
                        builder.Add(parts[i]);
                    }
                }
            }

            if (formatParameters)
            {
                ImmutableArray<IParameterSymbol> parameters = symbol.GetParameters();

                if (parameters.Length > 1)
                    FormatParameters(symbol, builder, DeclarationListOptions.DefaultValues.IndentChars);
            }

            return builder.ToImmutableArray();
        }

        public static ImmutableArray<SymbolDisplayPart> GetAttributesParts(
            ImmutableArray<AttributeData> attributes,
            Func<INamedTypeSymbol, bool> predicate,
            bool splitAttributes = true,
            bool includeAttributeArguments = false,
            bool isAssemblyAttribute = false,
            bool addNewLine = true)
        {
            ImmutableArray<SymbolDisplayPart>.Builder builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();

            AddAttributes(
                builder,
                attributes,
                predicate,
                splitAttributes: splitAttributes,
                includeAttributeArguments: includeAttributeArguments,
                isAssemblyAttribute: isAssemblyAttribute,
                addNewLine: addNewLine);

            return builder.ToImmutableArray();
        }

        private static void AddAttributes(
            ImmutableArray<SymbolDisplayPart>.Builder builder,
            ImmutableArray<AttributeData> attributes,
            Func<INamedTypeSymbol, bool> predicate = null,
            INamespaceSymbol containingNamespace = null,
            bool splitAttributes = true,
            bool includeAttributeArguments = false,
            bool isAssemblyAttribute = false,
            bool addNewLine = true)
        {
            using (IEnumerator<AttributeData> en = attributes
                .Where(f => predicate(f.AttributeClass))
                .OrderBy(f => ToDisplayString(f.AttributeClass, containingNamespace)).GetEnumerator())
            {
                if (en.MoveNext())
                {
                    builder.AddPunctuation("[");

                    if (isAssemblyAttribute)
                    {
                        builder.AddKeyword("assembly");
                        builder.AddPunctuation(":");
                        builder.AddSpace();
                    }

                    while (true)
                    {
                        builder.AddDisplayParts(en.Current.AttributeClass, containingNamespace);

                        if (includeAttributeArguments)
                            AddAttributeArguments(en.Current);

                        if (en.MoveNext())
                        {
                            if (splitAttributes)
                            {
                                builder.AddPunctuation("]");

                                if (addNewLine)
                                {
                                    builder.AddLineBreak();
                                }
                                else
                                {
                                    builder.AddSpace();
                                }

                                builder.AddPunctuation("[");

                                if (isAssemblyAttribute)
                                {
                                    builder.AddKeyword("assembly");
                                    builder.AddPunctuation(":");
                                    builder.AddSpace();
                                }
                            }
                            else
                            {
                                builder.AddPunctuation(",");
                                builder.AddSpace();
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    builder.AddPunctuation("]");

                    if (addNewLine)
                        builder.AddLineBreak();
                }
            }

            void AddAttributeArguments(AttributeData attributeData)
            {
                bool hasConstructorArgument = false;
                bool hasNamedArgument = false;

                AppendConstructorArguments();
                AppendNamedArguments();

                if (hasConstructorArgument || hasNamedArgument)
                {
                    builder.AddPunctuation(")");
                }

                void AppendConstructorArguments()
                {
                    ImmutableArray<TypedConstant>.Enumerator en = attributeData.ConstructorArguments.GetEnumerator();

                    if (en.MoveNext())
                    {
                        hasConstructorArgument = true;
                        builder.AddPunctuation("(");

                        while (true)
                        {
                            AddConstantValue(en.Current);

                            if (en.MoveNext())
                            {
                                builder.AddPunctuation(",");
                                builder.AddSpace();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                void AppendNamedArguments()
                {
                    ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator en = attributeData.NamedArguments.GetEnumerator();

                    if (en.MoveNext())
                    {
                        hasNamedArgument = true;

                        if (hasConstructorArgument)
                        {
                            builder.AddPunctuation(",");
                            builder.AddSpace();
                        }
                        else
                        {
                            builder.AddPunctuation("(");
                        }

                        while (true)
                        {
                            builder.Add(new SymbolDisplayPart(SymbolDisplayPartKind.PropertyName, null, en.Current.Key));
                            builder.AddSpace();
                            builder.AddPunctuation("=");
                            builder.AddSpace();
                            AddConstantValue(en.Current.Value);

                            if (en.MoveNext())
                            {
                                builder.AddPunctuation(",");
                                builder.AddSpace();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            void AddConstantValue(TypedConstant typedConstant)
            {
                switch (typedConstant.Kind)
                {
                    case TypedConstantKind.Primitive:
                        {
                            builder.Add(new SymbolDisplayPart(
                                GetSymbolDisplayPart(typedConstant.Type.SpecialType),
                                null,
                                SymbolDisplay.FormatPrimitive(typedConstant.Value, quoteStrings: true, useHexadecimalNumbers: false)));

                            break;
                        }
                    case TypedConstantKind.Enum:
                        {
                            OneOrMany<EnumFieldInfo> oneOrMany = EnumUtility.GetConstituentFields(typedConstant.Value, (INamedTypeSymbol)typedConstant.Type);

                            OneOrMany<EnumFieldInfo>.Enumerator en = oneOrMany.GetEnumerator();

                            if (en.MoveNext())
                            {
                                while (true)
                                {
                                    AddDisplayParts(builder, en.Current.Symbol, containingNamespace);

                                    if (en.MoveNext())
                                    {
                                        builder.AddSpace();
                                        builder.AddPunctuation("|");
                                        builder.AddSpace();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                builder.AddPunctuation("(");
                                AddDisplayParts(builder, (INamedTypeSymbol)typedConstant.Type, containingNamespace);
                                builder.AddPunctuation(")");
                                builder.Add(new SymbolDisplayPart(SymbolDisplayPartKind.NumericLiteral, null, typedConstant.Value.ToString()));
                            }

                            break;
                        }
                    case TypedConstantKind.Type:
                        {
                            builder.AddKeyword("typeof");
                            builder.AddPunctuation("(");
                            AddDisplayParts(builder, (ISymbol)typedConstant.Value, containingNamespace);
                            builder.AddPunctuation(")");

                            break;
                        }
                    case TypedConstantKind.Array:
                        {
                            var arrayType = (IArrayTypeSymbol)typedConstant.Type;

                            builder.AddKeyword("new");
                            builder.AddSpace();
                            AddDisplayParts(builder, arrayType.ElementType, containingNamespace);

                            builder.AddPunctuation("[");
                            builder.AddPunctuation("]");
                            builder.AddSpace();
                            builder.AddPunctuation("{");
                            builder.AddSpace();

                            ImmutableArray<TypedConstant>.Enumerator en = typedConstant.Values.GetEnumerator();

                            if (en.MoveNext())
                            {
                                while (true)
                                {
                                    AddConstantValue(en.Current);

                                    if (en.MoveNext())
                                    {
                                        builder.AddPunctuation(",");
                                        builder.AddSpace();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                            builder.AddSpace();
                            builder.AddPunctuation("}");
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }

                SymbolDisplayPartKind GetSymbolDisplayPart(SpecialType specialType)
                {
                    switch (specialType)
                    {
                        case SpecialType.System_Boolean:
                            return SymbolDisplayPartKind.Keyword;
                        case SpecialType.System_SByte:
                        case SpecialType.System_Byte:
                        case SpecialType.System_Int16:
                        case SpecialType.System_UInt16:
                        case SpecialType.System_Int32:
                        case SpecialType.System_UInt32:
                        case SpecialType.System_Int64:
                        case SpecialType.System_UInt64:
                        case SpecialType.System_Single:
                        case SpecialType.System_Double:
                            return SymbolDisplayPartKind.NumericLiteral;
                        case SpecialType.System_Char:
                        case SpecialType.System_String:
                            return SymbolDisplayPartKind.StringLiteral;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }
        }

        internal static void FormatParameters(
            ISymbol symbol,
            ImmutableArray<SymbolDisplayPart>.Builder builder,
            string indentChars)
        {
            int parenthesesDepth = 0;
            int bracesDepth = 0;
            int bracketsDepth = 0;
            int angleBracketsDepth = 0;

            int i = 0;

            int index = FindParameterListStart(symbol, builder);

            Debug.Assert(index >= 0);

            if (index == -1)
                return;

            builder.Insert(index + 1, SymbolDisplayPartFactory.Indentation(indentChars));
            builder.Insert(index + 1, SymbolDisplayPartFactory.LineBreak());

            i++;

            FormatParameters();

            void FormatParameters()
            {
                while (i < builder.Count)
                {
                    SymbolDisplayPart part = builder[i];

                    if (part.Kind == SymbolDisplayPartKind.Punctuation)
                    {
                        switch (part.ToString())
                        {
                            case ",":
                                {
                                    if (((angleBracketsDepth == 0 && parenthesesDepth == 1 && bracesDepth == 0 && bracketsDepth == 0)
                                            || (angleBracketsDepth == 0 && parenthesesDepth == 0 && bracesDepth == 0 && bracketsDepth == 1))
                                        && i < builder.Count - 1)
                                    {
                                        SymbolDisplayPart nextPart = builder[i + 1];

                                        if (nextPart.Kind == SymbolDisplayPartKind.Space)
                                        {
                                            builder[i + 1] = SymbolDisplayPartFactory.LineBreak();
                                            builder.Insert(i + 2, SymbolDisplayPartFactory.Indentation(indentChars));
                                        }
                                    }

                                    break;
                                }
                            case "(":
                                {
                                    parenthesesDepth++;
                                    break;
                                }
                            case ")":
                                {
                                    Debug.Assert(parenthesesDepth >= 0);
                                    parenthesesDepth--;

                                    if (parenthesesDepth == 0
                                        && symbol.IsKind(SymbolKind.Method, SymbolKind.NamedType))
                                    {
                                        return;
                                    }

                                    break;
                                }
                            case "[":
                                {
                                    bracketsDepth++;
                                    break;
                                }
                            case "]":
                                {
                                    Debug.Assert(bracketsDepth >= 0);
                                    bracketsDepth--;

                                    if (bracketsDepth == 0
                                        && symbol.Kind == SymbolKind.Property)
                                    {
                                        return;
                                    }

                                    break;
                                }
                            case "{":
                                {
                                    bracesDepth++;
                                    break;
                                }
                            case "}":
                                {
                                    Debug.Assert(bracesDepth >= 0);
                                    bracesDepth--;
                                    break;
                                }
                            case "<":
                                {
                                    angleBracketsDepth++;
                                    break;
                                }
                            case ">":
                                {
                                    Debug.Assert(angleBracketsDepth >= 0);
                                    angleBracketsDepth--;
                                    break;
                                }
                        }
                    }

                    i++;
                }
            }
        }

        internal static int FindParameterListStart(
            ISymbol symbol,
            IList<SymbolDisplayPart> parts)
        {
            int parenthesesDepth = 0;
            int bracesDepth = 0;
            int bracketsDepth = 0;
            int angleBracketsDepth = 0;

            int i = 0;

            while (i < parts.Count)
            {
                SymbolDisplayPart part = parts[i];

                if (part.Kind == SymbolDisplayPartKind.Punctuation)
                {
                    switch (part.ToString())
                    {
                        case "(":
                            {
                                parenthesesDepth++;

                                if (symbol.IsKind(SymbolKind.Method, SymbolKind.NamedType)
                                    && parenthesesDepth == 1
                                    && bracesDepth == 0
                                    && bracketsDepth == 0
                                    && angleBracketsDepth == 0)
                                {
                                    return i;
                                }

                                break;
                            }
                        case ")":
                            {
                                Debug.Assert(parenthesesDepth >= 0);
                                parenthesesDepth--;
                                break;
                            }
                        case "[":
                            {
                                bracketsDepth++;

                                if (symbol.Kind == SymbolKind.Property
                                    && parenthesesDepth == 0
                                    && bracesDepth == 0
                                    && bracketsDepth == 1
                                    && angleBracketsDepth == 0)
                                {
                                    return i;
                                }

                                break;
                            }
                        case "]":
                            {
                                Debug.Assert(bracketsDepth >= 0);
                                bracketsDepth--;
                                break;
                            }
                        case "{":
                            {
                                bracesDepth++;
                                break;
                            }
                        case "}":
                            {
                                Debug.Assert(bracesDepth >= 0);
                                bracesDepth--;
                                break;
                            }
                        case "<":
                            {
                                angleBracketsDepth++;
                                break;
                            }
                        case ">":
                            {
                                Debug.Assert(angleBracketsDepth >= 0);
                                angleBracketsDepth--;
                                break;
                            }
                    }
                }

                i++;
            }

            return -1;
        }

        private static string ToDisplayString(INamedTypeSymbol symbol, INamespaceSymbol containingNamespace)
        {
            ImmutableArray<SymbolDisplayPart>.Builder builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();

            builder.AddDisplayParts(symbol, containingNamespace);

            return builder.ToImmutableArray().ToDisplayString();
        }

        private static void AddDisplayParts(this ImmutableArray<SymbolDisplayPart>.Builder builder, ISymbol symbol, INamespaceSymbol containingNamespace)
        {
            if (containingNamespace != null
                && symbol.ContainingNamespace == containingNamespace)
            {
                builder.AddRange(symbol.ToDisplayParts(SymbolDisplayFormats.TypeNameAndContainingTypes));
            }
            else
            {
                builder.AddRange(symbol.ToDisplayParts(SymbolDisplayFormats.TypeNameAndContainingTypesAndNamespaces));
            }

            if (!(symbol is INamedTypeSymbol typeSymbol))
                return;

            ImmutableArray<ITypeSymbol> typeArguments = typeSymbol.TypeArguments;

            ImmutableArray<ITypeSymbol>.Enumerator en = typeArguments.GetEnumerator();

            if (en.MoveNext())
            {
                builder.AddPunctuation("<");

                while (true)
                {
                    if (en.Current.Kind == SymbolKind.NamedType)
                    {
                        builder.AddDisplayParts((INamedTypeSymbol)en.Current, containingNamespace);
                    }
                    else
                    {
                        Debug.Assert(en.Current.Kind == SymbolKind.TypeParameter, en.Current.Kind.ToString());

                        builder.Add(new SymbolDisplayPart(SymbolDisplayPartKind.TypeParameterName, en.Current, en.Current.Name));
                    }

                    if (en.MoveNext())
                    {
                        builder.AddPunctuation(",");
                        builder.AddSpace();
                    }
                    else
                    {
                        break;
                    }
                }

                builder.AddPunctuation(">");
            }
        }

        private static void AddSpace(this ImmutableArray<SymbolDisplayPart>.Builder builder)
        {
            builder.Add(SymbolDisplayPartFactory.Space());
        }

        private static void AddIndentation(this ImmutableArray<SymbolDisplayPart>.Builder builder)
        {
            builder.Add(SymbolDisplayPartFactory.Indentation());
        }

        private static void AddLineBreak(this ImmutableArray<SymbolDisplayPart>.Builder builder)
        {
            builder.Add(SymbolDisplayPartFactory.LineBreak());
        }

        private static void AddPunctuation(this ImmutableArray<SymbolDisplayPart>.Builder builder, string text)
        {
            builder.Add(SymbolDisplayPartFactory.Punctuation(text));
        }

        private static void AddKeyword(this ImmutableArray<SymbolDisplayPart>.Builder builder, string text)
        {
            builder.Add(SymbolDisplayPartFactory.Keyword(text));
        }
    }
}
