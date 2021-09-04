// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Spelling
{
    internal readonly struct WordListLoaderResult
    {
        internal WordListLoaderResult(WordList list, WordList caseSensitiveList, FixList fixList)
        {
            List = list;
            CaseSensitiveList = caseSensitiveList;
            FixList = fixList;
        }

        public WordList List { get; }

        public WordList CaseSensitiveList { get; }

        public FixList FixList { get; }
    }
}
