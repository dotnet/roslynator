// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class RuntimeMetadataReference
    {
        internal static readonly MetadataReference CorLibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

        private static ImmutableArray<string> _defaultAssemblyNames;
        private static ImmutableDictionary<string, string> _trustedPlatformAssemblyMap;

        internal static ImmutableArray<string> DefaultAssemblyNames
        {
            get
            {
                if (_defaultAssemblyNames.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _defaultAssemblyNames, Create());

                return _defaultAssemblyNames;

                static ImmutableArray<string> Create()
                {
                    return ImmutableArray.Create(
                        "netstandard.dll",
                        "System.Core.dll",
                        "System.Linq.dll",
                        "System.Linq.Expressions.dll",
                        "System.Runtime.Serialization.Formatters.dll",
                        "System.Runtime.dll",
                        "System.Collections.dll",
                        "System.Collections.Immutable.dll",
                        "System.Composition.AttributedModel.dll",
                        "System.ObjectModel.dll",
                        "System.Text.RegularExpressions.dll",
                        "System.Threading.Tasks.Extensions.dll",
                        "Microsoft.CodeAnalysis.dll",
                        "Microsoft.CodeAnalysis.CSharp.dll",
                        "Microsoft.CodeAnalysis.Workspaces.dll");
                }
            }
        }

        internal static ImmutableDictionary<string, string> TrustedPlatformAssemblyMap
        {
            get
            {
                if (_trustedPlatformAssemblyMap == null)
                    Interlocked.CompareExchange(ref _trustedPlatformAssemblyMap, CreateTrustedPlatformAssemblies(), null);

                return _trustedPlatformAssemblyMap;

                static ImmutableDictionary<string, string> CreateTrustedPlatformAssemblies()
                {
                    return AppContext
                        .GetData("TRUSTED_PLATFORM_ASSEMBLIES")
                        .ToString()
                        .Split(';')
                        .ToImmutableDictionary(f => Path.GetFileName(f));
                }
            }
        }
    }
}
