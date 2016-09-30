// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cleaner
{
    internal class Program
    {
#if DEBUG
        private static readonly string _currentDirectory = @"D:\Documents\Visual Studio 2015\Projects\Pihrtsoft.CodeAnalysis";
#else
        private static readonly string _currentDirectory = Path.GetDirectoryName(Environment.CurrentDirectory);
#endif

        private static void Main(string[] args)
        {
            CleanProjects(new string[]
            {
                "source"
            });

            Console.WriteLine("*** FINISHED ***");
            Console.ReadKey();
        }

        private static void CleanProjects(IEnumerable<string> projectDirectories)
        {
            foreach (string projectDirectory in projectDirectories)
            {
                foreach (string directory in Directory.EnumerateDirectories(
                    Path.Combine(_currentDirectory, projectDirectory),
                    "*",
                    SearchOption.TopDirectoryOnly))
                {
                    CleanProject(directory);
                }
            }
        }

        private static void CleanProject(string projectDirectory)
        {
            if (!Directory.EnumerateFiles(projectDirectory, "*.csproj", SearchOption.TopDirectoryOnly).Any())
                return;

            CleanDirectory(Path.Combine(projectDirectory, "bin"));
            CleanDirectory(Path.Combine(projectDirectory, "obj"));
        }

        private static void CleanDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                return;

            foreach (string item in Directory.EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly))
                DeleteDirectory(item);
        }

        private static void DeleteDirectory(string path)
        {
            Console.WriteLine($"{GetRelativePath(path)}");

            try
            {
                Directory.Delete(path, recursive: true);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
            }
        }

        private static string GetRelativePath(string path)
        {
            return path.Replace(_currentDirectory, "");
        }
    }
}
