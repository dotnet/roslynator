// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace Roslynator.Configuration
{
    internal class CodeAnalysisConfiguration
    {
        public const string ConfigFileName = "roslynator.config";

        private static CodeAnalysisConfiguration _current;

        public static CodeAnalysisConfiguration Default { get; } = new CodeAnalysisConfiguration();

        public static CodeAnalysisConfiguration Current
        {
            get
            {
                if (_current == null)
                    Interlocked.CompareExchange(ref _current, LoadConfigurationFromFile(), null);

                return _current;
            }
        }

        public CodeAnalysisConfiguration(
            IEnumerable<KeyValuePair<string, bool>> codeFixes = null,
            IEnumerable<KeyValuePair<string, bool>> refactorings = null,
            IEnumerable<string> ruleSets = null,
            bool prefixFieldIdentifierWithUnderscore = true)
        {
            CodeFixes = codeFixes?.ToImmutableDictionary() ?? ImmutableDictionary<string, bool>.Empty;
            Refactorings = refactorings?.ToImmutableDictionary() ?? ImmutableDictionary<string, bool>.Empty;
            RuleSets = ruleSets?.ToImmutableArray() ?? ImmutableArray<string>.Empty;
            PrefixFieldIdentifierWithUnderscore = prefixFieldIdentifierWithUnderscore;
        }

        public ImmutableDictionary<string, bool> CodeFixes { get; }

        public ImmutableDictionary<string, bool> Refactorings { get; }

        public ImmutableArray<string> RuleSets { get; }

        public bool PrefixFieldIdentifierWithUnderscore { get; set; }

        private static CodeAnalysisConfiguration LoadConfigurationFromFile()
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
                        try
                        {
                            return Load(path);
                        }
                        catch (Exception ex)
                        {
                            if (ex is IOException
                                || ex is UnauthorizedAccessException
                                || ex is XmlException)
                            {
                                Debug.Fail(ex.ToString());
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
            }

            return Default;
        }

        public static CodeAnalysisConfiguration Load(string uri)
        {
            XDocument doc = XDocument.Load(uri);

            XElement root = doc.Root;

            Debug.Assert(root?.HasName("Roslynator") == true, root?.Name.LocalName);

            Builder builder = default;

            if (root?.HasName("Roslynator") == true)
            {
                foreach (XElement element in root.Elements())
                {
                    if (element.HasName("Settings"))
                    {
                        builder = LoadSettings(element);
                    }
                    else
                    {
                        Debug.Fail(element.Name.LocalName);
                    }
                }
            }

            return builder?.ToImmutable() ?? Default;
        }

        private static Builder LoadSettings(XElement element)
        {
            var builder = new Builder();

            foreach (XElement e in element.Elements())
            {
                if (e.HasName("General"))
                {
                    LoadGeneral(e, builder);
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
                    LoadRuleSets(e, builder);
                }
                else
                {
                    Debug.Fail(e.Name.LocalName);
                }
            }

            return builder;
        }

        private static void LoadGeneral(XElement element, Builder builder)
        {
            foreach (XElement e in element.Elements())
            {
                if (e.HasName("PrefixFieldIdentifierWithUnderscore"))
                {
                    string value = e.Value;

                    if (bool.TryParse(value, out bool result))
                        builder.PrefixFieldIdentifierWithUnderscore = result;
                }
                else
                {
                    Debug.Fail(e.Name.LocalName);
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

        private static void LoadRuleSets(XElement element, Builder builder)
        {
            foreach (XElement e in element.Elements())
            {
                if (e.HasName("RuleSet"))
                {
                    string path = null;

                    foreach (XAttribute attribute in e.Attributes())
                    {
                        if (attribute.HasName("Path"))
                        {
                            path = attribute.Value.Trim();

                            path = Environment.ExpandEnvironmentVariables(path);
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

        private class Builder
        {
            private ImmutableDictionary<string, bool>.Builder _codeFixes;
            private ImmutableDictionary<string, bool>.Builder _refactorings;
            private ImmutableArray<string>.Builder _ruleSets;

            public ImmutableDictionary<string, bool>.Builder CodeFixes
            {
                get { return _codeFixes ?? (_codeFixes = ImmutableDictionary.CreateBuilder<string, bool>()); }
            }

            public ImmutableDictionary<string, bool>.Builder Refactorings
            {
                get { return _refactorings ?? (_refactorings = ImmutableDictionary.CreateBuilder<string, bool>()); }
            }

            public ImmutableArray<string>.Builder RuleSets
            {
                get { return _ruleSets ?? (_ruleSets = ImmutableArray.CreateBuilder<string>()); }
            }

            public bool PrefixFieldIdentifierWithUnderscore { get; set; } = Default.PrefixFieldIdentifierWithUnderscore;

            public CodeAnalysisConfiguration ToImmutable()
            {
                return new CodeAnalysisConfiguration(
                    codeFixes: CodeFixes?.ToImmutable() ?? ImmutableDictionary<string, bool>.Empty,
                    refactorings: Refactorings?.ToImmutable() ?? ImmutableDictionary<string, bool>.Empty,
                    ruleSets: RuleSets?.ToImmutable() ?? ImmutableArray<string>.Empty,
                    prefixFieldIdentifierWithUnderscore: PrefixFieldIdentifierWithUnderscore);
            }
        }
    }
}
