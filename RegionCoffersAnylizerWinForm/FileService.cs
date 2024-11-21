using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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



            public static void Save_xlxs(DataTable dataTable, string filePath)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                ExcelPackage package = new ExcelPackage();
                try
                {
                    // Создаем новый Excel пакет

                    // Создаем новый лист
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    // Заполняем заголовки столбцов
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
