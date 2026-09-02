// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NETCOREAPP

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Roslynator.CommandLine;

/// <summary>
/// Resolves assemblies that analyzers and code fix providers reference but that the .NET SDK does
/// not deploy next to them, most notably the <c>System.Composition.*</c> (MEF) assemblies used by
/// the <c>[ExportCodeFixProvider]</c> / <c>[Shared]</c> attributes.
/// <para>
/// The compiler only ever loads diagnostic analyzers, whose attributes live in
/// <c>Microsoft.CodeAnalysis</c>, so the SDK never ships <c>System.Composition</c> alongside its
/// analyzer/code-fix assemblies. Roslynator, however, also inspects code fix providers, which forces
/// <c>System.Composition.AttributedModel</c> to load while their attributes are read. The version an
/// SDK analyzer was compiled against tracks the SDK patch level (for example <c>10.0.0.10</c>) and is
/// therefore higher than the copy restored transitively for the tool, so the default load fails with
/// <c>FileNotFoundException</c>. Version matching is not an option because the user's SDK moves
/// independently of Roslynator's releases; the public surface of these attributes is stable across
/// versions, so the copy shipped with the tool is redirected in regardless of the requested version.
/// See https://github.com/dotnet/roslynator/issues/1649.
/// </para>
/// </summary>
internal static class AnalyzerDependencyResolver
{
    private static readonly string[] _redirectedAssemblyNames =
    {
        "System.Composition.AttributedModel",
        "System.Composition.Convention",
        "System.Composition.Hosting",
        "System.Composition.Runtime",
        "System.Composition.TypedParts",
    };

    private static bool _isRegistered;

    public static void Register()
    {
        if (_isRegistered)
            return;

        _isRegistered = true;
        AssemblyLoadContext.Default.Resolving += Resolve;
    }

    private static Assembly Resolve(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        if (!IsRedirected(assemblyName.Name))
            return null;

        // Reuse the copy already loaded with the tool, ignoring the requested version.
        foreach (Assembly assembly in AssemblyLoadContext.Default.Assemblies)
        {
            if (string.Equals(assembly.GetName().Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase))
                return assembly;
        }

        string path = Path.Combine(AppContext.BaseDirectory, assemblyName.Name + ".dll");

        return File.Exists(path)
            ? AssemblyLoadContext.Default.LoadFromAssemblyPath(path)
            : null;
    }

    private static bool IsRedirected(string name)
    {
        foreach (string redirectedName in _redirectedAssemblyNames)
        {
            if (string.Equals(name, redirectedName, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
#endif
