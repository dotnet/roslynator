// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#region usings
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;
using Roslynator.CSharp.Tests;
#endregion usings

namespace Roslynator.Tests
{
    internal static class Program
    {
        internal static async Task Main()
        {
            const string source = @"
using System;
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
    }   
}
";
            using (Workspace workspace = new AdhocWorkspace())
            {
                Project project = CSharpWorkspaceFactory.Instance.AddProject(workspace.CurrentSolution);

                Document document = CSharpWorkspaceFactory.Instance.AddDocument(project, source);

                SemanticModel semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
                SyntaxTree tree = await document.GetSyntaxTreeAsync().ConfigureAwait(false);
                SyntaxNode root = await tree.GetRootAsync().ConfigureAwait(false);

                string s = document.GetSyntaxRootAsync().Result.ToFullString();
                Console.WriteLine(s);
                Console.ReadKey();
            }
        }
    }
}
