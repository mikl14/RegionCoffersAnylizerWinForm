using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegionCoffersAnylizerWinForm
{
    public partial class filterForm : Form
    {
        private Dictionary<int,Filter> filters = new Dictionary<int,Filter>();
        private Dictionary<string,string> translatorEngtoRus = new Dictionary<string,string>();
        private Dictionary<string, string> translatorRustoEng = new Dictionary<string, string>();

        private int currentRow = 0;

        public filterForm()
        {
            InitializeComponent();
        }

        private void filterForm_Load(object sender, EventArgs e)
        {
          
            translatorEngtoRus.Add("FactOkvedOsn", "ОКВЕД фактический");
            translatorEngtoRus.Add("License", "Лицензия");

            translatorRustoEng.Add( "ОКВЕД фактический", "FactOkvedOsn");
            translatorRustoEng.Add("Лицензия", "License");

            updateWindow();
        }

        
        private void updateWindow()
        {
            if(ORM.filters.Count > 0)
            {
                for(int i = 0; i < ORM.filters.Count; i++)
                {
                    AddNewRow(translatorEngtoRus[ORM.filters[i].columnName], ORM.filters[i].isNot, ORM.filters[i].operate, ORM.filters[i].values);

                    filters[i] = ORM.filters[i];
                }
               
            }
        }

        private void showSelectWindow(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int rowIndex = (int)btn.Tag;

            StringBuilder sb = new StringBuilder();
            sb.Append(((ComboBox)flowLayoutPanel1.Controls[$"fieldBox{rowIndex}"]).SelectedItem.ToString());
            sb.Append("|");
            sb.Append(((ComboBox)flowLayoutPanel1.Controls[$"operatorBox{rowIndex}"]).SelectedItem.ToString());

            string value = sb.ToString();

            sb.Clear();
       
            var existingForm = Application.OpenForms.OfType<selectForm>().FirstOrDefault();

                    if (existingForm != null)
                    {
                        // Если окно уже открыто, просто активируем его
                        existingForm.Close();
                    }

                    selectForm selectForm = new selectForm(filters[rowIndex].values);

                    selectForm.type = value;
                    selectForm.index = rowIndex;
                    selectForm.DataPassed += selectForm_DataPassed;
                    selectForm.Show();

                   
            
        }

        private void AddNewRow(string fieldValue = "", bool notBoxState = false,string operatorValue = "",List<string> value = null)
        {
            // Создаем новые элементы управления
            ComboBox fieldBox = new ComboBox();
            ComboBox operatorBox = new ComboBox();

            Button showSelectForm = new Button();

       
            CheckBox notBox = new CheckBox();


            Button btnRemove = new Button();

            // Настраиваем элементы управления

            fieldBox.Items.AddRange(new string[] {"ОКВЕД фактический", "Лицензия" });
            fieldBox.Name = $"fieldBox{currentRow}";
            fieldBox.Width = 150;
            fieldBox.Text = fieldValue;
            fieldBox.SelectedIndexChanged += fieldBox_SelectedIndexChanged1;
            fieldBox.Tag = currentRow;
     

            notBox.Checked = notBoxState;
            notBox.CheckedChanged += NotBox_CheckedChanged;
            notBox.Name += $"notBox{currentRow}";
            notBox.Tag = currentRow;
            notBox.Text = "Не";
            notBox.Width = 50;
        
            operatorBox.Items.AddRange(new string[] { "равно", "входит", "начало" });
            operatorBox.Name = $"operatorBox{currentRow}";
            operatorBox.Width = 150;
            operatorBox.Text = operatorValue;
            operatorBox.SelectedIndexChanged += OperatorBox_SelectedIndexChanged;
            operatorBox.Tag = currentRow;
        



            showSelectForm.Click +=  showSelectWindow;
            showSelectForm.Name += $"showForm{currentRow}";
            showSelectForm.Tag = currentRow;
            if(value != null && value.Count > 0)
            {
                showSelectForm.BackColor = Color.Green;
            }
            


    
            btnRemove.Text = "Удалить";
            btnRemove.Tag = currentRow; // Сохраняем индекс строки
            btnRemove.Click += BtnRemove_Click;

            // Упаковываем элементы в контейнер (например, FlowLayoutPanel)
            flowLayoutPanel1.Controls.Add(fieldBox);
            flowLayoutPanel1.Controls.Add(notBox);
            flowLayoutPanel1.Controls.Add(operatorBox);
            flowLayoutPanel1.Controls.Add(showSelectForm);
            flowLayoutPanel1.Controls.Add(btnRemove);

            filters.Add(currentRow,new Filter());

            currentRow++;
        }

        private void OperatorBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ComboBox btn = sender as ComboBox;
            int rowIndex = (int)btn.Tag;

            filters[rowIndex].operate = flowLayoutPanel1.Controls[$"operatorBox{rowIndex}"].Text;

            filters[rowIndex].values.Clear();
            flowLayoutPanel1.Controls[$"showForm{rowIndex}"].BackColor = Color.White;
        }

        private void NotBox_CheckedChanged(object? sender, EventArgs e)
        {
            CheckBox btn = sender as CheckBox;
            int rowIndex = (int)btn.Tag;

            filters[rowIndex].isNot = ((CheckBox)flowLayoutPanel1.Controls[$"notBox{rowIndex}"]).Checked;

        }

        private void fieldBox_SelectedIndexChanged1(object? sender, EventArgs e)
        {
            ComboBox btn = sender as ComboBox;
            int rowIndex = (int)btn.Tag;

            filters[rowIndex].columnName = translatorRustoEng[flowLayoutPanel1.Controls[$"fieldBox{rowIndex}"].Text];

            filters[rowIndex].values = new List<string>();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int rowIndex = (int)btn.Tag;

            filters.Remove(rowIndex);

            // Удаляем элементы управления из контейнера
            flowLayoutPanel1.Controls.RemoveByKey($"fieldBox{rowIndex}");
            flowLayoutPanel1.Controls.RemoveByKey($"showForm{rowIndex}");
            flowLayoutPanel1.Controls.RemoveByKey($"notBox{rowIndex}");
            flowLayoutPanel1.Controls.RemoveByKey($"operatorBox{rowIndex}");
            flowLayoutPanel1.Controls.Remove(btn);
        }

        private void fieldBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SavePeople()
        {
            ORM.filters = filters.Select(x => x.Value).ToList();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            SavePeople();
            this.Close();
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            AddNewRow();
        }

        private void selectForm_DataPassed(object sender, List<string> data)
        {
            // Обработка переданных данных

            int index = Int32.Parse(data[0]);

            data.RemoveAt(0);

            filters[index].values = data;

            flowLayoutPanel1.Controls[$"showForm{index}"].BackColor = Color.Green;

           // MessageBox.Show($"Данные получены: {data}");
        }
    }
}
