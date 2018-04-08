// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis.MakeMemberReadOnly
{
    internal class UseReadOnlyAutoPropertyAnalysis : MakeMemberReadOnlyAnalysis
    {
        private UseReadOnlyAutoPropertyAnalysis()
        {
        }

        public static UseReadOnlyAutoPropertyAnalysis Instance { get; } = new UseReadOnlyAutoPropertyAnalysis();

        public override HashSet<ISymbol> GetAnalyzableSymbols(SymbolAnalysisContext context, INamedTypeSymbol containingType)
        {
            HashSet<ISymbol> properties = null;

            ImmutableArray<ISymbol> members = containingType.GetMembers();

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Kind == SymbolKind.Property)
                {
                    var propertySymbol = (IPropertySymbol)members[i];

                    if (!propertySymbol.IsIndexer
                        && !propertySymbol.IsReadOnly
                        && !propertySymbol.IsImplicitlyDeclared
                        && propertySymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                        && !propertySymbol.HasAttribute(context.GetTypeByMetadataName(MetadataNames.System_Runtime_Serialization_DataMemberAttribute)))
                    {
                        IMethodSymbol setMethod = propertySymbol.SetMethod;

                        if (setMethod?.DeclaredAccessibility == Accessibility.Private
                            && setMethod.GetAttributes().IsEmpty
                            && setMethod.GetSyntaxOrDefault(context.CancellationToken) is AccessorDeclarationSyntax accessor
                            && accessor.BodyOrExpressionBody() == null)
                        {
                            (properties ?? (properties = new HashSet<ISymbol>())).Add(propertySymbol);
                        }
                    }
                }
            }

            return properties;
        }

        protected override bool ValidateSymbol(ISymbol symbol)
        {
            return symbol?.Kind == SymbolKind.Property;
        }

        public override void ReportFixableSymbols(SymbolAnalysisContext context, INamedTypeSymbol containingType, HashSet<ISymbol> symbols)
        {
            foreach (PropertyDeclarationSyntax node in symbols.Select(f => (PropertyDeclarationSyntax)f.GetSyntax(context.CancellationToken)))
            {
                AccessorDeclarationSyntax setter = node.Setter();

                if (!setter.SpanContainsDirectives())
                    context.ReportDiagnostic(DiagnosticDescriptors.UseReadOnlyAutoProperty, setter);
            }
        }
    }
}
