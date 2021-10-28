// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    public static class WorkspaceSymbolExtensions
    {
        /// <summary>
        /// Creates a new <see cref="ExpressionSyntax"/> that represents default value of the specified type symbol.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <param name="options"></param>
        /// <param name="format"></param>
        public static ExpressionSyntax GetDefaultValueSyntax(
            this ITypeSymbol typeSymbol,
            DefaultSyntaxOptions options = DefaultSyntaxOptions.None,
            SymbolDisplayFormat format = null)
        {
            return GetDefaultValueSyntax(typeSymbol, options, default(TypeSyntax), format);
        }

        /// <summary>
        /// Creates a new <see cref="ExpressionSyntax"/> that represents default value of the specified type symbol.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <param name="type"></param>
        /// <param name="options"></param>
        public static ExpressionSyntax GetDefaultValueSyntax(
            this ITypeSymbol typeSymbol,
            TypeSyntax type,
            DefaultSyntaxOptions options = DefaultSyntaxOptions.None)
        {
            return GetDefaultValueSyntax(typeSymbol, options, type, default(SymbolDisplayFormat));
        }

        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/default-values-table
        private static ExpressionSyntax GetDefaultValueSyntax(
            this ITypeSymbol typeSymbol,
            DefaultSyntaxOptions options = DefaultSyntaxOptions.None,
            TypeSyntax type = null,
            SymbolDisplayFormat format = null)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if ((options & DefaultSyntaxOptions.UseDefault) != 0)
                return CreateDefault();

            if (typeSymbol.IsReferenceType
                || typeSymbol.TypeKind == TypeKind.Pointer
                || typeSymbol.IsNullableType())
            {
                return NullLiteralExpression();
            }

            if (typeSymbol.TypeKind == TypeKind.Enum)
            {
                IFieldSymbol fieldSymbol = CSharpUtility.FindEnumDefaultField((INamedTypeSymbol)typeSymbol);

                if (fieldSymbol != null)
                    return SimpleMemberAccessExpression(GetTypeSyntax(), IdentifierName(fieldSymbol.Name));

                return CastExpression(GetTypeSyntax(), NumericLiteralExpression(0)).WithSimplifierAnnotation();
            }

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

            return CreateDefault();

            TypeSyntax GetTypeSyntax()
            {
                return type ?? typeSymbol.ToTypeSyntax(format).WithSimplifierAnnotation();
            }

            ExpressionSyntax CreateDefault()
            {
                if ((options & DefaultSyntaxOptions.AllowDefaultLiteral) != 0)
                    return DefaultLiteralExpression();

                return DefaultExpression(GetTypeSyntax());
            }
        }

        internal static ExpressionSyntax GetDefaultValueSyntax(
            this IParameterSymbol parameterSymbol,
            SymbolDisplayFormat format = null)
        {
            if (parameterSymbol == null)
                throw new ArgumentNullException(nameof(parameterSymbol));

            if (!parameterSymbol.HasExplicitDefaultValue)
                throw new ArgumentException("Parameter does not specify default value.", nameof(parameterSymbol));

            object value = parameterSymbol.ExplicitDefaultValue;

            ITypeSymbol typeSymbol = parameterSymbol.Type;

            if (typeSymbol.TypeKind == TypeKind.Enum)
            {
                if (value == null)
                    return NullLiteralExpression();

                IFieldSymbol fieldSymbol = FindFieldWithConstantValue();

                TypeSyntax type = typeSymbol.ToTypeSyntax(format);

                if (fieldSymbol != null)
                {
                    return SimpleMemberAccessExpression(type, IdentifierName(fieldSymbol.Name));
                }
                else
                {
                    return CastExpression(type, LiteralExpression(value));
                }
            }

            if (value == null
                && !typeSymbol.IsReferenceTypeOrNullableType())
            {
                return DefaultExpression(typeSymbol.ToTypeSyntax(format));
            }

            return LiteralExpression(value);

            IFieldSymbol FindFieldWithConstantValue()
            {
                foreach (ISymbol symbol in typeSymbol.GetMembers())
                {
                    if (symbol.Kind == SymbolKind.Field)
                    {
                        var fieldSymbol = (IFieldSymbol)symbol;

                        if (fieldSymbol.HasConstantValue
                            && object.Equals(fieldSymbol.ConstantValue, value))
                        {
                            return fieldSymbol;
                        }
                    }
                }

                return null;
            }
        }
    }
}
