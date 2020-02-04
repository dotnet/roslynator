// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal static class CSharpSyntaxTreeExtensions
    {
        public static SyntaxTrivia GetFirstIndentation(this SyntaxTree tree, CancellationToken cancellationToken = default)
        {
            if (tree.GetRoot(cancellationToken) is CompilationUnitSyntax compilationUnit)
            {
                foreach (MemberDeclarationSyntax member in compilationUnit.Members)
                {
                    foreach (MemberDeclarationSyntax nestedMember in GetMembers(member))
                    {
                        SyntaxTrivia indentation = nestedMember.GetIndentation(cancellationToken);

                        if (!indentation.Span.IsEmpty)
                            return indentation;
                    }
                }
            }

            return default;

            static SyntaxList<MemberDeclarationSyntax> GetMembers(MemberDeclarationSyntax memberDeclaration)
            {
                switch (memberDeclaration)
                {
                    case NamespaceDeclarationSyntax namespaceDeclaration:
                        return namespaceDeclaration.Members;
                    case TypeDeclarationSyntax typeDeclaration:
                        return typeDeclaration.Members;
                    default:
                        return default;
                }
            }
        }
    }
}
