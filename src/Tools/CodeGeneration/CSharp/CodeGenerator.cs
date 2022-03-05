// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class CodeGenerator
    {
        public static CompilationUnitSyntax GenerateConfigOptions(IEnumerable<ConfigOptionMetadata> options, IEnumerable<AnalyzerMetadata> analyzers)
        {
            CompilationUnitSyntax compilationUnit = CompilationUnit(
                UsingDirectives("System.Collections.Generic"),
                NamespaceDeclaration(
                    "Roslynator",
                    ClassDeclaration(
                        Modifiers.Public_Static_Partial(),
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
                                            Argument(NameColon("defaultValue"), (f.DefaultValue != null) ? StringLiteralExpression(f.DefaultValue) : NullLiteralExpression()),
                                            Argument(NameColon("defaultValuePlaceholder"), StringLiteralExpression(f.DefaultValuePlaceholder)),
                                            Argument(NameColon("description"), StringLiteralExpression(f.Description))),
                                        default(InitializerExpressionSyntax)));
                            })
                            .Concat(new MemberDeclarationSyntax[]
                                {
                                    MethodDeclaration(
                                        Modifiers.Private_Static(),
                                        ParseTypeName("IEnumerable<KeyValuePair<string, string>>"),
                                        Identifier("GetRequiredOptions"),
                                        ParameterList(),
                                        Block(
                                            analyzers
                                                .Where(f => f.ConfigOptions.Any(f => f.IsRequired))
                                                .OrderBy(f => f.Id)
                                                .Select(f => (id: f.Id, keys: f.ConfigOptions.Where(f => f.IsRequired)))
                                                .Select(f =>
                                                {
                                                    ConfigOptionKeyMetadata mismatch = f.keys.FirstOrDefault(f => !options.Any(o => o.Key == f.Key));

                                                    Debug.Assert(mismatch.Key == null, mismatch.Key);

                                                    IEnumerable<string> optionKeys = f.keys
                                                        .Join(options, f => f.Key, f => f.Key, (_, g) => g)
                                                        .Select(f => $"ConfigOptionKeys.{f.Id}");

                                                    return YieldReturnStatement(
                                                        ParseExpression($"new KeyValuePair<string, string>(\"{f.id}\", JoinOptionKeys({string.Join(", ", optionKeys)}))"));
                                                })))
                                })
                            .ToSyntaxList())));

            compilationUnit = compilationUnit.NormalizeWhitespace();

            var rewriter = new WrapRewriter(WrapRewriterOptions.WrapArguments);

            return (CompilationUnitSyntax)rewriter.Visit(compilationUnit);
        }

        public static CompilationUnitSyntax GenerateLegacyConfigOptions(IEnumerable<AnalyzerMetadata> analyzers)
        {
            return CompilationUnit(
                UsingDirectives(),
                NamespaceDeclaration(
                    "Roslynator",
                    ClassDeclaration(
                        Modifiers.Public_Static_Partial(),
                        "LegacyConfigOptions",
                        analyzers
                            .SelectMany(f => f.Options)
                            .Where(f => !f.IsObsolete)
                            .OrderBy(f => f.Identifier)
                            .Select(f =>
                            {
                                return FieldDeclaration(
                                    Modifiers.Public_Static_ReadOnly(),
                                    IdentifierName("LegacyConfigOptionDescriptor"),
                                    f.Identifier,
                                    ImplicitObjectCreationExpression(
                                        ArgumentList(
                                            Argument(NameColon("key"), StringLiteralExpression($"roslynator.{f.ParentId}.{f.OptionKey}")),
                                            Argument(NameColon("defaultValue"), NullLiteralExpression()),
                                            Argument(NameColon("defaultValuePlaceholder"), StringLiteralExpression("true|false")),
                                            Argument(NameColon("description"), StringLiteralExpression(""))),
                                        default(InitializerExpressionSyntax)));
                            })
                            .ToSyntaxList<MemberDeclarationSyntax>())));
        }

        public static CompilationUnitSyntax GenerateConfigOptionKeys(IEnumerable<ConfigOptionMetadata> options)
        {
            CompilationUnitSyntax compilationUnit = CompilationUnit(
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

            compilationUnit = compilationUnit.NormalizeWhitespace();

            var rewriter = new WrapRewriter(WrapRewriterOptions.IndentFieldInitializer);

            return (CompilationUnitSyntax)rewriter.Visit(compilationUnit);
        }

        public static CompilationUnitSyntax GenerateConfigOptionValues(IEnumerable<ConfigOptionMetadata> options)
        {
            CompilationUnitSyntax compilationUnit = CompilationUnit(
                UsingDirectives(),
                NamespaceDeclaration(
                    "Roslynator",
                    ClassDeclaration(
                        Modifiers.Internal_Static_Partial(),
                        "ConfigOptionValues",
                        options
                            .OrderBy(option => option.Id)
                            .SelectMany(option =>
                            {
                                return option.Values.Select(v =>
                                {
                                    string value = StringUtility.FirstCharToUpper(v.Value);
                                    value = Regex.Replace(value, @"_\w", f => f.Value.Substring(1).ToUpperInvariant());

                                    return FieldDeclaration(
                                        Modifiers.Public_Const(),
                                        PredefinedStringType(),
                                        $"{option.Id}_{value}",
                                        StringLiteralExpression(v.Value));
                                });
                            })
                            .ToSyntaxList<MemberDeclarationSyntax>())));

            compilationUnit = compilationUnit.NormalizeWhitespace();

            var rewriter = new WrapRewriter(WrapRewriterOptions.IndentFieldInitializer);

            return (CompilationUnitSyntax)rewriter.Visit(compilationUnit);
        }
    }
}
