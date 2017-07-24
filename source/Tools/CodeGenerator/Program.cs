// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Roslynator.Metadata;

namespace CodeGenerator
{
    internal static class Program
    {
        private static readonly StringComparer _invariantComparer = StringComparer.InvariantCulture;

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

            var writer = new CodeFileWriter();

            RefactoringDescriptor[] refactorings = RefactoringDescriptor
                .LoadFromFile(Path.Combine(dirPath, @"Refactorings\Refactorings.xml"))
                .ToArray();

            var refactoringIdentifiersGenerator = new RefactoringIdentifiersGenerator();

            writer.SaveCode(
                Path.Combine(dirPath, @"Refactorings\RefactoringIdentifiers.Generated.cs"),
                refactoringIdentifiersGenerator.Generate(refactorings));

            var refactoringsOptionsPageGenerator = new RefactoringsOptionsPageGenerator();

            writer.SaveCode(
                Path.Combine(dirPath, @"VisualStudio.Core\RefactoringsOptionsPage.Generated.cs"),
                refactoringsOptionsPageGenerator.Generate(refactorings));

            CodeFixDescriptor[] codeFixes = CodeFixDescriptor
                .LoadFromFile(Path.Combine(dirPath, @"CodeFixes\CodeFixes.xml"))
                .ToArray();

            var codeFixIdentifiersGenerator = new CodeFixIdentifiersGenerator();

            writer.SaveCode(
                Path.Combine(dirPath, @"CodeFixes\CodeFixIdentifiers.Generated.cs"),
                codeFixIdentifiersGenerator.Generate(codeFixes));

            var codeFixesOptionsPageGenerator = new CodeFixesOptionsPageGenerator();

            writer.SaveCode(
                Path.Combine(dirPath, @"VisualStudio.Core\CodeFixesOptionsPage.Generated.cs"),
                codeFixesOptionsPageGenerator.Generate(codeFixes));

            CompilerDiagnosticDescriptor[] compilerDiagnostics = CompilerDiagnosticDescriptor
                .LoadFromFile(Path.Combine(dirPath, @"CodeFixes\Diagnostics.xml"))
                .OrderBy(f => f.Id, _invariantComparer)
                .ToArray();

            var compilerDiagnosticIdentifiersGenerator = new CompilerDiagnosticIdentifiersGenerator();

            writer.SaveCode(
                Path.Combine(dirPath, @"Core\CSharp\CompilerDiagnosticIdentifiers.cs"),
                compilerDiagnosticIdentifiersGenerator.Generate(compilerDiagnostics));

#if DEBUG
            Console.WriteLine("DONE");
            Console.ReadKey();
#endif
        }
    }
}
