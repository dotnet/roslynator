// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using DotMarkdown;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation.Markdown
{
    public class DocusaurusDocumentationWriter : MarkdownDocumentationWriter
    {
        public DocusaurusDocumentationWriter(DocumentationContext context, MarkdownWriter writer) : base(context, writer)
        {
        }

        public override void WriteStartDocument(ISymbol symbol)
        {
            if (symbol != null)
                WriteSidebarLabel(symbol);
        }

        private void WriteSidebarLabel(ISymbol symbol)
        {
            string label = GetSidebarLabel(symbol);

            WriteRaw("---");
            WriteLine();
            WriteRaw("sidebar_label: ");
            WriteRaw(label);
            WriteLine();
            WriteRaw("---");
            WriteLine();
            WriteLine();
        }

        private string GetSidebarLabel(ISymbol symbol)
        {
            if (symbol.IsKind(SymbolKind.Namespace))
            {
                switch (Context.Options.Layout)
                {
                    case DocumentationLayout.FlatNamespaces:
                        return symbol.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces_GlobalNamespace_OmittedAsContaining);
                    case DocumentationLayout.Hierarchic:
                        return symbol.Name;
                    default:
                        throw new InvalidOperationException($"Unknown value '{Context.Options.Layout}'.");
                }
            }
            else if (symbol.IsKind(SymbolKind.NamedType))
            {
                string label = symbol.ToDisplayString(TypeSymbolDisplayFormats.Name_TypeParameters);

                if (symbol.ContainingType != null)
                {
                    label = symbol.ContainingType.ToDisplayString(TypeSymbolDisplayFormats.Name_TypeParameters)
                        + "."
                        + label;
                }

                return label;
            }
            else
            {
                string label = symbol.Name;

                if (symbol is IMethodSymbol methodSymbol)
                {
                    if (methodSymbol.MethodKind == MethodKind.Constructor)
                    {
                        label = symbol.ContainingType.ToDisplayString(TypeSymbolDisplayFormats.Name_TypeParameters);
                    }
                }
                else if (symbol.Kind == SymbolKind.Property
                    && ((IPropertySymbol)symbol).IsIndexer)
                {
                    label = "Item[]";
                }

                ISymbol explicitImplementation = symbol.GetFirstExplicitInterfaceImplementation();

                if (explicitImplementation != null)
                {
                    label = explicitImplementation.ContainingType.ToDisplayString(TypeSymbolDisplayFormats.Name_TypeParameters)
                        + "."
                        + label;
                }

                return label;
            }
        }
    }
}
