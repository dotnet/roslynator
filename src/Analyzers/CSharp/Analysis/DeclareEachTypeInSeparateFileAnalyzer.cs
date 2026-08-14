// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DeclareEachTypeInSeparateFileAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.DeclareEachTypeInSeparateFile);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeCompilationUnit(f), SyntaxKind.CompilationUnit);
    }

    private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
    {
        var compilationUnit = (CompilationUnitSyntax)context.Node;

        SyntaxList<MemberDeclarationSyntax> compilationUnitMembers = compilationUnit.Members;

        if (!compilationUnitMembers.Any())
            return;

        if (ContainsSingleNamespaceWithSingleNonNamespaceMember(compilationUnitMembers))
            return;

        MemberDeclarationSyntax firstTypeDeclaration = null;
        var isFirstReported = false;

        Analyze(compilationUnitMembers);

        void Analyze(SyntaxList<MemberDeclarationSyntax> members)
        {
            (string Name, int Arity) firstTypeKey = default;
            var hasFirstTypeKey = false;
            HashSet<(string Name, int Arity)> declaredTypes = null;

            foreach (MemberDeclarationSyntax member in members)
            {
#if ROSLYN_4_0
                if (member is BaseNamespaceDeclarationSyntax namespaceDeclaration)
#else
                if (member is NamespaceDeclarationSyntax namespaceDeclaration)
#endif
                {
                    Analyze(namespaceDeclaration.Members);
                }
                else if (SyntaxFacts.IsTypeDeclaration(member.Kind())
#if ROSLYN_4_4
                    && !member.Modifiers.Contains(SyntaxKind.FileKeyword)
#endif
                    )
                {
                    SyntaxToken identifier = CSharpUtility.GetIdentifier(member);

                    if (identifier == default)
                        continue;

                    int arity = CSharpUtility.GetTypeParameterList(member)?.Parameters.Count ?? 0;
                    (string Name, int Arity) typeKey = (identifier.ValueText, arity);

                    if (declaredTypes is not null)
                    {
                        if (!declaredTypes.Add(typeKey)
                            && member.Modifiers.Contains(SyntaxKind.PartialKeyword))
                        {
                            continue;
                        }
                    }
                    else if (!hasFirstTypeKey)
                    {
                        firstTypeKey = typeKey;
                        hasFirstTypeKey = true;
                    }
                    else if (typeKey == firstTypeKey
                        && member.Modifiers.Contains(SyntaxKind.PartialKeyword))
                    {
                        continue;
                    }
                    else
                    {
                        declaredTypes = new HashSet<(string Name, int Arity)>() { firstTypeKey };
                        declaredTypes.Add(typeKey);
                    }

                    if (firstTypeDeclaration is null)
                    {
                        firstTypeDeclaration = member;
                    }
                    else
                    {
                        if (!isFirstReported)
                        {
                            ReportDiagnostic(context, firstTypeDeclaration);
                            isFirstReported = true;
                        }

                        ReportDiagnostic(context, member);
                    }
                }
            }
        }
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax member)
    {
        SyntaxToken token = CSharpUtility.GetIdentifier(member);

        if (token == default)
            return;

        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.DeclareEachTypeInSeparateFile, token);
    }

    private static bool ContainsSingleNamespaceWithSingleNonNamespaceMember(SyntaxList<MemberDeclarationSyntax> members)
    {
#if ROSLYN_4_0
        if (members.SingleOrDefault(shouldThrow: false) is BaseNamespaceDeclarationSyntax namespaceDeclaration)
#else
        if (members.SingleOrDefault(shouldThrow: false) is NamespaceDeclarationSyntax namespaceDeclaration)
#endif
        {
            MemberDeclarationSyntax member = namespaceDeclaration.Members.SingleOrDefault(shouldThrow: false);

            return member is not null
                && member.Kind() != SyntaxKind.NamespaceDeclaration;
        }

        return false;
    }
}
