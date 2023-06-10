using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sandwych.FormulaParser.Model;

public enum CellValueType : byte {
    Empty = 0,
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

    public static readonly CellValue Empty = new CellValue();

    private double _number;
    private string _text;

    private CellValueType _valueType;

    public CellValue()
    {
        _valueType = CellValueType.Empty;
        _number = double.NaN;
        _text = null!;  
    }

    public CellValue(string textValue)
    {
        _valueType = CellValueType.Number;
        _number = double.NaN;
        _text = textValue;
    }

    public CellValue(double numberValue)
    {
        _valueType = CellValueType.Number;
        _number = numberValue;
        _text = string.Empty;
    }

    public CellValue(bool boolValue)
    {
        _valueType = CellValueType.Boolean;
        _number = boolValue ? 1.0 : 0.0;
        _text = string.Empty;
    }

    public CellValue(DateTime dateTimeValue)
    {
        _valueType = CellValueType.DateTime;
        var diff = dateTimeValue - ExcelDateTimeStartValue;
        _number = diff.TotalDays;
        _text = string.Empty;
    }

    public CellValue(DateOnly dateValue)
    {
        _valueType = CellValueType.Number;
        var diff = dateValue.DayNumber - ExcelDateStartValue.DayNumber;
        _number = diff;
        _text = string.Empty;
    }

    public bool IsEmpty => _valueType == CellValueType.Empty;
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
            return (int)Math.Round(_number) != 0;
        }
        set
        {
            _valueType = CellValueType.Boolean;
            _number = value ? 1.0 : 0.0;
        }
    }

    public double NumberValue {
        get
        {
            if (!this.IsDouble)
            {
                throw new InvalidOperationException("Variant does not contain a double value.");
            }
            return _number;
        }
        set
        {
            _valueType = CellValueType.Number;
            _number = value;
        }
    }

    public DateOnly DateValue {
        get
        {
            if (!this.IsDate)
            {
                throw new InvalidOperationException("Variant does not contain a double value.");
            }
            return ExcelDateStartValue.AddDays((int)Math.Ceiling(_number));
        }
        set
        {
            _valueType = CellValueType.Number;
            var diff = value.DayNumber - ExcelDateStartValue.DayNumber;
            _number = diff;
        }
    }

    public DateTime DateTimeValue {
        get
        {
            if (!this.IsDateTimeTime)
            {
                throw new InvalidOperationException("Variant does not contain a double value.");
            }
            return ExcelDateTimeStartValue.AddDays(_number);
        }
        set
        {
            _valueType = CellValueType.Number;
            var diff = value - ExcelDateTimeStartValue;
            _number = diff.TotalDays;
        }
    }

    public string TextValue {
        get
        {
            if (!this.IsText)
            {
                throw new InvalidOperationException("Variant does not contain a string value.");
            }
            return _text;
        }
        set
        {
            _valueType = CellValueType.Text;
            _text = value;
        }
    }

    public int CompareTo(CellValue other) {
        if (this._valueType != other._valueType)
        {
            throw new ArgumentException("Different type", nameof(other));
        }

        return this._number.CompareTo(other._number);
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


