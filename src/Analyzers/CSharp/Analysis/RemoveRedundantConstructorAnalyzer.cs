// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemoveRedundantConstructorAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.RemoveRedundantConstructor);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeConstructorDeclaration(f), SyntaxKind.ConstructorDeclaration);
    }

    private static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
    {
        var constructor = (ConstructorDeclarationSyntax)context.Node;

        if (!constructor.ContainsDiagnostics
            && (constructor.ParameterList?.Parameters.Any()) == false
            && (constructor.Body?.Statements.Any()) == false)
        {
            SyntaxTokenList modifiers = constructor.Modifiers;

            if (modifiers.Contains(SyntaxKind.PublicKeyword)
                && !modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                ConstructorInitializerSyntax initializer = constructor.Initializer;

                if (initializer is null
                    || initializer.ArgumentList?.Arguments.Any() == false)
                {
                    if (!constructor.AttributeLists.Any(attributeList => attributeList.Attributes.Any())
                        && !constructor.HasDocumentationComment()
                        && CheckStructWithFieldInitializer(constructor))
                    {
                        IMethodSymbol symbol = context.SemanticModel.GetDeclaredSymbol(constructor, context.CancellationToken);

                        if (symbol is not null
                            && SymbolEqualityComparer.Default.Equals(symbol, symbol.ContainingType.InstanceConstructors.SingleOrDefault(shouldThrow: false))
                            && constructor.DescendantTrivia(constructor.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                        {
                            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.RemoveRedundantConstructor, constructor);
                        }
                    }
                }
            }
        }
    }

    private static bool CheckStructWithFieldInitializer(ConstructorDeclarationSyntax constructor)
    {
        var memberDeclaration = constructor.Parent as BaseTypeDeclarationSyntax;

        if (memberDeclaration is not null)
        {
            if (memberDeclaration.Modifiers.Contains(SyntaxKind.PartialKeyword))
                return false;

            SyntaxList<MemberDeclarationSyntax> members = GetMembers(memberDeclaration);

            static SyntaxList<MemberDeclarationSyntax> GetMembers(MemberDeclarationSyntax memberDeclaration)
            {
                if (memberDeclaration is StructDeclarationSyntax structDeclaration)
                {
                    return structDeclaration.Members;
                }
                else if (memberDeclaration is RecordDeclarationSyntax recordDeclaration
                    && recordDeclaration.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword))
                {
                    return recordDeclaration.Members;
                }
                else
                {
                    return default;
                }
            }

            foreach (MemberDeclarationSyntax member in members)
            {
                switch (member)
                {
                    case PropertyDeclarationSyntax property:
                        {
                            return property.Initializer is null;
                        }
                    case FieldDeclarationSyntax field:
                        {
                            foreach (VariableDeclaratorSyntax declarator in field.Declaration.Variables)
                            {
                                if (declarator.Initializer is not null)
                                    return false;
                            }

                            break;
                        }
                }
            }
        }

        return true;
    }
}
