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
    public partial class DicForm : Form
    {
        public DicForm()
        {
            InitializeComponent();
            
            string[] arrK = Compare.dicSyn.Keys.ToArray();
            List<string>[] arrV = Compare.dicSyn.Values.ToArray();
            DataGridViewRow[] arrR = new DataGridViewRow[arrK.Length];
            dataGridView1.Rows.Add("","");
            for (int i = 0; i < Compare.dicSyn.Keys.Count; i++)
            {
                string val = "";
                foreach (string s in arrV[i])
                {
                    val += s + " ";
                }
                arrR[i] = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                arrR[i].SetValues(arrK[i], val);
            }
            dataGridView1.Rows.RemoveAt(0);
            dataGridView1.SuspendLayout();
            dataGridView1.Rows.AddRange(arrR);
            dataGridView1.ResumeLayout();
            label1.Text = Properties.Settings.Default.DicPath;
        }
    }
}
