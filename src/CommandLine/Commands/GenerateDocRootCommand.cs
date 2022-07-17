// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotMarkdown;
using Microsoft.CodeAnalysis;
using Roslynator.Documentation;
using Roslynator.Documentation.Markdown;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class GenerateDocRootCommand : MSBuildWorkspaceCommand<CommandResult>
    {
        private static readonly Encoding _defaultEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public GenerateDocRootCommand(
            GenerateDocRootCommandLineOptions options,
            DocumentationDepth depth,
            RootDocumentationParts ignoredParts,
            IncludeContainingNamespaceFilter includeContainingNamespaceFilter,
            Visibility visibility,
            DocumentationHost documentationHost,
            in ProjectFilter projectFilter) : base(projectFilter)
        {
            Options = options;
            Depth = depth;
            IgnoredParts = ignoredParts;
            IncludeContainingNamespaceFilter = includeContainingNamespaceFilter;
            Visibility = visibility;
            DocumentationHost = documentationHost;
        }

        public GenerateDocRootCommandLineOptions Options { get; }

        public DocumentationDepth Depth { get; }

        public RootDocumentationParts IgnoredParts { get; }

        public IncludeContainingNamespaceFilter IncludeContainingNamespaceFilter { get; }

        public Visibility Visibility { get; }

        public DocumentationHost DocumentationHost { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            AssemblyResolver.Register();

            var documentationOptions = new DocumentationOptions(
                rootFileHeading: Options.Heading,
                ignoredNames: Options.IgnoredNames,
                rootDirectoryUrl: Options.RootDirectoryUrl,
                placeSystemNamespaceFirst: !Options.NoPrecedenceForSystem,
                markObsolete: !Options.NoMarkObsolete,
                depth: Depth,
                ignoredRootParts: IgnoredParts,
                includeContainingNamespaceFilter: IncludeContainingNamespaceFilter,
                includeSystemNamespace: Options.IncludeSystemNamespace,
                scrollToContent: Options.ScrollToContent);

            ImmutableArray<Compilation> compilations = await GetCompilationsAsync(projectOrSolution, cancellationToken);

            var documentationModel = new DocumentationModel(compilations, DocumentationFilterOptions.Instance);

            UrlSegmentProvider urlSegmentProvider = DefaultUrlSegmentProvider.Hierarchical;

            var externalProviders = new MicrosoftDocsUrlProvider[] { MicrosoftDocsUrlProvider.Instance };

            DocumentationUrlProvider GetUrlProvider()
            {
                switch (DocumentationHost)
                {
                    case DocumentationHost.GitHub:
                        return new GitHubDocumentationUrlProvider(urlSegmentProvider, externalProviders);
                    case DocumentationHost.Docusaurus:
                        return new DocusaurusDocumentationUrlProvider(urlSegmentProvider, externalProviders);
                    default:
                        throw new InvalidOperationException($"Unknown value '{DocumentationHost}'.");
                }
            }

            MarkdownWriterSettings GetMarkdownWriterSettings()
            {
                switch (DocumentationHost)
                {
                    case DocumentationHost.GitHub:
                        return MarkdownWriterSettings.Default;
                    case DocumentationHost.Docusaurus:
                        return new MarkdownWriterSettings(new MarkdownFormat(angleBracketEscapeStyle: AngleBracketEscapeStyle.EntityRef));
                    default:
                        throw new InvalidOperationException($"Unknown value '{DocumentationHost}'.");
                }
            }

            MarkdownWriterSettings markdownWriterSettings = GetMarkdownWriterSettings();

            DocumentationWriter CreateDocumentationWriter(DocumentationContext context)
            {
                MarkdownWriter writer = MarkdownWriter.Create(new StringBuilder(), markdownWriterSettings);

                switch (DocumentationHost)
                {
                    case DocumentationHost.GitHub:
                        return new MarkdownDocumentationWriter(context, writer);
                    case DocumentationHost.Docusaurus:
                        return new DocusaurusDocumentationWriter(context, writer);
                    default:
                        throw new InvalidOperationException($"Unknown value '{DocumentationHost}'.");
                }
            }

            var context = new DocumentationContext(
                documentationModel,
                GetUrlProvider(),
                documentationOptions,
                c => CreateDocumentationWriter(c));

            var generator = new DocumentationGenerator(context);

            string path = Options.Output;

            WriteLine($"Generate documentation root to '{path}'.", Verbosity.Minimal);

            string heading = Options.Heading;

            if (string.IsNullOrEmpty(heading))
            {
                string fileName = Path.GetFileName(Options.Output);

                heading = (fileName.EndsWith(".dll", StringComparison.Ordinal))
                    ? Path.GetFileNameWithoutExtension(fileName)
                    : fileName;
            }

            DocumentationGeneratorResult result = generator.GenerateRoot(heading);

            File.WriteAllText(path, result.Content, _defaultEncoding);

            WriteLine($"Documentation root successfully generated to '{path}'.", Verbosity.Minimal);

            return CommandResults.Success;
        }
    }
}
