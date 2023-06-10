using Sandwych.FormulaParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.FormulaParser.DemoApp;

/// <summary>
/// 用于演示的按列组织的数据，其实就是一个稀疏矩阵
/// </summary>
public class Sheet : Dictionary<int, Dictionary<int, CellValue>>
{
    public CellValue this[CellAddress cref]
    {
        get
        {
            var rows = this[cref.ColumnIndex];
            var val = rows[cref.RowIndex];
            return val;
        }
        set
        {
            if (this.TryGetValue(cref.ColumnIndex, out var rows))
            {
                rows[cref.RowIndex] = value;
            }
            else
            {
                this[cref.ColumnIndex] = new Dictionary<int, CellValue> { { cref.RowIndex, value } };
            }
        }
    }

    public CellValue this[string crefStr]
    {
        get => this[CellAddress.Parse(crefStr)];
        set => this[CellAddress.Parse(crefStr)] = value;
    }

    public bool TryGetCellValue(CellAddress cref, out CellValue cellValue)
    {
        if (this.TryGetValue(cref.ColumnIndex, out var rows))
        {
            if (rows.TryGetValue(cref.RowIndex, out var actualValue))
            {
                cellValue = actualValue;
                return true;
            }
            else
            {
                cellValue = default;
                return false;
            }

        }
        else
        {
            cellValue = default;
            return false;
        }

    }

}
