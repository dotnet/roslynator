// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyNullableOfTRefactoring
    {
        public static void AnalyzeGenericName(SyntaxNodeAnalysisContext context)
        {
            var genericName = (GenericNameSyntax)context.Node;

            if (!genericName.IsParentKind(SyntaxKind.QualifiedName, SyntaxKind.UsingDirective))
            {
                TypeArgumentListSyntax typeArgumentList = genericName.TypeArgumentList;

                if (typeArgumentList != null)
                {
                    SeparatedSyntaxList<TypeSyntax> arguments = typeArgumentList.Arguments;

                    if (arguments.Count == 1
                        && !arguments[0].IsKind(SyntaxKind.OmittedTypeArgument))
                    {
                        var namedTypeSymbol = context.SemanticModel.GetSymbol(genericName, context.CancellationToken) as INamedTypeSymbol;

                        if (namedTypeSymbol?.IsConstructedFrom(SpecialType.System_Nullable_T) == true)
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.SimplifyNullableOfT,
                                genericName);
                        }
                    }
                }
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, QualifiedNameSyntax qualifiedName)
        {
            if (!qualifiedName.IsParentKind(SyntaxKind.UsingDirective))
            {
                var typeSymbol = context.SemanticModel.GetSymbol(qualifiedName, context.CancellationToken) as INamedTypeSymbol;

                if (typeSymbol?.SupportsPredefinedType() == false
                    && typeSymbol.IsConstructedFrom(SpecialType.System_Nullable_T))
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.SimplifyNullableOfT, qualifiedName);
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            TypeSyntax type,
            TypeSyntax nullableType,
            CancellationToken cancellationToken)
        {
            TypeSyntax newType = NullableType(nullableType.WithoutTrivia(), QuestionToken())
                .WithTriviaFrom(type)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(type, newType, cancellationToken);
        }
    }
}
