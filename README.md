# Sandwych.FormulaParser

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Sandwych.FormulaParser** is a .NET C# library that provides a toy-level Excel formula parser for concept verification. It is built on top of the Parlot parsing combinator library. This parser allows you to evaluate and process custom formulas, providing a foundation for building more complex expression evaluation systems.

## Features

- Parse and evaluate Excel-like formulas
- Support for basic mathematical operators (`+`, `-`, `*`, `/`, `^`)
- Built-in functions such as `SUM`, `AVERAGE`, `MAX`, `MIN`, etc.
- Ability to define and use custom functions
- Support for cell references (e.g., `A1`, `B2`, etc.)
- Error handling and reporting for invalid formulas or references
- MIT licensed for free and open-source use

## Project Status

Current status: WORKING-IN-PROGRESS

## Installation

To use Sandwych.FormulaParser in your .NET C# project, you can either:

- Clone the repository and reference the project directly.
- Install the package from NuGet using the following command:

  ```bash
  dotnet add package Sandwych.FormulaParser
  ```

## Usage

To parse and evaluate an Excel formula using Sandwych.FormulaParser, follow these steps:

```csharp
var sheet1 = new Sheet();
sheet1["$A$12"] = new CellValue(10.0);
sheet1["C13"] = new CellValue(100.0);

var scope = new Scope(sheet1.TryGetCellValue);

var exprText = "ADDALL(SUM(1.0, 2.0, $A$12), 100.0) + C13 + 10 ^ 2";

// Custom functions
var customFunctions = new (string, Expression)[]
{
    ("ADDALL", (IEnumerable<double> args) => args.Aggregate((x, y) => x + y)),
    ("SUM", (IEnumerable<double> args) => args.Sum()),
    ("AVG", (IEnumerable<double> args) => args.Average()),
    ("MAX", (IEnumerable<double> args) => args.Max()),
    ("MIN", (IEnumerable<double> args) => args.Min()),
};

var compiler = new FormulaCompiler(customFunctions);

// Compile to .NET IL code and return as a delegate
var dg = compiler.Compile(exprText);

// Invoke the compiled delegate
var result = dg(scope);

// Output the result
Console.WriteLine($"Cell $A$12 = {scope.GetCellValue(CellAddress.Parse("$A$12"))}");
Console.WriteLine($"Cell C13 = {scope.GetCellValue(CellAddress.Parse("C13"))}");
Console.WriteLine($"{exprText} = {result}");

```

## Contributing

Contributions are welcome! If you find any bugs or would like to suggest new features, please create an issue or submit a pull request on the [GitHub repository](

https://github.com/oldrev/Sandwych.FormulaParser).

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.