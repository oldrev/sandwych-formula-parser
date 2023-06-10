using Parlot.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.FormulaParser.Model;

[Flags]
public enum CellAddressFlags : short {
    None = 0,
    AbsoluteColumn = 1,
    AbsoluteRow = 2,
}

public readonly struct CellAddress : IEquatable<CellAddress>    
{
    public short ColumnIndex { get; }
    public int RowIndex { get; }
    public CellAddressFlags Flags { get; }

    public CellAddress(short columnIndex, int rowIndex, CellAddressFlags flags)
    {
        this.ColumnIndex = columnIndex;
        this.RowIndex = rowIndex;
        this.Flags = flags;
    }

    public static CellAddress Parse(string refStr) => Tokens.CellRefTokenCompiled.Parse(refStr);

    public static CellAddress From(ReadOnlySpan<char> colText, ReadOnlySpan<char> rowText)
    {
        CellAddressFlags flags = CellAddressFlags.None;
        var colIndex = 0;
        var rowIndex = 0;

        if (colText[0] == '$')
        {
            flags |= CellAddressFlags.AbsoluteColumn;
            colIndex = SpreadsheetUtils.ParseColumnReference(colText.Slice(1));
        }
        else
        {
            colIndex = SpreadsheetUtils.ParseColumnReference(colText);
        }

        if (rowText[0] == '$')
        {
            flags |= CellAddressFlags.AbsoluteRow;
            rowIndex = int.Parse(rowText.Slice(1)) - 1;
        }
        else
        {
            rowIndex = int.Parse(rowText) - 1;
        }

        var ret = new CellAddress((short)colIndex, rowIndex, flags);
        return ret;
    }

    public override string ToString() {
        Span<char> buf = stackalloc char[32];
        int written = 0;
        if (this.Flags.HasFlag(CellAddressFlags.AbsoluteColumn))
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

        if (this.Flags.HasFlag(CellAddressFlags.AbsoluteRow))
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

    public bool Equals(CellAddress other)
    {
        return other.ColumnIndex == this.ColumnIndex && other.RowIndex == this.RowIndex;
    }
}
