// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.Diagnostics.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveInapplicableModifierDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveInapplicableModifier); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeClassDeclaration(f), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeConstructorDeclaration(f), SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeConversionOperatorDeclaration(f), SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeDelegateDeclaration(f), SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeDestructorDeclaration(f), SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeEnumDeclaration(f), SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeEventDeclaration(f), SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeEventFieldDeclaration(f), SyntaxKind.EventFieldDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeFieldDeclaration(f), SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeIndexerDeclaration(f), SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeInterfaceDeclaration(f), SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeMethodDeclaration(f), SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeOperatorDeclaration(f), SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzePropertyDeclaration(f), SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeStructDeclaration(f), SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeEventAccessorDeclaration(f), SyntaxKind.AddAccessorDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeEventAccessorDeclaration(f), SyntaxKind.RemoveAccessorDeclaration);
        }

        private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (ClassDeclarationSyntax)context.Node;

            foreach (SyntaxToken modifier in declaration.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                    case SyntaxKind.ExternKeyword:
                    case SyntaxKind.VolatileKeyword:
                    case SyntaxKind.AsyncKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (ConstructorDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = declaration.Modifiers;

            if (modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                foreach (SyntaxToken modifier in modifiers)
                {
                    switch (modifier.Kind())
                    {
                        case SyntaxKind.NewKeyword:
                        case SyntaxKind.PublicKeyword:
                        case SyntaxKind.ProtectedKeyword:
                        case SyntaxKind.InternalKeyword:
                        case SyntaxKind.PrivateKeyword:
                        case SyntaxKind.ConstKeyword:
                        case SyntaxKind.VirtualKeyword:
                        case SyntaxKind.SealedKeyword:
                        case SyntaxKind.OverrideKeyword:
                        case SyntaxKind.AbstractKeyword:
                        case SyntaxKind.ReadOnlyKeyword:
                        case SyntaxKind.VolatileKeyword:
                        case SyntaxKind.AsyncKeyword:
                        case SyntaxKind.PartialKeyword:
                            {
                                ReportDiagnostic(context, modifier);
                                break;
                            }
                    }
                }
            }
            else
            {
                foreach (SyntaxToken modifier in modifiers)
                {
                    switch (modifier.Kind())
                    {
                        case SyntaxKind.NewKeyword:
                        case SyntaxKind.ConstKeyword:
                        case SyntaxKind.VirtualKeyword:
                        case SyntaxKind.SealedKeyword:
                        case SyntaxKind.OverrideKeyword:
                        case SyntaxKind.AbstractKeyword:
                        case SyntaxKind.ReadOnlyKeyword:
                        case SyntaxKind.VolatileKeyword:
                        case SyntaxKind.AsyncKeyword:
                        case SyntaxKind.PartialKeyword:
                            {
                                ReportDiagnostic(context, modifier);
                                break;
                            }
                    }
                }
            }
        }

        private void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (ConversionOperatorDeclarationSyntax)context.Node;

            foreach (SyntaxToken modifier in declaration.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.NewKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.SealedKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                    case SyntaxKind.VolatileKeyword:
                    case SyntaxKind.AsyncKeyword:
                    case SyntaxKind.PartialKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (DelegateDeclarationSyntax)context.Node;

            foreach (SyntaxToken modifier in declaration.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.StaticKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.SealedKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                    case SyntaxKind.ExternKeyword:
                    case SyntaxKind.VolatileKeyword:
                    case SyntaxKind.AsyncKeyword:
                    case SyntaxKind.PartialKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (DestructorDeclarationSyntax)context.Node;

            foreach (SyntaxToken modifier in declaration.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.NewKeyword:
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.StaticKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.SealedKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                    case SyntaxKind.VolatileKeyword:
                    case SyntaxKind.AsyncKeyword:
                    case SyntaxKind.PartialKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (EnumDeclarationSyntax)context.Node;

            foreach (SyntaxToken modifier in declaration.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.StaticKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.SealedKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                    case SyntaxKind.ExternKeyword:
                    case SyntaxKind.UnsafeKeyword:
                    case SyntaxKind.VolatileKeyword:
                    case SyntaxKind.AsyncKeyword:
                    case SyntaxKind.PartialKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (EventDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = declaration.Modifiers;

            if (declaration.ExplicitInterfaceSpecifier != null)
            {
                AnalyzeExplicitInterfaceImplementation(context, modifiers);
            }
            else
            {
                AnalyzeVirtualMemberInSealedClass(context, declaration, modifiers);
            }

            foreach (SyntaxToken modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                    case SyntaxKind.VolatileKeyword:
                    case SyntaxKind.AsyncKeyword:
                    case SyntaxKind.PartialKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (EventFieldDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = declaration.Modifiers;

            SyntaxNode parent = declaration.Parent;

            if (parent?.IsKind(SyntaxKind.InterfaceDeclaration) == true)
            {
                AnalyzeInterfaceMember(context, modifiers);
            }
            else
            {
                AnalyzeVirtualMemberInSealedClass(context, parent, modifiers);
            }

            foreach (SyntaxToken modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                    case SyntaxKind.VolatileKeyword:
                    case SyntaxKind.AsyncKeyword:
                    case SyntaxKind.PartialKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (FieldDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = declaration.Modifiers;

            if (modifiers.Contains(SyntaxKind.ConstKeyword))
            {
                foreach (SyntaxToken modifier in modifiers)
                {
                    switch (modifier.Kind())
                    {
                        case SyntaxKind.StaticKeyword:
                        case SyntaxKind.VirtualKeyword:
                        case SyntaxKind.SealedKeyword:
                        case SyntaxKind.OverrideKeyword:
                        case SyntaxKind.AbstractKeyword:
                        case SyntaxKind.ReadOnlyKeyword:
                        case SyntaxKind.ExternKeyword:
                        case SyntaxKind.VolatileKeyword:
                        case SyntaxKind.AsyncKeyword:
                        case SyntaxKind.PartialKeyword:
                            {
                                ReportDiagnostic(context, modifier);
                                break;
                            }
                    }
                }
            }
            else
            {
                foreach (SyntaxToken modifier in modifiers)
                {
                    switch (modifier.Kind())
                    {
                        case SyntaxKind.ConstKeyword:
                        case SyntaxKind.VirtualKeyword:
                        case SyntaxKind.SealedKeyword:
                        case SyntaxKind.OverrideKeyword:
                        case SyntaxKind.AbstractKeyword:
                        case SyntaxKind.ExternKeyword:
                        case SyntaxKind.AsyncKeyword:
                        case SyntaxKind.PartialKeyword:
                            {
                                ReportDiagnostic(context, modifier);
                                break;
                            }
                    }
                }
            }
        }

        private void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (IndexerDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = declaration.Modifiers;

            SyntaxNode parent = declaration.Parent;

            if (parent?.IsKind(SyntaxKind.InterfaceDeclaration) == true)
            {
                AnalyzeAccessorListOfInterfaceMember(context, declaration.AccessorList);
                AnalyzeInterfaceMember(context, modifiers);
            }
            else if (declaration.ExplicitInterfaceSpecifier != null)
            {
                AnalyzeExplicitInterfaceImplementation(context, modifiers);
            }
            else
            {
                AnalyzeVirtualMemberInSealedClass(context, parent, modifiers);
            }

            foreach (SyntaxToken modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.ConstKeyword:
                    //case SyntaxKind.StaticKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                    case SyntaxKind.VolatileKeyword:
                    case SyntaxKind.AsyncKeyword:
                    case SyntaxKind.PartialKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (InterfaceDeclarationSyntax)context.Node;

            foreach (SyntaxToken modifier in declaration.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.StaticKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.SealedKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                    case SyntaxKind.ExternKeyword:
                    case SyntaxKind.VolatileKeyword:
                    case SyntaxKind.AsyncKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (MethodDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = declaration.Modifiers;

            SyntaxNode parent = declaration.Parent;

            if (parent?.IsKind(SyntaxKind.InterfaceDeclaration) == true)
            {
                AnalyzeInterfaceMember(context, modifiers);
            }
            else if (declaration.ExplicitInterfaceSpecifier != null)
            {
                AnalyzeExplicitInterfaceImplementation(context, modifiers);
            }
            else if (parent?.IsKind(SyntaxKind.ClassDeclaration) == true)
            {
                AnalyzeVirtualMemberInSealedClass(context, (ClassDeclarationSyntax)parent, modifiers);

                if (modifiers.Contains(SyntaxKind.PartialKeyword))
                    AnalyzePartialMethod(context, modifiers);
            }

            foreach (SyntaxToken modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                    case SyntaxKind.VolatileKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private static void AnalyzePartialMethod(SyntaxNodeAnalysisContext context, SyntaxTokenList modifiers)
        {
            foreach (SyntaxToken modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.NewKeyword:
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.SealedKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.ExternKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (OperatorDeclarationSyntax)context.Node;

            foreach (SyntaxToken modifier in declaration.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.NewKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.SealedKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                    case SyntaxKind.VolatileKeyword:
                    case SyntaxKind.AsyncKeyword:
                    case SyntaxKind.PartialKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (PropertyDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = declaration.Modifiers;

            SyntaxNode parent = declaration.Parent;

            if (parent?.IsKind(SyntaxKind.InterfaceDeclaration) == true)
            {
                AnalyzeAccessorListOfInterfaceMember(context, declaration.AccessorList);
                AnalyzeInterfaceMember(context, modifiers);
            }
            else if (declaration.ExplicitInterfaceSpecifier != null)
            {
                AnalyzeExplicitInterfaceImplementation(context, modifiers);
            }
            else
            {
                AnalyzeVirtualMemberInSealedClass(context, parent, modifiers);
            }

            foreach (SyntaxToken modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                    case SyntaxKind.VolatileKeyword:
                    case SyntaxKind.AsyncKeyword:
                    case SyntaxKind.PartialKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (StructDeclarationSyntax)context.Node;

            foreach (SyntaxToken modifier in declaration.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.StaticKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.SealedKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                    case SyntaxKind.ExternKeyword:
                    case SyntaxKind.VolatileKeyword:
                    case SyntaxKind.AsyncKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private static void AnalyzeInterfaceMember(SyntaxNodeAnalysisContext context, SyntaxTokenList modifiers)
        {
            foreach (SyntaxToken modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.StaticKeyword:
                    case SyntaxKind.AbstractKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private static void AnalyzeEventAccessorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var accessor = (AccessorDeclarationSyntax)context.Node;

            AnalyzeAccessor(context, accessor);
        }

        private static void AnalyzeAccessorListOfInterfaceMember(SyntaxNodeAnalysisContext context, AccessorListSyntax accessorList)
        {
            if (accessorList != null)
            {
                foreach (AccessorDeclarationSyntax accessor in accessorList.Accessors)
                    AnalyzeAccessor(context, accessor);
            }
        }

        private static void AnalyzeAccessor(SyntaxNodeAnalysisContext context, AccessorDeclarationSyntax accessor)
        {
            foreach (SyntaxToken modifier in accessor.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                        {
                            ReportDiagnostic(context, modifier);
                            break;
                        }
                }
            }
        }

        private static void AnalyzeExplicitInterfaceImplementation(SyntaxNodeAnalysisContext context, SyntaxTokenList modifiers)
        {
            foreach (SyntaxToken modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.AbstractKeyword:
                        {
                            context.ReportDiagnostic(DiagnosticDescriptors.RemoveInapplicableModifier, modifier);
                            break;
                        }
                }
            }
        }

        private static void AnalyzeVirtualMemberInSealedClass(SyntaxNodeAnalysisContext context, SyntaxNode parent, SyntaxTokenList modifiers)
        {
            if (parent?.IsKind(SyntaxKind.ClassDeclaration) == true)
                AnalyzeVirtualMemberInSealedClass(context, (ClassDeclarationSyntax)parent, modifiers);
        }

        private static void AnalyzeVirtualMemberInSealedClass(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration, SyntaxTokenList modifiers)
        {
            if (classDeclaration.Modifiers.Contains(SyntaxKind.SealedKeyword))
            {
                int index = modifiers.IndexOf(SyntaxKind.VirtualKeyword);

                if (index != -1)
                    ReportDiagnostic(context, modifiers[index]);
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxToken modifier)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.RemoveInapplicableModifier, modifier);
        }
    }
}
