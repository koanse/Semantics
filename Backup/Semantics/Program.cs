using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Collections;

namespace Semantics
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new CmpForm());
            List<Word> l = Compare.Parse("Owing to the significant assimilation of various European languages throughout history, modern English contains a very large vocabulary.");

        }
    }
    #region ru
    static public class SyntaxRu
    {
        static public List<WordRu> Analize(string text)
        {
            StreamWriter sr = new StreamWriter("in.txt", false, System.Text.Encoding.Default);
            sr.Write(text);
            sr.Close();
            Process pr = Process.Start("bin\\wrf.exe", "i:in.txt o:out.xml lc:log.txt xml n");
            pr.WaitForExit();
            FileStream fs = new FileStream("out.xml", FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            List<WordRu> lRoot = new List<WordRu>();
            Stack<WordRu> st = new Stack<WordRu>();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.XmlDeclaration:
                        break;
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "Node":
                                LinkTagRu lnkM = LinkTagRu.none, lnkD = LinkTagRu.none;
                                string sign = reader.GetAttribute("Sign");
                                string lTag = reader.GetAttribute("LeftTag"), rTag = reader.GetAttribute("RightTag");
                                if (sign == "<")
                                {
                                    lnkM = (LinkTagRu)Enum.Parse(typeof(LinkTagRu), lTag.Substring(0, lTag.Length - 3));
                                    lnkD = (LinkTagRu)Enum.Parse(typeof(LinkTagRu), rTag.Substring(0, rTag.Length - 3));
                                }
                                else if (sign == ">")
                                {
                                    lnkD = (LinkTagRu)Enum.Parse(typeof(LinkTagRu), lTag.Substring(0, lTag.Length - 3));
                                    lnkM = (LinkTagRu)Enum.Parse(typeof(LinkTagRu), rTag.Substring(0, rTag.Length - 3));
                                }                                
                                while (reader.Read() && reader.NodeType != XmlNodeType.Element) ;
                                string norm = reader.GetAttribute("Norm"), form = reader.GetAttribute("Form");
                                List<AttributeRu> lAt = new List<AttributeRu>();
                                while (true)
                                {
                                    while (reader.Read() && reader.NodeType != XmlNodeType.Element &&
                                        reader.NodeType != XmlNodeType.EndElement) ;
                                    if (reader.NodeType == XmlNodeType.EndElement)
                                        break;
                                    while (reader.Read() && reader.NodeType != XmlNodeType.Text) ;
                                    lAt.Add((AttributeRu)int.Parse(reader.Value));
                                    while (reader.Read() && reader.NodeType != XmlNodeType.EndElement) ;                                    
                                }
                                WordRu par = null;
                                if (st.Count > 0)
                                    par = st.Peek();
                                WordRu w = new WordRu(norm, form, lnkM, lnkD, lAt, new List<WordRu>(), par);
                                if (par != null)
                                    par.lCh.Add(w);
                                else
                                    lRoot.Add(w);
                                st.Push(w);
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == "Node")
                            st.Pop();
                        break;
                }
            }
            fs.Close();
            return lRoot;
        }
        static public void RemovePrepPunc(List<WordRu> lRoot)
        {
            Stack<WordRu> st = new Stack<WordRu>();
            List<WordRu> lRemove = new List<WordRu>();
            foreach (WordRu r in lRoot)
            {
                st.Push(r);
                while (st.Count > 0)
                {
                    WordRu w = st.Pop();
                    if (w.norm[0] == ',' && w.norm.Length > 1)
                    {
                        w.norm = w.norm.Substring(2);
                        w.form = w.form.Substring(2);
                    }
                    bool punc = w.lAt.Contains(AttributeRu.пунктуация), prep = w.lAt.Contains(AttributeRu.предлог);
                    if (punc || prep)
                        if (w.par == null)
                            lRemove.Add(w);
                        else
                        {
                            w.par.lCh.Remove(w);
                            if (w.lCh.Count > 0)
                            {
                                foreach (WordRu c in w.lCh)
                                {
                                    w.par.lCh.Add(c);
                                    if (prep)
                                        c.prep = w.norm;
                                    c.lnkM = w.lnkM;
                                }
                            }
                        }
                    foreach (WordRu c in w.lCh)
                        st.Push(c);
                }
            }
            foreach (WordRu w in lRemove)
                lRoot.Remove(w);
        }
        static public int Freq(string file)
        {
            StreamReader sr = new StreamReader(file, System.Text.Encoding.Default);
            string s = sr.ReadToEnd();
            List<WordRu> lRoot = Analize(s);
            SyntaxRu.RemovePrepPunc(lRoot);
            List<string> lNorm = new List<string>();
            List<int> lFreq = new List<int>();
            Stack<WordRu> st = new Stack<WordRu>();
            foreach (WordRu r in lRoot)
            {
                st.Push(r);
                while (st.Count > 0)
                {
                    WordRu w = st.Pop();
                    int index = lNorm.BinarySearch(w.norm);
                    if (index < 0)
                    {
                        lNorm.Insert(~index, w.norm);
                        lFreq.Insert(~index, 1);
                    }
                    else
                        lFreq[index]++;
                    foreach (WordRu c in w.lCh)
                        st.Push(c);
                }
            }
            DSDict.FreqDataTable ft = new DSDict.FreqDataTable();
            DSDictTableAdapters.FreqTableAdapter fta = new Semantics.DSDictTableAdapters.FreqTableAdapter();
            int count = 0;
            for (int i = 0; i < lNorm.Count; i++)
            {
                fta.FillByWord(ft, lNorm[i]);
                if (ft.Rows.Count == 0)
                    fta.Insert(lNorm[i], lFreq[i]);
                else
                    fta.UpdateByWord(lNorm[i], (int)ft[0]["freq"] + lFreq[i], lNorm[i]);
                count++;
            }
            return count;
        }
        static public int Syn(List<WordRu> lRoot1, List<WordRu> lRoot2)
        {
            List<string> lNorm1 = new List<string>(), lNorm2 = new List<string>(), lNorm = new List<string>();
            List<int> lFreq1 = new List<int>(), lFreq2 = new List<int>();
            List<List<WordRu>> llW1 = new List<List<WordRu>>(), llW2 = new List<List<WordRu>>();
            List<List<string>> llSyn1 = new List<List<string>>(), llSyn2 = new List<List<string>>();
            List<List<int>> llSFreq1 = new List<List<int>>(), llSFreq2 = new List<List<int>>();
            List<bool> lMatch1 = new List<bool>(), lMatch2 = new List<bool>();
            Stack<WordRu> st = new Stack<WordRu>();
            DSDictTableAdapters.FreqTableAdapter fta = new Semantics.DSDictTableAdapters.FreqTableAdapter();
            DSDictTableAdapters.SynTableAdapter sta = new Semantics.DSDictTableAdapters.SynTableAdapter();
            DSDict.FreqDataTable fdt = new DSDict.FreqDataTable();
            DSDict.SynDataTable sdt = new DSDict.SynDataTable();
            foreach (WordRu r in lRoot1)
            {
                st.Push(r);
                while (st.Count > 0)
                {
                    WordRu w = st.Pop();
                    int index = lNorm1.BinarySearch(w.norm);
                    if (index >= 0)
                    {
                        llW1[index].Add(w);
                        continue;
                    }
                    index = ~index;
                    lNorm1.Insert(index, w.norm);
                    if (fta.FillByWord(fdt, w.norm) > 0)
                        lFreq1.Insert(index, (int)fdt[0].freq);
                    else
                        lFreq1.Insert(index, 0);
                    sta.Fill(sdt, w.norm);
                    List<string> lSyn = new List<string>();
                    List<int> lSFreq = new List<int>();
                    foreach (DSDict.SynRow sr in sdt)
                    {
                        lSyn.Add((string)sr.word);
                        if (sr.IsfreqNull())
                            lSFreq.Add(0);
                        else
                            lSFreq.Add((int)sr.freq);
                    }
                    llW1.Insert(index, new List<WordRu>());
                    llW1[index].Add(w);
                    llSyn1.Insert(index, lSyn);
                    llSFreq1.Insert(index, lSFreq);

                    index = lNorm.BinarySearch(w.norm);
                    if (index < 0)
                    {
                        index = ~index;
                        lNorm.Insert(index, w.norm);
                        lMatch1.Insert(index, true);
                        lMatch2.Insert(index, false);
                    }
                    foreach (string syn in lSyn)
                    {
                        index = lNorm.BinarySearch(syn);
                        if (index < 0)
                        {
                            index = ~index;
                            lNorm.Insert(index, syn);
                            lMatch1.Insert(index, true);
                            lMatch2.Insert(index, false);
                        }
                    }
                }
            }
            foreach (WordRu r in lRoot2)
            {
                st.Push(r);
                while (st.Count > 0)
                {
                    WordRu w = st.Pop();
                    int index = lNorm2.BinarySearch(w.norm);
                    if (index >= 0)
                    {
                        llW2[index].Add(w);
                        continue;
                    }
                    index = ~index;
                    lNorm2.Insert(index, w.norm);
                    if (fta.FillByWord(fdt, w.norm) > 0)
                        lFreq2.Insert(index, (int)fdt[0].freq);
                    else
                        lFreq2.Insert(index, 0);
                    sta.Fill(sdt, w.norm);
                    List<string> lSyn = new List<string>();
                    List<int> lSFreq = new List<int>();
                    foreach (DSDict.SynRow sr in sdt)
                    {
                        lSyn.Add((string)sr.word);
                        if (sr.IsfreqNull())
                            lSFreq.Add(0);
                        else
                            lSFreq.Add((int)sr.freq);
                    }
                    llW2.Insert(index, new List<WordRu>());
                    llW2[index].Add(w);
                    llSyn2.Insert(index, lSyn);
                    llSFreq2.Insert(index, lSFreq);

                    index = lNorm.BinarySearch(w.norm);
                    if (index < 0)
                    {
                        index = ~index;
                        lNorm.Insert(index, w.norm);
                        lMatch1.Insert(index, true);
                        lMatch2.Insert(index, false);
                    }
                    else
                        lMatch2[index] = true;
                    foreach (string syn in lSyn)
                    {
                        index = lNorm.BinarySearch(syn);
                        if (index < 0)
                        {
                            index = ~index;
                            lNorm.Insert(index, syn);
                            lMatch1.Insert(index, true);
                            lMatch2.Insert(index, false);
                        }
                        else
                            lMatch2[index] = true;
                    }
                }
            }

            int count = 0;
            for (int i = 0; i < lNorm1.Count; i++)
            {
                int j;
                for (j = 0; j < llSyn1[i].Count; j++)
                {
                    int k = lNorm.BinarySearch(llSyn1[i][j]);
                    if (lMatch1[k] && lMatch2[k])
                        break;
                }
                if (j == llSyn1[i].Count)
                    continue;
                int index = lNorm.BinarySearch(lNorm1[i]);
                if (!lMatch1[index] || !lMatch2[index] || llSFreq1[i][j] > lFreq1[i])
                {
                    count += llW1[i].Count;
                    foreach (WordRu w in llW1[i])
                        w.norm = llSyn1[i][j];
                }
            }
            for (int i = 0; i < lNorm2.Count; i++)
            {
                int j;
                for (j = 0; j < llSyn2[i].Count; j++)
                {
                    int k = lNorm.BinarySearch(llSyn2[i][j]);
                    if (lMatch1[k] && lMatch2[k])
                        break;
                }
                if (j == llSyn2[i].Count)
                    continue;
                int index = lNorm.BinarySearch(lNorm2[i]);
                if (!lMatch1[index] || !lMatch2[index] || llSFreq2[i][j] > lFreq2[i])
                {
                    count += llW2[i].Count;
                    foreach (WordRu w in llW2[i])
                        w.norm = llSyn2[i][j];                    
                }
            }
            return count;
        }
        static public double Compare(List<WordRu> lRoot1, List<WordRu> lRoot2)
        {
            List<string> lHash1 = new List<string>(), lHash2 = new List<string>();
            Stack<WordRu> st = new Stack<WordRu>();
            foreach (WordRu r in lRoot1)
            {
                st.Push(r);
                while (st.Count > 0)
                {
                    WordRu w = st.Pop();
                    List<string> lCh = new List<string>();
                    foreach (WordRu c in w.lCh)
                    {
                        lCh.Add(c.norm);
                        st.Push(c);
                    }
                    lCh.Sort();
                    string x = "";
                    foreach (string s in lCh)
                        x += s;
                    x += w.norm;
                    int index = lHash1.BinarySearch(x);
                    if (index < 0)
                        lHash1.Insert(~index, x);
                }
            }
            foreach (WordRu r in lRoot2)
            {
                st.Push(r);
                while (st.Count > 0)
                {
                    WordRu w = st.Pop();
                    List<string> lCh = new List<string>();
                    foreach (WordRu c in w.lCh)
                    {
                        lCh.Add(c.norm);
                        st.Push(c);
                    }
                    lCh.Sort();
                    string x = "";
                    foreach (string s in lCh)
                        x += s;
                    x += w.norm;
                    int index = lHash2.BinarySearch(x);
                    if (index < 0)
                        lHash2.Insert(~index, x);
                }
            }
            int match = 0;
            foreach (string x in lHash1)
            {
                int index = lHash2.BinarySearch(x);
                if (index >= 0)
                    match++;
            }
            return (double)match / (lHash1.Count + lHash2.Count - match);
        }
    }
    public class WordRu
    {
        public string norm, form, prep;
        public LinkTagRu lnkM, lnkD;
        public List<AttributeRu> lAt;
        public List<WordRu> lCh;
        public WordRu par;
        public WordRu(string norm, string form, LinkTagRu lnkM, LinkTagRu lnkD, List<AttributeRu> lAt, List<WordRu> lCh, WordRu par)
        {
            this.norm = norm;
            this.form = form;
            this.prep = "";
            this.lnkM = lnkM;
            this.lnkD = lnkD;
            this.lAt = lAt;
            this.lCh = lCh;
            this.par = par;       
        }
        public override string ToString()
        {
            string attr = "";
            foreach (AttributeRu a in lAt)
                attr += string.Format("{0} ", a);
            return string.Format("{0} [{1}], предлог: {2}, {3} -> {4}, {5}",
                form, norm, prep, lnkD, lnkM, attr);
        }
    }    
    public enum LinkTagRu
    {
        none, noun, pers, adj, num, adv, prepnp, prep, sent, predic, fin, skas, aux, inf, dee,
        ptp, imper, subj, acc, dat, ins, gen, conj, chto, digit, pt, by, li, koe, emph, head, misc
    }
    public enum AttributeRu
    {
        сущ = 1, глаг, прил, нар, числ, мест, прич, дееприч, междом, частица, союз, предлог, вводн_слово,
        неизм, предикатив, иностр, пунктуация, цифры, ед_ч, мн_ч, жен_р, муж_р, ср_р, одуш, им_п,
        род_п, дат_п, вин_п, тв_п, пр_п, финитн_форма, инф, перв_л, вт_л, тр_л, прош_вр, наст_вр,
        пов_накл, пусто1, изъяв_накл, актив_залог, пассив_залог, пусто2, кратк, сравн_степ, загл_буква, эвристика
    }
    #endregion

    public class Word : IComparable
    {
        public string form, norm;
        public Attribute pos;

        #region IComparable Members

        public int CompareTo(object obj)
        {
            int cmp;
            if ((cmp = (obj as Word).norm.CompareTo(norm)) == 0)
                return pos.CompareTo((obj as Word).pos);
            return cmp;
        }

        #endregion
    }
    public enum Attribute
    {
        Существительное = 1, Глагол, Прилагательное, Наречие, Числительное, Местоимение, Междометие, Частица, Союз, Предлог,
        Вводное_слово, Артикль_или_другой_определитель, Неизменяемое_слово, Пусто, Местоимение_или_определитель,
        Слово_из_другого_языка, Сокращенная_форма_служебного_глагола_с_подлежащим, Слово_из_цифр, Знак_пунктуации,
        Единственное_число, Множественное_число, Поссесивный_падеж, Финитная_форма, Инфинитив, Причастие, Прошедшее_время,
        Настоящее_время, Первое_лицо, Второе_лицо, Третье_лицо, Сравнительная_степень, Превосходная_степень,
        Слово_с_большой_буквы, Нормализовано_эвристически
    }
    static public class Compare
    {
        public static List<Word> Parse(string text)
        {
            StreamWriter sr = new StreamWriter("in.txt", false, System.Text.Encoding.Default);
            sr.Write(text);
            sr.Close();
            Process pr = Process.Start("bin\\wrf.exe", "i:in.txt o:out.xml lc:log.txt xml e w");
            pr.WaitForExit();
            FileStream fs = new FileStream("out.xml", FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            List<Word> list = new List<Word>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Word")
                {
                    int pos = int.Parse(reader.GetAttribute("POS"));
                    if (!(pos >= 1 && pos <= 13 || pos == 15 || pos == 17 || pos == 18 || pos == 24 || pos == 25))
                        continue;
                    list.Add(new Word()
                    {
                        form = reader.GetAttribute("Form"),
                        norm = reader.GetAttribute("Norm"),
                        pos = (Attribute)pos
                    });
                }
            }
            fs.Close();
            return list;
        }
        public static double SimpleCompare(List<Word> lwx, List<Word> lwy, out double[] arrPx, out double[] arrPy, out double[] arrPxy)
        {
            int[] arrI = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 17, 18, 24, 25 };
            Attribute[] arr = new Attribute[arrI.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = (Attribute)arrI[i];
            }
            SortedDictionary<Word, int>[] arrSDx = Pos(lwx), arrSDy = Pos(lwy);
            arrPx = new double[arrI.Length];
            arrPy = new double[arrI.Length];
            arrPxy = new double[arrI.Length];
            for (int i = 0; i < arrI.Length; i++)
            {
                arrPx[i] = arrSDx[i].Count / lwx.Count;
                arrPy[i] = arrSDy[i].Count / lwy.Count;
                int s = 0;
                for (int i = 0; i < arrSDx[i].Count; i++)
                {
                    int cy = 0, cx;
                    Word w = arrSDx[i].Keys.ElementAt(i);
                    arrSDy[i].TryGetValue(w, out cy);
                    if (cy != 0)
                    {
                        arrSDx[i].TryGetValue(w, out cx);
                        s += Math.Min(cx, cy);
                    }
                }

            }                
        }
        public static SortedDictionary<Word, int>[] Pos(List<Word> lw)
        {
            int[] arrI = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 17, 18, 24, 25 };
            SortedDictionary<Word, int>[] arrSD = new SortedDictionary<Word,int>[25];
            arrSD.Initialize();
            foreach (Word w in lw)
            {
                int i = (int)w.pos;
                int val;
                if (arrSD[i].TryGetValue(w, out val))
                    arrSD[i][w] = val + 1;
                else
                    arrSD[i].Add(w, 1);
            }
            List<SortedDictionary<Word, int>> lSD = new List<SortedDictionary<Word, int>>();
            foreach (int i in arrI)
            {
                lSD.Add(arrSD[i]);
            }
            return lSD.ToArray();
        }
    }
}
