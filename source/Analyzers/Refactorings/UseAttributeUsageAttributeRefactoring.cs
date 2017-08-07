// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseAttributeUsageAttributeRefactoring
    {
        public static void AnalyzerNamedTypeSymbol(
            SymbolAnalysisContext context,
            INamedTypeSymbol attributeSymbol,
            INamedTypeSymbol attributeUsageAttributeSymbol)
        {
            var symbol = (INamedTypeSymbol)context.Symbol;

            if (symbol.InheritsFrom(attributeSymbol)
                && !symbol.HasAttribute(attributeUsageAttributeSymbol))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseAttributeUsageAttribute,
                    ((ClassDeclarationSyntax)symbol.GetSyntax()).Identifier);
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ClassDeclarationSyntax classDeclaration,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            INamedTypeSymbol symbol = semanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken);

            var attributeName = (NameSyntax)semanticModel
                .GetTypeByMetadataName(MetadataNames.System_AttributeUsageAttribute)
                .ToMinimalTypeSyntax(semanticModel, classDeclaration.SpanStart);

            TypeSyntax attributeTargetsType = semanticModel
                .GetTypeByMetadataName(MetadataNames.System_AttributeTargets)
                .ToMinimalTypeSyntax(semanticModel, classDeclaration.SpanStart);

            AttributeSyntax attribute = Attribute(
                attributeName,
                AttributeArgumentList(
                    AttributeArgument(SimpleMemberAccessExpression(attributeTargetsType, IdentifierName(Identifier("All").WithRenameAnnotation()))),
                    AttributeArgument(NameEquals(IdentifierName("AllowMultiple")), FalseLiteralExpression())));

            ClassDeclarationSyntax newNode = classDeclaration.AddAttributeLists(AttributeList(attribute));

            return await document.ReplaceNodeAsync(classDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
