// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class RuntimeMetadataReference
    {
        internal static readonly MetadataReference CorLibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

        private static readonly ImmutableDictionary<string, string> _assemblyMap = GetTrustedPlatformAssemblies();
        private static ImmutableArray<MetadataReference> _defaultProjectReferences;

        internal static ImmutableArray<MetadataReference> DefaultProjectReferences
        {
            get
            {
                if (_defaultProjectReferences.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _defaultProjectReferences, Create());

                return _defaultProjectReferences;

                ImmutableArray<MetadataReference> Create()
                {
                    return ImmutableArray.Create(
                        CorLibReference,
                        CreateFromAssemblyName("System.Core.dll"),
                        CreateFromAssemblyName("System.Linq.dll"),
                        CreateFromAssemblyName("System.Linq.Expressions.dll"),
                        CreateFromAssemblyName("System.Runtime.Serialization.Formatters.dll"),
                        CreateFromAssemblyName("System.Runtime.dll"),
                        CreateFromAssemblyName("System.Collections.dll"),
                        CreateFromAssemblyName("System.Collections.Immutable.dll"),
                        CreateFromAssemblyName("System.Composition.AttributedModel.dll"),
                        CreateFromAssemblyName("System.ObjectModel.dll"),
                        CreateFromAssemblyName("System.Text.RegularExpressions.dll"),
                        CreateFromAssemblyName("System.Threading.Tasks.Extensions.dll"),
                        CreateFromAssemblyName("Microsoft.CodeAnalysis.dll"),
                        CreateFromAssemblyName("Microsoft.CodeAnalysis.CSharp.dll"),
                        CreateFromAssemblyName("Microsoft.CodeAnalysis.Workspaces.dll"));
                }
            }
        }

        private static ImmutableDictionary<string, string> GetTrustedPlatformAssemblies()
        {
            return AppContext
                .GetData("TRUSTED_PLATFORM_ASSEMBLIES")
                .ToString()
                .Split(';')
                .ToImmutableDictionary(Path.GetFileName);
        }

        public static PortableExecutableReference CreateFromAssemblyName(string assemblyName)
        {
            return MetadataReference.CreateFromFile(GetAssemblyLocation(assemblyName));
        }

        public static string GetAssemblyLocation(string assemblyName)
        {
            return _assemblyMap[assemblyName];
        }
    }
}
