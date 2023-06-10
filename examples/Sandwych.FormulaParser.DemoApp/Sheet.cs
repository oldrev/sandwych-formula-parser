using Sandwych.FormulaParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.FormulaParser.DemoApp;

public class SparseArray<TValue> : Dictionary<int, TValue> { }

public class SheetColumn : SparseArray<CellValue>
{
}

/// <summary>
/// 用于演示的按列组织的数据，其实就是一个稀疏矩阵
/// </summary>
public class Sheet : SparseArray<SheetColumn>
{
    public CellValue this[CellAddress cref]
    {
        get => this[cref.RowIndex, cref.ColumnIndex];
        set => this[cref.RowIndex, cref.ColumnIndex] = value;
    }

    public CellValue this[int rowIndex, int columnIndex]
    {
        get
        {
            var rows = this[columnIndex];
            var val = rows[rowIndex];
            return val;
        }
        set
        {
            if (this.TryGetValue(columnIndex, out var rows))
            {
                rows[rowIndex] = value;
            }
            else
            {
                this[columnIndex] = new SheetColumn() { { rowIndex, value } };
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
