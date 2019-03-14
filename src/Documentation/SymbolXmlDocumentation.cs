// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SymbolXmlDocumentation
    {
        private static readonly Regex _simpleElementRegex = new Regex(@"
(?<=^\<(?<name>\w+)\>)
\r?\n
([^\r\n]+)
\r?\n
(?=\</\k<name>\>)", RegexOptions.IgnorePatternWhitespace);

        private readonly XElement _element;

        internal static SymbolXmlDocumentation Default { get; } = new SymbolXmlDocumentation(null, null);

        public SymbolXmlDocumentation(ISymbol symbol, XElement element)
        {
            Symbol = symbol;
            _element = element;
        }

        public ISymbol Symbol { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{_element}"; }
        }

        public XElement Element(string name)
        {
            foreach (XElement element in _element.Elements())
            {
                if (string.Equals(element.Name.LocalName, name, StringComparison.OrdinalIgnoreCase))
                    return element;
            }

            return default;
        }

        internal XElement Element(string name, string attributeName, string attributeValue)
        {
            foreach (XElement element in _element.Elements())
            {
                if (string.Equals(element.Name.LocalName, name, StringComparison.OrdinalIgnoreCase)
                    && element.Attribute(attributeName)?.Value == attributeValue)
                {
                    return element;
                }
            }

            return default;
        }

        public IEnumerable<XElement> Elements(string name)
        {
            foreach (XElement element in _element.Elements())
            {
                if (string.Equals(element.Name.LocalName, name, StringComparison.OrdinalIgnoreCase))
                    yield return element;
            }
        }

        public bool HasElement(string name)
        {
            return Element(name) != null;
        }

        public IEnumerable<string> GetElementsAsText(bool skipEmptyElement = false, bool makeSingleLine = false)
        {
            foreach (XElement element in _element.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "code":
                    case "example":
                    case "list":
                    case "param":
                    case "remarks":
                    case "returns":
                    case "summary":
                    case "typeparam":
                    case "value":
                        {
                            if (skipEmptyElement
                                && string.IsNullOrWhiteSpace(element.Value))
                            {
                                break;
                            }

                            yield return GetElementXml(element);
                            break;
                        }
                    case "c":
                    case "para":
                        {
                            Debug.Fail(element.Name.LocalName);

                            if (skipEmptyElement
                                && string.IsNullOrWhiteSpace(element.Value))
                            {
                                break;
                            }

                            yield return GetElementXml(element);
                            break;
                        }
                    case "see":
                    case "paramref":
                    case "typeparamref":
                        {
                            Debug.Fail(element.Name.LocalName);

                            yield return GetElementXml(element);
                            break;
                        }

                    case "exception":
                    case "permission":
                    case "seealso":
                        {
                            yield return GetElementXml(element);
                            break;
                        }
                    default:
                        {
                            Debug.Fail(element.Name.LocalName);
                            break;
                        }
                }
            }

            string GetElementXml(XElement element)
            {
                string xml = element.ToString(SaveOptions.DisableFormatting).Trim();

                if (makeSingleLine)
                    xml = _simpleElementRegex.Replace(xml, "$1");

                return xml;
            }
        }
    }
}
