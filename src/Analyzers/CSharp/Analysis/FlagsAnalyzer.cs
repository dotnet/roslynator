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
    public class FlagsAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.DeclareEnumMemberWithZeroValue,
                    DiagnosticDescriptors.CompositeEnumValueContainsUndefinedFlag,
                    DiagnosticDescriptors.DeclareEnumValueAsCombinationOfNames);
            }
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
            var typeSymbol = (INamedTypeSymbol)context.Symbol;

            if (typeSymbol.IsImplicitlyDeclared)
                return;

            if (typeSymbol.TypeKind != TypeKind.Enum)
                return;

            if (!typeSymbol.HasAttribute(MetadataNames.System_FlagsAttribute))
                return;

            ImmutableArray<ISymbol> members = default;

            if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.DeclareEnumMemberWithZeroValue))
            {
                members = typeSymbol.GetMembers();

                if (!ContainsFieldWithZeroValue(members))
                {
                    var enumDeclaration = (EnumDeclarationSyntax)typeSymbol.GetSyntax(context.CancellationToken);

                    context.ReportDiagnostic(DiagnosticDescriptors.DeclareEnumMemberWithZeroValue, enumDeclaration.Identifier);
                }
            }

            EnumSymbolInfo enumInfo = default;

            if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.CompositeEnumValueContainsUndefinedFlag))
            {
                enumInfo = EnumSymbolInfo.Create(typeSymbol);

                ImmutableArray<EnumFieldSymbolInfo> fields = enumInfo.Fields;

                for (int i = 0; i < fields.Length; i++)
                {
                    if (fields[i].HasValue
                        && fields[i].HasCompositeValue())
                    {
                        foreach (ulong value in (fields[i].DecomposeValue()))
                        {
                            if (!enumInfo.Contains(value))
                                ReportUndefinedFlag(context, fields[i].Symbol, value.ToString());
                        }
                    }
                }
            }

            if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.DeclareEnumValueAsCombinationOfNames))
            {
                if (members.IsDefault)
                    members = typeSymbol.GetMembers();

                foreach (ISymbol member in members)
                {
                    if (!(member is IFieldSymbol fieldSymbol))
                        continue;

                    if (!fieldSymbol.HasConstantValue)
                        return;

                    EnumFieldSymbolInfo fieldInfo = EnumFieldSymbolInfo.Create(fieldSymbol);

                    if (!fieldInfo.HasCompositeValue())
                        continue;

                    var declaration = (EnumMemberDeclarationSyntax)fieldInfo.Symbol.GetSyntax(context.CancellationToken);

                    ExpressionSyntax expression = declaration.EqualsValue?.Value;

                    if (expression != null
                        && (expression.IsKind(SyntaxKind.NumericLiteralExpression)
                            || expression
                                .DescendantNodes()
                                .Any(f => f.IsKind(SyntaxKind.NumericLiteralExpression))))
                    {
                        if (enumInfo.Symbol == null)
                        {
                            enumInfo = EnumSymbolInfo.Create(typeSymbol);

                            if (enumInfo.Fields.Any(f => !f.HasValue))
                                return;
                        }

                        List<EnumFieldSymbolInfo> values = enumInfo.Decompose(fieldInfo);

                        if (values?.Count > 1)
                            context.ReportDiagnostic(DiagnosticDescriptors.DeclareEnumValueAsCombinationOfNames, expression);
                    }
                }
            }
        }

        private static bool ContainsFieldWithZeroValue(ImmutableArray<ISymbol> members)
        {
            foreach (ISymbol member in members)
            {
                if (member.Kind == SymbolKind.Field)
                {
                    var fieldSymbol = (IFieldSymbol)member;

                    if (fieldSymbol.HasConstantValue)
                    {
                        EnumFieldSymbolInfo fieldInfo = EnumFieldSymbolInfo.Create(fieldSymbol);

                        if (fieldInfo.Value == 0)
                            return true;
                    }
                }
            }

            return false;
        }

        private static void ReportUndefinedFlag(SymbolAnalysisContext context, ISymbol fieldSymbol, string value)
        {
            var enumMember = (EnumMemberDeclarationSyntax)fieldSymbol.GetSyntax(context.CancellationToken);

            context.ReportDiagnostic(
                DiagnosticDescriptors.CompositeEnumValueContainsUndefinedFlag,
                enumMember.GetLocation(),
                ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Value", value) }),
                value);
        }
    }
}