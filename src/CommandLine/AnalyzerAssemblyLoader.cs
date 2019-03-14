// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
                using (IEnumerator<string> en = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories).GetEnumerator())
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
                        catch (Exception ex)
                        {
                            if (ex is IOException
                                || ex is SecurityException
                                || ex is UnauthorizedAccessException)
                            {
                                WriteLine(ex.Message, ConsoleColor.DarkGray, Verbosity.Diagnostic);
                                continue;
                            }
                            else
                            {
                                throw;
                            }
                        }

                        if (analyzerAssembly?.IsEmpty == false)
                            yield return new AnalyzerAssemblyInfo(analyzerAssembly, filePath);
                    }
                }
            }
            else
            {
                WriteLine($"File or directory not found: '{path}'", ConsoleColor.DarkGray, Verbosity.Normal);
            }

            AnalyzerAssembly Load(string filePath)
            {
                try
                {
                    return LoadFile(filePath, loadAnalyzers, loadFixers, language);
                }
                catch (Exception ex)
                {
                    if (ex is FileLoadException
                        || ex is BadImageFormatException
                        || ex is SecurityException)
                    {
                        WriteLine($"Cannot load assembly '{filePath}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

                        return null;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
