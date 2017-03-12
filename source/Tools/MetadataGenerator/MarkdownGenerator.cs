// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Roslynator.Metadata;

namespace MetadataGenerator
{
    internal class MarkdownGenerator
    {
        //public Collection<AnalyzerInfo> Analyzers { get; } = new Collection<AnalyzerInfo>();
        //public Collection<RefactoringInfo> Refactorings { get; } = new Collection<RefactoringInfo>();

        private static StringComparer StringComparer { get; } = StringComparer.InvariantCulture;

        public string CreateReadMeMarkDown(IEnumerable<AnalyzerDescriptor> analyzers, IEnumerable<RefactoringDescriptor> refactorings)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine(File.ReadAllText(@"..\text\ReadMe.txt", Encoding.UTF8));
                sw.WriteLine("### List of Analyzers");
                sw.WriteLine();

                foreach (AnalyzerDescriptor info in analyzers.OrderBy(f => f.Id, StringComparer))
                {
                    sw.WriteLine($"* {info.Id} - {info.Title.TrimEnd('.').EscapeMarkdown()}");
                }

                sw.WriteLine();
                sw.WriteLine("### List of Refactorings");
                sw.WriteLine();

                foreach (RefactoringDescriptor info in refactorings.OrderBy(f => f.Title, StringComparer))
                {
                    sw.WriteLine($"* [{info.Title.TrimEnd('.').EscapeMarkdown()}](source/Refactorings/Refactorings.md#{info.GetGitHubHref()})");
                }

                return sw.ToString();
            }
        }

        public string CreateRefactoringsMarkDown(IEnumerable<RefactoringDescriptor> refactorings)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("## " + "Roslynator Refactorings");

                foreach (RefactoringDescriptor info in refactorings
                    .OrderBy(f => f.Title, StringComparer))
                {
                    sw.WriteLine("");
                    sw.WriteLine("#### " + info.Title.EscapeMarkdown());
                    sw.WriteLine("");
                    sw.WriteLine($"* **Syntax**: {string.Join(", ", info.Syntaxes.Select(f => f.Name.EscapeMarkdown()))}");

                    if (!string.IsNullOrEmpty(info.Scope))
                        sw.WriteLine($"* **Scope**: {info.Scope.EscapeMarkdown()}");

                    sw.WriteLine("");

                    if (info.Images.Count > 0)
                    {
                        bool isFirst = true;

                        foreach (ImageDescriptor image in info.Images)
                        {
                            if (!isFirst)
                                sw.WriteLine();

                            sw.WriteLine(CreateImageMarkDown(info, image.Name));
                            isFirst = false;
                        }
                    }
                    else
                    {
                        sw.WriteLine(CreateImageMarkDown(info, info.Identifier));
                    }
                }

                return sw.ToString();
            }
        }

        public string CreateAnalyzersReadMe(IEnumerable<AnalyzerDescriptor> analyzers)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("## Roslynator Analyzers");
                sw.WriteLine();

                sw.WriteLine(" Id | Title | Category | Enabled by Default ");
                sw.WriteLine(" --- | --- | --- |:---:");

                foreach (AnalyzerDescriptor info in analyzers.OrderBy(f => f.Id, StringComparer))
                {
                    sw.Write(info.Id);
                    sw.Write('|');
                    sw.Write(info.Title.TrimEnd('.').EscapeMarkdown());
                    sw.Write('|');
                    sw.Write(info.Category.EscapeMarkdown());
                    sw.Write('|');
                    sw.Write((info.IsEnabledByDefault) ? "x" : "");

                    sw.WriteLine();
                }

                return sw.ToString();
            }
        }

        public string CreateRefactoringsReadMe(IEnumerable<RefactoringDescriptor> refactorings)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("## Roslynator Refactorings");
                sw.WriteLine();

                sw.WriteLine("Id | Title | Enabled by Default ");
                sw.WriteLine("--- | --- |:---:");

                foreach (RefactoringDescriptor info in refactorings.OrderBy(f => f.Title, StringComparer))
                {
                    sw.Write(info.Id);
                    sw.Write('|');
                    sw.Write($"[{info.Title.TrimEnd('.').EscapeMarkdown()}](Refactorings.md#{info.GetGitHubHref()})");
                    sw.Write('|');
                    sw.Write((info.IsEnabledByDefault) ? "x" : "");
                    sw.WriteLine();
                }

                return sw.ToString();
            }
        }

        public string CreateAnalyzersByCategoryMarkDown(IEnumerable<AnalyzerDescriptor> analyzers)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("## Roslynator Analyzers by Category");
                sw.WriteLine();

                sw.WriteLine(" Category | Title | Id | Enabled by Default ");
                sw.WriteLine(" --- | --- | --- |:---:");

                foreach (IGrouping<string, AnalyzerDescriptor> grouping in analyzers
                    .GroupBy(f => f.Category.EscapeMarkdown())
                    .OrderBy(f => f.Key, StringComparer))
                {
                    foreach (AnalyzerDescriptor info in grouping.OrderBy(f => f.Title, StringComparer))
                    {
                        sw.Write(grouping.Key);
                        sw.Write('|');
                        sw.Write(info.Title.TrimEnd('.').EscapeMarkdown());
                        sw.Write('|');
                        sw.Write(info.Id);
                        sw.Write('|');
                        sw.Write((info.IsEnabledByDefault) ? "x" : "");

                        sw.WriteLine();
                    }
                }

                return sw.ToString();
            }
        }

        private void WriteAnalyzersTable(IEnumerable<AnalyzerDescriptor> infos, StringWriter sw)
        {
            sw.WriteLine(" Id | Title | Enabled by Default ");
            sw.WriteLine(" --- | --- |:---:");

            foreach (AnalyzerDescriptor info in infos)
            {
                sw.Write(info.Id);
                sw.Write('|');
                sw.Write(info.Title.TrimEnd('.').EscapeMarkdown());
                sw.Write('|');
                sw.Write((info.IsEnabledByDefault) ? "x" : "");

                sw.WriteLine();
            }
        }

        public static IEnumerable<string> FindMissingImages(IEnumerable<RefactoringDescriptor> refactorings, string imagesDirPath)
        {
            foreach (RefactoringDescriptor refactoring in refactorings
                .OrderBy(f => f.Title, StringComparer))
            {
                foreach (ImageDescriptor image in refactoring.ImagesOrDefaultImage())
                {
                    string imagePath = Path.Combine(imagesDirPath, image.Name + ".png");

                    if (!File.Exists(imagePath))
                        yield return imagePath;
                }
            }
        }

        private static string CreateImageMarkDown(RefactoringDescriptor refactoring, string fileName)
        {
            return $"![{refactoring.Title.EscapeMarkdown()}](../../images/refactorings/{fileName.EscapeMarkdown()}.png)";
        }
    }
}
