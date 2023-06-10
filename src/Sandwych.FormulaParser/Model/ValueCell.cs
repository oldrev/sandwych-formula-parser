using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.FormulaParser.Model;

public struct ValueCell
{
    public CellValue Value { get; set; }

    public ValueCell(CellValue cellValue)
    {
        this.Value = cellValue;
    }
}
