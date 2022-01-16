// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace Roslynator.VisualStudio
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("9851B486-A654-4F6A-B1BA-9E1071DCCA25")]
    public class AnalyzersOptionsPage : UIElementDialogPage
    {
        private readonly AnalyzersOptionsPageControl _control = new();

        protected override UIElement Child => _control;
    }
}
