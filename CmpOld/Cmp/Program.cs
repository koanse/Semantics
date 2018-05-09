using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using Statistics;
using System.Text.RegularExpressions;

namespace Cmp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Compare.language = "en";
            //Compare.LoadCache("cache.bin");
            //Compare.InitSyn2();
            //Compare.InitW();
            //Compare.LoadDic("eng.bin");
            //Compare.LoadDic("rus.bin");
           //Compare.GenReplace();
            //string rep = Compare.Emulate(@"C:\Users\Anton\Documents\Visual Studio 2010\Projects\Semantics\Cmp\bin\Debug\cmpRepSynWнов");
            //Compare.ImportDic("SouleSynonyms.dsl");
            //Compare.ImportDic("RusSyn.dsl");
            //Compare.SaveDic("rus.bin");
            //Compare.SaveDic("eng.bin");
            //Compare.SaveCache();
            //Compare.GenRep();
            Compare.InitSyn3();
            Application.Run(new MainForm());            
        }
    }
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
        Существительное = 1, Глагол, Прилагательное, Наречие, Числительное, Местоимение, Причастие, Деепричастие,
        Междометие, Частица, Союз, Предлог, Вводное_слово,
        неизм, Предикатив, Слово_из_другого_языка, пунктуация, Слово_из_цифр, ед_ч, мн_ч, жен_р, муж_р, ср_р, одуш, им_п,
        род_п, дат_п, вин_п, тв_п, пр_п, финитн_форма, Инфинитив, перв_л, вт_л, тр_л, прош_вр, наст_вр,
        пов_накл, пусто1, изъяв_накл, актив_залог, пассив_залог, пусто2, кратк, сравн_степ, Слово_с_большой_буквы,
        Нормализовано_эвристически
    }
    static public class Compare
    {
        static SortedDictionary<string, object[]> sdSyn = new SortedDictionary<string, object[]>();
        static SortedDictionary<string, double> sdW = new SortedDictionary<string, double>();
        public static void InitW()
        {
            sdW.Add("Существительное", 0.4);
            sdW.Add("Глагол", 0.3);
            sdW.Add("Прилагательное", 0.2);
            sdW.Add("Наречие", 0.1);            
        }
        public static void InitSyn()
        {
            sdSyn.Add("leader", new object[] { "guide", 0.8 });
            sdSyn.Add("respectable", new object[] { "good", 0.7 });
            sdSyn.Add("behavior", new object[] { "manner", 0.9 });
            sdSyn.Add("place", new object[] { "corner", 0.8 });
            sdSyn.Add("become", new object[] { "get", 0.7 });
            sdSyn.Add("community", new object[] { "village", 0.6 });
            sdSyn.Add("identify", new object[] { "know", 0.8 });
            sdSyn.Add("appreciate", new object[] { "understand", 0.8 });
            sdSyn.Add("exam", new object[] { "test", 1.0   });
            sdSyn.Add("visualize", new object[] { "imagine", 0.8 });
            sdSyn.Add("assume", new object[] { "expect", 0.7 });
            sdSyn.Add("commercial", new object[] { "business", 0.9 });
            sdSyn.Add("let", new object[] { "allow", 0.8 });
            sdSyn.Add("rise", new object[] { "increase", 0.9 });
            sdSyn.Add("manual", new object[] { "guidebook", 0.9 });
            sdSyn.Add("information", new object[] { "advice", 0.8 });
            sdSyn.Add("protocol", new object[] { "etiquette", 0.8 });
            sdSyn.Add("tale", new object[] { "joke", 0.8 });
            sdSyn.Add("specially", new object[] { "especially", 1.0   });
            sdSyn.Add("accept", new object[] { "assume", 0.8 });
            sdSyn.Add("study", new object[] { "learn", 1.0   });


            sdSyn.Add("world", new object[] { "universe", 0.5 });
            sdSyn.Add("travel", new object[] { "roam", 0.5 });
            sdSyn.Add("easy", new object[] { "cool", 0.5 });
            sdSyn.Add("live", new object[] { "animate", 0.5 });
            sdSyn.Add("global", new object[] { "large", 0.5 });
            sdSyn.Add("meeting", new object[] { "summit", 0.5 });
            sdSyn.Add("late", new object[] { "down", 0.5 });
            sdSyn.Add("widespread", new object[] { "common", 0.5 });
            sdSyn.Add("language", new object[] { "semantic", 0.5 });
            sdSyn.Add("mean", new object[] { "doom", 0.5 });
            sdSyn.Add("corresponding", new object[] { "matching", 0.5 });
            sdSyn.Add("custom", new object[] { "duty", 0.5 });
        }
        public static void InitSyn3()
        {
            sdSyn.Add("leader", new object[] { "guide", 0.0 });
            sdSyn.Add("respectable", new object[] { "good", 0.0 });
            sdSyn.Add("behavior", new object[] { "manner", 0.0 });
            sdSyn.Add("place", new object[] { "corner", 0.0 });
            sdSyn.Add("become", new object[] { "get", 0.0 });
            sdSyn.Add("community", new object[] { "village", 0.0 });
            sdSyn.Add("identify", new object[] { "know", 0.0 });
            sdSyn.Add("appreciate", new object[] { "understand", 0.0 });
            sdSyn.Add("exam", new object[] { "test", 0.0 });
            sdSyn.Add("visualize", new object[] { "imagine", 0.0 });
            sdSyn.Add("assume", new object[] { "expect", 0.0 });
            sdSyn.Add("commercial", new object[] { "business", 0.0 });
            sdSyn.Add("let", new object[] { "allow", 0.0 });
            sdSyn.Add("rise", new object[] { "increase", 0.0 });
            sdSyn.Add("manual", new object[] { "guidebook", 0.0 });
            sdSyn.Add("information", new object[] { "advice", 0.0 });
            sdSyn.Add("protocol", new object[] { "etiquette", 0.0 });
            sdSyn.Add("tale", new object[] { "joke", 0.0 });
            sdSyn.Add("specially", new object[] { "especially", 0.0 });
            sdSyn.Add("accept", new object[] { "assume", 0.0 });
            sdSyn.Add("study", new object[] { "learn", 0.0 });


            sdSyn.Add("world", new object[] { "universe", 0.0 });
            sdSyn.Add("travel", new object[] { "roam", 0.0 });
            sdSyn.Add("easy", new object[] { "cool", 0.0 });
            sdSyn.Add("live", new object[] { "animate", 0.0 });
            sdSyn.Add("global", new object[] { "large", 0.0 });
            sdSyn.Add("meeting", new object[] { "summit", 0.0 });
            sdSyn.Add("late", new object[] { "down", 0.0 });
            sdSyn.Add("widespread", new object[] { "common", 0.0 });
            sdSyn.Add("language", new object[] { "semantic", 0.0 });
            sdSyn.Add("mean", new object[] { "doom", 0.0 });
            sdSyn.Add("corresponding", new object[] { "matching", 0.0 });
            sdSyn.Add("custom", new object[] { "duty", 0.0 });
        }
        public static void InitSyn2()
        {
            sdSyn.Add("leader", new object[] { "guide", 1.0 });
            sdSyn.Add("respectable", new object[] { "good", 1.0 });
            sdSyn.Add("behavior", new object[] { "manner", 1.0 });
            sdSyn.Add("place", new object[] { "corner", 1.0 });
            sdSyn.Add("become", new object[] { "get", 1.0 });
            sdSyn.Add("community", new object[] { "village", 1.0 });
            sdSyn.Add("identify", new object[] { "know", 1.0 });
            sdSyn.Add("appreciate", new object[] { "understand", 1.0 });
            sdSyn.Add("exam", new object[] { "test", 1.0 });
            sdSyn.Add("visualize", new object[] { "imagine", 1.0 });
            sdSyn.Add("assume", new object[] { "expect", 1.0 });
            sdSyn.Add("commercial", new object[] { "business", 1.0 });
            sdSyn.Add("let", new object[] { "allow", 1.0 });
            sdSyn.Add("rise", new object[] { "increase", 1.0 });
            sdSyn.Add("manual", new object[] { "guidebook", 1.0 });
            sdSyn.Add("information", new object[] { "advice", 1.0 });
            sdSyn.Add("protocol", new object[] { "etiquette", 1.0 });
            sdSyn.Add("tale", new object[] { "joke", 1.0 });
            sdSyn.Add("specially", new object[] { "especially", 1.0 });
            sdSyn.Add("accept", new object[] { "assume", 1.0 });
            sdSyn.Add("study", new object[] { "learn", 1.0 });

            sdSyn.Add("world", new object[] { "universe", 1.0 });
            sdSyn.Add("travel", new object[] { "roam", 1.0 });
            sdSyn.Add("easy", new object[] { "cool", 1.0});
            sdSyn.Add("live", new object[] { "animate", 1.0 });
            sdSyn.Add("global", new object[] { "large", 1.0 });
            sdSyn.Add("meeting", new object[] { "summit", 1.0 });
            sdSyn.Add("late", new object[] { "down", 1.0 });
            sdSyn.Add("widespread", new object[] { "common", 1.0 });
            sdSyn.Add("language", new object[] { "semantic", 1.0 });
            sdSyn.Add("mean", new object[] { "doom", 1.0 });
            sdSyn.Add("corresponding", new object[] { "matching", 1.0 });
            sdSyn.Add("custom", new object[] { "duty", 1.0 });
        }
        public static void ReplaceSyn(SortedDictionary<Word, double>[] arrSD)
        {
            foreach (SortedDictionary<Word, double> sd in arrSD)
            {
                Word[] arrK = sd.Keys.ToArray();
                foreach (Word w in arrK)
                {
                    object[] arr;
                    if (sdSyn.TryGetValue(w.norm, out arr))
                    {
                        double freq,p=(double)arr[1];
                        string syn=(string)arr[0];
                        if (sd.TryGetValue(new Word() { norm = syn }, out freq))
                        {
                            sd[new Word() { pos=w.pos, form=w.form,norm = syn }] += sd[w] * p;
                        }
                        else
                        {
                            Word wnew = new Word() { pos = w.pos, form = w.form, norm = syn };
                            sd.Add(wnew, sd[w] * p);
                        }
                        sd.Remove(w);
                    }
                }
            }
        }
        public static double BoolCompare(SortedDictionary<Word, double>[] arrSDx, SortedDictionary<Word, double>[] arrSDy,
            out double[] arrPx, out double[] arrPy, out double[] arrPxy, out double hx, out double hy, out double hxy, out string rep)
        {
            List<Word> lx = new List<Word>(), ly = new List<Word>();
            for (int i = 0; i < arrSDx.Length; i++)
            {
                foreach (Word w in arrSDx[i].Keys)
                {
                    int j = lx.BinarySearch(w);
                    if (j < 0)
                        lx.Insert(~j, w);
                }
                foreach (Word w in arrSDy[i].Keys)
                {
                    int j = ly.BinarySearch(w);
                    if (j < 0)
                        ly.Insert(~j, w);
                }
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
            double nVoc = 125000;
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
                hx + hy - hxy, hx, (hx + hy - hxy) / hx);
            return hx + hy - hxy;
        }
        public static SortedDictionary<Word, double> TFVector(List<Word> lw, out double len)
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
            for (int i = 0; i < arrV.Length; i++)
            {
                len += arrV[i] * arrV[i];
            }
            len = Math.Sqrt(len);
            return sd;
        }
        public static void GenReplace()
        {
            //string t1 = "A world guide to good manners. Travelling to all corners of the world gets easier and easier. We live in a global village, but how well do we know and understand each other? Here is a simple test. Imagine you have arranged a meeting at four o’clock. What time should you expect your foreign business colleagues to arrive? If they are German, they will be bang on time. If they are American, they will probably be fifteen minutes early. If they are British, they will be fifteen minutes late, and you should allow up to an hour for the Italians. When the European Community began to increase in size, several guidebooks appeared giving advice on international etiquette. At first mane people thought this was a joke, especially the British, who seemed to assume that the widespread understanding of their language meant a corresponding understanding of English customs. Very soon they had to change their ideas, as they realized that they had a lot to learn about how to behave with their foreign business friends";
            //string t1 = "A world guide to good manners. Travelling to all corners of the world gets easier and easier. We live in a global village, but how well do we know and understand each other? Here is a simple test. Imagine you have arranged a meeting at four o’clock. What time should you expect your foreign business colleagues to arrive? If they are German, they will be bang on time. If they are American, they will probably be fifteen minutes early. If they are British, they will be fifteen minutes late, and you should allow up to an hour for the Italians. When the European Community began to increase in size, several guidebooks appeared giving advice on international etiquette. At first many people thought this was a joke, especially the British, who seemed to assume that the widespread understanding of their language meant a corresponding understanding of English customs. Very soon they had to change their ideas, as they realized that they had a lot to learn about how to behave with their foreign business friends";
            string t1 = "A world guide to good manners. Travelling to all corners of the world gets easier and easier. We live in a global village, but how well do we know and understand each other? Here is a simple test. Imagine you have arranged a meeting at four o’clock. What time should you expect your foreign business colleagues to arrive? If they are German, they will be bang on time. If they are American, they will probably be fifteen minutes early. If they are British, they will be fifteen minutes late, and you should allow up to an hour for the Italians. When the European Community began to increase in size, several guidebooks appeared giving advice on international etiquette. At first many people thought this was a joke, especially the British, who seemed to assume that become become become become become become become become become become become become become. become become become become become become become become, become become become become become become become become become become become become become become become become become become become";
            string t2 = @"A nice introduction to effective cooking.
                        Flying to some places of the city becomes worse and worse. They walk in a local town, but how good do they see and hear each other? There is a complex pause. Consider they have collapsed a hope at five o’clock. What date should they hope their tourist school boys to come? If it is lucky, it will be go on date. If it is looser, it will certainly be sixteen hours later. If it is normal, it will be seventeen years earlier, or they should disallow up to an year for the cookers. 
                        When the big dog began to increase in value, some books seemed taking opinion on national features. At second some girls supported this was a surprise, especially the normal, they wanted to know that the common knowledge of this opinion was a bad enjoying of north features. Very late it had to overcome his property, as he looked that he had a lot to study about how to cook with their school home parents";
            //t2 = @"A nice leader to respectable behaviors. Roaming to all places of the kitchen becomes worse and worse. They are in a large community, but how respective do they identify and appreciate each other? There is a complex exam. Visualize they have locked a meeting at five o’clock. What date should they assume their some commercial fans to visit? If it is stupid, it will be pause on date. If it is north, it will certainly be twelve hours later. If it is official, it will be one year late, and they should let up to an year for the boys. When the big dog stopped to rise in value, some manuals happened taking information on strange protocol. At second some boys expected it was a tale, specially the north, who wanted to accept that the good taking of his break salt a some accepting of south roses. Very late she had to spent her weapons, as she wanted that she had a lot to study about how to go with their some school girls";
            //t2 = "A universe leader to respectable behaviors. Roaming to all places of the kitchen becomes cooler and cooler. They animate in a large community, but how respective do they identify and appreciate each other? There is a complex exam. Visualize they have locked a summit at five o’clock. What date should they assume their some commercial fans to visit? If it is stupid, it will be pause on date. If it is north, it will certainly be twelve hours later. If it is official, it will be one year dawn, and they should let up to an year for the boys. When the big dog stopped to rise in value, some manuals happened taking information on strange protocol. At second some boys expected it was a tale, specially the north, who wanted to accept that the common appreciating of his semantic doomed a matching accepting of south duties. Very late she had to spent her weapons, as she wanted that she had a lot to study about how to go with their some school girls";
            t2 = "A universe leader to respectable behaviors. Roaming to all places of the kitchen becomes cooler and cooler. They animate in a large community, but how respective do they identify and appreciate each other? There is a complex exam. Visualize they have locked a summit at five o’clock. What date should they assume their some commercial fans to visit? If it is stupid, it will be pause on date. If it is north, it will certainly be twelve hours later. If it is official, it will be one year dawn, and they should let up to an year for the boys. When the big dog stopped to rise in value, some manuals happened taking information on strange protocol. At second some boys expected it was a tale, specially the north, who wanted to accept that get get get get get get get get get get get get get get get get get get get get get get get get get get get get get get get get get get get get get get get get";
            Regex reg = new Regex(@"\b[а-яa-zА-ЯA-Z]+'?-?[а-яa-zА-ЯA-Z]+\b|a|i|A|I");
            MatchCollection mc1 = reg.Matches(t1), mc2 = reg.Matches(t2);
            Match[] arr1 = new Match[mc1.Count], arr2 = new Match[mc2.Count];
            mc1.CopyTo(arr1, 0);
            mc2.CopyTo(arr2, 0);
            List<Match> lm1 = new List<Match>(arr1), lm2 = new List<Match>(arr2);
            List<int> lReplace = new List<int>();
            for (int i = 0; i < 20; i++)
            {
                for (int j = i; j < lm1.Count; j += 20)
                {
                    lReplace.Add(j);
                }
                lReplace.Sort();
                lReplace.Reverse();
                string ans = t1;

                double sumW = 0;

                foreach (int index in lReplace)
                {
                    ans = ans.Remove(lm1[index].Index, lm1[index].Length);
                    ans = ans.Insert(lm1[index].Index, lm2[index].Value);

                    //
                    List<Word> lw = Parse(lm2[index].Value);
                    string norm = lw[0].norm;
                    object[] arr;
                    if (sdSyn.TryGetValue(norm, out arr))
                        sumW += (double)arr[1];
                    //
                }
                //double mark = (lm1.Count - lReplace.Count) * 100 / lm1.Count;
                double mark = (lm1.Count - lReplace.Count + sumW) * 100 / lm1.Count;
                TInput.C_EAnswer a = new TInput.C_EAnswer("test", "", "", t1, ans, new TInput.Tutor[] { new TInput.Tutor("", mark) });
                a.Save("t" + i.ToString() + ".xml");
            }
        }
        public static void GenRep()
        {
            LoadDic("eng.bin");
            string t1 = "A world guide to good manners. Travelling to all corners of the world gets easier and easier. We live in a global village, but how well do we know and understand each other? Here is a simple test. Imagine you have arranged a meeting at four o’clock. What time should you expect your foreign business colleagues to arrive? If they are German, they will be bang on time. If they are American, they will probably be fifteen minutes early. If they are British, they will be fifteen minutes late, and you should allow up to an hour for the Italians. When the European Community began to increase in size, several guidebooks appeared giving advice on international etiquette. At first many people thought this was a joke, especially the British, who seemed to assume that the widespread understanding of their language meant a corresponding understanding of English customs. Very soon they had to change their ideas, as they realized that they had a lot to learn about how to behave with their foreign business friends";
            List<Word> lwx = Parse(t1), lwxSorted = Parse(t1);
            lwxSorted.Sort();
            string[] arrW=dicSyn.Keys.ToArray();
            Random rnd=new Random();
            int replaced = 0;
            for (int i = 0; i < 20; i++)
            {
                for (int j = i; j < lwx.Count; j += 20)
                {
                    string w;
                    do
                    {
                        int index = rnd.Next(arrW.Length);
                        w = arrW[index];
                    }
                    while (lwxSorted.BinarySearch(new Word() { norm = w }) >= 0);
                    lwx[j].norm = w;
                    replaced++;
                }
                string t2 = "";
                foreach (Word w in lwx)
                {
                    t2 += w.norm + " ";
                }
                double mark = (double)(lwx.Count - replaced) / lwx.Count*100;
                TInput.C_EAnswer a = new TInput.C_EAnswer("test", "", "", t1, t2, new TInput.Tutor[] { new TInput.Tutor("", mark) });
                a.Save("r" + i.ToString() + ".xml");
            }
        }
        public static void GenRemove()
        {
            string t1 = "A world guide to good manners. Travelling to all corners of the world gets easier and easier. We live in a global village, but how well do we know and understand each other? Here is a simple test. Imagine you have arranged a meeting at four o’clock. What time should you expect your foreign business colleagues to arrive? If they are German, they will be bang on time. If they are American, they will probably be fifteen minutes early. If they are British, they will be fifteen minutes late, and you should allow up to an hour for the Italians. When the European Community began to increase in size, several guidebooks appeared giving advice on international etiquette. At first mane people thought this was a joke, especially the British, who seemed to assume that the widespread understanding of their language meant a corresponding understanding of English customs. Very soon they had to change their ideas, as they realized that they had a lot to learn about how to behave with their foreign business friends";
            Regex reg = new Regex(@"\b[а-яa-zА-ЯA-Z]+'?-?[а-яa-zА-ЯA-Z]+\b|a|i|A|I");
            MatchCollection mc1 = reg.Matches(t1);
            Match[] arr1 = new Match[mc1.Count];
            mc1.CopyTo(arr1, 0);
            List<Match> lm1 = new List<Match>(arr1);
            List<int> lReplace = new List<int>();
            for (int i = 0; i < 20; i++)
            {
                for (int j = i; j < lm1.Count; j += 20)
                {
                    lReplace.Add(j);
                }
                lReplace.Sort();
                lReplace.Reverse();
                string ans = t1;
                foreach (int index in lReplace)
                {
                    ans = ans.Remove(lm1[index].Index, lm1[index].Length);
                }
                double mark = (lm1.Count - lReplace.Count) * 100 / lm1.Count;
                TInput.C_EAnswer a = new TInput.C_EAnswer("test", "", "", t1, ans, new TInput.Tutor[] { new TInput.Tutor("", mark) });
                a.Save("replace" + i.ToString() + ".xml");
            }
        }
        public static List<Word> Opened(List<Word> lwx)
        {
            List<string> lOpenedPos = new List<string>(new string[] {
            "Существительное", "Глагол", "Прилагательное", "Наречие", "Числительное", "Слово_из_другого_языка",
                               "Сокращенная_форма_служебного_глагола_с_подлежащим", "Слово_с_большой_буквы", "Нормализовано_эвристически",
                               "сущ", "глаг", "прил", "нар", "прич", "дееприч", "неизм", "предикатив", "иностр", "загл_буква", "эвристика" });
            lOpenedPos.Sort();
            List<Word> lw = new List<Word>();
            foreach (Word w in lwx)
            {
                if (lOpenedPos.BinarySearch(w.pos) >= 0)
                    lw.Add(w);
            }
            return lw;
        }
        public static SortedDictionary<Word, double>[] POS(List<Word> lw)
        {
            SortedDictionary<string, SortedDictionary<Word, double>> sdsd = new SortedDictionary<string, SortedDictionary<Word, double>>();
            List<string> lS = new List<string>();
            lS.AddRange(Enum.GetNames(typeof(AttributeEn)));
            lS.AddRange(Enum.GetNames(typeof(AttributeRu)));
            foreach (string s in lS)
            {
                sdsd.Add(s, new SortedDictionary<Word, double>());
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
        public static List<string> lPosFilteredEn = new List<string>(new string[] {
            "Существительное", "Глагол", "Прилагательное", "Наречие", "Числительное", "Слово_из_другого_языка",
                               "Слово_с_большой_буквы", "Нормализовано_эвристически", "Сокращенная_форма_служебного_глагола_с_подлежащим" });
        public static List<string> lPosFilteredRu = new List<string>(new string[] {           
                               "сущ", "глаг", "прил", "нар", "прич", "дееприч", "неизм", "предикатив", "иностр", "загл_буква", "эвристика" });
        
        
        
        
        
        public static string language = "ru";
        public static string[] arrPosEn = new string[] { AttributeEn.Существительное.ToString(), AttributeEn.Глагол.ToString(),
            AttributeEn.Прилагательное.ToString(),AttributeEn.Наречие.ToString(),AttributeEn.Числительное.ToString(),
            AttributeEn.Местоимение.ToString(), AttributeEn.Междометие.ToString(),AttributeEn.Частица.ToString(),
            AttributeEn.Союз.ToString(),AttributeEn.Предлог.ToString(),AttributeEn.Вводное_слово.ToString(),
            AttributeEn.Артикль_или_другой_определитель.ToString(), AttributeEn.Местоимение_или_определитель.ToString(),
            AttributeEn.Слово_из_другого_языка.ToString(),AttributeEn.Сокращенная_форма_служебного_глагола_с_подлежащим.ToString(),
            AttributeEn.Слово_из_цифр.ToString(),AttributeEn.Причастие.ToString(),
            AttributeEn.Слово_с_большой_буквы.ToString(),AttributeEn.Нормализовано_эвристически.ToString(),
        };
        public static string[] arrPosRu = new string[] { 
            AttributeRu.Существительное.ToString(), AttributeRu.Глагол.ToString(), AttributeRu.Прилагательное.ToString(), AttributeRu.Наречие.ToString(),
            AttributeRu.Числительное.ToString(),AttributeRu.Местоимение.ToString(),AttributeRu.Причастие.ToString(),AttributeRu.Деепричастие.ToString(),
            AttributeRu.Междометие.ToString(),AttributeRu.Частица.ToString(),AttributeRu.Союз.ToString(),AttributeRu.Предлог.ToString(),
            AttributeRu.Вводное_слово.ToString(),AttributeRu.Предикатив.ToString(),AttributeRu.Слово_из_другого_языка.ToString(),AttributeRu.Слово_из_цифр.ToString(),
            AttributeRu.Слово_с_большой_буквы.ToString(),AttributeRu.Нормализовано_эвристически.ToString()
        };
        
        public static List<string> lPosFiltered;        
        public static SortedDictionary<string, List<Word>> cache=new SortedDictionary<string,List<Word>>();
        public static SortedDictionary<string, List<string>> dicSyn=new SortedDictionary<string,List<string>>();
        public static List<Word> Parse(string text)
        {
            if (cache.ContainsKey(text))
                return cache[text];
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
                        if((AttributeRu)pos==AttributeRu.Инфинитив)
                            pos=(int)AttributeRu.Глагол;
                        list.Add(new Word()
                        {
                            form = reader.GetAttribute("Form"),
                            norm = reader.GetAttribute("Norm"),
                            pos = ((AttributeRu)pos).ToString()
                        });
                    }
                }
                fs.Close();
                cache.Add(text, list);
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
                        if((AttributeEn)pos==AttributeEn.Инфинитив)
                            pos=(int)AttributeEn.Глагол;
                        list.Add(new Word()
                        {
                            form = reader.GetAttribute("Form"),
                            norm = reader.GetAttribute("Norm"),
                            pos = ((AttributeEn)pos).ToString()
                        });
                    }
                }
                fs.Close();
                cache.Add(text, list);
                return list;
            }
        }
        public static SortedDictionary<Word, double>[] POSFilter(List<Word> lw)
        {
            SortedDictionary<string, SortedDictionary<Word, double>> sdsd = new SortedDictionary<string, SortedDictionary<Word, double>>();
            foreach (string s in lPosFiltered)
            {
                sdsd.Add(s, new SortedDictionary<Word, double>());
            }
            foreach (Word w in lw)
            {
                if (lPosFiltered.BinarySearch(w.pos) < 0)
                    continue;
                if (sdsd[w.pos].ContainsKey(w))
                    sdsd[w.pos][w] += 1;
                else
                    sdsd[w.pos].Add(w, 1);
            }
            return sdsd.Values.ToArray();
        }
        public static void Syn(SortedDictionary<Word, double>[] arrSDx, SortedDictionary<Word, double>[] arrSDy, out string rep)
        {
            rep = "<table border=1 cellspacing=0><tr><td>ТекстX<td>ТекстY<tr><td>";
            for (int i = 0; i < arrSDx.Length; i++)
            {
                Word[] arrK = arrSDx[i].Keys.ToArray();
                for (int j = 0; j < arrK.Length; j++)
                {
                    if (arrSDy[i].ContainsKey(arrK[j]))
                        continue;
                    List<string> lS;
                    if (dicSyn.TryGetValue(arrK[j].norm, out lS))
                    {
                        List<Word> lW = new List<Word>();
                        foreach (string s in lS)
                        {
                            Word w = new Word() { norm = s, pos = arrK[j].pos };
                            if (arrSDy[i].ContainsKey(w))
                                lW.Add(w);
                        }
                        if (lW.Count > 0)
                        {
                            double freq = arrSDx[i][arrK[j]];
                            arrSDx[i].Remove(arrK[j]);
                            rep += string.Format("{0}({1})->", arrK[j].norm, freq);
                            freq /= lW.Count;
                            foreach (Word w in lW)
                            {
                                if (arrSDx[i].ContainsKey(w))
                                {
                                    arrSDx[i][w] += freq;
                                    rep += string.Format("{0}(+{1}) ", w.norm, freq);
                                }
                                else
                                {
                                    arrSDx[i].Add(w, freq);
                                    rep += string.Format("{0}({1}) ", w.norm, freq);
                                }

                            }
                            rep += "<br>";
                        }
                    }
                }
            }
            rep += "<td>";
            for (int i = 0; i < arrSDy.Length; i++)
            {
                Word[] arrK = arrSDy[i].Keys.ToArray();
                for (int j = 0; j < arrK.Length; j++)
                {
                    if (arrSDx[i].ContainsKey(arrK[j]))
                        continue;
                    List<string> lS;
                    if (dicSyn.TryGetValue(arrK[j].norm, out lS))
                    {
                        List<Word> lW = new List<Word>();
                        foreach (string s in lS)
                        {
                            Word w = new Word() { norm = s, pos = arrK[j].pos };
                            if (arrSDx[i].ContainsKey(w))
                                lW.Add(w);
                        }
                        if (lW.Count > 0)
                        {
                            double freq = arrSDy[i][arrK[j]];
                            arrSDy[i].Remove(arrK[j]);
                            rep += string.Format("{0}({1})->", arrK[j].norm, freq);
                            freq /= lW.Count;
                            foreach (Word w in lW)
                            {
                                if (arrSDy[i].ContainsKey(w))
                                {
                                    arrSDy[i][w] += freq;
                                    rep += string.Format("{0}(+{1}) ", w.norm, freq);
                                }
                                else
                                {
                                    arrSDy[i].Add(w, freq);
                                    rep += string.Format("{0}({1}) ", w.norm, freq);
                                }

                            }
                            rep += "<br>";
                        }
                    }
                }
            }
            rep += "</table>";
        }
        public static double KuznetsovCompare(SortedDictionary<Word, double>[] arrSDx,
            SortedDictionary<Word, double>[] arrSDy,
            out double hx, out double hy, out double hxy, out double[] arrI, // массив совм. инф. по ч.р., посл. эл. соотв. null
            out string rep)
        {
            //ReplaceSyn(arrSDx);
            //ReplaceSyn(arrSDy);

            // совместное распределение
            List<WW> lWW = new List<WW>();
            for (int i = 0; i < arrSDx.Length; i++)
            {
                Word[] arrKey = arrSDx[i].Keys.ToArray();
                double[] arrVal = arrSDx[i].Values.ToArray();
                for (int j = 0; j < arrKey.Length; j++)
                {
                    double val;
                    if (arrSDy[i].TryGetValue(arrKey[j], out val))
                    {
                        lWW.Add(new WW() { wx = arrKey[j], wy = arrKey[j], freq = Math.Min(arrVal[j], val) });
                        if (arrVal[j] > val)
                                lWW.Add(new WW() { wx = arrKey[j], wy = null, freq = Math.Abs(arrVal[j] - val) });
                        //if (arrVal[j] < val) !!!
                            //lWW.Add(new WW() { wx = null, wy = arrKey[j], freq = Math.Abs(arrVal[j] - val) }); !!!
                    }
                    else
                    {
                       lWW.Add(new WW() { wx = arrKey[j], wy = null, freq = arrVal[j] });
                    }
                }
                
                arrKey = arrSDy[i].Keys.ToArray();
                arrVal = arrSDy[i].Values.ToArray();
                for (int j = 0; j < arrKey.Length; j++)
                {
                    double val;
                    if (!arrSDx[i].TryGetValue(arrKey[j], out val))
                    {
                        //lWW.Add(new WW() { wx = null, wy = arrKey[j], freq = arrVal[j] }); !!!
                    }
                }
            }

            double n = 0;
            for (int i = 0; i < lWW.Count; i++)
            {
                n += lWW[i].freq;
            }
            // Hx
            double[] arrPHx = new double[lPosFiltered.Count + 1]; // посл. элемент - вероятность null
            for (int i = 0; i < lWW.Count; i++)
            {
                if (lWW[i].wx == null)
                {
                    arrPHx[lPosFiltered.Count]+=lWW[i].freq;
                    continue;
                }
                int j = lPosFiltered.BinarySearch(lWW[i].wx.pos);
                arrPHx[j]+=lWW[i].freq;
            }
            for (int i = 0; i < arrPHx.Length; i++)
            {
                arrPHx[i] /= n;
            }

            // Hy
            double[] arrPHy = new double[lPosFiltered.Count + 1]; // посл. элемент - вероятность null
            for (int i = 0; i < lWW.Count; i++)
            {
                if (lWW[i].wy == null)
                {
                    arrPHy[lPosFiltered.Count]+=lWW[i].freq;
                    continue;
                }
                int j = lPosFiltered.BinarySearch(lWW[i].wy.pos);
                arrPHy[j]+=lWW[i].freq;
            }
            for (int i = 0; i < arrPHy.Length; i++)
            {
                arrPHy[i] /= n;
            }

            // Hxy
            double[] arrPHxyX = new double[lPosFiltered.Count];
            double[] arrPHxyY = new double[lPosFiltered.Count];
            double[] arrPHxyXY = new double[lPosFiltered.Count];
            for (int i = 0; i < lWW.Count; i++)
            {
                if (lWW[i].wx != null&&lWW[i].wy!=null)
                {
                    int j = lPosFiltered.BinarySearch(lWW[i].wx.pos);
                    arrPHxyXY[j]+=lWW[i].freq;
                }
                else if (lWW[i].wy == null)
                {
                    int j = lPosFiltered.BinarySearch(lWW[i].wx.pos);
                    arrPHxyX[j] += lWW[i].freq;
                }
                else
                {
                    int j = lPosFiltered.BinarySearch(lWW[i].wy.pos);
                    arrPHxyY[j] += lWW[i].freq;
                }                
            }
            for (int i = 0; i < arrPHxyX.Length; i++)
            {
                arrPHxyX[i] /= n;
                arrPHxyY[i] /= n;
                arrPHxyXY[i] /= n;
            }

            // расчет arrI
            arrI = new double[arrPHx.Length];
            for (int i = 0; i < arrPHx.Length; i++)
            {
                if (arrPHx[i] > 0)
                    arrI[i] -= arrPHx[i] * Math.Log(arrPHx[i]);
                if (arrPHy[i] > 0)
                    arrI[i] -= arrPHy[i] * Math.Log(arrPHy[i]);
                if (i < arrPHxyX.Length)
                {
                    if (arrPHxyX[i] > 0)
                        arrI[i] += arrPHxyX[i] * Math.Log(arrPHxyX[i]);
                    if (arrPHxyY[i] > 0)
                        arrI[i] += arrPHxyY[i] * Math.Log(arrPHxyY[i]);
                    if (arrPHxyXY[i] > 0)
                        arrI[i] += arrPHxyXY[i] * Math.Log(arrPHxyXY[i]);
                }
            }            
            
            // энтропии
            hx = hy = hxy = 0;
            for (int i = 0; i < arrPHx.Length; i++)
            {
                if (arrPHx[i] > 0)
                    hx -= arrPHx[i] * Math.Log(arrPHx[i]);
                if (arrPHy[i] > 0)
                    hy -= arrPHy[i] * Math.Log(arrPHy[i]);
            }
            for (int i = 0; i < arrPHxyX.Length; i++)
            {
                if (arrPHxyX[i] > 0)
                    hxy -= arrPHxyX[i] * Math.Log(arrPHxyX[i]);
                if (arrPHxyY[i] > 0)
                    hxy -= arrPHxyY[i] * Math.Log(arrPHxyY[i]);
                if (arrPHxyXY[i] > 0)
                    hxy -= arrPHxyXY[i] * Math.Log(arrPHxyXY[i]);
            }


            // совместное распределение полное
            lWW = new List<WW>();
            for (int i = 0; i < arrSDx.Length; i++)
            {
                Word[] arrKey = arrSDx[i].Keys.ToArray();
                double[] arrVal = arrSDx[i].Values.ToArray();
                for (int j = 0; j < arrKey.Length; j++)
                {
                    double val;
                    if (arrSDy[i].TryGetValue(arrKey[j], out val))
                    {
                        lWW.Add(new WW() { wx = arrKey[j], wy = arrKey[j], freq = Math.Min(arrVal[j], val) });
                        if (arrVal[j] > val)
                            lWW.Add(new WW() { wx = arrKey[j], wy = null, freq = Math.Abs(arrVal[j] - val) });
                        if (arrVal[j] < val)
                        lWW.Add(new WW() { wx = null, wy = arrKey[j], freq = Math.Abs(arrVal[j] - val) });
                    }
                    else
                    {
                        lWW.Add(new WW() { wx = arrKey[j], wy = null, freq = arrVal[j] });
                    }
                }

                arrKey = arrSDy[i].Keys.ToArray();
                arrVal = arrSDy[i].Values.ToArray();
                for (int j = 0; j < arrKey.Length; j++)
                {
                    double val;
                    if (!arrSDx[i].TryGetValue(arrKey[j], out val))
                    {
                        lWW.Add(new WW() { wx = null, wy = arrKey[j], freq = arrVal[j] });
                    }
                }
            }
            n = 0;
            for (int i = 0; i < lWW.Count; i++)
            {
                n += lWW[i].freq;
            }
            
            rep = "Объединение тектов по словам<table border = 1 cellspacing = 0><tr><td>Слово текста X<td>Слово текста Y<td>Вероятность";
            for (int i = 0; i < lWW.Count; i++)
            {
                rep += string.Format("<tr><td>{0}<td>{1}<td>{2:g5}", lWW[i].wx == null ? "-" : lWW[i].wx.norm,
                    lWW[i].wy == null ? "-" : lWW[i].wy.norm, lWW[i].freq / n);
            }
            rep += "</table>";

            rep += string.Format("I<sub>xy</sub> = H<sub>x</sub> + H<sub>y</sub> - H<sub>xy</sub> = {0:g5} + {1:g5} - {2:g5} = {3:g5}<br>",
                hx, hy, hxy, hx + hy - hxy);
            return hx + hy - hxy;
        }        
        public static double CosCompare(SortedDictionary<Word, double>[] arrSDx, SortedDictionary<Word, double>[] arrSDy, out string rep)
        {
            SortedDictionary<Word, double> arrFreqX = new SortedDictionary<Word, double>(), arrFreqY = new SortedDictionary<Word, double>();
            Word[] arrK;
            for (int i = 0; i < arrSDx.Length; i++)
            {
                arrK = arrSDx[i].Keys.ToArray();
                foreach (Word w in arrK)
                {
                    try
                    {
                        arrFreqX.Add(w, arrSDx[i][w]);
                    }
                    catch
                    {
                        arrFreqX[w] += arrSDx[i][w];
                    }
                }
            }
            for (int i = 0; i < arrSDy.Length; i++)
            {
                arrK = arrSDy[i].Keys.ToArray();
                foreach (Word w in arrK)
                {
                    try
                    {
                        arrFreqY.Add(w, arrSDy[i][w]);
                    }
                    catch
                    {
                        arrFreqY[w] += arrSDy[i][w];
                    }
                }
            }
            double s = 0, xlen = 0, ylen = 0;
            arrK = arrFreqX.Keys.ToArray();
            double[] arrV = arrFreqX.Values.ToArray();
            for (int i = 0; i < arrK.Length; i++)
            {
                if (arrFreqY.ContainsKey(arrK[i]))
                    s += arrV[i] * arrFreqY[arrK[i]];
                xlen += arrV[i] * arrV[i];
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
                ylen += arrV[i] * arrV[i];
            }
            xlen = Math.Sqrt(xlen);
            ylen = Math.Sqrt(ylen);
            double cos = s / (xlen * ylen);
            rep += string.Format("</table>cos(fi) = (X,Y) / (|X|*|Y|) = {0:g4} / ({1:g4} * {2:g4}) = {3:g4}<br>", s, xlen, ylen, cos);
            return cos;
        }        
        
        /*public static string Emulate(string path)
        {
            string[] arrFile = Directory.GetFiles(path);
            string rep = "", regRep = "";
            string[] arrMethod = new string[] { "Куз.", "Bool", "Присв. веса", "Cos", "Взвеш" };
            TInput.C_EAnswer[] arrAns = new TInput.C_EAnswer[arrFile.Length];
            double[] arrTutMark = new double[arrFile.Length];
            double[][] mMark = new double[arrMethod.Length][];
            List<string> lPos = new List<string>(new string[] {
            "Существительное", "Глагол", "Прилагательное", "Наречие", "Числительное", "Слово_из_другого_языка",
                               "Слово_с_большой_буквы", "Нормализовано_эвристически", "Сокращенная_форма_служебного_глагола_с_подлежащим",
                               "сущ", "глаг", "прил", "нар", "прич", "дееприч", "неизм", "предикатив", "иностр", "загл_буква", "эвристика" });
            lPos.Sort();            
            for (int i = 0; i < arrMethod.Length; i++)
            {
                mMark[i] = new double[arrFile.Length];
            }
            double[][] mH = new double[lPos.Count][];
            for (int i = 0; i < mH.Length; i++)
            {
                mH[i] = new double[arrFile.Length];
            }
            for (int i = 0; i < arrFile.Length; i++)
            {
                arrAns[i] = new TInput.C_EAnswer(arrFile[i]);
                arrTutMark[i] = arrAns[i].GetTutor(0).Mark;
            }
            string htbl = "<table border=1 cellspacing=0><tr><td>Hx<td>Hy<td>Hxy";
            for (int i = 0; i < arrAns.Length; i++)
            {
                List<Word> lwx = Parse(arrAns[i].p_etalon_answer), lwy = Parse(arrAns[i].p_student_answer);
                double[] arrPx, arrPy, arrPxy;
                double hx, hy, hxy, I;
                string s,syn;
                SortedDictionary<Word, double>[] arrSDx=POSFilter(lwx), arrSDy=POSFilter(lwy);
                Syn(arrSDx, arrSDy, out syn);
                
                //ReplaceSyn(arrSDy);

                I = KuznetsovCompare(arrSDx, arrSDy, out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out s);
                htbl += string.Format("<tr><td>{0:g4}<td>{1:g4}<td>{2:g4}", hx, hy,hxy);
                double hw=0;
                for (int j = 0; j < lPos.Count; j++)
                {
                    double h = 0;
                    if (arrPx[j] > 0)
                        h -= arrPx[j] * Math.Log(arrPx[j]);
                    if (arrPy[j] > 0)
                        h -= arrPy[j] * Math.Log(arrPy[j]);
                    if (arrPxy[j] > 0)
                        h -= arrPxy[j] * Math.Log(arrPxy[j]);
                    mH[j][i] = h;
                    if(sdW.ContainsKey(lPos[j]))
                        hw += h * sdW[lPos[j]];
                }
                rep += s+syn;
                mMark[0][i] = I;
                mMark[1][i] = BoolCompare(arrSDx,arrSDy,out arrPx,out arrPy,out arrPxy,out hx,out hy,out hxy,out s)/hx;
                mMark[2][i] = hw;
                mMark[3][i] = CosCompare(arrSDx, arrSDy, out s);
                mMark[4][i]=double.NaN;                
            }
            htbl += "</table>";
            Sample sY = new Sample("y", "y", arrTutMark);
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

            List<Sample> lsX = new List<Sample>();
            for (int i = 0; i < lPos.Count; i++)
            {
                int j;
                for (j = 0; j < mH[i].Length; j++)
                {
                    if (mH[i][j] != 0)
                        break;
                }
                if (j == mH[i].Length)
                    continue;
                lsX.Add(new Sample(lPos[i], lPos[i], mH[i]));
            }
            arrReg[arrReg.Length-1] = new Regression(sY, lsX.ToArray());

            string tbl = "<table border=1 cellspacing=0><tr><td>№<td>Оценка преп.<td>Эталон. кол. сл.<td>Кол. сл. в ответе";
            for (int i = 0; i < arrMethod.Length; i++)
            {
                if (arrReg[i] == null)
                    continue;
                tbl += string.Format("<td>{0}<td>{0} мод.", arrMethod[i]);
            }
            for (int i = 0; i < arrTutMark.Length; i++)
            {
                tbl += string.Format("<tr><td>{0}<td>{1}<td>{2}<td>{3}", i, arrTutMark[i],
                    Parse(arrAns[i].p_etalon_answer).Count, Parse(arrAns[i].p_student_answer).Count);
                for (int j = 0; j < arrReg.Length; j++)
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
            return rep + "<br>Результаты" + tbl + "Сравнение методов" + regRep+"Энтропии"+htbl;

        }
        */
        public static void LoadCache(string file)
        {
            try
            {
                FileStream fs = File.Open(file, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                cache = (SortedDictionary<string, List<Word>>)bf.Deserialize(fs);
                fs.Close();
            }
            catch
            {
                cache = new SortedDictionary<string, List<Word>>();
            }
        }
        public static void LoadDic(string file)
        {
            try
            {
                FileStream fs = File.Open(file, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                dicSyn = (SortedDictionary<string, List<string>>)bf.Deserialize(fs);
                fs.Close();
            }
            catch
            {
                dicSyn = new SortedDictionary<string, List<string>>();
            }
        }
        public static void SaveCache(string file)
        {
            FileStream fs = File.Open("cache.bin", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, cache);
            fs.Close();
        }
        public static void SaveDic(string file)
        {
            FileStream fs = File.Open(file, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, dicSyn);
            fs.Close();
        }
        public static void ImportDic(string file)
        {
            StreamReader sr = new StreamReader(file, System.Text.Encoding.Default);
            string dic = sr.ReadToEnd();
            sr.Close();
            Regex regHead = new Regex(@"\r?\n(?<head>[а-яА-Яa-zA-Z]+.*[а-яА-Яa-zA-Z]+)\s*\r?\n");
            Match match = regHead.Match(dic);
            dicSyn=new SortedDictionary<string,List<string>>();
            while (match.Success)
            {
                Match next=match.NextMatch();
                string head = match.Groups["head"].Value.ToLower();
                if (!Regex.IsMatch(head, @"^\s*[а-яa-z]+-?[а-яa-z]+\s*$"))
                {
                    match = next;
                    continue;
                }
                int iNext;
                if (next.Success)
                    iNext = next.Index;
                else
                    iNext = dic.Length - 1;
                string article = dic.Substring(match.Index + match.Length, iNext - (match.Index + match.Length)).ToLower();
                article = Regex.Replace(article, @"@.*\n*.*@", "@").Replace("@", "");
                article = Regex.Replace(article, @"\(.*\)", "");
                article = Regex.Replace(article, @"ant\..*\[/trn\]", "[/trn]");
                string[] arrTag=new string[] { "ex","com","c","!trs","s","url","p","lang","ref"};
                foreach (string tag in arrTag)
                {
                    string str=string.Format(@"\[{0}\].*\n*.*\[/{0}\]", tag);
                    article = Regex.Replace(article, str, "");
                }
                MatchCollection mc = Regex.Matches(article, @"\[trn\].*\n*.*\[/trn\]");
                string trn = "";
                if (mc.Count > 0)
                {
                    foreach (Match m in mc)
                    {
                        trn += m.Value+",";
                    }
                }
                else
                    trn = article;
                trn=Regex.Replace(trn, @"\[.{1,5}\]","");
                string[] arr = trn.Split(new char[] { ',', ';','.' });                
                foreach (string s in arr)
                {
                    Match mt= Regex.Match(s, @"^\s*(?<syn>[а-яa-z]+-?[а-яa-z]+)\s*$");
                    if (mt.Success)
                    {
                        string syn=mt.Groups["syn"].Value;
                        List<string> list;
                        if (dicSyn.TryGetValue(head, out list))
                        {
                            int i = list.BinarySearch(syn);
                            if(i<0)
                                list.Insert(~i,syn);
                        }
                        else
                            dicSyn.Add(head, new List<string>(new string[] { syn }));
                        if (dicSyn.TryGetValue(syn, out list))
                        {
                            int i = list.BinarySearch(head);
                            if (i < 0)
                                list.Insert(~i, head);
                        }
                        else
                            dicSyn.Add(syn, new List<string>(new string[] { head }));
                    }
                }
                match = next;
            }
        }
        
    }
    public class CmpParams
    {
        public bool useI, useIWOpt, useIW, useCos;
        public SortedDictionary<string, double> sdW,sdWOpt;
        public Regression regI,regIWOpt,regIW,regCos;
        public void Save(string file)
        {
            FileStream fs = File.Open(file, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, useI);
            bf.Serialize(fs, useIWOpt);
            bf.Serialize(fs, useIW);
            bf.Serialize(fs, useCos);
            if(useIWOpt)
                bf.Serialize(fs, sdWOpt);
            if(useIW)
                bf.Serialize(fs, sdW);
            if(useI)
            bf.Serialize(fs, regI);
            if(useIWOpt)
            bf.Serialize(fs, regIWOpt);
            if(useIW)
            bf.Serialize(fs, regIW);
            if(useCos)
            bf.Serialize(fs, regCos);
            fs.Close();
        }
        public void Load(string file)
        {
            FileStream fs = File.Open(file, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            useI=(bool)bf.Deserialize(fs);
            useIWOpt=(bool)bf.Deserialize(fs);
            useIW=(bool)bf.Deserialize(fs);
            useCos=(bool)bf.Deserialize(fs);
            if(useIWOpt)
            sdWOpt=(SortedDictionary<string,double>)bf.Deserialize(fs);
            if (useIW)
            sdW = (SortedDictionary<string, double>)bf.Deserialize(fs);
            if (useI)
            regI=(Regression)bf.Deserialize(fs);
            if (useIWOpt)
            regIWOpt=(Regression)bf.Deserialize(fs);
            if (useIW)
            regIW=(Regression)bf.Deserialize(fs);
            if (useCos)
            regCos=(Regression)bf.Deserialize(fs);
            fs.Close();
        }
    }
    public class WW
    {
        public Word wx, wy;
        public double freq;
    }
}
