
using Sandwych.FormulaParser.Model;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Sandwych.FormulaParser.DemoApp;

class Program {

    static async Task Main() {

        CellValue cv = new CellValue();
        cv.BooleanValue = true;

        var x = Marshal.SizeOf<CellValue>();
        Console.WriteLine($"struct size: {x}");


        var scope = new Scope();
        scope.Set("$A$12", 10.0);
        scope.Set("C13", 100.0);

       // var exprText = "ADD(0, 1) + 2 * (3 + 1) + A12 * 2 * MIN(0, 2) + SUM(1,2,3,4,5)";
        var exprText = "ADD(SUM(1.0, 2.0, $A$12), 100.0) + C13";

        // 自定义函数
        var customFunctions = new (string, Expression)[]
        {
            ("ADD", (IEnumerable<double> args) => args.Sum()),
            ("SUM", (IEnumerable<double> args) => args.Aggregate((x, y) => x + y)),
            ("MAX", (IEnumerable<double> args) => args.Max()),
            ("MIN", (IEnumerable<double> args) => args.Min()),
        };

        var compiler = new FormulaCompiler(customFunctions);

        // 编译成 .NET IL 代码，并返回成委托
        var dg = compiler.Compile(exprText);

        // 调用编译后的委托
        var result = dg(scope);

        // 输出结果
        Console.WriteLine($"Cell $A$12 = {scope.Get("$A$12")}");
        Console.WriteLine($"Cell C13 = {scope.Get("C13")}");
        Console.WriteLine($"{exprText} = {result}");

        await Task.CompletedTask;

        SpreadsheetUtils.TryParseColumnReference("AA", out var ci);
        Console.WriteLine(ci.ToString());
    }

}
