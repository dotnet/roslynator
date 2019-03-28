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
            string assemblyPath = typeof(GlobalSuppressionsOptionsPageControl).Assembly.Location;

            if (!string.IsNullOrEmpty(assemblyPath))
            {
                string assemblyDirPath = Path.GetDirectoryName(assemblyPath);

                if (!string.IsNullOrEmpty(assemblyDirPath))
                {
                    string ruleSetPath = Path.Combine(assemblyDirPath, "roslynator.ruleset");

                    try
                    {
                        string fileToSelect = (File.Exists(ruleSetPath)) ? ruleSetPath : assemblyPath;

                        Process.Start("explorer.exe", $"/select, \"{fileToSelect}\"");
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
            }

            e.Handled = true;
        }
    }
}
