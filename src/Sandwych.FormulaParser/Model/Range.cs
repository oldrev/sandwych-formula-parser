using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.FormulaParser.Model;

public readonly struct Range
{
    public CellAddress From { get; }
    public CellAddress To { get; }

    public Range(CellAddress from, CellAddress to)
    {
        this.From = from;
        this.To = to;
    }

}
