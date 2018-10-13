// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DeclareEnumValueAsCombinationOfNamesAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.DeclareEnumValueAsCombinationOfNames); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        public static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var enumSymbol = (INamedTypeSymbol)context.Symbol;

            if (enumSymbol.TypeKind != TypeKind.Enum)
                return;

            if (!enumSymbol.HasAttribute(MetadataNames.System_FlagsAttribute))
                return;

            var infos = default(ImmutableArray<EnumFieldSymbolInfo>);

            foreach (ISymbol member in enumSymbol.GetMembers())
            {
                if (member is IFieldSymbol fieldSymbol)
                {
                    if (!fieldSymbol.HasConstantValue)
                        return;

                    var info = new EnumFieldSymbolInfo(fieldSymbol);

                    if (info.IsComposite())
                    {
                        var declaration = (EnumMemberDeclarationSyntax)info.Symbol.GetSyntax(context.CancellationToken);

                        ExpressionSyntax valueExpression = declaration.EqualsValue?.Value;

                        if (valueExpression != null
                            && (valueExpression.IsKind(SyntaxKind.NumericLiteralExpression)
                                || valueExpression
                                    .DescendantNodes()
                                    .Any(f => f.IsKind(SyntaxKind.NumericLiteralExpression))))
                        {
                            if (infos.IsDefault)
                            {
                                infos = EnumFieldSymbolInfo.CreateRange(enumSymbol);

                                if (infos.IsDefault)
                                    return;
                            }

                            List<EnumFieldSymbolInfo> values = info.Decompose(infos);

                            if (values?.Count > 1)
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.DeclareEnumValueAsCombinationOfNames,
                                    valueExpression);
                            }
                        }
                    }
                }
            }
        }
    }
}