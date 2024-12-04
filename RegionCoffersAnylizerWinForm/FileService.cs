using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;


namespace RegionCoffersAnylizerWinForm
{
    internal class FileService
    {

            public static ExcelPackage getExcelSheet(DataTable dataTable,string sheetName, ExcelPackage package)
            {

            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            for (int i = 1; i <= dataTable.Columns.Count; i++)
                {
                    worksheet.Cells[1, i].Value = dataTable.Columns[i - 1].ColumnName;
                }

                // Заполняем данные
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                
                        worksheet.Cells[i + 2, j + 1].Value = dataTable.Rows[i][j];
                    }
                }

            return package;
            }




            public static void Save_xlxs(DataTable[] dataTable, string filePath)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                ExcelPackage package = new ExcelPackage();
                try
                {
                    // Создаем новый Excel пакет

                // Заполняем заголовки столбцов
                    package = getExcelSheet(dataTable[0], "Сводная", package);
                    package = getExcelSheet(dataTable[1], "Выбраные МСП1", package);
                    package = getExcelSheet(dataTable[2], "Выбраные МСП4", package);
                    package = getExcelSheet(dataTable[3], "Выбраные НЕ МСП", package);

                    package = getExcelSheet(dataTable[4], "Все МСП1", package);
                    package = getExcelSheet(dataTable[5], "Все МСП4", package);
                    package = getExcelSheet(dataTable[6], "Все НЕ МСП", package);
               
                // Сохраняем файл
                File.WriteAllBytes(filePath, package.GetAsByteArray());

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при сохранении в Excel: {ex.Message}");
                }
            }
        
    }
}
