// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal static class XmlExtensions
    {
        public static void WriteContentTo(this XElement element, DocumentationWriter writer, bool inlineOnly = false)
        {
            using (IEnumerator<XNode> en = element.Nodes().GetEnumerator())
            {
                if (en.MoveNext())
                {
                    XNode node = null;

                    bool isFirst = true;
                    bool isLast = false;

                    do
                    {
                        node = en.Current;

                        isLast = !en.MoveNext();

                        if (node is XText t)
                        {
                            string value = t.Value;
                            value = TextUtility.RemoveLeadingTrailingNewLine(value, isFirst, isLast);

                            if (inlineOnly)
                                value = TextUtility.ToSingleLine(value);

                            writer.WriteString(value);
                        }
                        else if (node is XElement e)
                        {
                            switch (XmlTagMapper.GetTagOrDefault(e.Name.LocalName))
                            {
                                case XmlTag.C:
                                    {
                                        string value = e.Value;
                                        value = TextUtility.ToSingleLine(value);
                                        writer.WriteInlineCode(value);
                                        break;
                                    }
                                case XmlTag.Code:
                                    {
                                        if (inlineOnly)
                                            break;

                                        string value = e.Value;
                                        value = TextUtility.RemoveLeadingTrailingNewLine(value);

                                        writer.WriteCodeBlock(value);

                                        break;
                                    }
                                case XmlTag.List:
                                    {
                                        if (inlineOnly)
                                            break;

                                        string type = e.Attribute("type")?.Value;

                                        if (!string.IsNullOrEmpty(type))
                                        {
                                            switch (type)
                                            {
                                                case "bullet":
                                                    {
                                                        WriteList(writer, e.Elements());
                                                        break;
                                                    }
                                                case "number":
                                                    {
                                                        WriteList(writer, e.Elements(), isOrdered: true);
                                                        break;
                                                    }
                                                case "table":
                                                    {
                                                        WriteTable(writer, e.Elements());
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        Debug.Fail(type);
                                                        break;
                                                    }
                                            }
                                        }

                                        break;
                                    }
                                case XmlTag.Para:
                                    {
                                        writer.WriteLine();
                                        writer.WriteLine();
                                        WriteContentTo(e, writer);
                                        writer.WriteLine();
                                        writer.WriteLine();
                                        break;
                                    }
                                case XmlTag.ParamRef:
                                    {
                                        string parameterName = e.Attribute("name")?.Value;

                                        if (parameterName != null)
                                            writer.WriteBold(parameterName);

                                        break;
                                    }
                                case XmlTag.See:
                                    {
                                        string commentId = e.Attribute("cref")?.Value;

                                        if (commentId != null)
                                        {
                                            ISymbol symbol = writer.DocumentationModel.GetFirstSymbolForDeclarationId(commentId);

                                            //XTODO: repair roslyn documentation
                                            Debug.Assert(symbol != null
                                                || commentId == "T:Microsoft.CodeAnalysis.CSharp.SyntaxNode"
                                                || commentId == "T:Microsoft.CodeAnalysis.CSharp.SyntaxToken"
                                                || commentId == "T:Microsoft.CodeAnalysis.CSharp.SyntaxTrivia"
                                                || commentId == "T:Microsoft.CodeAnalysis.VisualBasic.SyntaxNode"
                                                || commentId == "T:Microsoft.CodeAnalysis.VisualBasic.SyntaxToken"
                                                || commentId == "T:Microsoft.CodeAnalysis.VisualBasic.SyntaxTrivia", commentId);

                                            if (symbol != null)
                                            {
                                                writer.WriteLink(symbol, SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters, SymbolDisplayAdditionalMemberOptions.UseItemPropertyName | SymbolDisplayAdditionalMemberOptions.UseOperatorName);
                                            }
                                            else
                                            {
                                                writer.WriteBold(TextUtility.RemovePrefixFromDocumentationCommentId(commentId));
                                            }
                                        }

                                        break;
                                    }
                                case XmlTag.TypeParamRef:
                                    {
                                        string typeParameterName = e.Attribute("name")?.Value;

                                        if (typeParameterName != null)
                                            writer.WriteBold(typeParameterName);

                                        break;
                                    }
                                case XmlTag.Example:
                                case XmlTag.Exception:
                                case XmlTag.Exclude:
                                case XmlTag.Include:
                                case XmlTag.InheritDoc:
                                case XmlTag.Param:
                                case XmlTag.Permission:
                                case XmlTag.Remarks:
                                case XmlTag.Returns:
                                case XmlTag.SeeAlso:
                                case XmlTag.Summary:
                                case XmlTag.TypeParam:
                                case XmlTag.Value:
                                    {
                                        break;
                                    }
                                default:
                                    {
                                        Debug.Fail(e.Name.LocalName);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            Debug.Fail(node.NodeType.ToString());
                        }

                        isFirst = false;
                    }
                    while (!isLast);
                }
            }
        }

        private static void WriteList(DocumentationWriter writer, IEnumerable<XElement> elements, bool isOrdered = false)
        {
            int number = 1;

            using (IEnumerator<XElement> en = Iterator().GetEnumerator())
            {
                if (en.MoveNext())
                {
                    if (isOrdered)
                    {
                        writer.WriteStartOrderedList();
                    }
                    else
                    {
                        writer.WriteStartBulletList();
                    }

                    do
                    {
                        WriteStartItem();
                        WriteContentTo(en.Current, writer, inlineOnly: true);
                        WriteEndItem();
                    }
                    while (en.MoveNext());

                    if (isOrdered)
                    {
                        writer.WriteEndOrderedList();
                    }
                    else
                    {
                        writer.WriteEndBulletList();
                    }
                }
            }

            IEnumerable<XElement> Iterator()
            {
                foreach (XElement element in elements)
                {
                    if (element.Name.LocalName == "item")
                    {
                        using (IEnumerator<XElement> en = element.Elements().GetEnumerator())
                        {
                            if (en.MoveNext())
                            {
                                XElement element2 = en.Current;

                                if (element2.Name.LocalName == "description")
                                {
                                    yield return element2;
                                }
                            }
                            else
                            {
                                yield return element;
                            }
                        }
                    }
                }
            }

            void WriteStartItem()
            {
                if (isOrdered)
                {
                    writer.WriteStartOrderedItem(number);
                    number++;
                }
                else
                {
                    writer.WriteStartBulletItem();
                }
            }

            void WriteEndItem()
            {
                if (isOrdered)
                {
                    writer.WriteEndOrderedItem();
                }
                else
                {
                    writer.WriteEndBulletItem();
                }
            }
        }

        private static void WriteTable(DocumentationWriter writer, IEnumerable<XElement> elements)
        {
            using (IEnumerator<XElement> en = elements.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    XElement element = en.Current;

                    string name = element.Name.LocalName;

                    if (name == "listheader"
                        && en.MoveNext())
                    {
                        int columnCount = element.Elements().Count();

                        writer.WriteStartTable(columnCount);
                        writer.WriteStartTableRow();

                        foreach (XElement element2 in element.Elements())
                        {
                            writer.WriteStartTableCell();
                            WriteContentTo(element2, writer, inlineOnly: true);
                            writer.WriteEndTableCell();
                        }

                        writer.WriteEndTableRow();
                        writer.WriteTableHeaderSeparator();

                        do
                        {
                            element = en.Current;

                            writer.WriteStartTableRow();

                            int count = 0;
                            foreach (XElement element2 in element.Elements())
                            {
                                writer.WriteStartTableCell();
                                WriteContentTo(element2, writer, inlineOnly: true);
                                writer.WriteEndTableCell();
                                count++;

                                if (count == columnCount)
                                    break;
                            }

                            while (count < columnCount)
                            {
                                writer.WriteTableCell(null);
                                count++;
                            }

                            writer.WriteEndTableRow();
                        }
                        while (en.MoveNext());

                        writer.WriteEndTable();
                    }
                }
            }
        }
    }
}
