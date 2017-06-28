// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal static class ModifierHelper
    {
        public static TNode InsertModifier<TNode>(TNode node, SyntaxKind modifierKind, IModifierComparer comparer = null) where TNode : SyntaxNode
        {
            return InsertModifier(node, Token(modifierKind), comparer);
        }

        public static TNode InsertModifier<TNode>(TNode node, SyntaxToken modifier, IModifierComparer comparer = null) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)ClassDeclarationModifierHelper.Instance.InsertModifier((ClassDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)ConstructorDeclarationModifierHelper.Instance.InsertModifier((ConstructorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)ConversionOperatorDeclarationModifierHelper.Instance.InsertModifier((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)DelegateDeclarationModifierHelper.Instance.InsertModifier((DelegateDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)DestructorDeclarationModifierHelper.Instance.InsertModifier((DestructorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)EnumDeclarationModifierHelper.Instance.InsertModifier((EnumDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)EventDeclarationModifierHelper.Instance.InsertModifier((EventDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)EventFieldDeclarationModifierHelper.Instance.InsertModifier((EventFieldDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)FieldDeclarationModifierHelper.Instance.InsertModifier((FieldDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)IndexerDeclarationModifierHelper.Instance.InsertModifier((IndexerDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)InterfaceDeclarationModifierHelper.Instance.InsertModifier((InterfaceDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)MethodDeclarationModifierHelper.Instance.InsertModifier((MethodDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)OperatorDeclarationModifierHelper.Instance.InsertModifier((OperatorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)PropertyDeclarationModifierHelper.Instance.InsertModifier((PropertyDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)StructDeclarationModifierHelper.Instance.InsertModifier((StructDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)AccessorDeclarationModifierHelper.Instance.InsertModifier((AccessorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)LocalDeclarationStatementModifierHelper.Instance.InsertModifier((LocalDeclarationStatementSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)LocalFunctionStatementModifierHelper.Instance.InsertModifier((LocalFunctionStatementSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)ParameterModifierHelper.Instance.InsertModifier((ParameterSyntax)(SyntaxNode)node, modifier, comparer);
            }

            Debug.Assert(node.IsKind(SyntaxKind.IncompleteMember), node.ToString());

            return node;
        }

        public static TNode RemoveModifier<TNode>(TNode node, SyntaxKind modifierKind) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)ClassDeclarationModifierHelper.Instance.RemoveModifier((ClassDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)ConstructorDeclarationModifierHelper.Instance.RemoveModifier((ConstructorDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)ConversionOperatorDeclarationModifierHelper.Instance.RemoveModifier((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)DelegateDeclarationModifierHelper.Instance.RemoveModifier((DelegateDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)DestructorDeclarationModifierHelper.Instance.RemoveModifier((DestructorDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)EnumDeclarationModifierHelper.Instance.RemoveModifier((EnumDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)EventDeclarationModifierHelper.Instance.RemoveModifier((EventDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)EventFieldDeclarationModifierHelper.Instance.RemoveModifier((EventFieldDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)FieldDeclarationModifierHelper.Instance.RemoveModifier((FieldDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)IndexerDeclarationModifierHelper.Instance.RemoveModifier((IndexerDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)InterfaceDeclarationModifierHelper.Instance.RemoveModifier((InterfaceDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)MethodDeclarationModifierHelper.Instance.RemoveModifier((MethodDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)OperatorDeclarationModifierHelper.Instance.RemoveModifier((OperatorDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)PropertyDeclarationModifierHelper.Instance.RemoveModifier((PropertyDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)StructDeclarationModifierHelper.Instance.RemoveModifier((StructDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)AccessorDeclarationModifierHelper.Instance.RemoveModifier((AccessorDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)LocalDeclarationStatementModifierHelper.Instance.RemoveModifier((LocalDeclarationStatementSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)LocalFunctionStatementModifierHelper.Instance.RemoveModifier((LocalFunctionStatementSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)ParameterModifierHelper.Instance.RemoveModifier((ParameterSyntax)(SyntaxNode)node, modifierKind);
            }

            Debug.Assert(node.IsKind(SyntaxKind.IncompleteMember), node.ToString());

            return node;
        }

        public static TNode RemoveModifier<TNode>(TNode node, SyntaxToken modifier) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)ClassDeclarationModifierHelper.Instance.RemoveModifier((ClassDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)ConstructorDeclarationModifierHelper.Instance.RemoveModifier((ConstructorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)ConversionOperatorDeclarationModifierHelper.Instance.RemoveModifier((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)DelegateDeclarationModifierHelper.Instance.RemoveModifier((DelegateDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)DestructorDeclarationModifierHelper.Instance.RemoveModifier((DestructorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)EnumDeclarationModifierHelper.Instance.RemoveModifier((EnumDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)EventDeclarationModifierHelper.Instance.RemoveModifier((EventDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)EventFieldDeclarationModifierHelper.Instance.RemoveModifier((EventFieldDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)FieldDeclarationModifierHelper.Instance.RemoveModifier((FieldDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)IndexerDeclarationModifierHelper.Instance.RemoveModifier((IndexerDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)InterfaceDeclarationModifierHelper.Instance.RemoveModifier((InterfaceDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)MethodDeclarationModifierHelper.Instance.RemoveModifier((MethodDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)OperatorDeclarationModifierHelper.Instance.RemoveModifier((OperatorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)PropertyDeclarationModifierHelper.Instance.RemoveModifier((PropertyDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)StructDeclarationModifierHelper.Instance.RemoveModifier((StructDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)AccessorDeclarationModifierHelper.Instance.RemoveModifier((AccessorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)LocalDeclarationStatementModifierHelper.Instance.RemoveModifier((LocalDeclarationStatementSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)LocalFunctionStatementModifierHelper.Instance.RemoveModifier((LocalFunctionStatementSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)ParameterModifierHelper.Instance.RemoveModifier((ParameterSyntax)(SyntaxNode)node, modifier);
            }

            Debug.Assert(node.IsKind(SyntaxKind.IncompleteMember), node.ToString());

            return node;
        }

        public static TNode RemoveModifierAt<TNode>(TNode node, int index) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)ClassDeclarationModifierHelper.Instance.RemoveModifierAt((ClassDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)ConstructorDeclarationModifierHelper.Instance.RemoveModifierAt((ConstructorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)ConversionOperatorDeclarationModifierHelper.Instance.RemoveModifierAt((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)DelegateDeclarationModifierHelper.Instance.RemoveModifierAt((DelegateDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)DestructorDeclarationModifierHelper.Instance.RemoveModifierAt((DestructorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)EnumDeclarationModifierHelper.Instance.RemoveModifierAt((EnumDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)EventDeclarationModifierHelper.Instance.RemoveModifierAt((EventDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)EventFieldDeclarationModifierHelper.Instance.RemoveModifierAt((EventFieldDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)FieldDeclarationModifierHelper.Instance.RemoveModifierAt((FieldDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)IndexerDeclarationModifierHelper.Instance.RemoveModifierAt((IndexerDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)InterfaceDeclarationModifierHelper.Instance.RemoveModifierAt((InterfaceDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)MethodDeclarationModifierHelper.Instance.RemoveModifierAt((MethodDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)OperatorDeclarationModifierHelper.Instance.RemoveModifierAt((OperatorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)PropertyDeclarationModifierHelper.Instance.RemoveModifierAt((PropertyDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)StructDeclarationModifierHelper.Instance.RemoveModifierAt((StructDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)AccessorDeclarationModifierHelper.Instance.RemoveModifierAt((AccessorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)LocalDeclarationStatementModifierHelper.Instance.RemoveModifierAt((LocalDeclarationStatementSyntax)(SyntaxNode)node, index);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)LocalFunctionStatementModifierHelper.Instance.RemoveModifierAt((LocalFunctionStatementSyntax)(SyntaxNode)node, index);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)ParameterModifierHelper.Instance.RemoveModifierAt((ParameterSyntax)(SyntaxNode)node, index);
            }

            Debug.Assert(node.IsKind(SyntaxKind.IncompleteMember), node.ToString());

            return node;
        }

        public static TNode RemoveAccessModifiers<TNode>(TNode node) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)ClassDeclarationModifierHelper.Instance.RemoveAccessModifiers((ClassDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)ConstructorDeclarationModifierHelper.Instance.RemoveAccessModifiers((ConstructorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)ConversionOperatorDeclarationModifierHelper.Instance.RemoveAccessModifiers((ConversionOperatorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)DelegateDeclarationModifierHelper.Instance.RemoveAccessModifiers((DelegateDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)DestructorDeclarationModifierHelper.Instance.RemoveAccessModifiers((DestructorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)EnumDeclarationModifierHelper.Instance.RemoveAccessModifiers((EnumDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)EventDeclarationModifierHelper.Instance.RemoveAccessModifiers((EventDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)EventFieldDeclarationModifierHelper.Instance.RemoveAccessModifiers((EventFieldDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)FieldDeclarationModifierHelper.Instance.RemoveAccessModifiers((FieldDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)IndexerDeclarationModifierHelper.Instance.RemoveAccessModifiers((IndexerDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)InterfaceDeclarationModifierHelper.Instance.RemoveAccessModifiers((InterfaceDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)MethodDeclarationModifierHelper.Instance.RemoveAccessModifiers((MethodDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)OperatorDeclarationModifierHelper.Instance.RemoveAccessModifiers((OperatorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)PropertyDeclarationModifierHelper.Instance.RemoveAccessModifiers((PropertyDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)StructDeclarationModifierHelper.Instance.RemoveAccessModifiers((StructDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)AccessorDeclarationModifierHelper.Instance.RemoveAccessModifiers((AccessorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)LocalDeclarationStatementModifierHelper.Instance.RemoveAccessModifiers((LocalDeclarationStatementSyntax)(SyntaxNode)node);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)LocalFunctionStatementModifierHelper.Instance.RemoveAccessModifiers((LocalFunctionStatementSyntax)(SyntaxNode)node);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)ParameterModifierHelper.Instance.RemoveAccessModifiers((ParameterSyntax)(SyntaxNode)node);
            }

            Debug.Assert(node.IsKind(SyntaxKind.IncompleteMember), node.ToString());

            return node;
        }
    }
}
