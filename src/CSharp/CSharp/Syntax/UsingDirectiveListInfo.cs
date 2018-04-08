// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about a list of using directives.
    /// </summary>
    public readonly struct UsingDirectiveListInfo : IEquatable<UsingDirectiveListInfo>, IReadOnlyList<UsingDirectiveSyntax>
    {
        internal UsingDirectiveListInfo(SyntaxNode parent, SyntaxList<UsingDirectiveSyntax> usings)
        {
            Parent = parent;
            Usings = usings;
        }

        private static UsingDirectiveListInfo Default { get; } = new UsingDirectiveListInfo();

        /// <summary>
        /// The declaration that contains the usings.
        /// </summary>
        public SyntaxNode Parent { get; }

        /// <summary>
        /// A list of usings.
        /// </summary>
        public SyntaxList<UsingDirectiveSyntax> Usings { get; }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return Parent != null; }
        }

        /// <summary>
        /// A number of usings in the list.
        /// </summary>
        public int Count
        {
            get { return Usings.Count; }
        }

        /// <summary>
        /// Gets the using directive at the specified index in the list.
        /// </summary>
        /// <returns>The using directive at the specified index in the list.</returns>
        /// <param name="index">The zero-based index of the using directive to get. </param>
        public UsingDirectiveSyntax this[int index]
        {
            get { return Usings[index]; }
        }

        IEnumerator<UsingDirectiveSyntax> IEnumerable<UsingDirectiveSyntax>.GetEnumerator()
        {
            return ((IReadOnlyList<UsingDirectiveSyntax>)Usings).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyList<UsingDirectiveSyntax>)Usings).GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator for the list of usings.
        /// </summary>
        /// <returns></returns>
        public SyntaxList<UsingDirectiveSyntax>.Enumerator GetEnumerator()
        {
            return Usings.GetEnumerator();
        }

        internal static UsingDirectiveListInfo Create(NamespaceDeclarationSyntax namespaceDeclaration)
        {
            if (namespaceDeclaration == null)
                return Default;

            return new UsingDirectiveListInfo(namespaceDeclaration, namespaceDeclaration.Usings);
        }

        internal static UsingDirectiveListInfo Create(CompilationUnitSyntax compilationUnit)
        {
            if (compilationUnit == null)
                return Default;

            return new UsingDirectiveListInfo(compilationUnit, compilationUnit.Usings);
        }

        internal static UsingDirectiveListInfo Create(SyntaxNode declaration)
        {
            switch (declaration?.Kind())
            {
                case SyntaxKind.CompilationUnit:
                    {
                        var typeDeclaration = (CompilationUnitSyntax)declaration;
                        return new UsingDirectiveListInfo(typeDeclaration, typeDeclaration.Usings);
                    }
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var namespaceDeclaration = (NamespaceDeclarationSyntax)declaration;
                        return new UsingDirectiveListInfo(namespaceDeclaration, namespaceDeclaration.Usings);
                    }
            }

            return Default;
        }

        /// <summary>
        /// Creates a new <see cref="UsingDirectiveListInfo"/> with the usings updated.
        /// </summary>
        /// <param name="usings"></param>
        /// <returns></returns>
        public UsingDirectiveListInfo WithUsings(IEnumerable<UsingDirectiveSyntax> usings)
        {
            return WithUsings(List(usings));
        }

        /// <summary>
        /// Creates a new <see cref="UsingDirectiveListInfo"/> with the usings updated.
        /// </summary>
        /// <param name="usings"></param>
        /// <returns></returns>
        public UsingDirectiveListInfo WithUsings(SyntaxList<UsingDirectiveSyntax> usings)
        {
            ThrowInvalidOperationIfNotInitialized();

            switch (Parent.Kind())
            {
                case SyntaxKind.CompilationUnit:
                    {
                        var declaration = (CompilationUnitSyntax)Parent;
                        declaration = declaration.WithUsings(usings);
                        return new UsingDirectiveListInfo(declaration, declaration.Usings);
                    }
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var declaration = (NamespaceDeclarationSyntax)Parent;
                        declaration = declaration.WithUsings(usings);
                        return new UsingDirectiveListInfo(declaration, declaration.Usings);
                    }
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Creates a new <see cref="UsingDirectiveListInfo"/> with the specified node removed.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public UsingDirectiveListInfo RemoveNode(SyntaxNode node, SyntaxRemoveOptions options)
        {
            ThrowInvalidOperationIfNotInitialized();

            switch (Parent.Kind())
            {
                case SyntaxKind.CompilationUnit:
                    {
                        var declaration = (CompilationUnitSyntax)Parent;
                        declaration = declaration.RemoveNode(node, options);
                        return new UsingDirectiveListInfo(declaration, declaration.Usings);
                    }
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var declaration = (NamespaceDeclarationSyntax)Parent;
                        declaration = declaration.RemoveNode(node, options);
                        return new UsingDirectiveListInfo(declaration, declaration.Usings);
                    }
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Creates a new <see cref="UsingDirectiveListInfo"/> with the specified old node replaced with a new node.
        /// </summary>
        /// <param name="oldNode"></param>
        /// <param name="newNode"></param>
        /// <returns></returns>
        public UsingDirectiveListInfo ReplaceNode(SyntaxNode oldNode, SyntaxNode newNode)
        {
            ThrowInvalidOperationIfNotInitialized();

            switch (Parent.Kind())
            {
                case SyntaxKind.CompilationUnit:
                    {
                        var declaration = (CompilationUnitSyntax)Parent;
                        declaration = declaration.ReplaceNode(oldNode, newNode);
                        return new UsingDirectiveListInfo(declaration, declaration.Usings);
                    }
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var declaration = (NamespaceDeclarationSyntax)Parent;
                        declaration = declaration.ReplaceNode(oldNode, newNode);
                        return new UsingDirectiveListInfo(declaration, declaration.Usings);
                    }
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Creates a new <see cref="UsingDirectiveListInfo"/> with the specified using directive added at the end.
        /// </summary>
        /// <param name="usingDirective"></param>
        /// <returns></returns>
        public UsingDirectiveListInfo Add(UsingDirectiveSyntax usingDirective)
        {
            return WithUsings(Usings.Add(usingDirective));
        }

        /// <summary>
        /// Creates a new <see cref="UsingDirectiveListInfo"/> with the specified usings added at the end.
        /// </summary>
        /// <param name="usings"></param>
        /// <returns></returns>
        public UsingDirectiveListInfo AddRange(IEnumerable<UsingDirectiveSyntax> usings)
        {
            return WithUsings(Usings.AddRange(usings));
        }

        /// <summary>
        /// True if the list has at least one using directive.
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return Usings.Any();
        }

        /// <summary>
        /// The first using directive in the list.
        /// </summary>
        /// <returns></returns>
        public UsingDirectiveSyntax First()
        {
            return Usings.First();
        }

        /// <summary>
        /// The first using directive in the list or null if the list is empty.
        /// </summary>
        /// <returns></returns>
        public UsingDirectiveSyntax FirstOrDefault()
        {
            return Usings.FirstOrDefault();
        }

        /// <summary>
        /// Searches for an using directive that matches the predicate and returns returns zero-based index of the first occurrence in the list.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int IndexOf(Func<UsingDirectiveSyntax, bool> predicate)
        {
            return Usings.IndexOf(predicate);
        }

        /// <summary>
        /// The index of the using directive in the list.
        /// </summary>
        /// <param name="usingDirective"></param>
        /// <returns></returns>
        public int IndexOf(UsingDirectiveSyntax usingDirective)
        {
            return Usings.IndexOf(usingDirective);
        }

        /// <summary>
        /// Creates a new <see cref="UsingDirectiveListInfo"/> with the specified using directive inserted at the index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="usingDirective"></param>
        /// <returns></returns>
        public UsingDirectiveListInfo Insert(int index, UsingDirectiveSyntax usingDirective)
        {
            return WithUsings(Usings.Insert(index, usingDirective));
        }

        /// <summary>
        /// Creates a new <see cref="UsingDirectiveListInfo"/> with the specified usings inserted at the index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="usings"></param>
        /// <returns></returns>
        public UsingDirectiveListInfo InsertRange(int index, IEnumerable<UsingDirectiveSyntax> usings)
        {
            return WithUsings(Usings.InsertRange(index, usings));
        }

        /// <summary>
        /// The last using directive in the list.
        /// </summary>
        /// <returns></returns>
        public UsingDirectiveSyntax Last()
        {
            return Usings.Last();
        }

        /// <summary>
        /// The last using directive in the list or null if the list is empty.
        /// </summary>
        /// <returns></returns>
        public UsingDirectiveSyntax LastOrDefault()
        {
            return Usings.LastOrDefault();
        }

        /// <summary>
        /// Searches for an using directive that matches the predicate and returns returns zero-based index of the last occurrence in the list.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int LastIndexOf(Func<UsingDirectiveSyntax, bool> predicate)
        {
            return Usings.LastIndexOf(predicate);
        }

        /// <summary>
        /// Searches for an using directive and returns zero-based index of the last occurrence in the list.
        /// </summary>
        /// <param name="usingDirective"></param>
        /// <returns></returns>
        public int LastIndexOf(UsingDirectiveSyntax usingDirective)
        {
            return Usings.LastIndexOf(usingDirective);
        }

        /// <summary>
        /// Creates a new <see cref="UsingDirectiveListInfo"/> with the specified using directive removed.
        /// </summary>
        /// <param name="usingDirective"></param>
        /// <returns></returns>
        public UsingDirectiveListInfo Remove(UsingDirectiveSyntax usingDirective)
        {
            return WithUsings(Usings.Remove(usingDirective));
        }

        /// <summary>
        /// Creates a new <see cref="UsingDirectiveListInfo"/> with the using directive at the specified index removed.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public UsingDirectiveListInfo RemoveAt(int index)
        {
            return WithUsings(Usings.RemoveAt(index));
        }

        /// <summary>
        /// Creates a new <see cref="UsingDirectiveListInfo"/> with the specified using directive replaced with the new using directive.
        /// </summary>
        /// <param name="usingInLine"></param>
        /// <param name="newUsingDirective"></param>
        /// <returns></returns>
        public UsingDirectiveListInfo Replace(UsingDirectiveSyntax usingInLine, UsingDirectiveSyntax newUsingDirective)
        {
            return WithUsings(Usings.Replace(usingInLine, newUsingDirective));
        }

        /// <summary>
        /// Creates a new <see cref="UsingDirectiveListInfo"/> with the using directive at the specified index replaced with a new using directive.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newUsingDirective"></param>
        /// <returns></returns>
        public UsingDirectiveListInfo ReplaceAt(int index, UsingDirectiveSyntax newUsingDirective)
        {
            return WithUsings(Usings.ReplaceAt(index, newUsingDirective));
        }

        /// <summary>
        /// Creates a new <see cref="UsingDirectiveListInfo"/> with the specified using directive replaced with new usings.
        /// </summary>
        /// <param name="usingInLine"></param>
        /// <param name="newUsingDirectives"></param>
        /// <returns></returns>
        public UsingDirectiveListInfo ReplaceRange(UsingDirectiveSyntax usingInLine, IEnumerable<UsingDirectiveSyntax> newUsingDirectives)
        {
            return WithUsings(Usings.ReplaceRange(usingInLine, newUsingDirectives));
        }

        private void ThrowInvalidOperationIfNotInitialized()
        {
            if (Parent == null)
                throw new InvalidOperationException($"{nameof(UsingDirectiveListInfo)} is not initalized.");
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Parent?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is UsingDirectiveListInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(UsingDirectiveListInfo other)
        {
            return EqualityComparer<SyntaxNode>.Default.Equals(Parent, other.Parent);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<SyntaxNode>.Default.GetHashCode(Parent);
        }

        public static bool operator ==(UsingDirectiveListInfo info1, UsingDirectiveListInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(UsingDirectiveListInfo info1, UsingDirectiveListInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
