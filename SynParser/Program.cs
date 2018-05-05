using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SynParser
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ParseSyn();
            //Application.Run(new Form1());
        }
        static void ParseSyn()
        {
            StreamReader sr = new StreamReader("dict.txt", System.Text.Encoding.Default);
            string s = sr.ReadToEnd();
            sr.Close();
            Form1 f = new Form1();
            f.Show();
            DSDict ds = new DSDict();
            string[] arrStr = s.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> lw = new List<string>();
            List<DSDict.WordsRow> lwr = new List<DSDict.WordsRow>();
            int gCount = 0;
            foreach (string line in arrStr)
            {
                Regex re = new Regex("(?<=^)\\w+(-)?\\w+(?=\\|)", RegexOptions.Compiled);
                Match m = re.Match(line);
                if (!m.Success)
                    continue;
                re = new Regex("(?<=^|,)\\w+(-)?\\w+(?=,|$)", RegexOptions.Compiled);
                MatchCollection mc = re.Matches(line.Remove(0, m.Length + 1));
                if (mc.Count == 0)
                    continue;
                DSDict.WordsRow wr = null;
                string mLower = m.Value.ToLower();
                int index = lw.BinarySearch(mLower);
                if (index >= 0)
                    wr = lwr[index];
                else
                {
                    wr = ds.Words.AddWordsRow(lw.Count, mLower);
                    lw.Insert(~index, mLower);
                    lwr.Insert(~index, wr);                                        
                }
                DSDict.GroupsRow gr = ds.Groups.AddGroupsRow(gCount++, wr);
                for (int i = 0; i < mc.Count; i++)
                {
                    mLower = mc[i].Value.ToLower();
                    index = lw.BinarySearch(mLower);
                    if (index >= 0)
                        wr = lwr[index];
                    else
                    {
                        wr = ds.Words.AddWordsRow(lw.Count, mLower);
                        lw.Insert(~index, mLower);
                        lwr.Insert(~index, wr);
                    }
                    try
                    {
                        ds.WG.AddWGRow(wr, gr);
                    }
                    catch { }
                }
                if (gCount % 1000 == 0)
                {
                    f.progressBar1.Value = gCount * 100 / arrStr.Length;
                    Application.DoEvents();
                }
            }
            DSDictTableAdapters.WordsTableAdapter wta = new DSDictTableAdapters.WordsTableAdapter();
            wta.Update(ds.Words);
            DSDictTableAdapters.GroupsTableAdapter gta = new DSDictTableAdapters.GroupsTableAdapter();
            gta.Update(ds.Groups);
            DSDictTableAdapters.WGTableAdapter wgta = new DSDictTableAdapters.WGTableAdapter();
            wgta.Update(ds.WG);
        }
    }
}
