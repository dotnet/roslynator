// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal readonly struct IndentationAnalysis
{
    private readonly int? _indentSize;
    private readonly SyntaxTrivia? _singleIndentation;

    private IndentationAnalysis(SyntaxTrivia indentation, int? indentSize, SyntaxTrivia? singleIndentation)
    {
        Indentation = indentation;
        _indentSize = indentSize;
        _singleIndentation = singleIndentation;
    }

    public SyntaxTrivia Indentation { get; }

    public int IndentSize => _indentSize ?? _singleIndentation?.Span.Length ?? 0;

    public int IndentationLength => Indentation.Span.Length;

    public int IncreasedIndentationLength => (IndentSize > 0) ? Indentation.Span.Length + IndentSize : 0;

    public bool IsDefault
    {
        get
        {
            return Indentation.IsKind(SyntaxKind.None)
                && _indentSize is null
                && _singleIndentation is null;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"Length = {Indentation.Span.Length} {nameof(IndentSize)} = {IndentSize}";

    public static IndentationAnalysis Create(SyntaxNode node, CancellationToken cancellationToken = default)
    {
        SyntaxTrivia indentation = SyntaxTriviaAnalysis.DetermineIndentation(node, cancellationToken);

        (SyntaxTrivia trivia1, SyntaxTrivia trivia2, bool isFromCompilationUnit) = DetermineSingleIndentation(node, cancellationToken);

        if (isFromCompilationUnit)
        {
            return new IndentationAnalysis(indentation, trivia1.Span.Length - trivia2.Span.Length, null);
        }
        else if (indentation.Span.Length > 0)
        {
            return (trivia1.Span.Length > 0)
                ? new IndentationAnalysis(indentation, null, trivia1)
                : new IndentationAnalysis(indentation, null, null);
        }
        else if (trivia1.Span.Length > 0)
        {
            return new IndentationAnalysis(indentation, null, trivia1);
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
        if (_singleIndentation is not null)
            return _singleIndentation.ToString();

        if (_indentSize == -1)
            return Indentation.ToString();

        if (Indentation.Span.Length == 0)
            return "";

        string indentation = Indentation.ToString();

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
        }
        while (node is not null);

        return default;

        SyntaxTrivia DetermineIndentationSize(CompilationUnitSyntax compilationUnit)
        {
            foreach (MemberDeclarationSyntax member in compilationUnit.Members)
            {
                if (member is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    MemberDeclarationSyntax member2 = namespaceDeclaration.Members.FirstOrDefault();

                    if (member2 is not null)
                        return SyntaxTriviaAnalysis.DetermineIndentation(member2, cancellationToken);
                }
                else if (member is TypeDeclarationSyntax typeDeclaration)
                {
                    MemberDeclarationSyntax member2 = typeDeclaration.Members.FirstOrDefault();

                    if (member2 is not null)
                        return SyntaxTriviaAnalysis.DetermineIndentation(member2, cancellationToken);
                }
                else if (member is GlobalStatementSyntax globalStatement)
                {
                    StatementSyntax statement2 = globalStatement.Statement;

                    if (statement2 is SwitchStatementSyntax switchStatement)
                    {
                        SwitchSectionSyntax switchSection = switchStatement.Sections.FirstOrDefault();

                        if (switchSection is not null)
                            return SyntaxTriviaAnalysis.DetermineIndentation(switchSection, cancellationToken);

                        break;
                    }
                    else
                    {
                        StatementSyntax statement3 = GetContainedStatement(statement2);

                        if (statement3 is not null)
                        {
                            if (statement3 is BlockSyntax block)
                                statement3 = block.Statements.FirstOrDefault();

                            if (statement3 is not null)
                                return SyntaxTriviaAnalysis.DetermineIndentation(statement3, cancellationToken);
                        }
                    }
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

        StatementSyntax GetContainedStatement(StatementSyntax statement)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.WhileStatement:
                    return ((WhileStatementSyntax)statement).Statement;

                case SyntaxKind.DoStatement:
                    return ((DoStatementSyntax)statement).Statement;

                case SyntaxKind.ForStatement:
                    return ((ForStatementSyntax)statement).Statement;

                case SyntaxKind.ForEachStatement:
                case SyntaxKind.ForEachVariableStatement:
                    return ((CommonForEachStatementSyntax)statement).Statement;

                case SyntaxKind.UsingStatement:
                    return ((UsingStatementSyntax)statement).Statement;

                case SyntaxKind.FixedStatement:
                    return ((FixedStatementSyntax)statement).Statement;

                case SyntaxKind.CheckedStatement:
                case SyntaxKind.UncheckedStatement:
                    return ((CheckedStatementSyntax)statement).Block;

                case SyntaxKind.UnsafeStatement:
                    return ((UnsafeStatementSyntax)statement).Block;

                case SyntaxKind.LockStatement:
                    return ((LockStatementSyntax)statement).Statement;

                case SyntaxKind.IfStatement:
                    return ((IfStatementSyntax)statement).Statement;

                case SyntaxKind.TryStatement:
                    return ((TryStatementSyntax)statement).Block;

                default:
                    return null;
            }
        }
    }
}
