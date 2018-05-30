// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about a type parameter.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct TypeParameterInfo : IEquatable<TypeParameterInfo>
    {
        private TypeParameterInfo(
            TypeParameterSyntax typeParameter,
            TypeParameterListSyntax typeParameterList)
        {
            TypeParameter = typeParameter;
            TypeParameterList = typeParameterList;
        }

        /// <summary>
        /// The type parameter.
        /// </summary>
        public TypeParameterSyntax TypeParameter { get; }

        /// <summary>
        /// Type parameter list that contains this type parameter.
        /// </summary>
        public TypeParameterListSyntax TypeParameterList { get; }

        /// <summary>
        /// The type parameter name.
        /// </summary>
        public string Name
        {
            get { return TypeParameter?.Identifier.ValueText; }
        }

        /// <summary>
        /// A list of type parameters that contains this type parameter.
        /// </summary>
        public SeparatedSyntaxList<TypeParameterSyntax> TypeParameters
        {
            get { return TypeParameterList?.Parameters ?? default(SeparatedSyntaxList<TypeParameterSyntax>); }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return TypeParameter != null; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return SyntaxInfoHelpers.ToDebugString(Success, this, TypeParameter); }
        }

        internal static TypeParameterInfo Create(TypeParameterSyntax typeParameter)
        {
            if (!(typeParameter.Parent is TypeParameterListSyntax typeParameterList))
                return default;

            return new TypeParameterInfo(typeParameter, typeParameterList);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return TypeParameter?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is TypeParameterInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(TypeParameterInfo other)
        {
            return EqualityComparer<TypeParameterListSyntax>.Default.Equals(TypeParameterList, other.TypeParameterList);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<TypeParameterListSyntax>.Default.GetHashCode(TypeParameterList);
        }

        public static bool operator ==(in TypeParameterInfo info1, in TypeParameterInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in TypeParameterInfo info1, in TypeParameterInfo info2)
        {
            return !(info1 == info2);
        }
    }
}