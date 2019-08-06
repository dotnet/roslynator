// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using static Pihrtsoft.Text.RegularExpressions.Linq.Patterns;

namespace VersionUpdater
{
    internal static class Program
    {
        private const string VsixXmlNamespace = "http://schemas.microsoft.com/developer/vsx-schema/2011";

        private static readonly Regex _assemblyVersionRegex = (
            AssertBack("[assembly: AssemblyVersion(\"")
            + Digits() + "." + Digits() + "." + Digits() + "." + Digits()
            + Assert("\")]")
        ).ToRegex();

        private static void Main(string[] args)
        {
            if (args == null)
                return;

            if (args.Length != 1)
                return;

            if (!Version.TryParse(args[0], out Version version))
                return;

            UpdateVersionInCsProj(@"..\src\Analyzers\Analyzers.csproj", version);
            UpdateVersionInCsProj(@"..\src\Analyzers.CodeFixes\Analyzers.CodeFixes.csproj", version);
            UpdateVersionInCsProj(@"..\src\CodeFixes\CodeFixes.csproj", version);
            UpdateVersionInCsProj(@"..\src\Common\Common.csproj", version);
            UpdateVersionInCsProj(@"..\src\Workspaces.Common\Workspaces.Common.csproj", version);
            UpdateVersionInCsProj(@"..\src\Refactorings\Refactorings.csproj", version);

            UpdateVersionInAssemblyInfo(@"..\src\VisualStudio\Properties\AssemblyInfo.cs", version);
            UpdateVersionInAssemblyInfo(@"..\src\VisualStudio.Common\Properties\AssemblyInfo.cs", version);
            UpdateVersionInAssemblyInfo(@"..\src\VisualStudio.Refactorings\Properties\AssemblyInfo.cs", version);

            UpdateVersionInVsixManifest(@"..\src\VisualStudio\source.extension.vsixmanifest", version);
            UpdateVersionInVsixManifest(@"..\src\VisualStudio.Refactorings\source.extension.vsixmanifest", version);
        }

        private static void UpdateVersionInCsProj(string filePath, Version version)
        {
            XDocument doc = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);

            XElement versionElement = null;

            foreach (XElement propertyGroupElement in doc.Root.Elements("PropertyGroup"))
            {
                versionElement = propertyGroupElement.Elements("Version").FirstOrDefault();

                if (versionElement != null)
                    break;
            }

            if (versionElement == null)
            {
                Console.WriteLine("Version element not found.");
                return;
            }

            versionElement.Value = version.ToString();

            Console.WriteLine($"Updating version to \"{version}\" in \"{filePath}\"");

            var xmlWriterSettings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                IndentChars = "  ",
                Indent = true
            };

            using (var sw = new StreamWriter(filePath))
            using (XmlWriter xmlWriter = XmlWriter.Create(sw, xmlWriterSettings))
            {
                doc.WriteTo(xmlWriter);
            }
        }

        private static void UpdateVersionInVsixManifest(string filePath, Version version)
        {
            XDocument doc = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);

            XName name = (XNamespace)VsixXmlNamespace + "Metadata";
            XAttribute versionAttribute = doc.Root
                .Elements(name).FirstOrDefault()?
                .Elements((XNamespace)VsixXmlNamespace + "Identity").FirstOrDefault()?
                .Attribute("Version");

            if (versionAttribute == null)
            {
                Console.WriteLine("Version attribute not found.");
                return;
            }

            versionAttribute.Value = version.ToString(3);

            Console.WriteLine($"Updating version to \"{version}\" in \"{filePath}\"");

            doc.Save(filePath);
        }

        private static void UpdateVersionInAssemblyInfo(string filePath, Version version)
        {
            string s = File.ReadAllText(filePath, Encoding.UTF8);

            if (!_assemblyVersionRegex.IsMatch(s))
            {
                Console.WriteLine($"Assembly version not found in \"{filePath}\"");
                return;
            }

            s = _assemblyVersionRegex.Replace(s, version.ToString(), 1);

            Console.WriteLine($"Updating version to '{version}' in '{filePath}'");

            File.WriteAllText(filePath, s, Encoding.UTF8);
        }
    }
}
