// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace Roslynator.Configuration
{
    public class CodeAnalysisConfiguration
    {
        public const string ConfigFileName = "roslynator.config";

        private static readonly IEqualityComparer<string> _keyComparer = StringComparer.OrdinalIgnoreCase;

        private static CodeAnalysisConfiguration _current;

        public static CodeAnalysisConfiguration Empty { get; } = new CodeAnalysisConfiguration();

        public static CodeAnalysisConfiguration Current
        {
            get
            {
                if (_current == null)
                    Interlocked.CompareExchange(ref _current, LoadConfiguration() ?? Empty, null);

                return _current;
            }
        }

        public CodeAnalysisConfiguration(
            IEnumerable<string> includes = null,
            IEnumerable<KeyValuePair<string, bool>> analyzers = null,
            IEnumerable<KeyValuePair<string, bool>> codeFixes = null,
            IEnumerable<KeyValuePair<string, bool>> refactorings = null,
            IEnumerable<string> ruleSets = null,
            bool prefixFieldIdentifierWithUnderscore = false)
        {
            Includes = includes?.ToImmutableArray() ?? ImmutableArray<string>.Empty;
            Analyzers = analyzers?.ToImmutableDictionary(_keyComparer) ?? ImmutableDictionary<string, bool>.Empty;
            CodeFixes = codeFixes?.ToImmutableDictionary(_keyComparer) ?? ImmutableDictionary<string, bool>.Empty;
            Refactorings = refactorings?.ToImmutableDictionary(_keyComparer) ?? ImmutableDictionary<string, bool>.Empty;
            RuleSets = ruleSets?.ToImmutableArray() ?? ImmutableArray<string>.Empty;
            PrefixFieldIdentifierWithUnderscore = prefixFieldIdentifierWithUnderscore;
        }

        public ImmutableArray<string> Includes { get; }

        public ImmutableDictionary<string, bool> Analyzers { get; }

        public ImmutableDictionary<string, bool> CodeFixes { get; }

        public ImmutableDictionary<string, bool> Refactorings { get; }

        public ImmutableArray<string> RuleSets { get; }

        public bool PrefixFieldIdentifierWithUnderscore { get; }

        internal IEnumerable<string> GetDisabledRefactorings()
        {
            foreach (KeyValuePair<string, bool> kvp in Refactorings)
            {
                if (!kvp.Value)
                    yield return kvp.Key;
            }
        }

        internal IEnumerable<string> GetDisabledCodeFixes()
        {
            foreach (KeyValuePair<string, bool> kvp in CodeFixes)
            {
                if (!kvp.Value)
                    yield return kvp.Key;
            }
        }

        private static CodeAnalysisConfiguration LoadConfiguration()
        {
            string path = typeof(CodeAnalysisConfiguration).Assembly.Location;

            if (!string.IsNullOrEmpty(path))
            {
                path = Path.GetDirectoryName(path);

                if (!string.IsNullOrEmpty(path))
                {
                    path = Path.Combine(path, ConfigFileName);

                    if (File.Exists(path))
                    {
                        return LoadAndCatchIfThrows(path, ex => Debug.Fail(ex.ToString()));
                    }
                }
            }

            return null;
        }

        internal static CodeAnalysisConfiguration LoadAndCatchIfThrows(string uri, Action<Exception> exceptionHandler = null)
        {
            try
            {
                return Load(uri);
            }
            catch (Exception ex) when (ex is IOException
                || ex is UnauthorizedAccessException
                || ex is XmlException)
            {
                exceptionHandler?.Invoke(ex);
                return null;
            }
        }

        public static CodeAnalysisConfiguration Load(string uri)
        {
            if (!TryGetNormalizedFullPath(uri, out uri))
                return null;

            Builder builder = null;

            Queue<string> queue = null;

            Load(uri, ref builder, ref queue);

            ImmutableArray<string> includes = queue?.ToImmutableArray() ?? ImmutableArray<string>.Empty;

            if (queue != null)
            {
                var loadedIncludes = new HashSet<string>(FileSystemHelpers.Comparer) { uri };

                do
                {
                    string include = queue.Dequeue();

                    if (!loadedIncludes.Contains(include)
                        && File.Exists(include))
                    {
                        try
                        {
                            Load(include, ref builder, ref queue);
                        }
                        catch (Exception ex) when (ex is IOException
                            || ex is UnauthorizedAccessException
                            || ex is XmlException)
                        {
                            Debug.Fail(ex.ToString());
                        }
                    }

                    loadedIncludes.Add(include);

                } while (queue.Count > 0);
            }

            if (builder == null)
                return Empty;

            return new CodeAnalysisConfiguration(
                includes: includes,
                analyzers: builder.Analyzers?.ToImmutable() ?? ImmutableDictionary<string, bool>.Empty,
                codeFixes: builder.CodeFixes?.ToImmutable() ?? ImmutableDictionary<string, bool>.Empty,
                refactorings: builder.Refactorings?.ToImmutable() ?? ImmutableDictionary<string, bool>.Empty,
                ruleSets: builder.RuleSets?.ToImmutable() ?? ImmutableArray<string>.Empty,
                prefixFieldIdentifierWithUnderscore: builder.PrefixFieldIdentifierWithUnderscore);
        }

        private static void Load(
            string uri,
            ref Builder builder,
            ref Queue<string> includes)
        {
            XDocument doc = XDocument.Load(uri);

            string directoryPath = null;

            XElement root = doc.Root;

            Debug.Assert(root?.HasName("Roslynator") == true, root?.Name.LocalName);

            if (root?.HasName("Roslynator") == true)
            {
                foreach (XElement element in root.Elements())
                {
                    if (element.HasName("Settings"))
                    {
                        if (builder == null)
                            builder = new Builder();

                        LoadSettings(element, builder, uri);
                    }
                    else if (element.HasName("Include"))
                    {
                        foreach (XAttribute attribute in element.Attributes())
                        {
                            if (attribute.HasName("Path"))
                            {
                                directoryPath ??= Path.GetDirectoryName(uri);

                                string path = LoadPath(attribute, directoryPath);

                                if (path != null)
                                    (includes ??= new Queue<string>()).Enqueue(path);
                            }
                            else
                            {
                                Debug.Fail(attribute.Name.LocalName);
                            }
                        }
                    }
                    else
                    {
                        Debug.Fail(element.Name.LocalName);
                    }
                }
            }
        }

        private static void LoadSettings(XElement element, Builder builder, string filePath)
        {
            foreach (XElement e in element.Elements())
            {
                if (e.HasName("General"))
                {
                    LoadGeneral(e, builder);
                }
                else if (e.HasName("Analyzers"))
                {
                    LoadAnalyzers(e, builder);
                }
                else if (e.HasName("Refactorings"))
                {
                    LoadRefactorings(e, builder);
                }
                else if (e.HasName("CodeFixes"))
                {
                    LoadCodeFixes(e, builder);
                }
                else if (e.HasName("RuleSets"))
                {
                    LoadRuleSets(e, builder, filePath);
                }
                else
                {
                    Debug.Fail(e.Name.LocalName);
                }
            }
        }

        private static void LoadGeneral(XElement element, Builder builder)
        {
            foreach (XElement e in element.Elements())
            {
                if (e.HasName("PrefixFieldIdentifierWithUnderscore"))
                {
                    if (bool.TryParse(e.Value, out bool result))
                    {
                        builder.PrefixFieldIdentifierWithUnderscore = result;
                    }
                    else
                    {
                        Debug.Fail(e.Value);
                    }
                }
                else
                {
                    Debug.Fail(e.Name.LocalName);
                }
            }
        }

        private static void LoadAnalyzers(XElement element, Builder builder)
        {
            foreach (XElement e in element.Elements())
            {
                string key = e.Name.LocalName;

                if (bool.TryParse(e.Value, out bool value))
                {
                    builder.Analyzers[key] = value;
                }
                else
                {
                    Debug.Fail(e.Value);
                }
            }
        }

        private static void LoadRefactorings(XElement element, Builder builder)
        {
            foreach (XElement e in element.Elements())
            {
                if (e.HasName("Refactoring"))
                {
                    string id = null;
                    bool? isEnabled = null;

                    foreach (XAttribute attribute in e.Attributes())
                    {
                        if (attribute.HasName("Id"))
                        {
                            id = attribute.Value;
                        }
                        else if (attribute.HasName("IsEnabled"))
                        {
                            isEnabled = attribute.GetValueAsBoolean();
                        }
                        else
                        {
                            Debug.Fail(attribute.Name.LocalName);
                        }
                    }

                    if (!string.IsNullOrEmpty(id)
                        && isEnabled != null)
                    {
                        builder.Refactorings[id] = isEnabled.Value;
                    }
                }
                else
                {
                    Debug.Fail(e.Name.LocalName);
                }
            }
        }

        private static void LoadCodeFixes(XElement element, Builder builder)
        {
            foreach (XElement e in element.Elements())
            {
                if (e.HasName("CodeFix"))
                {
                    string id = null;
                    bool? isEnabled = null;

                    foreach (XAttribute attribute in e.Attributes())
                    {
                        if (attribute.HasName("Id"))
                        {
                            id = attribute.Value;
                        }
                        else if (attribute.HasName("IsEnabled"))
                        {
                            isEnabled = attribute.GetValueAsBoolean();
                        }
                        else
                        {
                            Debug.Fail(attribute.Name.LocalName);
                        }
                    }

                    if (!string.IsNullOrEmpty(id)
                        && isEnabled != null)
                    {
                        builder.CodeFixes[id] = isEnabled.Value;
                    }
                }
                else
                {
                    Debug.Fail(e.Name.LocalName);
                }
            }
        }

        private static void LoadRuleSets(XElement element, Builder builder, string filePath)
        {
            string directoryPath = null;

            foreach (XElement e in element.Elements())
            {
                if (e.HasName("RuleSet"))
                {
                    string path = null;

                    foreach (XAttribute attribute in e.Attributes())
                    {
                        if (attribute.HasName("Path"))
                        {
                            directoryPath ??= Path.GetDirectoryName(filePath);

                            path = LoadPath(attribute, directoryPath);
                        }
                        else
                        {
                            Debug.Fail(attribute.Name.LocalName);
                        }
                    }

                    if (!string.IsNullOrEmpty(path))
                    {
                        builder.RuleSets.Add(path);
                    }
                }
                else
                {
                    Debug.Fail(e.Name.LocalName);
                }
            }
        }

        private static string LoadPath(XAttribute attribute, string basePath)
        {
            string path = attribute.Value.Trim();

            if (path.Contains("%LOCALAPPDATA%"))
            {
                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                path = path.Replace("%LOCALAPPDATA%", localAppDataPath);
            }

            return (TryGetNormalizedFullPath(path, basePath, out path))
                ? path
                : null;
        }

        public CodeAnalysisConfiguration WithPrefixFieldIdentifierWithUnderscore(bool prefixFieldIdentifierWithUnderscore)
        {
            return new CodeAnalysisConfiguration(
                includes: Includes,
                analyzers: Analyzers,
                codeFixes: CodeFixes,
                refactorings: Refactorings,
                ruleSets: RuleSets,
                prefixFieldIdentifierWithUnderscore: prefixFieldIdentifierWithUnderscore);
        }

        public CodeAnalysisConfiguration WithAnalyzers(IEnumerable<KeyValuePair<string, bool>> analyzers)
        {
            return new CodeAnalysisConfiguration(
                includes: Includes,
                analyzers: analyzers,
                codeFixes: CodeFixes,
                refactorings: Refactorings,
                ruleSets: RuleSets,
                prefixFieldIdentifierWithUnderscore: PrefixFieldIdentifierWithUnderscore);
        }

        public CodeAnalysisConfiguration WithRefactorings(IEnumerable<KeyValuePair<string, bool>> refactorings)
        {
            return new CodeAnalysisConfiguration(
                includes: Includes,
                analyzers: Analyzers,
                codeFixes: CodeFixes,
                refactorings: refactorings,
                ruleSets: RuleSets,
                prefixFieldIdentifierWithUnderscore: PrefixFieldIdentifierWithUnderscore);
        }

        public CodeAnalysisConfiguration WithCodeFixes(IEnumerable<KeyValuePair<string, bool>> codeFixes)
        {
            return new CodeAnalysisConfiguration(
                includes: Includes,
                analyzers: Analyzers,
                codeFixes: codeFixes,
                refactorings: Refactorings,
                ruleSets: RuleSets,
                prefixFieldIdentifierWithUnderscore: PrefixFieldIdentifierWithUnderscore);
        }

        internal void Save(string path)
        {
            var settings = new XElement(
                "Settings",
                new XElement(
                    "General",
                    new XElement("PrefixFieldIdentifierWithUnderscore", PrefixFieldIdentifierWithUnderscore)));

            if (Analyzers.Count > 0)
            {
                settings.Add(
                    new XElement(
                        "Analyzers",
                        Analyzers
                            .OrderBy(f => f.Key)
                            .Select(f => new XElement("Analyzer", new XAttribute("Id", f.Key), new XAttribute("Value", f.Value)))
                    ));
            }

            if (Refactorings.Any(f => !f.Value))
            {
                settings.Add(
                    new XElement(
                        "Refactorings",
                        Refactorings
                            .Where(f => !f.Value)
                            .OrderBy(f => f.Key)
                            .Select(f => new XElement("Refactoring", new XAttribute("Id", f.Key), new XAttribute("IsEnabled", f.Value)))
                    ));
            }

            if (CodeFixes.Any(f => !f.Value))
            {
                settings.Add(
                    new XElement(
                        "CodeFixes",
                        CodeFixes
                            .Where(f => !f.Value)
                            .OrderBy(f => f.Key)
                            .Select(f => new XElement("CodeFix", new XAttribute("Id", f.Key), new XAttribute("IsEnabled", f.Value)))
                    ));
            }

            if (RuleSets.Any())
            {
                settings.Add(
                    new XElement(
                        "RuleSets",
                        RuleSets.Select(f => new XElement("RuleSet", new XAttribute("Path", f)))));
            }

            var doc = new XDocument(new XElement("Roslynator", settings));

            var xmlWriterSettings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = false,
                NewLineChars = Environment.NewLine,
                IndentChars = "  ",
                Indent = true,
            };

            using (var fileStream = new FileStream(path, FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
            using (XmlWriter xmlWriter = XmlWriter.Create(streamWriter, xmlWriterSettings))
                doc.WriteTo(xmlWriter);
        }

        private class Builder
        {
            private ImmutableDictionary<string, bool>.Builder _analyzers;
            private ImmutableDictionary<string, bool>.Builder _codeFixes;
            private ImmutableDictionary<string, bool>.Builder _refactorings;
            private ImmutableArray<string>.Builder _ruleSets;

            public ImmutableDictionary<string, bool>.Builder Analyzers
            {
                get { return _analyzers ??= ImmutableDictionary.CreateBuilder<string, bool>(); }
            }

            public ImmutableDictionary<string, bool>.Builder CodeFixes
            {
                get { return _codeFixes ??= ImmutableDictionary.CreateBuilder<string, bool>(); }
            }

            public ImmutableDictionary<string, bool>.Builder Refactorings
            {
                get { return _refactorings ??= ImmutableDictionary.CreateBuilder<string, bool>(); }
            }

            public ImmutableArray<string>.Builder RuleSets
            {
                get { return _ruleSets ??= ImmutableArray.CreateBuilder<string>(); }
            }

            public bool PrefixFieldIdentifierWithUnderscore { get; set; } = Empty.PrefixFieldIdentifierWithUnderscore;
        }

        private static bool TryGetNormalizedFullPath(string path, out string result)
        {
            return TryGetNormalizedFullPath(path, null, out result);
        }

        private static bool TryGetNormalizedFullPath(string path, string basePath, out string result)
        {
            if (!FileSystemHelpers.TryGetNormalizedFullPath(path, basePath, out result))
            {
                Debug.WriteLine($"Path is invalid: {path}");
                return false;
            }

            return true;
        }
    }
}
