// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings.MakeMemberReadOnly
{
    internal class UseReadOnlyAutoPropertyRefactoring : MakeMemberReadOnlyRefactoring
    {
        private UseReadOnlyAutoPropertyRefactoring()
        {
        }

        public static UseReadOnlyAutoPropertyRefactoring Instance { get; } = new UseReadOnlyAutoPropertyRefactoring();

        public override HashSet<ISymbol> GetAnalyzableSymbols(SymbolAnalysisContext context, INamedTypeSymbol containingType)
        {
            HashSet<ISymbol> properties = null;

            ImmutableArray<ISymbol> members = containingType.GetMembers();

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].IsProperty())
                {
                    var propertySymbol = (IPropertySymbol)members[i];

                    if (!propertySymbol.IsIndexer
                        && !propertySymbol.IsReadOnly
                        && !propertySymbol.IsImplicitlyDeclared
                        && propertySymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                        && !propertySymbol.HasAttribute(context.GetTypeByMetadataName(MetadataNames.System_Runtime_Serialization_DataMemberAttribute)))
                    {
                        IMethodSymbol setMethod = propertySymbol.SetMethod;

                        if (setMethod?.IsPrivate() == true
                            && setMethod.GetAttributes().IsDefaultOrEmpty
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
            return symbol?.IsProperty() == true;
        }

        public override void ReportFixableSymbols(SymbolAnalysisContext context, INamedTypeSymbol containingType, HashSet<ISymbol> symbols)
        {
            foreach (PropertyDeclarationSyntax node in symbols.Select(f => f.GetSyntax(context.CancellationToken)))
            {
                AccessorDeclarationSyntax setter = node.Setter();

                if (!setter.SpanContainsDirectives())
                    context.ReportDiagnostic(DiagnosticDescriptors.UseReadOnlyAutoProperty, setter);
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            PropertyDeclarationSyntax newNode = propertyDeclaration
                .RemoveNode(propertyDeclaration.Setter(), SyntaxRemoveOptions.KeepExteriorTrivia)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
        }
    }
}
