// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    [Obsolete]
    internal class GeneratedCodeAnalyzer
    {
        public virtual StringComparison StringComparison { get; } = StringComparison.OrdinalIgnoreCase;

        public virtual bool IsGeneratedCode(CodeBlockAnalysisContext context)
        {
            return IsGeneratedCode(context.CodeBlock.SyntaxTree);
        }

        public virtual bool IsGeneratedCode(SyntaxNodeAnalysisContext context)
        {
            return IsGeneratedCode(context.Node.SyntaxTree);
        }

        public virtual bool IsGeneratedCode(SyntaxTreeAnalysisContext context)
        {
            return IsGeneratedCode(context.Tree);
        }

        public virtual bool IsGeneratedCode(SymbolAnalysisContext context)
        {
            return IsGeneratedCode(context.Symbol);
        }

        protected virtual bool IsGeneratedCode(SyntaxTree tree)
        {
            return IsGeneratedCodeFile(tree.FilePath);
        }

        private bool IsGeneratedCode(ISymbol symbol)
        {
            for (int i = 0; i < symbol.DeclaringSyntaxReferences.Length; i++)
            {
                if (IsGeneratedCode(symbol.DeclaringSyntaxReferences[i].SyntaxTree))
                    return true;
            }

            return false;
        }

        private bool IsGeneratedCodeFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            string fileName = Path.GetFileName(filePath);

            if (fileName.StartsWith("TemporaryGeneratedFile_", StringComparison))
                return true;

            string extension = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(extension))
                return false;

            string fileNameWithoutExtension = fileName.Substring(0, fileName.Length - extension.Length);

            return string.Equals(fileNameWithoutExtension, "AssemblyInfo", StringComparison)
                || fileNameWithoutExtension.EndsWith(".Designer", StringComparison)
                || fileNameWithoutExtension.EndsWith(".Generated", StringComparison)
                || fileNameWithoutExtension.EndsWith(".g", StringComparison)
                || fileNameWithoutExtension.EndsWith(".g.i", StringComparison)
                || fileNameWithoutExtension.EndsWith(".AssemblyAttributes", StringComparison);
        }
    }
}
