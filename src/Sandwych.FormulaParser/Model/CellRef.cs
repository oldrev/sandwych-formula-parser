using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.FormulaParser.Model;

[Flags]
public enum CellRefFlags : byte {
    None = 0,
    AbsoluteColumn = 1,
    AbsoluteRow = 2,
}

public readonly record struct CellRef(short ColumnIndex, int RowIndex, CellRefFlags Flags) {
    public static CellRef From(ReadOnlySpan<char> colText, ReadOnlySpan<char> rowText) {
        CellRefFlags flags = CellRefFlags.None;
        var colIndex = 0;
        var rowIndex = 0;

        if (colText[0] == '$')
        {
            flags |= CellRefFlags.AbsoluteColumn;
            colIndex = SpreadsheetUtils.ParseColumnReference(colText.Slice(1));
        }
        else
        {
            colIndex = SpreadsheetUtils.ParseColumnReference(colText);
        }

        if (rowText[0] == '$')
        {
            flags |= CellRefFlags.AbsoluteRow;
            rowIndex = int.Parse(rowText.Slice(1)) - 1;
        }
        else
        {
            rowIndex = int.Parse(rowText) - 1;
        }

        var ret = new CellRef((short)colIndex, rowIndex, flags);
        return ret;
    }

    public override string ToString() {
        Span<char> buf = stackalloc char[32];
        int written = 0;
        if (this.Flags.HasFlag(CellRefFlags.AbsoluteColumn))
        {
            buf[written] = '$';
            written++;
        }

        if (SpreadsheetUtils.TryConvertToColumnReference(this.ColumnIndex + 1, buf.Slice(written), out var w1))
        {
            written += w1;
        }
        else
        {
            return "#ERROR";
        }

        if (this.Flags.HasFlag(CellRefFlags.AbsoluteRow))
        {
            buf[written] = '$';
            written++;
        }
        if ((this.RowIndex + 1).TryFormat(buf.Slice(written), out var w2))
        {
            written += w2;
        }
        else
        {
            return "#ERROR";
        }

        var str = buf.Slice(0, written).ToString();

        return str;
    }

}
