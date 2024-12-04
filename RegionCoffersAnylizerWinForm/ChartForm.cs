using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Runtime.InteropServices.JavaScript.JSType;
using OxyPlot.Axes;
using System.Windows.Markup;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.X509Certificates;

namespace RegionCoffersAnylizerWinForm
{
    public partial class ChartForm : Form
    {
        public DataTable Data = new DataTable();

        public ChartForm()
        {
            InitializeComponent();
        }

        public Dictionary<string, decimal> columnToDictionary(string columnName)
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>(); ;

            foreach (DataColumn column in Data.Columns)
            {
                if (column.ColumnName == columnName)
                {
                    foreach (DataRow row in Data.Rows)
                    {
                        result.Add(row.ItemArray[0].ToString(),Decimal.Parse(row.ItemArray[column.Ordinal].ToString()));
                    }
                    return result;
                }
            }
           return result ;
        }

        private void ChartForm_Load(object sender, EventArgs e)
        {

            foreach (DataColumn column in Data.Columns)
            {
                comboBox1.Items.Add(column.ColumnName);
            }
            comboBox1.SelectedIndex = 2;
            // Создаем график
            buildGraph(comboBox1.GetItemText(comboBox1.SelectedItem));


        }

        public void buildGraph( string columnName)
        {

            Dictionary<string,decimal> dataBuf =  columnToDictionary(columnName);

           // dataBuf = dataBuf.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            var plotModel = new PlotModel { Title = "Значения по полю" };

            // Создаем BarSeries для вертикального графика
            var barSeries = new ColumnSeries();

            // Настраиваем оси
            var categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom}; // Устанавливаем категориальную ось слева
            categoryAxis.Labels.AddRange(dataBuf.Keys);
            plotModel.Axes.Add(categoryAxis);

            var linearAxis = new LinearAxis { Position = AxisPosition.Left }; // Устанавливаем линейную ось снизу
            plotModel.Axes.Add(linearAxis);

   
            // Отображаем график

            // Заполняем график данными из словаря

            
            foreach (var pair in categoryAxis.Labels)
            {
                if (dataBuf[pair] > 0)
                {
                    barSeries.Items.Add(new ColumnItem { Value = ((double)dataBuf[pair]) });
                }
            }



            plotModel.Series.Add(barSeries);

    
            plotView1.Model = plotModel;
        }

        private void plotView1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            buildGraph(comboBox1.Text);
        }
    }
}
