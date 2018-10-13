// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal readonly struct AttributeInfo
    {
        internal static IEqualityComparer<AttributeInfo> AttributeClassComparer { get; } = new AttributeClassEqualityComparer();

        public AttributeInfo(ISymbol target, AttributeData attributeData)
        {
            Target = target;
            AttributeData = attributeData;
        }

        public ISymbol Target { get; }

        public AttributeData AttributeData { get; }

        public INamedTypeSymbol AttributeClass
        {
            get { return AttributeData?.AttributeClass; }
        }

        public IMethodSymbol AttributeConstructor
        {
            get { return AttributeData?.AttributeConstructor; }
        }

        public SyntaxReference ApplicationSyntaxReference
        {
            get { return AttributeData?.ApplicationSyntaxReference; }
        }

        public ImmutableArray<TypedConstant> ConstructorArguments
        {
            get { return AttributeData?.ConstructorArguments ?? ImmutableArray<TypedConstant>.Empty; }
        }

        public ImmutableArray<KeyValuePair<string, TypedConstant>> NamedArguments
        {
            get { return AttributeData?.NamedArguments ?? ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty; }
        }

        private class AttributeClassEqualityComparer : EqualityComparer<AttributeInfo>
        {
            public override bool Equals(AttributeInfo x, AttributeInfo y)
            {
                return x.AttributeClass == y.AttributeClass;
            }

            public override int GetHashCode(AttributeInfo obj)
            {
                return obj.AttributeClass?.GetHashCode() ?? 0;
            }
        }
    }
}
