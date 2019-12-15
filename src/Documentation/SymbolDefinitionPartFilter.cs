// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    internal enum SymbolDefinitionPartFilter
    {
        None = 0,
        ContainingNamespace = 1,
        ContainingNamespaceInTypeHierarchy = 2,
        Attributes = 4,
        AssemblyAttributes = 8,
        AttributeArguments = 16,
        Accessibility = 32,
        Modifiers = 64,
        ParameterName = 128,
        ParameterDefaultValue = 256,
        BaseType = 512,
        BaseInterfaces = 1024,
        BaseList = BaseType | BaseInterfaces,
        Constraints = 2048,
        TrailingSemicolon = 4096,
        TrailingComma = 8192,
        All = ContainingNamespace | ContainingNamespaceInTypeHierarchy | Attributes | AssemblyAttributes | AttributeArguments | Accessibility | Modifiers | ParameterName | ParameterDefaultValue | BaseType | BaseInterfaces | Constraints | TrailingSemicolon | TrailingComma
    }
}
