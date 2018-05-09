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
    public partial class POSForm : Form
    {
        public POSForm()
        {
            InitializeComponent();
            if (Compare.language == "en")
            {
                for (int i = 0; i < Compare.arrPosEn.Length; i++)
                {
                    if(Compare.lPosFiltered.BinarySearch(Compare.arrPosEn[i])>=0)
                        checkedListBox1.Items.Add(Compare.arrPosEn[i],true);
                    else
                        checkedListBox1.Items.Add(Compare.arrPosEn[i], false);
                }
            }
            else
            {
                for (int i = 0; i < Compare.arrPosRu.Length; i++)
                {
                    if (Compare.lPosFiltered.BinarySearch(Compare.arrPosRu[i]) >= 0)
                        checkedListBox1.Items.Add(Compare.arrPosRu[i], true);
                    else
                        checkedListBox1.Items.Add(Compare.arrPosRu[i], false);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            List<string> ls = new List<string>();
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                ls.Add(checkedListBox1.CheckedItems[i].ToString());
            }
            ls.Sort();
            if (Compare.language == "en")
            {
                Properties.Settings.Default.POSEn.Clear();
                Properties.Settings.Default.POSEn.AddRange(ls.ToArray());
            }
            else
            {
                Properties.Settings.Default.POSRu.Clear();
                Properties.Settings.Default.POSRu.AddRange(ls.ToArray());
            }
            Compare.lPosFiltered = ls;
            Properties.Settings.Default.Save();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
