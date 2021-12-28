// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;

namespace Roslynator.Configuration
{
    internal class EditorConfigWriter : IDisposable
    {
        public const string AnalyzerCategoryPrefix = "dotnet_analyzer_diagnostic.category-";

        private readonly TextWriter _writer;
        private bool _disposed;

        public EditorConfigWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public void WriteGlobalDirective()
        {
            WriteEntry("is_global", true);
        }

        public void WriteEntries(IEnumerable<KeyValuePair<string, string>> entries, string keyPrefix = null)
        {
            foreach (KeyValuePair<string, string> entry in entries)
            {
                _writer.Write(keyPrefix);
                WriteEntry(entry);
            }
        }

        public void WriteRefactorings(IEnumerable<KeyValuePair<string, bool>> entries)
        {
            foreach (KeyValuePair<string, bool> entry in entries)
                WriteRefactoring(entry.Key, entry.Value);
        }

        public void WriteRefactoring(string id, bool enabled)
        {
            _writer.Write(ConfigOptionKeys.RefactoringPrefix);
            _writer.Write(id);
            _writer.Write(".enabled");
            WriteSeparator();
            WriteValue(enabled);
            WriteLine();
        }

        public void WriteCompilerDiagnosticFixes(IEnumerable<KeyValuePair<string, bool>> entries)
        {
            foreach (KeyValuePair<string, bool> entry in entries)
                WriteCompilerDiagnosticFix(entry.Key, entry.Value);
        }

        public void WriteCompilerDiagnosticFix(string id, bool enabled)
        {
            _writer.Write(ConfigOptionKeys.CompilerDiagnosticFixPrefix);
            _writer.Write(id);
            _writer.Write(".enabled");
            WriteSeparator();
            WriteValue(enabled);
            WriteLine();
        }

        public void WriteAnalyzers(IEnumerable<KeyValuePair<string, ReportDiagnostic>> entries)
        {
            foreach (KeyValuePair<string, ReportDiagnostic> entry in entries)
                WriteAnalyzer(entry.Key, entry.Value);
        }

        public void WriteAnalyzer(string id, ReportDiagnostic reportDiagnostic)
        {
            WriteEntry($"dotnet_diagnostic.{id}.severity", reportDiagnostic);
        }

        public void WriteAnalyzerCategory(string categoryName, ReportDiagnostic reportDiagnostic)
        {
            WriteAnalyzerCategory(categoryName, MapReportDiagnostic(reportDiagnostic));
        }

        public void WriteAnalyzerCategory(string categoryName, string severity)
        {
            WriteEntry($"{AnalyzerCategoryPrefix}{categoryName}.severity", severity);
        }

        public void WriteEntry(KeyValuePair<string, string> entry)
        {
            WriteEntry(entry.Key, entry.Value);
        }

        public void WriteEntry(string key, bool value)
        {
            WriteEntry(key, (value) ? "true" : "false");
        }

        public void WriteEntry(string key, ReportDiagnostic reportDiagnostic)
        {
            WriteEntry(key, MapReportDiagnostic(reportDiagnostic));
        }

        public void WriteEntry(string key, string value)
        {
            _writer.Write(key);
            WriteSeparator();
            _writer.WriteLine(value);
        }

        private void WriteSeparator()
        {
            _writer.Write(" = ");
        }

        public void WriteCommentChar()
        {
            Write("#");
        }

        private void WriteValue(bool value)
        {
            _writer.Write((value) ? "true" : "false");
        }

        public void Write(string value)
        {
            _writer.Write(value);
        }

        public void WriteLine()
        {
            _writer.WriteLine();
        }

        public void WriteLine(string value)
        {
            _writer.WriteLine(value);
        }

        public void WriteLineIf(bool condition)
        {
            if (condition)
                _writer.WriteLine();
        }

        public override string ToString()
        {
            return _writer.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _writer.Dispose();

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private static string MapReportDiagnostic(ReportDiagnostic reportDiagnostic)
        {
            switch (reportDiagnostic)
            {
                case ReportDiagnostic.Default:
                    return "default";
                case ReportDiagnostic.Error:
                    return "error";
                case ReportDiagnostic.Warn:
                    return "warning";
                case ReportDiagnostic.Info:
                    return "suggestion";
                case ReportDiagnostic.Hidden:
                    return "silent";
                case ReportDiagnostic.Suppress:
                    return "none";
                default:
                    throw new InvalidOperationException($"Unknown enum value '{reportDiagnostic}'.");
            }
        }
    }
}
