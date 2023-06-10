using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.FormulaParser.Model;

public interface ISpreadsheetDataProvider
{
    CellValue GetCellValue(CellAddress cref);

}
