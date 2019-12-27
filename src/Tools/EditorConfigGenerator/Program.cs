// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using System.Text;
using Roslynator.Metadata;
using Roslynator.Utilities;

namespace Roslynator.CodeGeneration
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            string rootPath = @"..\..\..\..\..";
#else
            string rootPath = System.Environment.CurrentDirectory;
#endif
            if (args?.Length > 0)
                rootPath = args[0];

            var metadata = new RoslynatorMetadata(rootPath);

            string path = Path.Combine(rootPath, "default.editorconfig");

            var sb = new StringBuilder();

            foreach (AnalyzerMetadata analyzer in metadata.Analyzers
                .Where(f => !f.IsObsolete)
                .OrderBy(f => f.Id))
            {
                sb.Append("dotnet_diagnostic.");
                sb.Append(analyzer.Id);
                sb.AppendLine(".severity = default");
            }

            string content = sb.ToString();

            FileHelper.WriteAllText(path, content, Encoding.UTF8, onlyIfChanges: false, fileMustExists: false);
        }
    }
}
