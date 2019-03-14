// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    internal enum SymbolDefinitionPartFilter
    {
        None = 0,
        ContainingNamespace = 1,
        Attributes = 2,
        AssemblyAttributes = 4,
        AttributeArguments = 8,
        Accessibility = 16,
        Modifiers = 32,
        ParameterName = 64,
        ParameterDefaultValue = 128,
        BaseType = 256,
        BaseInterfaces = 512,
        BaseList = BaseType | BaseInterfaces,
        Constraints = 1024,
        TrailingSemicolon = 2048,
        TrailingComma = 4096,
        All = ContainingNamespace | Attributes | AssemblyAttributes | AttributeArguments | Accessibility | Modifiers | ParameterName | ParameterDefaultValue | BaseList | Constraints | TrailingSemicolon | TrailingComma
    }
}
