// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public sealed class SourceReferenceProvider
    {
        private readonly ImmutableDictionary<string, ImmutableArray<SymbolReference>> _map;

        private SourceReferenceProvider(ImmutableDictionary<string, ImmutableArray<SymbolReference>> map)
        {
            _map = map;
        }

        public ImmutableArray<SourceReference> GetSourceReferences(ISymbol symbol)
        {
            string id = symbol.GetDocumentationCommentId();

            Debug.Assert(!string.IsNullOrEmpty(id), symbol.ToDisplayString());

            if (_map.TryGetValue(id, out ImmutableArray<SymbolReference> references))
            {
                return ImmutableArray.CreateRange(references, f => new SourceReference(f.Repository.Version, f.GetUrl()?.AbsoluteUri));
            }

            return ImmutableArray<SourceReference>.Empty;
        }

        public static SourceReferenceProvider Load(IEnumerable<string> paths)
        {
            ImmutableDictionary<string, ImmutableArray<SymbolReference>.Builder>.Builder dic = ImmutableDictionary.CreateBuilder<string, ImmutableArray<SymbolReference>.Builder>();

            foreach (string uri in paths)
                Load(uri, ref dic);

            return Load(dic);
        }

        public static SourceReferenceProvider Load(string path)
        {
            ImmutableDictionary<string, ImmutableArray<SymbolReference>.Builder>.Builder dic = ImmutableDictionary.CreateBuilder<string, ImmutableArray<SymbolReference>.Builder>();

            Load(path, ref dic);

            return Load(dic);
        }

        private static SourceReferenceProvider Load(ImmutableDictionary<string, ImmutableArray<SymbolReference>.Builder>.Builder dic)
        {
            ImmutableDictionary<string, ImmutableArray<SymbolReference>> map = dic.ToImmutableDictionary(f => f.Key, f => f.Value.ToImmutableArray());

            return new SourceReferenceProvider(map);
        }

        private static void Load(string uri, ref ImmutableDictionary<string, ImmutableArray<SymbolReference>.Builder>.Builder dic)
        {
            XDocument doc = XDocument.Load(uri);

            foreach (XElement repositoryElement in doc.Root.Elements("repository"))
            {
                string type = repositoryElement.Attribute("type").Value;

                string urlString = repositoryElement.Attribute("url").Value;

                if (!urlString.EndsWith("/", StringComparison.Ordinal))
                    urlString += "/";

                var url = new Uri(urlString);

                var version = Version.Parse(repositoryElement.Attribute("version").Value);

                string branch = repositoryElement.Attribute("branch")?.Value;
                string commit = repositoryElement.Attribute("commit").Value;

                var repository = new RepositoryInfo(name: type, url: url, version: version, branch: branch, commit: commit);

                XElement membersElements = repositoryElement.Element("members");

                if (membersElements != null)
                {
                    foreach (XElement memberElement in membersElements.Elements("member"))
                    {
                        string name = memberElement.Attribute("name").Value;

                        XElement locationElement = memberElement.Element("locations")?.Element("location");

                        string path = null;
                        string line = null;

                        if (locationElement != null)
                        {
                            path = locationElement.Attribute("path").Value;
                            line = locationElement.Attribute("line").Value;
                        }

                        if (!dic.TryGetValue(name, out ImmutableArray<SymbolReference>.Builder builder))
                        {
                            builder = ImmutableArray.CreateBuilder<SymbolReference>();
                            dic[name] = builder;
                        }

                        builder.Add(new SymbolReference(repository, path, line));
                    }
                }
            }
        }

        private class RepositoryInfo
        {
            public RepositoryInfo(string name, Uri url, Version version, string branch, string commit)
            {
                Name = name;
                Url = url;
                Version = version;
                Branch = branch;
                Commit = commit;
            }

            public string Name { get; }

            public Uri Url { get; }

            public Version Version { get; }

            public string Branch { get; }

            public string Commit { get; }

            public Uri FullUrl { get; private set; }

            public Uri GetUrl(string url, string line)
            {
                if (FullUrl == null)
                    FullUrl = new Uri(Url, $"blob/{Commit}/");

                string relativeUrl = (!string.IsNullOrEmpty(line)) ? $"{url}#L{line}" : url;

                return new Uri(FullUrl, relativeUrl);
            }
        }

        private readonly struct SymbolReference
        {
            public SymbolReference(RepositoryInfo repository, string path, string line)
            {
                Repository = repository;
                Path = path;
                Line = line;
            }

            public RepositoryInfo Repository { get; }

            public string Path { get; }

            public string Line { get; }

            public Uri GetUrl()
            {
                return (Path != null) ? Repository.GetUrl(Path, Line) : null;
            }
        }
    }
}
