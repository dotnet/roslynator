// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Helpers.ModifierHelpers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    internal static class Modifier
    {
        public static TNode Insert<TNode>(TNode node, SyntaxKind modifierKind, IModifierComparer comparer = null) where TNode : SyntaxNode
        {
            return Insert(node, Token(modifierKind), comparer);
        }

        public static TNode Insert<TNode>(TNode node, SyntaxToken modifier, IModifierComparer comparer = null) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)Insert((ClassDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)Insert((ConstructorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)Insert((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)Insert((DelegateDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)Insert((DestructorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)Insert((EnumDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)Insert((EventDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)Insert((EventFieldDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)Insert((FieldDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)Insert((IndexerDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)Insert((InterfaceDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)Insert((MethodDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)Insert((OperatorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)Insert((PropertyDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)Insert((StructDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)Insert((AccessorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)Insert((LocalDeclarationStatementSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)Insert((LocalFunctionStatementSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)Insert((ParameterSyntax)(SyntaxNode)node, modifier, comparer);
            }

            Debug.Assert(node.IsKind(SyntaxKind.IncompleteMember), node.ToString());

            return node;
        }

        public static TNode Remove<TNode>(TNode node, SyntaxKind modifierKind) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)Remove((ClassDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)Remove((ConstructorDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)Remove((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)Remove((DelegateDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)Remove((DestructorDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)Remove((EnumDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)Remove((EventDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)Remove((EventFieldDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)Remove((FieldDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)Remove((IndexerDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)Remove((InterfaceDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)Remove((MethodDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)Remove((OperatorDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)Remove((PropertyDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)Remove((StructDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)Remove((AccessorDeclarationSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)Remove((LocalDeclarationStatementSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)Remove((LocalFunctionStatementSyntax)(SyntaxNode)node, modifierKind);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)Remove((ParameterSyntax)(SyntaxNode)node, modifierKind);
            }

            Debug.Assert(node.IsKind(SyntaxKind.IncompleteMember), node.ToString());

            return node;
        }

        public static TNode Remove<TNode>(TNode node, SyntaxToken modifier) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)Remove((ClassDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)Remove((ConstructorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)Remove((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)Remove((DelegateDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)Remove((DestructorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)Remove((EnumDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)Remove((EventDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)Remove((EventFieldDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)Remove((FieldDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)Remove((IndexerDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)Remove((InterfaceDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)Remove((MethodDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)Remove((OperatorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)Remove((PropertyDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)Remove((StructDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)Remove((AccessorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)Remove((LocalDeclarationStatementSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)Remove((LocalFunctionStatementSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)Remove((ParameterSyntax)(SyntaxNode)node, modifier);
            }

            Debug.Assert(node.IsKind(SyntaxKind.IncompleteMember), node.ToString());

            return node;
        }

        public static TNode RemoveAt<TNode>(TNode node, int index) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((ClassDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((ConstructorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((DelegateDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((DestructorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((EnumDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((EventDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((EventFieldDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((FieldDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((IndexerDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((InterfaceDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((MethodDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((OperatorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((PropertyDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((StructDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAt((AccessorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)RemoveAt((LocalDeclarationStatementSyntax)(SyntaxNode)node, index);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)RemoveAt((LocalFunctionStatementSyntax)(SyntaxNode)node, index);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)RemoveAt((ParameterSyntax)(SyntaxNode)node, index);
            }

            Debug.Assert(node.IsKind(SyntaxKind.IncompleteMember), node.ToString());

            return node;
        }

        public static TNode RemoveAccess<TNode>(TNode node) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((ClassDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((ConstructorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((ConversionOperatorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((DelegateDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((DestructorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((EnumDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((EventDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((EventFieldDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((FieldDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((IndexerDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((InterfaceDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((MethodDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((OperatorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((PropertyDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((StructDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAccess((AccessorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)RemoveAccess((LocalDeclarationStatementSyntax)(SyntaxNode)node);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)RemoveAccess((LocalFunctionStatementSyntax)(SyntaxNode)node);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)RemoveAccess((ParameterSyntax)(SyntaxNode)node);
            }

            Debug.Assert(node.IsKind(SyntaxKind.IncompleteMember), node.ToString());

            return node;
        }

        public static TNode RemoveAll<TNode>(TNode node) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((ClassDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((ConstructorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((ConversionOperatorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((DelegateDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((DestructorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((EnumDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((EventDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((EventFieldDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((FieldDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((IndexerDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((InterfaceDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((MethodDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((OperatorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((PropertyDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((StructDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)RemoveAll((AccessorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)RemoveAll((LocalDeclarationStatementSyntax)(SyntaxNode)node);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)RemoveAll((LocalFunctionStatementSyntax)(SyntaxNode)node);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)RemoveAll((ParameterSyntax)(SyntaxNode)node);
            }

            Debug.Assert(node.IsKind(SyntaxKind.IncompleteMember), node.ToString());

            return node;
        }

        public static ClassDeclarationSyntax Insert(ClassDeclarationSyntax classDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return ClassDeclarationModifierHelper.Instance.InsertModifier(classDeclaration, modifier, comparer);
        }

        public static ClassDeclarationSyntax Insert(ClassDeclarationSyntax classDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return ClassDeclarationModifierHelper.Instance.InsertModifier(classDeclaration, modifierKind, comparer);
        }

        public static ConstructorDeclarationSyntax Insert(ConstructorDeclarationSyntax constructorDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return ConstructorDeclarationModifierHelper.Instance.InsertModifier(constructorDeclaration, modifier, comparer);
        }

        public static ConstructorDeclarationSyntax Insert(ConstructorDeclarationSyntax constructorDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return ConstructorDeclarationModifierHelper.Instance.InsertModifier(constructorDeclaration, modifierKind, comparer);
        }

        public static ConversionOperatorDeclarationSyntax Insert(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return ConversionOperatorDeclarationModifierHelper.Instance.InsertModifier(conversionOperatorDeclaration, modifier, comparer);
        }

        public static ConversionOperatorDeclarationSyntax Insert(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return ConversionOperatorDeclarationModifierHelper.Instance.InsertModifier(conversionOperatorDeclaration, modifierKind, comparer);
        }

        public static DelegateDeclarationSyntax Insert(DelegateDeclarationSyntax delegateDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return DelegateDeclarationModifierHelper.Instance.InsertModifier(delegateDeclaration, modifier, comparer);
        }

        public static DelegateDeclarationSyntax Insert(DelegateDeclarationSyntax delegateDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return DelegateDeclarationModifierHelper.Instance.InsertModifier(delegateDeclaration, modifierKind, comparer);
        }

        public static DestructorDeclarationSyntax Insert(DestructorDeclarationSyntax destructorDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return DestructorDeclarationModifierHelper.Instance.InsertModifier(destructorDeclaration, modifier, comparer);
        }

        public static DestructorDeclarationSyntax Insert(DestructorDeclarationSyntax destructorDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return DestructorDeclarationModifierHelper.Instance.InsertModifier(destructorDeclaration, modifierKind, comparer);
        }

        public static EnumDeclarationSyntax Insert(EnumDeclarationSyntax enumDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return EnumDeclarationModifierHelper.Instance.InsertModifier(enumDeclaration, modifier, comparer);
        }

        public static EnumDeclarationSyntax Insert(EnumDeclarationSyntax enumDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return EnumDeclarationModifierHelper.Instance.InsertModifier(enumDeclaration, modifierKind, comparer);
        }

        public static EventDeclarationSyntax Insert(EventDeclarationSyntax eventDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return EventDeclarationModifierHelper.Instance.InsertModifier(eventDeclaration, modifier, comparer);
        }

        public static EventDeclarationSyntax Insert(EventDeclarationSyntax eventDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return EventDeclarationModifierHelper.Instance.InsertModifier(eventDeclaration, modifierKind, comparer);
        }

        public static EventFieldDeclarationSyntax Insert(EventFieldDeclarationSyntax eventFieldDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return EventFieldDeclarationModifierHelper.Instance.InsertModifier(eventFieldDeclaration, modifier, comparer);
        }

        public static EventFieldDeclarationSyntax Insert(EventFieldDeclarationSyntax eventFieldDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return EventFieldDeclarationModifierHelper.Instance.InsertModifier(eventFieldDeclaration, modifierKind, comparer);
        }

        public static FieldDeclarationSyntax Insert(FieldDeclarationSyntax fieldDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return FieldDeclarationModifierHelper.Instance.InsertModifier(fieldDeclaration, modifier, comparer);
        }

        public static FieldDeclarationSyntax Insert(FieldDeclarationSyntax fieldDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return FieldDeclarationModifierHelper.Instance.InsertModifier(fieldDeclaration, modifierKind, comparer);
        }

        public static IndexerDeclarationSyntax Insert(IndexerDeclarationSyntax indexerDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return IndexerDeclarationModifierHelper.Instance.InsertModifier(indexerDeclaration, modifier, comparer);
        }

        public static IndexerDeclarationSyntax Insert(IndexerDeclarationSyntax indexerDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return IndexerDeclarationModifierHelper.Instance.InsertModifier(indexerDeclaration, modifierKind, comparer);
        }

        public static InterfaceDeclarationSyntax Insert(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return InterfaceDeclarationModifierHelper.Instance.InsertModifier(interfaceDeclaration, modifier, comparer);
        }

        public static InterfaceDeclarationSyntax Insert(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return InterfaceDeclarationModifierHelper.Instance.InsertModifier(interfaceDeclaration, modifierKind, comparer);
        }

        public static MethodDeclarationSyntax Insert(MethodDeclarationSyntax methodDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return MethodDeclarationModifierHelper.Instance.InsertModifier(methodDeclaration, modifier, comparer);
        }

        public static MethodDeclarationSyntax Insert(MethodDeclarationSyntax methodDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return MethodDeclarationModifierHelper.Instance.InsertModifier(methodDeclaration, modifierKind, comparer);
        }

        public static OperatorDeclarationSyntax Insert(OperatorDeclarationSyntax operatorDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return OperatorDeclarationModifierHelper.Instance.InsertModifier(operatorDeclaration, modifier, comparer);
        }

        public static OperatorDeclarationSyntax Insert(OperatorDeclarationSyntax operatorDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return OperatorDeclarationModifierHelper.Instance.InsertModifier(operatorDeclaration, modifierKind, comparer);
        }

        public static PropertyDeclarationSyntax Insert(PropertyDeclarationSyntax propertyDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return PropertyDeclarationModifierHelper.Instance.InsertModifier(propertyDeclaration, modifier, comparer);
        }

        public static PropertyDeclarationSyntax Insert(PropertyDeclarationSyntax propertyDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return PropertyDeclarationModifierHelper.Instance.InsertModifier(propertyDeclaration, modifierKind, comparer);
        }

        public static StructDeclarationSyntax Insert(StructDeclarationSyntax structDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return StructDeclarationModifierHelper.Instance.InsertModifier(structDeclaration, modifier, comparer);
        }

        public static StructDeclarationSyntax Insert(StructDeclarationSyntax structDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return StructDeclarationModifierHelper.Instance.InsertModifier(structDeclaration, modifierKind, comparer);
        }

        public static AccessorDeclarationSyntax Insert(AccessorDeclarationSyntax accessorDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return AccessorDeclarationModifierHelper.Instance.InsertModifier(accessorDeclaration, modifier, comparer);
        }

        public static AccessorDeclarationSyntax Insert(AccessorDeclarationSyntax accessorDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return AccessorDeclarationModifierHelper.Instance.InsertModifier(accessorDeclaration, modifierKind, comparer);
        }

        public static LocalDeclarationStatementSyntax Insert(LocalDeclarationStatementSyntax localDeclaration, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return LocalDeclarationStatementModifierHelper.Instance.InsertModifier(localDeclaration, modifier, comparer);
        }

        public static LocalDeclarationStatementSyntax Insert(LocalDeclarationStatementSyntax localDeclaration, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return LocalDeclarationStatementModifierHelper.Instance.InsertModifier(localDeclaration, modifierKind, comparer);
        }

        public static LocalFunctionStatementSyntax Insert(LocalFunctionStatementSyntax localFunction, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return LocalFunctionStatementModifierHelper.Instance.InsertModifier(localFunction, modifier, comparer);
        }

        public static LocalFunctionStatementSyntax Insert(LocalFunctionStatementSyntax localFunction, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return LocalFunctionStatementModifierHelper.Instance.InsertModifier(localFunction, modifierKind, comparer);
        }

        public static ParameterSyntax Insert(ParameterSyntax parameter, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            return ParameterModifierHelper.Instance.InsertModifier(parameter, modifier, comparer);
        }

        public static ParameterSyntax Insert(ParameterSyntax parameter, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return ParameterModifierHelper.Instance.InsertModifier(parameter, modifierKind, comparer);
        }

        public static ClassDeclarationSyntax Remove(ClassDeclarationSyntax classDeclaration, SyntaxToken modifier)
        {
            return ClassDeclarationModifierHelper.Instance.RemoveModifier(classDeclaration, modifier);
        }

        public static ClassDeclarationSyntax Remove(ClassDeclarationSyntax classDeclaration, SyntaxKind modifierKind)
        {
            return ClassDeclarationModifierHelper.Instance.RemoveModifier(classDeclaration, modifierKind);
        }

        public static ConstructorDeclarationSyntax Remove(ConstructorDeclarationSyntax constructorDeclaration, SyntaxToken modifier)
        {
            return ConstructorDeclarationModifierHelper.Instance.RemoveModifier(constructorDeclaration, modifier);
        }

        public static ConstructorDeclarationSyntax Remove(ConstructorDeclarationSyntax constructorDeclaration, SyntaxKind modifierKind)
        {
            return ConstructorDeclarationModifierHelper.Instance.RemoveModifier(constructorDeclaration, modifierKind);
        }

        public static ConversionOperatorDeclarationSyntax Remove(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, SyntaxToken modifier)
        {
            return ConversionOperatorDeclarationModifierHelper.Instance.RemoveModifier(conversionOperatorDeclaration, modifier);
        }

        public static ConversionOperatorDeclarationSyntax Remove(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, SyntaxKind modifierKind)
        {
            return ConversionOperatorDeclarationModifierHelper.Instance.RemoveModifier(conversionOperatorDeclaration, modifierKind);
        }

        public static DelegateDeclarationSyntax Remove(DelegateDeclarationSyntax delegateDeclaration, SyntaxToken modifier)
        {
            return DelegateDeclarationModifierHelper.Instance.RemoveModifier(delegateDeclaration, modifier);
        }

        public static DelegateDeclarationSyntax Remove(DelegateDeclarationSyntax delegateDeclaration, SyntaxKind modifierKind)
        {
            return DelegateDeclarationModifierHelper.Instance.RemoveModifier(delegateDeclaration, modifierKind);
        }

        public static DestructorDeclarationSyntax Remove(DestructorDeclarationSyntax destructorDeclaration, SyntaxToken modifier)
        {
            return DestructorDeclarationModifierHelper.Instance.RemoveModifier(destructorDeclaration, modifier);
        }

        public static DestructorDeclarationSyntax Remove(DestructorDeclarationSyntax destructorDeclaration, SyntaxKind modifierKind)
        {
            return DestructorDeclarationModifierHelper.Instance.RemoveModifier(destructorDeclaration, modifierKind);
        }

        public static EnumDeclarationSyntax Remove(EnumDeclarationSyntax enumDeclaration, SyntaxToken modifier)
        {
            return EnumDeclarationModifierHelper.Instance.RemoveModifier(enumDeclaration, modifier);
        }

        public static EnumDeclarationSyntax Remove(EnumDeclarationSyntax enumDeclaration, SyntaxKind modifierKind)
        {
            return EnumDeclarationModifierHelper.Instance.RemoveModifier(enumDeclaration, modifierKind);
        }

        public static EventDeclarationSyntax Remove(EventDeclarationSyntax eventDeclaration, SyntaxToken modifier)
        {
            return EventDeclarationModifierHelper.Instance.RemoveModifier(eventDeclaration, modifier);
        }

        public static EventDeclarationSyntax Remove(EventDeclarationSyntax eventDeclaration, SyntaxKind modifierKind)
        {
            return EventDeclarationModifierHelper.Instance.RemoveModifier(eventDeclaration, modifierKind);
        }

        public static EventFieldDeclarationSyntax Remove(EventFieldDeclarationSyntax eventFieldDeclaration, SyntaxToken modifier)
        {
            return EventFieldDeclarationModifierHelper.Instance.RemoveModifier(eventFieldDeclaration, modifier);
        }

        public static EventFieldDeclarationSyntax Remove(EventFieldDeclarationSyntax eventFieldDeclaration, SyntaxKind modifierKind)
        {
            return EventFieldDeclarationModifierHelper.Instance.RemoveModifier(eventFieldDeclaration, modifierKind);
        }

        public static FieldDeclarationSyntax Remove(FieldDeclarationSyntax fieldDeclaration, SyntaxToken modifier)
        {
            return FieldDeclarationModifierHelper.Instance.RemoveModifier(fieldDeclaration, modifier);
        }

        public static FieldDeclarationSyntax Remove(FieldDeclarationSyntax fieldDeclaration, SyntaxKind modifierKind)
        {
            return FieldDeclarationModifierHelper.Instance.RemoveModifier(fieldDeclaration, modifierKind);
        }

        public static IndexerDeclarationSyntax Remove(IndexerDeclarationSyntax indexerDeclaration, SyntaxToken modifier)
        {
            return IndexerDeclarationModifierHelper.Instance.RemoveModifier(indexerDeclaration, modifier);
        }

        public static IndexerDeclarationSyntax Remove(IndexerDeclarationSyntax indexerDeclaration, SyntaxKind modifierKind)
        {
            return IndexerDeclarationModifierHelper.Instance.RemoveModifier(indexerDeclaration, modifierKind);
        }

        public static InterfaceDeclarationSyntax Remove(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxToken modifier)
        {
            return InterfaceDeclarationModifierHelper.Instance.RemoveModifier(interfaceDeclaration, modifier);
        }

        public static InterfaceDeclarationSyntax Remove(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxKind modifierKind)
        {
            return InterfaceDeclarationModifierHelper.Instance.RemoveModifier(interfaceDeclaration, modifierKind);
        }

        public static MethodDeclarationSyntax Remove(MethodDeclarationSyntax methodDeclaration, SyntaxToken modifier)
        {
            return MethodDeclarationModifierHelper.Instance.RemoveModifier(methodDeclaration, modifier);
        }

        public static MethodDeclarationSyntax Remove(MethodDeclarationSyntax methodDeclaration, SyntaxKind modifierKind)
        {
            return MethodDeclarationModifierHelper.Instance.RemoveModifier(methodDeclaration, modifierKind);
        }

        public static OperatorDeclarationSyntax Remove(OperatorDeclarationSyntax operatorDeclaration, SyntaxToken modifier)
        {
            return OperatorDeclarationModifierHelper.Instance.RemoveModifier(operatorDeclaration, modifier);
        }

        public static OperatorDeclarationSyntax Remove(OperatorDeclarationSyntax operatorDeclaration, SyntaxKind modifierKind)
        {
            return OperatorDeclarationModifierHelper.Instance.RemoveModifier(operatorDeclaration, modifierKind);
        }

        public static PropertyDeclarationSyntax Remove(PropertyDeclarationSyntax propertyDeclaration, SyntaxToken modifier)
        {
            return PropertyDeclarationModifierHelper.Instance.RemoveModifier(propertyDeclaration, modifier);
        }

        public static PropertyDeclarationSyntax Remove(PropertyDeclarationSyntax propertyDeclaration, SyntaxKind modifierKind)
        {
            return PropertyDeclarationModifierHelper.Instance.RemoveModifier(propertyDeclaration, modifierKind);
        }

        public static StructDeclarationSyntax Remove(StructDeclarationSyntax structDeclaration, SyntaxToken modifier)
        {
            return StructDeclarationModifierHelper.Instance.RemoveModifier(structDeclaration, modifier);
        }

        public static StructDeclarationSyntax Remove(StructDeclarationSyntax structDeclaration, SyntaxKind modifierKind)
        {
            return StructDeclarationModifierHelper.Instance.RemoveModifier(structDeclaration, modifierKind);
        }

        public static AccessorDeclarationSyntax Remove(AccessorDeclarationSyntax accessorDeclaration, SyntaxToken modifier)
        {
            return AccessorDeclarationModifierHelper.Instance.RemoveModifier(accessorDeclaration, modifier);
        }

        public static AccessorDeclarationSyntax Remove(AccessorDeclarationSyntax accessorDeclaration, SyntaxKind modifierKind)
        {
            return AccessorDeclarationModifierHelper.Instance.RemoveModifier(accessorDeclaration, modifierKind);
        }

        public static LocalDeclarationStatementSyntax Remove(LocalDeclarationStatementSyntax localDeclaration, SyntaxToken modifier)
        {
            return LocalDeclarationStatementModifierHelper.Instance.RemoveModifier(localDeclaration, modifier);
        }

        public static LocalDeclarationStatementSyntax Remove(LocalDeclarationStatementSyntax localDeclaration, SyntaxKind modifierKind)
        {
            return LocalDeclarationStatementModifierHelper.Instance.RemoveModifier(localDeclaration, modifierKind);
        }

        public static LocalFunctionStatementSyntax Remove(LocalFunctionStatementSyntax localFunction, SyntaxToken modifier)
        {
            return LocalFunctionStatementModifierHelper.Instance.RemoveModifier(localFunction, modifier);
        }

        public static LocalFunctionStatementSyntax Remove(LocalFunctionStatementSyntax localFunction, SyntaxKind modifierKind)
        {
            return LocalFunctionStatementModifierHelper.Instance.RemoveModifier(localFunction, modifierKind);
        }

        public static ParameterSyntax Remove(ParameterSyntax parameter, SyntaxToken modifier)
        {
            return ParameterModifierHelper.Instance.RemoveModifier(parameter, modifier);
        }

        public static ParameterSyntax Remove(ParameterSyntax parameter, SyntaxKind modifierKind)
        {
            return ParameterModifierHelper.Instance.RemoveModifier(parameter, modifierKind);
        }

        public static ClassDeclarationSyntax RemoveAt(ClassDeclarationSyntax classDeclaration, int index)
        {
            return ClassDeclarationModifierHelper.Instance.RemoveModifierAt(classDeclaration, index);
        }

        public static ConstructorDeclarationSyntax RemoveAt(ConstructorDeclarationSyntax constructorDeclaration, int index)
        {
            return ConstructorDeclarationModifierHelper.Instance.RemoveModifierAt(constructorDeclaration, index);
        }

        public static ConversionOperatorDeclarationSyntax RemoveAt(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, int index)
        {
            return ConversionOperatorDeclarationModifierHelper.Instance.RemoveModifierAt(conversionOperatorDeclaration, index);
        }

        public static DelegateDeclarationSyntax RemoveAt(DelegateDeclarationSyntax delegateDeclaration, int index)
        {
            return DelegateDeclarationModifierHelper.Instance.RemoveModifierAt(delegateDeclaration, index);
        }

        public static DestructorDeclarationSyntax RemoveAt(DestructorDeclarationSyntax destructorDeclaration, int index)
        {
            return DestructorDeclarationModifierHelper.Instance.RemoveModifierAt(destructorDeclaration, index);
        }

        public static EnumDeclarationSyntax RemoveAt(EnumDeclarationSyntax enumDeclaration, int index)
        {
            return EnumDeclarationModifierHelper.Instance.RemoveModifierAt(enumDeclaration, index);
        }

        public static EventDeclarationSyntax RemoveAt(EventDeclarationSyntax eventDeclaration, int index)
        {
            return EventDeclarationModifierHelper.Instance.RemoveModifierAt(eventDeclaration, index);
        }

        public static EventFieldDeclarationSyntax RemoveAt(EventFieldDeclarationSyntax eventFieldDeclaration, int index)
        {
            return EventFieldDeclarationModifierHelper.Instance.RemoveModifierAt(eventFieldDeclaration, index);
        }

        public static FieldDeclarationSyntax RemoveAt(FieldDeclarationSyntax fieldDeclaration, int index)
        {
            return FieldDeclarationModifierHelper.Instance.RemoveModifierAt(fieldDeclaration, index);
        }

        public static IndexerDeclarationSyntax RemoveAt(IndexerDeclarationSyntax indexerDeclaration, int index)
        {
            return IndexerDeclarationModifierHelper.Instance.RemoveModifierAt(indexerDeclaration, index);
        }

        public static InterfaceDeclarationSyntax RemoveAt(InterfaceDeclarationSyntax interfaceDeclaration, int index)
        {
            return InterfaceDeclarationModifierHelper.Instance.RemoveModifierAt(interfaceDeclaration, index);
        }

        public static MethodDeclarationSyntax RemoveAt(MethodDeclarationSyntax methodDeclaration, int index)
        {
            return MethodDeclarationModifierHelper.Instance.RemoveModifierAt(methodDeclaration, index);
        }

        public static OperatorDeclarationSyntax RemoveAt(OperatorDeclarationSyntax operatorDeclaration, int index)
        {
            return OperatorDeclarationModifierHelper.Instance.RemoveModifierAt(operatorDeclaration, index);
        }

        public static PropertyDeclarationSyntax RemoveAt(PropertyDeclarationSyntax propertyDeclaration, int index)
        {
            return PropertyDeclarationModifierHelper.Instance.RemoveModifierAt(propertyDeclaration, index);
        }

        public static StructDeclarationSyntax RemoveAt(StructDeclarationSyntax structDeclaration, int index)
        {
            return StructDeclarationModifierHelper.Instance.RemoveModifierAt(structDeclaration, index);
        }

        public static AccessorDeclarationSyntax RemoveAt(AccessorDeclarationSyntax accessorDeclaration, int index)
        {
            return AccessorDeclarationModifierHelper.Instance.RemoveModifierAt(accessorDeclaration, index);
        }

        public static LocalDeclarationStatementSyntax RemoveAt(LocalDeclarationStatementSyntax localDeclaration, int index)
        {
            return LocalDeclarationStatementModifierHelper.Instance.RemoveModifierAt(localDeclaration, index);
        }

        public static LocalFunctionStatementSyntax RemoveAt(LocalFunctionStatementSyntax localFunction, int index)
        {
            return LocalFunctionStatementModifierHelper.Instance.RemoveModifierAt(localFunction, index);
        }

        public static ParameterSyntax RemoveAt(ParameterSyntax parameter, int index)
        {
            return ParameterModifierHelper.Instance.RemoveModifierAt(parameter, index);
        }

        public static ClassDeclarationSyntax RemoveAccess(ClassDeclarationSyntax classDeclaration)
        {
            return ClassDeclarationModifierHelper.Instance.RemoveAccessModifiers(classDeclaration);
        }

        public static ConstructorDeclarationSyntax RemoveAccess(ConstructorDeclarationSyntax constructorDeclaration)
        {
            return ConstructorDeclarationModifierHelper.Instance.RemoveAccessModifiers(constructorDeclaration);
        }

        public static ConversionOperatorDeclarationSyntax RemoveAccess(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration)
        {
            return ConversionOperatorDeclarationModifierHelper.Instance.RemoveAccessModifiers(conversionOperatorDeclaration);
        }

        public static DelegateDeclarationSyntax RemoveAccess(DelegateDeclarationSyntax delegateDeclaration)
        {
            return DelegateDeclarationModifierHelper.Instance.RemoveAccessModifiers(delegateDeclaration);
        }

        public static DestructorDeclarationSyntax RemoveAccess(DestructorDeclarationSyntax destructorDeclaration)
        {
            return DestructorDeclarationModifierHelper.Instance.RemoveAccessModifiers(destructorDeclaration);
        }

        public static EnumDeclarationSyntax RemoveAccess(EnumDeclarationSyntax enumDeclaration)
        {
            return EnumDeclarationModifierHelper.Instance.RemoveAccessModifiers(enumDeclaration);
        }

        public static EventDeclarationSyntax RemoveAccess(EventDeclarationSyntax eventDeclaration)
        {
            return EventDeclarationModifierHelper.Instance.RemoveAccessModifiers(eventDeclaration);
        }

        public static EventFieldDeclarationSyntax RemoveAccess(EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            return EventFieldDeclarationModifierHelper.Instance.RemoveAccessModifiers(eventFieldDeclaration);
        }

        public static FieldDeclarationSyntax RemoveAccess(FieldDeclarationSyntax fieldDeclaration)
        {
            return FieldDeclarationModifierHelper.Instance.RemoveAccessModifiers(fieldDeclaration);
        }

        public static IndexerDeclarationSyntax RemoveAccess(IndexerDeclarationSyntax indexerDeclaration)
        {
            return IndexerDeclarationModifierHelper.Instance.RemoveAccessModifiers(indexerDeclaration);
        }

        public static InterfaceDeclarationSyntax RemoveAccess(InterfaceDeclarationSyntax interfaceDeclaration)
        {
            return InterfaceDeclarationModifierHelper.Instance.RemoveAccessModifiers(interfaceDeclaration);
        }

        public static MethodDeclarationSyntax RemoveAccess(MethodDeclarationSyntax methodDeclaration)
        {
            return MethodDeclarationModifierHelper.Instance.RemoveAccessModifiers(methodDeclaration);
        }

        public static OperatorDeclarationSyntax RemoveAccess(OperatorDeclarationSyntax operatorDeclaration)
        {
            return OperatorDeclarationModifierHelper.Instance.RemoveAccessModifiers(operatorDeclaration);
        }

        public static PropertyDeclarationSyntax RemoveAccess(PropertyDeclarationSyntax propertyDeclaration)
        {
            return PropertyDeclarationModifierHelper.Instance.RemoveAccessModifiers(propertyDeclaration);
        }

        public static StructDeclarationSyntax RemoveAccess(StructDeclarationSyntax structDeclaration)
        {
            return StructDeclarationModifierHelper.Instance.RemoveAccessModifiers(structDeclaration);
        }

        public static AccessorDeclarationSyntax RemoveAccess(AccessorDeclarationSyntax accessorDeclaration)
        {
            return AccessorDeclarationModifierHelper.Instance.RemoveAccessModifiers(accessorDeclaration);
        }

        public static LocalDeclarationStatementSyntax RemoveAccess(LocalDeclarationStatementSyntax localDeclaration)
        {
            return LocalDeclarationStatementModifierHelper.Instance.RemoveAccessModifiers(localDeclaration);
        }

        public static LocalFunctionStatementSyntax RemoveAccess(LocalFunctionStatementSyntax localFunction)
        {
            return LocalFunctionStatementModifierHelper.Instance.RemoveAccessModifiers(localFunction);
        }

        public static ParameterSyntax RemoveAccess(ParameterSyntax parameter)
        {
            return ParameterModifierHelper.Instance.RemoveAccessModifiers(parameter);
        }

        public static ClassDeclarationSyntax RemoveAll(ClassDeclarationSyntax classDeclaration)
        {
            return ClassDeclarationModifierHelper.Instance.RemoveModifiers(classDeclaration);
        }

        public static ConstructorDeclarationSyntax RemoveAll(ConstructorDeclarationSyntax constructorDeclaration)
        {
            return ConstructorDeclarationModifierHelper.Instance.RemoveModifiers(constructorDeclaration);
        }

        public static ConversionOperatorDeclarationSyntax RemoveAll(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration)
        {
            return ConversionOperatorDeclarationModifierHelper.Instance.RemoveModifiers(conversionOperatorDeclaration);
        }

        public static DelegateDeclarationSyntax RemoveAll(DelegateDeclarationSyntax delegateDeclaration)
        {
            return DelegateDeclarationModifierHelper.Instance.RemoveModifiers(delegateDeclaration);
        }

        public static DestructorDeclarationSyntax RemoveAll(DestructorDeclarationSyntax destructorDeclaration)
        {
            return DestructorDeclarationModifierHelper.Instance.RemoveModifiers(destructorDeclaration);
        }

        public static EnumDeclarationSyntax RemoveAll(EnumDeclarationSyntax enumDeclaration)
        {
            return EnumDeclarationModifierHelper.Instance.RemoveModifiers(enumDeclaration);
        }

        public static EventDeclarationSyntax RemoveAll(EventDeclarationSyntax eventDeclaration)
        {
            return EventDeclarationModifierHelper.Instance.RemoveModifiers(eventDeclaration);
        }

        public static EventFieldDeclarationSyntax RemoveAll(EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            return EventFieldDeclarationModifierHelper.Instance.RemoveModifiers(eventFieldDeclaration);
        }

        public static FieldDeclarationSyntax RemoveAll(FieldDeclarationSyntax fieldDeclaration)
        {
            return FieldDeclarationModifierHelper.Instance.RemoveModifiers(fieldDeclaration);
        }

        public static IndexerDeclarationSyntax RemoveAll(IndexerDeclarationSyntax indexerDeclaration)
        {
            return IndexerDeclarationModifierHelper.Instance.RemoveModifiers(indexerDeclaration);
        }

        public static InterfaceDeclarationSyntax RemoveAll(InterfaceDeclarationSyntax interfaceDeclaration)
        {
            return InterfaceDeclarationModifierHelper.Instance.RemoveModifiers(interfaceDeclaration);
        }

        public static MethodDeclarationSyntax RemoveAll(MethodDeclarationSyntax methodDeclaration)
        {
            return MethodDeclarationModifierHelper.Instance.RemoveModifiers(methodDeclaration);
        }

        public static OperatorDeclarationSyntax RemoveAll(OperatorDeclarationSyntax operatorDeclaration)
        {
            return OperatorDeclarationModifierHelper.Instance.RemoveModifiers(operatorDeclaration);
        }

        public static PropertyDeclarationSyntax RemoveAll(PropertyDeclarationSyntax propertyDeclaration)
        {
            return PropertyDeclarationModifierHelper.Instance.RemoveModifiers(propertyDeclaration);
        }

        public static StructDeclarationSyntax RemoveAll(StructDeclarationSyntax structDeclaration)
        {
            return StructDeclarationModifierHelper.Instance.RemoveModifiers(structDeclaration);
        }

        public static AccessorDeclarationSyntax RemoveAll(AccessorDeclarationSyntax accessorDeclaration)
        {
            return AccessorDeclarationModifierHelper.Instance.RemoveModifiers(accessorDeclaration);
        }

        public static LocalDeclarationStatementSyntax RemoveAll(LocalDeclarationStatementSyntax localDeclaration)
        {
            return LocalDeclarationStatementModifierHelper.Instance.RemoveModifiers(localDeclaration);
        }

        public static LocalFunctionStatementSyntax RemoveAll(LocalFunctionStatementSyntax localFunction)
        {
            return LocalFunctionStatementModifierHelper.Instance.RemoveModifiers(localFunction);
        }

        public static ParameterSyntax RemoveAll(ParameterSyntax parameter)
        {
            return ParameterModifierHelper.Instance.RemoveModifiers(parameter);
        }

        public static string GetName(SyntaxKind modifierKind)
        {
            switch (modifierKind)
            {
                case SyntaxKind.NewKeyword:
                    return "new";
                case SyntaxKind.PublicKeyword:
                    return "public";
                case SyntaxKind.ProtectedKeyword:
                    return "protected";
                case SyntaxKind.InternalKeyword:
                    return "internal";
                case SyntaxKind.PrivateKeyword:
                    return "private";
                case SyntaxKind.ConstKeyword:
                    return "const";
                case SyntaxKind.StaticKeyword:
                    return "static";
                case SyntaxKind.VirtualKeyword:
                    return "virtual";
                case SyntaxKind.SealedKeyword:
                    return "sealed";
                case SyntaxKind.OverrideKeyword:
                    return "override";
                case SyntaxKind.AbstractKeyword:
                    return "abstract";
                case SyntaxKind.ReadOnlyKeyword:
                    return "readonly";
                case SyntaxKind.ExternKeyword:
                    return "extern";
                case SyntaxKind.UnsafeKeyword:
                    return "unsafe";
                case SyntaxKind.VolatileKeyword:
                    return "volatile";
                case SyntaxKind.AsyncKeyword:
                    return "async";
                case SyntaxKind.PartialKeyword:
                    return "partial";
                case SyntaxKind.ThisKeyword:
                    return "this";
                case SyntaxKind.ParamsKeyword:
                    return "params";
                case SyntaxKind.InKeyword:
                    return "in";
                case SyntaxKind.OutKeyword:
                    return "out";
                case SyntaxKind.RefKeyword:
                    return "ref";
                default:
                    {
                        Debug.Fail(modifierKind.ToString());
                        return null;
                    }
            }
        }
    }
}
