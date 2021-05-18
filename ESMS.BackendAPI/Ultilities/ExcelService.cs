using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Ultilities
{
    public static class ExcelService
    {
        public static void CreateExcelFile(string filepath, string sheetName)
        {
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(
                filepath,
                SpreadsheetDocumentType.Workbook
            );

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = sheetName
            };
            sheets.Append(sheet);
            workbookpart.Workbook.Save();

            spreadsheetDocument.Close();
        }

        public static void InsertTextExistingExcel(string docName, string text, string cellName, UInt32 cellNumber)
        {
            // Open the document for editing.
            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(docName, true))
            {
                WorksheetPart worksheetPart = spreadSheet.WorkbookPart.WorksheetParts.First();
                // Get the SharedStringTablePart. If it does not exist, create a new one.
                SharedStringTablePart shareStringPart;
                if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                {
                    shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                }
                else
                {
                    shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
                }

                // Insert the text into the SharedStringTablePart.
                int index = InsertSharedStringItem(text, shareStringPart);


                // Insert cell A1 into the new worksheet.
                Cell cell = InsertCellInWorksheet(cellName, cellNumber, worksheetPart);

                // Set the value of cell A1.
                cell.CellValue = new CellValue(index.ToString());
                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                // Save the new worksheet.
                worksheetPart.Worksheet.Save();
            }
        }
        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }
        // Given a WorkbookPart, inserts a new worksheet.
        private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart)
        {
            // Add a new worksheet part to the workbook.
            WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new sheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            string sheetName = "Sheet" + sheetId;

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);
            workbookPart.Workbook.Save();

            return newWorksheetPart;
        }
        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }

        // Retrieve the value of a cell, given a file name, sheet name, 
        // and address name.
        public static string GetCellValue(string fileName,
            string sheetName,
            string addressName)
        {
            string value = null;

            // Open the spreadsheet document for read-only access.
            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(fileName, false))
            {
                // Retrieve a reference to the workbook part.
                WorkbookPart wbPart = document.WorkbookPart;

                // Find the sheet with the supplied name, and then use that 
                // Sheet object to retrieve a reference to the first worksheet.
                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().
                  Where(s => s.Name == sheetName).FirstOrDefault();

                // Throw an exception if there is no sheet.
                if (theSheet == null)
                {
                    throw new ArgumentException("sheetName");
                }

                // Retrieve a reference to the worksheet part.
                WorksheetPart wsPart =
                    (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                // Use its Worksheet property to get a reference to the cell 
                // whose address matches the address you supplied.
                Cell theCell = wsPart.Worksheet.Descendants<Cell>().
                  Where(c => c.CellReference == addressName).FirstOrDefault();

                // If the cell does not exist, return an empty string.
                if (theCell.InnerText.Length > 0)
                {
                    value = theCell.InnerText;

                    // If the cell represents an integer number, you are done. 
                    // For dates, this code returns the serialized value that 
                    // represents the date. The code handles strings and 
                    // Booleans individually. For shared strings, the code 
                    // looks up the corresponding value in the shared string 
                    // table. For Booleans, the code converts the value into 
                    // the words TRUE or FALSE.
                    if (theCell.DataType != null)
                    {
                        switch (theCell.DataType.Value)
                        {
                            case CellValues.SharedString:

                                // For shared strings, look up the value in the
                                // shared strings table.
                                var stringTable =
                                    wbPart.GetPartsOfType<SharedStringTablePart>()
                                    .FirstOrDefault();

                                // If the shared string table is missing, something 
                                // is wrong. Return the index that is in
                                // the cell. Otherwise, look up the correct text in 
                                // the table.
                                if (stringTable != null)
                                {
                                    value =
                                        stringTable.SharedStringTable
                                        .ElementAt(int.Parse(value)).InnerText;
                                }
                                break;

                            case CellValues.Boolean:
                                switch (value)
                                {
                                    case "0":
                                        value = "FALSE";
                                        break;
                                    default:
                                        value = "TRUE";
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
            return value;
        }
        // Retrieve a List of all the sheets in a workbook.
        // The Sheets class contains a collection of 
        // OpenXmlElement objects, each representing one of 
        // the sheets.
        public static Sheets GetAllWorksheets(string fileName)
        {
            Sheets theSheets = null;

            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart wbPart = document.WorkbookPart;
                theSheets = wbPart.Workbook.Sheets;
            }
            return theSheets;
        }
    }
}

