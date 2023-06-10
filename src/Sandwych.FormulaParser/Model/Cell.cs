using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.FormulaParser.Model;

public readonly struct Cell
{
    public string Formula { get; }

    public Cell(string formula)
    {
        this.Formula = formula;
    }
}
