// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;
using System.Windows;

namespace Roslynator.VisualStudio
{
    internal static class RuleSetHelpers
    {
        private const string RuleSetXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!--

This rule set can be used to:

 1) Enable/disable analyzer(s) by DEFAULT.
 2) Change DEFAULT severity (action) of the analyzer(s).
 
Default configuration is applied once when analyzers are loaded.
Therefore, it may be neccessary to restart Visual Studio for changes to take effect.

Although it is possible to edit ruleset manually, Visual Studio has built-in support for editing ruleset.
Just add ruleset file to a solution and open it.

-->
<RuleSet Name=""roslynator.ruleset"" ToolsVersion=""16.0"">
  
  <!-- Specify default action that should be applied to all analyzers except those explicitly specified. -->
  <!-- <IncludeAll Action=""None,Hidden,Info,Warning,Error"" /> -->
  
  <!-- Specify zero or more paths to other rulesets that should be included. -->
  <!-- <Include Path="""" Action=""Default,None,Hidden,Info,Warning,Error"" /> -->

  <Rules AnalyzerId=""Roslynator.CSharp.Analyzers"" RuleNamespace=""Roslynator.CSharp.Analyzers"">
    <!-- <Rule Id=""RCS...."" Action=""None,Hidden,Info,Warning,Error"" /> -->
  </Rules>

</RuleSet>
";

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
