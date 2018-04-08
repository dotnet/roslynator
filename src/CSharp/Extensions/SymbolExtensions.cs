// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    /// <summary>
    /// A set of extension methods for <see cref="ISymbol"/> and its derived types.
    /// </summary>
    public static class SymbolExtensions
    {
        #region ISymbol
        internal static ISymbol FindFirstImplementedInterfaceMember(this ISymbol symbol, bool allInterfaces = false)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return FindFirstImplementedInterfaceMemberImpl(symbol, null, allInterfaces);
        }

        internal static ISymbol FindImplementedInterfaceMember(this ISymbol symbol, INamedTypeSymbol interfaceSymbol, bool allInterfaces = false)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (interfaceSymbol == null)
                throw new ArgumentNullException(nameof(interfaceSymbol));

            return FindFirstImplementedInterfaceMemberImpl(symbol, interfaceSymbol, allInterfaces);
        }

        private static ISymbol FindFirstImplementedInterfaceMemberImpl(this ISymbol symbol, INamedTypeSymbol interfaceSymbol, bool allInterfaces)
        {
            INamedTypeSymbol containingType = symbol.ContainingType;

            if (containingType != null)
            {
                ImmutableArray<INamedTypeSymbol> interfaces = containingType.GetInterfaces(allInterfaces);

                for (int i = 0; i < interfaces.Length; i++)
                {
                    if (interfaceSymbol == null
                        || interfaces[i].Equals(interfaceSymbol))
                    {
                        ImmutableArray<ISymbol> members = interfaces[i].GetMembers();

                        for (int j = 0; j < members.Length; j++)
                        {
                            if (symbol.Equals(containingType.FindImplementationForInterfaceMember(members[j])))
                                return members[j];
                        }
                    }
                }
            }

            return default(ISymbol);
        }

        /// <summary>
        /// Returns true if the the symbol implements any interface member.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="allInterfaces">If true, use <see cref="ITypeSymbol.AllInterfaces"/>, otherwise use <see cref="ITypeSymbol.Interfaces"/>.</param>
        /// <returns></returns>
        public static bool ImplementsInterfaceMember(this ISymbol symbol, bool allInterfaces = false)
        {
            return FindFirstImplementedInterfaceMember(symbol, allInterfaces) != null;
        }

        /// <summary>
        /// Returns true if the symbol implements any member of the specified interface.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interfaceSymbol"></param>
        /// <param name="allInterfaces">If true, use <see cref="ITypeSymbol.AllInterfaces"/>, otherwise use <see cref="ITypeSymbol.Interfaces"/>.</param>
        /// <returns></returns>
        public static bool ImplementsInterfaceMember(this ISymbol symbol, INamedTypeSymbol interfaceSymbol, bool allInterfaces = false)
        {
            return FindImplementedInterfaceMember(symbol, interfaceSymbol, allInterfaces) != null;
        }

        internal static TSymbol FindFirstImplementedInterfaceMember<TSymbol>(this ISymbol symbol, bool allInterfaces = false) where TSymbol : ISymbol
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return FindFirstImplementedInterfaceMemberImpl<TSymbol>(symbol, null, allInterfaces);
        }

        internal static TSymbol FindFirstImplementedInterfaceMember<TSymbol>(this ISymbol symbol, INamedTypeSymbol interfaceSymbol, bool allInterfaces = false) where TSymbol : ISymbol
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (interfaceSymbol == null)
                throw new ArgumentNullException(nameof(interfaceSymbol));

            return FindFirstImplementedInterfaceMemberImpl<TSymbol>(symbol, interfaceSymbol, allInterfaces);
        }

        private static TSymbol FindFirstImplementedInterfaceMemberImpl<TSymbol>(this ISymbol symbol, INamedTypeSymbol interfaceSymbol, bool allInterfaces = false) where TSymbol : ISymbol
        {
            INamedTypeSymbol containingType = symbol.ContainingType;

            if (containingType != null)
            {
                ImmutableArray<INamedTypeSymbol> interfaces = containingType.GetInterfaces(allInterfaces);

                for (int i = 0; i < interfaces.Length; i++)
                {
                    if (interfaceSymbol == null
                        || interfaces[i].Equals(interfaceSymbol))
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
            }

            return default(TSymbol);
        }

        /// <summary>
        /// Returns true if the symbol implements any interface member.
        /// </summary>
        /// <typeparam name="TSymbol"></typeparam>
        /// <param name="symbol"></param>
        /// <param name="allInterfaces">If true, use <see cref="ITypeSymbol.AllInterfaces"/>, otherwise use <see cref="ITypeSymbol.Interfaces"/>.</param>
        /// <returns></returns>
        public static bool ImplementsInterfaceMember<TSymbol>(this ISymbol symbol, bool allInterfaces = false) where TSymbol : ISymbol
        {
            return !EqualityComparer<TSymbol>.Default.Equals(
                FindFirstImplementedInterfaceMember<TSymbol>(symbol, allInterfaces),
                default(TSymbol));
        }

        /// <summary>
        /// Returns true if the symbol implements any member of the specified interface.
        /// </summary>
        /// <typeparam name="TSymbol"></typeparam>
        /// <param name="symbol"></param>
        /// <param name="interfaceSymbol"></param>
        /// <param name="allInterfaces">If true, use <see cref="ITypeSymbol.AllInterfaces"/>, otherwise use <see cref="ITypeSymbol.Interfaces"/>.</param>
        /// <returns></returns>
        public static bool ImplementsInterfaceMember<TSymbol>(this ISymbol symbol, INamedTypeSymbol interfaceSymbol, bool allInterfaces = false) where TSymbol : ISymbol
        {
            return !EqualityComparer<TSymbol>.Default.Equals(
                FindFirstImplementedInterfaceMember<TSymbol>(symbol, interfaceSymbol, allInterfaces),
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

        /// <summary>
        /// Returns true if the symbol is one of the specified kinds.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <returns></returns>
        public static bool IsKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2)
        {
            if (symbol == null)
                return false;

            SymbolKind kind = symbol.Kind;

            return kind == kind1
                || kind == kind2;
        }

        /// <summary>
        /// Returns true if the symbol is one of the specified kinds.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <returns></returns>
        public static bool IsKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2, SymbolKind kind3)
        {
            if (symbol == null)
                return false;

            SymbolKind kind = symbol.Kind;

            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        /// <summary>
        /// Returns true if the symbol is one of the specified kinds.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns true if the symbol is one of the specified kinds.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns true if the symbol represents an error.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static bool IsErrorType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.ErrorType;
        }

        /// <summary>
        /// Returns true if the symbol is an async method.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static bool IsAsyncMethod(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Method
                && ((IMethodSymbol)symbol).IsAsync;
        }

        internal static bool IsPropertyOfAnonymousType(this ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Property
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

        /// <summary>
        /// Get a value indicating whether the symbol has specified attribute.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="attributeSymbol"></param>
        /// <returns></returns>
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

        internal static ImmutableArray<IParameterSymbol> ParametersOrDefault(this ISymbol symbol)
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
                    return default(ImmutableArray<IParameterSymbol>);
            }
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

        internal static bool IsName(this ISymbol symbol, string name)
        {
            return StringUtility.Equals(symbol.Name, name);
        }

        internal static bool IsName(this ISymbol symbol, string name1, string name2)
        {
            return StringUtility.Equals(symbol.Name, name1, name2);
        }

        internal static bool IsContainingType(this ISymbol symbol, SpecialType specialType)
        {
            return symbol?.ContainingType?.SpecialType == specialType;
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

        internal static IEnumerable<IEventSymbol> OverriddenEvents(this IEventSymbol eventSymbol)
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
        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, bool value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is bool value2
                    && value == value2;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, char value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is char value2
                    && value == value2;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, sbyte value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is sbyte value2
                    && value == value2;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, byte value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is byte value2
                    && value == value2;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, short value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is short value2
                    && value == value2;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, ushort value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is ushort value2
                    && value == value2;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, int value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is int value2
                    && value == value2;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, uint value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is uint value2
                    && value == value2;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, long value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is long value2
                    && value == value2;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, ulong value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is ulong value2
                    && value == value2;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, decimal value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is decimal value2
                    && value == value2;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, float value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is float value2
                    && value == value2;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, double value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is double value2
                    && value == value2;
            }

            return false;
        }

        /// <summary>
        /// Get a value indicating whether the field symbol has specified constant value.
        /// </summary>
        /// <param name="fieldSymbol"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasConstantValue(this IFieldSymbol fieldSymbol, string value)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));

            if (fieldSymbol.HasConstantValue)
            {
                object constantValue = fieldSymbol.ConstantValue;

                return constantValue is string value2
                    && value == value2;
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

        internal static IEnumerable<IMethodSymbol> OverriddenMethods(this IMethodSymbol methodSymbol)
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

        /// <summary>
        /// If this method is a reduced extension method, returns the definition of extension method from which this was reduced. Otherwise, returns this symbol.
        /// </summary>
        /// <param name="methodSymbol"></param>
        /// <returns></returns>
        public static IMethodSymbol ReducedFromOrSelf(this IMethodSymbol methodSymbol)
        {
            return methodSymbol?.ReducedFrom ?? methodSymbol;
        }

        /// <summary>
        /// Returns true if this method is a reduced extension method.
        /// </summary>
        /// <param name="methodSymbol"></param>
        /// <returns></returns>
        public static bool IsReducedExtensionMethod(this IMethodSymbol methodSymbol)
        {
            return methodSymbol?.MethodKind == MethodKind.ReducedExtension;
        }

        /// <summary>
        /// Returns true if this method is an ordinary extension method (i.e. "this" parameter has not been removed).
        /// </summary>
        /// <param name="methodSymbol"></param>
        /// <returns></returns>
        public static bool IsOrdinaryExtensionMethod(this IMethodSymbol methodSymbol)
        {
            return methodSymbol?.IsExtensionMethod == true
                && methodSymbol.MethodKind == MethodKind.Ordinary;
        }

        internal static bool IsReturnType(this IMethodSymbol methodSymbol, SpecialType specialType)
        {
            return methodSymbol?.ReturnType.SpecialType == specialType;
        }

        internal static bool HasSingleParameter(this IMethodSymbol methodSymbol, SpecialType parameterType)
        {
            return methodSymbol.Parameters.SingleOrDefault(shouldThrow: false)?.Type.SpecialType == parameterType;
        }

        internal static bool HasTwoParameters(this IMethodSymbol methodSymbol, SpecialType firstParameterType, SpecialType secondParameterType)
        {
            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            return parameters.Length == 2
                && parameters[0].Type.SpecialType == firstParameterType
                && parameters[1].Type.SpecialType == secondParameterType;
        }
        #endregion IMethodSymbol

        #region IParameterSymbol
        /// <summary>
        /// Returns true if the parameter was declared as a parameter array that has a specified element type.
        /// </summary>
        /// <param name="parameterSymbol"></param>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public static bool IsParameterArrayOf(this IParameterSymbol parameterSymbol, SpecialType elementType)
        {
            return parameterSymbol?.IsParams == true
                && (parameterSymbol.Type as IArrayTypeSymbol)?.ElementType.SpecialType == elementType;
        }

        /// <summary>
        /// Returns true if the parameter was declared as a parameter array that has one of specified element types.
        /// </summary>
        /// <param name="parameterSymbol"></param>
        /// <param name="elementType1"></param>
        /// <param name="elementType2"></param>
        /// <returns></returns>
        public static bool IsParameterArrayOf(
            this IParameterSymbol parameterSymbol,
            SpecialType elementType1,
            SpecialType elementType2)
        {
            return parameterSymbol?.IsParams == true
                && (parameterSymbol.Type as IArrayTypeSymbol)?
                    .ElementType
                    .SpecialType
                    .Is(elementType1, elementType2) == true;
        }

        /// <summary>
        /// Returns true if the parameter was declared as a parameter array that has one of specified element types.
        /// </summary>
        /// <param name="parameterSymbol"></param>
        /// <param name="elementType1"></param>
        /// <param name="elementType2"></param>
        /// <param name="elementType3"></param>
        /// <returns></returns>
        public static bool IsParameterArrayOf(
            this IParameterSymbol parameterSymbol,
            SpecialType elementType1,
            SpecialType elementType2,
            SpecialType elementType3)
        {
            return parameterSymbol?.IsParams == true
                && (parameterSymbol.Type as IArrayTypeSymbol)?
                    .ElementType
                    .SpecialType
                    .Is(elementType1, elementType2, elementType3) == true;
        }

        /// <summary>
        /// Returns true if the parameter was declared as "ref" or "out" parameter.
        /// </summary>
        /// <param name="parameterSymbol"></param>
        /// <returns></returns>
        public static bool IsRefOrOut(this IParameterSymbol parameterSymbol)
        {
            if (parameterSymbol == null)
                throw new ArgumentNullException(nameof(parameterSymbol));

            RefKind refKind = parameterSymbol.RefKind;

            return refKind == RefKind.Ref
                || refKind == RefKind.Out;
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

        internal static IEnumerable<IPropertySymbol> OverriddenProperties(this IPropertySymbol propertySymbol)
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
        /// <summary>
        /// Returns true if the type is <see cref="Nullable{T}"/> and it has specified type argument.
        /// </summary>
        /// <param name="namedTypeSymbol"></param>
        /// <param name="specialType"></param>
        /// <returns></returns>
        public static bool IsNullableOf(this INamedTypeSymbol namedTypeSymbol, SpecialType specialType)
        {
            return namedTypeSymbol.IsNullableType()
                && namedTypeSymbol.TypeArguments[0].SpecialType == specialType;
        }

        /// <summary>
        /// Returns true if the type is <see cref="Nullable{T}"/> and it has specified type argument.
        /// </summary>
        /// <param name="namedTypeSymbol"></param>
        /// <param name="typeArgument"></param>
        /// <returns></returns>
        public static bool IsNullableOf(this INamedTypeSymbol namedTypeSymbol, ITypeSymbol typeArgument)
        {
            return namedTypeSymbol.IsNullableType()
                && namedTypeSymbol.TypeArguments[0] == typeArgument;
        }
        #endregion INamedTypeSymbol

        #region INamespaceSymbol
        internal static IEnumerable<INamespaceSymbol> ContainingNamespacesAndSelf(this INamespaceSymbol @namespace)
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

        #region ITypeSymbol
        /// <summary>
        /// Returns true if the type is <see cref="Nullable{T}"/> and it has specified type argument.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <param name="specialType"></param>
        /// <returns></returns>
        public static bool IsNullableOf(this ITypeSymbol typeSymbol, SpecialType specialType)
        {
            return (typeSymbol as INamedTypeSymbol)?.IsNullableOf(specialType) == true;
        }

        /// <summary>
        /// Returns true if the type is <see cref="Nullable{T}"/> and it has specified type argument.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <param name="typeArgument"></param>
        /// <returns></returns>
        public static bool IsNullableOf(this ITypeSymbol typeSymbol, ITypeSymbol typeArgument)
        {
            return (typeSymbol as INamedTypeSymbol)?.IsNullableOf(typeArgument) == true;
        }

        /// <summary>
        /// Returns true if the type is <see cref="Void"/>.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns></returns>
        public static bool IsVoid(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Void;
        }

        /// <summary>
        /// Returns true if the type is <see cref="string"/>.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns></returns>
        public static bool IsString(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_String;
        }

        /// <summary>
        /// Returns true if the type is <see cref="object"/>.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns></returns>
        public static bool IsObject(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Object;
        }

        /// <summary>
        /// Gets a list of base types of this type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<INamedTypeSymbol> BaseTypes(this ITypeSymbol type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            INamedTypeSymbol baseType = type.BaseType;

            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }

        /// <summary>
        /// Gets a list of base types of this type (including this type).
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns true if the type implements specified interface.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <param name="interfaceType"></param>
        /// <param name="allInterfaces">If true, use <see cref="ITypeSymbol.AllInterfaces"/>, otherwise use <see cref="ITypeSymbol.Interfaces"/>.</param>
        /// <returns></returns>
        public static bool Implements(this ITypeSymbol typeSymbol, SpecialType interfaceType, bool allInterfaces = false)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<INamedTypeSymbol> interfaces = typeSymbol.GetInterfaces(allInterfaces);

            for (int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i].ConstructedFrom.SpecialType == interfaceType)
                    return true;
            }

            return false;
        }

        internal static bool IsOrImplements(this ITypeSymbol typeSymbol, SpecialType interfaceType, bool allInterfaces = false)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return typeSymbol.SpecialType == interfaceType
                || typeSymbol.Implements(interfaceType, allInterfaces: allInterfaces);
        }

        /// <summary>
        /// Returns true if the type implements any of specified interfaces.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <param name="interfaceType1"></param>
        /// <param name="interfaceType2"></param>
        /// <param name="allInterfaces">If true, use <see cref="ITypeSymbol.AllInterfaces"/>, otherwise use <see cref="ITypeSymbol.Interfaces"/>.</param>
        /// <returns></returns>
        public static bool ImplementsAny(this ITypeSymbol typeSymbol, SpecialType interfaceType1, SpecialType interfaceType2, bool allInterfaces = false)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<INamedTypeSymbol> interfaces = typeSymbol.GetInterfaces(allInterfaces);

            for (int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i].ConstructedFrom.SpecialType.Is(interfaceType1, interfaceType2))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the type implements any of specified interfaces.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <param name="interfaceType1"></param>
        /// <param name="interfaceType2"></param>
        /// <param name="interfaceType3"></param>
        /// <param name="allInterfaces">If true, use <see cref="ITypeSymbol.AllInterfaces"/>, otherwise use <see cref="ITypeSymbol.Interfaces"/>.</param>
        /// <returns></returns>
        public static bool ImplementsAny(this ITypeSymbol typeSymbol, SpecialType interfaceType1, SpecialType interfaceType2, SpecialType interfaceType3, bool allInterfaces = false)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<INamedTypeSymbol> interfaces = typeSymbol.GetInterfaces(allInterfaces);

            for (int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i].ConstructedFrom.SpecialType.Is(interfaceType1, interfaceType2, interfaceType3))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the type implements specified interface.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <param name="interfaceSymbol"></param>
        /// <param name="allInterfaces">If true, use <see cref="ITypeSymbol.AllInterfaces"/>, otherwise use <see cref="ITypeSymbol.Interfaces"/>.</param>
        /// <returns></returns>
        public static bool Implements(this ITypeSymbol typeSymbol, INamedTypeSymbol interfaceSymbol, bool allInterfaces = false)
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

        internal static bool IsEnumWithFlags(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            return typeSymbol?.TypeKind == TypeKind.Enum
                && typeSymbol.HasAttribute(semanticModel.GetTypeByMetadataName(MetadataNames.System_FlagsAttribute));
        }

        /// <summary>
        /// Returns true if the type can be declared explicitly in a source code.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns></returns>
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

                        return !ContainsAnonymousType(namedTypeSymbol.TypeArguments);
                    }
            }

            return false;

            bool ContainsAnonymousType(ImmutableArray<ITypeSymbol> typeSymbols)
            {
                foreach (ITypeSymbol symbol in typeSymbols)
                {
                    if (symbol.IsAnonymousType)
                        return true;

                    if (symbol.Kind == SymbolKind.NamedType)
                    {
                        var namedTypeSymbol = (INamedTypeSymbol)symbol;

                        if (ContainsAnonymousType(namedTypeSymbol.TypeArguments))
                            return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Returns true if the type inherits from a specified base type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <param name="includeInterfaces"></param>
        /// <returns></returns>
        public static bool InheritsFrom(this ITypeSymbol type, ITypeSymbol baseType, bool includeInterfaces = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (baseType == null)
                return false;

            INamedTypeSymbol t = type.BaseType;

            while (t != null)
            {
                Debug.Assert(t.TypeKind.Is(TypeKind.Class, TypeKind.Error), t.TypeKind.ToString());

                if (t.Equals(baseType))
                    return true;

                t = t.BaseType;
            }

            if (includeInterfaces
                && baseType.TypeKind == TypeKind.Interface)
            {
                foreach (INamedTypeSymbol interfaceType in type.AllInterfaces)
                {
                    if (interfaceType.Equals(baseType))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the type is equal or inherits from a specified base type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <param name="includeInterfaces"></param>
        /// <returns></returns>
        public static bool EqualsOrInheritsFrom(this ITypeSymbol type, ITypeSymbol baseType, bool includeInterfaces = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.Equals(baseType)
                || InheritsFrom(type, baseType, includeInterfaces);
        }

        /// <summary>
        /// Searches for a member that matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members.
        /// </summary>
        /// <typeparam name="TSymbol"></typeparam>
        /// <param name="typeSymbol"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TSymbol FindMember<TSymbol>(this ITypeSymbol typeSymbol, Func<TSymbol, bool> predicate = null) where TSymbol : ISymbol
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return FindMemberImpl(typeSymbol.GetMembers(), predicate);
        }

        /// <summary>
        /// Searches for a member that has the specified name and matches the conditions defined by the specified predicate, if any, and returns the first occurrence within the type's members.
        /// </summary>
        /// <typeparam name="TSymbol"></typeparam>
        /// <param name="typeSymbol"></param>
        /// <param name="name"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TSymbol FindMember<TSymbol>(this ITypeSymbol typeSymbol, string name, Func<TSymbol, bool> predicate = null) where TSymbol : ISymbol
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return FindMemberImpl(typeSymbol.GetMembers(name), predicate);
        }

        private static TSymbol FindMemberImpl<TSymbol>(ImmutableArray<ISymbol> members, Func<TSymbol, bool> predicate) where TSymbol : ISymbol
        {
            if (predicate != null)
            {
                foreach (ISymbol symbol in members)
                {
                    if (symbol is TSymbol tsymbol
                        && predicate(tsymbol))
                    {
                        return tsymbol;
                    }
                }
            }
            else
            {
                foreach (ISymbol symbol in members)
                {
                    if (symbol is TSymbol tsymbol)
                        return tsymbol;
                }
            }

            return default(TSymbol);
        }

        /// <summary>
        /// Returns true if the type contains member that matches the conditions defined by the specified predicate, if any.
        /// </summary>
        /// <typeparam name="TSymbol"></typeparam>
        /// <param name="typeSymbol"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool ContainsMember<TSymbol>(this ITypeSymbol typeSymbol, Func<TSymbol, bool> predicate = null) where TSymbol : ISymbol
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return ContainsMemberImpl(typeSymbol.GetMembers(), predicate);
        }

        /// <summary>
        /// Returns true if the type contains member that has the specified name and matches the conditions defined by the specified predicate, if any.
        /// </summary>
        /// <typeparam name="TSymbol"></typeparam>
        /// <param name="typeSymbol"></param>
        /// <param name="name"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool ContainsMember<TSymbol>(this ITypeSymbol typeSymbol, string name, Func<TSymbol, bool> predicate = null) where TSymbol : ISymbol
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return ContainsMemberImpl(typeSymbol.GetMembers(name), predicate);
        }

        private static bool ContainsMemberImpl<TSymbol>(ImmutableArray<ISymbol> members, Func<TSymbol, bool> predicate) where TSymbol : ISymbol
        {
            if (predicate != null)
            {
                foreach (ISymbol symbol in members)
                {
                    if (symbol is TSymbol tsymbol
                        && predicate(tsymbol))
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (ISymbol symbol in members)
                {
                    if (symbol is TSymbol tsymbol)
                        return true;
                }
            }

            return false;
        }

        internal static IFieldSymbol FindFieldWithConstantValue(this ITypeSymbol typeSymbol, object value)
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

        internal static bool EqualsOrInheritsFromTaskOfT(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            return typeSymbol?.EqualsOrInheritsFrom(semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T)) == true;
        }

        /// <summary>
        /// Returns true if the type is <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns></returns>
        public static bool IsIEnumerableOfT(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T;
        }

        /// <summary>
        /// Returns true if the type is <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns></returns>
        public static bool IsIEnumerableOrIEnumerableOfT(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?
                .SpecialType
                .Is(SpecialType.System_Collections_IEnumerable, SpecialType.System_Collections_Generic_IEnumerable_T) == true;
        }

        /// <summary>
        /// Returns true if the type is a reference type or a nullable type.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns></returns>
        public static bool IsReferenceTypeOrNullableType(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.IsReferenceType == true
                || typeSymbol.IsNullableType();
        }

        /// <summary>
        /// Returns true if the type is a nullable type.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns></returns>
        public static bool IsNullableType(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
        }

        private static ImmutableArray<INamedTypeSymbol> GetInterfaces(this ITypeSymbol typeSymbol, bool allInterfaces)
        {
            return (allInterfaces) ? typeSymbol.AllInterfaces : typeSymbol.Interfaces;
        }
        #endregion ITypeSymbol
    }
}
