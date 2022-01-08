// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CommandLine
{
    internal static class AssemblyFactory
    {
        public static Assembly FromExpression(
            string expressionText,
            string className,
            string methodName,
            string returnTypeName,
            string parameterType,
            string parameterName)
        {
            ExpressionSyntax expression = ParseExpression(expressionText);

            CompilationUnitSyntax compilationUnit = CompilationUnit(
                default,
                default,
                default,
                SingletonList<MemberDeclarationSyntax>(
                    NamespaceDeclaration(
                        ParseName("Roslynator.Runtime"),
                        default,
                        List(new UsingDirectiveSyntax[]
                            {
                                UsingDirective(ParseName("System")),
                                UsingDirective(ParseName("System.Collections.Generic")),
                                UsingDirective(ParseName("System.Linq")),
                                UsingDirective(ParseName("System.Text")),
                                UsingDirective(ParseName("System.Text.RegularExpressions")),
                                UsingDirective(ParseName("Microsoft.CodeAnalysis")),
                                UsingDirective(ParseName("Microsoft.CodeAnalysis.CSharp")),
#if DEBUG
                                UsingDirective(ParseName("Roslynator")),
                                UsingDirective(ParseName("Roslynator.CSharp")),
#endif
                            }),
                        SingletonList<MemberDeclarationSyntax>(
                            ClassDeclaration(
                                default,
                                TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)),
                                Identifier(className),
                                default,
                                default,
                                default,
                                SingletonList<MemberDeclarationSyntax>(
                                    MethodDeclaration(
                                        default,
                                        TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)),
                                        ParseTypeName(returnTypeName),
                                        default,
                                        Identifier(methodName),
                                        default,
                                        ParameterList(
                                            SingletonSeparatedList(
                                                Parameter(
                                                    default,
                                                    default,
                                                    ParseTypeName(parameterType),
                                                    Identifier(parameterName),
                                                    default))),
                                        default,
                                        default,
                                        ArrowExpressionClause(expression),
                                        Token(SyntaxKind.SemicolonToken))))))));

            SyntaxTree syntaxTree = CSharpSyntaxTree.Create(compilationUnit);

            var namespaceDeclaration = (NamespaceDeclarationSyntax)compilationUnit.Members[0];
            var classDeclaration = (ClassDeclarationSyntax)namespaceDeclaration.Members[0];
            var methodDeclaration = (MethodDeclarationSyntax)classDeclaration.Members[0];

            expression = methodDeclaration.ExpressionBody!.Expression;

            return FromSyntaxTree(syntaxTree, expressionSpan: expression.Span);
        }

        public static Assembly FromSourceText(string text)
        {
            return FromSyntaxTree(CSharpSyntaxTree.ParseText(text));
        }

        public static Assembly FromSyntaxTree(SyntaxTree syntaxTree, TextSpan? expressionSpan = null)
        {
            string assemblyName = Path.GetRandomFileName();

            ImmutableArray<MetadataReference> metadataReferences = RuntimeMetadataReference.DefaultMetadataReferences;

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new SyntaxTree[] { syntaxTree },
                references: metadataReferences,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics();

            if (expressionSpan != null)
            {
                foreach (Diagnostic diagnostic in diagnostics)
                {
                    if (diagnostic.Severity == DiagnosticSeverity.Error
                        && diagnostic.Location.Kind == LocationKind.SourceFile
                        && !expressionSpan.Value.Contains(diagnostic.Location.SourceSpan))
                    {
                        expressionSpan = null;
                        break;
                    }
                }
            }

            var hasDiagnostic = false;

            foreach (Diagnostic diagnostic in diagnostics.OrderBy(f => f.Location.SourceSpan))
            {
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                {
                    WriteDiagnostic(diagnostic, expressionSpan);
                    hasDiagnostic = true;
                }
            }

            if (hasDiagnostic)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                EmitResult emitResult = compilation.Emit(memoryStream);

                if (!emitResult.Success)
                {
                    foreach (Diagnostic diagnostic in emitResult.Diagnostics.OrderBy(f => f.Location.SourceSpan))
                        WriteDiagnostic(diagnostic, expressionSpan);

                    return null;
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(memoryStream.ToArray());
            }
        }

        private static void WriteDiagnostic(Diagnostic diagnostic, TextSpan? expressionSpan = null)
        {
            string location;
            if (expressionSpan != null)
            {
                location = $"1,{diagnostic.Location.SourceSpan.Start - expressionSpan.Value.Start + 1}";
            }
            else
            {
                LinePosition start = diagnostic.Location.GetLineSpan().Span.Start;
                location = $"{start.Line + 1},{start.Character + 1}";
            }

            Logger.WriteLine($"({location}): {diagnostic.Id}: {diagnostic.GetMessage()}", Verbosity.Minimal);
        }
    }
}
