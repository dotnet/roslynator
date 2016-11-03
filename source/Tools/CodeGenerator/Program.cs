// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Roslynator.Metadata;

namespace CodeGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
#if DEBUG
                args = new string[] { @"..\..\..\.." };
#else
                args = new string[] { Environment.CurrentDirectory };
#endif
            }

            string dirPath = args[0];

            RefactoringInfo[] refactorings = RefactoringInfo
                .LoadFromFile(Path.Combine(dirPath, @"Refactorings\Refactorings.xml"))
                .OrderBy(f => f.Identifier, StringComparer.InvariantCulture)
                .ToArray();

            var writer = new CodeFileWriter();

            var refactoringIdentifiersGenerator = new RefactoringIdentifiersGenerator();
            writer.SaveCode(
                 Path.Combine(dirPath, @"Refactorings\RefactoringIdentifiers.cs"),
                refactoringIdentifiersGenerator.Generate(refactorings));

            var optionsPagePropertiesGenerator = new OptionsPagePropertiesGenerator();
            writer.SaveCode(
                 Path.Combine(dirPath, @"VisualStudio.Common\RefactoringsOptionsPage.Generated.cs"),
                optionsPagePropertiesGenerator.Generate(refactorings));

#if DEBUG
            Console.WriteLine("DONE");
            Console.ReadKey();
#endif
        }
    }
}
