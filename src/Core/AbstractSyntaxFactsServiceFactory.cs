// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    internal abstract class AbstractSyntaxFactsServiceFactory
    {
        public abstract SyntaxFactsService GetService(string language);

        public abstract SyntaxFactsService GetServiceOrDefault(string language);

        public abstract bool IsSupportedLanguage(string language);
    }
}
