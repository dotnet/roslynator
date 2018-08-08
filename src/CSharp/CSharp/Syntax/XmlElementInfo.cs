// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about a <see cref="XmlElementSyntax"/> or <see cref="XmlEmptyElementSyntax"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct XmlElementInfo : IEquatable<XmlElementInfo>
    {
        private XmlElementInfo(XmlNodeSyntax element, string localName)
        {
            Element = element;
            LocalName = localName;
        }

        /// <summary>
        /// The xml element.
        /// </summary>
        public XmlNodeSyntax Element { get; }

        /// <summary>
        /// Local name of the element.
        /// </summary>
        public string LocalName { get; }

        /// <summary>
        /// Element kind.
        /// </summary>
        public SyntaxKind Kind
        {
            get { return Element?.Kind() ?? SyntaxKind.None; }
        }

        /// <summary>
        /// Determines whether the element is <see cref="SyntaxKind.XmlEmptyElement"/>.
        /// </summary>
        public bool IsEmptyElement
        {
            get { return Kind == SyntaxKind.XmlEmptyElement; }
        }

        internal bool IsContentEmptyOrWhitespace
        {
            get
            {
                if (!Success)
                    return false;

                if (IsEmptyElement)
                    return true;

                var element = (XmlElementSyntax)Element;

                SyntaxList<XmlNodeSyntax> content = element.Content;

                int count = content.Count;

                if (count == 0)
                    return true;

                if (count == 1)
                {
                    XmlNodeSyntax node = content[0];

                    if (node.IsKind(SyntaxKind.XmlText))
                    {
                        var xmlText = (XmlTextSyntax)node;

                        return xmlText.TextTokens.All(IsWhitespaceOrNewLine);
                    }
                }

                return false;

                bool IsWhitespaceOrNewLine(SyntaxToken token)
                {
                    switch (token.Kind())
                    {
                        case SyntaxKind.XmlTextLiteralNewLineToken:
                            return true;
                        case SyntaxKind.XmlTextLiteralToken:
                            return string.IsNullOrWhiteSpace(token.ValueText);
                        default:
                            return false;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return Element != null; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return SyntaxInfoHelpers.ToDebugString(Success, this, Element); }
        }

        internal XmlElementKind GetElementKind()
        {
            return XmlElementNameKindMapper.GetKindOrDefault(LocalName);
        }

        internal static XmlElementInfo Create(XmlNodeSyntax node)
        {
            switch (node)
            {
                case XmlElementSyntax element:
                    {
                        string localName = element.StartTag?.Name?.LocalName.ValueText;

                        if (localName == null)
                            return default;

                        return new XmlElementInfo(element, localName);
                    }
                case XmlEmptyElementSyntax element:
                    {
                        string localName = element.Name?.LocalName.ValueText;

                        if (localName == null)
                            return default;

                        return new XmlElementInfo(element, localName);
                    }
            }

            return default;
        }

        internal bool IsLocalName(string localName, StringComparison comparison = StringComparison.Ordinal)
        {
            return string.Equals(LocalName, localName, comparison);
        }

        internal bool IsLocalName(string localName1, string localName2, StringComparison comparison = StringComparison.Ordinal)
        {
            return IsLocalName(localName1, comparison)
                || IsLocalName(localName2, comparison);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Element?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is XmlElementInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(XmlElementInfo other)
        {
            return EqualityComparer<XmlNodeSyntax>.Default.Equals(Element, other.Element);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<XmlNodeSyntax>.Default.GetHashCode(Element);
        }

        public static bool operator ==(in XmlElementInfo info1, in XmlElementInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in XmlElementInfo info1, in XmlElementInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
