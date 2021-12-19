// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace Roslynator.VisualStudio
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class GeneralOptionsPage : UIElementDialogPage
    {
        private bool _isActive;

        private readonly GeneralOptionsPageControl _control = new();

        protected override UIElement Child
        {
            get { return _control; }
        }

        [Category("General")]
        [Browsable(false)]
        public string ApplicationVersion { get; set; }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            if (!_isActive)
            {
                _isActive = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _isActive = false;
        }
    }
}
