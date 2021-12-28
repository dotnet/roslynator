// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.IO;
using System.Threading;

namespace Roslynator.Migration
{
    internal static class AnalyzersMapping
    {
        private const string _mappingData = @"
RCS0017;AddNewLineAfterBinaryOperatorInsteadOfBeforeIt;RCS0027;AddNewLineBeforeBinaryOperatorInsteadOfAfterIt
RCS0017;AddNewLineAfterBinaryOperatorInsteadOfBeforeIt;RCS0027i;AddNewLineBeforeBinaryOperatorInsteadOfAfterIt
RCS0018;AddNewLineAfterConditionalOperatorInsteadOfBeforeIt;RCS0028;AddNewLineBeforeConditionalOperatorInsteadOfAfterIt
RCS0018;AddNewLineAfterConditionalOperatorInsteadOfBeforeIt;RCS0028i;AddNewLineBeforeConditionalOperatorInsteadOfAfterIt
RCS0019;AddNewLineAfterExpressionBodyArrowInsteadOfBeforeIt;RCS0032;AddNewLineBeforeExpressionBodyArrowInsteadOfAfterIt
RCS0019;AddNewLineAfterExpressionBodyArrowInsteadOfBeforeIt;RCS0032i;AddNewLineBeforeExpressionBodyArrowInsteadOfAfterIt
RCS0035;RemoveEmptyLineBetweenSingleLineAccessors;RCS0011;AddEmptyLineBetweenSingleLineAccessors
RCS0035;RemoveEmptyLineBetweenSingleLineAccessors;RCS0011i;AddEmptyLineBetweenSingleLineAccessors
RCS0037;RemoveEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace;RCS0015;AddEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace
RCS0037;RemoveEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace;RCS0015i;AddEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace
RCS0040;RemoveNewLineBetweenClosingBraceAndWhileKeyword;RCS0051;AddNewLineBetweenClosingBraceAndWhileKeyword
RCS0040;RemoveNewLineBetweenClosingBraceAndWhileKeyword;RCS0051i;AddNewLineBetweenClosingBraceAndWhileKeyword
RCS1023;FormatEmptyBlock;RCS0022;AddNewLineAfterOpeningBraceOfEmptyBlock
RCS1024;FormatAccessorList;RCS0025;AddNewLineBeforeAccessorOfFullProperty
RCS1024;FormatAccessorList;RCS0042;RemoveNewLinesFromAccessorListOfAutoProperty
RCS1024;FormatAccessorList;RCS0043;RemoveNewLinesFromAccessorWithSingleLineExpression
RCS1025;AddNewLineBeforeEnumMember;RCS0031;AddNewLineBeforeEnumMember
RCS1026;AddNewLineBeforeStatement;RCS0033;AddNewLineBeforeStatement
RCS1027;AddNewLineBeforeEmbeddedStatement;RCS0030;AddNewLineBeforeEmbeddedStatement
RCS1028;AddNewLineAfterSwitchLabel;RCS0024;AddNewLineAfterSwitchLabel
RCS1029;FormatBinaryOperatorOnNextLine;RCS0027;AddNewLineBeforeBinaryOperatorInsteadOfAfterIt
RCS1030;AddEmptyLineAfterEmbeddedStatement;RCS0001;AddEmptyLineAfterEmbeddedStatement
RCS1057;AddEmptyLineBetweenDeclarations;RCS0009;AddEmptyLineBetweenDeclarationAndDocumentationComment
RCS1057;AddEmptyLineBetweenDeclarations;RCS0010;AddEmptyLineBetweenDeclarations
RCS1067;RemoveArgumentListFromObjectCreation;RCS1050a;RemoveArgumentListFromObjectCreation
RCS1076;FormatDeclarationBraces;RCS0023;AddNewLineAfterOpeningBraceOfTypeDeclaration
RCS1086;UseLinefeedAsNewLine;RCS0045;UseLinefeedAsNewLine
RCS1087;UseCarriageReturnAndLinefeedAsNewLine;RCS0044;UseCarriageReturnAndLinefeedAsNewLine
RCS1088;UseSpacesInsteadOfTab;RCS0046;UseSpacesInsteadOfTab
RCS1092;AddEmptyLineBeforeWhileInDoStatement;RCS0004;AddEmptyLineBeforeClosingBraceOfDoStatement
RCS1153;AddEmptyLineAfterClosingBrace;RCS0008;AddEmptyLineBetweenBlockAndStatement
RCS1183;FormatInitializerWithSingleExpressionOnSingleLine;RCS0048;RemoveNewLinesFromInitializerWithSingleLineExpression
RCS1184;FormatConditionalExpression;RCS0028;AddNewLineBeforeConditionalOperatorInsteadOfAfterIt
RCS1185;FormatSingleLineBlock;RCS0021;AddNewLineAfterOpeningBraceOfBlock
RCS1245;SimplifyConditionalExpression2;RCS1104a;SimplifyConditionalExpressionWhenItIncludesNegationOfCondition
";

        private static ImmutableDictionary<string, ImmutableArray<string>> _mapping;

        public static ImmutableDictionary<string, ImmutableArray<string>> Mapping
        {
            get
            {
                if (_mapping == null)
                    Interlocked.CompareExchange(ref _mapping, LoadMapping(), null);

                return _mapping;
            }
        }

        private static ImmutableDictionary<string, ImmutableArray<string>> LoadMapping()
        {
            ImmutableDictionary<string, ImmutableArray<string>>.Builder dic = ImmutableDictionary.CreateBuilder<string, ImmutableArray<string>>();

            using (var sr = new StringReader(_mappingData))
            {
                string line = null;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        string[] split = line.Split(';');

                        string oldId = split[0];
                        string newId = split[2];

                        dic[oldId] = (dic.TryGetValue(oldId, out ImmutableArray<string> newIds))
                            ? newIds.Add(newId)
                            : ImmutableArray.Create(newId);
                    }
                }
            }

            return dic.ToImmutableDictionary();
        }
    }
}
