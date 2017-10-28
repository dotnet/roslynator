// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public struct MethodInfo
    {
        internal MethodInfo(IMethodSymbol symbol, SemanticModel semanticModel)
            : this()
        {
            Symbol = symbol;
            SemanticModel = semanticModel;
        }

        public IMethodSymbol Symbol { get; }

        public SemanticModel SemanticModel { get; }

        public bool IsValid
        {
            get { return Symbol != null; }
        }

        public bool IsReturnType(SpecialType specialType)
        {
            return ReturnType.SpecialType == specialType;
        }

        public bool IsReturnType(string fullyQualifiedMetadataName)
        {
            return ReturnType.Equals(SemanticModel.GetTypeByMetadataName(fullyQualifiedMetadataName));
        }

        public bool IsContainingType(SpecialType specialType)
        {
            return ContainingType?.SpecialType == specialType;
        }

        public bool IsContainingType(string fullyQualifiedMetadataName)
        {
            return ContainingType?.Equals(SemanticModel.GetTypeByMetadataName(fullyQualifiedMetadataName)) == true;
        }

        public bool ReturnsObject
        {
            get { return IsReturnType(SpecialType.System_Object); }
        }

        public bool ReturnsBoolean
        {
            get { return IsReturnType(SpecialType.System_Boolean); }
        }

        public bool ReturnsInt
        {
            get { return IsReturnType(SpecialType.System_Int32); }
        }

        public bool ReturnsString
        {
            get { return IsReturnType(SpecialType.System_String); }
        }

        public bool ReturnsIEnumerableOf(ITypeSymbol typeArgument)
        {
            return ReturnType.IsIEnumerableOf(typeArgument);
        }

        public bool ReturnsIEnumerableOf(Func<ITypeSymbol, bool> typeArgumentPredicate)
        {
            return ReturnType.IsIEnumerableOf(typeArgumentPredicate);
        }

        internal bool IsName(string name)
        {
            return string.Equals(Name, name, StringComparison.Ordinal);
        }

        internal bool IsName(string name1, string name2)
        {
            return IsName(name1)
                || IsName(name2);
        }

        private bool IsNullOrName(string name)
        {
            return name == null || IsName(name);
        }

        internal bool HasParameter(SpecialType parameterType)
        {
            ImmutableArray<IParameterSymbol> parameters = Parameters;

            return parameters.Length == 1
                && parameters[0].Type.SpecialType == parameterType;
        }

        internal bool HasParameters(SpecialType firstParameterType, SpecialType secondParameterType)
        {
            ImmutableArray<IParameterSymbol> parameters = Parameters;

            return parameters.Length == 2
                && parameters[0].Type.SpecialType == firstParameterType
                && parameters[1].Type.SpecialType == secondParameterType;
        }

        public bool IsPublic
        {
            get { return DeclaredAccessibility == Accessibility.Public; }
        }

        public bool IsInternal
        {
            get { return DeclaredAccessibility == Accessibility.Internal; }
        }

        public bool IsProtected
        {
            get { return DeclaredAccessibility == Accessibility.Protected; }
        }

        public bool IsPrivate
        {
            get { return DeclaredAccessibility == Accessibility.Private; }
        }

        internal bool IsPublicStaticStringMethod(string name = null, bool isGenericMethod = false)
        {
            return IsNullOrName(name)
                && IsContainingType(SpecialType.System_String)
                && IsPublic
                && IsStatic
                && IsGenericMethod == isGenericMethod;
        }

        internal bool IsPublicStaticRegexMethod(string name = null, bool isGenericMethod = false)
        {
            return IsNullOrName(name)
                && IsContainingType(MetadataNames.System_Text_RegularExpressions_Regex)
                && IsPublic
                && IsStatic
                && IsGenericMethod == isGenericMethod;
        }

        internal bool IsPublicInstanceStringMethod(string name = null, bool isGenericMethod = false)
        {
            return IsNullOrName(name)
                && IsContainingType(SpecialType.System_String)
                && IsPublic
                && !IsStatic
                && IsGenericMethod == isGenericMethod;
        }

        internal bool IsLinqExtensionOfIEnumerableOfTWithoutParameters(
            string name,
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfT(name, parameterCount: 1, allowImmutableArrayExtension: allowImmutableArrayExtension);
        }

        internal bool IsLinqElementAt(
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfT("ElementAt", parameterCount: 2, allowImmutableArrayExtension: allowImmutableArrayExtension)
                && Parameters[1].Type.IsInt();
        }

        internal bool IsLinqWhere(
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfTWithPredicate("Where", parameterCount: 2, allowImmutableArrayExtension: allowImmutableArrayExtension);
        }

        internal bool IsLinqWhereWithIndex()
        {
            return IsLinqExtensionOfIEnumerableOfT("Where", parameterCount: 2, allowImmutableArrayExtension: false)
                && SymbolUtility.IsPredicateFunc(Parameters[1].Type, Symbol.TypeArguments[0], SemanticModel.Compilation.GetSpecialType(SpecialType.System_Int32), SemanticModel);
        }

        internal bool IsLinqSelect(bool allowImmutableArrayExtension = false)
        {
            if (IsValid
                && Symbol.IsPublic()
                && Symbol.ReturnType.IsConstructedFromIEnumerableOfT()
                && IsName("Select")
                && Symbol.Arity == 2)
            {
                INamedTypeSymbol containingType = Symbol.ContainingType;

                if (containingType != null)
                {
                    if (containingType.Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)))
                    {
                        ImmutableArray<IParameterSymbol> parameters = Parameters;

                        return parameters.Length == 2
                            && parameters[0].Type.IsConstructedFromIEnumerableOfT()
                            && SymbolUtility.IsFunc(parameters[1].Type, Symbol.TypeArguments[0], Symbol.TypeArguments[1], SemanticModel);
                    }
                    else if (allowImmutableArrayExtension
                        && containingType.Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions)))
                    {
                        ImmutableArray<IParameterSymbol> parameters = Parameters;

                        return parameters.Length == 2
                            && parameters[0].Type.IsConstructedFromImmutableArrayOfT(SemanticModel)
                            && SymbolUtility.IsFunc(parameters[1].Type, Symbol.TypeArguments[0], Symbol.TypeArguments[1], SemanticModel);
                    }
                }
            }

            return false;
        }

        internal bool IsLinqCast()
        {
            return IsValid
                && Symbol.IsPublic()
                && Symbol.ReturnType.IsConstructedFromIEnumerableOfT()
                && IsName("Cast")
                && Symbol.Arity == 1
                && Symbol.Parameters.SingleOrDefault(shouldThrow: false)?.Type.IsIEnumerable() == true
                && Symbol
                    .ContainingType?
                    .Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)) == true;
        }

        internal bool IsLinqExtensionOfIEnumerableOfT(
            string name = null,
            int parameterCount = -1,
            bool allowImmutableArrayExtension = false)
        {
            if (IsValid
                && Symbol.IsPublic()
                && IsNullOrName(name))
            {
                INamedTypeSymbol containingType = Symbol.ContainingType;

                if (containingType != null)
                {
                    if (containingType.Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)))
                    {
                        ImmutableArray<IParameterSymbol> parameters = Parameters;

                        return (parameterCount == -1 || parameters.Length == parameterCount)
                            && parameters[0].Type.IsConstructedFromIEnumerableOfT();
                    }
                    else if (allowImmutableArrayExtension
                        && containingType.Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions)))
                    {
                        ImmutableArray<IParameterSymbol> parameters = Parameters;

                        return (parameterCount == -1 || parameters.Length == parameterCount)
                            && parameters[0].Type.IsConstructedFromImmutableArrayOfT(SemanticModel);
                    }
                }
            }

            return false;
        }

        internal bool IsLinqExtensionOfIEnumerableOfTWithPredicate(
            string name = null,
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfTWithPredicate(name, parameterCount: 2, allowImmutableArrayExtension: allowImmutableArrayExtension);
        }

        private bool IsLinqExtensionOfIEnumerableOfTWithPredicate(
            string name,
            int parameterCount,
            bool allowImmutableArrayExtension = false)
        {
            if (IsValid
                && Symbol.IsPublic()
                && IsNullOrName(name))
            {
                INamedTypeSymbol containingType = Symbol.ContainingType;

                if (containingType != null)
                {
                    if (containingType.Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)))
                    {
                        ImmutableArray<IParameterSymbol> parameters = Parameters;

                        return parameters.Length == parameterCount
                            && parameters[0].Type.IsConstructedFromIEnumerableOfT()
                            && SymbolUtility.IsPredicateFunc(parameters[1].Type, Symbol.TypeArguments[0], SemanticModel);
                    }
                    else if (allowImmutableArrayExtension
                        && containingType.Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions)))
                    {
                        ImmutableArray<IParameterSymbol> parameters = Parameters;

                        return parameters.Length == parameterCount
                            && parameters[0].Type.IsConstructedFromImmutableArrayOfT(SemanticModel)
                            && SymbolUtility.IsPredicateFunc(parameters[1].Type, Symbol.TypeArguments[0], SemanticModel);
                    }
                }
            }

            return false;
        }

        #region IMethodSymbol

        public MethodKind MethodKind => Symbol.MethodKind;

        public int Arity => Symbol.Arity;

        public bool IsGenericMethod => Symbol.IsGenericMethod;

        public bool IsExtensionMethod => Symbol.IsExtensionMethod;

        public bool IsAsync => Symbol.IsAsync;

        public bool ReturnsVoid => Symbol.ReturnsVoid;

        public ITypeSymbol ReturnType => Symbol.ReturnType;

        public ImmutableArray<ITypeSymbol> TypeArguments => Symbol.TypeArguments;

        public ImmutableArray<ITypeParameterSymbol> TypeParameters => Symbol.TypeParameters;

        public ImmutableArray<IParameterSymbol> Parameters => Symbol.Parameters;

        public IMethodSymbol ConstructedFrom => Symbol.ConstructedFrom;

        public IMethodSymbol OverriddenMethod => Symbol.OverriddenMethod;

        public IMethodSymbol ReducedFrom => Symbol.ReducedFrom;

        public SymbolKind Kind => Symbol.Kind;

        public string Name => Symbol.Name;

        public string MetadataName => Symbol.MetadataName;

        public ISymbol ContainingSymbol => Symbol.ContainingSymbol;

        public INamedTypeSymbol ContainingType => Symbol.ContainingType;

        public INamespaceSymbol ContainingNamespace => Symbol.ContainingNamespace;

        public bool IsStatic => Symbol.IsStatic;

        public bool IsVirtual => Symbol.IsVirtual;

        public bool IsOverride => Symbol.IsOverride;

        public bool IsAbstract => Symbol.IsAbstract;

        public bool IsSealed => Symbol.IsSealed;

        public Accessibility DeclaredAccessibility => Symbol.DeclaredAccessibility;

        #endregion IMethodSymbol
    }
}
