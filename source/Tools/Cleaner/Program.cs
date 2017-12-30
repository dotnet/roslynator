// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cleaner
{
    internal static class Program
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

            CleanProjects(args);
#if DEBUG
            Console.WriteLine("DONE");
            Console.ReadKey();
#endif
        }

        private static void CleanProjects(IEnumerable<string> projectDirectories)
        {
            foreach (string projectDirectory in projectDirectories)
            {
                foreach (string directory in Directory.EnumerateDirectories(projectDirectory, "*", SearchOption.TopDirectoryOnly))
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
            Console.WriteLine(path);

            try
            {
                Directory.Delete(path, recursive: true);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
