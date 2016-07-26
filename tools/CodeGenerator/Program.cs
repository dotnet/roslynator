// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Pihrtsoft.CodeAnalysis.Metadata;

namespace CodeGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RefactoringInfo[] refactorings = RefactoringInfo
                .LoadFromFile(@"..\source\Refactorings\Refactorings.xml")
                .OrderBy(f => f.Identifier, StringComparer.InvariantCulture)
                .ToArray();

            var writer = new CodeFileWriter();

            var refactoringIdentifiersGenerator = new RefactoringIdentifiersGenerator();
            writer.SaveCode(
                @"..\source\Refactorings\RefactoringIdentifiers.cs",
                refactoringIdentifiersGenerator.Generate(refactorings));

            var optionsPagePropertiesGenerator = new OptionsPagePropertiesGenerator();
            writer.SaveCode(
                @"..\source\VisualStudio.Common\RefactoringsOptionsPage.Generated.cs",
                optionsPagePropertiesGenerator.Generate(refactorings));

            Console.WriteLine("*** FINISHED ***");
            Console.ReadKey();
        }
    }
}
