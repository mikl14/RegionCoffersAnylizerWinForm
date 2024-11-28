using Microsoft.EntityFrameworkCore;
using RegionCoffersAnylizerWinForm.Models;
using System.Data;
using System.Runtime;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RegionCoffersAnylizerWinForm
{
    public partial class Form1 : Form
    {

        DataTable[] dataTables = new DataTable[5];
        NalogiContext db = new NalogiContext();

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            Load load = new Load();

            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            loadApp(load,true);

            
        }

        public void loadApp(Load form, bool firstLoad)
        {
            Task.Run(() =>
            {
                Application.EnableVisualStyles();
                if (firstLoad)
                {
                    Application.SetCompatibleTextRenderingDefault(false);
                }


                Application.Run(form);
            });

            // ����� ������ � �������� ����� �� ������� ������
            this.Invoke((MethodInvoker)delegate
            {
                db.Database.SetCommandTimeout(200);
                ORM oRM = new ORM();

                oRM.InitDatas(db, "volgograd", "yan_september_15_10");

                comboBox1.Items.AddRange(oRM.tablesNames.ToArray());

                dataTables = oRM.getRegionDataTable();
                dataGridView1.DataSource = dataTables[0];
                dataGridView1.AllowUserToAddRows = false;

            });

            // �������� �������� ����� ����� ���������� ����
            form.Invoke((MethodInvoker)delegate
            {
                form.Close();
            });
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
            ORM oRM = new ORM();
            oRM.InitDatas(db, comboBox1.Text, "yan_september_15_10");

            foreach (var dataTable in dataTables)
            {
                dataTable.Clear();
            }
            dataGridView1.DataSource = dataTables[0];

            dataTables = oRM.getRegionDataTable();

            dataGridView1.DataSource = dataTables[0];
            dataGridView1.AllowUserToAddRows = false;

            GC.Collect();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChartForm secondForm = new ChartForm();
            secondForm.Data = dataTables[0];



            // ��������� ������ �����
            secondForm.Show();
        }
    }
}
