using Microsoft.EntityFrameworkCore;
using RegionCoffersAnylizerWinForm.Models;

namespace RegionCoffersAnylizerWinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NalogiContext db = new NalogiContext();
            db.Database.SetCommandTimeout(200);

            ORM oRM = new ORM();

            oRM.InitDatas(db,"adygea", "april");

            dataGridView1.DataSource = oRM.getRegionDataTable();
            dataGridView1.AllowUserToAddRows = false;
        }
    }
}
