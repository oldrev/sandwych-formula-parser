using Parlot;
using Parlot.Fluent;
using Sandwych.FormulaParser.Model;
using System.Linq.Expressions;
using System.Reflection;
using static Parlot.Fluent.Parsers;
using LE = System.Linq.Expressions;

namespace Sandwych.FormulaParser;

public class FormulaCompiler {

    static readonly Type Scope_Type = typeof(Scope);
    static readonly MethodInfo Scope_Get = Scope_Type.GetMethod(nameof(Scope.Get)) ?? throw new NullReferenceException();
    static readonly MethodInfo Scope_GetCellValue = Scope_Type.GetMethod(nameof(Scope.GetCellValue)) ?? throw new NullReferenceException();
    // static readonly MethodInfo Scope_SetLocal = Scope_Type.GetMethod(nameof(Scope.SetLocal)) ?? throw new NullReferenceException();
    // static readonly MethodInfo Scope_SetGlobal = Scope_Type.GetMethod(nameof(Scope.SetGlobal)) ?? throw new NullReferenceException();
    // static readonly MethodInfo Scope_Set = Scope_Type.GetMethod(nameof(Scope.Set)) ?? throw new NullReferenceException();
    // static readonly ConstructorInfo Scope_New_parent = Scope_Type.GetConstructor(new[] { Scope_Type }) ?? throw new NullReferenceException();
    static readonly LE.ParameterExpression ScopeParameterExpression = LE.Expression.Parameter(Scope_Type, "__scope__");




    private readonly Parser<LE.Expression> s_expressionParser;

    public IReadOnlyDictionary<string, LE.Expression> Functions => _functions;

    readonly Dictionary<string, LE.Expression> _functions = new Dictionary<string, LE.Expression>();

    private static LE.Expression MakeScopeGetCallExpr(ReadOnlySpan<char> varName) {
        var getCallExpr = LE.Expression.Call(ScopeParameterExpression, Scope_Get, LE.Expression.Constant(varName.ToString()));
        return LE.Expression.Convert(getCallExpr, typeof(double));
    }

    private LE.Expression MakeUserFunctionCallExpr(ReadOnlySpan<char> funcName, IEnumerable<LE.Expression> args) {
        var funcExpr = Functions[funcName.ToString()];
        var argsExpr = LE.Expression.NewArrayInit(typeof(double), args);
        return LE.Expression.Invoke(funcExpr, argsExpr);
    }

    private static LE.Expression MakeGetCellValueFunctionCallExpr(CellRef cellRef) {
        var cellRefExpr = LE.Expression.Constant(cellRef);
        var getCellValueCallExpr = LE.Expression.Call(ScopeParameterExpression, Scope_GetCellValue, cellRefExpr);
        return getCellValueCallExpr;
    }



    public FormulaCompiler(IEnumerable<(string, LE.Expression)>? functions = null) {
        if (functions != null)
        {
            foreach (var f in functions)
            {
                _functions.Add(f.Item1, f.Item2);
            }
        }


        /*
         * Grammar:
         * expression     => factor ( ( "-" | "+" ) factor )* ;
         * factor         => power ( ( "/" | "*" ) power )* ;
         * power         => unary ( "^" unary )* ;
         * unary          => ( "-" ) unary
         *                 | primary ;
         * primary        => NUMBER
         *                  | "(" expression ")" ;
        */

        // The Deferred helper creates a parser that can be referenced by others before it is defined
        var formulaExpression = Deferred<LE.Expression>();

        var numberLiteral = Terms.Decimal()
            .Then<LE.Expression>(static d => LE.Expression.Constant((double)d))
            ;

        var slashToken = Terms.Char('/');
        var starToken = Terms.Char('*');
        var minusToken = Terms.Char('-');
        var plusToken = Terms.Char('+');
        var leftParenToken = Terms.Char('(');
        var rightParenToken = Terms.Char(')');
        var commaToken = Terms.Char(',');
        var caretToken = Terms.Char('^');
        var powerToken = Terms.Text("**");

        var excelFunctionCallStartToken = SkipWhiteSpace(Literals.Identifier().And(Literals.Char('(')));
        var excelFunctionCallEndToken = Terms.Char(')');

        Parser<LE.Expression> excelFunctionCallExpression =
            excelFunctionCallStartToken.And(Separated(commaToken, formulaExpression)).And(excelFunctionCallEndToken)
            .Then(x => this.MakeUserFunctionCallExpr(x.Item1.ToString(), x.Item3));

        // "(" expression ")"
        var groupExpression = Between(leftParenToken, formulaExpression, rightParenToken);

        Parser<LE.Expression> cellRef = Tokens.CellRefToken
            .Then(static x => MakeGetCellValueFunctionCallExpr(x));

        // primary => NUMBER | (CellRef | excelFunctionCallExpression) | "(" expression ")";
        var primaryExpression = OneOf(numberLiteral, cellRef.Or(excelFunctionCallExpression), groupExpression);

        // The Recursive helper allows to create parsers that depend on themselves.
        // ( "-" ) unary | primary;
        var unaryExpression = Recursive<LE.Expression>((u) =>
            minusToken.And(u)
                .Then<LE.Expression>(static x => LE.Expression.Negate(x.Item2))
                .Or(primaryExpression));

        // power => unary ( "^" unary )* ;
        var powerExpression = unaryExpression.And(ZeroOrMany(caretToken.And(unaryExpression)))
            .Then(static x =>
            {
                // unary
                var result = x.Item1;

                // (("/" | "*") unary ) *
                foreach (var op in x.Item2)
                {
                    result = op.Item1 switch
                    {
                        '^' => LE.Expression.MakeBinary(LE.ExpressionType.Power, result, op.Item2),
                        _ => null
                    };
                }

                return result;
            });

        // factor => unary ( ( "/" | "*" ) unary )* ;
        var factorExpression = powerExpression.And(ZeroOrMany(slashToken.Or(starToken).And(powerExpression)))
            .Then(static x =>
            {
                // unary
                var result = x.Item1;

                // (("/" | "*") unary ) *
                foreach (var op in x.Item2)
                {
                    result = op.Item1 switch
                    {
                        '/' => LE.Expression.MakeBinary(LE.ExpressionType.Divide, result, op.Item2),
                        '*' => LE.Expression.MakeBinary(LE.ExpressionType.Multiply, result, op.Item2),
                        _ => null
                    };
                }

                return result;
            });

        // arithmeticExpression => factor ( ( "-" | "+" ) factor )* ;
        formulaExpression.Parser = factorExpression.And(ZeroOrMany(plusToken.Or(minusToken).And(factorExpression)))
            .Then(static x =>
            {
                // factor
                var result = x.Item1;

                // (("-" | "+") factor ) *
                foreach (var op in x.Item2)
                {
                    result = op.Item1 switch
                    {
                        '+' => LE.Expression.MakeBinary(LE.ExpressionType.Add, result, op.Item2),
                        '-' => LE.Expression.MakeBinary(LE.ExpressionType.Subtract, result, op.Item2),
                        _ => null
                    };
                }

                return result;
            });

        var expression = formulaExpression;

        s_expressionParser = expression;
    }

    public Func<Scope, double> Compile(string exprText) {
        // var assignScope = Expression.Assign(scopeVar, Expression.New(LuaContext_New_parent, Context));
        var bodyExpr = s_expressionParser.Parse(exprText);

        var blockExpr = LE.Expression.Block(typeof(double), bodyExpr);

        // 创建 lambda 表达式
        var lambdaExpr = LE.Expression.Lambda<Func<Scope, double>>(blockExpr, ScopeParameterExpression);
        return lambdaExpr.Compile();
    }


}

