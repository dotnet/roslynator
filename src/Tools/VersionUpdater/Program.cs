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

            if (Version.TryParse(args.ElementAtOrDefault(1), out Version version))
            {
                UpdateVersionInCsProj(@"..\src\Core\Core.csproj", version);
                UpdateVersionInCsProj(@"..\src\CSharp\CSharp.csproj", version);
                UpdateVersionInCsProj(@"..\src\CSharp.Workspaces\CSharp.Workspaces.csproj", version);
                UpdateVersionInCsProj(@"..\src\VisualBasic\VisualBasic.csproj", version);
                UpdateVersionInCsProj(@"..\src\VisualBasic.Workspaces\VisualBasic.Workspaces.csproj", version);
                UpdateVersionInCsProj(@"..\src\Workspaces.Core\Workspaces.Core.csproj", version);
            }

            if (Version.TryParse(args.FirstOrDefault(), out version))
            {
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

            if (!VerifyVersion(versionElement.Value, version, filePath))
                return;

            versionElement.Value = version.ToString();

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

            version = Version.Parse(version.ToString(3));

            if (!VerifyVersion(versionAttribute.Value, version, filePath))
                return;

            versionAttribute.Value = version.ToString();

            doc.Save(filePath);
        }

        private static void UpdateVersionInAssemblyInfo(string filePath, Version version)
        {
            string s = File.ReadAllText(filePath, Encoding.UTF8);

            Match match = _assemblyVersionRegex.Match(s);

            if (!match.Success)
            {
                Console.WriteLine($"Assembly version not found in \"{filePath}\"");
                return;
            }

            if (!VerifyVersion(match.Value, version, filePath))
                return;

            s = s.Substring(0, match.Index) + version.ToString() + s.Substring(match.Index + match.Length);

            File.WriteAllText(filePath, s, Encoding.UTF8);
        }

        private static bool VerifyVersion(string text, Version version, string filePath)
        {
            if (!Version.TryParse(text, out Version oldVersion))
            {
                Console.WriteLine($"Cannot parse version '{text}'.");
                return false;
            }

            if (version == oldVersion)
                return false;

            if (version < oldVersion)
                throw new ArgumentException("New version is lower than old version.", nameof(oldVersion));

            Console.WriteLine($"Updating version from {oldVersion} to {version} in \"{filePath}\"");

            return true;
        }
    }
}
