using Sandwych.FormulaParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.FormulaParser;

/// <summary>
/// Holds a scope and its variables
/// </summary>
public class Scope {
    readonly Scope _parent;
    readonly Dictionary<string, object> _variables = new Dictionary<string, object>();

    /// <summary>
    /// Used to create scopes
    /// </summary>
    public Scope(Scope parent) {
        _parent = parent;
    }

    /// <summary>
    /// Creates a base scope
    /// </summary>
    public Scope() : this(null) { }

    /// <summary>
    /// Sets or creates a variable in the local scope
    /// </summary>
    public void SetLocal(string name, object value) {
        _variables[name] = value;
    }

    /// <summary>
    /// Sets or creates a variable in the global scope
    /// </summary>
    public void SetGlobal(string name, object value) {
        if (_parent == null)
        {
            _variables[name] = value;
        }
        else
        {
            _parent.SetGlobal(name, value);
        }
    }

    /// <summary>
    /// Returns the nearest declared variable value or nil
    /// </summary>
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

    /// <summary>
    /// Sets the nearest declared variable or creates a new one
    /// </summary>
    public void Set(string name, object? value) {
        object? obj = null;
        if (_parent == null || _variables.TryGetValue(name, out obj))
            _variables[name] = value;
        else
            _parent.Set(name, value);
    }

    public double GetCellValue(CellRef cell)
    {
        return 0;
    }


}

