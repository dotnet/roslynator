// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Roslynator.CodeFixes;
using Roslynator.Documentation;
using Roslynator.Documentation.Markdown;
//using static System.Console;
using static Roslynator.CodeFixes.ConsoleHelpers;

#pragma warning disable RCS1090

namespace Roslynator.CommandLine
{
    internal static class Program
    {
        private static readonly Encoding _defaultEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private static void Main(string[] args)
        {
            WriteLine($"Roslynator Command Line Tool version {typeof(Program).GetTypeInfo().Assembly.GetName().Version}");
            WriteLine("Copyright (c) Josef Pihrt. All rights reserved.");
            WriteLine();

            Parser.Default.ParseArguments<FixCommandLineOptions, GenerateDocCommandLineOptions, GenerateDeclarationsCommandLineOptions, GenerateDocRootCommandLineOptions>(args)
                .MapResult(
                  (FixCommandLineOptions options) => FixAsync(options).Result,
                  (GenerateDocCommandLineOptions options) => GenerateDoc(options),
                  (GenerateDeclarationsCommandLineOptions options) => GenerateDeclarations(options),
                  (GenerateDocRootCommandLineOptions options) => GenerateDocRoot(options),
                  _ => 1);
        }

        private static async Task<int> FixAsync(FixCommandLineOptions options)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            CancellationToken cancellationToken = cts.Token;

            if (options.MSBuildPath != null)
            {
                MSBuildLocator.RegisterMSBuildPath(options.MSBuildPath);
            }
            else
            {
                VisualStudioInstance instance = MSBuildLocator.QueryVisualStudioInstances()
                    .OrderBy(f => f.Version)
                    .LastOrDefault();

                if (instance == null)
                {
                    WriteLine("MSBuild location not found. Use option '--msbuild-path' to specify MSBuild location", ConsoleColor.Red);
                    return 1;
                }

                WriteLine($"MSBuild location is '{instance.MSBuildPath}'");

                MSBuildLocator.RegisterInstance(instance);
            }

            var properties = new Dictionary<string, string>();

            foreach (string property in options.Properties)
            {
                int index = property.IndexOf("=");

                if (index == -1)
                {
                    WriteLine($"Unable to parse property '{property}'", ConsoleColor.Red);
                    return 1;
                }

                string key = property.Substring(0, index);

                properties[key] = property.Substring(index + 1);
            }

            if (properties.Count > 0)
            {
                WriteLine("Add MSBuild properties");

                int maxLength = properties.Max(f => f.Key.Length);

                foreach (KeyValuePair<string, string> kvp in properties)
                    WriteLine($"  {kvp.Key.PadRight(maxLength)} = {kvp.Value}");
            }

            // https://github.com/Microsoft/MSBuildLocator/issues/16
            if (!properties.ContainsKey("AlwaysCompileMarkupFilesInSeparateDomain"))
                properties["AlwaysCompileMarkupFilesInSeparateDomain"] = bool.FalseString;

            using (MSBuildWorkspace workspace = MSBuildWorkspace.Create(properties))
            {
                workspace.WorkspaceFailed += (o, e) => WriteLine(e.Diagnostic.Message, ConsoleColor.Yellow);

                string solutionPath = options.SolutionPath;

                WriteLine($"Load solution '{solutionPath}'", ConsoleColor.Cyan);

                try
                {
                    Solution solution;

                    try
                    {
                        solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter(), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        if (ex is FileNotFoundException
                            || ex is InvalidOperationException)
                        {
                            WriteLine(ex.ToString(), ConsoleColor.Red);
                            return 1;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    WriteLine($"Done loading solution '{solutionPath}'", ConsoleColor.Green);

                    var codeFixerOptions = new CodeFixerOptions(
                        ignoreCompilerErrors: options.IgnoreCompilerErrors,
                        ignoreAnalyzerReferences: options.IgnoreAnalyzerReferences,
                        ignoredDiagnosticIds: options.IgnoredDiagnostics,
                        ignoredCompilerDiagnosticIds: options.IgnoredCompilerDiagnostics,
                        ignoredProjectNames: options.IgnoredProjects,
                        batchSize: options.BatchSize);

                    var codeFixer = new CodeFixer(workspace, analyzerAssemblies: options.AnalyzerAssemblies, options: codeFixerOptions);

                    await codeFixer.FixAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    WriteLine("Fixing was canceled.");
                }
            }

            return 0;
        }

        private static int GenerateDoc(GenerateDocCommandLineOptions options)
        {
            if (options.MaxDerivedTypes < 0)
            {
                WriteLine("Maximum number of derived items must be equal or greater than 0.");
                return 1;
            }

            if (!TryGetIgnoredRootParts(options.IgnoredRootParts, out RootDocumentationParts ignoredRootParts))
                return 1;

            if (!TryGetIgnoredNamespaceParts(options.IgnoredNamespaceParts, out NamespaceDocumentationParts ignoredNamespaceParts))
                return 1;

            if (!TryGetIgnoredTypeParts(options.IgnoredTypeParts, out TypeDocumentationParts ignoredTypeParts))
                return 1;

            if (!TryGetIgnoredMemberParts(options.IgnoredMemberParts, out MemberDocumentationParts ignoredMemberParts))
                return 1;

            if (!TryGetOmitContainingNamespaceParts(options.OmitContainingNamespaceParts, out OmitContainingNamespaceParts omitContainingNamespaceParts))
                return 1;

            if (!TryGetVisibility(options.Visibility, out DocumentationVisibility visibility))
                return 1;

            DocumentationModel documentationModel = CreateDocumentationModel(options.References, options.Assemblies, visibility, options.AdditionalXmlDocumentation);

            if (documentationModel == null)
                return 1;

            var documentationOptions = new DocumentationOptions(
                ignoredNames: options.IgnoredNames,
                preferredCultureName: options.PreferredCulture,
                maxDerivedTypes: options.MaxDerivedTypes,
                includeClassHierarchy: !options.NoClassHierarchy,
                placeSystemNamespaceFirst: !options.NoPrecedenceForSystem,
                formatDeclarationBaseList: !options.NoFormatBaseList,
                formatDeclarationConstraints: !options.NoFormatConstraints,
                markObsolete: !options.NoMarkObsolete,
                includeMemberInheritedFrom: !options.OmitMemberInheritedFrom,
                includeMemberOverrides: !options.OmitMemberOverrides,
                includeMemberImplements: !options.OmitMemberImplements,
                includeMemberConstantValue: !options.OmitMemberConstantValue,
                includeInheritedInterfaceMembers: options.IncludeInheritedInterfaceMembers,
                includeAllDerivedTypes: options.IncludeAllDerivedTypes,
                includeAttributeArguments: !options.OmitAttributeArguments,
                includeInheritedAttributes: !options.OmitInheritedAttributes,
                omitIEnumerable: !options.IncludeIEnumerable,
                depth: options.Depth,
                inheritanceStyle: options.InheritanceStyle,
                ignoredRootParts: ignoredRootParts,
                ignoredNamespaceParts: ignoredNamespaceParts,
                ignoredTypeParts: ignoredTypeParts,
                ignoredMemberParts: ignoredMemberParts,
                omitContainingNamespaceParts: omitContainingNamespaceParts,
                scrollToContent: options.ScrollToContent);

            var generator = new MarkdownDocumentationGenerator(documentationModel, WellKnownUrlProviders.GitHub, documentationOptions);

            string directoryPath = options.OutputPath;

            if (!options.NoDelete
                && Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.Delete(directoryPath, recursive: true);
                }
                catch (IOException ex)
                {
                    WriteLine(ex.ToString());
                }
            }

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            CancellationToken cancellationToken = cts.Token;

            WriteLine($"Documentation is being generated to '{options.OutputPath}'.");

            foreach (DocumentationGeneratorResult documentationFile in generator.Generate(heading: options.Heading, cancellationToken))
            {
                string path = Path.Combine(directoryPath, documentationFile.FilePath);

#if DEBUG
                WriteLine($"saving '{path}'");
#else
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, documentationFile.Content, _defaultEncoding);
#endif
            }

            WriteLine($"Documentation successfully generated to '{options.OutputPath}'.");

            return 0;
        }

        private static int GenerateDeclarations(GenerateDeclarationsCommandLineOptions options)
        {
            if (!TryGetIgnoredDeclarationListParts(options.IgnoredParts, out DeclarationListParts ignoredParts))
                return 1;

            if (!TryGetVisibility(options.Visibility, out DocumentationVisibility visibility))
                return 1;

            DocumentationModel documentationModel = CreateDocumentationModel(options.References, options.Assemblies, visibility, options.AdditionalXmlDocumentation);

            if (documentationModel == null)
                return 1;

            var declarationListOptions = new DeclarationListOptions(
                ignoredNames: options.IgnoredNames,
                indent: !options.NoIndent,
                indentChars: options.IndentChars,
                nestNamespaces: options.NestNamespaces,
                newLineBeforeOpenBrace: !options.NoNewLineBeforeOpenBrace,
                emptyLineBetweenMembers: options.EmptyLineBetweenMembers,
                formatBaseList: options.FormatBaseList,
                formatConstraints: options.FormatConstraints,
                formatParameters: options.FormatParameters,
                splitAttributes: !options.MergeAttributes,
                includeAttributeArguments: !options.OmitAttributeArguments,
                omitIEnumerable: !options.IncludeIEnumerable,
                useDefaultLiteral: !options.NoDefaultLiteral,
                fullyQualifiedNames: options.FullyQualifiedNames,
                depth: options.Depth,
                ignoredParts: ignoredParts);

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            CancellationToken cancellationToken = cts.Token;

            WriteLine($"Declaration list is being generated to '{options.OutputPath}'.");

            Task<string> task = DeclarationListGenerator.GenerateAsync(
                documentationModel,
                declarationListOptions,
                namespaceComparer: NamespaceSymbolComparer.GetInstance(systemNamespaceFirst: !options.NoPrecedenceForSystem),
                cancellationToken: cancellationToken);

            string content = task.Result;

            string path = options.OutputPath;

#if DEBUG
            WriteLine($"saving '{path}'");
#else
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, content, Encoding.UTF8);
#endif

            WriteLine($"Declaration list successfully generated to '{options.OutputPath}'.");

            return 0;
        }

        private static int GenerateDocRoot(GenerateDocRootCommandLineOptions options)
        {
            if (!TryGetVisibility(options.Visibility, out DocumentationVisibility visibility))
                return 1;

            DocumentationModel documentationModel = CreateDocumentationModel(options.References, options.Assemblies, visibility);

            if (documentationModel == null)
                return 1;

            if (!TryGetIgnoredRootParts(options.Parts, out RootDocumentationParts ignoredParts))
                return 1;

            var documentationOptions = new DocumentationOptions(
                ignoredNames: options.IgnoredNames,
                rootDirectoryUrl: options.RootDirectoryUrl,
                includeClassHierarchy: !options.NoClassHierarchy,
                placeSystemNamespaceFirst: !options.NoPrecedenceForSystem,
                markObsolete: !options.NoMarkObsolete,
                depth: options.Depth,
                ignoredRootParts: ignoredParts,
                omitContainingNamespaceParts: (options.OmitContainingNamespace) ? OmitContainingNamespaceParts.Root : OmitContainingNamespaceParts.None,
                scrollToContent: options.ScrollToContent);

            var generator = new MarkdownDocumentationGenerator(documentationModel, WellKnownUrlProviders.GitHub, documentationOptions);

            string path = options.OutputPath;

            WriteLine($"Documentation root is being generated to '{path}'.");

            string heading = options.Heading;

            if (string.IsNullOrEmpty(heading))
            {
                string fileName = Path.GetFileName(options.OutputPath);

                heading = (fileName.EndsWith(".dll", StringComparison.Ordinal))
                    ? Path.GetFileNameWithoutExtension(fileName)
                    : fileName;
            }

            DocumentationGeneratorResult result = generator.GenerateRoot(heading);

            File.WriteAllText(path, result.Content, _defaultEncoding);

            WriteLine($"Documentation root successfully generated to '{path}'.");

            return 0;
        }

        private static DocumentationModel CreateDocumentationModel(IEnumerable<string> assemblyReferences, IEnumerable<string> assemblies, DocumentationVisibility visibility, IEnumerable<string> additionalXmlDocumentationPaths = null)
        {
            var references = new List<PortableExecutableReference>();

            foreach (string path in assemblyReferences.SelectMany(f => GetAssemblyReferences(f)))
            {
                if (path == null)
                    return null;

                references.Add(MetadataReference.CreateFromFile(path));
            }

            foreach (string assemblyPath in assemblies)
            {
                if (!TryGetReference(references, assemblyPath, out PortableExecutableReference reference))
                {
                    if (File.Exists(assemblyPath))
                    {
                        reference = MetadataReference.CreateFromFile(assemblyPath);
                        references.Add(reference);
                    }
                    else
                    {
                        WriteLine($"Assembly not found: '{assemblyPath}'.");
                        return null;
                    }
                }
            }

            CSharpCompilation compilation = CSharpCompilation.Create(
                "",
                syntaxTrees: default(IEnumerable<SyntaxTree>),
                references: references,
                options: default(CSharpCompilationOptions));

            return new DocumentationModel(
                compilation,
                assemblies.Select(assemblyPath =>
                {
                    TryGetReference(references, assemblyPath, out PortableExecutableReference reference);
                    return (IAssemblySymbol)compilation.GetAssemblyOrModuleSymbol(reference);
                }),
                visibility: visibility,
                additionalXmlDocumentationPaths: additionalXmlDocumentationPaths);
        }

        private static IEnumerable<string> GetAssemblyReferences(string path)
        {
            if (!File.Exists(path))
            {
                WriteLine($"File not found: '{path}'.");
                return null;
            }

            if (string.Equals(Path.GetExtension(path), ".dll", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { path };
            }
            else
            {
                return File.ReadLines(path).Where(f => !string.IsNullOrWhiteSpace(f));
            }
        }

        private static bool TryGetIgnoredRootParts(IEnumerable<string> values, out RootDocumentationParts parts)
        {
            if (!values.Any())
            {
                parts = DocumentationOptions.Default.IgnoredRootParts;
                return true;
            }

            parts = RootDocumentationParts.None;

            foreach (string value in values)
            {
                if (Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out RootDocumentationParts result))
                {
                    parts |= result;
                }
                else
                {
                    WriteLine($"Unknown root documentation part '{value}'.");
                    return false;
                }
            }

            return true;
        }

        private static bool TryGetIgnoredNamespaceParts(IEnumerable<string> values, out NamespaceDocumentationParts parts)
        {
            if (!values.Any())
            {
                parts = DocumentationOptions.Default.IgnoredNamespaceParts;
                return true;
            }

            parts = NamespaceDocumentationParts.None;

            foreach (string value in values)
            {
                if (Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out NamespaceDocumentationParts result))
                {
                    parts |= result;
                }
                else
                {
                    WriteLine($"Unknown namespace documentation part '{value}'.");
                    return false;
                }
            }

            return true;
        }

        private static bool TryGetIgnoredTypeParts(IEnumerable<string> values, out TypeDocumentationParts parts)
        {
            if (!values.Any())
            {
                parts = DocumentationOptions.Default.IgnoredTypeParts;
                return true;
            }

            parts = TypeDocumentationParts.None;

            foreach (string value in values)
            {
                if (Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out TypeDocumentationParts result))
                {
                    parts |= result;
                }
                else
                {
                    WriteLine($"Unknown type documentation part '{value}'.");
                    return false;
                }
            }

            return true;
        }

        private static bool TryGetIgnoredMemberParts(IEnumerable<string> values, out MemberDocumentationParts parts)
        {
            if (!values.Any())
            {
                parts = DocumentationOptions.Default.IgnoredMemberParts;
                return true;
            }

            parts = MemberDocumentationParts.None;

            foreach (string value in values)
            {
                if (Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out MemberDocumentationParts result))
                {
                    parts |= result;
                }
                else
                {
                    WriteLine($"Unknown member documentation part '{value}'.");
                    return false;
                }
            }

            return true;
        }

        private static bool TryGetIgnoredDeclarationListParts(IEnumerable<string> values, out DeclarationListParts parts)
        {
            if (!values.Any())
            {
                parts = DeclarationListOptions.Default.IgnoredParts;
                return true;
            }

            parts = DeclarationListParts.None;

            foreach (string value in values)
            {
                if (Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out DeclarationListParts result))
                {
                    parts |= result;
                }
                else
                {
                    WriteLine($"Unknown declaration list part '{value}'.");
                    return false;
                }
            }

            return true;
        }

        private static bool TryGetOmitContainingNamespaceParts(IEnumerable<string> values, out OmitContainingNamespaceParts parts)
        {
            if (!values.Any())
            {
                parts = DocumentationOptions.Default.OmitContainingNamespaceParts;
                return true;
            }

            parts = OmitContainingNamespaceParts.None;

            foreach (string value in values)
            {
                if (Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out OmitContainingNamespaceParts result))
                {
                    parts |= result;
                }
                else
                {
                    WriteLine($"Unknown omit containing namespace part '{value}'.");
                    return false;
                }
            }

            return true;
        }

        private static bool TryGetVisibility(string value, out DocumentationVisibility visibility)
        {
            if (!Enum.TryParse(value.Replace("-", ""), ignoreCase: true, out visibility))
            {
                WriteLine($"Unknown visibility '{value}'.");
                return false;
            }

            return true;
        }

        private static bool TryGetReference(List<PortableExecutableReference> references, string path, out PortableExecutableReference reference)
        {
            if (path.Contains(Path.DirectorySeparatorChar))
            {
                foreach (PortableExecutableReference r in references)
                {
                    if (r.FilePath == path)
                    {
                        reference = r;
                        return true;
                    }
                }
            }
            else
            {
                foreach (PortableExecutableReference r in references)
                {
                    string filePath = r.FilePath;

                    int index = filePath.LastIndexOf(Path.DirectorySeparatorChar);

                    if (string.Compare(filePath, index + 1, path, 0, path.Length, StringComparison.Ordinal) == 0)
                    {
                        reference = r;
                        return true;
                    }
                }
            }

            reference = null;
            return false;
        }

        private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
        {
            public void Report(ProjectLoadProgress value)
            {
                string text = Path.GetFileName(value.FilePath);

                if (value.TargetFramework != null)
                    text += $" ({value.TargetFramework})";

                WriteLine($"  {value.Operation,-9} {value.ElapsedTime:mm\\:ss\\.ff}  {text}");
            }
        }
    }
}
