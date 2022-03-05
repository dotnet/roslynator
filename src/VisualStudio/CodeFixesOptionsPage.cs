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
        public string DisabledCodeFixes { get; set; }

        [Browsable(false)]
        public string CodeFixes
        {
            get { return string.Join(",", Items.Select(f => (f.Value) ? f.Key : (f.Key + "!"))); }
            set
            {
                Items.Clear();

                if (!string.IsNullOrEmpty(value))
                {
                    foreach (string s in value.Split(','))
                    {
                        string id = s;
                        var enabled = true;

                        if (s.EndsWith("!"))
                        {
                            id = s.Remove(s.Length - 1);
                            enabled = false;
                        }

                        if (id.Length > 0)
                        {
                            if (id.Contains("."))
                            {
                                Items[id] = enabled;
                            }
                            else if (id.StartsWith(CodeFixIdentifier.CodeFixIdPrefix, StringComparison.Ordinal))
                            {
                                foreach (string compilerDiagnosticId in CodeFixMap.GetCompilerDiagnosticIds(id))
                                    Items[$"{compilerDiagnosticId}.{id}"] = enabled;
                            }
                            else if (id.StartsWith("CS", StringComparison.Ordinal))
                            {
                                foreach (string codeFixId in CodeFixMap.GetCodeFixIds(id))
                                    Items[$"{id}.{codeFixId}"] = enabled;
                            }
                            else
                            {
                                Debug.Fail(id);
                            }
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

        internal void UpdateConfig()
        {
            CodeAnalysisConfig.UpdateVisualStudioConfig(f => f.WithCodeFixes(GetItems()));
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
