using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Statistics;

namespace Cmp
{
    public partial class MainForm : Form
    {
        public CmpParams cp;
        public MainForm()
        {
            InitializeComponent();
            if (Properties.Settings.Default.Language == "en")
                языкToolStripMenuItem.Checked = false;
            else
                языкToolStripMenuItem.Checked = true;
            if (Properties.Settings.Default.Syn)
                использоватьToolStripMenuItem.Checked = true;
            else
                использоватьToolStripMenuItem.Checked = false;
            Compare.language = Properties.Settings.Default.Language;
            Compare.LoadDic(Properties.Settings.Default.DicPath);
            Compare.LoadCache(Properties.Settings.Default.CachePath);
            if (Properties.Settings.Default.POSEn == null)
                Properties.Settings.Default.POSEn = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.POSRu == null)
                Properties.Settings.Default.POSRu = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.Language == "en")
            {
                if (Properties.Settings.Default.POSEn.Count == 0)
                    Compare.lPosFiltered = new List<string>(Compare.arrPosEn);
                else
                    Compare.lPosFiltered = new List<string>(Properties.Settings.Default.POSEn.Cast<string>());
            }
            else
            {
                if (Properties.Settings.Default.POSRu.Count == 0)
                    Compare.lPosFiltered = new List<string>(Compare.arrPosRu);
                else
                    Compare.lPosFiltered = new List<string>(Properties.Settings.Default.POSRu.Cast<string>());
            }
            Compare.lPosFiltered.Sort();
        }

        private void использоватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (использоватьToolStripMenuItem.Checked)
            {
                использоватьToolStripMenuItem.Checked=false;
                Properties.Settings.Default.Syn = false;
            }
            else
            {
                использоватьToolStripMenuItem.Checked=true;
                Properties.Settings.Default.Syn = true;                
            }
            Properties.Settings.Default.Save();
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                Compare.LoadDic(openFileDialog1.FileName);
                Properties.Settings.Default.DicPath = openFileDialog1.FileName;
                Properties.Settings.Default.Syn = true;
                использоватьToolStripMenuItem.Checked = true;
                Properties.Settings.Default.Save();
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки");
            }
        }

        private void просмотрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DicForm df = new DicForm();
                df.ShowDialog();
            }
            catch
            {
                MessageBox.Show("Ошибка просмотра");
            }

        }

        private void импортDSLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            try
            {
            Compare.ImportDic(openFileDialog1.FileName);
            }
            catch
            {
                MessageBox.Show("Ошибка импорта словаря");
                return;
            }
            MessageBox.Show("Импорт словаря выполнен. Укажите, куда следует сохранить словарь");
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            try
            {
            Compare.SaveDic(saveFileDialog1.FileName);
            Properties.Settings.Default.DicPath = saveFileDialog1.FileName;
            Properties.Settings.Default.Syn = true;
            использоватьToolStripMenuItem.Checked = true;
            Properties.Settings.Default.Save();
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения");
            }
        }

        private void очиститьКэшТекстовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
            Compare.cache = new SortedDictionary<string, List<Word>>();
            Compare.SaveCache(Properties.Settings.Default.CachePath);
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения");
            }
        }

        private void языкToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.POSEn == null)
                Properties.Settings.Default.POSEn = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.POSRu == null)
                Properties.Settings.Default.POSRu = new System.Collections.Specialized.StringCollection();
            if (языкToolStripMenuItem.Checked)
            {
                языкToolStripMenuItem.Checked = false;
                Compare.language = "en";
                Properties.Settings.Default.Language = "en";
            }
            else
            {
                языкToolStripMenuItem.Checked = true;
                Compare.language = "ru";
                Properties.Settings.Default.Language = "ru";
            }
            if (Properties.Settings.Default.Language == "en")
            {
                if (Properties.Settings.Default.POSEn.Count == 0)
                    Compare.lPosFiltered = new List<string>(Compare.arrPosEn);
                else
                    Compare.lPosFiltered = new List<string>(Properties.Settings.Default.POSEn.Cast<string>());
            }
            else
            {
                if (Properties.Settings.Default.POSRu.Count == 0)
                    Compare.lPosFiltered = new List<string>(Compare.arrPosRu);
                else
                    Compare.lPosFiltered = new List<string>(Properties.Settings.Default.POSRu.Cast<string>());
            }
            Compare.lPosFiltered.Sort();
            Properties.Settings.Default.Save();
        }

        private void фильтрЧастейРечиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            POSForm pf = new POSForm();
            pf.ShowDialog();
        }

        private void начатьОбучениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
            TeachForm tf = new TeachForm();
            if (tf.ShowDialog() != DialogResult.OK)
                return;
            string[] arrFile = tf.openFileDialog1.FileNames;
            SortedDictionary<string, double> sdW = new SortedDictionary<string, double>();
            for (int i = 0; i < tf.dataGridView1.Rows.Count; i++)
            {
                sdW.Add(tf.dataGridView1[0, i].Value.ToString(), double.Parse(tf.dataGridView1[1, i].Value.ToString()));
            }
            string regRep = "";
            TInput.C_EAnswer[] arrAns = new TInput.C_EAnswer[arrFile.Length];
            double[] arrTutMark = new double[arrFile.Length];
            double[] arrIMark = new double[arrFile.Length];
            double[] arrIW2Mark = new double[arrFile.Length];
            double[] arrCos = new double[arrFile.Length];
            double[][] mH = new double[Compare.lPosFiltered.Count][];
            for (int i = 0; i < mH.Length; i++)
            {
                mH[i] = new double[arrFile.Length];
            }
            int tut = int.Parse(tf.textBox1.Text)-1;
            for (int i = 0; i < arrFile.Length; i++)
            {
                arrAns[i] = new TInput.C_EAnswer(arrFile[i]);
                arrTutMark[i] = arrAns[i].GetTutor(tut).Mark;
            }
            for (int i = 0; i < arrAns.Length; i++)
            {
                List<Word> lwx = Compare.Parse(arrAns[i].p_etalon_answer), lwy = Compare.Parse(arrAns[i].p_student_answer);
                string s, syn;                
                SortedDictionary<Word, double>[] arrSDx = Compare.POSFilter(lwx), arrSDy = Compare.POSFilter(lwy);
                if(Properties.Settings.Default.Syn)
                    Compare.Syn(arrSDx, arrSDy, out syn);
                if (tf.checkBox1.Checked || tf.checkBox2.Checked || tf.checkBox3.Checked)
                {
                    double[] arrPx, arrPy, arrPxy;
                    double hx, hy, hxy;
                    arrIMark[i] = Compare.KuznetsovCompare(arrSDx, arrSDy, out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out s);
                    double hw = 0;
                    for (int j = 0; j < Compare.lPosFiltered.Count; j++)
                    {
                        double h = 0;
                        if (arrPx[j] > 0)
                            h -= arrPx[j] * Math.Log(arrPx[j]);
                        if (arrPy[j] > 0)
                            h -= arrPy[j] * Math.Log(arrPy[j]);
                        if (arrPxy[j] > 0)
                            h += arrPxy[j] * Math.Log(arrPxy[j]);
                        mH[j][i] = h;
                        double w;
                        if (sdW.TryGetValue(Compare.lPosFiltered[j], out w))
                            hw += h * w;
                    }
                    arrIW2Mark[i] = hw;
                }
                if(tf.checkBox4.Checked)
                    arrCos[i]= Compare.CosCompare(arrSDx, arrSDy, out s);
            }
            Sample sY = new Sample("Q", "Q", arrTutMark);
            Regression[] arrReg = new Regression[4];
            if(tf.checkBox1.Checked)
                arrReg[0] = new Regression(sY, new Sample[] { new Sample("I", "I", arrIMark) });
            if (tf.checkBox2.Checked)
            {
                List<Sample> lsX = new List<Sample>();
                for (int i = 0; i < Compare.lPosFiltered.Count; i++)
                {
                    int j;
                    for (j = 0; j < mH[i].Length; j++)
                    {
                        if (mH[i][j] != 0)
                            break;
                    }
                    if (j == mH[i].Length)
                        continue;
                    lsX.Add(new Sample(Compare.lPosFiltered[i], Compare.lPosFiltered[i], mH[i]));
                }
                arrReg[1] = new Regression(sY, lsX.ToArray());
            }
            if(tf.checkBox3.Checked)
            arrReg[2] = new Regression(sY, new Sample[] { new Sample("IW2", "IW2", arrIW2Mark) });
            if (tf.checkBox4.Checked)
            arrReg[3] = new Regression(sY, new Sample[] { new Sample("cos", "cos", arrCos) });
            string tbl = "<table border=1 cellspacing=0><tr><td>№<td>Оценка преподавателя<td>Число слов в эталоне"+
                "<td>Число слов в ответе";
            if (tf.checkBox1.Checked)
                tbl += "<td>Количество информации по ВСММ<td>Тарированная оценка по ВСММ";
            if (tf.checkBox2.Checked)
                tbl+="<td>Тарированная оценка по ВСММ с опт. весами";
            if (tf.checkBox3.Checked)
                tbl+="<td>Количество информации по ВСММ с зад. весами<td>Тарированная оценка по ВСММ с зад. весами";
            if (tf.checkBox4.Checked)    
                tbl+="<td>Критерий косинуса угла<td>Тарированная оценка критерия косинуса";            
            for (int i = 0; i < arrTutMark.Length; i++)
            {
                tbl += string.Format("<tr><td>{0}<td>{1}<td>{2}<td>{3}", i+1, arrTutMark[i],
                    Compare.Parse(arrAns[i].p_etalon_answer).Count, Compare.Parse(arrAns[i].p_student_answer).Count);
                if (tf.checkBox1.Checked)
                tbl += string.Format("<td>{0:g6}<td>{1:g4}", arrIMark[i], arrReg[0].arrYMod[i]);
                if (tf.checkBox2.Checked)
                tbl += string.Format("<td>{0:g4}", arrReg[1].arrYMod[i]);
                if (tf.checkBox3.Checked)
                tbl += string.Format("<td>{0:g6}<td>{1:g4}", arrIW2Mark[i], arrReg[2].arrYMod[i]);
                if (tf.checkBox4.Checked)
                tbl += string.Format("<td>{0:g6}<td>{1:g4}", arrCos[i], arrReg[3].arrYMod[i]);
            }
            tbl += "</table>";
            regRep = "<table border=1 cellspacing=0><tr><td>Метод<td>Регрессия";
            if (tf.checkBox1.Checked)
                regRep += string.Format("<tr><td>{0}<td>{1}", "Критерий совместной информации по ВСММ", arrReg[0].RegReport());
            if (tf.checkBox2.Checked)
                regRep += string.Format("<tr><td>{0}<td>{1}", "Критерий совместной информации по ВСММ с опт. весами", arrReg[1].RegReport());
            if (tf.checkBox3.Checked)
                regRep += string.Format("<tr><td>{0}<td>{1}", "Критерий совместной информации по ВСММ с зад. весами", arrReg[2].RegReport());
            if (tf.checkBox4.Checked)
                regRep += string.Format("<tr><td>{0}<td>{1}", "Критерий косинуса", arrReg[3].RegReport());            
            regRep += "</table>";
            regRep = "<br><big>Результаты</big>" + tbl + "<big><br><br>Сравнение методов</big>" + regRep;
            SortedDictionary<string, double> sdWOpt=null;
            if (tf.checkBox2.Checked)
            {
                sdWOpt = new SortedDictionary<string, double>();
                for (int i = 0; i < arrReg[1].arrSX.Length; i++)
                {
                    sdWOpt.Add(arrReg[1].arrSX[i].name, arrReg[1].arrB[1 + i]);
                }
            }
            cp = new CmpParams()
            {
                useI = tf.checkBox1.Checked,
                useIW = tf.checkBox2.Checked,
                useIW2 = tf.checkBox3.Checked,
                useCos = tf.checkBox4.Checked,
                sdW = sdW,
                sdWOpt=sdWOpt,
                regI = arrReg[0],
                regIW = arrReg[1],
                regIW2 = arrReg[2],
                regCos = arrReg[3]
            };
            RepForm rf = new RepForm(regRep);
            rf.Text = "Результаты обучения";
            rf.ShowDialog();
            }
            catch
            {
                MessageBox.Show("Ошибка обучения");
            }
        }

        private void сохранитьРезультатыОбученияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                cp.Save(saveFileDialog1.FileName);
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения");
            }
        }

        private void загрузитьРезультатыОбученияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            cp = new CmpParams();
            cp.Load(openFileDialog1.FileName);
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки");
            }
        }

        private void оценкаОтветовАОСToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                string[] arrFile = openFileDialog1.FileNames;
                string rep = "";
                TInput.C_EAnswer[] arrAns = new TInput.C_EAnswer[arrFile.Length];
                double[] arrIMark = new double[arrFile.Length];
                double[] arrIWMark = new double[arrFile.Length];
                double[] arrIW2Mark = new double[arrFile.Length];
                double[] arrCos = new double[arrFile.Length];
                double[][] mH = new double[Compare.lPosFiltered.Count][];
                for (int i = 0; i < mH.Length; i++)
                {
                    mH[i] = new double[arrFile.Length];
                }
                for (int i = 0; i < arrFile.Length; i++)
                {
                    arrAns[i] = new TInput.C_EAnswer(arrFile[i]);
                }
                string htbl = "<table border=1 cellspacing=0><tr><td>№<td>Hx<td>Hy<td>Hxy";
                for (int i = 0; i < arrAns.Length; i++)
                {
                    List<Word> lwx = Compare.Parse(arrAns[i].p_etalon_answer), lwy = Compare.Parse(arrAns[i].p_student_answer);
                    string s, syn;
                    SortedDictionary<Word, double>[] arrSDx = Compare.POSFilter(lwx), arrSDy = Compare.POSFilter(lwy);
                    rep += string.Format("<big>Пара текстов №{0}</big>", i + 1);
                    if (Properties.Settings.Default.Syn)
                    {
                        Compare.Syn(arrSDx, arrSDy, out syn);
                        rep += "<br>Нормализация по синонимам" + syn + "<br><br>";
                    }
                    if (cp.useI || cp.useIW || cp.useIW2)
                    {
                        double[] arrPx, arrPy, arrPxy;
                        double hx, hy, hxy;
                        arrIMark[i] = Compare.KuznetsovCompare(arrSDx, arrSDy, out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out s);
                        rep += "Расчет информационных характеристик текстов<br>" + s + "<br><br>";
                        htbl += string.Format("<tr><td>{0}<td>{1:g4}<td>{2:g4}<td>{3:g4}", i + 1, hx, hy, hxy);
                        double hw = 0, hwOpt = 0;
                        for (int j = 0; j < Compare.lPosFiltered.Count; j++)
                        {
                            double h = 0;
                            if (arrPx[j] > 0)
                                h -= arrPx[j] * Math.Log(arrPx[j]);
                            if (arrPy[j] > 0)
                                h -= arrPy[j] * Math.Log(arrPy[j]);
                            if (arrPxy[j] > 0)
                                h += arrPxy[j] * Math.Log(arrPxy[j]);
                            mH[j][i] = h;
                            double w;
                            if (cp.sdWOpt.TryGetValue(Compare.lPosFiltered[j], out w))
                                hwOpt += h * w;
                            if (cp.sdW.TryGetValue(Compare.lPosFiltered[j], out w))
                                hw += h * w;
                        }
                        arrIWMark[i] = hwOpt + cp.regIW.arrB[0];
                        arrIW2Mark[i] = hw;
                    }
                    if (cp.useCos)
                    {
                        arrCos[i] = Compare.CosCompare(arrSDx, arrSDy, out s);
                        rep += "Расчет критерия косинуса<br>" + s + "<br><br>";
                    }
                }
                htbl += "</table>";
                string tbl = "<table border=1 cellspacing=0><tr><td>№<td>Число слов в эталоне<td>Число слов в ответе";
                if (cp.useI)
                    tbl += "<td>Количество информации по ВСММ<td>Тарированная оценка по ВСММ";
                if (cp.useIW)
                    tbl += "<td>Тарированная оценка по ВСММ с опт. весами";
                if (cp.useIW2)
                    tbl += "<td>Количество информации по ВСММ с зад. весами<td>Тарированная оценка по ВСММ с зад. весами";
                if (cp.useCos)
                    tbl += "<td>Критерий косинуса угла<td>Тарированная оценка критерия косинуса";
                for (int i = 0; i < arrFile.Length; i++)
                {
                    tbl += string.Format("<tr><td>{0}<td>{1}<td>{2}", i + 1, Compare.Parse(arrAns[i].p_etalon_answer).Count, Compare.Parse(arrAns[i].p_student_answer).Count);
                    if (cp.useI)
                        tbl += string.Format("<td>{0:g6}<td>{1:g4}", arrIMark[i], cp.regI.RegValue(new double[] { arrIMark[i] }));
                    if (cp.useIW)
                        tbl += string.Format("<td>{0:g4}", arrIWMark[i]);
                    if (cp.useIW2)
                        tbl += string.Format("<td>{0:g6}<td>{1:g4}", arrIW2Mark[i], cp.regIW2.RegValue(new double[] { arrIW2Mark[i] }));
                    if (cp.useCos)
                        tbl += string.Format("<td>{0:g6}<td>{1:g4}", arrCos[i], cp.regCos.RegValue(new double[] { arrCos[i] }));
                }
                tbl += "</table>";
                rep += "<br><big>Результаты</big>" + tbl + "<br><br>";
                if (cp.useI || cp.useIW || cp.useIW2)
                    rep += "<br><big>Таблица энтропий информационных объектов</big>" + htbl + "<br><br>";
                RepForm rf = new RepForm(rep);
                rf.Text = "Результаты оценки";
                rf.ShowDialog();
            }
            catch
            {
                MessageBox.Show("Ошибка оценки близости текстов");
            }

        }

        private void ввестиСравниваемыеТекстыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CmpForm cf = new CmpForm();
                if (cf.ShowDialog() != DialogResult.OK)
                    return;
                string rep = "";
                TInput.C_EAnswer[] arrAns = new TInput.C_EAnswer[1];
                double[] arrIMark = new double[1];
                double[] arrIWMark = new double[1];
                double[] arrIW2Mark = new double[1];
                double[] arrCos = new double[1];
                double[][] mH = new double[Compare.lPosFiltered.Count][];
                for (int i = 0; i < mH.Length; i++)
                {
                    mH[i] = new double[1];
                }
                arrAns[0] = new TInput.C_EAnswer("", "", "", cf.textBox1.Text, cf.textBox2.Text, new TInput.Tutor[] { new TInput.Tutor() });
                string htbl = "<table border=1 cellspacing=0><tr><td>№<td>Hx<td>Hy<td>Hxy";
                for (int i = 0; i < arrAns.Length; i++)
                {
                    List<Word> lwx = Compare.Parse(arrAns[i].p_etalon_answer), lwy = Compare.Parse(arrAns[i].p_student_answer);
                    string s, syn;
                    SortedDictionary<Word, double>[] arrSDx = Compare.POSFilter(lwx), arrSDy = Compare.POSFilter(lwy);
                    rep += string.Format("<big>Пара текстов №{0}</big>", i + 1);
                    if (Properties.Settings.Default.Syn)
                    {
                        Compare.Syn(arrSDx, arrSDy, out syn);
                        rep += "<br>Нормализация по синонимам" + syn + "<br><br>";
                    }
                    if (cp.useI || cp.useIW || cp.useIW2)
                    {
                        double[] arrPx, arrPy, arrPxy;
                        double hx, hy, hxy;
                        arrIMark[i] = Compare.KuznetsovCompare(arrSDx, arrSDy, out arrPx, out arrPy, out arrPxy, out hx, out hy, out hxy, out s);
                        rep += "Расчет информационных характеристик текстов<br>" + s + "<br><br>";
                        htbl += string.Format("<tr><td>{0}<td>{1:g4}<td>{2:g4}<td>{3:g4}", i + 1, hx, hy, hxy);
                        double hw = 0, hwOpt = 0;
                        for (int j = 0; j < Compare.lPosFiltered.Count; j++)
                        {
                            double h = 0;
                            if (arrPx[j] > 0)
                                h -= arrPx[j] * Math.Log(arrPx[j]);
                            if (arrPy[j] > 0)
                                h -= arrPy[j] * Math.Log(arrPy[j]);
                            if (arrPxy[j] > 0)
                                h += arrPxy[j] * Math.Log(arrPxy[j]);
                            mH[j][i] = h;
                            double w;
                            if (cp.sdWOpt.TryGetValue(Compare.lPosFiltered[j], out w))
                                hwOpt += h * w;
                            if (cp.sdW.TryGetValue(Compare.lPosFiltered[j], out w))
                                hw += h * w;
                        }
                        arrIWMark[i] = hwOpt + cp.regIW.arrB[0];
                        arrIW2Mark[i] = hw;
                    }
                    if (cp.useCos)
                    {
                        arrCos[i] = Compare.CosCompare(arrSDx, arrSDy, out s);
                        rep += "Расчет критерия косинуса<br>" + s + "<br><br>";
                    }
                }
                htbl += "</table>";
                string tbl = "<table border=1 cellspacing=0><tr><td>№<td>Число слов в эталоне<td>Число слов в ответе";
                if (cp.useI)
                    tbl += "<td>Количество информации по ВСММ<td>Тарированная оценка по ВСММ";
                if (cp.useIW)
                    tbl += "<td>Тарированная оценка по ВСММ с опт. весами";
                if (cp.useIW2)
                    tbl += "<td>Количество информации по ВСММ с зад. весами<td>Тарированная оценка по ВСММ с зад. весами";
                if (cp.useCos)
                    tbl += "<td>Критерий косинуса угла<td>Тарированная оценка критерия косинуса";
                for (int i = 0; i < arrAns.Length; i++)
                {
                    tbl += string.Format("<tr><td>{0}<td>{1}<td>{2}", i + 1, Compare.Parse(arrAns[i].p_etalon_answer).Count, Compare.Parse(arrAns[i].p_student_answer).Count);
                    if (cp.useI)
                        tbl += string.Format("<td>{0:g6}<td>{1:g4}", arrIMark[i], cp.regI.RegValue(new double[] { arrIMark[i] }));
                    if (cp.useIW)
                        tbl += string.Format("<td>{0:g4}", arrIWMark[i]);
                    if (cp.useIW2)
                        tbl += string.Format("<td>{0:g6}<td>{1:g4}", arrIW2Mark[i], cp.regIW2.RegValue(new double[] { arrIW2Mark[i] }));
                    if (cp.useCos)
                        tbl += string.Format("<td>{0:g6}<td>{1:g4}", arrCos[i], cp.regCos.RegValue(new double[] { arrCos[i] }));
                }
                tbl += "</table>";
                rep += "<br><big>Результаты</big>" + tbl + "<br><br>";
                if (cp.useI || cp.useIW || cp.useIW2)
                    rep += "<br><big>Таблица энтропий информационных объектов</big>" + htbl + "<br><br>";
                RepForm rf = new RepForm(rep);
                rf.Text = "Результаты оценки";
                rf.ShowDialog();
            }
            catch
            {
                MessageBox.Show("Ошибка оценки близости текстов");
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Выполнил магистрант Кондауров А.С.");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            начатьОбучениеToolStripMenuItem_Click(null, null);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            сохранитьРезультатыОбученияToolStripMenuItem_Click(null, null);
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            загрузитьРезультатыОбученияToolStripMenuItem_Click(null, null);
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            оценкаОтветовАОСToolStripMenuItem_Click(null, null);
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ввестиСравниваемыеТекстыToolStripMenuItem_Click(null, null);
        }

        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            просмотрToolStripMenuItem_Click(null, null);
        }

        private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            загрузитьToolStripMenuItem_Click(null, null);
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            импортDSLToolStripMenuItem_Click(null, null);
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            фильтрЧастейРечиToolStripMenuItem_Click(null, null);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Compare.SaveCache(Properties.Settings.Default.CachePath);
        }

        private void содержаниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                HelpForm hf = new HelpForm();
                hf.Show();
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки справки");
            }
        }

    }
}
