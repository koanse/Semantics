using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cmp
{
    public partial class TeachForm : Form
    {
        public TeachForm()
        {
            InitializeComponent();
            for (int i = 0; i < Compare.lPosFiltered.Count; i++)
            {
                dataGridView1.Rows.Add(Compare.lPosFiltered[i], "1");
            }
            dataGridView1.Rows.Add("Неопределенная_часть_речи", "1");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                dataGridView1.Enabled = true;
            else
                dataGridView1.Enabled = false;
        }
    }
}
