// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;
using System.Windows;

namespace Roslynator.VisualStudio
{
    internal static partial class DefaultRuleSet
    {
        private const string FileName = "roslynator.ruleset";

        public static string GetDirectoryPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "JosefPihrt",
                "Roslynator");
        }

        public static string GetFilePath()
        {
            return Path.Combine(GetDirectoryPath(), FileName);
        }

        public static void CreateFileIfNotExists(bool showErrorMessage = false)
        {
            string directoryPath = GetDirectoryPath();

            string ruleSetPath = Path.Combine(directoryPath, FileName);

            if (!File.Exists(ruleSetPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);

                    File.WriteAllText(ruleSetPath, RuleSetXml, Encoding.UTF8);
                }
                catch (Exception ex) when (ex is IOException
                    || ex is UnauthorizedAccessException)
                {
                    if (showErrorMessage)
                        MessageBox.Show(ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
