// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Roslynator.CodeGeneration.EditorConfig;
using Roslynator.Metadata;

namespace Roslynator.CodeGeneration;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args is null || args.Length == 0)
        {
            args = new string[] { Environment.CurrentDirectory };
        }

        string rootPath = args[0];

        var metadata = new RoslynatorMetadata(rootPath);

        string configFileContent = File.ReadAllText("configuration.md");

        configFileContent += @"# Full List of Options

```editorconfig"
            + EditorConfigGenerator.GenerateEditorConfig(metadata, commentOut: false)
            + @"```
";

        var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        File.WriteAllText(
            Path.Combine("configuration.md"),
            configFileContent,
            utf8NoBom);
    }
}
