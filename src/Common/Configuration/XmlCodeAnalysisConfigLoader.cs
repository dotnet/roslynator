// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Roslynator.Configuration
{
    internal static class XmlCodeAnalysisConfigLoader
    {
        public static XmlCodeAnalysisConfig Load(string path)
        {
            return LoadAndCatchIfThrows(path, exceptionHandler: ex => Debug.Fail(ex.ToString()));
        }

        internal static XmlCodeAnalysisConfig LoadAndCatchIfThrows(string uri, XmlConfigLoadOptions options = XmlConfigLoadOptions.None, Action<Exception> exceptionHandler = null)
        {
            try
            {
                return Load(uri, options);
            }
            catch (Exception ex) when (ex is IOException
                || ex is UnauthorizedAccessException
                || ex is XmlException)
            {
                exceptionHandler?.Invoke(ex);
                return null;
            }
        }

        internal static XmlCodeAnalysisConfig Load(string path, XmlConfigLoadOptions options = XmlConfigLoadOptions.None)
        {
            if (!TryGetNormalizedFullPath(path, out path))
                return null;

            Builder builder = null;

            Queue<string> queue = null;

            Load(path, ref builder, ref queue);

            ImmutableArray<string> includes = queue?.ToImmutableArray() ?? ImmutableArray<string>.Empty;

            if (queue != null
                && (options & XmlConfigLoadOptions.SkipIncludes) == 0)
            {
                var loadedIncludes = new HashSet<string>(FileSystemHelpers.Comparer) { path };

                do
                {
                    string include = queue.Dequeue();

                    if (!loadedIncludes.Contains(include)
                        && Path.GetFileName(include) == XmlCodeAnalysisConfig.FileName
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
                return null;

            return new XmlCodeAnalysisConfig(
                includes: includes,
                analyzers: builder.Analyzers?.ToImmutable() ?? ImmutableDictionary<string, bool>.Empty,
                codeFixes: builder.CodeFixes?.ToImmutable() ?? ImmutableDictionary<string, bool>.Empty,
                refactorings: builder.Refactorings?.ToImmutable() ?? ImmutableDictionary<string, bool>.Empty,
                ruleSets: builder.RuleSets?.ToImmutable() ?? ImmutableArray<string>.Empty,
                prefixFieldIdentifierWithUnderscore: builder.PrefixFieldIdentifierWithUnderscore,
                maxLineLength: builder.MaxLineLength);
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
                else if (e.HasName("Formatting"))
                {
                    LoadFormatting(e, builder);
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

        private static void LoadFormatting(XElement element, Builder builder)
        {
            foreach (XElement e in element.Elements())
            {
                if (e.HasName("MaxLineLength"))
                {
                    if (int.TryParse(e.Value, out int result))
                    {
                        builder.MaxLineLength = result;
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

            public bool PrefixFieldIdentifierWithUnderscore { get; set; } = ConfigOptionDefaultValues.PrefixFieldIdentifierWithUnderscore;

            public int MaxLineLength { get; set; } = ConfigOptionDefaultValues.MaxLineLength;
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
