using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegionCoffersAnylizerWinForm
{
    public partial class selectForm : Form
    {
        public string type;

        public int index;

        HashSet<string> okveds = new HashSet<string>();

        List<string> selectedOkveds = new List<string>();

        public event EventHandler<List<string>> DataPassed;

        public selectForm(List<string> selectedOkveds)
        {
            this.selectedOkveds = selectedOkveds; 
            InitializeComponent();
        }
        private void selectForm_Load(object sender, EventArgs e)
        {
            checkedListBox1.Visible = false;
            textBox1.Visible = false;

            if (type == "ОКВЕД фактический|входит")
            {
                checkedListBox1.Visible = true;
                okveds = ORM.okveds;
                if(okveds.Contains(null))
                {
                    okveds.Remove(null);
                }
                checkedListBox1.Items.AddRange(okveds.Order().ToArray());

                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (selectedOkveds.Contains(checkedListBox1.Items[i]))
                    {
                        checkedListBox1.SetItemChecked(i, true);
                    }
                }
                selectedOkveds.Clear();
            }
            else
            {
                textBox1 .Visible = true;
                if (selectedOkveds.Count > 0)
                {
                    textBox1.Text = selectedOkveds[0];
                    selectedOkveds.Clear();
                }
            }

           
        }


        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (type == "ОКВЕД фактический|входит")
            {
                selectedOkveds.Add(index.ToString());
                selectedOkveds.AddRange(checkedListBox1.CheckedItems.Cast<string>().ToHashSet());
                DataPassed?.Invoke(this, selectedOkveds);
            }
            else
            {
                selectedOkveds.Add(index.ToString());
                selectedOkveds.Add(textBox1.Text);
                DataPassed?.Invoke(this, selectedOkveds);
            }

            // Закрытие дочерней формы
            this.Close();
        }
    }
}
