// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Roslynator.Extensions;

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

        public bool IsValid
        {
            get { return Symbol != null; }
        }

        public IMethodSymbol Symbol { get; }

        private SemanticModel SemanticModel { get; }

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

        public bool ReturnsChar
        {
            get { return IsReturnType(SpecialType.System_Char); }
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

        public bool ReturnsIEnumerableOf(SpecialType specialTypeArgument)
        {
            return ReturnType.IsIEnumerableOf(specialTypeArgument);
        }

        public bool HasName(string name)
        {
            return string.Equals(Name, name, StringComparison.Ordinal);
        }

        private bool IsNullOrName(string name)
        {
            return name == null || HasName(name);
        }

        public bool HasParameter(SpecialType parameterType)
        {
            ImmutableArray<IParameterSymbol> parameters = Parameters;

            return parameters.Length == 1
                && parameters[0].Type.SpecialType == parameterType;
        }

        public bool HasParameters(SpecialType firstParameterType, SpecialType secondParameterType)
        {
            ImmutableArray<IParameterSymbol> parameters = Parameters;

            return parameters.Length == 2
                && parameters[0].Type.SpecialType == firstParameterType
                && parameters[1].Type.SpecialType == secondParameterType;
        }

        public bool IsInstance
        {
            get { return !IsStatic; }
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

        public IMethodSymbol OriginalDefinition => Symbol.OriginalDefinition;

        public IMethodSymbol OverriddenMethod => Symbol.OverriddenMethod;

        public IMethodSymbol ReducedFrom => Symbol.ReducedFrom;

        public SymbolKind Kind => Symbol.Kind;

        public string Language => Symbol.Language;

        public string Name => Symbol.Name;

        public string MetadataName => Symbol.MetadataName;

        public ISymbol ContainingSymbol => Symbol.ContainingSymbol;

        public INamedTypeSymbol ContainingType => Symbol.ContainingType;

        public INamespaceSymbol ContainingNamespace => Symbol.ContainingNamespace;

        public bool IsDefinition => Symbol.IsDefinition;

        public bool IsStatic => Symbol.IsStatic;

        public bool IsVirtual => Symbol.IsVirtual;

        public bool IsOverride => Symbol.IsOverride;

        public bool IsAbstract => Symbol.IsAbstract;

        public bool IsSealed => Symbol.IsSealed;

        public Accessibility DeclaredAccessibility => Symbol.DeclaredAccessibility;

        #endregion

        public bool IsPublicStaticStringMethod(string name = null, bool isGenericMethod = false)
        {
            return IsNullOrName(name)
                && IsContainingType(SpecialType.System_String)
                && IsPublic
                && IsStatic
                && IsGenericMethod == isGenericMethod;
        }

        public bool IsPublicInstanceStringMethod(string name = null, bool isGenericMethod = false)
        {
            return IsNullOrName(name)
                && IsContainingType(SpecialType.System_String)
                && IsPublic
                && IsInstance
                && IsGenericMethod == isGenericMethod;
        }
    }
}
