// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

        private static readonly Regex _packageJsonVersionRegex = (
            AssertBack("\"version\": \"")
            + Digits() + "." + Digits() + "." + Digits()
            + Assert("\"")
        ).ToRegex();

        private static readonly Regex _readmeVersionRegex = (
            AssertBack("josefpihrt-vscode.roslynator-")
            + Digits() + "." + Digits() + "." + Digits()
            + Assert("/")
        ).ToRegex();

        private static void Main(string[] args)
        {
#if DEBUG
            const string rootPath = @"..\..\..\..\..";

            const string versionText = "1.0.0.0";
            const string apiVersionText = "1.0.0.0";
#else
            if (args == null)
                return;

            string rootPath = args.First();
            string versionText = args.ElementAtOrDefault(1);
            string apiVersionText = args.ElementAtOrDefault(2);
#endif
            if (Version.TryParse(apiVersionText, out Version version))
            {
                UpdateVersionInCsProj(rootPath + @"\Core\Core.csproj", version);
                UpdateVersionInCsProj(rootPath + @"\CSharp\CSharp.csproj", version);
                UpdateVersionInCsProj(rootPath + @"\CSharp.Workspaces\CSharp.Workspaces.csproj", version);
                UpdateVersionInCsProj(rootPath + @"\VisualBasic\VisualBasic.csproj", version);
                UpdateVersionInCsProj(rootPath + @"\VisualBasic.Workspaces\VisualBasic.Workspaces.csproj", version);
                UpdateVersionInCsProj(rootPath + @"\Workspaces.Core\Workspaces.Core.csproj", version);
            }

            if (Version.TryParse(versionText, out version))
            {
                UpdateVersionInCsProj(rootPath + @"\Analyzers\Analyzers.csproj", version);
                UpdateVersionInCsProj(rootPath + @"\Analyzers.CodeFixes\Analyzers.CodeFixes.csproj", version);
                UpdateVersionInCsProj(rootPath + @"\CodeFixes\CodeFixes.csproj", version);
                UpdateVersionInCsProj(rootPath + @"\Common\Common.csproj", version);
                UpdateVersionInCsProj(rootPath + @"\Workspaces.Common\Workspaces.Common.csproj", version);
                UpdateVersionInCsProj(rootPath + @"\Refactorings\Refactorings.csproj", version);

                UpdateVersionInFile(rootPath + @"\VisualStudio\Properties\AssemblyInfo.cs", version, _assemblyVersionRegex, Encoding.UTF8);
                UpdateVersionInFile(rootPath + @"\VisualStudio.Common\Properties\AssemblyInfo.cs", version, _assemblyVersionRegex, Encoding.UTF8);
                UpdateVersionInFile(rootPath + @"\VisualStudio.Refactorings\Properties\AssemblyInfo.cs", version, _assemblyVersionRegex, Encoding.UTF8);

                UpdateVersionInVsixManifest(rootPath + @"\VisualStudio\source.extension.vsixmanifest", version);
                UpdateVersionInVsixManifest(rootPath + @"\VisualStudio.Refactorings\source.extension.vsixmanifest", version);

                version = Version.Parse(version.ToString(3));

                var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

                UpdateVersionInFile(rootPath + @"\VisualStudioCode\package\README.md", version, _readmeVersionRegex, utf8NoBom);
                UpdateVersionInFile(rootPath + @"\VisualStudioCode\package\package.json", version, _packageJsonVersionRegex, utf8NoBom);
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

        private static void UpdateVersionInFile(string filePath, Version version, Regex regex, Encoding encoding)
        {
            string s = File.ReadAllText(filePath, Encoding.UTF8);

            Match match = regex.Match(s);

            if (!match.Success)
            {
                Console.WriteLine($"Version not found in \"{filePath}\"");
                return;
            }

            if (!VerifyVersion(match.Value, version, filePath))
                return;

            s = regex.Replace(s, version.ToString());

            File.WriteAllText(filePath, s, encoding);
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
