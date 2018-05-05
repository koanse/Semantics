using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Semantics
{
    public partial class CmpForm : Form
    {
        public CmpForm()
        {
            InitializeComponent();
            tbT1.Text = tbT2.Text = "I want to see your face";
        }
        void ok_Click(object sender, EventArgs e)
        {
            tvS1.Nodes.Clear();
            tvS2.Nodes.Clear();
            tvNorm1.Nodes.Clear();
            tvNorm2.Nodes.Clear();
            List<WordRu> lRoot1 = SyntaxRu.Analize(tbT1.Text), lRoot2 = SyntaxRu.Analize(tbT2.Text);
            SyntaxRu.RemovePrepPunc(lRoot1);
            SyntaxRu.RemovePrepPunc(lRoot2);
            tvS1.Nodes.AddRange(TNodes(lRoot1));
            tvS2.Nodes.AddRange(TNodes(lRoot2));
            int x = SyntaxRu.Syn(lRoot1, lRoot2);
            tvNorm1.Nodes.AddRange(TNodes(lRoot1));
            tvNorm2.Nodes.AddRange(TNodes(lRoot2));
            double cmp = SyntaxRu.Compare(lRoot1, lRoot2);
            string res = "совпадают";
            if (cmp < 0.4)
                res = "отличаются";
            MessageBox.Show(string.Format("Тексты {0} по смыслу ({1})", res, cmp));                
        }
        void cancel_Click(object sender, EventArgs e)
        {
            //if (openFileDialog1.ShowDialog() != DialogResult.OK)
            //    return;
            //int i = Syntax.Freq(openFileDialog1.FileName);
            //MessageBox.Show(i.ToString());
            Close();
        }
        TreeNode[] TNodes(List<WordRu> lRoot)
        {
            List<TreeNode> lTN = new List<TreeNode>();
            Stack<TreeNode> stTN = new Stack<TreeNode>();
            Stack<WordRu> stW = new Stack<WordRu>();
            foreach (WordRu r in lRoot)
            {
                stW.Push(r);
                TreeNode root = new TreeNode(r.ToString());
                lTN.Add(root);
                stTN.Push(root);
                while (stW.Count > 0)
                {
                    WordRu w = stW.Pop();
                    TreeNode tn = stTN.Pop();
                    foreach (WordRu c in w.lCh)
                    {
                        TreeNode tnc = new TreeNode(c.ToString());
                        tn.Nodes.Add(tnc);
                        stW.Push(c);
                        stTN.Push(tnc);
                    }
                }
            }
            return lTN.ToArray();
        }
    }
}
