// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    internal enum SymbolDefinitionPartFilter
    {
        None = 0,
        Assemblies = 1,
        ContainingNamespace = 1 << 1,
        ContainingNamespaceInTypeHierarchy = 1 << 2,
        Attributes = 1 << 3,
        AssemblyAttributes = 1 << 4,
        AttributeArguments = 1 << 5,
        Accessibility = 1 << 6,
        Modifiers = 1 << 7,
        ParameterName = 1 << 8,
        ParameterDefaultValue = 1 << 9,
        BaseType = 1 << 10,
        BaseInterfaces = 1 << 11,
        BaseList = BaseType | BaseInterfaces,
        Constraints = 1 << 12,
        TrailingSemicolon = 1 << 13,
        TrailingComma = 1 << 14,
        All = Assemblies | ContainingNamespace | ContainingNamespaceInTypeHierarchy | Attributes | AssemblyAttributes | AttributeArguments | Accessibility | Modifiers | ParameterName | ParameterDefaultValue | BaseType | BaseInterfaces | Constraints | TrailingSemicolon | TrailingComma
    }
}
