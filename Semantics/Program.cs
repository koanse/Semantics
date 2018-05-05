using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Collections;
using System.Text;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using Statistics;

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
            
            //List<Word> l1 = Compare.ParseEn("Owing to the significant assimilation of various European languages throughout history, modern English contains a very large vocabulary.");
            //List<Word> l2 = Compare.ParseEn("Owing to the significant assimilation large vocabulary.");
            //double[] arrPx, arrPy, arrPxy;
            //double hx,hy,hxy,Ixy;
            //string rep1,rep2;
            //Ixy = Compare.KuznetsovCompare(l1, l2, out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out rep1);
            //Ixy = Compare.SimpleCompare(l1, l2, out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out rep2);
            //RepForm rf = new RepForm(rep1+"<br>"+rep2);
            //rf.ShowDialog();
            /*string t1 = "Historically, English originated from the fusion of languages and dialects, now collectively termed Old English, "+
                "which were brought to the eastern coast of Great Britain by Germanic (Anglo-Saxon) settlers by the 5th century – "+
                "with the word English being derived from the name of the Angles";
            string t2 = "A butterfly is a mainly day-flying insect of the order Lepidoptera, the butterflies and moths. Like other holometabolous insects, "+
                "the butterfly's life cycle consists of four parts, egg, larva, pupa and adult. Most species are diurnal. Butterflies have large, "+
                "often brightly coloured wings, and conspicuous, fluttering flight." +
                "Butterflies comprise the true butterflies (superfamily Papilionoidea)," +
                " the skippers (superfamily Hesperioidea) and the moth-butterflies (superfamily Hedyloidea)";
            List<Word>[] arrText;
            double[] arrMark;
            string[] arrReplaced;
            string rep;
            //Compare.GenerateTexts(Compare.ParseEn(t1), Compare.ParseEn(t2), out arrText, out arrMark, out arrReplaced, out rep);
            rep = Compare.EmulateEn(t1, t2);
            RepForm rf = new RepForm(rep);
            rf.ShowDialog();*/
            //MessageBox.Show(Compare.YandexFreq("fgh").ToString());
            //rf.ShowDialog();
            /*string t2 = "A butterfly is a mainly day-flying insect of the order Lepidoptera, the butterflies and moths. Like other holometabolous insects, " +
                "the butterfly's life cycle consists of four parts, egg, larva, pupa and adult. Most species are diurnal. Butterflies have large, " +
                "often brightly coloured wings, and conspicuous, fluttering flight." +
                "Butterflies comprise the true butterflies (superfamily Papilionoidea)," +
                " the skippers (superfamily Hesperioidea) and the moth-butterflies (superfamily Hedyloidea)";
            List<Word>[] arrSent = Compare.ParseBySentencesEn(t2);*/
            string t = "Следует отметить, что слово «фрактал» не является математическим термином и не имеет общепринятого строгого математического определения.";
            string t2 = "Существует простая рекурсивная процедура получения фрактальных кривых на плоскости. Зададим произвольную ломаную с конечным числом звеньев, называемую генератором.";
            string t3 = "По виду решаемой задачи можно выделить следующие разделы математического программирования: 1. Линейное программирование – раздел математического программирования, изучающий задачу поиска минимальной (максимальной) линейной функции при линейных ограничениях в виде равенств или неравенств. 2. Нелинейное программирование – раздел математического программирования, изучающий методы решения и характер экстремума в задачах оптимизации с нелинейной целевой функцией и (или) нелинейными ограничениями. 3. Стохастическое программирование - раздел математического программирования, изучающий модели выбора оптимальных решений в ситуациях, характеризуемых случайными величинами.";
            string t4 = "Закон переименовывает основной правоохранительный орган России из милиции в полицию и оговаривает основные принципы его деятельности, меры общественного контроля над ним, права и обязанности полицейских.";
            string rep="";
            Word[] arrW;
            double[] arrI;
            //Compare.WordInfo(Compare.ParseBySentencesRu(t), out arrW, out arrI, out rep);
            //Compare.SaveCache();
            //RepForm rf = new RepForm(rep);
            //rf.ShowDialog();
            //Compare.ImportAns();
            double[] arrTM,arrM;
            Compare.Initialize();
            Compare.LoadCache();            
            //Compare.SaveCache();
            //Compare.CheckCache();
            //Compare.SaveCache();
            //Compare.SaveCache();
            //Compare.contextLen = 4;
            //Compare.WordInfo2(Compare.ParseBySentencesRu(t4), out arrW, out arrI,out rep);
            //Compare.language = "en";
            //Compare.contextLen = 5;
            Compare.CheckProbCache();
            
            //Compare.IMinThreshold = 2.9;
            //Compare.IMaxThreshold = 14.4;
            Compare.language = "en";
            //Compare.cacheW.Clear();
            //Compare.cacheSent.Clear();
            rep = Compare.EmulateRu(@"C:\Users\Anton\Documents\Visual Studio 2010\Projects\Semantics\Semantics\bin\Debug\cmp100");
            //rep = Compare.OptimizeBoolMinMax(@"C:\Users\Anton\Documents\Visual Studio 2010\Projects\Semantics\Semantics\bin\Debug\cmpEn");
            rep += Compare.log;
            Compare.SaveCache();
            //rep = Compare.NigmaFreq("следующие разделы математического").ToString();
            RepForm rf = new RepForm(rep);
            rf.ShowDialog();
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
    #endregion

    [Serializable]
    public class Word : IComparable
    {
        public string form, norm;
        public string pos;

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return norm.CompareTo((obj as Word).norm);
        }

        #endregion
    }
    public enum AttributeEn
    {
        Существительное = 1, Глагол, Прилагательное, Наречие, Числительное, Местоимение, Междометие, Частица, Союз, Предлог,
        Вводное_слово, Артикль_или_другой_определитель, Неизменяемое_слово, Пусто, Местоимение_или_определитель,
        Слово_из_другого_языка, Сокращенная_форма_служебного_глагола_с_подлежащим, Слово_из_цифр, Знак_пунктуации,
        Единственное_число, Множественное_число, Поссесивный_падеж, Финитная_форма, Инфинитив, Причастие, Прошедшее_время,
        Настоящее_время, Первое_лицо, Второе_лицо, Третье_лицо, Сравнительная_степень, Превосходная_степень,
        Слово_с_большой_буквы, Нормализовано_эвристически
    }
    public enum AttributeRu
    {
        сущ = 1, глаг, прил, нар, числ, мест, прич, дееприч, междом, частица, союз, предлог, вводн_слово,
        неизм, предикатив, иностр, пунктуация, цифры, ед_ч, мн_ч, жен_р, муж_р, ср_р, одуш, им_п,
        род_п, дат_п, вин_п, тв_п, пр_п, финитн_форма, инф, перв_л, вт_л, тр_л, прош_вр, наст_вр,
        пов_накл, пусто1, изъяв_накл, актив_залог, пассив_залог, пусто2, кратк, сравн_степ, загл_буква, эвристика
    }
    static public class Compare
    {
        public delegate int FreqCalc(string text);
        public static double nVoc = 125000;
        public static double IMin=1.6,IMax=9.2;
        public static double IDefault = 3;
        public static bool IInfiniteIsIDefault = true;
        public static double IMinThreshold=0.8, IMaxThreshold = 8.9;
        public static int contextLen = 3,intervalCount=11;
        public static string language = "ru";
        public static bool[] arrAvailable=new bool[] { true,true,true,true,true,true};
        public static FreqCalc[] arrFreqCalc = new FreqCalc[] { new FreqCalc(YandexFreq), new FreqCalc(YandexXmlFreq),
            new FreqCalc(GoogleFreq),new FreqCalc(BingFreq), new FreqCalc(YahooFreq),new FreqCalc(NigmaFreq) };
        public static string log="Ошибки поисковых серверов<br>";
        public static SortedDictionary<string, List<Word>> cacheW;
        public static SortedDictionary<string, List<Word>[]> cacheSent;
        public static SortedDictionary<string, double> cacheP;
        public static List<string> lOpenedPos = new List<string>(new string[]
        {
            "Существительное", "Глагол", "Прилагательное", "Наречие", "Числительное", "Неизменяемое_слово", "Слово_из_другого_языка",
                               "Сокращенная_форма_служебного_глагола_с_подлежащим", "Слово_с_большой_буквы", "Нормализовано_эвристически",
                               "сущ", "глаг", "прил", "нар", "прич", "дееприч", "неизм", "предикатив", "иностр", "загл_буква", "эвристика"
        });
        public static void Initialize()
        {
            lOpenedPos.Sort();
        }            
        public static List<Word> Parse(string text)
        {
            if (cacheW.ContainsKey(text))
                return cacheW[text];
            if (language == "ru")
            {
                StreamWriter sr = new StreamWriter("in.txt", false, System.Text.Encoding.Default);
                sr.Write(text);
                sr.Close();
                Process pr = Process.Start("bin\\wrf.exe", "i:in.txt o:out.xml lc:log.txt xml w");
                pr.WaitForExit();
                FileStream fs = new FileStream("out.xml", FileMode.Open);
                XmlReader reader = XmlReader.Create(fs);
                List<Word> list = new List<Word>();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Word" &&
                        char.IsLetterOrDigit(reader.GetAttribute("Form")[0]))
                    {
                        int pos = int.Parse(reader.GetAttribute("POS"));
                        list.Add(new Word()
                        {
                            form = reader.GetAttribute("Form"),
                            norm = reader.GetAttribute("Norm"),
                            pos = ((AttributeRu)pos).ToString()
                        });
                    }
                }
                fs.Close();
                cacheW.Add(text, list);
                return list;
            }
            else
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
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Word" &&
                        char.IsLetterOrDigit(reader.GetAttribute("Form")[0]))
                    {
                        int pos = int.Parse(reader.GetAttribute("POS"));
                        list.Add(new Word()
                        {
                            form = reader.GetAttribute("Form"),
                            norm = reader.GetAttribute("Norm"),
                            pos = ((AttributeEn)pos).ToString()
                        });
                    }
                }
                fs.Close();
                cacheW.Add(text, list);
                return list;
            }
        }
        public static List<Word>[] ParseBySentences(string text)
        {
            if (cacheSent.ContainsKey(text))
                return cacheSent[text];
            if (language == "ru")
            {
                StreamWriter sr = new StreamWriter("in.txt", false, System.Text.Encoding.Default);
                sr.Write(text);
                sr.Close();
                Process pr = Process.Start("bin\\wrf.exe", "i:in.txt o:out.xml lc:log.txt xml w");
                pr.WaitForExit();
                FileStream fs = new FileStream("out.xml", FileMode.Open);
                XmlReader reader = XmlReader.Create(fs);
                List<List<Word>> lSent = new List<List<Word>>();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Sentence")
                        lSent.Add(new List<Word>());
                    else if (reader.NodeType == XmlNodeType.Element && reader.Name == "Word")
                    {
                        int pos = int.Parse(reader.GetAttribute("POS"));
                        string norm = reader.GetAttribute("Norm").Replace(",", "").Replace(" ", "");
                        if (norm ==""|| !char.IsLetterOrDigit(norm[0]))
                            continue;
                        lSent.Last().Add(new Word()
                        {
                            form = reader.GetAttribute("Form"),
                            norm = norm,
                            pos = ((AttributeRu)pos).ToString()
                        });
                    }
                }
                fs.Close();
                cacheSent.Add(text, lSent.ToArray());
                return lSent.ToArray();
            }
            else
            {
                StreamWriter sr = new StreamWriter("in.txt", false, System.Text.Encoding.Default);
                sr.Write(text);
                sr.Close();
                Process pr = Process.Start("bin\\wrf.exe", "i:in.txt o:out.xml lc:log.txt xml e w");
                pr.WaitForExit();
                FileStream fs = new FileStream("out.xml", FileMode.Open);
                XmlReader reader = XmlReader.Create(fs);
                List<List<Word>> lSent = new List<List<Word>>();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Sentence")
                        lSent.Add(new List<Word>());
                    else if (reader.NodeType == XmlNodeType.Element && reader.Name == "Word")
                    {
                        int pos = int.Parse(reader.GetAttribute("POS"));
                        string norm = reader.GetAttribute("Norm").Replace(",", "").Replace(" ", "");
                        if (norm==""||!char.IsLetterOrDigit(norm[0]))
                            continue;
                        lSent.Last().Add(new Word()
                        {
                            form = reader.GetAttribute("Form"),
                            norm = reader.GetAttribute("Norm"),
                            pos = ((AttributeEn)pos).ToString()
                        });
                    }
                }
                fs.Close();
                cacheSent.Add(text, lSent.ToArray());
                return lSent.ToArray();
            }
        }
        public static int YandexFreq(string text)
        {
            string q = "http://yandex.ru/yandsearch?text="+UTF8Query(text);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string s = wc.DownloadString(q);
            int freq = 0;
            if (!s.Contains("Искомая комбинация слов нигде не встречается")&&
                !s.Contains("Задан пустой поисковый запрос"))
            {
                try
                {
                    int pos = s.IndexOf("<strong class=\"b-head-logo__text\">") + "<strong class=\"b-head-logo__text\">".Length;
                    string count = "";
                    while (s.Substring(pos, 3) != "тыс" && s.Substring(pos, 3) != "млн" && s.Substring(pos, 9) != "</strong>")
                    {
                        if (char.IsDigit(s[pos])||s[pos]==',')
                            count += s[pos];
                        pos++;
                    }
                    long lf = long.Parse(count);
                    if (s.Substring(pos, 3) == "тыс")
                        lf *= 1000;
                    else if (s.Substring(pos, 3) == "млн")
                        lf *= 1000000;
                    if (lf > int.MaxValue)
                        freq = int.MaxValue;
                    else
                        freq = (int)lf;
                }
                catch (Exception e)
                {
                    freq = -1;
                    log += string.Format("{0}, Яндекс<br>Запрос: {1}<br>Ошибка: {2}<br><br>", DateTime.Now, text, e.Message);
                }
            }
            return freq;
        }
        public static int BingFreq(string text)
        {
            string q = "http://www.bing.com/search?q=" + UTF8Query(text);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string s = wc.DownloadString(q);
            int freq = 0;
            if (!s.Contains("Попробуйте сформулировать запрос иначе или использовать синонимы"))
            {
                try
                {
                    int pos = s.IndexOf("Результаты: 1 —") + "Результаты: 1 —".Length;
                    while (s.Substring(pos, 2) != "из")
                        pos++;
                    string count = "";
                    while (s.Substring(pos, 7) != "</span>")
                    {
                        if (s.Substring(pos, 6) == "&#160;")
                            pos += 6;
                        if (char.IsDigit(s[pos]))
                            count += s[pos];
                        pos++;
                    }
                    long lf = long.Parse(count);
                    if (lf > int.MaxValue)
                        freq = int.MaxValue;
                    else
                        freq = (int)lf;
                }
                catch (Exception e)
                {
                    freq = -1;
                    log += string.Format("{0}, Bing<br>Запрос: {1}<br>Ошибка: {2}<br><br>", DateTime.Now, text, e.Message);
                }
            }
            return freq;
        }
        public static int NigmaFreq(string text)
        {
            string q = "http://www.nigma.ru/?s=" + UTF8Query(text);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            int freq = 0;
            try
            {
                string s = wc.DownloadString(q);
                if (!s.Contains("Результатов не найдено"))
                {
                    int pos = s.IndexOf("<table id=\"results_count\">") + "<table id=\"results_count\">".Length;
                    string count = "";
                    while (s.Substring(pos, 3) != "тыс" && s.Substring(pos, 3) != "млн" && s.Substring(pos, 7) != "</span>")
                    {
                        if (char.IsDigit(s[pos]) || s[pos] == '|')
                            count += s[pos];
                        pos++;
                    }
                    long lf = long.Parse(count);
                    if (s.Substring(pos, 3) == "тыс")
                        lf *= 1000;
                    else if (s.Substring(pos, 3) == "млн")
                        lf *= 1000000;
                    if (lf > int.MaxValue)
                        freq = int.MaxValue;
                    else
                        freq = (int)lf;
                }
            }
            catch (Exception e)
            {
                freq = -1;
                log += string.Format("{0}, Nigma<br>Запрос: {1}<br>Ошибка: {2}<br><br>", DateTime.Now, text, e.Message);
            }
            return freq;
        }
        public static int YahooFreq(string text)
        {
            string q = "http://search.yahoo.com/search?p=" + UTF8Query(text);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            int freq = 0;
            try
            {

                string s = wc.DownloadString(q);
                int pos = s.IndexOf("<strong id=\"resultCount\">") + "<strong id=\"resultCount\">".Length;
                string count = "";
                while (s.Substring(pos, 9) != "</strong>")
                {
                    if (char.IsDigit(s[pos]))
                        count += s[pos];
                    pos++;
                }
                long lf = long.Parse(count);
                if (lf > int.MaxValue)
                    freq = int.MaxValue;
                else
                    freq = (int)lf;
            }
            catch (Exception e)
            {
                freq = -1;
                log += string.Format("{0}, Yahoo<br>Запрос: {1}<br>Ошибка: {2}<br><br>", DateTime.Now, text, e.Message);
            }
            return freq;
        }
        public static int GoogleFreq(string text)
        {
            string q = "http://www.google.ru/search?q=" + UTF8Query(text);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.Default;
            int freq = 0;
            try
            {
                string s = wc.DownloadString(q);
                if (!s.Contains("Искомая комбинация слов нигде не встречается") &&
                     !s.Contains("Нет результатов для") &&
                    !s.Contains("Не найдено ни одного документа"))
                {
                    int pos = s.IndexOf("<div id=resultStats>") + "<div id=resultStats>".Length;
                    string count = "";
                    while (s.Substring(pos, 6) != "<nobr>")
                    {
                        if (char.IsDigit(s[pos]))
                            count += s[pos];
                        pos++;
                    }
                    long fl = long.Parse(count);
                    if (fl > int.MaxValue)
                        freq = int.MaxValue;
                    else
                        freq = (int)fl;                    
                }
            }
            catch (Exception e)
            {
                freq = -1;
                log += string.Format("{0}, Google<br>Запрос: {1}<br>Ошибка: {2}<br><br>", DateTime.Now, text, e.Message);
            }
            return freq;
        }
        public static int YandexXmlFreq(string text)
        {
            string q = "http://xmlsearch.yandex.ru/xmlsearch?user=coant&key=03.84849253:7e855e3361b44d76ba7ae5d98cd5a387&query="+UTF8Query(text);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string s = wc.DownloadString(q);
            int freq = 0;
            if (!s.Contains("нигде не встречается"))
            {
                {
                    try
                    {
                        int pos = s.IndexOf("<found priority=\"phrase\">") + "<found priority=\"phrase\">".Length;
                        string count = "";
                        while (char.IsDigit(s[pos]))
                        {
                            count += s[pos++];
                        }
                        long lf = long.Parse(count);
                        if (lf > int.MaxValue)
                            freq = int.MaxValue;
                        else
                            freq = (int)lf;
                    }
                    catch (Exception e)
                    {
                        freq = -1;
                        log += string.Format("{0}, Яндекс XML<br>Запрос: {1}<br>Ошибка: {2}<br><br>", DateTime.Now, text, e.Message);
                    }
                }
            }
            return freq;
        }
        public static string UTF8Query(string text)
        {
            string q = "%22";
            Encoder enc = Encoding.UTF8.GetEncoder();
            char[] arrC = text.ToArray();
            byte[] arrB = new byte[6];
            for (int i = 0; i < arrC.Length; i++)
            {
                if (arrC[i] == ' ')
                {
                    q += "+";
                    continue;
                }
                bool completed;
                int cUsed, bUsed;
                enc.Convert(arrC, i, 1, arrB, 0, 6, true, out cUsed, out bUsed, out completed);
                if (bUsed == 1)
                    q += arrC[i];
                else
                    for (int j = 0; j < bUsed; j++)
                    {
                        q += string.Format("%{0:X2}", arrB[j]);
                    }
            }
            q += "%22";
            return q;
        }
        public static double[] Prob(string[] arrText)
        {
            SortedDictionary<string, double> sd = new SortedDictionary<string, double>();
            List<string> lText = new List<string>();
            foreach (string s in arrText)
            {
                if (cacheP.ContainsKey(s))
                {
                    if (!sd.ContainsKey(s))
                        sd.Add(s, cacheP[s]);
                }
                else
                {
                    int i = lText.BinarySearch(s);
                    if (i < 0)
                        lText.Insert(~i, s);
                }
            }
            Random rnd = new Random();
            List<int> lI = new List<int>();
            for (int i = 0; i < arrAvailable.Length; i++)
            {
                if (arrAvailable[i])
                    lI.Add(i);
            }
            do
            {
                if (lI.Count == 0)
                    break;
                List<List<string>> llText = new List<List<string>>();
                for (int i = 0; i < lI.Count; i++)
                {
                    llText.Add(new List<string>());
                }
                while (lText.Count > 0)
                {
                    string s = lText[0];
                    lText.RemoveAt(0);
                    int i = rnd.Next(0, lI.Count);
                    llText[i].Add(s);
                }
                while (lI.Count > 0)
                {
                    List<int> lRemove = new List<int>();
                    for (int i = 0; i < lI.Count; i++)
                    {
                        if (llText[i].Count == 0)
                            continue;
                        int j = rnd.Next(0, llText[i].Count);
                        string[] arrT = llText[i][j].Split('|');
                        int m = arrFreqCalc[lI[i]](arrT[0]), n = arrFreqCalc[lI[i]](arrT[1]);
                        if (m >= 0&&n>=0)
                        {
                            if (m > n)
                                m = n = 1;
                            sd.Add(llText[i][j], (double)m/n);
                            cacheP.Add(llText[i][j], (double)m / n);
                            llText[i].RemoveAt(j);
                        }
                        else
                        {
                            lRemove.Add(i);
                            lText.AddRange(llText[i]);
                            arrAvailable[lI[i]] = false;
                        }
                    }
                    lRemove.Reverse();
                    foreach (int i in lRemove)
                    {
                        lI.RemoveAt(i);
                        llText.RemoveAt(i);
                    }
                    int count = 0;
                    foreach (List<string> list in llText)
                    {
                        count += list.Count;
                    }
                    if (count == 0)
                        break;
                }
            }
            while (lText.Count > 0);

            foreach (string s in lText)
            {
                cacheP.Add(s, -1);
                sd.Add(s, -1);
            }
            double[] arr = new double[arrText.Length];
            for (int i = 0; i < arrText.Length; i++)
            {
                arr[i] = sd[arrText[i]];
            }
            return arr;
        } 
        public static void WordInfo(List<Word>[] arrSent, out Word[] arrWord, out double[] arrInfo, out string rep)
        {
            List<Word> lw = new List<Word>();
            List<double> lInfo = new List<double>();
            List<string> lContext = new List<string>();
            List<string> lCW = new List<string>();
            List<string> lText = new List<string>();
            rep = "<table border=1 cellspacing=0><tr><td>Слово W<td>Контекст C<td>P(W/C)<td>Количество информации I(W/C), бит";
            for (int i = 0; i < arrSent.Length; i++)
            {
                for (int j = 0; j < arrSent[i].Count; j++)
                {
                    string context = "", s;
                    if (j >= contextLen)
                    {
                        for (int k = 0; k < contextLen; k++)
                        {
                            context += arrSent[i][j - contextLen + k].form + " ";
                        }
                        s = context + " " + arrSent[i][j].form;
                    }
                    else
                    {
                        for (int k = 0; k < contextLen && j + k + 1 < arrSent[i].Count; k++)
                        {
                            context += arrSent[i][j + k + 1].form + " ";
                        }
                        s = arrSent[i][j].form + " " + context;
                    }
                    lContext.Add(context);
                    lCW.Add(s);
                    lText.Add(s+"|"+context);
                    lw.Add(arrSent[i][j]);
                }
            }
            double[] arrP = Prob(lText.ToArray());
            for (int i = 0; i < lw.Count; i++)
            {
                double I=Math.Log(1 / arrP[i], 2);
                if (arrP[i]==-1||double.IsNaN(arrP[i])||(IInfiniteIsIDefault&&arrP[i]==0))
                    I = IDefault;
                lInfo.Add(I);
                rep += string.Format("<tr><td>{0}<td>{1}<td>{2:g4}<td>{3:g4}",
                    lw[i].form, lContext[i], arrP[i], I);
            }
            arrWord = lw.ToArray();
            arrInfo = lInfo.ToArray();
            rep += "</table>";
        }
        public static double KuznetsovCompare(SortedDictionary<Word, int>[] arrSDx, SortedDictionary<Word, int>[] arrSDy,
            out double[] arrPx, out double[] arrPy, out double[] arrPxy,
            out double hx, out double hy, out double hxy, out string rep)
        {
            string[] arr = new string[arrSDx.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                if (arrSDx[i].Count > 0)
                    arr[i] = arrSDx[i].Keys.First().pos;
                else if (arrSDy[i].Count > 0)
                    arr[i] = arrSDy[i].Keys.First().pos;
                else
                    arr[i] = "нет";
            }            
            SortedDictionary<Word, int>[] arrSDxy = new SortedDictionary<Word, int>[arr.Length];
            for (int i = 0; i < arrSDxy.Length; i++)
            {
                arrSDxy[i] = new SortedDictionary<Word, int>();
                Word[] arrKey = arrSDx[i].Keys.ToArray();
                int[] arrVal = arrSDx[i].Values.ToArray();
                for (int j = 0; j < arrKey.Length; j++)
                {
                    arrSDxy[i].Add(arrKey[j], arrVal[j]);
                }
            }

            arrPx = new double[arr.Length];
            arrPy = new double[arr.Length];
            arrPxy = new double[arr.Length];
            int nx = 0, ny = 0, nxy = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                int n = 0;
                int[] arrVal = arrSDx[i].Values.ToArray();
                for (int j = 0; j < arrVal.Length; j++)
                {
                    n += arrVal[j];
                }
                arrPx[i] = n;
                nx += n;
                n = 0;
                arrVal = arrSDy[i].Values.ToArray();
                for (int j = 0; j < arrVal.Length; j++)
                {
                    n += arrVal[j];
                }
                arrPy[i] = n;
                ny += n;

                Word[] arrKey = arrSDxy[i].Keys.ToArray();
                arrVal = arrSDxy[i].Values.ToArray();
                List<Word> lRemove = new List<Word>();
                for (int j = 0; j < arrKey.Length; j++)
                {
                    int val;
                    if (arrSDy[i].TryGetValue(arrKey[j], out val))
                        arrSDxy[i][arrKey[j]] = Math.Min(arrVal[j], val);
                    else
                        lRemove.Add(arrKey[j]);
                }
                foreach (Word w in lRemove)
                {
                    arrSDxy[i].Remove(w);
                }
            }
            
            for (int i = 0; i < arr.Length; i++)
            {
                arrPx[i] /= nx;
                arrPy[i] /= ny;
            }
            for (int i = 0; i < arr.Length; i++)
            {
                int n = 0;
                int[] arrVal = arrSDxy[i].Values.ToArray();
                for (int j = 0; j < arrVal.Length; j++)
                {
                    n += arrVal[j];
                }
                arrPxy[i] = n;
                nxy += n;
            }
            nxy = nx + ny - nxy;
            for (int i = 0; i < arr.Length; i++)
            {
                arrPxy[i] /= nxy;
            }
            hx = hy = hxy = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arrPx[i] > 0)
                    hx += -arrPx[i] * Math.Log(arrPx[i]);
                if (arrPy[i] > 0)
                    hy += -arrPy[i] * Math.Log(arrPy[i]);
                if (arrPxy[i] > 0)
                    hxy += -arrPxy[i] * Math.Log(arrPxy[i]);
            }

            rep = "ТекстX<table border = 1 cellspacing = 0><tr>";
            for (int i = 0; i < arr.Length; i++)
            {
                if (arrPx[i] == 0)
                    continue;
                    rep += string.Format("<td>{0}", arr[i]);
            }
            rep += "<tr>";
            for (int i = 0; i < arr.Length; i++)
            {
                if (arrPx[i] == 0)
                    continue;
                rep += "<td>";
                Word[] arrKey = arrSDx[i].Keys.ToArray();
                int[] arrVal = arrSDx[i].Values.ToArray();
                for (int j = 0; j < arrKey.Length; j++)
                {
                    rep += string.Format("{0} - {1}<br>", arrKey[j].norm, arrVal[j]);
                }
            }
            rep += "<tr>";
            for (int i = 0; i < arrPx.Length; i++)
            {
                if (arrPx[i] == 0)
                    continue;
                rep += string.Format("<td>P({0}) = {1:g4}", arr[i].ToString().Substring(0,3), arrPx[i]);
            }
            rep += "</table>";

            rep+= "ТекстY<table border = 1 cellspacing = 0><tr>";
            for (int i = 0; i < arr.Length; i++)
            {
                if (arrPy[i] == 0)
                    continue;
                rep += string.Format("<td>{0}", arr[i]);
            }
            rep += "<tr>";
            for (int i = 0; i < arr.Length; i++)
            {
                if (arrPy[i] == 0)
                    continue;
                rep += "<td>";
                Word[] arrKey = arrSDy[i].Keys.ToArray();
                int[] arrVal = arrSDy[i].Values.ToArray();
                for (int j = 0; j < arrKey.Length; j++)
                {
                    rep += string.Format("{0} - {1}<br>", arrKey[j].norm, arrVal[j]);
                }
            }
            rep += "<tr>";
            for (int i = 0; i < arrPy.Length; i++)
            {
                if (arrPy[i] == 0)
                    continue;
                rep += string.Format("<td>P({0}) = {1:g4}", arr[i].ToString().Substring(0, 3), arrPy[i]);
            }
            rep += "</table>";

            rep += "Совместное распределение XY<table border = 1 cellspacing = 0><tr>";
            for (int i = 0; i < arr.Length; i++)
            {
                if (arrPxy[i] == 0)
                    continue;
                rep += string.Format("<td>{0}", arr[i]);
            }
            rep += "<tr>";
            for (int i = 0; i < arr.Length; i++)
            {
                if (arrPxy[i] == 0)
                    continue;
                rep += "<td>";
                Word[] arrKey = arrSDxy[i].Keys.ToArray();
                int[] arrVal = arrSDxy[i].Values.ToArray();
                for (int j = 0; j < arrKey.Length; j++)
                {
                    rep += string.Format("{0} - {1}<br>", arrKey[j].norm, arrVal[j]);
                }
            }
            rep += "<tr>";
            for (int i = 0; i < arrPxy.Length; i++)
            {
                if (arrPxy[i] == 0)
                    continue;
                rep += string.Format("<td>P({0}) = {1:g4}", arr[i].ToString().Substring(0, 3), arrPxy[i]);
            }
            rep += "</table>";
            rep += string.Format("I<sub>xy</sub> = H<sub>x</sub> + H<sub>y</sub> - H<sub>xy</sub> = {0:g5} + {1:g5} - {2:g5} = {3:g5}<br>",
                hx, hy, hxy, hx + hy - hxy);            
            return hx + hy - hxy;
        }
        public static double BoolCompare(List<Word> lwx, List<Word> lwy,
            out double[] arrPx, out double[] arrPy, out double[] arrPxy, out double hx, out double hy, out double hxy, out string rep)
        {
            List<Word> lx = new List<Word>(), ly = new List<Word>();
            foreach (Word w in lwx)
            {
                int i = lx.BinarySearch(w);
                if (i < 0)
                    lx.Insert(~i, w);
            }
            foreach (Word w in lwy)
            {
                int i = ly.BinarySearch(w);
                if (i < 0)
                    ly.Insert(~i, w);
            }
            rep = "<table border = 1 cellspacing = 0><tr><td>Слово<td>X<sub>A</sub><td>X<sub>B</sub>";
            int n10 = 0, n01, n11 = 0, n00;
            foreach (Word w in lx)
            {
                int i = ly.BinarySearch(w);
                if (i >= 0)
                {
                    n11++;
                    ly.RemoveAt(i);
                    rep += string.Format("<tr><td>{0}<td>1<td>1", w.norm);
                }
                else
                {
                    n10++;
                    rep += string.Format("<tr><td>{0}<td>1<td>0", w.norm);
                }
            }
            n01 = ly.Count;
            foreach (Word w in ly)
            {
                rep += string.Format("<tr><td>{0}<td>0<td>1", w.norm);
            }
            rep += "</table>";
            n00 = (int)nVoc - n10 - n01 - n11;
            //nVoc = n11 + n01 + n10;
            arrPx = new double[] { (n00 + n01) / nVoc, (n10 + n11) / nVoc };
            arrPy = new double[] { (n10 + n00) / nVoc, (n01 + n11) / nVoc };
            arrPxy = new double[] { n00 / nVoc, n01 / nVoc, n10 / nVoc, n11 / nVoc };
            hx = hy = hxy = 0;
            for (int i = 0; i < arrPx.Length; i++)
            {
                if (arrPx[i] > 0)
                {
                    hx += -arrPx[i] * Math.Log(arrPx[i]);
                }
                if (arrPy[i] > 0)
                    hy += -arrPy[i] * Math.Log(arrPy[i]);
            }
            for (int i = 0; i < arrPxy.Length; i++)
            {
                if (arrPxy[i] > 0)
                    hxy += -arrPxy[i] * Math.Log(arrPxy[i]);
            }
            rep += "<table border = 1 cellspacing = 0><tr><td>X<sub>A</sub>\\X<sub>B</sub><td>0<td>1";
            rep += string.Format("<tr><td>0<td>{0:g4}<td>{1:g4}<tr><td>1<td>{2:g4}<td>{3:g4}", arrPxy[0], arrPxy[1], arrPxy[2], arrPxy[3]);
            rep += "</table>";
            rep += string.Format("I<sub>AB</sub> = H<sub>A</sub> + H<sub>B</sub> - H<sub>AB</sub> = {0:g5} + {1:g5} - {2:g5} = {3:g5}<br>",
                hx, hy, hxy, hx + hy - hxy);
            rep += string.Format("I<sub>AB</sub> / H<sub>A</sub> = {0:g5} / {1:g5} = {2:g5}<br>",
                hx + hy - hxy, hx, (hx + hy - hxy)/hx);
            return hx + hy - hxy;
        }
        public static double BoolPosCompare(List<Word> lwx, List<Word> lwy, out string rep)
        {
            List<Word> lwxOpen = new List<Word>(), lwyOpen = new List<Word>();
            for (int i = 0; i < lwx.Count; i++)
            {
                if (IsOpenedPos(lwx[i].pos))
                {
                    int j = lwxOpen.BinarySearch(lwx[i]);
                    if (j < 0)
                        lwxOpen.Insert(~j, lwx[i]);
                }
            }
            for (int i = 0; i < lwy.Count; i++)
            {
                if (IsOpenedPos(lwy[i].pos))
                {
                    int j = lwyOpen.BinarySearch(lwy[i]);
                    if (j < 0)
                        lwyOpen.Insert(~j, lwy[i]);
                }
            }
            double[] arrPx, arrPy, arrPxy;
            double hx, hy, hxy, I;
            I = BoolCompare(lwxOpen, lwyOpen, out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out rep) / hx;
            return I;
        }
        public static double BoolInfMinPosCompare(Word[] arrWX, Word[] arrWY, double[] arrIX, double[] arrIY, out string rep)
        {
            rep= string.Format("Порог значимости слова: {0}<br>", IMinThreshold);
            List<Word> lwx = new List<Word>(), lwy = new List<Word>();
            for (int i = 0; i < arrWX.Length; i++)
            {
                if (arrIX[i] >= IMinThreshold && IsOpenedPos(arrWX[i].pos))
                {
                    int j = lwx.BinarySearch(arrWX[i]);
                    if (j < 0)
                        lwx.Insert(~j, arrWX[i]);
                }
            }
            for (int i = 0; i < arrWY.Length; i++)
            {
                if (arrIY[i] >= IMinThreshold && IsOpenedPos(arrWY[i].pos))
                {
                    int j = lwy.BinarySearch(arrWY[i]);
                    if (j < 0)
                        lwy.Insert(~j, arrWY[i]);
                }
            }
            double[] arrPx, arrPy, arrPxy;
            double hx, hy, hxy, I;
            string s;
            I = BoolCompare(lwx, lwy, out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out s)/hx;
            rep+=s;
            return I;
        }
        public static double BoolInfMinMaxPosCompare(Word[] arrWX, Word[] arrWY, double[] arrIX, double[] arrIY, out string rep)
        {
            rep = string.Format("Интервал значимости слова: [{0};{1}]<br>", IMinThreshold, IMaxThreshold);
            List<Word> lwx = new List<Word>(), lwy = new List<Word>();
            for (int i = 0; i < arrWX.Length; i++)
            {
                if (arrIX[i] >= IMinThreshold && IsOpenedPos(arrWX[i].pos) && arrIX[i] <= IMaxThreshold)
                {
                    int j = lwx.BinarySearch(arrWX[i]);
                    if (j < 0)
                        lwx.Insert(~j, arrWX[i]);
                }
            }
            for (int i = 0; i < arrWY.Length; i++)
            {
                if (arrIY[i] >= IMinThreshold && IsOpenedPos(arrWY[i].pos) && arrIY[i] <= IMaxThreshold)
                {
                    int j = lwy.BinarySearch(arrWY[i]);
                    if (j < 0)
                        lwy.Insert(~j, arrWY[i]);
                }
            }
            double[] arrPx, arrPy, arrPxy;
            double hx, hy, hxy, I;
            string s;
            I = BoolCompare(lwx, lwy, out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out s) / hx;
            rep += s;
            return I;
        }        
        public static double BoolDiscInfVectorCompare(Word[] arrWX, Word[] arrWY, double[] arrIX, double[] arrIY, out string rep)
        {
            // X,Y - дискр. кол. инф. в слове
            double len;
            SortedDictionary<Word, int> sdX= DiscrInfVector(arrWX,arrIX,out len);
            SortedDictionary<Word, int> sdY = DiscrInfVector(arrWY, arrIY, out len);            
            arrWX = sdX.Keys.ToArray();
            int[] arrInd = sdX.Values.ToArray();
            int count=0;
            double[,] arrPxy = new double[intervalCount, intervalCount];
            rep = "<table cellspacing=0 border=1><tr><td>Слово<td>Дискр. кол. инф. X<td>Дискр. кол. инф. Y";
            for (int i = 0; i < arrWX.Length; i++)
            {
                int jY = 0;
                if (sdY.ContainsKey(arrWX[i]))
                    jY = sdY[arrWX[i]];
                arrPxy[arrInd[i], jY]++;
                count++;
                rep += string.Format("<tr><td>{0}<td>{1}<td>{2}", arrWX[i].norm, arrInd[i], jY);
            }
            arrWY = sdY.Keys.ToArray();
            arrInd = sdY.Values.ToArray();
            for (int i = 0; i < arrWY.Length; i++)
            {
                arrPxy[0,arrInd[i]]++;
                count++;
                rep += string.Format("<tr><td>{0}<td>{1}<td>{2}", arrWY[i].norm, 0,arrInd[i]);
            }
            double[] arrPx=new double[intervalCount], arrPy=new double[intervalCount];
            double hxy=0;
            for (int i = 0; i < intervalCount; i++)
            {
                for (int j = 0; j < intervalCount; j++)
                {
                    arrPxy[i, j] /= count;
                    arrPx[i] += arrPxy[i, j];
                    arrPy[j] += arrPxy[i, j];
                    if (arrPxy[i,j] > 0)
                    hxy += arrPxy[i, j] * Math.Log(1 / arrPxy[i, j]);
                }                
            }
            double hx=0, hy=0,I;
            for (int i = 0; i < intervalCount; i++)
            {
                if(arrPx[i]>0)
                    hx+=arrPx[i] * Math.Log(1 / arrPx[i]);
                if (arrPy[i] > 0)
                hy += arrPy[i] * Math.Log(1 / arrPy[i]);
            }
            I = hx + hy - hxy;
            rep += string.Format("</table>I = {0:g4} + {1:g4} - {2:g4} = {3:g4}<br>", hx, hy, hxy, I);
            rep += string.Format("I / H(x) = {0:g4}<br>", I/hx);
            return I/hx;
        }
        public static double BoolDiscInfVectorPosCompare(Word[] arrWX, Word[] arrWY, double[] arrIX, double[] arrIY, out string rep)
        {
            // X,Y - дискр. кол. инф. в слове, фильтрация по ч.р.
            double len;
            SortedDictionary<Word, int> sdX = DiscrInfVector(arrWX, arrIX, out len);
            SortedDictionary<Word, int> sdY = DiscrInfVector(arrWY, arrIY, out len);
            arrWX = sdX.Keys.ToArray();
            int[] arrInd = sdX.Values.ToArray();
            int count = 0;
            double[,] arrPxy = new double[intervalCount, intervalCount];
            rep = "<table cellspacing=0 border=1><tr><td>Слово<td>Дискр. кол. инф. X<td>Дискр. кол. инф. Y";
            for (int i = 0; i < arrWX.Length; i++)
            {
                if (!IsOpenedPos(arrWX[i].pos))
                    continue;
                int jY = 0;
                if (sdY.ContainsKey(arrWX[i]))
                    jY = sdY[arrWX[i]];
                arrPxy[arrInd[i], jY]++;
                count++;
                rep += string.Format("<tr><td>{0}<td>{1}<td>{2}", arrWX[i].norm, arrInd[i], jY);
            }
            arrWY = sdY.Keys.ToArray();
            arrInd = sdY.Values.ToArray();
            for (int i = 0; i < arrWY.Length; i++)
            {
                if (!IsOpenedPos(arrWY[i].pos))
                    continue;
                arrPxy[0, arrInd[i]]++;
                count++;
                rep += string.Format("<tr><td>{0}<td>{1}<td>{2}", arrWY[i].norm, 0, arrInd[i]);
            }
            double[] arrPx = new double[intervalCount], arrPy = new double[intervalCount];
            double hxy = 0;
            for (int i = 0; i < intervalCount; i++)
            {
                for (int j = 0; j < intervalCount; j++)
                {
                    arrPxy[i, j] /= count;
                    arrPx[i] += arrPxy[i, j];
                    arrPy[j] += arrPxy[i, j];
                    if (arrPxy[i, j] > 0)
                        hxy += arrPxy[i, j] * Math.Log(1 / arrPxy[i, j]);
                }
            }
            double hx = 0, hy = 0, I;
            for (int i = 0; i < intervalCount; i++)
            {
                if (arrPx[i] > 0)
                    hx += arrPx[i] * Math.Log(1 / arrPx[i]);
                if (arrPy[i] > 0)
                    hy += arrPy[i] * Math.Log(1 / arrPy[i]);
            }
            I = hx + hy - hxy;
            rep += string.Format("</table>I = {0:g4} + {1:g4} - {2:g4} = {3:g4}<br>", hx, hy, hxy, I);
            rep += string.Format("I / H(x) = {0:g4}<br>", I / hx);
            return I / hx;
        }        
        public static double CosCompare(List<Word> lwx, List<Word> lwy, out string rep)
        {
            double s = 0,xlen,ylen;
            SortedDictionary<Word, double> arrFreqX = TFVector(lwx,out xlen), arrFreqY = TFVector(lwy,out ylen);            
            Word[] arrK = arrFreqX.Keys.ToArray();
            double[] arrV = arrFreqX.Values.ToArray();
            for (int i =0;i<arrK.Length;i++)
            {
                if (arrFreqY.ContainsKey(arrK[i]))
                    s += arrV[i] * arrFreqY[arrK[i]];
            }
            rep = "ТекстX<table cellspacing=0 border=1><tr><td>Слово<td>TF";        
            for (int i = 0; i < arrK.Length; i++)
            {
                rep += string.Format("<tr><td>{0}<td>{1:g4}", arrK[i].norm, arrV[i]);
            }
            rep += "</table>ТекстY<table cellspacing=0 border=1><tr><td>Слово<td>TF";
            arrK = arrFreqY.Keys.ToArray();
            arrV = arrFreqY.Values.ToArray();
            for (int i = 0; i < arrK.Length; i++)
            {
                rep += string.Format("<tr><td>{0}<td>{1:g4}", arrK[i].norm, arrV[i]);
            }
            double cos=s / (xlen * ylen);
            rep += string.Format("</table>cos(fi) = (X,Y) / (|X|*|Y|) = {0:g4} / ({1:g4} * {2:g4}) = {3:g4}<br>", s, xlen, ylen, cos);
            return cos;
        }
        public static double CosPosCompare(List<Word> lwx, List<Word> lwy, out string rep)
        {
            string s;
            List<Word> lwxOpen = new List<Word>(), lwyOpen = new List<Word>();
            for (int i = 0; i < lwx.Count; i++)
            {
                if (IsOpenedPos(lwx[i].pos))
                {
                    lwxOpen.Add(lwx[i]);
                }
            }
            for (int i = 0; i < lwy.Count; i++)
            {
                if (IsOpenedPos(lwy[i].pos))
                {
                    lwyOpen.Add(lwy[i]);
                }
            }
            double cos = CosCompare(lwxOpen, lwyOpen, out s);
            rep = s;
            return cos;
        }
        public static double CosPosBoolCompare(List<Word> lwx, List<Word> lwy, out string rep)
        {
            // все частоты в векторе равны
            string s;
            List<Word> lwxOpen = new List<Word>(), lwyOpen = new List<Word>();
            for (int i = 0; i < lwx.Count; i++)
            {
                if (IsOpenedPos(lwx[i].pos))
                {
                    int j = lwxOpen.BinarySearch(lwx[i]);
                    if (j < 0)
                        lwxOpen.Insert(~j, lwx[i]);
                }
            }
            for (int i = 0; i < lwy.Count; i++)
            {
                if (IsOpenedPos(lwy[i].pos))
                {
                    int j = lwyOpen.BinarySearch(lwy[i]);
                    if (j < 0)
                        lwyOpen.Insert(~j, lwy[i]);
                }
            }
            double cos = CosCompare(lwxOpen, lwyOpen, out s);
            rep = s;
            return cos;
        }
        public static double CosBoolCompare(List<Word> lwx, List<Word> lwy, out string rep)
        {
            // все частоты в векторе равны
            string s;
            List<Word> lwxOpen = new List<Word>(), lwyOpen = new List<Word>();
            for (int i = 0; i < lwx.Count; i++)
            {
                //if (IsOpenedPos(lwx[i].pos))
                {
                    int j = lwxOpen.BinarySearch(lwx[i]);
                    if (j < 0)
                        lwxOpen.Insert(~j, lwx[i]);
                }
            }
            for (int i = 0; i < lwy.Count; i++)
            {
                //if (IsOpenedPos(lwy[i].pos))
                {
                    int j = lwyOpen.BinarySearch(lwy[i]);
                    if (j < 0)
                        lwyOpen.Insert(~j, lwy[i]);
                }
            }
            double cos = CosCompare(lwxOpen, lwyOpen, out s);
            rep = s;
            return cos;
        }
        public static double CosInfMinPosCompare(Word[] arrWX, Word[] arrWY, double[] arrIX, double[] arrIY, out string rep)
        {
            rep = string.Format("Порог значимости слова: {0}<br>Текст X", IMinThreshold);
            List<Word> lwx = new List<Word>(), lwy = new List<Word>();
            for (int i = 0; i < arrWX.Length; i++)
            {
                if (arrIX[i] >= IMinThreshold && IsOpenedPos(arrWX[i].pos))
                {
                    lwx.Add(arrWX[i]);
                }
            }
            for (int i = 0; i < arrWY.Length; i++)
            {
                if (arrIY[i] >= IMinThreshold && IsOpenedPos(arrWY[i].pos))
                {
                    lwy.Add(arrWY[i]);
                }
            }
            string s;
            double cos = CosCompare(lwx, lwy, out s);
            rep += s;
            return cos;
        }
        public static double CosInfMinMaxPosCompare(Word[] arrWX, Word[] arrWY, double[] arrIX, double[] arrIY, out string rep)
        {
            rep = string.Format("Интервал значимости слова: [{0};{1}]<br", IMinThreshold, IMaxThreshold);
            List<Word> lwx = new List<Word>(), lwy = new List<Word>();
            for (int i = 0; i < arrWX.Length; i++)
            {
                if (arrIX[i] >= IMinThreshold && IsOpenedPos(arrWX[i].pos) && arrIX[i] <= IMaxThreshold)
                {
                    lwx.Add(arrWX[i]);
                }
            }
            for (int i = 0; i < arrWY.Length; i++)
            {
                if (arrIY[i] >= IMinThreshold && IsOpenedPos(arrWY[i].pos) && arrIY[i] <= IMaxThreshold)
                {
                    lwy.Add(arrWY[i]);
                }
            }
            string s;
            double cos = CosCompare(lwx, lwy, out s);
            rep += s;
            return cos;
        }
        public static double CosInfVectorCompare(SortedDictionary<Word,double> infVecX, SortedDictionary<Word,double> infVecY,
            double xlen,double ylen,out string rep)
        {
            // В качестве элемента вектора - количество информации в слове
            double s = 0;
            Word[] arrK = infVecX.Keys.ToArray();
            double[] arrV = infVecX.Values.ToArray();
            rep = "ТекстX<table cellspacing=0 border=1><tr><td>Слово W<td>Кол. инф. I(W)";
            for (int i = 0; i < arrK.Length; i++)
            {
                if (infVecY.ContainsKey(arrK[i]))
                {
                    s += arrV[i] * infVecY[arrK[i]];
                }
                rep += string.Format("<tr><td>{0}<td>{1:g4}", arrK[i].norm, arrV[i]);
            }
            rep += "</table>ТекстY<table cellspacing=0 border=1><tr><td>Слово W<td>Кол. инф. I(W)";
            arrK = infVecY.Keys.ToArray();
            arrV = infVecY.Values.ToArray();
            for (int i = 0; i < arrK.Length; i++)
            {
                rep += string.Format("<tr><td>{0}<td>{1:g4}", arrK[i].norm, arrV[i]);
            }
            double cos = s / (xlen * ylen);
            rep += string.Format("</table>cos(fi) = (X,Y) / (|X|*|Y|) = {0:g4} / ({1:g4} * {2:g4}) = {3:g4}<br>", s, xlen, ylen, cos);
            return cos;
        }
        public static double CosInfVectorPosCompare(SortedDictionary<Word, double> infVecX, SortedDictionary<Word, double> infVecY,
            double xlen, double ylen, out string rep)
        {
            // В качестве элемента вектора - количество информации в слове, фильтрация по частям речи
            double s = 0;
            Word[] arrK = infVecX.Keys.ToArray();
            double[] arrV = infVecX.Values.ToArray();
            for (int i = 0; i < arrK.Length; i++)
            {
                if(IsOpenedPos(arrK[i].pos))
                if (infVecY.ContainsKey(arrK[i]))
                {
                    s += arrV[i] * infVecY[arrK[i]];
                }
            }
            rep = "</table>ТекстX<table cellspacing=0 border=1><tr><td>Слово W<td>Кол. инф. I(W)";
            for (int i = 0; i < arrK.Length; i++)
            {
                rep += string.Format("<tr><td>{0}<td>{1:g4}", arrK[i].norm, arrV[i]);
            }
            rep += "</table>ТекстY<table cellspacing=0 border=1><tr><td>Слово W<td>Кол. инф. I(W)";
            arrK = infVecY.Keys.ToArray();
            arrV = infVecY.Values.ToArray();
            for (int i = 0; i < arrK.Length; i++)
            {
                rep += string.Format("<tr><td>{0}<td>{1:g4}", arrK[i].norm, arrV[i]);
            }
            double cos = s / (xlen * ylen);
            rep += string.Format("</table>cos(fi) = (X,Y) / (|X|*|Y|) = {0:g4} / ({1:g4} * {2:g4}) = {3:g4}<br>", s, xlen, ylen, cos);
            return cos;
        }
        public static SortedDictionary<Word,double> TFVector(List<Word> lw,out double len)
        {
            SortedDictionary<Word, double> sd = new SortedDictionary<Word, double>();
            foreach (Word w in lw)
            {
                if (sd.ContainsKey(w))
                {
                    sd[w] += 1;
                }
                else
                    sd.Add(w, 1);
            }
            double[] arrV = sd.Values.ToArray();
            len = 0;
            for(int i=0;i<arrV.Length;i++)
            {
                len += arrV[i] * arrV[i];
            }
            len = Math.Sqrt(len);
            return sd;
        }
        public static SortedDictionary<Word, double> InfVector(Word[] arrW, double[] arrI, out double len)
        {
            SortedDictionary<Word, double> sd = new SortedDictionary<Word, double>();
            for (int i = 0; i < arrW.Length; i++)
            {                
                if (sd.ContainsKey(arrW[i]))
                {
                    double I = Math.Max(sd[arrW[i]],arrI[i]);
                    sd[arrW[i]] = Math.Min(I,IMax);
                }
                else
                    sd.Add(arrW[i], Math.Min(arrI[i],IMax));
            }
            double[] arrV = sd.Values.ToArray();
            len = 0;
            for (int i=0;i<arrV.Length;i++)
            {
                len += arrV[i]*arrV[i];
            }
            len = Math.Sqrt(len);
            return sd;
        }
        public static SortedDictionary<Word, int> DiscrInfVector(Word[] arrW, double[] arrI, out double len)
        {
            // вектор дискретизированных кол. инф.
            SortedDictionary<Word, int> sd = new SortedDictionary<Word, int>();
            double step = (IMax - IMin) / intervalCount;
            for (int i = 0; i < arrW.Length; i++)
            {
                int j = ((int)((Math.Max(Math.Min(arrI[i], IMax),IMin) - IMin) / step)) % intervalCount;
                if (sd.ContainsKey(arrW[i]))
                    sd[arrW[i]] = Math.Max(sd[arrW[i]], j);
                else
                    sd.Add(arrW[i], j);
            }
            int[] arrV = sd.Values.ToArray();
            len = 0;
            for (int i = 0; i < arrV.Length; i++)
            {
                len += arrV[i] * arrV[i];
            }
            len = Math.Sqrt(len);
            return sd;
        }
        public static SortedDictionary<Word, int>[] Pos(List<Word> lw)
        {
            SortedDictionary<string,SortedDictionary<Word, int>> sdsd = new SortedDictionary<string,SortedDictionary<Word,int>>();
            List<string> lS=new List<string>();
            lS.AddRange(Enum.GetNames(typeof(AttributeEn)));
            lS.AddRange(Enum.GetNames(typeof(AttributeRu)));
            foreach (string s in lS)
            {
                sdsd.Add(s, new SortedDictionary<Word, int>());
            }
            foreach (Word w in lw)
            {
                if (sdsd[w.pos].ContainsKey(w))
                    sdsd[w.pos][w] += 1;
                else
                    sdsd[w.pos].Add(w, 1);
            }
            return sdsd.Values.ToArray();
        }
        public static SortedDictionary<Word, int>[] PosOpened(List<Word> lw)
        {
            SortedDictionary<string, SortedDictionary<Word, int>> sdsd = new SortedDictionary<string, SortedDictionary<Word, int>>();
            List<string> lS = new List<string>();
            lS.AddRange(Enum.GetNames(typeof(AttributeEn)));
            lS.AddRange(Enum.GetNames(typeof(AttributeRu)));
            foreach (string s in lS)
            {
                sdsd.Add(s, new SortedDictionary<Word, int>());
            }
            foreach (Word w in lw)
            {
                if (!IsOpenedPos(w.pos))
                    continue;
                if (sdsd[w.pos].ContainsKey(w))
                    sdsd[w.pos][w] += 1;
                else
                    sdsd[w.pos].Add(w, 1);
            }
            return sdsd.Values.ToArray();
        }
        public static bool IsOpenedPos(string pos)
        {
            if (lOpenedPos.BinarySearch(pos)>=0)
                return true;
            return false;
        }
        public static void GenerateTexts(List<Word> lwx, List<Word> lwy,
            out List<Word>[] arrText, out double[] arrMark, out string[] arrReplaced, out string rep)
        {
            List<Word> lwx2 = new List<Word>();
            foreach (Word w in lwx)
            {
                lwx2.Add(new Word() { form = w.form, norm = w.norm, pos = w.pos });
            }
            List<Word> lx = new List<Word>(), ly = new List<Word>();
            foreach (Word w in lwx)
            {
                int i = lx.BinarySearch(w);
                if (i < 0)
                    lx.Insert(~i, w);
            }
            foreach (Word w in lwy)
            {
                int i = ly.BinarySearch(w);
                if (i < 0 && lx.BinarySearch(w) < 0)
                    ly.Insert(~i, w);
            }
            Random rnd = new Random();
            int count = lx.Count, sumReplaced = 0;
            List<List<Word>> lText = new List<List<Word>>();
            List<double> lMark = new List<double>();
            List<string> lReplaced = new List<string>();
            rep = "<table border=1 cellspacing=0><tr><td>№<td>Оценка<td>Замена<td>Число замен";
            for (int i = 0; i < count; i++)
            {
                int replaced = 0;
                int ix = rnd.Next(lx.Count - 1), iy = rnd.Next(ly.Count - 1);
                for (int j = 0; j < lwx2.Count; j++)
                {
                    if (lwx2[j].CompareTo(lx[ix]) == 0)
                    {
                        lwx2[j] = new Word() { form = ly[iy].form, norm = ly[iy].norm, pos = ly[iy].pos };
                        replaced++;
                    }
                }
                sumReplaced += replaced;
                lMark.Add((lwx2.Count - sumReplaced) / (double)lwx.Count * 100);
                lReplaced.Add(string.Format("{0}->{1}({2})", lx[ix].norm, ly[iy].norm, replaced));
                rep += string.Format("<tr><td>{0}<td>{1:g3}<td>{2}->{3}<td>{4}", i, lMark.Last(), lx[ix].norm, ly[iy].norm, replaced);
                lText.Add(new List<Word>());
                foreach (Word w in lwx2)
                {
                    lText.Last().Add(new Word() { form = w.form, norm = w.norm, pos = w.pos });
                }
                lx.RemoveAt(ix);
                ly.RemoveAt(iy);
            }
            rep += "</table>";
            arrText = lText.ToArray();
            arrMark = lMark.ToArray();
            arrReplaced = lReplaced.ToArray();
        }        
        public static string EmulateEn(string t1, string t2)
        {
            string rep = string.Format("Изменяемый текст:<br>{0}<br>Текст, из которого берутся слова для замены:<br>{1}<br>",t1,t2), rep2="";
            List<Word> lwx = Parse(t1), lwy = Parse(t2);
            List<Word>[] arrText;
            double[] arrMark;
            string[] arrReplaced;
            string tmp;
            GenerateTexts(lwx, lwy, out arrText, out arrMark, out arrReplaced, out tmp);
            rep += "<table border = 1 cellspacing = 0><tr><td>№<td>I1<td>I2/Hx<td>Оценка<td>Замена";
            for (int i = 0; i < arrText.Length; i++)
            {
                double[] arrPx,arrPy,arrPxy;
                double hx,hy,hxy;
                string s;
                double I1 = KuznetsovCompare(Pos(lwx), Pos(arrText[i]),
                    out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out s);
                rep2 += s + "<br>";
                double I2 = BoolCompare(lwx, arrText[i], out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out s);
                rep2 += s + "<br>";
                I2 /= hx;
                rep += string.Format("<tr><td>{0}<td>{1:g4}<td>{2:g4}<td>{3:g3}<td>{4}",
                    i,I1, I2, arrMark[i], arrReplaced[i]);
            }            
            return rep + "</table>" + rep2;
        }
        public static string EmulateRu(string path)
        {
            string[] arrFile = Directory.GetFiles(path);
            string rep = "",regRep="";
            //string[] arrMethod = { "Оценка без инф.", "Оценка ключ. инф. Min Max", "Оценка ч.р.", "Оценка инф. вектор","Оценка Куз.", "Оценка Куз. ч.р.",
            //                        "Оценка cos", "Оценка cos ключ. инф.", "Оценка cos ч.р.", "Оценка cos инф. вектор", "Оценка cos ч.р. инф. вектор" };
            string[] arrMethod = { "Оценка бул.", "Оценка бул. ч.р.", "Оценка морф.", "Оценка морф. ч.р.", "Оценка cos", "Оценка cos ч.р.", "Оценка cos бул.", "Оценка морф.2" };
            double[][] mMark = new double[11][];
            for (int i = 0; i < mMark.Length; i++)
            {
                mMark[i] = new double[arrFile.Length];
            }
            double[] arrTutMark = new double[arrFile.Length];
            TInput.C_EAnswer[] arrAns = new TInput.C_EAnswer[arrFile.Length];
            for (int i = 0; i < arrFile.Length; i++)
            {
                arrAns[i] = new TInput.C_EAnswer(arrFile[i]);
                //arrAns[i] = new TInput.C_EAnswer(arrAns[i].p_FIO, arrAns[i].p_group, arrAns[i].p_question, "A world guide to good manners. Travelling to all corners of the world gets easier and easier. We live in a global village, but how well do we know and understand each other? Here is a simple test. Imagine you have arranged a meeting at four o’clock. What time should you expect your foreign business colleagues to arrive? If they are German, they will be bang on time. If they are American, they will probably be fifteen minutes early. If they are British, they will be fifteen minutes late, and you should allow up to an hour for the Italians. When the European Community began to increase in size, several guidebooks appeared giving advice on international etiquette. At first many people thought this was a joke, especially the British, who seemed to assume that the widespread understanding of their language meant a corresponding understanding of English customs. Very soon they had to change their ideas, as they realized that they had a lot to learn about how to behave with their foreign business friends.",
                //    arrAns[i].p_student_answer, new TInput.Tutor[] { arrAns[i].GetTutor(0), arrAns[i].GetTutor(1) });
                //arrAns[i].Save(arrFile[i]);
            }
            for (int i = 0; i < arrAns.Length; i++)
            {
                List<Word> lwx = Parse(arrAns[i].p_etalon_answer), lwy = Parse(arrAns[i].p_student_answer);
                List<Word>[] arrSentX = ParseBySentences(arrAns[i].p_etalon_answer);
                List<Word>[] arrSentY = ParseBySentences(arrAns[i].p_student_answer);
                arrTutMark[i] = arrAns[i].GetTutor(0).Mark;
                Word[] arrWX, arrWY;
                double[] arrIX, arrIY;
                string s;
                //WordInfo(arrSentX, out arrWX, out arrIX, out s);
                //rep += "Текст X"+s;
                //WordInfo(arrSentY, out arrWY, out arrIY,out s);
                //rep += "Текст Y" + s;
                double xlen,ylen;
                //SortedDictionary<Word, double> infVecX = InfVector(arrWX, arrIX, out xlen);
                //SortedDictionary<Word, double> infVecY = InfVector(arrWY, arrIY, out ylen);
                double[] arrPx, arrPy, arrPxy;
                double hx, hy, hxy, II=0, I2=0,IK=0, IK2,IKP=0,IS=0, IP=0,cos=0, cosI=0,cosIP=0,cosI2=0,cosI2P=0,cosB;
                IS = BoolCompare(lwx, lwy, out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out s) / hx;
                rep += s;
                //II=BoolInfMinMaxPosCompare(arrWX,arrWY,arrIX,arrIY, out s);
                //rep+=s;
                IP = BoolPosCompare(lwx, lwy, out s);
                rep += s;
                //I2 = BoolDiscInfVectorPosCompare(arrWX,arrWY,arrIX,arrIY, out s);
                //rep += s;
                IK = KuznetsovCompare(Pos(lwx), Pos(lwy),
                    out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out s);
                rep += s;
                
                IKP = KuznetsovCompare(PosOpened(lwx), PosOpened(lwy),
                    out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out s);
                rep += s;
                cos = CosCompare(lwx, lwy, out s);
                rep += s;
                //cosI = CosInfMinMaxPosCompare(arrWX,arrWY,arrIX,arrIY, out s);
                //rep += s;
                cosIP = CosPosCompare(lwx, lwy, out s);
                rep += s;
                cosB = CosBoolCompare(lwx, lwy, out s);
                rep += s;

                IK2 = Math.Abs(hx-hxy)/hx;
                //cosI2 = CosInfVectorCompare(infVecX, infVecY, xlen,ylen, out s);
                //rep += s;
                //cosI2P = CosInfVectorPosCompare(infVecX, infVecY, xlen,ylen, out s);
                //rep += s;

                //II = IS; I2 = IS; cosI = cos; cosI2 = cos; cosI2P = cos;

                /*mMark[0][i] = IS;
                mMark[1][i] = II;
                mMark[2][i] = IP;
                mMark[3][i] = I2;
                mMark[4][i] = IK;
                mMark[5][i] = IKP;
                mMark[6][i] = cos;
                mMark[7][i] = cosI;
                mMark[8][i] = cosIP;
                mMark[9][i] = cosI2;
                mMark[10][i] = cosI2P;*/
                mMark[0][i] = IS;
                mMark[1][i] = IP;
                mMark[2][i] = IK;
                mMark[3][i] = IKP;
                mMark[4][i] = cos;
                mMark[5][i] = cosIP;
                mMark[6][i] = cosB;
                mMark[7][i] = IK2;
            }
            Sample sY = new Sample("y","y",arrTutMark);
            Regression[] arrReg = new Regression[mMark.Length];
            for (int i = 0; i < mMark.Length; i++)
            {
                try
                {
                    arrReg[i] = new Regression(sY, new Sample[] { new Sample("x", "x", mMark[i]) });
                }
                catch
                {
                    arrReg[i] = null;
                }
            }

            string tbl = "<table border=1 cellspacing=0><tr><td>№<td>Оценка преп.<td>Эталон. кол. сл.<td>Кол. сл. в ответе";
            for (int i = 0; i < arrMethod.Length;i++ )
            {
                if (arrReg[i] == null)
                    continue;
                tbl += string.Format("<td>{0}<td>{0} мод.", arrMethod[i]);
            }
            for (int i = 0; i < arrTutMark.Length; i++)
            {
                tbl += string.Format("<tr><td>{0}<td>{1}<td>{2}<td>{3}",i,arrTutMark[i],
                    Parse(arrAns[i].p_etalon_answer).Count,Parse(arrAns[i].p_student_answer).Count);
                for (int j = 0; j < mMark.Length; j++)
                {
                    if (arrReg[j] == null)
                        continue;
                    tbl += string.Format("<td>{0:g6}<td>{1:g4}", mMark[j][i], arrReg[j].arrYMod[i]);
                }
            }
            tbl += "</table>";
            regRep = "<table border=1 cellspacing=0><tr><td>Метод<td>Регрессия";
            for (int i = 0; i < arrReg.Length; i++)
            {
                if (arrReg[i] == null)
                    continue;
                regRep += string.Format("<tr><td>{0}<td>{1}", arrMethod[i], arrReg[i].RegReport());
            }
            regRep += "</table>";
            return rep+"<br>Результаты"+tbl+"Сравнение методов"+regRep;
        }
        public static string OptimizeBoolMin(string path)
        {
            string rep,s;
            string[] arrFile = Directory.GetFiles(path);
            double[] arrMark=new double[arrFile.Length];
            double[] arrTutMark = new double[arrFile.Length];
            TInput.C_EAnswer[] arrAns = new TInput.C_EAnswer[arrFile.Length];
            List<Word>[][] arrArrSentX=new List<Word>[arrFile.Length][], arrArrSentY=new List<Word>[arrFile.Length][];
            List<Word>[] arrLWX = new List<Word>[arrFile.Length], arrLWY = new List<Word>[arrFile.Length];
            for (int i = 0; i < arrFile.Length; i++)
            {
                arrAns[i] = new TInput.C_EAnswer(arrFile[i]);
                arrLWX[i] = Parse(arrAns[i].p_etalon_answer);
                arrLWY[i] = Parse(arrAns[i].p_student_answer);
                arrArrSentX[i]=ParseBySentences(arrAns[i].p_etalon_answer);
                arrArrSentY[i]=ParseBySentences(arrAns[i].p_student_answer);
                arrTutMark[i]=arrAns[i].GetTutor(0).Mark;
            }
            Regression rMin = null;
            rep = "Критерий совместной информации для булевой модели текста, отсев слов по нижней границе";
            rep += "<table border=1><tr><td>IMin<td>IDef<td>Ср. абс. ошибка";
            double crit = double.MaxValue, IMinOpt=0,IDefOpt=0;
            Sample sY = new Sample("y", "y", arrTutMark);
            for (double IMinCur = 0; IMinCur < 10; IMinCur += 0.1)
            {
                for (double IDefCur = IMinCur - 1; IDefCur < IMinCur + 2; IDefCur+=2)
                {
                    IMinThreshold = IMinCur;
                    IDefault = IDefCur;
                    for (int i = 0; i < arrAns.Length; i++)
                    {
                        Word[] arrWX, arrWY;
                        double[] arrIX, arrIY;
                        WordInfo(arrArrSentX[i], out arrWX, out arrIX,out s);
                        WordInfo(arrArrSentY[i], out arrWY, out arrIY, out s);
                        arrMark[i] = BoolInfMinPosCompare(arrWX,arrWY,arrIX,arrIY, out s);
                    }
                    Regression r = new Regression(sY, new Sample[] { new Sample("x", "x", arrMark) });
                    rep += string.Format("<tr><td>{0:g4}<td>{1:g4}<td>{2:g4}",IMinCur,IDefCur,r.avErr);
                    if (r.avErr < crit)
                    {
                        crit = r.avErr;
                        rMin = r;
                        IMinOpt = IMinCur;
                        IDefOpt = IDefCur;
                    }
                }
            }
            rep += string.Format("</table>Результат: {0:g4}, {1:g4}, {2:g4}<br>", IMinOpt, IDefOpt, rMin.avErr);

            rep += "Критерий совместной информации для булевой модели текста, отсев слов по нижней и верхней границе";
            rep += "<table border=1><tr><td>IMin<td>IMax<td>IDef<td>Ср. абс. ошибка";
            crit = double.MaxValue;
            double IMaxOpt = 0;
            for (double IMinCur = 0; IMinCur < 10; IMinCur += 0.5)
            for (double IMaxCur = IMinCur+1; IMaxCur < 15; IMaxCur += 0.5)
            {
                for (double IDefCur = IMinCur - 1; IDefCur < IMinCur + 2; IDefCur += 2)
                {
                    IMinThreshold = IMinCur;
                    IMaxThreshold = IMaxCur;
                    IDefault = IDefCur;
                    for (int i = 0; i < arrAns.Length; i++)
                    {
                        Word[] arrWX, arrWY;
                        double[] arrIX, arrIY;
                        WordInfo(arrArrSentX[i], out arrWX, out arrIX, out s);
                        WordInfo(arrArrSentY[i], out arrWY, out arrIY, out s);
                        arrMark[i] = BoolInfMinMaxPosCompare(arrWX, arrWY, arrIX, arrIY, out s);
                    }
                    Regression r = new Regression(sY, new Sample[] { new Sample("x", "x", arrMark) });
                    rep += string.Format("<tr><td>{0:g4}<td>{1:g4}<td>{2:g4}<td>{3:g4}", IMinCur, IMaxCur,IDefCur, r.avErr);
                    if (r.avErr < crit)
                    {
                        crit = r.avErr;
                        rMin = r;
                        IMinOpt = IMinCur;
                        IMaxOpt = IMaxCur;
                        IDefOpt = IDefCur;
                    }
                }
            }
            rep += string.Format("</table>Результат: {0:g4}, {1:g4}, {2:g4}, {3:g4}<br>", IMinOpt, IMaxOpt,IDefOpt, rMin.avErr);
            return rep;
        }
        public static string OptimizeBoolMinMax(string path)
        {
            string rep, s;
            string[] arrFile = Directory.GetFiles(path);
            double[] arrMark = new double[arrFile.Length];
            double[] arrTutMark = new double[arrFile.Length];
            TInput.C_EAnswer[] arrAns = new TInput.C_EAnswer[arrFile.Length];
            List<Word>[][] arrArrSentX = new List<Word>[arrFile.Length][], arrArrSentY = new List<Word>[arrFile.Length][];
            List<Word>[] arrLWX = new List<Word>[arrFile.Length], arrLWY = new List<Word>[arrFile.Length];
            for (int i = 0; i < arrFile.Length; i++)
            {
                arrAns[i] = new TInput.C_EAnswer(arrFile[i]);
                arrLWX[i] = Parse(arrAns[i].p_etalon_answer);
                arrLWY[i] = Parse(arrAns[i].p_student_answer);
                arrArrSentX[i] = ParseBySentences(arrAns[i].p_etalon_answer);
                arrArrSentY[i] = ParseBySentences(arrAns[i].p_student_answer);
                arrTutMark[i] = arrAns[i].GetTutor(0).Mark;
            }
            Regression rMin = null;            
            double crit = double.MaxValue, IMinOpt = 0;
            Sample sY = new Sample("y", "y", arrTutMark);
            rep = "Критерий совместной информации для булевой модели текста, отсев слов по нижней и верхней границе";
            rep += "<table border=1><tr><td>IMin<td>IMax<td>Ср. абс. ошибка";
            crit = double.MaxValue;
            double IMaxOpt = 0;
            for (double IMinCur = 0; IMinCur < 3; IMinCur += 0.1)
            {
                IDefault = IMinCur;
                Word[][] arrArrWX = new Word[arrAns.Length][], arrArrWY = new Word[arrAns.Length][];
                double[][] arrArrIX = new double[arrAns.Length][], arrArrIY = new double[arrAns.Length][];
                for (int i = 0; i < arrAns.Length; i++)
                {
                    WordInfo(arrArrSentX[i], out arrArrWX[i], out arrArrIX[i], out s);
                    WordInfo(arrArrSentY[i], out arrArrWY[i], out arrArrIY[i], out s);
                }                
                for (double IMaxCur = IMinCur + 5; IMaxCur < 15; IMaxCur += 0.1)
                {
                    
                        IMinThreshold = IMinCur;
                        IMaxThreshold = IMaxCur;
                        for (int i = 0; i < arrAns.Length; i++)
                        {                            
                            arrMark[i] = BoolInfMinMaxPosCompare(arrArrWX[i], arrArrWY[i], arrArrIX[i], arrArrIY[i], out s);
                        }
                        Regression r = new Regression(sY, new Sample[] { new Sample("x", "x", arrMark) });
                        rep += string.Format("<tr><td>{0:g4}<td>{1:g4}<td>{2:g4}", IMinCur, IMaxCur, r.avErr);
                        if (r.avErr < crit)
                        {
                            crit = r.avErr;
                            rMin = r;
                            IMinOpt = IMinCur;
                            IMaxOpt = IMaxCur;
                        }
                    
                }
            }
            rep += string.Format("</table>Результат: {0:g4}, {1:g4}, {2:g4}<br>", IMinOpt, IMaxOpt, rMin.avErr);
            return rep;
        }
        public static string OptimizeBoolDiscInfVectorPos(string path)
        {
            string rep, s;
            string[] arrFile = Directory.GetFiles(path);
            double[] arrMark = new double[arrFile.Length];
            double[] arrTutMark = new double[arrFile.Length];
            TInput.C_EAnswer[] arrAns = new TInput.C_EAnswer[arrFile.Length];
            List<Word>[][] arrArrSentX = new List<Word>[arrFile.Length][], arrArrSentY = new List<Word>[arrFile.Length][];
            List<Word>[] arrLWX = new List<Word>[arrFile.Length], arrLWY = new List<Word>[arrFile.Length];
            Word[][] arrArrWX = new Word[arrAns.Length][], arrArrWY = new Word[arrAns.Length][];
            double[][] arrArrIX = new double[arrAns.Length][], arrArrIY = new double[arrAns.Length][];
            for (int i = 0; i < arrFile.Length; i++)
            {
                arrAns[i] = new TInput.C_EAnswer(arrFile[i]);
                arrLWX[i] = Parse(arrAns[i].p_etalon_answer);
                arrLWY[i] = Parse(arrAns[i].p_student_answer);
                arrArrSentX[i] = ParseBySentences(arrAns[i].p_etalon_answer);
                arrArrSentY[i] = ParseBySentences(arrAns[i].p_student_answer);
                arrTutMark[i] = arrAns[i].GetTutor(0).Mark;
                WordInfo(arrArrSentX[i], out arrArrWX[i], out arrArrIX[i], out s);
                WordInfo(arrArrSentY[i], out arrArrWY[i], out arrArrIY[i], out s);
            }
            Regression rMin = null;
            double crit = double.MaxValue, IMinOpt = 0;
            int intervCountOpt = 0;
            Sample sY = new Sample("y", "y", arrTutMark);
            rep = "Критерий совместной информации для векторной модели текста с взвешиванием слов по дискретизированному количеству информации";
            rep += "<table border=1><tr><td>IMin<td>IMax<td>Число интервалов<td>Ср. абс. ошибка";
            crit = double.MaxValue;
            double IMaxOpt = 0;
            for(int intervCountCur=3;intervCountCur<15;intervCountCur++)
            for (double IMinCur = 0; IMinCur < 3; IMinCur += 0.2)
            {
                for (double IMaxCur = IMinCur + 7; IMaxCur < 15; IMaxCur += 0.2)
                {
                    IMin = IMinCur;
                    IMax = IMaxCur;
                    intervalCount = intervCountCur;
                    for (int i = 0; i < arrAns.Length; i++)
                    {
                        arrMark[i] = BoolDiscInfVectorPosCompare(arrArrWX[i], arrArrWY[i], arrArrIX[i], arrArrIY[i], out s);
                    }
                    Regression r = new Regression(sY, new Sample[] { new Sample("x", "x", arrMark) });
                    rep += string.Format("<tr><td>{0:g4}<td>{1:g4}<td>{2:g4}<td>{3:g4}", IMinCur, IMaxCur, intervCountCur,r.avErr);
                    if (r.avErr < crit)
                    {
                        crit = r.avErr;
                        rMin = r;
                        IMinOpt = IMinCur;
                        IMaxOpt = IMaxCur;
                        intervCountOpt = intervCountCur;
                    }

                }
            }
            rep += string.Format("</table>Результат: {0:g4}, {1:g4}, {2:g4},{3:g4}<br>", IMinOpt, IMaxOpt, intervCountOpt, rMin.avErr);
            return rep;
        }
        public static void LoadCache()
        {
            try
            {
                FileStream fs = File.Open("cache.bin", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                cacheW = (SortedDictionary<string, List<Word>>)bf.Deserialize(fs);
                cacheSent = (SortedDictionary<string, List<Word>[]>)bf.Deserialize(fs);
                cacheP = (SortedDictionary<string, double>)bf.Deserialize(fs);
                fs.Close();
            }
            catch
            {
                cacheW = new SortedDictionary<string, List<Word>>();
                cacheSent = new SortedDictionary<string, List<Word>[]>();
                cacheP = new SortedDictionary<string, double>();
            }
        }
        public static void SaveCache()
        {
            FileStream fs = File.Open("cache.bin", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, cacheW);
            bf.Serialize(fs, cacheSent);
            bf.Serialize(fs, cacheP);
            fs.Close();
        }
        public static void CheckProbCache()
        {
            List<string> lK = new List<string>();
            double[] arrV = cacheP.Values.ToArray();
            string[] arrK = cacheP.Keys.ToArray();
            for (int i = 0; i < arrK.Length; i++)
            {
                if (arrV[i] == -1)
                    lK.Add(arrK[i]);
            }
            foreach (string k in lK)
            {
                cacheP.Remove(k);
            }
            Prob(lK.ToArray());
        }
        public static void ImportAns()
        {
            string[] arrFile = Directory.GetFiles(@"C:\Users\Anton\Documents\Visual Studio 2010\Projects\Semantics\Semantics\bin\Debug\Answers old");
            int[] arrMark= {79,91,92,79,82,82,60,68,48,57,73,64,82,91,36,59,67,63,58,32,55,70,51,77,93,64,65,81,52,68,81,36,64,74,36,42,60,64,72,78,58,83,95,55,70,89,38,62,69,43,66,65,21,33,55,35,56,64};
            for (int i = 0; i < arrFile.Length; i++)
			{			 
                XmlDocument doc = new XmlDocument();
                doc.Load(arrFile[i]);
                XmlElement el = doc.DocumentElement;
                string FIO = el["FIO"].InnerText;
                string group = el["group"].InnerText;
                string question = el["question"].InnerText;
                string etalon_answer = el["etalon_answer"].InnerText;
                string student_answer = el["student_answer"].InnerText;
                List<TInput.Tutor> lTutor = new List<TInput.Tutor>();
                lTutor.Add(new TInput.Tutor("", arrMark[i]));
                TInput.C_EAnswer ans = new TInput.C_EAnswer(FIO, group, question, etalon_answer,
                    student_answer, lTutor.ToArray());
                ans.Save(string.Format(@"out\{0:d4}.xml",i));
            }
        }
    }
}
