// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    public static class SymbolExtensions
    {
        private static SymbolDisplayFormat DefaultSymbolDisplayFormat { get; } = new SymbolDisplayFormat(
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        #region IParameterSymbol
        internal static ExpressionSyntax GetDefaultValueSyntax(this IParameterSymbol parameterSymbol)
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
                        IFieldSymbol fieldSymbol = type.FindField(f => f.HasConstantValue && value.Equals(f.ConstantValue));

                        if (fieldSymbol != null)
                        {
                            return SimpleMemberAccessExpression(type.ToTypeSyntax(), IdentifierName(fieldSymbol.Name));
                        }
                        else
                        {
                            return CastExpression(type.ToTypeSyntax().WithSimplifierAnnotation(), LiteralExpression(value));
                        }
                    }
                }
                else
                {
                    return LiteralExpression(value);
                }
            }

            return null;
        }
        #endregion

        #region ITypeSymbol
        public static TypeSyntax ToTypeSyntax(this ITypeSymbol typeSymbol, SymbolDisplayFormat format = null)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            format = format ?? DefaultSymbolDisplayFormat;

            ThrowIfExplicitDeclarationIsNotSupported(typeSymbol, format);

            return ParseTypeName(typeSymbol.ToDisplayString(format));
        }

        public static TypeSyntax ToMinimalTypeSyntax(this ITypeSymbol typeSymbol, SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            format = format ?? DefaultSymbolDisplayFormat;

            ThrowIfExplicitDeclarationIsNotSupported(typeSymbol, format);

            return ParseTypeName(typeSymbol.ToMinimalDisplayString(semanticModel, position, format));
        }

        private static void ThrowIfExplicitDeclarationIsNotSupported(ITypeSymbol typeSymbol, SymbolDisplayFormat format)
        {
            if (!typeSymbol.SupportsExplicitDeclaration())
                throw new ArgumentException($"Type '{typeSymbol.ToDisplayString(format)}' does not support explicit declaration.", nameof(typeSymbol));
        }

        public static ExpressionSyntax ToDefaultValueSyntax(this ITypeSymbol typeSymbol, TypeSyntax type = null)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return ToDefaultValueSyntax(typeSymbol, type, default(SemanticModel), -1, default(SymbolDisplayFormat));
        }

        public static ExpressionSyntax ToDefaultValueSyntax(this ITypeSymbol typeSymbol, SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return ToDefaultValueSyntax(typeSymbol, default(TypeSyntax), semanticModel, position, format);
        }

        private static ExpressionSyntax ToDefaultValueSyntax(ITypeSymbol typeSymbol, TypeSyntax type, SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
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
                    type = type ?? ((semanticModel != null)
                            ? typeSymbol.ToMinimalTypeSyntax(semanticModel, position, format)
                            : typeSymbol.ToTypeSyntax().WithSimplifierAnnotation());

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

            type = type ?? ((semanticModel != null)
                ? typeSymbol.ToMinimalTypeSyntax(semanticModel, position, format)
                : typeSymbol.ToTypeSyntax().WithSimplifierAnnotation());

            Debug.Assert(type != null);

            return DefaultExpression(type);
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
        #endregion
    }
}
