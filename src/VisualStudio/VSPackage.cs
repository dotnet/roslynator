// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Roslynator.VisualStudio
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [Guid("7AD86013-9E55-4BBE-98CC-DE72FADAB1E6")]
    [ProvideOptionPage(typeof(GeneralOptionsPage), "Roslynator", "General", 0, 0, true)]
    [ProvideOptionPage(typeof(RefactoringsOptionsPage), "Roslynator", "Refactorings", 0, 0, true)]
    [ProvideOptionPage(typeof(CodeFixesOptionsPage), "Roslynator", "Code Fixes", 0, 0, true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string)]
    public sealed partial class VSPackage : Package, IVsSolutionEvents
    {
    }
}
