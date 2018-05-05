using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TInput
{
    public partial class InputForm : Form
    {        
        public InputForm()
        {
            InitializeComponent();
            tbG.Text = Properties.Settings.Default.Group;
            tbQ.Text = Properties.Settings.Default.Question;
            tbE.Text = Properties.Settings.Default.Etalon;
            folderBrowserDialog1.SelectedPath = Properties.Settings.Default.Path;
            if (Properties.Settings.Default.Tutors != null)
                foreach (string s in Properties.Settings.Default.Tutors)
                {
                    dgv.Rows.Add(s, 0);
                }
            
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                Properties.Settings.Default.Path = folderBrowserDialog1.SelectedPath;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbFIO.Text = tbG.Text = tbQ.Text = tbE.Text = tbAns.Text = "";
            dgv.Rows.Clear();
            Properties.Settings.Default.Group = Properties.Settings.Default.Question =
                Properties.Settings.Default.Etalon = "";
            Properties.Settings.Default.Tutors = null;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Завершить работу?", "Выход", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Properties.Settings.Default.Save();
                Close();
            }            
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbFIO.Text == "" || dgv.RowCount == 1)
                    throw new Exception("Ошибка ввода");
                Tutor[] arrT = new Tutor[dgv.RowCount - 1];
                for (int i = 0; i < dgv.RowCount - 1; i++)
                {
                    arrT[i] = new Tutor(dgv[0, i].Value.ToString(),
                        double.Parse(dgv[1, i].Value.ToString()));
                }
                C_EAnswer a = new C_EAnswer(tbFIO.Text, tbG.Text, tbQ.Text, tbE.Text, tbAns.Text, arrT);
                a.Save(string.Format("{0}\\{1}.xml", folderBrowserDialog1.SelectedPath, tbFIO.Text));

                if (Properties.Settings.Default.Tutors != null)
                    Properties.Settings.Default.Tutors.Clear();
                else
                    Properties.Settings.Default.Tutors = new System.Collections.Specialized.StringCollection();
                
                
                tbFIO.Text = tbAns.Text = "";
                if (!cbG.Checked)
                    tbG.Text = Properties.Settings.Default.Group = "";
                else
                    Properties.Settings.Default.Group = tbG.Text;
                if (!cbQ.Checked)
                    tbQ.Text = Properties.Settings.Default.Question = "";
                else
                    Properties.Settings.Default.Question = tbQ.Text;
                if (!cbE.Checked)
                    tbE.Text = Properties.Settings.Default.Etalon = "";
                else
                    Properties.Settings.Default.Etalon = tbE.Text;
                if (!cbT.Checked)
                {
                    dgv.Rows.Clear();
                    Properties.Settings.Default.Tutors = null;
                }
                else
                {
                    Properties.Settings.Default.Tutors = new System.Collections.Specialized.StringCollection();
                    for (int i = 0; i < dgv.RowCount - 1; i++)
                    {
                        Properties.Settings.Default.Tutors.Add(dgv[0, i].Value.ToString());
                        dgv[1, i].Value = 0;
                    }
                }
                this.ActiveControl = tbFIO;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                if (sender as TextBox == tbFIO)
                    this.ActiveControl = tbG;
                else if (sender as TextBox == tbG)
                    this.ActiveControl = tbQ;
                else if (sender as TextBox == tbQ)
                    this.ActiveControl = tbE;
                else if (sender as TextBox == tbE)
                    this.ActiveControl = tbAns;
                else if (sender as TextBox == tbAns)
                    this.ActiveControl = dgv;
        }
    }
}
