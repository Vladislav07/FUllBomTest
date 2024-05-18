using SpreadsheetGear;
using System.Data;

namespace ExportToPDF
{
    public class BooksRepo
    {
        SpreadsheetGear.IWorkbookSet workbookSet;
        public BooksRepo()
        {
            workbookSet = SpreadsheetGear.Factory.GetWorkbookSet();
        }

        public void GetTableDt(string path, ref System.Data.DataTable dt)
        {
            SpreadsheetGear.IWorkbook workbook = this.AddBook(path);
            SpreadsheetGear.IWorksheet sheet = ReturnSheet(workbook);
            int rowEnd = SearchLastline(sheet, 1);

            IRange rangeFirst = sheet.Cells[0,0, 0, 49];
            IRange rangeBody = sheet.Cells[1, 0, rowEnd, 49];
            FillHeadTable(rangeFirst, ref dt);
            FillDataTable(rangeBody, ref dt);
        }

        private void FillHeadTable(IRange range, ref DataTable dt)
        {
            int count =(int)range.CellCount;
            for (int i = 0; i < count; i++)
            {
                dt.Columns.Add(range.Cells[0,i].Formula.ToString()); 
            }
                 
        }
      
        private void FillDataTable(IRange rows, ref DataTable dt)
        {
            foreach (IRange currentRow in rows.Rows)
            {
                int count = (int)currentRow.CellCount;
                    DataRow workRow = dt.NewRow();
                    for (int i = 0; i < count; i++)
                    {
                        workRow[i] = currentRow.Cells[0, i].Formula.ToString();
                       
                    }
                dt.Rows.Add(workRow);
            }
        }
        

        private SpreadsheetGear.IWorkbook AddBook(string way)
        {
            SpreadsheetGear.IWorkbook workbook = workbookSet.Workbooks.Open(way);
            return workbook;
        }
        private SpreadsheetGear.IWorksheet ReturnSheet(SpreadsheetGear.IWorkbook book)
        {
            SpreadsheetGear.IWorksheet sheet = book.Worksheets["Лист1"];
            return sheet;
        }
        private int SearchLastline(SpreadsheetGear.IWorksheet sheet, int rowStart)
        {

            int u = rowStart;

            IRange lastRowRange;
            do
            {
                u++;
                lastRowRange = sheet.Cells[u, 0];
            }
            while (lastRowRange.Value != null);

            return u - 1;
        }

    }
}
