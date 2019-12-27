// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Roslynator.Metadata;
using Roslynator.Utilities;

namespace Roslynator.CodeGeneration
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            string rootPath = @"..\..\..\..\..";
#else
            string rootPath = System.Environment.CurrentDirectory;
#endif
            if (args?.Length > 0)
                rootPath = args[0];

            var metadata = new RoslynatorMetadata(rootPath);

            string path = Path.Combine(rootPath, "default.roslynator.config");

            string content = CreateDefaultConfigFile(metadata.Refactorings, metadata.CodeFixes);

            FileHelper.WriteAllText(path, content, Encoding.UTF8, onlyIfChanges: false, fileMustExists: false);
        }

        public static string CreateDefaultConfigFile(
            IEnumerable<RefactoringMetadata> refactorings,
            IEnumerable<CodeFixMetadata> codeFixes)
        {
            using (var stringWriter = new Utf8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter, new XmlWriterSettings() { Indent = true, IndentChars = "  " }))
                {
                    string newLineChars = writer.Settings.NewLineChars;
                    string indentChars = writer.Settings.IndentChars;

                    writer.WriteStartDocument();

                    writer.WriteStartElement("Roslynator");
                    writer.WriteStartElement("Settings");

                    writer.WriteStartElement("General");
                    writer.WriteElementString("PrefixFieldIdentifierWithUnderscore", "true");
                    writer.WriteEndElement();

                    writer.WriteStartElement("Refactorings");

                    foreach (RefactoringMetadata refactoring in refactorings
                        .Where(f => !f.IsObsolete)
                        .OrderBy(f => f.Id))
                    {
                        writer.WriteWhitespace(newLineChars);
                        writer.WriteWhitespace(indentChars);
                        writer.WriteWhitespace(indentChars);
                        writer.WriteStartElement("Refactoring");
                        writer.WriteAttributeString("Id", refactoring.Id);
                        writer.WriteAttributeString("IsEnabled", (refactoring.IsEnabledByDefault) ? "true" : "false");
                        writer.WriteEndElement();

                        writer.WriteWhitespace(" ");
                        writer.WriteComment($" {refactoring.Title} ");
                    }

                    writer.WriteWhitespace(newLineChars);
                    writer.WriteWhitespace(indentChars);
                    writer.WriteEndElement();

                    writer.WriteStartElement("CodeFixes");

                    foreach (CodeFixMetadata codeFix in codeFixes
                        .Where(f => !f.IsObsolete)
                        .OrderBy(f => f.Id))
                    {
                        writer.WriteWhitespace(newLineChars);
                        writer.WriteWhitespace(indentChars);
                        writer.WriteWhitespace(indentChars);
                        writer.WriteStartElement("CodeFix");
                        writer.WriteAttributeString("Id", codeFix.Id);
                        writer.WriteAttributeString("IsEnabled", (codeFix.IsEnabledByDefault) ? "true" : "false");
                        writer.WriteEndElement();

                        writer.WriteWhitespace(" ");
                        writer.WriteComment($" {codeFix.Title} (fixes {string.Join(", ", codeFix.FixableDiagnosticIds)}) ");
                    }

                    writer.WriteWhitespace(newLineChars);
                    writer.WriteWhitespace(indentChars);
                    writer.WriteEndElement();
                }

                return stringWriter.ToString();
            }
        }
    }
}
