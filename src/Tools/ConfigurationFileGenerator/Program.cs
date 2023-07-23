// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;
using Roslynator.CodeGeneration.EditorConfig;

namespace Roslynator.CodeGeneration;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Invalid number of arguments");
            return;
        }

        if (args is null || args.Length == 0)
        {
            args = new string[] { Environment.CurrentDirectory };
        }

        string rootPath = args[0];
        string configurationSourcePath = args[1];
        string configurationDestinationPath = args[2];

        var metadata = new RoslynatorMetadata(rootPath);

        string configFileContent = File.ReadAllText(configurationSourcePath);

        configFileContent += @"# Full List of Options

```editorconfig title="".editorconfig"""
            + EditorConfigGenerator.GenerateEditorConfig(metadata, commentOut: false)
            + @"```
";

        var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        string directoryPath = Path.GetDirectoryName(configurationDestinationPath);

        if (directoryPath.Length > 0)
            Directory.CreateDirectory(directoryPath);

        File.WriteAllText(
            configurationDestinationPath,
            configFileContent,
            utf8NoBom);
    }
}
