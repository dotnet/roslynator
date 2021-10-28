// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class RuntimeMetadataReference
    {
        private static ImmutableArray<string> _trustedPlatformAssemblies;
        private static ImmutableArray<MetadataReference> _metadataReferences;

        internal static ImmutableArray<string> TrustedPlatformAssemblies
        {
            get
            {
                if (_trustedPlatformAssemblies.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _trustedPlatformAssemblies, CreateTrustedPlatformAssemblies());

                return _trustedPlatformAssemblies;

                static ImmutableArray<string> CreateTrustedPlatformAssemblies()
                {
                    // works only for .NET Core, it returns null for .NET Framework
                    return AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")?
                        .ToString()
                        .Split(';')
                        .ToImmutableArray()
                        ?? ImmutableArray<string>.Empty;
                }
            }
        }

        internal static ImmutableArray<MetadataReference> DefaultMetadataReferences
        {
            get
            {
                if (_metadataReferences.IsDefault)
                {
                    ImmutableArray<MetadataReference> references = CreateMetadataReferences();
#if DEBUG
                    foreach (string assemblyName in new[]
                    {
                        "Roslynator.Core.dll" ,
                        "Roslynator.CSharp.dll" ,
                        "Roslynator.Workspaces.Core.dll" ,
                        "Roslynator.CSharp.Workspaces.dll"
                    })
                    {
                        Debug.Assert(references.OfType<PortableExecutableReference>().Any(f => f.FilePath.EndsWith(assemblyName, StringComparison.OrdinalIgnoreCase)), assemblyName);
                    }
#endif
                    ImmutableInterlocked.InterlockedInitialize(ref _metadataReferences, references);
                }

                return _metadataReferences;

                static ImmutableArray<MetadataReference> CreateMetadataReferences()
                {
                    if (TrustedPlatformAssemblies.IsEmpty)
                    {
                        // An attempt to obtain .NET Framework assemblies

                        var types = new Type[]
                        {
                            typeof(object),
                            typeof(System.Text.RegularExpressions.Regex),
                            typeof(System.Xml.XmlAttribute),
                            typeof(System.Xml.Linq.XObject),
                            typeof(Enumerable),
                            typeof(Compilation), // Microsoft.CodeAnalysis.dll
                            typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation), // Microsoft.CodeAnalysis.CSharp.dll
                            typeof(Document), // Microsoft.CodeAnalysis.Workspaces.dll
#if DEBUG
                            typeof(Roslynator.AccessibilityFilter), // Roslynator.Core.dll
                            typeof(Roslynator.CSharp.CSharpFactory), // Roslynator.CSharp.dll
                            typeof(Roslynator.CSharp.WorkspaceExtensions), // Roslynator.CSharp.Workspaces.dll
                            typeof(Roslynator.WorkspaceExtensions), // Roslynator.Workspaces.Core.dll
#endif
                        };

                        Assembly assembly = Assembly.Load("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51");

                        IEnumerable<Assembly> assemblies = types.Select(f => f.Assembly).Append(assembly);

                        Debug.Assert(assemblies.Select(f => f.Location).Distinct().Count() == assemblies.Count());

                        return assemblies
                            .Select(f => MetadataReference.CreateFromFile(f.Location))
                            .ToImmutableArray<MetadataReference>();
                    }
                    else
                    {
                        return TrustedPlatformAssemblies
                            .Where(f =>
                            {
                                string name = Path.GetFileName(f);

                                return name.StartsWith("Microsoft.", StringComparison.Ordinal)
                                    || name.StartsWith("System.", StringComparison.Ordinal)
#if DEBUG
                                    || string.Equals(name, "Roslynator.Core.dll", StringComparison.Ordinal)
                                    || string.Equals(name, "Roslynator.CSharp.dll", StringComparison.Ordinal)
                                    || string.Equals(name, "Roslynator.Workspaces.Core.dll", StringComparison.Ordinal)
                                    || string.Equals(name, "Roslynator.CSharp.Workspaces.dll", StringComparison.Ordinal)
#endif
                                    || string.Equals(name, "netstandard.dll", StringComparison.Ordinal)
                                    || string.Equals(name, "mscorlib.dll", StringComparison.Ordinal);
                            })
                            .Select(f => (MetadataReference)MetadataReference.CreateFromFile(f))
                            .ToImmutableArray();
                    }
                }
            }
        }
    }
}
