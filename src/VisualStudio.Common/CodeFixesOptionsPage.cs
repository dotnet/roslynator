// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Roslynator.CodeFixes;
using Roslynator.Configuration;

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
                        else if (id.StartsWith(CodeFixIdentifier.CodeFixIdPrefix, StringComparison.Ordinal))
                        {
                            foreach (string compilerDiagnosticId in CodeFixMap.GetCompilerDiagnosticIds(id))
                                DisabledItems.Add($"{compilerDiagnosticId}.{id}");
                        }
                        else if (id.StartsWith("CS", StringComparison.Ordinal))
                        {
                            foreach (string codeFixId in CodeFixMap.GetCodeFixIds(id))
                                DisabledItems.Add($"{id}.{codeFixId}");
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
                UpdateConfig();
            }
        }

        protected override void OnApply()
        {
            foreach (BaseModel model in Control.Items)
                SetIsEnabled(model.Id, model.Enabled);
        }

        internal void UpdateConfig()
        {
            IEnumerable<KeyValuePair<string, bool>> codeFixes = GetDisabledItems()
                .Select(f => new KeyValuePair<string, bool>(f, false));

            CodeAnalysisConfig.UpdateVisualStudioConfig(f =>  f.WithCodeFixes(codeFixes));
        }

        protected override void Fill(ICollection<BaseModel> items)
        {
            items.Clear();

            foreach (CompilerDiagnosticFix compilerDiagnosticFix in CodeFixMap.GetCompilerDiagnosticFixes()
                .OrderBy(f => f.CompilerDiagnosticId)
                .ThenBy(f => f.CodeFixId))
            {
                var model = new CodeFixModel(
                    compilerDiagnosticFix.CompilerDiagnosticId,
                    compilerDiagnosticFix.CompilerDiagnosticTitle,
                    compilerDiagnosticFix.CodeFixId,
                    compilerDiagnosticFix.CodeFixTitle);

                model.Enabled = IsEnabled(model.Id);

                items.Add(model);
            }
        }
    }
}
