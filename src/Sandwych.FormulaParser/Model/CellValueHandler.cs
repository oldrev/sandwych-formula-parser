using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.FormulaParser.Model;

public delegate bool CellValueHandler(CellAddress cell, out CellValue value);
