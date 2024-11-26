using Microsoft.EntityFrameworkCore;
using RegionCoffersAnylizerWinForm.Models;
using System.Data;
using System.Windows.Forms;

namespace RegionCoffersAnylizerWinForm
{
    public partial class Form1 : Form
    {

        DataTable[] dataTables = new DataTable[5];
        ORM oRM = new ORM();
        NalogiContext db = new NalogiContext();

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
      
            db.Database.SetCommandTimeout(200);

         

            comboBox1.Items.AddRange(new string[] { "adygea", "volgograd", "rostov", "tula" });

            oRM.InitDatas(db, "volgograd", "yan_september_15_10");

            dataTables = oRM.getRegionDataTable();
            dataGridView1.DataSource = dataTables[0];
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
                FileService.Save_xlxs(dataTables, filename);

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            oRM.InitDatas(db, comboBox1.Text, "yan_september_15_10");

            dataTables = oRM.getRegionDataTable();
        }
    }
}
