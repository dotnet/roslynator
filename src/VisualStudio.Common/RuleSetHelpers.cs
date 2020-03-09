// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;
using System.Windows;

namespace Roslynator.VisualStudio
{
    internal static partial class RuleSetHelpers
    {
        private const string RuleSetFileName = "roslynator.ruleset";

        public static string GetRuleSetDirectoryPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"JosefPihrt\Roslynator\VisualStudio\2019");
        }

        public static string GetRuleSetPath()
        {
            return Path.Combine(GetRuleSetDirectoryPath(), RuleSetFileName);
        }

        public static void EnsureRuleSetExistsInLocalAppData(bool showErrorMessage = false)
        {
            string directoryPath = GetRuleSetDirectoryPath();

            string ruleSetPath = Path.Combine(directoryPath, RuleSetFileName);

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
