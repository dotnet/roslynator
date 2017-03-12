// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Extensions
{
    public static class SymbolExtensions
    {
        private static SymbolDisplayFormat DefaultSymbolDisplayFormat { get; } = new SymbolDisplayFormat(
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static TypeSyntax ToTypeSyntax(this ITypeSymbol typeSymbol, SymbolDisplayFormat symbolDisplayFormat = null)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            symbolDisplayFormat = symbolDisplayFormat ?? DefaultSymbolDisplayFormat;

            ThrowIfExplicitDeclarationIsNotSupported(typeSymbol, symbolDisplayFormat);

            return ParseTypeName(typeSymbol.ToDisplayString(symbolDisplayFormat));
        }

        public static TypeSyntax ToMinimalTypeSyntax(this ITypeSymbol typeSymbol, SemanticModel semanticModel, int position, SymbolDisplayFormat symbolDisplayFormat = null)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            symbolDisplayFormat = symbolDisplayFormat ?? DefaultSymbolDisplayFormat;

            ThrowIfExplicitDeclarationIsNotSupported(typeSymbol, symbolDisplayFormat);

            return ParseTypeName(typeSymbol.ToMinimalDisplayString(semanticModel, position, symbolDisplayFormat));
        }

        public static ExpressionSyntax ToDefaultExpression(this ITypeSymbol typeSymbol, TypeSyntax type = null)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return ToDefaultExpression(typeSymbol, type, default(SemanticModel), -1, default(SymbolDisplayFormat));
        }

        public static ExpressionSyntax ToDefaultExpression(this ITypeSymbol typeSymbol, SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return ToDefaultExpression(typeSymbol, default(TypeSyntax), semanticModel, position, format);
        }

        private static ExpressionSyntax ToDefaultExpression(ITypeSymbol typeSymbol, TypeSyntax type, SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
        {
            if (typeSymbol.IsErrorType())
                return null;

            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Boolean:
                    return FalseLiteralExpression();
                case SpecialType.System_Char:
                    return CharacterLiteralExpression('\0');
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                    return NumericLiteralExpression(0);
            }

            if (typeSymbol.IsConstructedFrom(SpecialType.System_Nullable_T))
                return NullLiteralExpression();

            if (typeSymbol.BaseType?.SpecialType == SpecialType.System_Enum)
            {
                IFieldSymbol fieldSymbol = typeSymbol.FindFieldWithConstantValue(0);

                if (fieldSymbol != null)
                {
                    if (type == null)
                    {
                        type = (semanticModel != null)
                            ? typeSymbol.ToMinimalTypeSyntax(semanticModel, position, format)
                            : typeSymbol.ToTypeSyntax().WithSimplifierAnnotation();
                    }

                    Debug.Assert(type != null);

                    return SimpleMemberAccessExpression(type, IdentifierName(fieldSymbol.Name));
                }
                else
                {
                    return NumericLiteralExpression(0);
                }
            }

            if (typeSymbol.IsReferenceType)
                return NullLiteralExpression();

            if (type == null)
            {
                type = (semanticModel != null)
                    ? typeSymbol.ToMinimalTypeSyntax(semanticModel, position, format)
                    : typeSymbol.ToTypeSyntax().WithSimplifierAnnotation();
            }

            Debug.Assert(type != null);

            return DefaultExpression(type);
        }

        public static ExpressionSyntax ToDefaultExpression(this IParameterSymbol parameterSymbol)
        {
            if (parameterSymbol == null)
                throw new ArgumentNullException(nameof(parameterSymbol));

            if (parameterSymbol.HasExplicitDefaultValue)
            {
                object value = parameterSymbol.ExplicitDefaultValue;

                ITypeSymbol type = parameterSymbol.Type;

                if (type.IsEnum())
                {
                    if (value != null)
                    {
                        foreach (IFieldSymbol fieldSymbol in type.GetFields())
                        {
                            if (fieldSymbol.HasConstantValue
                                && value.Equals(fieldSymbol.ConstantValue))
                            {
                                return SimpleMemberAccessExpression(type.ToTypeSyntax(), IdentifierName(fieldSymbol.Name));
                            }
                        }

                        return CastExpression(type.ToTypeSyntax().WithSimplifierAnnotation(), ConstantExpression(value));
                    }
                }
                else
                {
                    return ConstantExpression(value);
                }
            }

            return null;
        }

        private static void ThrowIfExplicitDeclarationIsNotSupported(ITypeSymbol typeSymbol, SymbolDisplayFormat symbolDisplayFormat)
        {
            if (!typeSymbol.SupportsExplicitDeclaration())
                throw new ArgumentException($"Type '{typeSymbol.ToDisplayString(symbolDisplayFormat)}' does not support explicit declaration.", nameof(typeSymbol));
        }

        public static bool SupportsPredefinedType(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Object:
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                    return true;
                default:
                    return false;
            }
        }

        public static bool SupportsConstantValue(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                    return true;
                default:
                    return false;
            }
        }

        public static bool SupportsPrefixOrPostfixUnaryOperator(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Char:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_Decimal:
                    return true;
            }

            return typeSymbol.IsEnum();
        }
    }
}
