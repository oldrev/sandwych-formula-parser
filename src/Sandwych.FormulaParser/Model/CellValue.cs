using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sandwych.FormulaParser.Model;

public enum CellValueType : byte {
    Number = 1,
    Date,
    Time,
    DateTime,
    Boolean,
    Text,
}

public struct CellValue : IComparable, IComparable<CellValue>, IEquatable<CellValue>, ISpanFormattable {
    public static readonly DateOnly ExcelDateStartValue = new DateOnly(1900, 1, 1);
    public static readonly DateTime ExcelDateTimeStartValue = new DateTime(1900, 1, 1);

    private double _doubleValue;
    private string _textValue;

    private CellValueType _valueType;

    public bool IsBoolean => _valueType == CellValueType.Boolean;
    public bool IsDouble => _valueType == CellValueType.Number;
    public bool IsText => _valueType == CellValueType.Text;
    public bool IsDate => _valueType == CellValueType.Date;
    public bool IsTime => _valueType == CellValueType.Time;
    public bool IsDateTimeTime => _valueType == CellValueType.DateTime;

    public bool BooleanValue {
        get
        {
            if (!this.IsBoolean)
            {
                throw new InvalidOperationException("Variant does not contain a bool value.");
            }
            return (int)Math.Round(_doubleValue) != 0;
        }
        set
        {
            _valueType = CellValueType.Boolean;
            _doubleValue = value ? 1.0 : 0.0;
        }
    }

    public double NumberValue {
        get
        {
            if (!this.IsDouble)
            {
                throw new InvalidOperationException("Variant does not contain a double value.");
            }
            return _doubleValue;
        }
        set
        {
            _valueType = CellValueType.Number;
            _doubleValue = value;
        }
    }

    public DateOnly DateValue {
        get
        {
            if (!this.IsDate)
            {
                throw new InvalidOperationException("Variant does not contain a double value.");
            }
            return ExcelDateStartValue.AddDays((int)Math.Ceiling(_doubleValue));
        }
        set
        {
            _valueType = CellValueType.Number;
            var diff = value.DayNumber - ExcelDateStartValue.DayNumber;
            _doubleValue = diff;
        }
    }

    public DateTime DateTimeValue {
        get
        {
            if (!this.IsDateTimeTime)
            {
                throw new InvalidOperationException("Variant does not contain a double value.");
            }
            return ExcelDateTimeStartValue.AddDays(_doubleValue);
        }
        set
        {
            _valueType = CellValueType.Number;
            var diff = value - ExcelDateTimeStartValue;
            _doubleValue = diff.TotalDays;
        }
    }

    public string TextValue {
        get
        {
            if (!this.IsText)
            {
                throw new InvalidOperationException("Variant does not contain a string value.");
            }
            return _textValue;
        }
        set
        {
            _valueType = CellValueType.Text;
            _textValue = value;
        }
    }

    public int CompareTo(CellValue other) {
        if (this._valueType != other._valueType)
        {
            throw new ArgumentException("Different type", nameof(other));
        }

        return this._doubleValue.CompareTo(other._doubleValue);
    }

    public int CompareTo(object? value) {
        if (value == null) return 1;
        if (value is not CellValue cv)
        {
            throw new ArgumentException("Must be CellValue");
        }

        return CompareTo(cv);
    }

    public bool Equals(CellValue other) {
        throw new NotImplementedException();
    }

    public string ToString(string? format, IFormatProvider? formatProvider) {
        throw new NotImplementedException();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    public static explicit operator bool(CellValue cv) {
        if (!cv.IsBoolean)
        {
            throw new InvalidCastException("Variant cannot be cast to bool.");
        }
        return cv.BooleanValue;
    }

    public static explicit operator double(CellValue cv) {
        if (!cv.IsDouble)
        {
            throw new InvalidCastException("Variant cannot be cast to double.");
        }
        return cv.NumberValue;
    }

    public static explicit operator string(CellValue cv) {
        if (!cv.IsText)
        {
            throw new InvalidCastException("Variant cannot be cast to string.");
        }
        return cv.TextValue;
    }


}


