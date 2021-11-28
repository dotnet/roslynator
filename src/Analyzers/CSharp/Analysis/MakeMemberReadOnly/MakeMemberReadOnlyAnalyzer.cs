// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis.MakeMemberReadOnly
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MakeMemberReadOnlyAnalyzer : BaseDiagnosticAnalyzer
    {
        private static readonly MetadataName Microsoft_AspNetCore_Components_ParameterAttribute = MetadataName.Parse("Microsoft.AspNetCore.Components.ParameterAttribute");
        private static readonly MetadataName Microsoft_AspNetCore_Components_CascadingParameterAttribute = MetadataName.Parse("Microsoft.AspNetCore.Components.CascadingParameterAttribute");

        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(
                        ref _supportedDiagnostics,
                        DiagnosticRules.MakeFieldReadOnly,
                        DiagnosticRules.UseReadOnlyAutoProperty);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(
                f => AnalyzeTypeDeclaration(f),
                SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.RecordStructDeclaration);
        }

        private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            if (typeDeclaration.Modifiers.Contains(SyntaxKind.PartialKeyword))
                return;

            MakeMemberReadOnlyWalker walker = null;

            try
            {
                walker = MakeMemberReadOnlyWalker.GetInstance();

                walker.SemanticModel = context.SemanticModel;
                walker.CancellationToken = context.CancellationToken;

                AnalyzeTypeDeclaration(context, typeDeclaration, walker);
            }
            finally
            {
                if (walker != null)
                    MakeMemberReadOnlyWalker.Free(walker);
            }
        }

        private static void AnalyzeTypeDeclaration(
            SyntaxNodeAnalysisContext context,
            TypeDeclarationSyntax typeDeclaration,
            MakeMemberReadOnlyWalker walker)
        {
            bool skipField = !DiagnosticRules.MakeFieldReadOnly.IsEffective(context);

            bool skipProperty = !DiagnosticRules.UseReadOnlyAutoProperty.IsEffective(context)
                || ((CSharpCompilation)context.Compilation).LanguageVersion < LanguageVersion.CSharp6;

            Dictionary<string, (SyntaxNode node, ISymbol symbol)> symbols = walker.Symbols;

            foreach (MemberDeclarationSyntax memberDeclaration in typeDeclaration.Members)
            {
                switch (memberDeclaration.Kind())
                {
                    case SyntaxKind.PropertyDeclaration:
                        {
                            if (skipProperty)
                                break;

                            var propertyDeclaration = (PropertyDeclarationSyntax)memberDeclaration;

                            AccessorDeclarationSyntax setter = propertyDeclaration.Setter();

                            if (setter?.IsKind(SyntaxKind.InitAccessorDeclaration) == false
                                && setter.BodyOrExpressionBody() == null
                                && !setter.AttributeLists.Any()
                                && !setter.SpanContainsDirectives())
                            {
                                IPropertySymbol propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

                                IMethodSymbol setMethod = propertySymbol.SetMethod;

                                if (setMethod?.DeclaredAccessibility == Accessibility.Private
                                    && setMethod.GetAttributes().IsEmpty
                                    && !propertySymbol.IsIndexer
                                    && !propertySymbol.IsReadOnly
                                    && ValidateType(propertySymbol.Type)
                                    && propertySymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                                    && AnalyzePropertyAttributes(propertySymbol))
                                {
                                    symbols[propertySymbol.Name] = (propertyDeclaration, propertySymbol);
                                }
                            }

                            break;
                        }
                    case SyntaxKind.FieldDeclaration:
                        {
                            if (skipField)
                                break;

                            var fieldDeclaration = (FieldDeclarationSyntax)memberDeclaration;

                            foreach (VariableDeclaratorSyntax declarator in fieldDeclaration.Declaration.Variables)
                            {
                                if (context.SemanticModel.GetDeclaredSymbol(declarator, context.CancellationToken) is IFieldSymbol fieldSymbol
                                    && !fieldSymbol.IsConst
                                    && fieldSymbol.DeclaredAccessibility == Accessibility.Private
                                    && !fieldSymbol.IsReadOnly
                                    && !fieldSymbol.IsVolatile
                                    && ValidateType(fieldSymbol.Type))
                                {
                                    symbols[fieldSymbol.Name] = (declarator, fieldSymbol);
                                }
                            }

                            break;
                        }
                }
            }

            if (symbols.Count > 0)
            {
                foreach (MemberDeclarationSyntax memberDeclaration in typeDeclaration.Members)
                {
                    walker.Visit(memberDeclaration);

                    if (symbols.Count == 0)
                        break;
                }

                if (symbols.Count > 0)
                {
                    foreach (KeyValuePair<string, (SyntaxNode node, ISymbol symbol)> kvp in symbols)
                    {
                        if (kvp.Value.node is PropertyDeclarationSyntax propertyDeclaration)
                        {
                            AccessorDeclarationSyntax setter = propertyDeclaration.Setter();

                            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseReadOnlyAutoProperty, setter);
                        }
                    }

                    foreach (IGrouping<VariableDeclarationSyntax, SyntaxNode> grouping in symbols
                        .Select(f => f.Value.node)
                        .Where(f => f.IsKind(SyntaxKind.VariableDeclarator))
                        .GroupBy(f => (VariableDeclarationSyntax)f.Parent))
                    {
                        int count = grouping.Key.Variables.Count;

                        if (count == 1
                            || count == grouping.Count())
                        {
                            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.MakeFieldReadOnly, grouping.Key.Parent);
                        }
                    }
                }
            }
        }

        private static bool AnalyzePropertyAttributes(IPropertySymbol propertySymbol)
        {
            foreach (AttributeData attributeData in propertySymbol.GetAttributes())
            {
                INamedTypeSymbol attributeClass = attributeData.AttributeClass;

                if (string.Equals(attributeClass.Name, "DependencyAttribute", StringComparison.Ordinal))
                    return false;

                if (attributeClass.HasMetadataName(MetadataNames.System_Runtime_Serialization_DataMemberAttribute))
                    return false;

                if (attributeClass.HasMetadataName(Microsoft_AspNetCore_Components_ParameterAttribute))
                    return false;

                if (attributeClass.HasMetadataName(Microsoft_AspNetCore_Components_CascadingParameterAttribute))
                    return false;
            }

            return true;
        }

        private static bool ValidateType(ITypeSymbol type)
        {
            if (type.Kind == SymbolKind.ErrorType)
                return false;

            return type.IsReferenceType
                || type.TypeKind == TypeKind.Enum
                || CSharpFacts.IsSimpleType(type.SpecialType)
                || type.IsReadOnlyStruct();
        }
    }
}
