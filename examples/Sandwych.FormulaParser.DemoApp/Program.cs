
using Sandwych.FormulaParser.Model;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Sandwych.FormulaParser.DemoApp;


class Program {

    static async Task Main() {

        var sheet1 = new Sheet();
        sheet1["$A$12"] = new CellValue(10.0);
        sheet1["C13"] = new CellValue(100.0);

        var scope = new Scope(sheet1.TryGetCellValue);

        var exprText = "ADDALL(SUM(1.0, 2.0, $A$12), 100.0) + C13 + 10 ^ 2";

        // 自定义函数
        var customFunctions = new (string, Expression)[]
        {
            ("ADDALL", (IEnumerable<double> args) => args.Aggregate((x, y) => x + y)),
            ("SUM", (IEnumerable<double> args) => args.Sum()),
            ("AVG", (IEnumerable<double> args) => args.Average()),
            ("MAX", (IEnumerable<double> args) => args.Max()),
            ("MIN", (IEnumerable<double> args) => args.Min()),
        };

        var compiler = new FormulaCompiler(customFunctions);

        // 编译成 .NET IL 代码，并返回成委托
        var dg = compiler.Compile(exprText);

        // 调用编译后的委托
        var result = dg(scope);

        // 输出结果
        Console.WriteLine($"Cell $A$12 = {scope.GetCellValue(CellAddress.Parse("$A$12"))}");
        Console.WriteLine($"Cell C13 = {scope.GetCellValue(CellAddress.Parse("C13"))}");
        Console.WriteLine($"{exprText} = {result}");

        await Task.CompletedTask;
    }

}
