// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseAttributeUsageAttributeDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseAttributeUsageAttribute); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                INamedTypeSymbol attributeSymbol = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Attribute);
                INamedTypeSymbol attributeUsageAttributeSymbol = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_AttributeUsageAttribute);

                if (attributeSymbol != null
                    && attributeUsageAttributeSymbol != null)
                {
                    startContext.RegisterSymbolAction(
                       nodeContext => UseAttributeUsageAttributeRefactoring.AnalyzerNamedTypeSymbol(nodeContext, attributeSymbol, attributeUsageAttributeSymbol),
                       SymbolKind.NamedType);
                }
            });
        }
    }
}
