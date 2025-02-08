using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis;

internal abstract class UnnecessaryBlankLineAnalysis
{
    protected abstract DiagnosticDescriptor Descriptor { get; }

    public void Initialize(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(f => AnalyzeClassDeclaration(f), SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeStructDeclaration(f), SyntaxKind.StructDeclaration);
#if ROSLYN_4_0
        context.RegisterSyntaxNodeAction(f => AnalyzeRecordDeclaration(f), SyntaxKind.RecordDeclaration, SyntaxKind.RecordStructDeclaration);
#else
        context.RegisterSyntaxNodeAction(f => AnalyzeRecordDeclaration(f), SyntaxKind.RecordDeclaration);
#endif
        context.RegisterSyntaxNodeAction(f => AnalyzeInterfaceDeclaration(f), SyntaxKind.InterfaceDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeEnumDeclaration(f), SyntaxKind.EnumDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeNamespaceDeclaration(f), SyntaxKind.NamespaceDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeTryStatement(f), SyntaxKind.TryStatement);
        context.RegisterSyntaxNodeAction(f => AnalyzeElseClause(f), SyntaxKind.ElseClause);

        context.RegisterSyntaxNodeAction(f => AnalyzeIfStatement(f), SyntaxKind.IfStatement);
        context.RegisterSyntaxNodeAction(f => AnalyzeCommonForEachStatement(f), SyntaxKind.ForEachStatement);
        context.RegisterSyntaxNodeAction(f => AnalyzeCommonForEachStatement(f), SyntaxKind.ForEachVariableStatement);
        context.RegisterSyntaxNodeAction(f => AnalyzeForStatement(f), SyntaxKind.ForStatement);
        context.RegisterSyntaxNodeAction(f => AnalyzeUsingStatement(f), SyntaxKind.UsingStatement);
        context.RegisterSyntaxNodeAction(f => AnalyzeWhileStatement(f), SyntaxKind.WhileStatement);
        context.RegisterSyntaxNodeAction(f => AnalyzeDoStatement(f), SyntaxKind.DoStatement);
        context.RegisterSyntaxNodeAction(f => AnalyzeLockStatement(f), SyntaxKind.LockStatement);
        context.RegisterSyntaxNodeAction(f => AnalyzeFixedStatement(f), SyntaxKind.FixedStatement);

        context.RegisterSyntaxNodeAction(f => AnalyzeAccessorList(f), SyntaxKind.AccessorList);
        context.RegisterSyntaxNodeAction(f => AnalyzeBlock(f), SyntaxKind.Block);
        context.RegisterSyntaxNodeAction(f => AnalyzeSingleLineDocumentationCommentTrivia(f), SyntaxKind.SingleLineDocumentationCommentTrivia);

        context.RegisterSyntaxNodeAction(f => AnalyzeInitializer(f), SyntaxKind.ArrayInitializerExpression);
        context.RegisterSyntaxNodeAction(f => AnalyzeInitializer(f), SyntaxKind.CollectionInitializerExpression);
        context.RegisterSyntaxNodeAction(f => AnalyzeInitializer(f), SyntaxKind.ObjectInitializerExpression);

        context.RegisterSyntaxNodeAction(f => AnalyzeCompilationUnit(f), SyntaxKind.CompilationUnit);
    }

    private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        AnalyzeDeclaration(
            context,
            classDeclaration.Members,
            classDeclaration.OpenBraceToken,
            classDeclaration.CloseBraceToken);
    }

    private void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
    {
        var structDeclaration = (StructDeclarationSyntax)context.Node;

        AnalyzeDeclaration(
            context,
            structDeclaration.Members,
            structDeclaration.OpenBraceToken,
            structDeclaration.CloseBraceToken);
    }

    private void AnalyzeRecordDeclaration(SyntaxNodeAnalysisContext context)
    {
        var recordDeclaration = (RecordDeclarationSyntax)context.Node;

        AnalyzeDeclaration(
            context,
            recordDeclaration.Members,
            recordDeclaration.OpenBraceToken,
            recordDeclaration.CloseBraceToken);
    }

    private void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
    {
        var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

        AnalyzeDeclaration(
            context,
            interfaceDeclaration.Members,
            interfaceDeclaration.OpenBraceToken,
            interfaceDeclaration.CloseBraceToken);
    }

    private void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
    {
        var enumDeclaration = (EnumDeclarationSyntax)context.Node;

        AnalyzeDeclaration(
            context,
            enumDeclaration.Members,
            enumDeclaration.OpenBraceToken,
            enumDeclaration.CloseBraceToken);
    }

    private void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
    {
        var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

        SyntaxList<MemberDeclarationSyntax> members = namespaceDeclaration.Members;
        SyntaxList<ExternAliasDirectiveSyntax> externs = namespaceDeclaration.Externs;

        if (externs.Any())
        {
            AnalyzeStart(context, externs[0], namespaceDeclaration.OpenBraceToken);
        }
        else
        {
            SyntaxList<UsingDirectiveSyntax> usings = namespaceDeclaration.Usings;

            if (usings.Any())
            {
                AnalyzeStart(context, usings[0], namespaceDeclaration.OpenBraceToken);
            }
            else if (members.Any())
            {
                AnalyzeStart(context, members[0], namespaceDeclaration.OpenBraceToken);
            }
        }

        if (members.Any())
            AnalyzeEnd(context, members.Last(), namespaceDeclaration.CloseBraceToken);
    }

    private void AnalyzeTryStatement(SyntaxNodeAnalysisContext context)
    {
        var tryStatement = (TryStatementSyntax)context.Node;

        BlockSyntax block = tryStatement.Block;

        if (block is not null)
        {
            SyntaxList<CatchClauseSyntax> catches = tryStatement.Catches;

            if (catches.Any())
            {
                SyntaxNode previousNode = block;

                foreach (CatchClauseSyntax catchClause in catches)
                {
                    Analyze(context, previousNode, catchClause);

                    previousNode = catchClause;
                }

                FinallyClauseSyntax finallyClause = tryStatement.Finally;

                if (finallyClause is not null)
                    Analyze(context, previousNode, finallyClause);
            }
        }
    }

    private void AnalyzeElseClause(SyntaxNodeAnalysisContext context)
    {
        var elseClause = (ElseClauseSyntax)context.Node;

        SyntaxNode parent = elseClause.Parent;

        if (parent is IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = ifStatement.Statement;

            if (statement is not null)
                Analyze(context, statement, elseClause);

            statement = elseClause.Statement;

            if (statement is not null)
                Analyze(context, elseClause.ElseKeyword, statement);
        }
    }

    private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
    {
        var ifStatement = (IfStatementSyntax)context.Node;

        AnalyzeEmbeddedStatement(context, ifStatement.CloseParenToken, ifStatement.Statement);
    }

    private void AnalyzeCommonForEachStatement(SyntaxNodeAnalysisContext context)
    {
        var forEachStatement = (CommonForEachStatementSyntax)context.Node;

        AnalyzeEmbeddedStatement(context, forEachStatement.CloseParenToken, forEachStatement.Statement);
    }

    private void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
    {
        var forStatement = (ForStatementSyntax)context.Node;

        AnalyzeEmbeddedStatement(context, forStatement.CloseParenToken, forStatement.Statement);
    }

    private void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
    {
        var usingStatement = (UsingStatementSyntax)context.Node;

        AnalyzeEmbeddedStatement(context, usingStatement.CloseParenToken, usingStatement.Statement);
    }

    private void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
    {
        var whileStatement = (WhileStatementSyntax)context.Node;

        AnalyzeEmbeddedStatement(context, whileStatement.CloseParenToken, whileStatement.Statement);
    }

    private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
    {
        var doStatement = (DoStatementSyntax)context.Node;

        AnalyzeEmbeddedStatement(context, doStatement.DoKeyword, doStatement.Statement);
    }

    private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
    {
        var lockStatement = (LockStatementSyntax)context.Node;

        AnalyzeEmbeddedStatement(context, lockStatement.CloseParenToken, lockStatement.Statement);
    }

    private void AnalyzeFixedStatement(SyntaxNodeAnalysisContext context)
    {
        var fixedStatement = (FixedStatementSyntax)context.Node;

        AnalyzeEmbeddedStatement(context, fixedStatement.CloseParenToken, fixedStatement.Statement);
    }

    private void AnalyzeEmbeddedStatement(SyntaxNodeAnalysisContext context, SyntaxToken token, StatementSyntax statement)
    {
        if (statement?.IsEmbedded() == true)
            Analyze(context, token, statement);
    }

    private void Analyze(
        SyntaxNodeAnalysisContext context,
        SyntaxNode node1,
        SyntaxNode node2)
    {
        SyntaxTriviaList trailingTrivia = node1.GetTrailingTrivia();
        SyntaxTriviaList leadingTrivia = node2.GetLeadingTrivia();

        if (IsStandardTriviaBetweenLines(trailingTrivia, leadingTrivia))
            return;

        if (node1
            .SyntaxTree
            .GetLineSpan(TextSpan.FromBounds(node1.Span.End, node2.SpanStart), context.CancellationToken)
            .GetLineCount() != 3)
        {
            return;
        }

        var trivia = default(SyntaxTrivia);

        foreach (SyntaxTrivia t in leadingTrivia)
        {
            if (!t.IsWhitespaceTrivia())
            {
                trivia = t;
                break;
            }
        }

        if (!trivia.IsEndOfLineTrivia())
            return;

        if (!trailingTrivia.IsEmptyOrWhitespace())
            return;

        if (!leadingTrivia.IsEmptyOrWhitespace())
            return;

        ReportDiagnostic(
            context,
            TextSpan.FromBounds(node2.FullSpan.Start, trivia.Span.End));
    }

    private void Analyze(
        SyntaxNodeAnalysisContext context,
        SyntaxToken token,
        SyntaxNode node)
    {
        SyntaxTriviaList trailingTrivia = token.TrailingTrivia;
        SyntaxTriviaList leadingTrivia = node.GetLeadingTrivia();

        if (IsStandardTriviaBetweenLines(trailingTrivia, leadingTrivia))
            return;

        if (token
            .SyntaxTree
            .GetLineSpan(TextSpan.FromBounds(token.Span.End, node.SpanStart), context.CancellationToken)
            .GetLineCount() != 3)
        {
            return;
        }

        var trivia = default(SyntaxTrivia);

        foreach (SyntaxTrivia t in leadingTrivia)
        {
            if (!t.IsWhitespaceTrivia())
            {
                trivia = t;
                break;
            }
        }

        if (!trivia.IsEndOfLineTrivia())
            return;

        if (!trailingTrivia.IsEmptyOrWhitespace())
            return;

        if (!leadingTrivia.IsEmptyOrWhitespace())
            return;

        Location location = Location.Create(token.SyntaxTree, TextSpan.FromBounds(node.FullSpan.Start, trivia.Span.End));

        ReportDiagnostic(context, location);
    }

    private void AnalyzeDeclaration<TMember>(
        SyntaxNodeAnalysisContext context,
        SyntaxList<TMember> members,
        SyntaxToken openBrace,
        SyntaxToken closeBrace) where TMember : MemberDeclarationSyntax
    {
        if (members.Any())
        {
            AnalyzeStart(context, members[0], openBrace);
            AnalyzeEnd(context, members.Last(), closeBrace);
        }
        else
        {
            AnalyzeEmptyBraces(context, openBrace, closeBrace);
        }
    }

    private void AnalyzeDeclaration<TMember>(
        SyntaxNodeAnalysisContext context,
        SeparatedSyntaxList<TMember> members,
        SyntaxToken openBrace,
        SyntaxToken closeBrace) where TMember : MemberDeclarationSyntax
    {
        if (members.Any())
        {
            AnalyzeStart(context, members[0], openBrace);

            int count = members.SeparatorCount;

            SyntaxNodeOrToken nodeOrToken = (count == members.Count)
                ? members.GetSeparator(count - 1)
                : members.Last();

            AnalyzeEnd(context, nodeOrToken, closeBrace);
        }
        else
        {
            AnalyzeEmptyBraces(context, openBrace, closeBrace);
        }
    }

    private void AnalyzeBlock(SyntaxNodeAnalysisContext context)
    {
        var block = (BlockSyntax)context.Node;

        SyntaxList<StatementSyntax> statements = block.Statements;

        if (statements.Any())
        {
            AnalyzeStart(context, statements[0], block.OpenBraceToken);
            AnalyzeEnd(context, statements.Last(), block.CloseBraceToken);
        }
        else
        {
            AnalyzeEmptyBraces(context, block.OpenBraceToken, block.CloseBraceToken);
        }
    }

    private void AnalyzeSingleLineDocumentationCommentTrivia(SyntaxNodeAnalysisContext context)
    {
        var comment = (DocumentationCommentTriviaSyntax)context.Node;

        if (comment is IStructuredTriviaSyntax structuredTrivia)
        {
            SyntaxTrivia trivia = structuredTrivia.ParentTrivia;
            SyntaxTriviaList leadingTrivia = trivia.Token.LeadingTrivia;

            int index = leadingTrivia.IndexOf(trivia);

            if (index >= 0
                && index < leadingTrivia.Count - 1
                && leadingTrivia[index + 1].IsEndOfLineTrivia())
            {
                ReportDiagnostic(context, leadingTrivia[index + 1].GetLocation());
            }
        }
    }

    private void AnalyzeInitializer(SyntaxNodeAnalysisContext context)
    {
        var initializer = (InitializerExpressionSyntax)context.Node;

        SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

        ExpressionSyntax first = expressions.FirstOrDefault();

        if (first is null)
            return;

        if (IsExpectedTrailingTrivia(initializer.OpenBraceToken.TrailingTrivia))
        {
            SyntaxTriviaList leading = first.GetLeadingTrivia();

            if (leading.Any())
            {
                TextSpan? span = GetEmptyLineSpan(leading, isEnd: false);

                if (span is not null)
                    ReportDiagnostic(context, span.Value);
            }
        }

        if (IsExpectedTrailingTrivia(expressions.GetTrailingTrivia()))
        {
            SyntaxTriviaList leading = initializer.CloseBraceToken.LeadingTrivia;

            if (leading.Any())
            {
                TextSpan? span = GetEmptyLineSpan(leading, isEnd: true);

                if (span is not null)
                    ReportDiagnostic(context, span.Value);
            }
        }

        static bool IsExpectedTrailingTrivia(SyntaxTriviaList triviaList)
        {
            foreach (SyntaxTrivia trivia in triviaList)
            {
                switch (trivia.Kind())
                {
                    case SyntaxKind.WhitespaceTrivia:
                        break;
                    case SyntaxKind.EndOfLineTrivia:
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }
    }

    private void AnalyzeAccessorList(SyntaxNodeAnalysisContext context)
    {
        var accessorList = (AccessorListSyntax)context.Node;

        SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;

        if (accessors.Any())
        {
            AnalyzeStart(context, accessors[0], accessorList.OpenBraceToken);
            AnalyzeEnd(context, accessors.Last(), accessorList.CloseBraceToken);
        }
    }

    private void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
    {
        var compilationUnit = (CompilationUnitSyntax)context.Node;

        SyntaxTriviaList leading = compilationUnit.EndOfFileToken.LeadingTrivia;

        SyntaxTriviaList.Reversed.Enumerator en = leading.Reverse().GetEnumerator();

        if (en.MoveNext()
            && en.Current.IsEndOfLineTrivia())
        {
            int start = en.Current.SpanStart;

            if (en.MoveNext())
            {
                if (!en.Current.IsWhitespaceTrivia()
                    || en.MoveNext())
                {
                    if (CSharpFacts.IsCommentTrivia(en.Current.Kind())
                        || SyntaxFacts.IsPreprocessorDirective(en.Current.Kind()))
                    {
                        return;
                    }

                    do
                    {
                        Debug.Assert(en.Current.IsWhitespaceOrEndOfLineTrivia(), en.Current.Kind().ToString());

                        if (en.Current.IsEndOfLineTrivia())
                        {
                            start = en.Current.SpanStart;
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (en.MoveNext());
                }
            }

            ReportDiagnostic(context, TextSpan.FromBounds(start, leading.Span.End));
        }
    }

    private void AnalyzeStart(
        SyntaxNodeAnalysisContext context,
        SyntaxNode node,
        SyntaxToken brace)
    {
        if (brace.IsMissing)
            return;

        if ((node.GetSpanStartLine() - brace.GetSpanEndLine()) <= 1)
            return;

        TextSpan? span = GetEmptyLineSpan(node.GetLeadingTrivia(), isEnd: false);

        if (span is null)
            return;

        ReportDiagnostic(context, span.Value);
    }

    private void AnalyzeEnd(
        SyntaxNodeAnalysisContext context,
        SyntaxNodeOrToken nodeOrToken,
        SyntaxToken brace)
    {
        if (brace.IsMissing)
            return;

        int braceLine = brace.GetSpanStartLine();
        int nodeOrTokenLine = nodeOrToken.SyntaxTree.GetLineSpan(nodeOrToken.Span).EndLine();

        if (braceLine - nodeOrTokenLine <= 1)
            return;

        TextSpan? span = GetEmptyLineSpan(brace.LeadingTrivia, isEnd: true);

        if (span is null)
            return;

        ReportDiagnostic(context, span.Value);
    }

    private void AnalyzeEmptyBraces(
        SyntaxNodeAnalysisContext context,
        SyntaxToken openBrace,
        SyntaxToken closeBrace)
    {
        if (openBrace.IsMissing)
            return;

        if (closeBrace.IsMissing)
            return;

        SyntaxTree tree = context.Node.SyntaxTree;

        if (tree.GetLineCount(TextSpan.FromBounds(openBrace.SpanStart, closeBrace.Span.End)) <= 2)
            return;

        TextSpan? span = GetEmptyLineSpan(closeBrace.LeadingTrivia, isEnd: true);

        if (span is null)
            return;

        ReportDiagnostic(context, span.Value);
    }

    private static TextSpan? GetEmptyLineSpan(
        SyntaxTriviaList triviaList,
        bool isEnd)
    {
        SyntaxTriviaList.Enumerator en = triviaList.GetEnumerator();
        while (en.MoveNext())
        {
            switch (en.Current.Kind())
            {
                case SyntaxKind.EndOfLineTrivia:
                {
                    SyntaxTrivia endOfLine = en.Current;

                    if (isEnd)
                    {
                        while (en.MoveNext())
                        {
                            if (!en.Current.IsWhitespaceOrEndOfLineTrivia())
                                return null;
                        }
                    }

                    return TextSpan.FromBounds(triviaList.Span.Start, endOfLine.Span.End);
                }
                case SyntaxKind.WhitespaceTrivia:
                {
                    break;
                }
                default:
                {
                    return null;
                }
            }
        }

        return null;
    }

    private static bool IsStandardTriviaBetweenLines(SyntaxTriviaList trailingTrivia, SyntaxTriviaList leadingTrivia)
    {
        if (leadingTrivia.Any()
            && leadingTrivia.All(f => f.IsWhitespaceTrivia()))
        {
            SyntaxTriviaList.Enumerator en = trailingTrivia.GetEnumerator();

            while (en.MoveNext())
            {
                SyntaxKind kind = en.Current.Kind();

                if (kind == SyntaxKind.WhitespaceTrivia)
                    continue;

                return kind == SyntaxKind.EndOfLineTrivia
                    && !en.MoveNext();
            }
        }

        return false;
    }

    private void ReportDiagnostic(SyntaxNodeAnalysisContext context, TextSpan span)
    {
        ReportDiagnostic(context, Location.Create(context.Node.SyntaxTree, span));
    }

    private void ReportDiagnostic(SyntaxNodeAnalysisContext context, Location location)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            Descriptor,
            location);
    }
}
