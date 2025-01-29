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
        string coffersTable = Properties.app.Default.coffersTable;



        public Form1()
        {
            InitializeComponent();
            comboBox1.Text = "volgograd";
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Load load = new Load();
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        

            loadApp(load, true,"");
            
        }

        public void loadApp(Load form, bool firstLoad,string db_name)
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

            // Вызов метода в дочерней форме из другого потока
            this.Invoke((MethodInvoker)delegate
            {
                if(!firstLoad)
                {
                    this.Visible = false;
                    db.Database.SetCommandTimeout(400);
                  //  ORM.initTables(db);
                    ORM.InitDatas(db, db_name, coffersTable);
                    

                   // comboBox1.Items.AddRange(ORM.tablesNames.ToArray());

                    dataTables = ORM.getRegionDataTable();
                    dataGridView1.DataSource = dataTables[0];
                    dataGridView1.AllowUserToAddRows = false;
                    this.Visible = true;
                }
                else
                {
                    ORM.initTables(db);
                    comboBox1.Items.AddRange(ORM.tablesNames.ToArray());
                }

            });

            // Закрытие дочерней формы после выполнения кода
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

            Load load = new Load();
            loadApp(load, false, comboBox1.Text);
       

          /*  if (!dataTables.Contains(null))
            {
                foreach (var dataTable in dataTables)
                {
                    dataTable.Clear();
                }
            }
          */
          //  dataTables = ORM.getRegionDataTable();

            //dataGridView1.DataSource = dataTables[0];
            //dataGridView1.AllowUserToAddRows = false;

            GC.Collect();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChartForm secondForm = new ChartForm();
            secondForm.Data = dataTables[0];



            // Открываем вторую форму
            secondForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            filterForm filterForm = new filterForm();

            filterForm.Show();
        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            Load load = new Load();
            loadApp(load,false,comboBox1.Text);
     

            /*comboBox1.Items.AddRange(ORM.tablesNames.ToArray());

            dataTables = ORM.getRegionDataTable();
            dataGridView1.DataSource = dataTables[0];
            dataGridView1.AllowUserToAddRows = false;*/
        }
    }
}
