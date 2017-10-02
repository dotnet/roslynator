// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddStaticModifierToAllPartialClassDeclarationsRefactoring
    {
        public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol symbol)
        {
            if (symbol.IsClass()
                && symbol.IsStatic
                && !symbol.IsImplicitClass
                && !symbol.IsImplicitlyDeclared)
            {
                ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

                if (syntaxReferences.Length > 1)
                {
                    bool isStatic = false;
                    List<ClassDeclarationSyntax> classDeclarations = null;

                    foreach (SyntaxReference syntaxReference in syntaxReferences)
                    {
                        SyntaxNode node = syntaxReference.GetSyntax(context.CancellationToken);

                        Debug.Assert(node.IsKind(SyntaxKind.ClassDeclaration), node.Kind().ToString());

                        if (node.IsKind(SyntaxKind.ClassDeclaration))
                        {
                            var classDeclaration = (ClassDeclarationSyntax)node;
                            SyntaxTokenList modifiers = classDeclaration.Modifiers;

                            if (modifiers.Contains(SyntaxKind.StaticKeyword))
                            {
                                isStatic = true;
                            }
                            else if (!classDeclaration.ContainsDirectives(modifiers.Span))
                            {
                                (classDeclarations ?? (classDeclarations = new List<ClassDeclarationSyntax>())).Add(classDeclaration);
                            }
                        }
                    }

                    if (isStatic
                        && classDeclarations != null)
                    {
                        foreach (ClassDeclarationSyntax classDeclaration in classDeclarations)
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.AddStaticModifierToAllPartialClassDeclarations,
                                classDeclaration.Identifier);
                        }
                    }
                }
            }
        }
    }
}
