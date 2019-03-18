// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Roslynator.CodeFixes;
using Roslynator.Configuration;
using Roslynator.CSharp;

namespace Roslynator.VisualStudio
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("B804AA29-91D5-4C54-9B76-C442DA0AE60D")]
    public partial class CodeFixesOptionsPage : BaseOptionsPage
    {
        public CodeFixesOptionsPage()
        {
            Control.TitleColumnHeaderText = "Fix";
        }

        [Browsable(false)]
        public string DisabledCodeFixes
        {
            get { return string.Join(",", DisabledItems); }
            set
            {
                DisabledItems.Clear();

                if (!string.IsNullOrEmpty(value))
                {
                    foreach (string id in value.Split(','))
                    {
                        if (id.Contains("."))
                        {
                            DisabledItems.Add(id);
                        }
                        else if (id.StartsWith(CodeFixIdentifiers.Prefix, StringComparison.Ordinal))
                        {
                            if (CodeFixMap.CodeFixDescriptorsById.TryGetValue(id, out CodeFixDescriptor codeFixDescriptor))
                            {
                                foreach (string compilerDiagnosticId in codeFixDescriptor.FixableDiagnosticIds)
                                    DisabledItems.Add($"{compilerDiagnosticId}.{codeFixDescriptor.Id}");
                            }
                        }
                        else if (id.StartsWith("CS", StringComparison.Ordinal))
                        {
                            if (CodeFixMap.CodeFixDescriptorsByCompilerDiagnosticId.TryGetValue(id, out ReadOnlyCollection<CodeFixDescriptor> codeFixDescriptors))
                            {
                                foreach (CodeFixDescriptor codeFixDescriptor in codeFixDescriptors)
                                    DisabledItems.Add($"{id}.{codeFixDescriptor.Id}");
                            }
                        }
                        else
                        {
                            Debug.Fail(id);
                        }
                    }
                }
            }
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);

            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                SettingsManager.Instance.UpdateVisualStudioSettings(this);
                SettingsManager.Instance.ApplyTo(CodeFixSettings.Current);
            }
        }

        protected override void OnApply()
        {
            foreach (BaseModel model in Control.Items)
                SetIsEnabled(model.Id, model.Enabled);
        }

        protected override void Fill(ICollection<BaseModel> items)
        {
            items.Clear();

            foreach ((CodeFixDescriptor codeFixDescriptor, string compilerDiagnosticId) in CodeFixMap.CodeFixDescriptorsById
                .SelectMany(kvp => kvp.Value.FixableDiagnosticIds.Select(compilerDiagnosticId => (codeFixDescriptor: kvp.Value, compilerDiagnosticId: compilerDiagnosticId)))
                .OrderBy(f => f.compilerDiagnosticId)
                .ThenBy(f => f.codeFixDescriptor.Id))
            {
                var model = new CodeFixModel(compilerDiagnosticId, CodeFixMap.CompilerDiagnosticsById[compilerDiagnosticId].Title.ToString(), codeFixDescriptor.Id, codeFixDescriptor.Title);

                model.Enabled = IsEnabled(model.Id);

                items.Add(model);
            }
        }
    }
}
