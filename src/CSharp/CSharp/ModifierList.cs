// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    /// <summary>
    /// A set of static methods that allows manipulation with modifiers.
    /// </summary>
    public static class ModifierList
    {
        /// <summary>
        /// Returns an index the specified token should be inserted at.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="token"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int GetInsertIndex(SyntaxTokenList tokens, SyntaxToken token, IComparer<SyntaxToken> comparer = null)
        {
            if (comparer == null)
                comparer = ModifierComparer.Default;

            int index = -1;

            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                int result = comparer.Compare(tokens[i], token);

                if (result == 0)
                {
                    return i + 1;
                }
                else if (result < 0
                    && index == -1)
                {
                    index = i + 1;
                }
            }

            if (index == -1)
                return 0;

            return index;
        }

        /// <summary>
        /// Returns an index a token with the specified kind should be inserted at.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="kind"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int GetInsertIndex(SyntaxTokenList tokens, SyntaxKind kind, IComparer<SyntaxKind> comparer = null)
        {
            if (comparer == null)
                comparer = ModifierKindComparer.Default;

            int index = -1;

            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                int result = comparer.Compare(tokens[i].Kind(), kind);

                if (result == 0)
                {
                    return i + 1;
                }
                else if (result < 0
                    && index == -1)
                {
                    index = i + 1;
                }
            }

            if (index == -1)
                return 0;

            return index;
        }

        internal static SyntaxNode Insert(SyntaxNode node, Accessibility accessibility, IComparer<SyntaxKind> comparer = null)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    {
                        return Insert(node, SyntaxKind.PrivateKeyword, comparer);
                    }
                case Accessibility.Protected:
                    {
                        return Insert(node, SyntaxKind.ProtectedKeyword, comparer);
                    }
                case Accessibility.ProtectedAndInternal:
                    {
                        node = Insert(node, SyntaxKind.PrivateKeyword, comparer);

                        return Insert(node, SyntaxKind.ProtectedKeyword, comparer);
                    }
                case Accessibility.Internal:
                    {
                        return Insert(node, SyntaxKind.InternalKeyword, comparer);
                    }
                case Accessibility.Public:
                    {
                        return Insert(node, SyntaxKind.PublicKeyword, comparer);
                    }
                case Accessibility.ProtectedOrInternal:
                    {
                        node = Insert(node, SyntaxKind.ProtectedKeyword, comparer);

                        return Insert(node, SyntaxKind.InternalKeyword, comparer);
                    }
            }

            return node;
        }

        /// <summary>
        /// Creates a new node with a modifier of the specified kind inserted.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <param name="kind"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static TNode Insert<TNode>(TNode node, SyntaxKind kind, IComparer<SyntaxKind> comparer = null) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ClassDeclarationSyntax>.Instance.Insert((ClassDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConstructorDeclarationSyntax>.Instance.Insert((ConstructorDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConversionOperatorDeclarationSyntax>.Instance.Insert((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DelegateDeclarationSyntax>.Instance.Insert((DelegateDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DestructorDeclarationSyntax>.Instance.Insert((DestructorDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EnumDeclarationSyntax>.Instance.Insert((EnumDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventDeclarationSyntax>.Instance.Insert((EventDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventFieldDeclarationSyntax>.Instance.Insert((EventFieldDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<FieldDeclarationSyntax>.Instance.Insert((FieldDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<IndexerDeclarationSyntax>.Instance.Insert((IndexerDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<InterfaceDeclarationSyntax>.Instance.Insert((InterfaceDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<MethodDeclarationSyntax>.Instance.Insert((MethodDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<OperatorDeclarationSyntax>.Instance.Insert((OperatorDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<PropertyDeclarationSyntax>.Instance.Insert((PropertyDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<StructDeclarationSyntax>.Instance.Insert((StructDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<AccessorDeclarationSyntax>.Instance.Insert((AccessorDeclarationSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalDeclarationStatementSyntax>.Instance.Insert((LocalDeclarationStatementSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalFunctionStatementSyntax>.Instance.Insert((LocalFunctionStatementSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)ModifierList<ParameterSyntax>.Instance.Insert((ParameterSyntax)(SyntaxNode)node, kind, comparer);
                case SyntaxKind.IncompleteMember:
                    return (TNode)(SyntaxNode)ModifierList<IncompleteMemberSyntax>.Instance.Insert((IncompleteMemberSyntax)(SyntaxNode)node, kind, comparer);
            }

            throw new ArgumentException($"'{node.Kind()}' cannot have modifiers.", nameof(node));
        }

        /// <summary>
        /// Creates a new node with the specified modifier inserted.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <param name="modifier"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static TNode Insert<TNode>(TNode node, SyntaxToken modifier, IComparer<SyntaxToken> comparer = null) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ClassDeclarationSyntax>.Instance.Insert((ClassDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConstructorDeclarationSyntax>.Instance.Insert((ConstructorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConversionOperatorDeclarationSyntax>.Instance.Insert((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DelegateDeclarationSyntax>.Instance.Insert((DelegateDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DestructorDeclarationSyntax>.Instance.Insert((DestructorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EnumDeclarationSyntax>.Instance.Insert((EnumDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventDeclarationSyntax>.Instance.Insert((EventDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventFieldDeclarationSyntax>.Instance.Insert((EventFieldDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<FieldDeclarationSyntax>.Instance.Insert((FieldDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<IndexerDeclarationSyntax>.Instance.Insert((IndexerDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<InterfaceDeclarationSyntax>.Instance.Insert((InterfaceDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<MethodDeclarationSyntax>.Instance.Insert((MethodDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<OperatorDeclarationSyntax>.Instance.Insert((OperatorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<PropertyDeclarationSyntax>.Instance.Insert((PropertyDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<StructDeclarationSyntax>.Instance.Insert((StructDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<AccessorDeclarationSyntax>.Instance.Insert((AccessorDeclarationSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalDeclarationStatementSyntax>.Instance.Insert((LocalDeclarationStatementSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalFunctionStatementSyntax>.Instance.Insert((LocalFunctionStatementSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)ModifierList<ParameterSyntax>.Instance.Insert((ParameterSyntax)(SyntaxNode)node, modifier, comparer);
                case SyntaxKind.IncompleteMember:
                    return (TNode)(SyntaxNode)ModifierList<IncompleteMemberSyntax>.Instance.Insert((IncompleteMemberSyntax)(SyntaxNode)node, modifier, comparer);
            }

            throw new ArgumentException($"'{node.Kind()}' cannot have modifiers.", nameof(node));
        }

        /// <summary>
        /// Creates a new node with a modifier of the specified kind removed.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static TNode Remove<TNode>(TNode node, SyntaxKind kind) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ClassDeclarationSyntax>.Instance.Remove((ClassDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConstructorDeclarationSyntax>.Instance.Remove((ConstructorDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConversionOperatorDeclarationSyntax>.Instance.Remove((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DelegateDeclarationSyntax>.Instance.Remove((DelegateDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DestructorDeclarationSyntax>.Instance.Remove((DestructorDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EnumDeclarationSyntax>.Instance.Remove((EnumDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventDeclarationSyntax>.Instance.Remove((EventDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventFieldDeclarationSyntax>.Instance.Remove((EventFieldDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<FieldDeclarationSyntax>.Instance.Remove((FieldDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<IndexerDeclarationSyntax>.Instance.Remove((IndexerDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<InterfaceDeclarationSyntax>.Instance.Remove((InterfaceDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<MethodDeclarationSyntax>.Instance.Remove((MethodDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<OperatorDeclarationSyntax>.Instance.Remove((OperatorDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<PropertyDeclarationSyntax>.Instance.Remove((PropertyDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<StructDeclarationSyntax>.Instance.Remove((StructDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<AccessorDeclarationSyntax>.Instance.Remove((AccessorDeclarationSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalDeclarationStatementSyntax>.Instance.Remove((LocalDeclarationStatementSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalFunctionStatementSyntax>.Instance.Remove((LocalFunctionStatementSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)ModifierList<ParameterSyntax>.Instance.Remove((ParameterSyntax)(SyntaxNode)node, kind);
                case SyntaxKind.IncompleteMember:
                    return (TNode)(SyntaxNode)ModifierList<IncompleteMemberSyntax>.Instance.Remove((IncompleteMemberSyntax)(SyntaxNode)node, kind);
            }

            throw new ArgumentException($"'{node.Kind()}' cannot have modifiers.", nameof(node));
        }

        /// <summary>
        /// Creates a new node with the specified modifier removed.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public static TNode Remove<TNode>(TNode node, SyntaxToken modifier) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ClassDeclarationSyntax>.Instance.Remove((ClassDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConstructorDeclarationSyntax>.Instance.Remove((ConstructorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConversionOperatorDeclarationSyntax>.Instance.Remove((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DelegateDeclarationSyntax>.Instance.Remove((DelegateDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DestructorDeclarationSyntax>.Instance.Remove((DestructorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EnumDeclarationSyntax>.Instance.Remove((EnumDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventDeclarationSyntax>.Instance.Remove((EventDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventFieldDeclarationSyntax>.Instance.Remove((EventFieldDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<FieldDeclarationSyntax>.Instance.Remove((FieldDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<IndexerDeclarationSyntax>.Instance.Remove((IndexerDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<InterfaceDeclarationSyntax>.Instance.Remove((InterfaceDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<MethodDeclarationSyntax>.Instance.Remove((MethodDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<OperatorDeclarationSyntax>.Instance.Remove((OperatorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<PropertyDeclarationSyntax>.Instance.Remove((PropertyDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<StructDeclarationSyntax>.Instance.Remove((StructDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<AccessorDeclarationSyntax>.Instance.Remove((AccessorDeclarationSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalDeclarationStatementSyntax>.Instance.Remove((LocalDeclarationStatementSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalFunctionStatementSyntax>.Instance.Remove((LocalFunctionStatementSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)ModifierList<ParameterSyntax>.Instance.Remove((ParameterSyntax)(SyntaxNode)node, modifier);
                case SyntaxKind.IncompleteMember:
                    return (TNode)(SyntaxNode)ModifierList<IncompleteMemberSyntax>.Instance.Remove((IncompleteMemberSyntax)(SyntaxNode)node, modifier);
            }

            throw new ArgumentException($"'{node.Kind()}' cannot have modifiers.", nameof(node));
        }

        /// <summary>
        /// Creates a new node with a modifier at the specified index removed.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static TNode RemoveAt<TNode>(TNode node, int index) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ClassDeclarationSyntax>.Instance.RemoveAt((ClassDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConstructorDeclarationSyntax>.Instance.RemoveAt((ConstructorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConversionOperatorDeclarationSyntax>.Instance.RemoveAt((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DelegateDeclarationSyntax>.Instance.RemoveAt((DelegateDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DestructorDeclarationSyntax>.Instance.RemoveAt((DestructorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EnumDeclarationSyntax>.Instance.RemoveAt((EnumDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventDeclarationSyntax>.Instance.RemoveAt((EventDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventFieldDeclarationSyntax>.Instance.RemoveAt((EventFieldDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<FieldDeclarationSyntax>.Instance.RemoveAt((FieldDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<IndexerDeclarationSyntax>.Instance.RemoveAt((IndexerDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<InterfaceDeclarationSyntax>.Instance.RemoveAt((InterfaceDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<MethodDeclarationSyntax>.Instance.RemoveAt((MethodDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<OperatorDeclarationSyntax>.Instance.RemoveAt((OperatorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<PropertyDeclarationSyntax>.Instance.RemoveAt((PropertyDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<StructDeclarationSyntax>.Instance.RemoveAt((StructDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<AccessorDeclarationSyntax>.Instance.RemoveAt((AccessorDeclarationSyntax)(SyntaxNode)node, index);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalDeclarationStatementSyntax>.Instance.RemoveAt((LocalDeclarationStatementSyntax)(SyntaxNode)node, index);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalFunctionStatementSyntax>.Instance.RemoveAt((LocalFunctionStatementSyntax)(SyntaxNode)node, index);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)ModifierList<ParameterSyntax>.Instance.RemoveAt((ParameterSyntax)(SyntaxNode)node, index);
                case SyntaxKind.IncompleteMember:
                    return (TNode)(SyntaxNode)ModifierList<IncompleteMemberSyntax>.Instance.RemoveAt((IncompleteMemberSyntax)(SyntaxNode)node, index);
            }

            throw new ArgumentException($"'{node.Kind()}' cannot have modifiers.", nameof(node));
        }

        /// <summary>
        /// Creates a new node with modifiers that matches the predicate removed.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TNode RemoveAll<TNode>(TNode node, Func<SyntaxToken, bool> predicate) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ClassDeclarationSyntax>.Instance.RemoveAll((ClassDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConstructorDeclarationSyntax>.Instance.RemoveAll((ConstructorDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConversionOperatorDeclarationSyntax>.Instance.RemoveAll((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DelegateDeclarationSyntax>.Instance.RemoveAll((DelegateDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DestructorDeclarationSyntax>.Instance.RemoveAll((DestructorDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EnumDeclarationSyntax>.Instance.RemoveAll((EnumDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventDeclarationSyntax>.Instance.RemoveAll((EventDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventFieldDeclarationSyntax>.Instance.RemoveAll((EventFieldDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<FieldDeclarationSyntax>.Instance.RemoveAll((FieldDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<IndexerDeclarationSyntax>.Instance.RemoveAll((IndexerDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<InterfaceDeclarationSyntax>.Instance.RemoveAll((InterfaceDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<MethodDeclarationSyntax>.Instance.RemoveAll((MethodDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<OperatorDeclarationSyntax>.Instance.RemoveAll((OperatorDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<PropertyDeclarationSyntax>.Instance.RemoveAll((PropertyDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<StructDeclarationSyntax>.Instance.RemoveAll((StructDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<AccessorDeclarationSyntax>.Instance.RemoveAll((AccessorDeclarationSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalDeclarationStatementSyntax>.Instance.RemoveAll((LocalDeclarationStatementSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalFunctionStatementSyntax>.Instance.RemoveAll((LocalFunctionStatementSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)ModifierList<ParameterSyntax>.Instance.RemoveAll((ParameterSyntax)(SyntaxNode)node, predicate);
                case SyntaxKind.IncompleteMember:
                    return (TNode)(SyntaxNode)ModifierList<IncompleteMemberSyntax>.Instance.RemoveAll((IncompleteMemberSyntax)(SyntaxNode)node, predicate);
            }

            throw new ArgumentException($"'{node.Kind()}' cannot have modifiers.", nameof(node));
        }

        /// <summary>
        /// Creates a new node with all modifiers removed.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static TNode RemoveAll<TNode>(TNode node) where TNode : SyntaxNode
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ClassDeclarationSyntax>.Instance.RemoveAll((ClassDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.ConstructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConstructorDeclarationSyntax>.Instance.RemoveAll((ConstructorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<ConversionOperatorDeclarationSyntax>.Instance.RemoveAll((ConversionOperatorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.DelegateDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DelegateDeclarationSyntax>.Instance.RemoveAll((DelegateDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.DestructorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<DestructorDeclarationSyntax>.Instance.RemoveAll((DestructorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.EnumDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EnumDeclarationSyntax>.Instance.RemoveAll((EnumDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.EventDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventDeclarationSyntax>.Instance.RemoveAll((EventDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.EventFieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<EventFieldDeclarationSyntax>.Instance.RemoveAll((EventFieldDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.FieldDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<FieldDeclarationSyntax>.Instance.RemoveAll((FieldDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.IndexerDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<IndexerDeclarationSyntax>.Instance.RemoveAll((IndexerDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.InterfaceDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<InterfaceDeclarationSyntax>.Instance.RemoveAll((InterfaceDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.MethodDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<MethodDeclarationSyntax>.Instance.RemoveAll((MethodDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.OperatorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<OperatorDeclarationSyntax>.Instance.RemoveAll((OperatorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.PropertyDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<PropertyDeclarationSyntax>.Instance.RemoveAll((PropertyDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.StructDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<StructDeclarationSyntax>.Instance.RemoveAll((StructDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return (TNode)(SyntaxNode)ModifierList<AccessorDeclarationSyntax>.Instance.RemoveAll((AccessorDeclarationSyntax)(SyntaxNode)node);
                case SyntaxKind.LocalDeclarationStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalDeclarationStatementSyntax>.Instance.RemoveAll((LocalDeclarationStatementSyntax)(SyntaxNode)node);
                case SyntaxKind.LocalFunctionStatement:
                    return (TNode)(SyntaxNode)ModifierList<LocalFunctionStatementSyntax>.Instance.RemoveAll((LocalFunctionStatementSyntax)(SyntaxNode)node);
                case SyntaxKind.Parameter:
                    return (TNode)(SyntaxNode)ModifierList<ParameterSyntax>.Instance.RemoveAll((ParameterSyntax)(SyntaxNode)node);
                case SyntaxKind.IncompleteMember:
                    return (TNode)(SyntaxNode)ModifierList<IncompleteMemberSyntax>.Instance.RemoveAll((IncompleteMemberSyntax)(SyntaxNode)node);
            }

            throw new ArgumentException($"'{node.Kind()}' cannot have modifiers.", nameof(node));
        }

        /// <summary>
        /// Creates a new list of modifiers with the modifier of the specified kind inserted.
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="kind"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static SyntaxTokenList Insert(SyntaxTokenList modifiers, SyntaxKind kind, IComparer<SyntaxKind> comparer = null)
        {
            if (!modifiers.Any())
                return modifiers.Add(Token(kind));

            return InsertImpl(modifiers, Token(kind), GetInsertIndex(modifiers, kind, comparer));
        }

        /// <summary>
        /// Creates a new list of modifiers with a specified modifier inserted.
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="modifier"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static SyntaxTokenList Insert(SyntaxTokenList modifiers, SyntaxToken modifier, IComparer<SyntaxToken> comparer = null)
        {
            if (!modifiers.Any())
                return modifiers.Add(modifier);

            return InsertImpl(modifiers, modifier, GetInsertIndex(modifiers, modifier, comparer));
        }

        private static SyntaxTokenList InsertImpl(SyntaxTokenList modifiers, SyntaxToken modifier, int index)
        {
            if (index == 0)
            {
                SyntaxToken firstModifier = modifiers[index];

                SyntaxTriviaList trivia = firstModifier.LeadingTrivia;

                if (trivia.Any())
                {
                    SyntaxTriviaList leadingTrivia = modifier.LeadingTrivia;

                    if (!leadingTrivia.IsSingleElasticMarker())
                        trivia = trivia.AddRange(leadingTrivia);

                    modifier = modifier.WithLeadingTrivia(trivia);

                    modifiers = modifiers.ReplaceAt(index, firstModifier.WithoutLeadingTrivia());
                }
            }

            return modifiers.Insert(index, modifier);
        }
    }
}