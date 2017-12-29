// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CodeGeneration
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
#if DEBUG
                args = new string[] { @"..\..\..\.." };
#else
                args = new string[] { Environment.CurrentDirectory };
#endif
            }

            string dirPath = args[0];

            StringComparer comparer = StringComparer.InvariantCulture;

            SortRefactoringsAndAddMissingIds(Path.Combine(dirPath, @"Refactorings\Refactorings.xml"), comparer);

            var generator = new MetadataGenerator(dirPath, comparer);

            generator.Generate();

            generator.FindFilesToDelete();

            generator.FindMissingSamples();
        }

        public static void SortRefactoringsAndAddMissingIds(string filePath, IComparer<string> comparer)
        {
            XDocument doc = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);

            XElement root = doc.Root;

            IEnumerable<XElement> newElements = root
                .Elements()
                .OrderBy(f => f.Attribute("Identifier").Value, comparer);

            if (newElements.Any(f => f.Attribute("Id") == null))
            {
                int maxValue = newElements.Where(f => f.Attribute("Id") != null)
                    .Select(f => int.Parse(f.Attribute("Id").Value.Substring(2)))
                    .DefaultIfEmpty()
                    .Max();

                int idNumber = maxValue + 1;

                newElements = newElements.Select(f =>
                {
                    if (f.Attribute("Id") != null)
                    {
                        return f;
                    }
                    else
                    {
                        string id = RefactoringIdentifiers.Prefix + idNumber.ToString().PadLeft(4, '0');
                        f.ReplaceAttributes(new XAttribute("Id", id), f.Attributes());
                        idNumber++;
                        return f;
                    }
                });
            }

            newElements = newElements.OrderBy(f => f.Attribute("Id").Value, comparer);

            root.ReplaceAll(newElements);

            doc.Save(filePath);
        }
    }
}
