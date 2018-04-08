// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EnumMemberShouldDeclareExplicitValueAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.EnumMemberShouldDeclareExplicitValue); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeEnumMemberDeclaration, SyntaxKind.EnumMemberDeclaration);
        }

        public static void AnalyzeEnumMemberDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumMember = (EnumMemberDeclarationSyntax)context.Node;

            if (HasImplicitValue(enumMember, context.SemanticModel, context.CancellationToken))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.EnumMemberShouldDeclareExplicitValue,
                    enumMember.Identifier);
            }
        }

        internal static bool HasImplicitValue(EnumMemberDeclarationSyntax enumMember, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            EqualsValueClauseSyntax equalsValue = enumMember.EqualsValue;

            if (equalsValue == null)
            {
                return semanticModel
                    .GetDeclaredSymbol(enumMember, cancellationToken)?
                    .HasConstantValue == true;
            }

            return false;
        }
    }
}
