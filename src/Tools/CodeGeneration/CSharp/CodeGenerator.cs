// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class CodeGenerator
    {
        public static CompilationUnitSyntax GenerateConfigOptions(IEnumerable<OptionMetadata> options)
        {
            return CompilationUnit(
                UsingDirectives(),
                NamespaceDeclaration(
                    "Roslynator",
                    ClassDeclaration(
                        Modifiers.Public_Static(),
                        "ConfigOptions",
                        options
                            .OrderBy(f => f.Id)
                            .Select(f =>
                            {
                                return FieldDeclaration(
                                    Modifiers.Public_Static_ReadOnly(),
                                    IdentifierName("ConfigOptionDescriptor"),
                                    f.Id,
                                    ImplicitObjectCreationExpression(
                                        ArgumentList(
                                            Argument(NameColon("key"), ParseExpression($"ConfigOptionKeys.{f.Id}")),
                                            Argument(NameColon("defaultValue"), StringLiteralExpression(f.DefaultValue)),
                                            Argument(NameColon("description"), StringLiteralExpression(f.Description)),
                                            Argument(NameColon("valuePlaceholder"), StringLiteralExpression(f.ValuePlaceholder))),
                                        default(InitializerExpressionSyntax)));
                            })
                            .ToSyntaxList<MemberDeclarationSyntax>())));
        }

        public static CompilationUnitSyntax GenerateConfigOptionKeys(IEnumerable<OptionMetadata> options)
        {
            return CompilationUnit(
                UsingDirectives(),
                NamespaceDeclaration(
                    "Roslynator",
                    ClassDeclaration(
                        Modifiers.Internal_Static_Partial(),
                        "ConfigOptionKeys",
                        options
                            .OrderBy(f => f.Id)
                            .Select(f =>
                            {
                                return FieldDeclaration(
                                    Modifiers.Public_Const(),
                                    PredefinedStringType(),
                                    f.Id,
                                    StringLiteralExpression(f.Key));
                            })
                            .ToSyntaxList<MemberDeclarationSyntax>())));
        }
    }
}
