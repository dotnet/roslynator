// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.DiagnosticAnalyzers;
using static Roslynator.CSharp.Analyzers.UnusedParameter.UnusedParameterRefactoring;

namespace Roslynator.CSharp.Analyzers.UnusedParameter
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnusedParameterDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UnusedParameter,
                    DiagnosticDescriptors.UnusedThisParameter,
                    DiagnosticDescriptors.UnusedTypeParameter);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                Compilation compilation = startContext.Compilation;

                INamedTypeSymbol serializationInfoSymbol = compilation.GetTypeByMetadataName(MetadataNames.System_Runtime_Serialization_SerializationInfo);
                INamedTypeSymbol streamingContextSymbol = null;

                if (serializationInfoSymbol != null)
                    streamingContextSymbol = compilation.GetTypeByMetadataName(MetadataNames.System_Runtime_Serialization_StreamingContext);

                startContext.RegisterSyntaxNodeAction(f => AnalyzeConstructorDeclaration(f, serializationInfoSymbol, streamingContextSymbol), SyntaxKind.ConstructorDeclaration);
                startContext.RegisterSyntaxNodeAction(AnalyzeOperatorDeclaration, SyntaxKind.OperatorDeclaration);
                startContext.RegisterSyntaxNodeAction(AnalyzeConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
                startContext.RegisterSyntaxNodeAction(AnalyzeIndexerDeclaration, SyntaxKind.IndexerDeclaration);

                INamedTypeSymbol eventArgsSymbol = compilation.GetTypeByMetadataName(MetadataNames.System_EventArgs);

                if (eventArgsSymbol != null)
                {
                    startContext.RegisterSyntaxNodeAction(f => AnalyzeMethodDeclaration(f, eventArgsSymbol), SyntaxKind.MethodDeclaration);
                    startContext.RegisterSyntaxNodeAction(f => AnalyzeLocalFunctionStatement(f, eventArgsSymbol), SyntaxKind.LocalFunctionStatement);
                    startContext.RegisterSyntaxNodeAction(f => AnalyzeSimpleLambdaExpression(f, eventArgsSymbol), SyntaxKind.SimpleLambdaExpression);
                    startContext.RegisterSyntaxNodeAction(f => AnalyzeParenthesizedLambdaExpression(f, eventArgsSymbol), SyntaxKind.ParenthesizedLambdaExpression);
                    startContext.RegisterSyntaxNodeAction(f => AnalyzeAnonymousMethodExpression(f, eventArgsSymbol), SyntaxKind.AnonymousMethodExpression);
                }
            });
        }
    }
}
