// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Roslynator.VisualStudio
{
    /// <summary>
    /// Interaction logic for AnalyzersOptionsPageControl.xaml
    /// </summary>
    public partial class AnalyzersOptionsPageControl : UserControl
    {
        public AnalyzersOptionsPageControl()
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
                catch (Exception ex) when (ex is InvalidOperationException
                    || ex is FileNotFoundException
                    || ex is Win32Exception)
                {
                    MessageBox.Show(ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            e.Handled = true;
        }
    }
}
