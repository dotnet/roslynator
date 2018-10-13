// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal static class DocumentationUtility
    {
        public static bool IsVisibleAttribute(INamedTypeSymbol attributeType)
        {
            return !IsNotVisibleAttribute(attributeType);
        }

        private static bool IsNotVisibleAttribute(INamedTypeSymbol attributeType)
        {
            switch (attributeType.MetadataName)
            {
                case "ConditionalAttribute":
                case "DebuggableAttribute":
                case "DebuggerBrowsableAttribute":
                case "DebuggerDisplayAttribute":
                case "DebuggerHiddenAttribute":
                case "DebuggerNonUserCodeAttribute":
                case "DebuggerStepperBoundaryAttribute":
                case "DebuggerStepThroughAttribute":
                case "DebuggerTypeProxyAttribute":
                case "DebuggerVisualizerAttribute":
                    return attributeType.ContainingNamespace.HasMetadataName(MetadataNames.System_Diagnostics);
                case "SuppressMessageAttribute":
                    return attributeType.ContainingNamespace.HasMetadataName(MetadataNames.System_Diagnostics_CodeAnalysis);
                case "DefaultMemberAttribute":
                case "AssemblyConfigurationAttribute":
                    return attributeType.ContainingNamespace.HasMetadataName(MetadataNames.System_Reflection);
                case "AsyncStateMachineAttribute":
                case "CompilationRelaxationsAttribute":
                case "CompilerGeneratedAttribute":
                case "IsReadOnlyAttribute":
                case "InternalsVisibleToAttribute":
                case "IteratorStateMachineAttribute":
                case "MethodImplAttribute":
                case "RuntimeCompatibilityAttribute":
                case "StateMachineAttribute":
                case "TupleElementNamesAttribute":
                case "TypeForwardedFromAttribute":
                case "TypeForwardedToAttribute":
                    return attributeType.ContainingNamespace.HasMetadataName(MetadataNames.System_Runtime_CompilerServices);
#if DEBUG
                case "AssemblyCompanyAttribute":
                case "AssemblyCopyrightAttribute":
                case "AssemblyDescriptionAttribute":
                case "AssemblyFileVersionAttribute":
                case "AssemblyInformationalVersionAttribute":
                case "AssemblyProductAttribute":
                case "AssemblyTitleAttribute":
                case "AttributeUsageAttribute":
                case "CLSCompliantAttribute":
                case "FlagsAttribute":
                case "FooAttribute":
                case "ObsoleteAttribute":
                case "TargetFrameworkAttribute":
                    return false;
#endif
            }

            Debug.Fail(attributeType.ToDisplayString());
            return false;
        }
    }
}
