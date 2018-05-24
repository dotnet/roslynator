// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;

namespace Roslynator
{
    internal static class FullyQualifiedMetadataNames
    {
        public static FullyQualifiedMetadataName System_Collections_Generic { get; } = new FullyQualifiedMetadataName(Namespaces.System_Collections, "Generic");
        public static FullyQualifiedMetadataName System_Collections_Generic_List_T { get; } = new FullyQualifiedMetadataName(Namespaces.System_Collections_Generic, "List`1");
        public static FullyQualifiedMetadataName System_Collections_Immutable_ImmutableArray_T { get; } = new FullyQualifiedMetadataName(Namespaces.System_Collections_Immutable, "ImmutableArray`1");
        public static FullyQualifiedMetadataName System_Func_T2 { get; } = new FullyQualifiedMetadataName(Namespaces.System, "Func`2");
        public static FullyQualifiedMetadataName System_Func_T3 { get; } = new FullyQualifiedMetadataName(Namespaces.System, "Func`3");
        public static FullyQualifiedMetadataName System_Linq_Enumerable { get; } = new FullyQualifiedMetadataName(Namespaces.System_Linq, "Enumerable");
        public static FullyQualifiedMetadataName System_Linq_ImmutableArrayExtensions { get; } = new FullyQualifiedMetadataName(Namespaces.System_Linq, "ImmutableArrayExtensions");

        private static class Namespaces
        {
            public static readonly ImmutableArray<string> System = ImmutableArray.Create("System");
            public static readonly ImmutableArray<string> System_Linq = ImmutableArray.Create("System", "Linq");
            public static readonly ImmutableArray<string> System_Collections = ImmutableArray.Create("System", "Collections");
            public static readonly ImmutableArray<string> System_Collections_Generic = ImmutableArray.Create("System", "Collections", "Generic");
            public static readonly ImmutableArray<string> System_Collections_Immutable = ImmutableArray.Create("System", "Collections", "Immutable");
        }
    }
}
