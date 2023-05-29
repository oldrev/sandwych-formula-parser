using Parlot.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Parlot;
using static Parlot.Fluent.Parsers;
using Sandwych.FormulaParser.Model;

namespace Sandwych.FormulaParser;


public static class Tokens
{

    ////////////////////////////////////////////////////////// Literals ///////////////////////////////////////////////

    private static readonly Parser<TextSpan> OneToThreeLettersLiteral = Literals.Pattern(char.IsLetter, 1, 3);

    // /[1-9]/
    private static readonly Parser<TextSpan> GreaterOrEqualOneIntLiteral = Capture(
        Literals.Pattern(x => char.IsDigit(x) && x != '0', 1, 1).And(Literals.Pattern(char.IsDigit, 0, 0)));

    private static readonly Parser<char> OptionalDollarLiteral = ZeroOrOne(Literals.Char('$'));


    private static readonly Parser<TextSpan> ColAbsRelLiteral = Capture(Literals.Char('$').And(OneToThreeLettersLiteral));
    private static readonly Parser<TextSpan> ColRelRefLiteral = OneToThreeLettersLiteral;

    // /[$]?[A-Za-z]{1,3}/
    public static readonly Parser<TextSpan> ColRefLiteral = Capture(ColAbsRelLiteral.Or(ColRelRefLiteral));

    // pattern: /[$]?[1-9][0-9]*/
    public static readonly Parser<TextSpan> RowRefLiteral = Capture(OptionalDollarLiteral.And(GreaterOrEqualOneIntLiteral));

    // pattern: /[$]?[A-Za-z]{1,3}[$]?[1-9][0-9]*/,
    public static readonly Parser<(TextSpan, TextSpan)> CellRefLiteral = ColRefLiteral.And(RowRefLiteral).Then(x => x);

    //////////////////////////////////////////////////////////// Tokens ////////////////////////////////////////////////////////////

    public static readonly Parser<TextSpan> IdentifierToken = Terms.Identifier();

    public static readonly Parser<string> StringLiteralToken = Terms.String(StringLiteralQuotes.SingleOrDouble).Then(x => x.ToString());

    public static readonly Parser<bool> BooleanLiteralToken
        = SkipWhiteSpace(Literals.Text("TRUE", caseInsensitive: true).Then(x => true)
            .Or(Literals.Text("FALSE", caseInsensitive: true).Then(x => false)));

    public static readonly Parser<double> NumberLiteralToken
        = Terms.Decimal(NumberOptions.None).Then(x => (double)x);

    // pattern: /[A-Za-z_]+[A-Za-z_0-9.]*\(/
    public static readonly Parser<TextSpan> FunctionCallToken = SkipWhiteSpace(Capture(IdentifierToken.And(Literals.Char('('))));

    public static readonly Parser<CellRef> CellRefToken = SkipWhiteSpace(CellRefLiteral)
        .Then(x => CellRef.From(x.Item1.Span, x.Item2.Span));

}
