using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.FormulaParser;

public static class SpreadsheetUtils
{
    /// <summary>
    /// 超快 26 进制转换
    /// </summary>
    public static bool TryConvertToColumnReference(int value, Span<char> result, out int charsWritten)
    {
        if (value < 1)
        {
            charsWritten = -1;
            return false;
        }

        const int Base = 26;
        const char Offset = (char)('A' - 1);//(char)64; // ('A' - 1)

        charsWritten = 0;

        while (value > 0)
        {
            value = Math.DivRem(value, Base, out var remainder);

            if (charsWritten >= result.Length)
            {
                return false;
            }

            result[charsWritten] = (char)(remainder + Offset);
            charsWritten++;
        }

        // 反转结果
        for (int i = 0, j = charsWritten - 1; i < j; i++, j--)
        {
            char temp = result[i];
            result[i] = result[j];
            result[j] = temp;
        }

        return true;
    }


    /// <summary>
    /// 不是很快的 26 进制转换
    /// </summary>
    public static string? IndexToColumnReference(int value)
    {
        Span<char> buf = stackalloc char[4];

        if (TryConvertToColumnReference(value, buf, out var charsWritten))
        {
            return new string(buf.Slice(0, charsWritten));
        }
        else
        {
            return null;
        }
    }

    public static bool TryParseColumnReference(ReadOnlySpan<char> column, out int columnIndex)
    {
        columnIndex = 0;
        if (column.Length == 0)
        {
            return false;
        }

        var power = 1;

        for (var i = column.Length - 1; i >= 0; i--)
        {
            char c = column[i];
            if (!char.IsLetter(c))
            {
                return false;
            }

            int value = c - 'A' + 1;
            columnIndex += value * power;
            power *= 26;
        }

        columnIndex--;
        return true;
    }

    public static short ParseColumnReference(ReadOnlySpan<char> column)
    {
        if(TryParseColumnReference(column, out var index))
        {
            return (short)index;
        }
        else
        {
            throw new FormatException();
        }
    }

}
