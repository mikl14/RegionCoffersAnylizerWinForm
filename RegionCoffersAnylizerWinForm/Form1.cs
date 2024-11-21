using Microsoft.EntityFrameworkCore;
using RegionCoffersAnylizerWinForm.Models;
using System.Data;
using System.Windows.Forms;

namespace RegionCoffersAnylizerWinForm
{
    public partial class Form1 : Form
    {

        DataTable dataTable;
        ORM oRM = new ORM();
        NalogiContext db = new NalogiContext();

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
      
            db.Database.SetCommandTimeout(200);

         

            comboBox1.Items.AddRange(new string[] { "adygea", "piterburg", "rostov", "tula" });

            oRM.InitDatas(db, "adygea", "yan_september_15_10");

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            oRM.InitDatas(db, comboBox1.Text, "yan_september_15_10");

            dataTable = oRM.getRegionDataTable();
        }
    }
}
