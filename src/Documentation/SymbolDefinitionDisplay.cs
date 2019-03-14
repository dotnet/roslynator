// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp;

namespace Roslynator.Documentation
{
    internal static class SymbolDefinitionDisplay
    {
        public static ImmutableArray<SymbolDisplayPart> GetDisplayParts(
            ISymbol symbol,
            SymbolDisplayFormat format,
            SymbolDisplayTypeDeclarationOptions typeDeclarationOptions = SymbolDisplayTypeDeclarationOptions.None,
            SymbolDisplayAdditionalOptions additionalOptions = SymbolDisplayAdditionalOptions.None,
            Func<ISymbol, AttributeData, bool> shouldDisplayAttribute = null)
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

            IEnumerable<AttributeData> attributes = (additionalOptions.HasOption(SymbolDisplayAdditionalOptions.IncludeAttributes))
                ? GetAttributes(symbol, shouldDisplayAttribute)
                : ImmutableArray<AttributeData>.Empty;

            ImmutableArray<SymbolDisplayPart>.Builder builder = default;

            INamedTypeSymbol baseType = null;
            ImmutableArray<INamedTypeSymbol> interfaces = ImmutableArray<INamedTypeSymbol>.Empty;

            if (typeSymbol != null
                && (typeDeclarationOptions & SymbolDisplayTypeDeclarationOptions.BaseList) != 0)
            {
                if ((typeDeclarationOptions & SymbolDisplayTypeDeclarationOptions.BaseType) != 0
                    && typeSymbol.TypeKind.Is(TypeKind.Class, TypeKind.Interface))
                {
                    baseType = typeSymbol.BaseType;

                    if (baseType?.SpecialType == SpecialType.System_Object)
                        baseType = null;
                }

                if ((typeDeclarationOptions & SymbolDisplayTypeDeclarationOptions.Interfaces) != 0)
                {
                    interfaces = typeSymbol.Interfaces;

                    if (additionalOptions.HasOption(SymbolDisplayAdditionalOptions.OmitIEnumerable)
                        && interfaces.Any(f => f.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T))
                    {
                        interfaces = interfaces.RemoveAll(f => f.SpecialType == SpecialType.System_Collections_IEnumerable);
                    }
                }
            }

            int baseListCount = interfaces.Length;

            if (baseType != null)
                baseListCount++;

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

            if (baseListCount > 0)
            {
                InitializeBuilder();

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
                    builder.AddDisplayParts(baseType, format, additionalOptions);

                    if (interfaces.Any())
                    {
                        builder.AddPunctuation(",");

                        if (additionalOptions.HasOption(SymbolDisplayAdditionalOptions.FormatBaseList))
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

                IComparer<INamedTypeSymbol> comparer = (additionalOptions.HasOption(SymbolDisplayAdditionalOptions.OmitContainingNamespace))
                    ? SymbolDefinitionComparer.SystemFirstOmitContainingNamespace.TypeComparer
                    : SymbolDefinitionComparer.SystemFirst.TypeComparer;

                interfaces = interfaces.Sort(comparer);

                ImmutableArray<INamedTypeSymbol>.Enumerator en = interfaces.GetEnumerator();

                if (en.MoveNext())
                {
                    while (true)
                    {
                        builder.AddDisplayParts(en.Current, format, additionalOptions);

                        if (en.MoveNext())
                        {
                            builder.AddPunctuation(",");

                            if (additionalOptions.HasOption(SymbolDisplayAdditionalOptions.FormatBaseList))
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
                    if (!additionalOptions.HasOption(SymbolDisplayAdditionalOptions.FormatConstraints)
                        || (baseListCount == 1 && constraintCount == 1))
                    {
                        builder.AddSpace();
                    }
                }
            }

            if (whereIndex != -1)
            {
                InitializeBuilder();

                if (baseListCount == 0)
                    builder.AddRange(parts, whereIndex);

                for (int i = whereIndex; i < parts.Length; i++)
                {
                    if (parts[i].IsKeyword("where")
                        && additionalOptions.HasOption(SymbolDisplayAdditionalOptions.FormatConstraints)
                        && (baseListCount > 1 || constraintCount > 1))
                    {
                        builder.AddLineBreak();
                        builder.AddIndentation();
                    }

                    builder.Add(parts[i]);
                }
            }

            if (builder == null
                && attributes.Any())
            {
                builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>(parts.Length);

                AddAttributes(builder, attributes, format, additionalOptions, includeTrailingNewLine: true);

                builder.AddRange(parts);
            }

            bool hasEventAccessorList = false;

            if (additionalOptions.HasOption(SymbolDisplayAdditionalOptions.IncludeAccessorAttributes))
            {
                if (symbol.Kind == SymbolKind.Property)
                {
                    var propertySymbol = (IPropertySymbol)symbol;

                    IMethodSymbol getMethod = propertySymbol.GetMethod;
                    if (getMethod != null)
                    {
                        builder = builder ?? parts.ToBuilder();

                        AddAccessorAttributes(builder, getMethod, format, additionalOptions, shouldDisplayAttribute: shouldDisplayAttribute);
                    }

                    IMethodSymbol setMethod = propertySymbol.SetMethod;
                    if (setMethod != null)
                    {
                        builder = builder ?? parts.ToBuilder();

                        AddAccessorAttributes(builder, setMethod, format, additionalOptions, shouldDisplayAttribute: shouldDisplayAttribute);
                    }
                }
                else if (symbol.Kind == SymbolKind.Event)
                {
                    var eventSymbol = (IEventSymbol)symbol;

                    IEnumerable<AttributeData> addAttributes = GetAttributes(eventSymbol.AddMethod, shouldDisplayAttribute);
                    IEnumerable<AttributeData> removeAttributes = GetAttributes(eventSymbol.RemoveMethod, shouldDisplayAttribute);

                    if (addAttributes.Any()
                        || removeAttributes.Any())
                    {
                        hasEventAccessorList = true;

                        builder = builder ?? parts.ToBuilder();

                        AddEventAccessorAttributes(builder, addAttributes, removeAttributes, format, additionalOptions);
                    }
                }
            }

            ImmutableArray<IParameterSymbol> parameters = symbol.GetParameters();

            if (additionalOptions.HasOption(SymbolDisplayAdditionalOptions.IncludeParameterAttributes)
                && parameters.Any(f => GetAttributes(f, shouldDisplayAttribute).Any()))
            {
                builder = builder ?? parts.ToBuilder();

                AddParameterAttributes(builder, symbol, parameters, format, additionalOptions, shouldDisplayAttribute);
            }

            if (additionalOptions.HasOption(SymbolDisplayAdditionalOptions.FormatParameters)
                && parameters.Length > 1)
            {
                builder = builder ?? parts.ToBuilder();

                FormatParameters(symbol, builder, DefinitionListFormat.Default.IndentChars);
            }

            if (additionalOptions.HasOption(SymbolDisplayAdditionalOptions.PreferDefaultLiteral))
            {
                if ((format.ParameterOptions & SymbolDisplayParameterOptions.IncludeDefaultValue) != 0
                    && parameters.Any(f => f.HasExplicitDefaultValue && HasDefaultExpression(f.Type, f.ExplicitDefaultValue)))
                {
                    builder = builder ?? parts.ToBuilder();

                    builder = ReplaceDefaultExpressionWithDefaultLiteral(symbol, builder);
                }

                if ((format.MemberOptions & SymbolDisplayMemberOptions.IncludeConstantValue) != 0
                    && symbol.IsKind(SymbolKind.Field))
                {
                    var fieldSymbol = (IFieldSymbol)symbol;

                    if (fieldSymbol.IsConst
                        && fieldSymbol.HasConstantValue
                        && HasDefaultExpression(fieldSymbol.Type, fieldSymbol.ConstantValue))
                    {
                        builder = builder ?? parts.ToBuilder();

                        builder = ReplaceDefaultExpressionWithDefaultLiteral(symbol, builder);
                    }
                }
            }

            if (ShouldAddTrailingSemicolon())
            {
                if (builder == null)
                {
                    parts = parts.Add(new SymbolDisplayPart(SymbolDisplayPartKind.Punctuation, null, ";"));
                }
                else
                {
                    builder.AddPunctuation(";");
                }
            }

            return builder?.ToImmutableArray() ?? parts;

            void InitializeBuilder()
            {
                if (builder == null)
                {
                    builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>(parts.Length);

                    if (attributes.Any())
                        AddAttributes(builder, attributes, format, additionalOptions, includeTrailingNewLine: true);
                }
            }

            bool ShouldAddTrailingSemicolon()
            {
                if (additionalOptions.HasOption(SymbolDisplayAdditionalOptions.IncludeTrailingSemicolon))
                {
                    if (typeSymbol?.TypeKind == TypeKind.Delegate)
                        return true;

                    switch (symbol.Kind)
                    {
                        case SymbolKind.Event:
                            return !hasEventAccessorList;
                        case SymbolKind.Field:
                            return symbol.ContainingType?.TypeKind != TypeKind.Enum;
                        case SymbolKind.Method:
                            return true;
                    }
                }

                return false;
            }
        }

        public static ImmutableArray<SymbolDisplayPart> GetAttributesParts(
            ISymbol symbol,
            SymbolDisplayFormat format,
            SymbolDisplayAdditionalOptions additionalOptions,
            Func<ISymbol, AttributeData, bool> shouldDisplayAttribute,
            bool includeTrailingNewLine = false,
            bool? formatAttributes = null)
        {
            IEnumerable<AttributeData> attributes = GetAttributes(symbol, shouldDisplayAttribute);

            if (!attributes.Any())
                return ImmutableArray<SymbolDisplayPart>.Empty;

            ImmutableArray<SymbolDisplayPart>.Builder parts = ImmutableArray.CreateBuilder<SymbolDisplayPart>();

            AddAttributes(
                parts: parts,
                attributes: attributes,
                format: format,
                additionalOptions: additionalOptions,
                includeTrailingNewLine: includeTrailingNewLine,
                formatAttributes: formatAttributes);

            return parts.ToImmutableArray();
        }

        private static void AddAttributes(
            ImmutableArray<SymbolDisplayPart>.Builder parts,
            IEnumerable<AttributeData> attributes,
            SymbolDisplayFormat format,
            SymbolDisplayAdditionalOptions additionalOptions,
            bool includeTrailingNewLine = true,
            bool? formatAttributes = null)
        {
            IComparer<INamedTypeSymbol> comparer = (additionalOptions.HasOption(SymbolDisplayAdditionalOptions.OmitContainingNamespace))
                ? SymbolDefinitionComparer.SystemFirstOmitContainingNamespace.TypeComparer
                : SymbolDefinitionComparer.SystemFirst.TypeComparer;

            attributes = attributes.OrderBy(f => f.AttributeClass, comparer);

            using (IEnumerator<AttributeData> en = attributes.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    parts.AddPunctuation("[");

                    while (true)
                    {
                        AddAttribute(parts, en.Current, format, additionalOptions);

                        if (en.MoveNext())
                        {
                            if (formatAttributes ?? additionalOptions.HasOption(SymbolDisplayAdditionalOptions.FormatAttributes))
                            {
                                parts.AddPunctuation("]");

                                if (includeTrailingNewLine)
                                {
                                    parts.AddLineBreak();
                                }
                                else
                                {
                                    parts.AddSpace();
                                }

                                parts.AddPunctuation("[");
                            }
                            else
                            {
                                parts.AddPunctuation(",");
                                parts.AddSpace();
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    parts.AddPunctuation("]");

                    if (includeTrailingNewLine)
                        parts.AddLineBreak();
                }
            }
        }

        private static void AddAttribute(
            ImmutableArray<SymbolDisplayPart>.Builder parts,
            AttributeData attribute,
            SymbolDisplayFormat format,
            SymbolDisplayAdditionalOptions additionalOptions)
        {
            parts.AddDisplayParts(attribute.AttributeClass, format, additionalOptions, removeAttributeSuffix: true);

            if (additionalOptions.HasOption(SymbolDisplayAdditionalOptions.IncludeAttributeArguments))
                AddAttributeArguments();

            void AddAttributeArguments()
            {
                bool hasConstructorArgument = false;
                bool hasNamedArgument = false;

                AppendConstructorArguments();
                AppendNamedArguments();

                if (hasConstructorArgument || hasNamedArgument)
                {
                    parts.AddPunctuation(")");
                }

                void AppendConstructorArguments()
                {
                    ImmutableArray<TypedConstant>.Enumerator en = attribute.ConstructorArguments.GetEnumerator();

                    if (en.MoveNext())
                    {
                        hasConstructorArgument = true;
                        parts.AddPunctuation("(");

                        while (true)
                        {
                            AddConstantValue(en.Current);

                            if (en.MoveNext())
                            {
                                parts.AddPunctuation(",");
                                parts.AddSpace();
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
                    ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator en = attribute.NamedArguments.GetEnumerator();

                    if (en.MoveNext())
                    {
                        hasNamedArgument = true;

                        if (hasConstructorArgument)
                        {
                            parts.AddPunctuation(",");
                            parts.AddSpace();
                        }
                        else
                        {
                            parts.AddPunctuation("(");
                        }

                        while (true)
                        {
                            parts.Add(new SymbolDisplayPart(SymbolDisplayPartKind.PropertyName, null, en.Current.Key));
                            parts.AddSpace();
                            parts.AddPunctuation("=");
                            parts.AddSpace();
                            AddConstantValue(en.Current.Value);

                            if (en.MoveNext())
                            {
                                parts.AddPunctuation(",");
                                parts.AddSpace();
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
                            parts.Add(new SymbolDisplayPart(
                                GetSymbolDisplayPart(typedConstant.Type.SpecialType),
                                null,
                                SymbolDisplay.FormatPrimitive(typedConstant.Value, quoteStrings: true, useHexadecimalNumbers: false)));

                            break;
                        }
                    case TypedConstantKind.Enum:
                        {
                            OneOrMany<EnumFieldSymbolInfo> oneOrMany = EnumUtility.GetConstituentFields(typedConstant.Value, (INamedTypeSymbol)typedConstant.Type);

                            OneOrMany<EnumFieldSymbolInfo>.Enumerator en = oneOrMany.GetEnumerator();

                            if (en.MoveNext())
                            {
                                while (true)
                                {
                                    AddDisplayParts(parts, en.Current.Symbol, format, additionalOptions);

                                    if (en.MoveNext())
                                    {
                                        parts.AddSpace();
                                        parts.AddPunctuation("|");
                                        parts.AddSpace();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                parts.AddPunctuation("(");
                                AddDisplayParts(parts, (INamedTypeSymbol)typedConstant.Type, format, additionalOptions);
                                parts.AddPunctuation(")");
                                parts.Add(new SymbolDisplayPart(SymbolDisplayPartKind.NumericLiteral, null, typedConstant.Value.ToString()));
                            }

                            break;
                        }
                    case TypedConstantKind.Type:
                        {
                            parts.AddKeyword("typeof");
                            parts.AddPunctuation("(");
                            AddDisplayParts(parts, (ISymbol)typedConstant.Value, format, additionalOptions);
                            parts.AddPunctuation(")");

                            break;
                        }
                    case TypedConstantKind.Array:
                        {
                            var arrayType = (IArrayTypeSymbol)typedConstant.Type;

                            parts.AddKeyword("new");
                            parts.AddSpace();
                            AddDisplayParts(parts, arrayType.ElementType, format, additionalOptions);

                            parts.AddPunctuation("[");
                            parts.AddPunctuation("]");
                            parts.AddSpace();
                            parts.AddPunctuation("{");
                            parts.AddSpace();

                            ImmutableArray<TypedConstant>.Enumerator en = typedConstant.Values.GetEnumerator();

                            if (en.MoveNext())
                            {
                                while (true)
                                {
                                    AddConstantValue(en.Current);

                                    if (en.MoveNext())
                                    {
                                        parts.AddPunctuation(",");
                                        parts.AddSpace();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                            parts.AddSpace();
                            parts.AddPunctuation("}");
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

        private static void AddParameterAttributes(
            ImmutableArray<SymbolDisplayPart>.Builder parts,
            ISymbol symbol,
            ImmutableArray<IParameterSymbol> parameters,
            SymbolDisplayFormat format,
            SymbolDisplayAdditionalOptions additionalOptions,
            Func<ISymbol, AttributeData, bool> shouldDisplayAttribute)
        {
            int i = FindParameterListStart(symbol, parts);

            if (i == -1)
                return;

            i++;

            int parameterIndex = 0;

            AddParameterAttributes();

            int parenthesesDepth = 1;
            int bracesDepth = 0;
            int bracketsDepth = 0;
            int angleBracketsDepth = 0;

            while (i < parts.Count)
            {
                SymbolDisplayPart part = parts[i];

                if (part.Kind == SymbolDisplayPartKind.Punctuation)
                {
                    switch (part.ToString())
                    {
                        case ",":
                            {
                                if (((angleBracketsDepth == 0 && parenthesesDepth == 1 && bracesDepth == 0 && bracketsDepth == 0)
                                        || (angleBracketsDepth == 0 && parenthesesDepth == 0 && bracesDepth == 0 && bracketsDepth == 1))
                                    && i < parts.Count - 1)
                                {
                                    SymbolDisplayPart nextPart = parts[i + 1];

                                    if (nextPart.Kind == SymbolDisplayPartKind.Space)
                                    {
                                        i += 2;
                                        parameterIndex++;

                                        AddParameterAttributes();
                                        continue;
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

            void AddParameterAttributes()
            {
                IParameterSymbol parameter = parameters[parameterIndex];

                ImmutableArray<SymbolDisplayPart> attributeParts = GetAttributesParts(
                    parameter,
                    format,
                    additionalOptions,
                    shouldDisplayAttribute: shouldDisplayAttribute);

                if (attributeParts.Any())
                {
                    parts.Insert(i, SymbolDisplayPartFactory.Space());
                    parts.InsertRange(i, attributeParts);
                    i += attributeParts.Length + 1;
                }
            }
        }

        private static void AddAccessorAttributes(
            ImmutableArray<SymbolDisplayPart>.Builder parts,
            IMethodSymbol methodSymbol,
            SymbolDisplayFormat format,
            SymbolDisplayAdditionalOptions additionalOptions,
            Func<ISymbol, AttributeData, bool> shouldDisplayAttribute)
        {
            ImmutableArray<SymbolDisplayPart> attributeParts = GetAttributesParts(
                methodSymbol,
                format: format,
                additionalOptions: additionalOptions,
                shouldDisplayAttribute: shouldDisplayAttribute,
                formatAttributes: false);

            if (attributeParts.Any())
            {
                string keyword = GetKeyword();

                SymbolDisplayPart part = parts.FirstOrDefault(f => f.IsKeyword(keyword));

                Debug.Assert(part.Kind == SymbolDisplayPartKind.Keyword);

                if (part.Kind == SymbolDisplayPartKind.Keyword)
                {
                    int index = parts.IndexOf(part);

                    parts.Insert(index, SymbolDisplayPartFactory.Space());
                    parts.InsertRange(index, attributeParts);
                }
            }

            string GetKeyword()
            {
                switch (methodSymbol.MethodKind)
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
                        throw new InvalidOperationException();
                }
            }
        }

        private static void AddEventAccessorAttributes(
            ImmutableArray<SymbolDisplayPart>.Builder parts,
            IEnumerable<AttributeData> addAttributes,
            IEnumerable<AttributeData> removeAttributes,
            SymbolDisplayFormat format,
            SymbolDisplayAdditionalOptions additionalOptions)
        {
            parts.AddSpace();
            parts.AddPunctuation("{");
            parts.AddSpace();

            AddAccessorAttributes(addAttributes);

            parts.AddKeyword("add");
            parts.AddPunctuation(";");
            parts.AddSpace();

            AddAccessorAttributes(removeAttributes);

            parts.AddKeyword("remove");
            parts.AddPunctuation(";");
            parts.AddSpace();
            parts.AddPunctuation("}");

            void AddAccessorAttributes(IEnumerable<AttributeData> attributes)
            {
                AddAttributes(
                    parts: parts,
                    attributes: attributes,
                    format: format,
                    additionalOptions: additionalOptions,
                    includeTrailingNewLine: false,
                    formatAttributes: false);

                parts.AddSpace();
            }
        }

        private static void FormatParameters(
            ISymbol symbol,
            ImmutableArray<SymbolDisplayPart>.Builder parts,
            string indentChars)
        {
            int parenthesesDepth = 0;
            int bracesDepth = 0;
            int bracketsDepth = 0;
            int angleBracketsDepth = 0;

            int i = 0;

            int index = FindParameterListStart(symbol, parts);

            Debug.Assert(index != -1);

            if (index == -1)
                return;

            parts.Insert(index + 1, SymbolDisplayPartFactory.Indentation(indentChars));
            parts.Insert(index + 1, SymbolDisplayPartFactory.LineBreak());

            i++;

            while (i < parts.Count)
            {
                SymbolDisplayPart part = parts[i];

                if (part.Kind == SymbolDisplayPartKind.Punctuation)
                {
                    switch (part.ToString())
                    {
                        case ",":
                            {
                                if (((angleBracketsDepth == 0 && parenthesesDepth == 1 && bracesDepth == 0 && bracketsDepth == 0)
                                        || (angleBracketsDepth == 0 && parenthesesDepth == 0 && bracesDepth == 0 && bracketsDepth == 1))
                                    && i < parts.Count - 1)
                                {
                                    SymbolDisplayPart nextPart = parts[i + 1];

                                    if (nextPart.Kind == SymbolDisplayPartKind.Space)
                                    {
                                        parts[i + 1] = SymbolDisplayPartFactory.LineBreak();
                                        parts.Insert(i + 2, SymbolDisplayPartFactory.Indentation(indentChars));
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

        private static int FindParameterListStart(
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

        private static ImmutableArray<SymbolDisplayPart>.Builder ReplaceDefaultExpressionWithDefaultLiteral(
            ISymbol symbol,
            ImmutableArray<SymbolDisplayPart>.Builder parts)
        {
            ImmutableArray<SymbolDisplayPart>.Builder builder = null;

            int prevIndex = 0;
            int i = 0;

            while (i < parts.Count)
            {
                if (parts[i].IsKeyword("default"))
                    ReplaceDefaultExpressionWithDefaultLiteral3();

                i++;
            }

            if (builder == null)
                return parts;

            for (int j = prevIndex; j < parts.Count; j++)
                builder.Add(parts[j]);

            return builder;

            void ReplaceDefaultExpressionWithDefaultLiteral3()
            {
                int openParenIndex = i + 1;

                if (openParenIndex >= parts.Count
                    || !parts[openParenIndex].IsPunctuation("("))
                {
                    return;
                }

                int closeParenIndex = FindClosingParentheses(openParenIndex + 1);

                if (closeParenIndex == -1)
                    return;

                if (builder == null)
                    builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>(parts.Count);

                for (int l = prevIndex; l < openParenIndex; l++)
                    builder.Add(parts[l]);

                i = closeParenIndex;

                prevIndex = i + 1;
            }

            int FindClosingParentheses(int startIndex)
            {
                int depth = 1;

                int j = startIndex;

                while (j < parts.Count)
                {
                    SymbolDisplayPart part = parts[j];

                    if (part.IsPunctuation())
                    {
                        string text = part.ToString();

                        if (text == "(")
                        {
                            depth++;
                        }
                        else if (text == ")")
                        {
                            Debug.Assert(depth > 0, "Parentheses depth should be greater than 0\r\n" + symbol.ToDisplayString(Roslynator.SymbolDisplayFormats.Test));

                            depth--;

                            if (depth == 0)
                                return j;
                        }
                    }

                    j++;
                }

                return -1;
            }
        }

        private static void AddDisplayParts(
            this ImmutableArray<SymbolDisplayPart>.Builder parts,
            ISymbol symbol,
            SymbolDisplayFormat format,
            SymbolDisplayAdditionalOptions additionalOptions,
            bool removeAttributeSuffix = false)
        {
            SymbolDisplayFormat format2;
            if (additionalOptions.HasOption(SymbolDisplayAdditionalOptions.OmitContainingNamespace))
            {
                format2 = SymbolDefinitionDisplayFormats.TypeNameAndContainingTypes;
            }
            else if (format.GlobalNamespaceStyle == SymbolDisplayGlobalNamespaceStyle.Included)
            {
                format2 = SymbolDefinitionDisplayFormats.TypeNameAndContainingTypesAndNamespacesAndGlobalNamespace;
            }
            else
            {
                format2 = SymbolDefinitionDisplayFormats.TypeNameAndContainingTypesAndNamespaces;
            }

            parts.AddRange(symbol.ToDisplayParts(format2));

            if (!(symbol is INamedTypeSymbol typeSymbol))
                return;

            if (removeAttributeSuffix)
            {
                SymbolDisplayPart last = parts.Last();

                if (last.Kind == SymbolDisplayPartKind.ClassName)
                {
                    const string attributeSuffix = "Attribute";

                    string text = last.ToString();
                    if (text.EndsWith(attributeSuffix, StringComparison.Ordinal))
                    {
                        parts[parts.Count - 1] = last.WithText(text.Remove(text.Length - attributeSuffix.Length));
                    }
                }
            }

            ImmutableArray<ITypeSymbol> typeArguments = typeSymbol.TypeArguments;

            ImmutableArray<ITypeSymbol>.Enumerator en = typeArguments.GetEnumerator();

            if (en.MoveNext())
            {
                parts.AddPunctuation("<");

                while (true)
                {
                    if (en.Current.Kind == SymbolKind.NamedType)
                    {
                        parts.AddDisplayParts((INamedTypeSymbol)en.Current, format, additionalOptions);
                    }
                    else
                    {
                        Debug.Assert(en.Current.Kind == SymbolKind.TypeParameter, en.Current.Kind.ToString());

                        parts.Add(new SymbolDisplayPart(SymbolDisplayPartKind.TypeParameterName, en.Current, en.Current.Name));
                    }

                    if (en.MoveNext())
                    {
                        parts.AddPunctuation(",");
                        parts.AddSpace();
                    }
                    else
                    {
                        break;
                    }
                }

                parts.AddPunctuation(">");
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

        private static void InsertRange(this ImmutableArray<SymbolDisplayPart>.Builder builder, int index, ImmutableArray<SymbolDisplayPart> parts)
        {
            for (int i = parts.Length - 1; i >= 0; i--)
            {
                builder.Insert(index, parts[i]);
            }
        }

        private static bool HasOption(this SymbolDisplayAdditionalOptions options, SymbolDisplayAdditionalOptions option)
        {
            return (options & option) == option;
        }

        private static IEnumerable<AttributeData> GetAttributes(ISymbol symbol, Func<ISymbol, AttributeData, bool> predicate)
        {
            if (symbol == null)
                return ImmutableArray<AttributeData>.Empty;

            ImmutableArray<AttributeData> attributes = symbol.GetAttributes();

            if (predicate != null)
                return attributes.Where(f => predicate(symbol, f));

            return attributes;
        }

        private static bool HasDefaultExpression(ITypeSymbol type, object constantValue)
        {
            if (constantValue != null)
                return false;

            if (type.IsReferenceType)
                return false;

            if (type.TypeKind == TypeKind.Pointer)
                return false;

            if (type.IsNullableType())
                return false;

            return true;
        }
    }
}
