// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    public struct XmlElementInfo
    {
        private static XmlElementInfo Default { get; } = new XmlElementInfo();

        private XmlElementInfo(XmlNodeSyntax element, string localName, XmlElementKind elementKind)
        {
            Element = element;
            LocalName = localName;
            ElementKind = elementKind;
        }

        public XmlNodeSyntax Element { get; }

        public string LocalName { get; }

        public XmlElementKind ElementKind { get; }

        public SyntaxKind Kind
        {
            get { return Element?.Kind() ?? SyntaxKind.None; }
        }

        public bool IsXmlElement
        {
            get { return Kind == SyntaxKind.XmlElement; }
        }

        public bool IsXmlEmptyElement
        {
            get { return Kind == SyntaxKind.XmlEmptyElement; }
        }

        public bool Success
        {
            get { return Element != null; }
        }

        internal static XmlElementInfo Create(XmlNodeSyntax node)
        {
            switch (node)
            {
                case XmlElementSyntax element:
                    {
                        string localName = element.StartTag?.Name?.LocalName.ValueText;

                        if (localName == null)
                            return Default;

                        return new XmlElementInfo(element, localName, GetXmlElementKind(localName));
                    }
                case XmlEmptyElementSyntax element:
                    {
                        string localName = element.Name?.LocalName.ValueText;

                        if (localName == null)
                            return Default;

                        return new XmlElementInfo(element, localName, GetXmlElementKind(localName));
                    }
            }

            return Default;
        }

        private static XmlElementKind GetXmlElementKind(string localName)
        {
            switch (localName)
            {
                case "include":
                case "INCLUDE":
                    return XmlElementKind.Include;
                case "exclude":
                case "EXCLUDE":
                    return XmlElementKind.Exclude;
                case "inheritdoc":
                case "INHERITDOC":
                    return XmlElementKind.InheritDoc;
                case "summary":
                case "SUMMARY":
                    return XmlElementKind.Summary;
                case "exception":
                case "EXCEPTION":
                    return XmlElementKind.Exception;
                default:
                    return XmlElementKind.None;
            }
        }

        internal bool IsLocalName(string localName)
        {
            return string.Equals(LocalName, localName, StringComparison.Ordinal);
        }

        internal bool IsLocalName(string localName1, string localName2)
        {
            return IsLocalName(localName1)
                || IsLocalName(localName2);
        }
    }
}
