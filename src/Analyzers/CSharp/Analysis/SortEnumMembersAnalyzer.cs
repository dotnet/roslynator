// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Comparers;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SortEnumMembersAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.SortEnumMembers); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeEnumDeclaration, SyntaxKind.EnumDeclaration);
        }

        public static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            if (IsListUnsorted(enumDeclaration.Members, context.SemanticModel, context.CancellationToken)
                && !enumDeclaration.ContainsDirectives(enumDeclaration.BracesSpan()))
            {
                SyntaxToken identifier = enumDeclaration.Identifier;
                context.ReportDiagnostic(DiagnosticDescriptors.SortEnumMembers, identifier, identifier);
            }
        }

        private static bool IsListUnsorted(
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            int count = members.Count;

            if (count > 1)
            {
                IFieldSymbol firstField = semanticModel.GetDeclaredSymbol(members.First(), cancellationToken);

                if (firstField?.HasConstantValue == true)
                {
                    SpecialType enumSpecialType = firstField.ContainingType.EnumUnderlyingType.SpecialType;

                    object previousValue = firstField.ConstantValue;

                    for (int i = 1; i < count - 1; i++)
                    {
                        IFieldSymbol field = semanticModel.GetDeclaredSymbol(members[i], cancellationToken);

                        if (field?.HasConstantValue != true)
                            return false;

                        object value = field.ConstantValue;

                        if (EnumValueComparer.GetInstance(enumSpecialType).Compare(previousValue, value) > 0)
                        {
                            i++;

                            while (i < count)
                            {
                                if (semanticModel.GetDeclaredSymbol(members[i], cancellationToken)?.HasConstantValue != true)
                                    return false;

                                i++;
                            }

                            return true;
                        }

                        previousValue = value;
                    }
                }
            }

            return false;
        }
    }
}
