// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class SymbolExtensions
    {
        #region ISymbol
        public static ISymbol FindImplementedInterfaceMember(this ISymbol symbol, bool allInterfaces = false)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            INamedTypeSymbol containingType = symbol.ContainingType;

            if (containingType != null)
            {
                ImmutableArray<INamedTypeSymbol> interfaces = containingType.GetInterfaces(allInterfaces);

                for (int i = 0; i < interfaces.Length; i++)
                {
                    ImmutableArray<ISymbol> members = interfaces[i].GetMembers();

                    for (int j = 0; j < members.Length; j++)
                    {
                        if (symbol.Equals(containingType.FindImplementationForInterfaceMember(members[j])))
                            return members[j];
                    }
                }
            }

            return default(ISymbol);
        }

        internal static bool ImplementsInterfaceMember(this ISymbol symbol, bool allInterfaces = false)
        {
            return FindImplementedInterfaceMember(symbol, allInterfaces) != null;
        }

        public static TSymbol FindImplementedInterfaceMember<TSymbol>(this ISymbol symbol, bool allInterfaces = false) where TSymbol : ISymbol
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            INamedTypeSymbol containingType = symbol.ContainingType;

            if (containingType != null)
            {
                ImmutableArray<INamedTypeSymbol> interfaces = containingType.GetInterfaces(allInterfaces);

                for (int i = 0; i < interfaces.Length; i++)
                {
                    ImmutableArray<ISymbol> members = interfaces[i].GetMembers();

                    for (int j = 0; j < members.Length; j++)
                    {
                        if ((members[j] is TSymbol tmember)
                            && symbol.Equals(containingType.FindImplementationForInterfaceMember(tmember)))
                        {
                            return tmember;
                        }
                    }
                }
            }

            return default(TSymbol);
        }

        internal static bool ImplementsInterfaceMember<TSymbol>(this ISymbol symbol, bool allInterfaces = false) where TSymbol : ISymbol
        {
            return !EqualityComparer<TSymbol>.Default.Equals(
                FindImplementedInterfaceMember<TSymbol>(symbol, allInterfaces),
                default(TSymbol));
        }

        internal static bool IsAnyInterfaceMemberExplicitlyImplemented(this INamedTypeSymbol symbol, ISymbol interfaceSymbol)
        {
            foreach (ISymbol member in symbol.GetMembers())
            {
                switch (member.Kind)
                {
                    case SymbolKind.Event:
                        {
                            foreach (IEventSymbol eventSymbol in ((IEventSymbol)member).ExplicitInterfaceImplementations)
                            {
                                if (eventSymbol.ContainingType?.Equals(interfaceSymbol) == true)
                                    return true;
                            }

                            break;
                        }
                    case SymbolKind.Method:
                        {
                            foreach (IMethodSymbol methodSymbol in ((IMethodSymbol)member).ExplicitInterfaceImplementations)
                            {
                                if (methodSymbol.ContainingType?.Equals(interfaceSymbol) == true)
                                    return true;
                            }

                            break;
                        }
                    case SymbolKind.Property:
                        {
                            foreach (IPropertySymbol propertySymbol in ((IPropertySymbol)member).ExplicitInterfaceImplementations)
                            {
                                if (propertySymbol.ContainingType?.Equals(interfaceSymbol) == true)
                                    return true;
                            }

                            break;
                        }
                }
            }

            return false;
        }

        public static bool IsKind(this ISymbol symbol, SymbolKind kind)
        {
            return symbol?.Kind == kind;
        }

        public static bool IsKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2)
        {
            if (symbol == null)
                return false;

            SymbolKind kind = symbol.Kind;

            return kind == kind1
                || kind == kind2;
        }

        public static bool IsKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2, SymbolKind kind3)
        {
            if (symbol == null)
                return false;

            SymbolKind kind = symbol.Kind;

            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        public static bool IsKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2, SymbolKind kind3, SymbolKind kind4)
        {
            if (symbol == null)
                return false;

            SymbolKind kind = symbol.Kind;

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4;
        }

        public static bool IsKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2, SymbolKind kind3, SymbolKind kind4, SymbolKind kind5)
        {
            if (symbol == null)
                return false;

            SymbolKind kind = symbol.Kind;

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5;
        }

        public static bool IsPublic(this ISymbol symbol)
        {
            return symbol?.DeclaredAccessibility == Accessibility.Public;
        }

        public static bool IsInternal(this ISymbol symbol)
        {
            return symbol?.DeclaredAccessibility == Accessibility.Internal;
        }

        public static bool IsProtected(this ISymbol symbol)
        {
            return symbol?.DeclaredAccessibility == Accessibility.Protected;
        }

        public static bool IsPrivate(this ISymbol symbol)
        {
            return symbol?.DeclaredAccessibility == Accessibility.Private;
        }

        public static bool IsArrayType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.ArrayType;
        }

        public static bool IsDynamicType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.DynamicType;
        }

        public static bool IsErrorType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.ErrorType;
        }

        public static bool IsEvent(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Event;
        }

        public static bool IsField(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Field;
        }

        public static bool IsLocal(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Local;
        }

        public static bool IsMethod(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Method;
        }

        public static bool IsNamedType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.NamedType;
        }

        public static bool IsNamespace(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Namespace;
        }

        public static bool IsParameter(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Parameter;
        }

        public static bool IsProperty(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Property;
        }

        public static bool IsTypeParameter(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.TypeParameter;
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

        public static bool IsAsyncMethod(this ISymbol symbol)
        {
            return symbol?.IsMethod() == true
                && ((IMethodSymbol)symbol).IsAsync;
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

        public static bool IsAccessible(this ISymbol symbol, SemanticModel semanticModel, int position)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return semanticModel.IsAccessible(position, symbol);
        }

        internal static bool IsAnonymousTypeProperty(this ISymbol symbol)
        {
            return symbol.IsProperty()
                && symbol.ContainingType.IsAnonymousType;
        }

        internal static SyntaxNode GetSyntax(this ISymbol symbol, CancellationToken cancellationToken = default(CancellationToken))
        {
            return symbol
                .DeclaringSyntaxReferences[0]
                .GetSyntax(cancellationToken);
        }

        internal static Task<SyntaxNode> GetSyntaxAsync(this ISymbol symbol, CancellationToken cancellationToken = default(CancellationToken))
        {
            return symbol
                .DeclaringSyntaxReferences[0]
                .GetSyntaxAsync(cancellationToken);
        }

        internal static SyntaxNode GetSyntaxOrDefault(this ISymbol symbol, CancellationToken cancellationToken = default(CancellationToken))
        {
            return symbol
                .DeclaringSyntaxReferences
                .FirstOrDefault()?
                .GetSyntax(cancellationToken);
        }

        internal static Task<SyntaxNode> GetSyntaxOrDefaultAsync(this ISymbol symbol, CancellationToken cancellationToken = default(CancellationToken))
        {
            return symbol
                .DeclaringSyntaxReferences
                .FirstOrDefault()?
                .GetSyntaxAsync(cancellationToken);
        }

        internal static bool TryGetSyntax<TNode>(this ISymbol symbol, out TNode node) where TNode : SyntaxNode
        {
            return TryGetSyntax(symbol, default(CancellationToken), out node);
        }

        internal static bool TryGetSyntax<TNode>(this ISymbol symbol, CancellationToken cancellationToken, out TNode node) where TNode : SyntaxNode
        {
            ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

            if (syntaxReferences.Any())
            {
                node = syntaxReferences[0].GetSyntax(cancellationToken) as TNode;

                if (node != null)
                    return true;
            }

            node = null;
            return false;
        }

        public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attributeSymbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (attributeSymbol != null)
            {
                ImmutableArray<AttributeData> attributes = symbol.GetAttributes();

                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i].AttributeClass.Equals(attributeSymbol))
                        return true;
                }
            }

            return false;
        }

        internal static AttributeData GetAttributeByMetadataName(this INamedTypeSymbol typeSymbol, string fullyQualifiedMetadataName, Compilation compilation)
        {
            ImmutableArray<AttributeData> attributes = typeSymbol.GetAttributes();

            if (attributes.Any())
            {
                INamedTypeSymbol attributeType = compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);

                if (attributeType != null)
                {
                    foreach (AttributeData attributeData in attributes)
                    {
                        if (attributeData.AttributeClass.Equals(attributeType))
                            return attributeData;
                    }
                }
            }

            return null;
        }

        public static bool IsDeclaredAccessibility(this ISymbol symbol, Accessibility accessibility)
        {
            return symbol?.DeclaredAccessibility == accessibility;
        }

        public static bool IsDeclaredAccessibility(this ISymbol symbol, Accessibility accessibility1, Accessibility accessibility2)
        {
            if (symbol == null)
                return false;

            Accessibility accessibility = symbol.DeclaredAccessibility;

            return accessibility == accessibility1
                || accessibility == accessibility2;
        }

        public static bool IsDeclaredAccessibility(this ISymbol symbol, Accessibility accessibility1, Accessibility accessibility2, Accessibility accessibility3)
        {
            if (symbol == null)
                return false;

            Accessibility accessibility = symbol.DeclaredAccessibility;

            return accessibility == accessibility1
                || accessibility == accessibility2
                || accessibility == accessibility3;
        }

        internal static ImmutableArray<IParameterSymbol> GetParameters(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            switch (symbol.Kind)
            {
                case SymbolKind.Method:
                    return ((IMethodSymbol)symbol).Parameters;
                case SymbolKind.Property:
                    return ((IPropertySymbol)symbol).Parameters;
                default:
                    return ImmutableArray<IParameterSymbol>.Empty;
            }
        }

        public static ISymbol OverriddenSymbol(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            switch (symbol.Kind)
            {
                case SymbolKind.Method:
                    return ((IMethodSymbol)symbol).OverriddenMethod;
                case SymbolKind.Property:
                    return ((IPropertySymbol)symbol).OverriddenProperty;
                case SymbolKind.Event:
                    return ((IEventSymbol)symbol).OverriddenEvent;
            }

            return null;
        }

        internal static ISymbol BaseOverriddenSymbol(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            switch (symbol.Kind)
            {
                case SymbolKind.Method:
                    return ((IMethodSymbol)symbol).BaseOverriddenMethod();
                case SymbolKind.Property:
                    return ((IPropertySymbol)symbol).BaseOverriddenProperty();
                case SymbolKind.Event:
                    return ((IEventSymbol)symbol).BaseOverriddenEvent();
            }

            return null;
        }
        #endregion ISymbol

        #region IEventSymbol
        internal static IEventSymbol BaseOverriddenEvent(this IEventSymbol eventSymbol)
        {
            if (eventSymbol == null)
                throw new ArgumentNullException(nameof(eventSymbol));

            while (true)
            {
                IEventSymbol overriddenEvent = eventSymbol.OverriddenEvent;

                if (overriddenEvent == null)
                    return eventSymbol;

                eventSymbol = overriddenEvent;
            }
        }

        public static IEnumerable<IEventSymbol> OverriddenEvents(this IEventSymbol eventSymbol)
        {
            if (eventSymbol == null)
                throw new ArgumentNullException(nameof(eventSymbol));

            IEventSymbol overriddenEvent = eventSymbol.OverriddenEvent;

            while (overriddenEvent != null)
            {
                yield return overriddenEvent;

                overriddenEvent = overriddenEvent.OverriddenEvent;
            }
        }
        #endregion IEventSymbol

        #region IFieldSymbol
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, bool value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is bool
                    && (bool)constantValue == value;
            }

            return false;
        }

        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, char value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is char
                    && (char)constantValue == value;
            }

            return false;
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

        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, decimal value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is decimal
                    && (decimal)constantValue == value;
            }

            return false;
        }

        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, float value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is float
                    && (float)constantValue == value;
            }

            return false;
        }

        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, double value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is double
                    && (double)constantValue == value;
            }

            return false;
        }

        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, string value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is string
                    && (string)constantValue == value;
            }

            return false;
        }
        #endregion IFieldSymbol

        #region IMethodSymbol
        internal static IMethodSymbol BaseOverriddenMethod(this IMethodSymbol methodSymbol)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            while (true)
            {
                IMethodSymbol overriddenMethod = methodSymbol.OverriddenMethod;

                if (overriddenMethod == null)
                    return methodSymbol;

                methodSymbol = overriddenMethod;
            }
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

        public static IMethodSymbol ReducedFromOrSelf(this IMethodSymbol methodSymbol)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            return methodSymbol.ReducedFrom ?? methodSymbol;
        }

        public static bool IsReducedExtensionMethod(this IMethodSymbol methodSymbol)
        {
            return methodSymbol?.MethodKind == MethodKind.ReducedExtension;
        }

        public static bool IsNonReducedExtensionMethod(this IMethodSymbol methodSymbol)
        {
            return methodSymbol?.IsExtensionMethod == true
                && methodSymbol.MethodKind != MethodKind.ReducedExtension;
        }
        #endregion IMethodSymbol

        #region IParameterSymbol
        public static bool IsParamsOf(this IParameterSymbol parameterSymbol, SpecialType elementType)
        {
            if (parameterSymbol == null)
                throw new ArgumentNullException(nameof(parameterSymbol));

            if (parameterSymbol.IsParams)
            {
                var arrayType = parameterSymbol.Type as IArrayTypeSymbol;
                return arrayType?.ElementType.SpecialType == elementType;
            }

            return false;
        }

        public static bool IsParamsOf(
            this IParameterSymbol parameterSymbol,
            SpecialType elementType1,
            SpecialType elementType2)
        {
            if (parameterSymbol == null)
                throw new ArgumentNullException(nameof(parameterSymbol));

            if (parameterSymbol.IsParams)
            {
                var arrayType = parameterSymbol.Type as IArrayTypeSymbol;

                return arrayType?.ElementType.IsSpecialType(elementType1, elementType2) == true;
            }

            return false;
        }

        public static bool IsParamsOf(
            this IParameterSymbol parameterSymbol,
            SpecialType elementType1,
            SpecialType elementType2,
            SpecialType elementType3)
        {
            if (parameterSymbol == null)
                throw new ArgumentNullException(nameof(parameterSymbol));

            if (parameterSymbol.IsParams)
            {
                var arrayType = parameterSymbol.Type as IArrayTypeSymbol;

                return arrayType?.ElementType.IsSpecialType(elementType1, elementType2, elementType3) == true;
            }

            return false;
        }

        public static bool IsRef(this IParameterSymbol parameterSymbol)
        {
            return parameterSymbol?.RefKind == RefKind.Ref;
        }

        public static bool IsOut(this IParameterSymbol parameterSymbol)
        {
            return parameterSymbol?.RefKind == RefKind.Out;
        }

        public static bool IsRefOrOut(this IParameterSymbol parameterSymbol)
        {
            return parameterSymbol?.RefKind.IsRefOrOut() == true;
        }
        #endregion IParameterSymbol

        #region IPropertySymbol
        internal static IPropertySymbol BaseOverriddenProperty(this IPropertySymbol propertySymbol)
        {
            if (propertySymbol == null)
                throw new ArgumentNullException(nameof(propertySymbol));

            while (true)
            {
                IPropertySymbol overriddenProperty = propertySymbol.OverriddenProperty;

                if (overriddenProperty == null)
                    return propertySymbol;

                propertySymbol = overriddenProperty;
            }
        }

        public static IEnumerable<IPropertySymbol> OverriddenProperties(this IPropertySymbol propertySymbol)
        {
            if (propertySymbol == null)
                throw new ArgumentNullException(nameof(propertySymbol));

            IPropertySymbol overriddenProperty = propertySymbol.OverriddenProperty;

            while (overriddenProperty != null)
            {
                yield return overriddenProperty;

                overriddenProperty = overriddenProperty.OverriddenProperty;
            }
        }
        #endregion IPropertySymbol

        #region INamedTypeSymbol
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

            return namedTypeSymbol.IsConstructedFrom(SpecialType.System_Nullable_T)
                && namedTypeSymbol.TypeArguments[0] == typeArgument;
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

        internal static bool IsConstructedFromImmutableArrayOfT(this INamedTypeSymbol namedTypeSymbol, SemanticModel semanticModel)
        {
            INamedTypeSymbol symbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Collections_Immutable_ImmutableArray_T);

            return symbol != null
                && namedTypeSymbol.ConstructedFrom.Equals(symbol);
        }

        public static bool IsIEnumerableOf(this INamedTypeSymbol namedTypeSymbol, ITypeSymbol typeArgument)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            if (typeArgument == null)
                throw new ArgumentNullException(nameof(typeArgument));

            return IsConstructedFrom(namedTypeSymbol, SpecialType.System_Collections_Generic_IEnumerable_T)
                && namedTypeSymbol.TypeArguments[0].Equals(typeArgument);
        }

        public static bool IsIEnumerableOf(this INamedTypeSymbol namedTypeSymbol, SpecialType specialTypeArgument)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            return IsConstructedFrom(namedTypeSymbol, SpecialType.System_Collections_Generic_IEnumerable_T)
                && namedTypeSymbol.TypeArguments[0].SpecialType == specialTypeArgument;
        }

        public static bool IsConstructedFromIEnumerableOfT(this INamedTypeSymbol namedTypeSymbol)
        {
            return IsConstructedFrom(namedTypeSymbol, SpecialType.System_Collections_Generic_IEnumerable_T);
        }

        internal static bool IsConstructedFromTaskOfT(this INamedTypeSymbol namedTypeSymbol, SemanticModel semanticModel)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return namedTypeSymbol
                .ConstructedFrom
                .EqualsOrInheritsFrom(semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T));
        }

        public static bool IsIEnumerableOrConstructedFromIEnumerableOfT(this INamedTypeSymbol namedTypeSymbol)
        {
            return IsIEnumerable(namedTypeSymbol)
                || IsConstructedFromIEnumerableOfT(namedTypeSymbol);
        }
        #endregion INamedTypeSymbol

        #region INamespaceSymbol
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
        #endregion INamespaceSymbol

        #region ITypeParameterSymbol
        internal static bool VerifyConstraint(
            this ITypeParameterSymbol typeParameterSymbol,
            bool allowReference,
            bool allowValueType,
            bool allowConstructor)
        {
            if (typeParameterSymbol == null)
                throw new ArgumentNullException(nameof(typeParameterSymbol));

            if (!CheckConstraint(typeParameterSymbol, allowReference, allowValueType, allowConstructor))
                return false;

            ImmutableArray<ITypeSymbol> constraintTypes = typeParameterSymbol.ConstraintTypes;

            if (!constraintTypes.Any())
                return true;

            var stack = new Stack<ITypeSymbol>(constraintTypes);

            while (stack.Count > 0)
            {
                ITypeSymbol type = stack.Pop();

                switch (type.TypeKind)
                {
                    case TypeKind.Class:
                        {
                            if (!allowReference)
                                return false;

                            break;
                        }
                    case TypeKind.Struct:
                        {
                            if (allowValueType)
                                return false;

                            break;
                        }
                    case TypeKind.Interface:
                        {
                            break;
                        }
                    case TypeKind.TypeParameter:
                        {
                            var typeParameterSymbol2 = (ITypeParameterSymbol)type;

                            if (!CheckConstraint(typeParameterSymbol2, allowReference, allowValueType, allowConstructor))
                                return false;

                            foreach (ITypeSymbol constraintType in typeParameterSymbol2.ConstraintTypes)
                                stack.Push(constraintType);

                            break;
                        }
                    case TypeKind.Error:
                        {
                            return false;
                        }
                    default:
                        {
                            Debug.Fail(type.TypeKind.ToString());
                            return false;
                        }
                }
            }

            return true;
        }

        private static bool CheckConstraint(
            ITypeParameterSymbol typeParameterSymbol,
            bool allowReference,
            bool allowValueType,
            bool allowConstructor)
        {
            return (allowReference || !typeParameterSymbol.HasReferenceTypeConstraint)
                && (allowValueType || !typeParameterSymbol.HasValueTypeConstraint)
                && (allowConstructor || !typeParameterSymbol.HasConstructorConstraint);
        }
        #endregion ITypeParameterSymbol

        #region ITypeSymbol
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

        public static bool IsVoid(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Void;
        }

        public static bool IsInt(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Int32;
        }

        public static bool IsBoolean(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Boolean;
        }

        public static bool IsString(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_String;
        }

        public static bool IsObject(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Object;
        }

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

        public static bool Implements(this ITypeSymbol typeSymbol, SpecialType specialType, bool allInterfaces = false)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<INamedTypeSymbol> interfaces = typeSymbol.GetInterfaces(allInterfaces);

            for (int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i].ConstructedFrom.SpecialType == specialType)
                    return true;
            }

            return false;
        }

        public static bool ImplementsAny(this ITypeSymbol typeSymbol, SpecialType specialType1, SpecialType specialType2, bool allInterfaces = false)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<INamedTypeSymbol> interfaces = typeSymbol.GetInterfaces(allInterfaces);

            for (int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i].ConstructedFrom.IsSpecialType(specialType1, specialType2))
                    return true;
            }

            return false;
        }

        public static bool ImplementsAny(this ITypeSymbol typeSymbol, SpecialType specialType1, SpecialType specialType2, SpecialType specialType3, bool allInterfaces = false)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<INamedTypeSymbol> interfaces = typeSymbol.GetInterfaces(allInterfaces);

            for (int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i].ConstructedFrom.IsSpecialType(specialType1, specialType2, specialType3))
                    return true;
            }

            return false;
        }

        public static bool Implements(this ITypeSymbol typeSymbol, ITypeSymbol interfaceSymbol, bool allInterfaces = false)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (interfaceSymbol != null)
            {
                ImmutableArray<INamedTypeSymbol> interfaces = typeSymbol.GetInterfaces(allInterfaces);

                for (int i = 0; i < interfaces.Length; i++)
                {
                    if (interfaces[i].Equals(interfaceSymbol))
                        return true;
                }
            }

            return false;
        }

        public static bool IsClass(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Class;
        }

        public static bool IsInterface(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Interface;
        }

        public static bool IsStruct(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Struct;
        }

        public static bool IsEnum(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Enum;
        }

        internal static bool IsEnumWithFlagsAttribute(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return IsEnumWithFlagsAttribute(typeSymbol, semanticModel.Compilation);
        }

        internal static bool IsEnumWithFlagsAttribute(this ITypeSymbol typeSymbol, Compilation compilation)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (compilation == null)
                throw new ArgumentNullException(nameof(compilation));

            return typeSymbol.IsEnum()
                && typeSymbol.HasAttribute(compilation.GetTypeByMetadataName(MetadataNames.System_FlagsAttribute));
        }

        public static bool IsDelegate(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Delegate;
        }

        public static bool SupportsExplicitDeclaration(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (typeSymbol.IsAnonymousType)
                return false;

            switch (typeSymbol.Kind)
            {
                case SymbolKind.TypeParameter:
                case SymbolKind.DynamicType:
                    {
                        return true;
                    }
                case SymbolKind.ArrayType:
                    {
                        return SupportsExplicitDeclaration(((IArrayTypeSymbol)typeSymbol).ElementType);
                    }
                case SymbolKind.NamedType:
                    {
                        var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                        if (typeSymbol.IsTupleType)
                        {
                            foreach (IFieldSymbol tupleElement in namedTypeSymbol.TupleElements)
                            {
                                if (!SupportsExplicitDeclaration(tupleElement.Type))
                                    return false;
                            }

                            return true;
                        }

                        return !IsAnyTypeArgumentAnonymousType(namedTypeSymbol);
                    }
            }

            return false;
        }

        public static bool IsConstructedFrom(this ITypeSymbol typeSymbol, SpecialType specialType)
        {
            return (typeSymbol as INamedTypeSymbol)?.ConstructedFrom.SpecialType == specialType;
        }

        public static bool IsConstructedFrom(this ITypeSymbol typeSymbol, SpecialType specialType1, SpecialType specialType2)
        {
            return (typeSymbol as INamedTypeSymbol)?.ConstructedFrom.IsSpecialType(specialType1, specialType2) == true;
        }

        public static bool IsConstructedFrom(this ITypeSymbol typeSymbol, SpecialType specialType1, SpecialType specialType2, SpecialType specialType3)
        {
            return (typeSymbol as INamedTypeSymbol)?.ConstructedFrom.IsSpecialType(specialType1, specialType2, specialType3) == true;
        }

        public static bool IsConstructedFrom(this ITypeSymbol typeSymbol, ISymbol symbol)
        {
            return typeSymbol?.IsNamedType() == true
                && ((INamedTypeSymbol)typeSymbol).ConstructedFrom?.Equals(symbol) == true;
        }

        internal static bool IsPredefinedValueType(this ITypeSymbol typeSymbol)
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
                    return true;
                default:
                    return false;
            }
        }

        internal static bool InheritsFromException(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return typeSymbol.IsClass()
                && typeSymbol.BaseType?.IsObject() == false
                && typeSymbol.InheritsFrom(semanticModel.GetTypeByMetadataName(MetadataNames.System_Exception));
        }

        internal static bool IsEventHandlerOrConstructedFromEventHandlerOfT(
            this ITypeSymbol typeSymbol,
            SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return typeSymbol.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_EventHandler))
                || typeSymbol.IsConstructedFrom(semanticModel.GetTypeByMetadataName(MetadataNames.System_EventHandler_T));
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

        public static bool IsTypeKind(this ITypeSymbol typeSymbol, TypeKind typeKind)
        {
            return typeSymbol?.TypeKind == typeKind;
        }

        public static bool IsTypeKind(this ITypeSymbol typeSymbol, TypeKind typeKind1, TypeKind typeKind2)
        {
            if (typeSymbol == null)
                return false;

            TypeKind typeKind = typeSymbol.TypeKind;

            return typeKind == typeKind1
                || typeKind == typeKind2;
        }

        public static bool IsTypeKind(this ITypeSymbol typeSymbol, TypeKind typeKind1, TypeKind typeKind2, TypeKind typeKind3)
        {
            if (typeSymbol == null)
                return false;

            TypeKind typeKind = typeSymbol.TypeKind;

            return typeKind == typeKind1
                || typeKind == typeKind2
                || typeKind == typeKind3;
        }

        public static bool IsSpecialType(this ITypeSymbol typeSymbol, SpecialType specialType)
        {
            return typeSymbol?.SpecialType == specialType;
        }

        public static bool IsSpecialType(this ITypeSymbol typeSymbol, SpecialType specialType1, SpecialType specialType2)
        {
            if (typeSymbol == null)
                return false;

            SpecialType specialType = typeSymbol.SpecialType;

            return specialType == specialType1
                || specialType == specialType2;
        }

        public static bool IsSpecialType(this ITypeSymbol typeSymbol, SpecialType specialType1, SpecialType specialType2, SpecialType specialType3)
        {
            if (typeSymbol == null)
                return false;

            SpecialType specialType = typeSymbol.SpecialType;

            return specialType == specialType1
                || specialType == specialType2
                || specialType == specialType3;
        }

        public static bool IsSpecialType(this ITypeSymbol typeSymbol, SpecialType specialType1, SpecialType specialType2, SpecialType specialType3, SpecialType specialType4)
        {
            if (typeSymbol == null)
                return false;

            SpecialType specialType = typeSymbol.SpecialType;

            return specialType == specialType1
                || specialType == specialType2
                || specialType == specialType3
                || specialType == specialType4;
        }

        public static bool IsSpecialType(this ITypeSymbol typeSymbol, SpecialType specialType1, SpecialType specialType2, SpecialType specialType3, SpecialType specialType4, SpecialType specialType5)
        {
            if (typeSymbol == null)
                return false;

            SpecialType specialType = typeSymbol.SpecialType;

            return specialType == specialType1
                || specialType == specialType2
                || specialType == specialType3
                || specialType == specialType4
                || specialType == specialType5;
        }

        public static bool IsSpecialType(this ITypeSymbol typeSymbol, SpecialType specialType1, SpecialType specialType2, SpecialType specialType3, SpecialType specialType4, SpecialType specialType5, SpecialType specialType6)
        {
            if (typeSymbol == null)
                return false;

            SpecialType specialType = typeSymbol.SpecialType;

            return specialType == specialType1
                || specialType == specialType2
                || specialType == specialType3
                || specialType == specialType4
                || specialType == specialType5
                || specialType == specialType6;
        }

        public static ISymbol FindMember(this ITypeSymbol typeSymbol, string name)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return typeSymbol.GetMembers(name).FirstOrDefault();
        }

        public static ISymbol FindMember(this ITypeSymbol typeSymbol, Func<ISymbol, bool> predicate)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<ISymbol> members = typeSymbol.GetMembers();

            for (int i = 0; i < members.Length; i++)
            {
                if (predicate(members[i]))
                    return members[i];
            }

            return default(ISymbol);
        }

        public static ISymbol FindMember(this ITypeSymbol typeSymbol, string name, Func<ISymbol, bool> predicate)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<ISymbol> members = typeSymbol.GetMembers(name);

            for (int i = 0; i < members.Length; i++)
            {
                if (predicate(members[i]))
                    return members[i];
            }

            return default(ISymbol);
        }

        public static IEventSymbol FindEvent(this ITypeSymbol typeSymbol, string name)
        {
            return (IEventSymbol)FindMember(typeSymbol, name, IsEvent);
        }

        public static IEventSymbol FindEvent(this ITypeSymbol typeSymbol, Func<IEventSymbol, bool> predicate)
        {
            return (IEventSymbol)FindMember(typeSymbol, f => f.IsEvent() && predicate((IEventSymbol)f));
        }

        public static IEventSymbol FindEvent(this ITypeSymbol typeSymbol, string name, Func<IEventSymbol, bool> predicate)
        {
            return (IEventSymbol)FindMember(typeSymbol, name, f => f.IsEvent() && predicate((IEventSymbol)f));
        }

        public static IFieldSymbol FindField(this ITypeSymbol typeSymbol, string name)
        {
            return (IFieldSymbol)FindMember(typeSymbol, name, IsField);
        }

        public static IFieldSymbol FindField(this ITypeSymbol typeSymbol, Func<IFieldSymbol, bool> predicate)
        {
            return (IFieldSymbol)FindMember(typeSymbol, f => f.IsField() && predicate((IFieldSymbol)f));
        }

        public static IFieldSymbol FindField(this ITypeSymbol typeSymbol, string name, Func<IFieldSymbol, bool> predicate)
        {
            return (IFieldSymbol)FindMember(typeSymbol, name, f => f.IsField() && predicate((IFieldSymbol)f));
        }

        public static IMethodSymbol FindMethod(this ITypeSymbol typeSymbol, string name)
        {
            return (IMethodSymbol)FindMember(typeSymbol, name, IsMethod);
        }

        public static IMethodSymbol FindMethod(this ITypeSymbol typeSymbol, Func<IMethodSymbol, bool> predicate)
        {
            return (IMethodSymbol)FindMember(typeSymbol, f => f.IsMethod() && predicate((IMethodSymbol)f));
        }

        public static IMethodSymbol FindMethod(this ITypeSymbol typeSymbol, string name, Func<IMethodSymbol, bool> predicate)
        {
            return (IMethodSymbol)FindMember(typeSymbol, name, f => f.IsMethod() && predicate((IMethodSymbol)f));
        }

        public static IPropertySymbol FindProperty(this ITypeSymbol typeSymbol, string name)
        {
            return (IPropertySymbol)FindMember(typeSymbol, name, IsProperty);
        }

        public static IPropertySymbol FindProperty(this ITypeSymbol typeSymbol, Func<IPropertySymbol, bool> predicate)
        {
            return (IPropertySymbol)FindMember(typeSymbol, f => f.IsProperty() && predicate((IPropertySymbol)f));
        }

        public static IPropertySymbol FindProperty(this ITypeSymbol typeSymbol, string name, Func<IPropertySymbol, bool> predicate)
        {
            return (IPropertySymbol)FindMember(typeSymbol, name, f => f.IsProperty() && predicate((IPropertySymbol)f));
        }

        public static bool ExistsMember(this ITypeSymbol typeSymbol, string name)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return typeSymbol.GetMembers(name).Any();
        }

        public static bool ExistsMember(this ITypeSymbol typeSymbol, Func<ISymbol, bool> predicate)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<ISymbol> members = typeSymbol.GetMembers();

            for (int i = 0; i < members.Length; i++)
            {
                if (predicate(members[i]))
                    return true;
            }

            return false;
        }

        public static bool ExistsMember(this ITypeSymbol typeSymbol, string name, Func<ISymbol, bool> predicate)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<ISymbol> members = typeSymbol.GetMembers(name);

            for (int i = 0; i < members.Length; i++)
            {
                if (predicate(members[i]))
                    return true;
            }

            return false;
        }

        public static bool ExistsEvent(this ITypeSymbol typeSymbol, Func<IEventSymbol, bool> predicate = null)
        {
            return (predicate != null)
                ? ExistsMember(typeSymbol, f => f.IsEvent() && predicate((IEventSymbol)f))
                : ExistsMember(typeSymbol, IsEvent);
        }

        public static bool ExistsEvent(this ITypeSymbol typeSymbol, string name, Func<IEventSymbol, bool> predicate = null)
        {
            return (predicate != null)
                ? ExistsMember(typeSymbol, name, f => f.IsEvent() && predicate((IEventSymbol)f))
                : ExistsMember(typeSymbol, name, IsEvent);
        }

        public static bool ExistsField(this ITypeSymbol typeSymbol, Func<IFieldSymbol, bool> predicate = null)
        {
            return (predicate != null)
                ? ExistsMember(typeSymbol, f => f.IsField() && predicate((IFieldSymbol)f))
                : ExistsMember(typeSymbol, IsField);
        }

        public static bool ExistsField(this ITypeSymbol typeSymbol, string name, Func<IFieldSymbol, bool> predicate = null)
        {
            return (predicate != null)
                ? ExistsMember(typeSymbol, name, f => f.IsField() && predicate((IFieldSymbol)f))
                : ExistsMember(typeSymbol, name, IsField);
        }

        public static bool ExistsMethod(this ITypeSymbol typeSymbol, Func<IMethodSymbol, bool> predicate = null)
        {
            return (predicate != null)
                ? ExistsMember(typeSymbol, f => f.IsMethod() && predicate((IMethodSymbol)f))
                : ExistsMember(typeSymbol, IsMethod);
        }

        public static bool ExistsMethod(this ITypeSymbol typeSymbol, string name, Func<IMethodSymbol, bool> predicate = null)
        {
            return (predicate != null)
                ? ExistsMember(typeSymbol, name, f => f.IsMethod() && predicate((IMethodSymbol)f))
                : ExistsMember(typeSymbol, name, IsMethod);
        }

        public static bool ExistsProperty(this ITypeSymbol typeSymbol, Func<IPropertySymbol, bool> predicate = null)
        {
            return (predicate != null)
                ? ExistsMember(typeSymbol, f => f.IsProperty() && predicate((IPropertySymbol)f))
                : ExistsMember(typeSymbol, IsProperty);
        }

        public static bool ExistsProperty(this ITypeSymbol typeSymbol, string name, Func<IPropertySymbol, bool> predicate = null)
        {
            return (predicate != null)
                ? ExistsMember(typeSymbol, name, f => f.IsProperty() && predicate((IPropertySymbol)f))
                : ExistsMember(typeSymbol, name, IsProperty);
        }

        internal static IFieldSymbol FindFieldWithConstantValue(this ITypeSymbol typeSymbol, int value)
        {
            foreach (ISymbol symbol in typeSymbol.GetMembers())
            {
                if (symbol.IsField())
                {
                    var fieldSymbol = (IFieldSymbol)symbol;

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
            }

            return null;
        }

        internal static bool IsTaskOrInheritsFromTask(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            INamedTypeSymbol taskSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task);

            return typeSymbol.EqualsOrInheritsFrom(taskSymbol);
        }

        internal static bool IsConstructedFromTaskOfT(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
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

        internal static bool IsConstructedFromImmutableArrayOfT(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol.Kind == SymbolKind.NamedType)
            {
                INamedTypeSymbol symbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Collections_Immutable_ImmutableArray_T);

                return symbol != null
                    && ((INamedTypeSymbol)typeSymbol).ConstructedFrom.Equals(symbol);
            }

            return false;
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
                    && namedTypeSymbol.TypeArguments[0].Equals(typeArgument);
            }

            return false;
        }

        public static bool IsIEnumerableOf(this ITypeSymbol typeSymbol, Func<ITypeSymbol, bool> typeArgumentPredicate)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (typeArgumentPredicate == null)
                throw new ArgumentNullException(nameof(typeArgumentPredicate));

            if (typeSymbol.IsNamedType())
            {
                var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                return namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T
                    && typeArgumentPredicate(namedTypeSymbol.TypeArguments[0]);
            }

            return false;
        }

        public static bool IsConstructedFromIEnumerableOfT(this ITypeSymbol typeSymbol)
        {
            return IsConstructedFrom(typeSymbol, SpecialType.System_Collections_Generic_IEnumerable_T);
        }

        public static bool IsIEnumerableOrConstructedFromIEnumerableOfT(this ITypeSymbol typeSymbol)
        {
            return IsIEnumerable(typeSymbol)
                || IsConstructedFromIEnumerableOfT(typeSymbol);
        }

        public static bool IsReferenceTypeOrNullableType(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.IsReferenceType == true
                || IsConstructedFrom(typeSymbol, SpecialType.System_Nullable_T);
        }

        internal static ImmutableArray<INamedTypeSymbol> GetInterfaces(this ITypeSymbol typeSymbol, bool allInterfaces)
        {
            return (allInterfaces) ? typeSymbol.AllInterfaces : typeSymbol.Interfaces;
        }
        #endregion ITypeSymbol
    }
}
