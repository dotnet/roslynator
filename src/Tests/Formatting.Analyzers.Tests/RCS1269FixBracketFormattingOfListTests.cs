using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests;

public sealed class RCS1269FixBracketFormattingOfListTests :
    AbstractCSharpDiagnosticVerifier<FixBracketFormattingOfListAnalyzer, FixBracketFormattingOfListCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.FixBracketFormattingOfList;

    public override CSharpTestOptions Options =>
        base.Options.AddConfigOption(ConfigOptionKeys.TargetBracesStyle, "both");


    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Singleline_AlignedToParenthesis()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M[|(
                       object x, object y)|] 
                {
                }
            }
            """,
            """
            class C
            {
                void M(
                       object x, object y
                ) 
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Singleline_Unindentated()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M[|(
            object x, object y)|] 
                {
                }
            }
            """,
            """
            class C
            {
                void M(
            object x, object y
                ) 
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Singleline_Comment()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M[|( // x
                object x, object y)|] 
                {
                }
            }
            """,
            """
            class C
            {
                void M( // x
                object x, object y
                ) 
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Singleline_EmptyLine_Comment()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M[|(
            // x
            object x, object y)|] 
                {
                }
            }
            """,
            """
            class C
            {
                void M(
            // x
            object x, object y
                ) 
                {
                }
            }
            """);
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_AlignedToParenthesis()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            using System;

            class C
            {
                void M()
                {
                }
            
                [Obsolete]
                void M2[|(
                        object x,
                        object y)|] 
                {
                }
            }
            """,
            """
            using System;

            class C
            {
                void M()
                {
                }
            
                [Obsolete]
                void M2(
                        object x,
                        object y
                ) 
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_AlignedToParenthesis_WhitespaceAfterParenthesis()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M[|( object x,
                       object y)|] 
                {
                }
            }
            """,
            """
            class C
            {
                void M(
                    object x,
                       object y
                ) 
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_FirstParameterNotWrapped()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            namespace N
            {
                class C
                {
                    void M[|(object x,
                        object y)|] 
                    {
                    }
                }
            }
            """,
            """
            namespace N
            {
                class C
                {
                    void M(
                        object x,
                        object y
                    ) 
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_TwoParametersOnSameLine()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M[|(
                    object x,
                    object y,object z)|] 
                {
                }
            }
            """,
            """
            class C
            {
                void M(
                    object x,
                    object y,object z
                ) 
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_ClosingBracket_Indent_Required()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M[|(
                    object x,
                    object y, object z,
                    object p4
                    )|] 
                {
                }
            }
            """,
            """
            class C
            {
                void M(
                    object x,
                    object y, object z,
                    object p4
                ) 
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_ClosingBracket_Indent_Required2()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M[|(
                    object x,
                    object y, object z,
                    object p4
              )|] 
                {
                }
            }
            """,
            """
            class C
            {
                void M(
                    object x,
                    object y, object z,
                    object p4
                ) 
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_ClosingBracket_Indent_Required3()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M[|(
                    object x,
                    object y, object z,
                    object p4
            )|] 
                {
                }
            }
            """,
            """
            class C
            {
                void M(
                    object x,
                    object y, object z,
                    object p4
                ) 
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_ClosingBracket_Indent_Required_with_prio_comment()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M[|(
                    object x,
                    object y, object z,
                    object p4
                    // comment
                    )|] 
                {
                }
            }
            """,
            """
            class C
            {
                void M(
                    object x,
                    object y, object z,
                    object p4
                    // comment
                ) 
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_Comment()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M[|( // x
             // x
                object x, // xx
                object y)|]
                {
                }
            }
            """,
            """
            class C
            {
                void M( // x
             // x
                object x, // xx
                object y
                )
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_FirstParameterIsMultilineLambda()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            using System;

            class C
            {
                void M(Func<string, string> x, object y)
                {
                    M[|(f =>
            {
                return null;
            },
            y)|];
                }
            }
            """,
            """
            using System;

            class C
            {
                void M(Func<string, string> x, object y)
                {
                    M(
                        f =>
            {
                return null;
            },
            y
                    );
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_SecondParameterIsMultilineLambdaWithExpressionBody()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            using System;

            class C
            {
                void M(object x, Func<string, string> y)
                {
                    M[|(x, f => f
                        .ToString()
                        .ToString())|];
                }
            }
            """,
            """
            using System;

            class C
            {
                void M(object x, Func<string, string> y)
                {
                    M(
                        x, f => f
                        .ToString()
                        .ToString()
                    );
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_IndentationsDiffer_Tab()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
            	void M(object x, string y)
            	{
            		M[|(x, y.ToString()
            			.ToString())|];
            	}
            }
            """,
            """
            class C
            {
            	void M(object x, string y)
            	{
            		M(
            			x, y.ToString()
            			.ToString()
            		);
            	}
            }
            """,
            options: Options.SetConfigOption("indent_style", "tab")
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_SingleMultilineParameter_WrongIndentation()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M(string x)
                {
                    M[|((x != null)
                ? x
                : "")|];
                }
            }
            """,
            """
            class C
            {
                void M(string x)
                {
                    M(
                        (x != null)
                ? x
                : ""
                    );
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_SingleMultilineParameter_WrongIndentation2()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M(string x)
                {
                    M[|(M2[|(
                            x,
                            x)|])|];
                }
            
                string M2(string x, string y) => null;
            }
            """,
            """
            class C
            {
                void M(string x)
                {
                    M(
                        M2(
                            x,
                            x
                        )
                    );
                }
            
                string M2(string x, string y) => null;
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_SingleMultilineParameter_NoIndentation()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M(string x)
                {
                    M[|((x != null)
            ? x
            : "")|];
                }
            }
            """,
            """
            class C
            {
                void M(string x)
                {
                    M(
                        (x != null)
            ? x
            : ""
                    );
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_BaseConstructorArguments()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C : B
            {
                public C(string x, string y) : base[|(x, (y != null)
                    ? y
                    : "")|]
                {
                }
            }

            class B
            {
                public B(string x, string y)
                {
                }
            }
            """,
            """
            class C : B
            {
                public C(string x, string y) : base(
                    x, (y != null)
                    ? y
                    : ""
                )
                {
                }
            }

            class B
            {
                public B(string x, string y)
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_BaseConstructorArguments2()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C : B
            {
                public C(string x, string y)
                    : base[|(x, (y != null)
                        ? y
                        : "")|]
                {
                }
            }

            class B
            {
                public B(string x, string y)
                {
                }
            }
            """,
            """
            class C : B
            {
                public C(string x, string y)
                    : base(
                        x, (y != null)
                        ? y
                        : ""
                    )
                {
                }
            }

            class B
            {
                public B(string x, string y)
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_StringLiteral()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M(string x, string y)
                {
                    M[|(x, @"
            a
            b
            c
            ")|];
                }
            }
            """,
            """
            class C
            {
                void M(string x, string y)
                {
                    M(
                        x, @"
            a
            b
            c
            "
                    );
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_AttributeList()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            using System;

            class C
            {
                [|[Flags, Obsolete[|(
                    "",
                    true)|]]|]
                enum E
                {
                }
            }
            """,
            """
            using System;

            class C
            {
                [
                    Flags, Obsolete(
                    "",
                    true
                    )
                ]
                enum E
                {
                }
            }
            """);
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_AttributeList_NotIndented()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            using System;

            class C
            {
                [|[Flags, Obsolete[|(
            "",
            true)|]]|]
                enum E
                {
                }
            }
            """,
            """
            using System;

            class C
            {
                [
                    Flags, Obsolete(
            "",
            true
                    )
                ]
                enum E
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_OneOfParametersIsMultiline()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M[|(string x, string y
                    = default(string), string z = default)|]
                {
                }
            }
            """,
            """
            class C
            {
                void M(
                    string x, string y
                    = default(string), string z = default
                )
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_TupleType()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                private [|(string x, string y,
                    string z)|] M[|([|(string a,
                        string b)|] p)|]
                {
                    return [|(null, null,
                        null)|];
                }
            }
            """,
            """
            class C
            {
                private (
                    string x, string y,
                    string z
                ) M(
                    (
                        string a,
                        string b
                    ) p
                )
                {
                    return (
                        null, null,
                        null
                    );
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_TupleExpression()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M[|([|(string x, string y,
                    string z)|] p)|]
                {
                }
            }
            """,
            """
            class C
            {
                void M(
                    (
                        string x, string y,
                    string z
                    ) p
                )
                {
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_TupleExpression_LocalDeclaration()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    [|(string x,
                        string y, string z)|] = default((string, string, string));
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    (
                        string x,
                        string y, string z
                    ) = default((string, string, string));
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_ArrayInitializer()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    var x = new string[] [|{ "", default, new string[|(
                        ' ',
                        1)|] }|];
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    var x = new string[] {
                        "", default, new string(
                        ' ',
                        1
                        ) 
                    };
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_PreprocessorDirectives()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            #define FOO
            using System.Collections.Generic;

            class C
            {
                void M()
                {
                    var x = new List<string>[|(new string[]
                        {
                            "",
                            "",
            #if FOO
                            "",
            #endif
                        })|];
                }
            }
            """,
            """
            #define FOO
            using System.Collections.Generic;

            class C
            {
                void M()
                {
                    var x = new List<string>(
                        new string[]
                        {
                            "",
                            "",
            #if FOO
                            "",
            #endif
                        }
                    );
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_SingleParameterIsMultilineLambdaWithExpressionBody()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            using System.Linq;

            class C
            {
                void M()
                {
                    string s = string.Concat[|(Enumerable.Empty<string>().Select[|(f => {
                        return f;
                    })|])|];
                }
            }
            """,
            """
            using System.Linq;

            class C
            {
                void M()
                {
                    string s = string.Concat(
                        Enumerable.Empty<string>().Select(
                            f => {
                        return f;
                    }
                        )
                    );
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task TestNoDiagnostic_Singleline()
    {
        await VerifyNoDiagnosticAsync(
            """
            class C
            {
                void M(object x, object y, object z) 
                {
                }

                void M2(object x, object y, object z) 
                {
                }
            }
            """);
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task TestNoDiagnostic_ArrayInitializerWithMultilineComment()
    {
        await VerifyNoDiagnosticAsync(
            """
            class C
            {
                void M()
                {
                    var x = new string[]
                    {
                        /* x */ ""
                    };
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_LambdaBlockBodyInGlobalStatement()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            using System.Linq;

            foreach (var item in Enumerable.Range(0, 10))
            {
                var s = "";
            }

            var items = Enumerable.Range(0, 10)
                .Select[|(f =>
                {
                    return f;
                })|]
                .Select(f => f);
            """,
            """
            using System.Linq;

            foreach (var item in Enumerable.Range(0, 10))
            {
                var s = "";
            }

            var items = Enumerable.Range(0, 10)
                .Select(
                    f =>
                {
                    return f;
                }
                )
                .Select(f => f);
            """,
            options: Options.WithCompilationOptions(Options.CompilationOptions.WithOutputKind(OutputKind.ConsoleApplication))
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_ArrayInitializerInGlobalStatement()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            Foo.Method[|(
                foo: new Foo[]
                {
                    new Foo[|(
                        "")|]
                })|];
            """,
            """
            Foo.Method(
                foo: new Foo[]
                {
                    new Foo(
                        ""
                    )
                }
            );
            """,
            additionalFiles:
                new (string source, string expectedSource)[]
                {
                    (
                        source:
                            """
                            public class Foo
                            {
                                public Foo(string v1)
                                {
                                }
                            
                                internal static void Method(Foo[] foo)
                                {
                                }
                            }
                            """,
                        expectedSource: null
                    )
                },
            options: Options.WithCompilationOptions(Options.CompilationOptions.WithOutputKind(OutputKind.ConsoleApplication))
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_ObjectInitializer()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                public C() { }

                public C(C value) { }
            
                public string P { get; set; }

                C M()
                {
                    return new C[|(new C
                    {
                        P = ""
                    })|];
                }
            }
            """,
            """
            class C
            {
                public C() { }

                public C(C value) { }
            
                public string P { get; set; }

                C M()
                {
                    return new C(
                        new C
                    {
                        P = ""
                    }
                    );
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_CollectionExpression()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                public C P { get; set; }

                public string M1(string[] values)
                {
                    string x =
                        P
                            .M1[|([|[null,
                                // x
                                null,
                            ]|])|]
                            .ToString();

                    return x;
                }

                public string M2(string value, string[] values)
                {
                    string x =
                        P
                            .M2[|(
                                "", [|[ null,
                                    // y
                                    null,]|])|]
                                .ToString();

                    return x;
                }
            }
            """,
            """
            class C
            {
                public C P { get; set; }

                public string M1(string[] values)
                {
                    string x =
                        P
                            .M1(
                                [
                                    null,
                                // x
                                null,
                                ]
                            )
                            .ToString();

                    return x;
                }

                public string M2(string value, string[] values)
                {
                    string x =
                        P
                            .M2(
                                "", [
                                    null,
                                    // y
                                    null,
                                ]
                            )
                                .ToString();

                    return x;
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfList)]
    public async Task Test_Multiline_SwitchExpression()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            using System;

            class C
            {
                string M(string value) =>
                    M[|(value switch
                    {
                        "a" => "a",
                        "b" => "b",
                        _ => throw new Exception()
                    })|];
            }
            """,
            """
            using System;

            class C
            {
                string M(string value) =>
                    M(
                        value switch
                    {
                        "a" => "a",
                        "b" => "b",
                        _ => throw new Exception()
                    }
                    );
            }
            """
        );
    }
}
