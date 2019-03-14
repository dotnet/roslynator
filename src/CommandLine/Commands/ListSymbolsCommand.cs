// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using DotMarkdown;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Roslynator.Documentation;
using Roslynator.Documentation.Html;
using Roslynator.Documentation.Json;
using Roslynator.Documentation.Markdown;
using Roslynator.Documentation.Xml;
using Roslynator.FindSymbols;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class ListSymbolsCommand : MSBuildWorkspaceCommand
    {
        public ListSymbolsCommand(
            ListSymbolsCommandLineOptions options,
            SymbolFilterOptions symbolFilterOptions,
            SymbolDefinitionFormatOptions formatOptions,
            SymbolDefinitionListLayout layout,
            SymbolDefinitionPartFilter ignoredParts,
            in ProjectFilter projectFilter) : base(projectFilter)
        {
            Options = options;
            SymbolFilterOptions = symbolFilterOptions;
            FormatOptions = formatOptions;
            Layout = layout;
            IgnoredParts = ignoredParts;
        }

        public ListSymbolsCommandLineOptions Options { get; }

        public SymbolFilterOptions SymbolFilterOptions { get; }

        public SymbolDefinitionFormatOptions FormatOptions { get; }

        public SymbolDefinitionListLayout Layout { get; }

        public SymbolDefinitionPartFilter IgnoredParts { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            AssemblyResolver.Register();

            var format = new DefinitionListFormat(
                layout: Layout,
                parts: SymbolDefinitionPartFilter.All & ~IgnoredParts,
                formatOptions: FormatOptions,
                groupByAssembly: Options.GroupByAssembly,
                emptyLineBetweenMembers: Options.EmptyLineBetweenMembers,
                emptyLineBetweenMemberGroups: true,
                omitIEnumerable: true,
                preferDefaultLiteral: true,
                indentChars: Options.IndentChars);

            ImmutableArray<Compilation> compilations = await GetCompilationsAsync(projectOrSolution, cancellationToken);

            IEnumerable<IAssemblySymbol> assemblies = compilations.Select(f => f.Assembly);

            HashSet<IAssemblySymbol> externalAssemblies = null;

            foreach (string reference in Options.References)
            {
                IAssemblySymbol externalAssembly = FindExternalAssembly(compilations, reference);

                if (externalAssembly == null)
                {
                    WriteLine($"Cannot find external assembly '{reference}'", Verbosity.Quiet);
                    return CommandResult.Fail;
                }

                (externalAssemblies ?? (externalAssemblies = new HashSet<IAssemblySymbol>())).Add(externalAssembly);
            }

            if (externalAssemblies != null)
                assemblies = assemblies.Concat(externalAssemblies);

            TestOutput(compilations, assemblies, format, cancellationToken);

            string text = null;

            SymbolDocumentationProvider documentationProvider = (Options.Documentation)
                ? new SymbolDocumentationProvider(compilations)
                : null;

            using (var stringWriter = new StringWriter())
            {
                SymbolDefinitionWriter writer = new SymbolDefinitionTextWriter(
                    stringWriter,
                    filter: SymbolFilterOptions,
                    format: format,
                    documentationProvider: documentationProvider);

                writer.WriteDocument(assemblies, cancellationToken);

                text = stringWriter.ToString();
            }

            WriteLine(Verbosity.Minimal);
            WriteLine(text, Verbosity.Minimal);

            foreach (string path in Options.Output)
            {
                WriteLine($"Save '{path}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                string extension = Path.GetExtension(path);

                if (string.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase))
                {
                    var xmlWriterSettings = new XmlWriterSettings() { Indent = true, IndentChars = Options.IndentChars };

                    using (XmlWriter xmlWriter = XmlWriter.Create(path, xmlWriterSettings))
                    using (SymbolDefinitionWriter writer = new SymbolDefinitionXmlWriter(xmlWriter, SymbolFilterOptions, format, documentationProvider))
                    {
                        writer.WriteDocument(assemblies, cancellationToken);
                    }
                }
                else if (string.Equals(extension, ".html", StringComparison.OrdinalIgnoreCase))
                {
                    var xmlWriterSettings = new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true, IndentChars = "" };

                    using (XmlWriter xmlWriter = XmlWriter.Create(path, xmlWriterSettings))
                    using (SymbolDefinitionWriter writer = new SymbolDefinitionHtmlWriter(xmlWriter, SymbolFilterOptions, format, documentationProvider))
                    {
                        writer.WriteDocument(assemblies, cancellationToken);
                    }
                }
                else if (string.Equals(extension, ".md", StringComparison.OrdinalIgnoreCase))
                {
                    var markdownWriterSettings = new MarkdownWriterSettings();

                    using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                    using (var streamWriter = new StreamWriter(fileStream, Encodings.UTF8NoBom))
                    using (MarkdownWriter markdownWriter = MarkdownWriter.Create(streamWriter, markdownWriterSettings))
                    using (SymbolDefinitionWriter writer = new SymbolDefinitionMarkdownWriter(markdownWriter, SymbolFilterOptions, format))
                    {
                        writer.WriteDocument(assemblies, cancellationToken);
                    }
                }
                else if (string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase))
                {
                    using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                    using (var streamWriter = new StreamWriter(fileStream, Encodings.UTF8NoBom))
                    using (var jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        string indentChars = format.IndentChars;

                        if (indentChars.Length > 0)
                        {
                            jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                            jsonWriter.Indentation = indentChars.Length;
                            jsonWriter.IndentChar = indentChars[0];
                        }
                        else
                        {
                            jsonWriter.Formatting = Newtonsoft.Json.Formatting.None;
                        }

                        using (SymbolDefinitionWriter writer = new SymbolDefinitionJsonWriter(jsonWriter, SymbolFilterOptions, format, documentationProvider))
                        {
                            writer.WriteDocument(assemblies, cancellationToken);
                        }
                    }
                }
                else
                {
                    File.WriteAllText(path, text, Encoding.UTF8);
                }
            }

#if DEBUG
            if (ShouldWrite(Verbosity.Normal))
                WriteSummary(assemblies, SymbolFilterOptions, Verbosity.Normal);
#endif

            return CommandResult.Success;
        }

#if DEBUG
        private static void WriteSummary(IEnumerable<IAssemblySymbol> assemblies, SymbolFilterOptions filter, Verbosity verbosity)
        {
            WriteLine(verbosity);

            WriteLine($"{assemblies.Count()} assemblies", verbosity);

            INamedTypeSymbol[] types = assemblies
                .SelectMany(a => a.GetTypes(t => filter.IsMatch(t)))
                .ToArray();

            IEnumerable<INamespaceSymbol> namespaces = null;

            if (filter.SymbolGroups == SymbolGroupFilter.None)
            {
                namespaces = assemblies
                    .SelectMany(a => a.GetNamespaces(n => !n.IsGlobalNamespace && filter.IsMatch(n)))
                    .Distinct(MetadataNameEqualityComparer<INamespaceSymbol>.Instance);
            }
            else
            {
                namespaces = types
                    .Select(f => f.ContainingNamespace)
                    .Distinct(MetadataNameEqualityComparer<INamespaceSymbol>.Instance)
                    .Where(f => filter.IsMatch(f));
            }

            WriteLine($"  {namespaces.Count()} namespaces", verbosity);

            WriteLine($"    {types.Length} types", verbosity);

            if (types.Length > 0)
            {
                foreach (IGrouping<SymbolGroup, INamedTypeSymbol> grouping in types.GroupBy(f => f.GetSymbolGroup()))
                {
                    SymbolGroup group = grouping.Key;
                    WriteLine($"      {grouping.Count()} {group.GetPluralText()}", verbosity);

                    if (group == SymbolGroup.Class)
                        WriteLine($"        {grouping.Count(f => f.IsStatic)} static {group.GetPluralText()}", verbosity);
                }

                WriteLine(verbosity);

                ISymbol[] members = types
                    .Where(f => f.TypeKind.Is(TypeKind.Class, TypeKind.Struct, TypeKind.Interface))
                    .SelectMany(t => t.GetMembers().Where(m => !m.IsKind(SymbolKind.NamedType) && filter.IsMatch(m)))
                    .ToArray();

                WriteLine($"    {members.Length} members", verbosity);

                if (members.Length > 0)
                {
                    foreach (IGrouping<SymbolGroup, ISymbol> grouping in members.GroupBy(f => f.GetSymbolGroup()))
                    {
                        SymbolGroup group = grouping.Key;
                        WriteLine($"      {grouping.Count()} {group.GetPluralText()}", verbosity);

                        if (group == SymbolGroup.Method)
                            WriteLine($"        {grouping.Count(f => f.IsKind(SymbolKind.Method) && ((IMethodSymbol)f).IsExtensionMethod)} extension {group.GetPluralText()}", verbosity);
                    }

                    WriteLine(verbosity);
                }
            }
        }
#endif

        private static IAssemblySymbol FindExternalAssembly(IEnumerable<Compilation> compilations, string path)
        {
            foreach (Compilation compilation in compilations)
            {
                foreach (MetadataReference externalReference in compilation.ExternalReferences)
                {
                    if (externalReference is PortableExecutableReference reference)
                    {
                        if (string.Equals(path, reference.FilePath, StringComparison.OrdinalIgnoreCase)
                            || string.Equals(path, Path.GetFileName(reference.FilePath), StringComparison.OrdinalIgnoreCase))
                        {
                            return compilation.GetAssemblyOrModuleSymbol(reference) as IAssemblySymbol;
                        }
                    }
                }
            }

            return null;
        }

        [Conditional("DEBUG")]
        private void TestOutput(
            ImmutableArray<Compilation> compilations,
            IEnumerable<IAssemblySymbol> assemblies,
            DefinitionListFormat format,
            CancellationToken cancellationToken)
        {
            using (SymbolDefinitionWriter textWriter = new SymbolDefinitionTextWriter(
                ConsoleOut,
                filter: SymbolFilterOptions,
                format: format))
            {
                textWriter.WriteDocument(assemblies, cancellationToken);
            }

            using (var jsonWriter = new JsonTextWriter(ConsoleOut))
            {
                string indentChars = format.IndentChars;

                if (indentChars.Length > 0)
                {
                    jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                    jsonWriter.Indentation = indentChars.Length;
                    jsonWriter.IndentChar = indentChars[0];
                }
                else
                {
                    jsonWriter.Formatting = Newtonsoft.Json.Formatting.None;
                }

                using (SymbolDefinitionWriter writer = new SymbolDefinitionJsonWriter(jsonWriter, SymbolFilterOptions, format, null))
                {
                    writer.WriteDocument(assemblies, cancellationToken);
                }
            }

            WriteLine();

            using (XmlWriter xmlWriter = XmlWriter.Create(ConsoleOut, new XmlWriterSettings() { Indent = true, IndentChars = Options.IndentChars }))
            using (SymbolDefinitionWriter writer = new SymbolDefinitionXmlWriter(xmlWriter, SymbolFilterOptions, format, new SymbolDocumentationProvider(compilations)))
            {
                writer.WriteDocument(assemblies, cancellationToken);
            }

            WriteLine();

            using (XmlWriter xmlWriter = XmlWriter.Create(ConsoleOut, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true, IndentChars = "" }))
            using (SymbolDefinitionWriter writer = new SymbolDefinitionHtmlWriter(xmlWriter, SymbolFilterOptions, format, new SymbolDocumentationProvider(compilations)))
                writer.WriteDocument(assemblies, cancellationToken);

            WriteLine();

            using (MarkdownWriter markdownWriter = MarkdownWriter.Create(ConsoleOut))
            using (SymbolDefinitionWriter writer = new SymbolDefinitionMarkdownWriter(markdownWriter, SymbolFilterOptions, format, null))
            {
                writer.WriteDocument(assemblies, cancellationToken);
            }

            WriteLine();
        }
    }
}
