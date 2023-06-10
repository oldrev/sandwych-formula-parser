using Sandwych.FormulaParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.FormulaParser;

public class Scope {
    readonly Scope? _parent = null;
    readonly Dictionary<string, object?> _variables = new Dictionary<string, object?>();

    public CellValueHandler? CellValueHandler { get; private set; }

    public Scope(CellValueHandler? cellValueHandler = null, Scope? parent = null)
    {
        this.CellValueHandler = cellValueHandler;
        _parent = parent;
    }

    public void SetLocal(string name, object? value) {
        _variables[name] = value;
    }

    public void SetGlobal(string name, object? value) {
        if (_parent == null)
        {
            _variables[name] = value;
        }
        else
        {
            _parent.SetGlobal(name, value);
        }
    }

    public object? Get(string name)
    {
        if (_variables.TryGetValue(name, out var obj))
        {
            return obj;
        }
        else
        {
            if (_parent != null)
            {
                return _parent.Get(name);
            }
            else
            {
                throw new KeyNotFoundException(name);
            }

        }
    }

    public void Set(string name, object? value) {
        object? obj = null;
        if (_parent == null || _variables.TryGetValue(name, out obj))
        {
            _variables[name] = value;
        }
        else
        {
            _parent.Set(name, value);
        }
    }

    public double GetCellValue(CellAddress cell)
    {
        if(this.CellValueHandler != null && this.CellValueHandler(cell, out var childValue))
        {
            return childValue.NumberValue;
        }
        else if (_parent != null && _parent.CellValueHandler != null && _parent.CellValueHandler(cell, out var parentValue))
        {
            return parentValue.NumberValue;
        }
        else
        {
            return double.NaN;
        }
    }


}

