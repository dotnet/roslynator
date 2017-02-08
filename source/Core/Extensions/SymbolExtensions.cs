// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Extensions
{
    public static class SymbolExtensions
    {
        [DebuggerStepThrough]
        public static bool IsMethodKind(this IMethodSymbol methodSymbol, MethodKind methodKind)
        {
            return methodSymbol?.MethodKind == methodKind;
        }

        [DebuggerStepThrough]
        public static bool IsMethodKind(this IMethodSymbol methodSymbol, MethodKind methodKind1, MethodKind methodKind2)
        {
            if (methodSymbol == null)
                return false;

            MethodKind methodKind = methodSymbol.MethodKind;

            return methodKind == methodKind1
                || methodKind == methodKind2;
        }

        [DebuggerStepThrough]
        public static bool IsMethodKind(this IMethodSymbol methodSymbol, MethodKind methodKind1, MethodKind methodKind2, MethodKind methodKind3)
        {
            if (methodSymbol == null)
                return false;

            MethodKind methodKind = methodSymbol.MethodKind;

            return methodKind == methodKind1
                || methodKind == methodKind2
                || methodKind == methodKind3;
        }

        public static IEnumerable<IMethodSymbol> OverriddenMethods(this IMethodSymbol methodSymbol)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            IMethodSymbol overriddenMethod = methodSymbol.OverriddenMethod;

            while (overriddenMethod != null)
            {
                yield return overriddenMethod;

                overriddenMethod = overriddenMethod.OverriddenMethod;
            }
        }

        public static IParameterSymbol SingleParameterOrDefault(this IMethodSymbol methodSymbol)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            if (parameters.Length == 1)
            {
                return parameters.First();
            }
            else
            {
                return null;
            }
        }

        public static IParameterSymbol SingleParameterOrDefault(this IPropertySymbol propertySymbol)
        {
            if (propertySymbol == null)
                throw new ArgumentNullException(nameof(propertySymbol));

            ImmutableArray<IParameterSymbol> parameters = propertySymbol.Parameters;

            if (parameters.Length == 1)
            {
                return parameters.First();
            }
            else
            {
                return null;
            }
        }

        public static IMethodSymbol ReducedFromOrSelf(this IMethodSymbol methodSymbol)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            return methodSymbol.ReducedFrom ?? methodSymbol;
        }

        public static ISymbol FindImplementedInterfaceMember(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            INamedTypeSymbol containingType = symbol.ContainingType;

            if (containingType != null)
            {
                ImmutableArray<INamedTypeSymbol> allInterfaces = containingType.AllInterfaces;

                for (int i = 0; i < allInterfaces.Length; i++)
                {
                    ImmutableArray<ISymbol> members = allInterfaces[i].GetMembers();

                    for (int j = 0; j < members.Length; j++)
                    {
                        if (symbol.Equals(containingType.FindImplementationForInterfaceMember(members[i])))
                            return members[i];
                    }
                }
            }

            return default(ISymbol);
        }

        public static TSymbol FindImplementedInterfaceMember<TSymbol>(this ISymbol symbol) where TSymbol : ISymbol
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            INamedTypeSymbol containingType = symbol.ContainingType;

            if (containingType != null)
            {
                ImmutableArray<INamedTypeSymbol> allInterfaces = containingType.AllInterfaces;

                for (int i = 0; i < allInterfaces.Length; i++)
                {
                    ImmutableArray<ISymbol> members = allInterfaces[i].GetMembers();

                    for (int j = 0; j < members.Length; j++)
                    {
                        if (members[j] is TSymbol)
                        {
                            var tmember = (TSymbol)members[j];

                            if (symbol.Equals(containingType.FindImplementationForInterfaceMember(tmember)))
                                return tmember;
                        }
                    }
                }
            }

            return default(TSymbol);
        }

        public static bool IsNullableOf(this ITypeSymbol typeSymbol, SpecialType specialType)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return typeSymbol.IsNamedType()
                && IsNullableOf((INamedTypeSymbol)typeSymbol, specialType);
        }

        public static bool IsNullableOf(this ITypeSymbol typeSymbol, ITypeSymbol typeArgument)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return typeSymbol.IsNamedType()
                && IsNullableOf((INamedTypeSymbol)typeSymbol, typeArgument);
        }

        public static bool IsNullableOf(this INamedTypeSymbol namedTypeSymbol, SpecialType specialType)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            return namedTypeSymbol.IsConstructedFrom(SpecialType.System_Nullable_T)
                && namedTypeSymbol.TypeArguments[0].SpecialType == specialType;
        }

        public static bool IsNullableOf(this INamedTypeSymbol namedTypeSymbol, ITypeSymbol typeArgument)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            if (typeArgument == null)
                throw new ArgumentNullException(nameof(typeArgument));

            return namedTypeSymbol.IsConstructedFrom(SpecialType.System_Nullable_T)
                && namedTypeSymbol.TypeArguments[0] == typeArgument;
        }

        public static IEnumerable<INamespaceSymbol> ContainingNamespacesAndSelf(this INamespaceSymbol @namespace)
        {
            if (@namespace == null)
                throw new ArgumentNullException(nameof(@namespace));

            do
            {
                yield return @namespace;

                @namespace = @namespace.ContainingNamespace;

            } while (@namespace != null);
        }

        [DebuggerStepThrough]
        public static bool IsKind(this ISymbol symbol, SymbolKind kind)
        {
            return symbol?.Kind == kind;
        }

        [DebuggerStepThrough]
        public static bool IsKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2)
        {
            if (symbol == null)
                return false;

            SymbolKind kind = symbol.Kind;

            return kind == kind1
                || kind == kind2;
        }

        [DebuggerStepThrough]
        public static bool IsKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2, SymbolKind kind3)
        {
            if (symbol == null)
                return false;

            SymbolKind kind = symbol.Kind;

            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        [DebuggerStepThrough]
        public static bool IsPublic(this ISymbol symbol)
        {
            return symbol?.DeclaredAccessibility == Accessibility.Public;
        }

        [DebuggerStepThrough]
        public static bool IsInternal(this ISymbol symbol)
        {
            return symbol?.DeclaredAccessibility == Accessibility.Internal;
        }

        [DebuggerStepThrough]
        public static bool IsProtected(this ISymbol symbol)
        {
            return symbol?.DeclaredAccessibility == Accessibility.Protected;
        }

        [DebuggerStepThrough]
        public static bool IsPrivate(this ISymbol symbol)
        {
            return symbol?.DeclaredAccessibility == Accessibility.Private;
        }

        [DebuggerStepThrough]
        public static bool IsArrayType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.ArrayType;
        }

        [DebuggerStepThrough]
        public static bool IsDynamicType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.DynamicType;
        }

        [DebuggerStepThrough]
        public static bool IsErrorType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.ErrorType;
        }

        [DebuggerStepThrough]
        public static bool IsEvent(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Event;
        }

        [DebuggerStepThrough]
        public static bool IsField(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Field;
        }

        [DebuggerStepThrough]
        public static bool IsLocal(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Local;
        }

        [DebuggerStepThrough]
        public static bool IsMethod(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Method;
        }

        [DebuggerStepThrough]
        public static bool IsNamedType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.NamedType;
        }

        [DebuggerStepThrough]
        public static bool IsNamespace(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Namespace;
        }

        [DebuggerStepThrough]
        public static bool IsParameter(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Parameter;
        }

        [DebuggerStepThrough]
        public static bool IsProperty(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Property;
        }

        [DebuggerStepThrough]
        public static bool IsTypeParameter(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.TypeParameter;
        }

        public static bool IsPublicProperty(this ISymbol symbol)
        {
            return symbol?.IsPublic() == true
                && symbol.IsProperty();
        }

        public static bool IsPublicInstanceProperty(this ISymbol symbol)
        {
            return symbol?.IsPublic() == true
                && !symbol.IsStatic
                && symbol.IsProperty();
        }

        public static bool IsPublicStaticProperty(this ISymbol symbol)
        {
            return symbol?.IsPublic() == true
                && symbol.IsStatic
                && symbol.IsProperty();
        }

        public static bool IsPrivateField(this ISymbol symbol)
        {
            return symbol?.IsPrivate() == true
                && symbol.IsField();
        }

        public static bool IsInstanceField(this ISymbol symbol)
        {
            return symbol?.IsStatic == false
                && symbol.IsField();
        }

        public static bool IsStaticField(this ISymbol symbol)
        {
            return symbol?.IsStatic == true
                && symbol.IsField();
        }

        public static bool IsEnumField(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Field
                && symbol.ContainingType?.TypeKind == TypeKind.Enum;
        }

        public static IEnumerable<INamespaceSymbol> ContainingNamespaces(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            INamespaceSymbol containingNamespace = symbol.ContainingNamespace;

            do
            {
                yield return containingNamespace;

                containingNamespace = containingNamespace.ContainingNamespace;

            } while (containingNamespace != null);
        }

        public static bool IsStaticClass(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.IsStatic == true
                && typeSymbol.IsClass();
        }

        public static bool IsInstanceClass(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.IsStatic == false
                && typeSymbol.IsClass();
        }

        public static bool IsVoidMethod(this ISymbol typeSymbol)
        {
            return typeSymbol?.IsMethod() == true
                && ((IMethodSymbol)typeSymbol).ReturnsVoid;
        }

        public static bool IsAsyncMethod(this ISymbol symbol)
        {
            return symbol?.IsMethod() == true
                && ((IMethodSymbol)symbol).IsAsync;
        }

        public static bool IsInstanceMethod(this ISymbol symbol)
        {
            return symbol?.IsStatic == false
                && symbol.IsMethod();
        }

        public static bool IsStaticMethod(this ISymbol symbol)
        {
            return symbol?.IsStatic == true
                && symbol.IsMethod();
        }

        public static bool IsPublicInstanceMethod(this ISymbol symbol)
        {
            return symbol?.IsPublic() == true
                && !symbol.IsStatic
                && symbol.IsMethod();
        }

        public static bool IsPublicMethod(this ISymbol symbol)
        {
            return symbol?.IsPublic() == true
                && symbol.IsMethod();
        }

        public static bool IsPublicStaticMethod(this ISymbol symbol)
        {
            return symbol?.IsPublic() == true
                && symbol.IsStatic
                && symbol.IsMethod();
        }

        public static bool IsPubliclyVisible(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            do
            {
                switch (symbol.Kind)
                {
                    case SymbolKind.Event:
                    case SymbolKind.Field:
                    case SymbolKind.Method:
                    case SymbolKind.NamedType:
                    case SymbolKind.Property:
                        {
                            Accessibility accessibility = symbol.DeclaredAccessibility;

                            if (accessibility == Accessibility.Public
                                || accessibility == Accessibility.Protected
                                || accessibility == Accessibility.ProtectedOrInternal)
                            {
                                INamedTypeSymbol containingType = symbol.ContainingType;

                                if (containingType != null)
                                {
                                    symbol = containingType;
                                    break;
                                }
                                else
                                {
                                    return symbol.ContainingNamespace != null;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    case SymbolKind.Namespace:
                        {
                            return true;
                        }
                }

            } while (symbol != null);

            return false;
        }

        public static bool IsAccessible(this ISymbol symbol, int position, SemanticModel semanticModel)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return semanticModel.IsAccessible(position, symbol);
        }

        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, sbyte value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is sbyte
                    && (sbyte)constantValue == value;
            }

            return false;
        }

        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, byte value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is byte
                    && (byte)constantValue == value;
            }

            return false;
        }

        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, short value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is short
                    && (short)constantValue == value;
            }

            return false;
        }

        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, ushort value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is ushort
                    && (ushort)constantValue == value;
            }

            return false;
        }

        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, int value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is int
                    && (int)constantValue == value;
            }

            return false;
        }

        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, uint value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is uint
                    && (uint)constantValue == value;
            }

            return false;
        }

        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, long value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is long
                    && (long)constantValue == value;
            }

            return false;
        }

        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, ulong value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is ulong
                    && (ulong)constantValue == value;
            }

            return false;
        }

        [DebuggerStepThrough]
        public static bool IsVoid(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Void;
        }

        [DebuggerStepThrough]
        public static bool IsInt32(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Int32;
        }

        [DebuggerStepThrough]
        public static bool IsBoolean(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Boolean;
        }

        [DebuggerStepThrough]
        public static bool IsString(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_String;
        }

        [DebuggerStepThrough]
        public static bool IsObject(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Object;
        }

        [DebuggerStepThrough]
        public static bool IsChar(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Char;
        }

        public static IEnumerable<INamedTypeSymbol> BaseTypes(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            INamedTypeSymbol baseType = typeSymbol.BaseType;

            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }

        public static IEnumerable<ITypeSymbol> BaseTypesAndSelf(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ITypeSymbol current = typeSymbol;

            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static bool Implements(this ITypeSymbol typeSymbol, SpecialType specialType)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<INamedTypeSymbol> allInterfaces = typeSymbol.AllInterfaces;

            for (int i = 0; i < allInterfaces.Length; i++)
            {
                if (allInterfaces[i].SpecialType == specialType)
                    return true;
            }

            return false;
        }

        [DebuggerStepThrough]
        public static bool IsClass(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Class;
        }

        [DebuggerStepThrough]
        public static bool IsInterface(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Interface;
        }

        [DebuggerStepThrough]
        public static bool IsStruct(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Struct;
        }

        [DebuggerStepThrough]
        public static bool IsEnum(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Enum;
        }

        public static bool SupportsExplicitDeclaration(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (!typeSymbol.IsAnonymousType)
            {
                switch (typeSymbol.Kind)
                {
                    case SymbolKind.TypeParameter:
                        return true;
                    case SymbolKind.ArrayType:
                        return SupportsExplicitDeclaration(((IArrayTypeSymbol)typeSymbol).ElementType);
                    case SymbolKind.NamedType:
                        return !IsAnyTypeArgumentAnonymousType((INamedTypeSymbol)typeSymbol);
                }
            }

            return false;
        }

        private static bool IsAnyTypeArgumentAnonymousType(INamedTypeSymbol namedTypeSymbol)
        {
            ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

            if (typeArguments.Length > 0)
            {
                var stack = new Stack<ITypeSymbol>(typeArguments);

                while (stack.Count > 0)
                {
                    ITypeSymbol type = stack.Pop();

                    if (type.IsAnonymousType)
                        return true;

                    if (type.IsNamedType())
                    {
                        typeArguments = ((INamedTypeSymbol)type).TypeArguments;

                        for (int i = 0; i < typeArguments.Length; i++)
                            stack.Push(typeArguments[i]);
                    }
                }
            }

            return false;
        }

        public static bool IsConstructedFrom(this INamedTypeSymbol namedTypeSymbol, SpecialType specialType)
        {
            return namedTypeSymbol?.ConstructedFrom?.SpecialType == specialType;
        }

        public static bool IsConstructedFrom(this INamedTypeSymbol namedTypeSymbol, ISymbol symbol)
        {
            return namedTypeSymbol?.ConstructedFrom?.Equals(symbol) == true;
        }

        public static bool IsConstructedFrom(this ITypeSymbol typeSymbol, SpecialType specialType)
        {
            return typeSymbol?.IsNamedType() == true
                && ((INamedTypeSymbol)typeSymbol).ConstructedFrom?.SpecialType == specialType;
        }

        public static bool IsConstructedFrom(this ITypeSymbol typeSymbol, ISymbol symbol)
        {
            return typeSymbol?.IsNamedType() == true
                && ((INamedTypeSymbol)typeSymbol).ConstructedFrom?.Equals(symbol) == true;
        }

        internal static bool IsConstructedFrom(this ITypeSymbol typeSymbol, string fullyQualifiedMetadataName, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (typeSymbol.IsNamedType())
            {
                INamedTypeSymbol symbol = semanticModel.GetTypeByMetadataName(fullyQualifiedMetadataName);

                return symbol != null
                    && ((INamedTypeSymbol)typeSymbol).ConstructedFrom.Equals(symbol);
            }

            return false;
        }

        internal static bool IsConstructedFrom(this INamedTypeSymbol namedTypeSymbol, string fullyQualifiedMetadataName, SemanticModel semanticModel)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            INamedTypeSymbol symbol = semanticModel.GetTypeByMetadataName(fullyQualifiedMetadataName);

            return symbol != null
                && namedTypeSymbol.ConstructedFrom.Equals(symbol);
        }

        public static bool IsTaskOrDerivedFromTask(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            INamedTypeSymbol taskSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task);

            return typeSymbol.EqualsOrInheritsFrom(taskSymbol);
        }

        public static bool IsConstructedFromTaskOfT(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (typeSymbol.IsNamedType())
            {
                INamedTypeSymbol taskOfT = semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T);

                return ((INamedTypeSymbol)typeSymbol).ConstructedFrom.EqualsOrInheritsFrom(taskOfT);
            }

            return false;
        }

        public static bool IsConstructedFromImmutableArrayOfT(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            return IsConstructedFrom(typeSymbol, MetadataNames.System_Collections_Immutable_ImmutableArray_T, semanticModel);
        }

        public static bool IsConstructedFromImmutableArrayOfT(this INamedTypeSymbol namedTypeSymbol, SemanticModel semanticModel)
        {
            return IsConstructedFrom(namedTypeSymbol, MetadataNames.System_Collections_Immutable_ImmutableArray_T, semanticModel);
        }

        public static bool IsIEnumerable(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Collections_IEnumerable;
        }

        public static bool IsIEnumerableOf(this ITypeSymbol typeSymbol, ITypeSymbol typeArgument)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (typeArgument == null)
                throw new ArgumentNullException(nameof(typeArgument));

            if (typeSymbol.IsNamedType())
            {
                var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                return IsConstructedFrom(namedTypeSymbol, SpecialType.System_Collections_Generic_IEnumerable_T)
                    && namedTypeSymbol.TypeArguments.First().Equals(typeArgument);
            }

            return false;
        }

        public static bool IsIEnumerableOf(this INamedTypeSymbol namedTypeSymbol, ITypeSymbol typeArgument)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            if (typeArgument == null)
                throw new ArgumentNullException(nameof(typeArgument));

            return IsConstructedFrom(namedTypeSymbol, SpecialType.System_Collections_Generic_IEnumerable_T)
                && namedTypeSymbol.TypeArguments.First().Equals(typeArgument);
        }

        public static bool IsConstructedFromIEnumerableOfT(this ITypeSymbol typeSymbol)
        {
            return IsConstructedFrom(typeSymbol, SpecialType.System_Collections_Generic_IEnumerable_T);
        }

        public static bool IsConstructedFromIEnumerableOfT(this INamedTypeSymbol namedTypeSymbol)
        {
            return IsConstructedFrom(namedTypeSymbol, SpecialType.System_Collections_Generic_IEnumerable_T);
        }

        public static bool IsIEnumerableOrConstructedFromIEnumerableOfT(this ITypeSymbol typeSymbol)
        {
            return IsIEnumerable(typeSymbol)
                || IsConstructedFromIEnumerableOfT(typeSymbol);
        }

        public static bool InheritsFrom(this ITypeSymbol type, ITypeSymbol baseType, bool includeInterfaces = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.BaseTypes().Any(f => f.Equals(baseType)))
            {
                return true;
            }
            else if (includeInterfaces)
            {
                return type.AllInterfaces.Any(f => f.Equals(baseType));
            }
            else
            {
                return false;
            }
        }

        public static bool EqualsOrInheritsFrom(this ITypeSymbol type, ITypeSymbol baseType, bool includeInterfaces = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.Equals(baseType)
                || InheritsFrom(type, baseType, includeInterfaces);
        }

        [DebuggerStepThrough]
        public static bool IsTypeKind(this ITypeSymbol typeSymbol, TypeKind typeKind)
        {
            return typeSymbol?.TypeKind == typeKind;
        }

        [DebuggerStepThrough]
        public static bool IsTypeKind(this ITypeSymbol typeSymbol, TypeKind typeKind1, TypeKind typeKind2)
        {
            if (typeSymbol == null)
                return false;

            TypeKind typeKind = typeSymbol.TypeKind;

            return typeKind == typeKind1
                || typeKind == typeKind2;
        }

        [DebuggerStepThrough]
        public static bool IsTypeKind(this ITypeSymbol typeSymbol, TypeKind typeKind1, TypeKind typeKind2, TypeKind typeKind3)
        {
            if (typeSymbol == null)
                return false;

            TypeKind typeKind = typeSymbol.TypeKind;

            return typeKind == typeKind1
                || typeKind == typeKind2
                || typeKind == typeKind3;
        }

        public static IEnumerable<IFieldSymbol> GetFields(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<ISymbol> members = typeSymbol.GetMembers();

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Kind == SymbolKind.Field)
                    yield return (IFieldSymbol)members[i];
            }
        }

        public static IEnumerable<IFieldSymbol> GetFields(this ITypeSymbol typeSymbol, string name)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<ISymbol> members = typeSymbol.GetMembers(name);

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Kind == SymbolKind.Field)
                    yield return (IFieldSymbol)members[i];
            }
        }

        public static IEnumerable<IMethodSymbol> GetMethods(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<ISymbol> members = typeSymbol.GetMembers();

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Kind == SymbolKind.Method)
                    yield return (IMethodSymbol)members[i];
            }
        }

        public static IEnumerable<IMethodSymbol> GetMethods(this ITypeSymbol typeSymbol, string name)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<ISymbol> members = typeSymbol.GetMembers(name);

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Kind == SymbolKind.Method)
                    yield return (IMethodSymbol)members[i];
            }
        }

        internal static IFieldSymbol FindFieldWithConstantValue(this ITypeSymbol typeSymbol, int value)
        {
            foreach (IFieldSymbol fieldSymbol in typeSymbol.GetFields())
            {
                if (fieldSymbol.HasConstantValue)
                {
                    object constantValue = fieldSymbol.ConstantValue;

                    if (constantValue is int
                        && (int)constantValue == value)
                    {
                        return fieldSymbol;
                    }
                }
            }

            return null;
        }

        public static bool IsParamsOf(this IParameterSymbol parameterSymbol, ITypeSymbol elementType)
        {
            if (parameterSymbol == null)
                throw new ArgumentNullException(nameof(parameterSymbol));

            if (elementType == null)
                throw new ArgumentNullException(nameof(elementType));

            if (parameterSymbol.IsParams)
            {
                ITypeSymbol type = parameterSymbol.Type;

                if (type.IsArrayType())
                {
                    var arrayType = (IArrayTypeSymbol)type;

                    return arrayType.ElementType.Equals(elementType);
                }
            }

            return false;
        }
    }
}
