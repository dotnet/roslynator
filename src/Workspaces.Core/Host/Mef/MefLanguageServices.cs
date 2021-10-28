// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.Host;

namespace Roslynator.Host.Mef
{
    internal class MefLanguageServices
    {
        private readonly ImmutableArray<Lazy<ILanguageService, LanguageServiceMetadata>> _services;

        private ImmutableDictionary<Type, Lazy<ILanguageService, LanguageServiceMetadata>> _serviceMap
            = ImmutableDictionary<Type, Lazy<ILanguageService, LanguageServiceMetadata>>.Empty;

        public MefLanguageServices(
            MefWorkspaceServices workspaceServices,
            string language)
        {
            WorkspaceServices = workspaceServices;
            Language = language;

            _services = workspaceServices.GetExports<ILanguageService, LanguageServiceMetadata>()
                .Where(lazy => lazy.Metadata.Language == language)
                .ToImmutableArray();
        }

        public MefWorkspaceServices WorkspaceServices { get; }

        public string Language { get; }

        public bool HasServices => _services.Length > 0;

        public TLanguageService GetService<TLanguageService>()
        {
            if (TryGetService(typeof(TLanguageService), out Lazy<ILanguageService, LanguageServiceMetadata> service))
            {
                return (TLanguageService)service.Value;
            }
            else
            {
                return default;
            }
        }

        internal bool TryGetService(Type serviceType, out Lazy<ILanguageService, LanguageServiceMetadata> service)
        {
            if (!_serviceMap.TryGetValue(serviceType, out service))
            {
                service = ImmutableInterlocked.GetOrAdd(
                    ref _serviceMap,
                    serviceType,
                    type =>
                    {
                        string fullName = type.FullName;

                        return _services.SingleOrDefault(lz => lz.Metadata.ServiceType == fullName, shouldThrow: false);
                    });
            }

            return service != default;
        }
    }
}
