using Microsoft.EntityFrameworkCore;
using RegionCoffersAnylizerWinForm.Models;
using System.Data;

namespace RegionCoffersAnylizerWinForm
{
    public partial class Form1 : Form
    {

        DataTable dataTable;

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            NalogiContext db = new NalogiContext();
            db.Database.SetCommandTimeout(200);

            ORM oRM = new ORM();

            oRM.InitDatas(db, "adygea", "april");

            dataTable = oRM.getRegionDataTable();
            dataGridView1.DataSource = dataTable;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel File (*.xlsx)|*.xlsx";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                FileService.Save_xlxs(dataTable, filename);
   
            }
        }
    }
}
