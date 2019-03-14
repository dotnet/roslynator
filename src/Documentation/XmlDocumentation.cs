// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    public sealed class XmlDocumentation
    {
        private const string DefaultIndentChars = "            ";

        private static readonly Regex _indentationRegex = new Regex("(?<=\n)" + DefaultIndentChars);

        private readonly XDocument _document;
        private readonly XElement _membersElement;
        private ImmutableDictionary<string, XElement> _elementsById;

        private XmlDocumentation(XDocument document)
        {
            _document = document;
            _membersElement = document.Root.Element("members");
        }

        public static XmlDocumentation Load(string filePath)
        {
            string rawXml = File.ReadAllText(filePath);

            rawXml = Unindent(rawXml);

            XDocument document = XDocument.Parse(rawXml, LoadOptions.PreserveWhitespace);

            return new XmlDocumentation(document);
        }

        public static string Unindent(string rawXml)
        {
            string s = "";

            using (var sr = new StringReader(rawXml))
            using (XmlReader xr = XmlReader.Create(sr))
            {
                if (xr.ReadToDescendant("member"))
                {
                    xr.ReadStartElement();

                    if (xr.NodeType == XmlNodeType.Whitespace)
                        s = xr.Value;
                }
            }

            int index = s.Length;

            for (int i = s.Length - 1; i >= 0; i--)
            {
                char ch = s[i];

                if (ch == '\n'
                    || ch == '\r')
                {
                    break;
                }

                index--;
            }

            if (index < s.Length)
            {
                rawXml = (string.CompareOrdinal(s, index, DefaultIndentChars, 0, DefaultIndentChars.Length) == 0)
                    ? _indentationRegex.Replace(rawXml, "")
                    : Regex.Replace(rawXml, "(?<=\n)" + s.Substring(index), "");
            }

            return rawXml;
        }

        public SymbolXmlDocumentation GetXmlDocumentation(ISymbol symbol)
        {
            return GetXmlDocumentation(symbol, symbol.GetDocumentationCommentId());
        }

        internal SymbolXmlDocumentation GetXmlDocumentation(ISymbol symbol, string commentId)
        {
            if (_elementsById == null)
            {
                Interlocked.CompareExchange(ref _elementsById, LoadElements(), null);
            }

            if (_elementsById.TryGetValue(commentId, out XElement element))
            {
                return new SymbolXmlDocumentation(symbol, element);
            }

            return null;

            ImmutableDictionary<string, XElement> LoadElements()
            {
                return _membersElement.Elements().ToImmutableDictionary(f => f.Attribute("name").Value, f => f);
            }
        }
    }
}
