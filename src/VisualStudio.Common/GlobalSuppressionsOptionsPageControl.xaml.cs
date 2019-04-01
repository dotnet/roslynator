using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Roslynator.CSharp;
using System.ComponentModel;

namespace Roslynator.VisualStudio
{
    /// <summary>
    /// Interaction logic for GlobalSuppressionsOptionsPageControl.xaml
    /// </summary>
    public partial class GlobalSuppressionsOptionsPageControl : UserControl
    {
        public GlobalSuppressionsOptionsPageControl()
        {
            InitializeComponent();
        }

        private void OpenLocation_Click(object sender, RoutedEventArgs e)
        {
            string appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                @"JosefPihrt\Roslynator\VisualStudio\2019");
            string ruleSetPath = Path.Combine(appDataFolder, "roslynator.ruleset");

            if (!File.Exists(ruleSetPath))
            {
                try
                {
                    string defaultRulesetFileName = GetDefaultRulesetFileName();
                    if (defaultRulesetFileName != null && File.Exists(defaultRulesetFileName))
                    {
                        if (!Directory.Exists(appDataFolder))
                        {
                            Directory.CreateDirectory(appDataFolder);
                        }
                        File.Copy(defaultRulesetFileName, ruleSetPath);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is InvalidOperationException
                        || ex is FileNotFoundException
                        || ex is Win32Exception)
                    {
                        MessageBox.Show(ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            if (File.Exists(ruleSetPath))
            {
                try
                {
                    Process.Start("explorer.exe", $"/select, \"{ruleSetPath}\"");
                }
                catch (Exception ex)
                {
                    if (ex is InvalidOperationException
                        || ex is FileNotFoundException
                        || ex is Win32Exception)
                    {
                        MessageBox.Show(ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            e.Handled = true;
        }

        private string GetDefaultRulesetFileName()
        {
            string assemblyPath = typeof(GlobalSuppressionsOptionsPageControl).Assembly.Location;
            if (!string.IsNullOrEmpty(assemblyPath))
            {
                string assemblyDirPath = Path.GetDirectoryName(assemblyPath);

                if (!string.IsNullOrEmpty(assemblyDirPath))
                {
                    return  Path.Combine(assemblyDirPath, "roslynator.ruleset");
                }
            }
            return null;
        }
    }
}
