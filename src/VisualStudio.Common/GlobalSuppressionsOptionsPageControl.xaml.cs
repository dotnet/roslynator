using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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
            RuleSetHelpers.EnsureRuleSetExistsInLocalAppData();

            string ruleSetPath = RuleSetHelpers.GetRuleSetPath();

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
    }
}
