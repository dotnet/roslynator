// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.Host;

namespace Roslynator.Host.Mef
{
    internal class MefWorkspaceServices
    {
        private static MefWorkspaceServices _default;
        private readonly MefHostServices _mefServices;
        private IEnumerable<string> _languages;

        private ImmutableDictionary<string, MefLanguageServices> _languageServicesMap
            = ImmutableDictionary<string, MefLanguageServices>.Empty;

        public MefWorkspaceServices(MefHostServices mefServices)
        {
            _mefServices = mefServices;
        }

        public static MefWorkspaceServices Default
        {
            get
            {
                if (_default == null)
                {
                    var services = new MefWorkspaceServices(MefHostServices.Default);
                    Interlocked.CompareExchange(ref _default, services, null);
                }

                return _default;
            }
        }

        public IEnumerable<string> SupportedLanguages => GetSupportedLanguages();

        private IEnumerable<string> GetSupportedLanguages()
        {
            if (_languages == null)
            {
                ImmutableArray<string> languages = _mefServices.GetExports<ILanguageService, LanguageServiceMetadata>()
                    .Select(lazy => lazy.Metadata.Language)
                    .Distinct()
                    .ToImmutableArray();

                Interlocked.CompareExchange(ref _languages, languages, null);
            }

            return _languages;
        }

        public bool IsSupported(string languageName) => GetSupportedLanguages().Contains(languageName);

        public MefLanguageServices GetLanguageServices(string languageName)
        {
            ImmutableDictionary<string, MefLanguageServices> languageServicesMap = _languageServicesMap;

            if (!languageServicesMap.TryGetValue(languageName, out MefLanguageServices languageServices))
            {
                languageServices = ImmutableInterlocked.GetOrAdd(
                    ref _languageServicesMap,
                    languageName,
                    _ => new MefLanguageServices(this, languageName));
            }

            if (!languageServices.HasServices)
                throw new NotSupportedException($"The language '{languageName}' is not supported.");

            return languageServices;
        }

        internal bool TryGetLanguageServices(string languageName, out MefLanguageServices languageServices)
        {
            return _languageServicesMap.TryGetValue(languageName, out languageServices);
        }

        internal TLanguageService GetService<TLanguageService>(string languageName)
        {
            return GetLanguageServices(languageName).GetService<TLanguageService>();
        }

        internal IEnumerable<Lazy<TExtension>> GetExports<TExtension>()
        {
            return _mefServices.GetExports<TExtension>();
        }

        internal IEnumerable<Lazy<TExtension, TMetadata>> GetExports<TExtension, TMetadata>()
        {
            return _mefServices.GetExports<TExtension, TMetadata>();
        }
    }
}
