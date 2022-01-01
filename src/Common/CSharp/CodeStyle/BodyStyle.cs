// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.CodeStyle
{
    internal readonly struct BodyStyle
    {
        private readonly BodyStyleOption _option;

        private BodyStyle(BodyStyleOption option, bool useBlockWhenDeclarationIsMultiLine, bool useBlockWhenExpressionIsMultiLine)
        {
            _option = option;
            UseBlockWhenDeclarationIsMultiLine = useBlockWhenDeclarationIsMultiLine;
            UseBlockWhenExpressionIsMultiLine = useBlockWhenExpressionIsMultiLine;
        }

        public bool IsDefault => _option == BodyStyleOption.None && !UseBlockWhenDeclarationIsMultiLine && !UseBlockWhenExpressionIsMultiLine;

        public bool UseExpression => _option == BodyStyleOption.Expression;

        public bool UseBlock => _option == BodyStyleOption.Block;

        public bool UseBlockWhenDeclarationIsMultiLine { get; }

        public bool UseBlockWhenExpressionIsMultiLine { get; }

        public static BodyStyle Create(SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            var option = BodyStyleOption.None;

            if (configOptions.TryGetValue(ConfigOptionKeys.BodyStyle, out string rawValue))
            {
                if (string.Equals(rawValue, ConfigOptionValues.BodyStyle_Block, StringComparison.OrdinalIgnoreCase))
                {
                    option = BodyStyleOption.Block;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.BodyStyle_Expression, StringComparison.OrdinalIgnoreCase))
                {
                    option = BodyStyleOption.Expression;
                }
            }
            else if (configOptions.TryGetValueAsBool(LegacyConfigOptions.ConvertExpressionBodyToBlockBody, out bool useBlockBody))
            {
                option = (useBlockBody) ? BodyStyleOption.Block : BodyStyleOption.Expression;
            }

            bool useBlockBodyWhenDeclarationIsMultiLine = configOptions.IsEnabled(ConfigOptions.PreferBlockBodyWhenDeclarationSpansOverMultipleLines)
                || configOptions.IsEnabled(LegacyConfigOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine);

            bool UseBlockBodyWhenExpressionIsMultiline = configOptions.IsEnabled(ConfigOptions.PreferBlockBodyWhenExpressionSpansOverMultipleLines)
                || configOptions.IsEnabled(LegacyConfigOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine);

            return new BodyStyle(option, useBlockBodyWhenDeclarationIsMultiLine, UseBlockBodyWhenExpressionIsMultiline);
        }

        internal enum BodyStyleOption
        {
            None,
            Block,
            Expression,
        }
    }
}
