// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis;
using Roslynator.CodeFixes;
using Roslynator.CSharp;

namespace Roslynator
{
    public static class CodeFixMap
    {
        private static ReadOnlyDictionary<string, CodeFixDescriptor> _codeFixDescriptorsById;
        private static ReadOnlyDictionary<string, ReadOnlyCollection<CodeFixDescriptor>> _codeFixDescriptorsByCompilerDiagnosticId;
        private static ReadOnlyDictionary<string, DiagnosticDescriptor> _compilerDiagnosticsById;

        public static ReadOnlyDictionary<string, CodeFixDescriptor> CodeFixDescriptorsById
        {
            get
            {
                if (_codeFixDescriptorsById == null)
                    Interlocked.CompareExchange(ref _codeFixDescriptorsById, LoadCodeFixDescriptorsById(), null);

                return _codeFixDescriptorsById;
            }
        }

        private static ReadOnlyDictionary<string, CodeFixDescriptor> LoadCodeFixDescriptorsById()
        {
            Dictionary<string, CodeFixDescriptor> dic = typeof(CodeFixDescriptors)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(f => (CodeFixDescriptor)f.GetValue(null))
                .ToDictionary(f => f.Id, f => f);

            return new ReadOnlyDictionary<string, CodeFixDescriptor>(dic);
        }

        public static ReadOnlyDictionary<string, ReadOnlyCollection<CodeFixDescriptor>> CodeFixDescriptorsByCompilerDiagnosticId
        {
            get
            {
                if (_codeFixDescriptorsByCompilerDiagnosticId == null)
                    Interlocked.CompareExchange(ref _codeFixDescriptorsByCompilerDiagnosticId, LoadCodeFixDescriptorsByCompilerDiagnosticId(), null);

                return _codeFixDescriptorsByCompilerDiagnosticId;
            }
        }

        private static ReadOnlyDictionary<string, ReadOnlyCollection<CodeFixDescriptor>> LoadCodeFixDescriptorsByCompilerDiagnosticId()
        {
            Dictionary<string, ReadOnlyCollection<CodeFixDescriptor>> dic = typeof(CodeFixDescriptors)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(f => (CodeFixDescriptor)f.GetValue(null))
                .SelectMany(f => f.FixableDiagnosticIds.Select(id => (codeFixDescriptor: f, id: id)))
                .GroupBy(f => f.id)
                .ToDictionary(f => f.Key, f => new ReadOnlyCollection<CodeFixDescriptor>(f.Select(ff => ff.codeFixDescriptor).ToList()));

            return new ReadOnlyDictionary<string, ReadOnlyCollection<CodeFixDescriptor>>(dic);
        }

        public static ReadOnlyDictionary<string, DiagnosticDescriptor> CompilerDiagnosticsById
        {
            get
            {
                if (_compilerDiagnosticsById == null)
                    Interlocked.CompareExchange(ref _compilerDiagnosticsById, LoadCompilerDiagnosticsById(), null);

                return _compilerDiagnosticsById;
            }
        }

        private static ReadOnlyDictionary<string, DiagnosticDescriptor> LoadCompilerDiagnosticsById()
        {
            Dictionary<string, DiagnosticDescriptor> dic = typeof(CompilerDiagnosticDescriptors)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(f => (DiagnosticDescriptor)f.GetValue(null))
                .ToDictionary(f => f.Id, f => f);

            return new ReadOnlyDictionary<string, DiagnosticDescriptor>(dic);
        }
    }
}
