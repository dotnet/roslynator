// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct IndentationAnalysis
    {
        private readonly SyntaxTrivia _indentation;
        private readonly int _indentSize;

        private IndentationAnalysis(SyntaxTrivia indentation, int indentSize)
        {
            _indentation = indentation;
            _indentSize = indentSize;
        }

        public SyntaxTrivia Indentation => (_indentSize == -1) ? default : _indentation;

        public int IndentSize => (_indentSize == -1) ? _indentation.Span.Length : _indentSize;

        public int IndentationLength => (_indentSize == -1) ? 0 : Indentation.Span.Length;

        public int IncreasedIndentationLength => (_indentSize == -1) ? _indentation.Span.Length : (Indentation.Span.Length + _indentSize);

        public bool IsDefault => _indentation == default && _indentSize == 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"Length = {Indentation.Span.Length} {nameof(IndentSize)} = {IndentSize}";

        public static IndentationAnalysis Create(SyntaxNode node, CancellationToken cancellationToken = default)
        {
            SyntaxTrivia indentation = SyntaxTriviaAnalysis.DetermineIndentation(node, cancellationToken);

            (SyntaxTrivia trivia1, SyntaxTrivia trivia2, bool isFromCompilationUnit) = DetermineSingleIndentation(node, cancellationToken);

            if (isFromCompilationUnit)
            {
                return new IndentationAnalysis(indentation, trivia1.Span.Length - trivia2.Span.Length);
            }
            else if (trivia1.Span.Length > 0)
            {
                return new IndentationAnalysis(trivia1, -1);
            }
            else
            {
                return default;
            }
        }

        public string GetIncreasedIndentation()
        {
            string singleIndentation = GetSingleIndentation();

            return Indentation.ToString() + singleIndentation;
        }

        public SyntaxTrivia GetIncreasedIndentationTrivia()
        {
            return SyntaxFactory.Whitespace(GetIncreasedIndentation());
        }

        public SyntaxTriviaList GetIncreasedIndentationTriviaList()
        {
            return SyntaxFactory.TriviaList(GetIncreasedIndentationTrivia());
        }

        public string GetSingleIndentation()
        {
            if (_indentSize == -1)
                return _indentation.ToString();

            if (_indentation.Span.Length == 0)
                return "";

            string indentation = _indentation.ToString();

            if (indentation[indentation.Length - 1] == '\t')
                return "\t";

            return new string(indentation[0], IndentSize);
        }

        private static (SyntaxTrivia, SyntaxTrivia, bool isFromCompilationUnit) DetermineSingleIndentation(SyntaxNode node, CancellationToken cancellationToken = default)
        {
            do
            {
                switch (node)
                {
                    case MemberDeclarationSyntax member:
                        {
                            switch (node.Parent)
                            {
                                case NamespaceDeclarationSyntax @namespace:
                                    {
                                        (SyntaxTrivia trivia1, SyntaxTrivia trivia2) = GetIndentationSize(member, @namespace.CloseBraceToken);

                                        if (trivia1.Span.Length > 0)
                                            return (trivia1, trivia2, true);

                                        break;
                                    }
                                case BaseTypeDeclarationSyntax baseType:
                                    {
                                        (SyntaxTrivia trivia1, SyntaxTrivia trivia2) = GetIndentationSize(member, baseType.CloseBraceToken);

                                        if (trivia1.Span.Length > 0)
                                            return (trivia1, trivia2, true);

                                        break;
                                    }
                                case CompilationUnitSyntax compilationUnit:
                                    {
                                        SyntaxTrivia trivia = DetermineIndentationSize(compilationUnit);
                                        return (trivia, default, false);
                                    }
                                default:
                                    {
                                        return default;
                                    }
                            }

                            break;
                        }
                    case AccessorDeclarationSyntax accessor:
                        {
                            switch (node.Parent)
                            {
                                case AccessorListSyntax accessorList:
                                    {
                                        (SyntaxTrivia trivia1, SyntaxTrivia trivia2) = GetIndentationSize(accessor, accessorList.CloseBraceToken);

                                        if (trivia1.Span.Length > 0)
                                            return (trivia1, trivia2, true);

                                        break;
                                    }
                                default:
                                    {
                                        return default;
                                    }
                            }

                            break;
                        }
                    case BlockSyntax _:
                        {
                            break;
                        }
                    case StatementSyntax statement:
                        {
                            switch (node.Parent)
                            {
                                case SwitchSectionSyntax switchSection:
                                    {
                                        (SyntaxTrivia trivia1, SyntaxTrivia trivia2) = GetIndentationSize(statement, switchSection);

                                        if (trivia1.Span.Length > 0)
                                            return (trivia1, trivia2, true);

                                        break;
                                    }
                                case BlockSyntax block:
                                    {
                                        (SyntaxTrivia trivia1, SyntaxTrivia trivia2) = GetIndentationSize(statement, block.CloseBraceToken);

                                        if (trivia1.Span.Length > 0)
                                            return (trivia1, trivia2, true);

                                        break;
                                    }
                                case StatementSyntax statement2:
                                    {
                                        (SyntaxTrivia trivia1, SyntaxTrivia trivia2) = GetIndentationSize(statement, statement2);

                                        if (trivia1.Span.Length > 0)
                                            return (trivia1, trivia2, true);

                                        break;
                                    }
                                case ElseClauseSyntax elseClause:
                                    {
                                        (SyntaxTrivia trivia1, SyntaxTrivia trivia2) = GetIndentationSize(statement, elseClause);

                                        if (trivia1.Span.Length > 0)
                                            return (trivia1, trivia2, true);

                                        break;
                                    }
                                case GlobalStatementSyntax:
                                    {
                                        break;
                                    }
                                default:
                                    {
                                        return default;
                                    }
                            }

                            break;
                        }
                    case CompilationUnitSyntax compilationUnit:
                        {
                            SyntaxTrivia trivia = DetermineIndentationSize(compilationUnit);
                            return (trivia, default, false);
                        }
                }

                node = node.Parent;

            } while (node != null);

            return default;

            SyntaxTrivia DetermineIndentationSize(CompilationUnitSyntax compilationUnit)
            {
                foreach (MemberDeclarationSyntax member in compilationUnit.Members)
                {
                    if (member is NamespaceDeclarationSyntax namespaceDeclaration)
                    {
                        MemberDeclarationSyntax member2 = namespaceDeclaration.Members.FirstOrDefault();

                        if (member2 != null)
                            return SyntaxTriviaAnalysis.DetermineIndentation(member2, cancellationToken);
                    }
                    else if (member is TypeDeclarationSyntax typeDeclaration)
                    {
                        MemberDeclarationSyntax member2 = typeDeclaration.Members.FirstOrDefault();

                        if (member2 != null)
                            return SyntaxTriviaAnalysis.DetermineIndentation(member2, cancellationToken);
                    }
                }

                return default;
            }

            (SyntaxTrivia, SyntaxTrivia) GetIndentationSize(SyntaxNodeOrToken nodeOrToken1, SyntaxNodeOrToken nodeOrToken2)
            {
                SyntaxTrivia indentation1 = SyntaxTriviaAnalysis.DetermineIndentation(nodeOrToken1, cancellationToken);

                int length1 = indentation1.Span.Length;

                if (length1 > 0)
                {
                    SyntaxTrivia indentation2 = SyntaxTriviaAnalysis.DetermineIndentation(nodeOrToken2, cancellationToken);

                    int length2 = indentation2.Span.Length;

                    if (length1 > length2)
                    {
                        return (indentation1, indentation2);
                    }
                }

                return default;
            }
        }
    }
}
