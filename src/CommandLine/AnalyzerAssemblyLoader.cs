// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal static class AnalyzerAssemblyLoader
    {
        public const string DefaultSearchPattern = "*.dll";

        public static AnalyzerAssembly LoadFile(
            string filePath,
            bool loadAnalyzers = true,
            bool loadFixers = true,
            string language = null)
        {
            Assembly assembly = Assembly.LoadFrom(filePath);

            return AnalyzerAssembly.Load(assembly, loadAnalyzers: loadAnalyzers, loadFixers: loadFixers, language: language);
        }

        public static IEnumerable<AnalyzerAssemblyInfo> LoadFrom(
            string path,
            string searchPattern = DefaultSearchPattern,
            bool loadAnalyzers = true,
            bool loadFixers = true,
            string language = null)
        {
            if (File.Exists(path))
            {
                AnalyzerAssembly analyzerAssembly = Load(path);

                if (analyzerAssembly?.IsEmpty == false)
                    yield return new AnalyzerAssemblyInfo(analyzerAssembly, path);
            }
            else if (Directory.Exists(path))
            {
                using (IEnumerator<string> en = Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories).GetEnumerator())
                {
                    while (true)
                    {
                        string filePath = null;
                        AnalyzerAssembly analyzerAssembly = null;

                        try
                        {
                            if (en.MoveNext())
                            {
                                filePath = en.Current;
                                analyzerAssembly = Load(filePath);
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (Exception ex) when (ex is IOException
                            || ex is SecurityException
                            || ex is UnauthorizedAccessException)
                        {
                            WriteError(ex, ConsoleColor.DarkGray, Verbosity.Diagnostic);
                            continue;
                        }

                        if (analyzerAssembly?.IsEmpty == false)
                            yield return new AnalyzerAssemblyInfo(analyzerAssembly, filePath);
                    }
                }
            }
            else
            {
                WriteLine($"File or directory not found: '{path}'", ConsoleColors.DarkGray, Verbosity.Normal);
            }

            AnalyzerAssembly Load(string filePath)
            {
                try
                {
                    return LoadFile(filePath, loadAnalyzers, loadFixers, language);
                }
                catch (Exception ex) when (ex is FileLoadException
                    || ex is BadImageFormatException
                    || ex is SecurityException)
                {
                    WriteLine($"Cannot load assembly '{filePath}'", ConsoleColors.DarkGray, Verbosity.Diagnostic);

                    return null;
                }
            }
        }
    }
}
