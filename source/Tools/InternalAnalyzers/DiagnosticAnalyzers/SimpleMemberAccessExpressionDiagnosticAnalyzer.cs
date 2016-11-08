// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Internal.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SimpleMemberAccessExpressionDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.SimplifyGetTypeInfoInvocation); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSimpleMemberAccessExpression(f), SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            string identifierText = memberAccess.Name?.Identifier.ValueText;

            if (identifierText == "Type" || identifierText == "ConvertedType")
            {
                SemanticModel semanticModel = context.SemanticModel;
                CancellationToken cancellationToken = context.CancellationToken;

                var propertySymbol = semanticModel.GetSymbol(memberAccess, cancellationToken) as IPropertySymbol;

                if ((propertySymbol.Name == "Type" || propertySymbol.Name == "ConvertedType")
                    && propertySymbol?.Type == semanticModel.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.ITypeSymbol"))
                {
                    ExpressionSyntax expression = memberAccess.Expression;

                    if (expression?.IsKind(SyntaxKind.InvocationExpression) == true)
                    {
                        IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(expression, cancellationToken);

                        if (methodSymbol?.Name == "GetTypeInfo"
                            && methodSymbol.IsExtensionMethod
                            && methodSymbol.MethodKind == MethodKind.ReducedExtension
                            && methodSymbol.ReturnType == semanticModel.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.TypeInfo"))
                        {
                            ImmutableArray<IParameterSymbol> parameters = methodSymbol.ReducedFrom.Parameters;

                            if (parameters.Length == 3
                                && parameters[0].Type == semanticModel.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.SemanticModel"))
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.SimplifyGetTypeInfoInvocation,
                                    memberAccess.GetLocation());
                            }
                        }
                    }
                }
            }
        }
    }
}
