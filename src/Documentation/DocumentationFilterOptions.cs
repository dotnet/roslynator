// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation
{
    internal class DocumentationFilterOptions : SymbolFilterOptions
    {
        private static readonly MetadataName _defaultMemberAttribute = MetadataName.Parse("System.Reflection.DefaultMemberAttribute");

        public static ImmutableArray<MetadataName> IgnoredAttributes { get; } = GetIgnoredAttributes().Select(MetadataName.Parse).ToImmutableArray();

        public static DocumentationFilterOptions Instance { get; } = new DocumentationFilterOptions(
            visibility: VisibilityFilter.Public,
            symbolGroups: SymbolGroupFilter.TypeOrMember,
            rules: null,
            attributeRules: ImmutableArray.Create<AttributeFilterRule>(new IgnoredAttributeNameFilterRule(IgnoredAttributes)));

        private static string[] GetIgnoredAttributes()
        {
            return new string[]
            {
                "System.Diagnostics.CodeAnalysis.SuppressMessageAttribute",
                "System.Diagnostics.ConditionalAttribute",
                "System.Diagnostics.DebuggableAttribute",
                "System.Diagnostics.DebuggerBrowsableAttribute",
                "System.Diagnostics.DebuggerDisplayAttribute",
                "System.Diagnostics.DebuggerHiddenAttribute",
                "System.Diagnostics.DebuggerNonUserCodeAttribute",
                "System.Diagnostics.DebuggerStepperBoundaryAttribute",
                "System.Diagnostics.DebuggerStepThroughAttribute",
                "System.Diagnostics.DebuggerTypeProxyAttribute",
                "System.Diagnostics.DebuggerVisualizerAttribute",
                "System.Runtime.CompilerServices.AsyncIteratorStateMachineAttribute",
                "System.Runtime.CompilerServices.AsyncStateMachineAttribute",
                "System.Runtime.CompilerServices.CompilationRelaxationsAttribute",
                "System.Runtime.CompilerServices.CompilerGeneratedAttribute",
                "System.Runtime.CompilerServices.IsReadOnlyAttribute",
                "System.Runtime.CompilerServices.IteratorStateMachineAttribute",
                "System.Runtime.CompilerServices.MethodImplAttribute",
                "System.Runtime.CompilerServices.StateMachineAttribute",
                "System.Runtime.CompilerServices.TupleElementNamesAttribute",
                "System.Runtime.CompilerServices.TypeForwardedFromAttribute",
                "System.Runtime.CompilerServices.TypeForwardedToAttribute"
            };
        }

#if DEBUG
        private static readonly MetadataNameSet _knownVisibleAttributes = new MetadataNameSet(new string[]
        {
            "Microsoft.CodeAnalysis.CommitHashAttribute",
            "System.AttributeUsageAttribute",
            "System.CLSCompliantAttribute",
            "System.ComVisibleAttribute",
            "System.FlagsAttribute",
            "System.ObsoleteAttribute",
            "System.ComponentModel.DefaultValueAttribute",
            "System.ComponentModel.EditorBrowsableAttribute",
            "System.Composition.MetadataAttributeAttribute",
            "System.Reflection.AssemblyCompanyAttribute",
            "System.Reflection.AssemblyCopyrightAttribute",
            "System.Reflection.AssemblyDescriptionAttribute",
            "System.Reflection.AssemblyFileVersionAttribute",
            "System.Reflection.AssemblyInformationalVersionAttribute",
            "System.Reflection.AssemblyMetadataAttribute",
            "System.Reflection.AssemblyProductAttribute",
            "System.Reflection.AssemblyTitleAttribute",
            "System.Reflection.AssemblyTrademarkAttribute",
            "System.Runtime.CompilerServices.InternalImplementationOnlyAttribute",
            "System.Runtime.InteropServices.GuidAttribute",
            "System.Runtime.Versioning.TargetFrameworkAttribute",
            "System.Xml.Serialization.XmlArrayItemAttribute",
            "System.Xml.Serialization.XmlAttributeAttribute",
            "System.Xml.Serialization.XmlElementAttribute",
            "System.Xml.Serialization.XmlRootAttribute",
        });
#endif
        internal DocumentationFilterOptions(
            VisibilityFilter visibility = VisibilityFilter.All,
            SymbolGroupFilter symbolGroups = SymbolGroupFilter.TypeOrMember,
            IEnumerable<SymbolFilterRule> rules = null,
            IEnumerable<AttributeFilterRule> attributeRules = null) : base(visibility, symbolGroups, rules, attributeRules)
        {
        }

        public override SymbolFilterReason GetReason(ISymbol symbol, AttributeData attribute)
        {
            SymbolFilterReason reason = base.GetReason(symbol, attribute);

            if (reason != SymbolFilterReason.None)
                return reason;

            if (symbol.IsKind(SymbolKind.NamedType)
                && attribute.AttributeClass.HasMetadataName(_defaultMemberAttribute))
            {
                var namedType = (INamedTypeSymbol)symbol;

                if (namedType.GetMembers().Any(f => f.IsKind(SymbolKind.Property) && ((IPropertySymbol)f).IsIndexer))
                    return SymbolFilterReason.Ignored;
            }

#if DEBUG
            switch (attribute.AttributeClass.MetadataName)
            {
                case "FooAttribute":
                case "BarAttribute":
                    return SymbolFilterReason.None;
            }

            if (_knownVisibleAttributes.Contains(attribute.AttributeClass))
                return SymbolFilterReason.None;

            Debug.Fail(attribute.AttributeClass.ToDisplayString());
#endif
            return reason;
        }
    }
}
