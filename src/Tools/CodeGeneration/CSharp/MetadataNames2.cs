// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;

namespace Roslynator.CodeGeneration.CSharp
{
    internal static class MetadataNames2
    {
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp, "Syntax");

        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_AnonymousFunctionExpressionSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "AnonymousFunctionExpressionSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_BaseTypeSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "BaseTypeSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_CrefSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "CrefSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "ExpressionSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_InterpolatedStringContentSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "InterpolatedStringContentSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_MemberCrefSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "MemberCrefSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "MemberDeclarationSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_PatternSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "PatternSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_QueryClauseSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "QueryClauseSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_SelectOrGroupClauseSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "SelectOrGroupClauseSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_SimpleNameSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "SimpleNameSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "StatementSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_SwitchLabelSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "SwitchLabelSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_TypeParameterConstraintSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "TypeParameterConstraintSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "TypeSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_VariableDesignationSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "VariableDesignationSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_XmlAttributeSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "XmlAttributeSyntax");
        public static readonly MetadataName Microsoft_CodeAnalysis_CSharp_Syntax_XmlNodeSyntax = new MetadataName(Namespaces.Microsoft_CodeAnalysis_CSharp_Syntax, "XmlNodeSyntax");

        private static class Namespaces
        {
            public static readonly ImmutableArray<string> Microsoft_CodeAnalysis_CSharp = ImmutableArray.Create("Microsoft", "CodeAnalysis", "CSharp");
            public static readonly ImmutableArray<string> Microsoft_CodeAnalysis_CSharp_Syntax = ImmutableArray.Create("Microsoft", "CodeAnalysis", "CSharp", "Syntax");
        }
    }
}
