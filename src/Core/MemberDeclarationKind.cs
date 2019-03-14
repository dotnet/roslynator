// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    internal enum MemberDeclarationKind
    {
        None = 0,
        Const = 1,
        Field = 2,
        StaticConstructor = 3,
        Constructor = 4,
        Destructor = 5,
        Event = 6,
        Property = 7,
        Indexer = 8,
        Method = 9,
        ExplicitlyImplementedEvent = 10,
        ExplicitlyImplementedProperty = 11,
        ExplicitlyImplementedIndexer = 12,
        ExplicitlyImplementedMethod = 13,
        ConversionOperator = 14,
        Operator = 15
    }
}
