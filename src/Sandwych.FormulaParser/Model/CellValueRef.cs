using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.FormulaParser.Model;

public readonly ref struct CellValueRef
{
    private readonly ref CellValue _value;
    public ref CellValue Value => ref _value;

    public CellValueRef(ref CellValue value)
    {
        _value = ref value;
    }
}

