using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System.Data.OleDb;
using System.Data;
using muWrapper;

namespace Statistics
{
    [Serializable]
    public class Sample : ICloneable
    {
        public string name, id, idHtml;
        public double[] arr;
        public int indexMin, indexMax;
        public double min, max, av, av2;
        public double sum, sum2, dev, devStd, sigma, sigmaStd;
        public double[] arrFreq, arrP, arrMid;    // частоты и середины интервалов
        public double var;                  // коэф. вариации
        public double mc1, mc2, mc3, mc4;   // центр. моменты
        public double mb1, mb2, mb3, mb4;   // нач. моменты
        public double asym, exc;            // асим. и эксцесс

        public Sample() { }
        public Sample(string name, string id, double[] arr)
        {
            this.name = name;
            this.id = idHtml = id;
            this.arr = arr;
            Calculate();
        }
        public Sample(string name, string id, string idHtml, double[] arr)
        {
            this.name = name;
            this.id = id;
            this.idHtml = idHtml;
            this.arr = arr;
            Calculate();
        }
        public Sample(double[] arr)
        {
            this.arr = arr;
            Calculate();
        }
        public double this[int index]
        {
            get { return arr[index]; }
            set { arr[index] = value; }
        }
        public void Calculate()
        {
            min = double.MaxValue;
            max = double.MinValue;
            sum = 0;
            sum2 = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] < min)
                {
                    min = arr[i];
                    indexMin = i;
                }
                if (arr[i] > max)
                {
                    max = arr[i];
                    indexMax = i;
                }
                sum += arr[i];
                sum2 += arr[i] * arr[i];
            }
            av = sum / arr.Length;
            av2 = sum2 / arr.Length;
            dev = av2 - (av * av);
            devStd = dev * arr.Length / (arr.Length - 1);
            sigma = (double)Math.Sqrt(dev);
            sigmaStd = (double)Math.Sqrt(devStd);

            // моменты
            mb1 = av;
            mb2 = sum2 / arr.Length;
            mc2 = dev;
            mc1 = mc3 = mc4 = 0;
            mb1 = mb2 = mb3 = mb4 = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                mb3 += arr[i] * arr[i] * arr[i];
                mb4 += arr[i] * arr[i] * arr[i] * arr[i];
                mc1 += arr[i] - mb1;
                mc3 += (arr[i] - mb1) * (arr[i] - mb1) * (arr[i] - mb1);
                mc4 += (arr[i] - mb1) * (arr[i] - mb1) * (arr[i] - mb1) * (arr[i] - mb1);
            }
            mb3 /= arr.Length;
            mb4 /= arr.Length;
            mc1 /= arr.Length;
            mc3 /= arr.Length;
            mc4 /= arr.Length;

            // вариация, асим. и эксцесс
            var = sigmaStd / av;
            asym = mc3 / (sigmaStd * sigmaStd * sigmaStd);
            exc = mc4 / (sigmaStd * sigmaStd * sigmaStd * sigmaStd);
        }
        public int[] DropoutErrors(double alpha, out string rep)
        {
            rep = string.Format("ОТСЕВ ПОГРЕШНОСТЕЙ ДЛЯ {0}<br>Уровень значимости: α = {1}<table border = 1 cellspacing = 0>" +
                "<tr><td>№ наблюдения<td>Значение<td>τ<sub>набл</sub><td>τ<sub>теор</sub>", name, alpha);
            List<int> lIgnore = new List<int>(), lI = new List<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                lI.Add(i);
            }
            while (true)
            {
                double min = double.MaxValue, max = double.MinValue;
                double sum = 0, sum2 = 0;
                int iMin = -1, iMax = -1;
                foreach (int i in lI)
                {
                    if (arr[i] < min)
                    {
                        min = arr[i];
                        iMin = i;
                    }
                    if (arr[i] > max)
                    {
                        max = arr[i];
                        iMax = i;
                    }
                    sum += arr[i];
                    sum2 += arr[i] * arr[i];
                }
                int n = lI.Count;
                double average = sum / n;
                double devStd = (sum2 / n - (sum / n) * (sum / n)) * n / (n - 1);
                double sigmaStd = (double)Math.Sqrt(devStd);
                double tauXMin, tauXMax;
                tauXMin = Math.Abs(min - average) / sigmaStd;
                tauXMax = Math.Abs(max - average) / sigmaStd;
                int index;
                double tauMax;
                if (tauXMin >= tauXMax)
                {
                    index = iMin;
                    tauMax = tauXMin;
                }
                else
                {
                    index = iMax;
                    tauMax = tauXMax;
                }
                double tauCrit;
                if (n <= 25)
                    tauCrit = StatTables.GetTau(1 - alpha, n);
                else
                {
                    double t = StatTables.GetStudInv(n - 2, alpha);
                    tauCrit = t * Math.Sqrt(n - 1) / Math.Sqrt(n - 2 + t * t);
                }
                if (tauMax > tauCrit)
                {
                    rep += string.Format("<tr><td>{0}<td>{1:g5}<td>{2:g3}<td>{3:g3}{4}", index, arr[index], tauMax, tauCrit,
                        n <= 25 ? "" : "(исп. t(α,n-2))");
                    int pos = lIgnore.BinarySearch(index);
                    lIgnore.Insert(~pos, index);
                    pos = lI.BinarySearch(index);
                    lI.RemoveAt(pos);
                }
                else
                    break;
            }
            return lIgnore.ToArray();
        }
        public void DoHistogram()
        {
            int k = 1 + (int)(3.32 * Math.Log10(arr.Length));
            if (k < 6 && arr.Length >= 6)
                k = 6;
            else if (k > 20)
                k = 20;
            arrFreq = new double[k];
            double h = (max - min) / k;
            for (int i = 0; i < arr.Length; i++)
            {
                int j = (int)((arr[i] - min) / h);
                if (j == arrFreq.Length)
                    j--;
                arrFreq[j]++;
            }
            arrP = new double[k];
            for (int i = 0; i < arrP.Length; i++)
            {
                arrP[i] = arrFreq[i] / (double)arr.Length;
            }
            arrMid = new double[k];
            for (int i = 0; i < arrMid.Length; i++)
                arrMid[i] = min + h * (i + 0.5);
        }
        public void AddValue(double x)
        {
            double[] arrNew = new double[arr.Length + 1];
            arr.CopyTo(arrNew, 0);
            arrNew[arr.Length] = x;
            arr = arrNew;
        }
        public void RemoveValues(int[] arrIndex)
        {
            double[] arrNew = new double[arr.Length - arrIndex.Length];
            List<int> lI = new List<int>(arrIndex);
            lI.Sort();
            lI.Reverse();
            List<double> l = new List<double>(arr);
            foreach (int i in lI)
            {
                l.RemoveAt(i);
            }
            arr = l.ToArray();
        }
        public string Report()
        {
            string s = string.Format("<P>ВЫБОРОЧНЫЕ ХАРАКТЕРИСТИКИ {0}, {1}<BR>" +
                "Минимум: {1}<SUB>min</SUB> = {2:g5}<BR>" +
                "Максимум: {1}<SUB>max</SUB> = {3:g5}<BR>" +
                "Размах выборки: w = {4:g5}<BR>" +
                "Среднее: {1}<SUB>ср</SUB> = {5:g5}<BR>" +
                "Средний квадрат: {1}<SUP>2</SUP><SUB>ср</SUB> = {6:g5}<BR>" +
                "Дисперсия: s<SUP>2</SUP> = {7:g5}<BR>" +
                "Среднее квадр. откл.: s = {8:g5}<BR>" +
                "Испр. дисперсия: s<SUP>2</SUP><SUB>испр</SUB> = {9:g5}<BR>" +
                "Испр. среднее квадр. откл.: s<SUB>испр</SUB> = {10:g5}<BR>" +
                "Асимметрия: A = {11:g5}<BR>" +
                "Эксцесс: E = {12:g5}<BR>" +
                "Коэффициент вариации: v = {13:g5}<BR></P>",
                name, idHtml, min, max, min - max, av, av2, dev, sigma, devStd, sigmaStd, asym, exc, var);
            return s;
        }
        public string CheckNorm(double alpha, out double[] arrPNorm)
        {
            NormalDistribution ndist = new NormalDistribution(av, sigmaStd);
            double step = arrMid[1] - arrMid[0];
            arrPNorm = new double[arrP.Length];
            arrPNorm[0] = ndist.CumulativeDistribution(arrMid[0] + step / 2);
            for (int i = 1; i < arrPNorm.Length - 1; i++)
            {
                arrPNorm[i] = ndist.CumulativeDistribution(arrMid[i] + step / 2) - arrPNorm[i - 1];
            }
            arrPNorm[arrPNorm.Length - 1] = 1 - arrPNorm[arrPNorm.Length - 2];
            double chi2 = 0;
            for (int i = 0; i < arrP.Length; i++)
            {
                chi2 += (arrP[i] - arrPNorm[i]) * (arrP[i] - arrPNorm[i]) / arrPNorm[i];
            }
            double chi2Theor = StatTables.GetChi2Inv(arrP.Length - 1 - 2, alpha);

            return string.Format("Проверка нормальности распределения по критерию хи-квадрат Пирсона<br>Уровень значимости: α = {0}<br>" +
                "Наблюдаемое значение критерия: χ<sup>2</sup><sub>набл</sub> = {1:g3}<br>" +
                "Теоретическое значение: χ<sup>2</sup><sub>теор</sub> = χ<sup>2</sup>(n-3,1-α) = χ<sup>2</sup>({2};{3}) = {4:g3}<br>{5}",
                alpha, chi2, arrP.Length - 3, 1 - alpha, chi2Theor,
                chi2 < chi2Theor ? "χ<sup>2</sup><sub>набл</sub><χ<sup>2</sup><sub>теор</sub> => гипотеза о нормальности принимается" :
                "χ<sup>2</sup><sub>набл</sub>≥<χ<sup>2</sup><sub>теор</sub> => гипотеза о нормальности отвергается");
        }
        public object Clone()
        {
            return new Sample(name, id, idHtml, (double[])arr.Clone());
        }
        public void Norm()
        {
            for (int i = 0; i < arr.Length; i++)
                arr[i] = (arr[i] - av) / sigma;
            Calculate();
        }
        public double Norm(double x)
        {
            return (x - av) / sigma;
        }
        public double Inv(double z)
        {
            return z * sigma + av;
        }

        public static Sample Transform(Sample s, string t)
        {
            Sample st = new Sample();
            double[] arrT = new double[s.arr.Length];
            switch (t)
            {
                case "x":
                    st.idHtml = s.idHtml;
                    for (int i = 0; i < arrT.Length; i++)
                        arrT[i] = s.arr[i];
                    break;
                case "x^2":
                    st.idHtml = string.Format("{0}<sup>2</sup>", s.idHtml);
                    for (int i = 0; i < arrT.Length; i++)
                        arrT[i] = s.arr[i] * s.arr[i];
                    break;
                case "x^3":
                    for (int i = 0; i < arrT.Length; i++)
                        arrT[i] = s.arr[i] * s.arr[i] * s.arr[i];
                    st.idHtml = string.Format("{0}<sup>3</sup>", s.idHtml);
                    break;
                case "1/x":
                    for (int i = 0; i < arrT.Length; i++)
                        arrT[i] = 1 / s.arr[i];
                    st.idHtml = string.Format("{0}<sup>-1</sup>", s.idHtml);
                    break;
                case "1/x^2":
                    for (int i = 0; i < arrT.Length; i++)
                        arrT[i] = 1 / (s.arr[i] * s.arr[i]);
                    st.idHtml = string.Format("{0}<sup>-2</sup>", s.idHtml);
                    break;
                case "1/x^3":
                    for (int i = 0; i < arrT.Length; i++)
                        arrT[i] = 1 / (s.arr[i] * s.arr[i] * s.arr[i]);
                    st.idHtml = string.Format("{0}<sup>-3</sup>", s.idHtml);
                    break;
                case "sqrt(x)":
                    for (int i = 0; i < arrT.Length; i++)
                        arrT[i] = Math.Sqrt(s.arr[i]);
                    st.idHtml = string.Format("{0}<sup>1/2</sup>", s.idHtml);
                    break;
                case "1/sqrt(x)":
                    for (int i = 0; i < arrT.Length; i++)
                        arrT[i] = 1 / Math.Sqrt(s.arr[i]);
                    st.idHtml = string.Format("{0}<sup>-1/2</sup>", s.idHtml);
                    break;
                case "ln(x)":
                    for (int i = 0; i < arrT.Length; i++)
                        arrT[i] = Math.Log(s.arr[i]);
                    st.idHtml = string.Format("ln({0})", s.idHtml);
                    break;
                case "e(x)":
                    for (int i = 0; i < arrT.Length; i++)
                        arrT[i] = Math.Exp(s.arr[i]);
                    st.idHtml = string.Format("exp({0})", s.idHtml);
                    break;
                default:
                    throw new Exception();
            }
            for (int i = 0; i < arrT.Length; i++)
                if (double.IsInfinity(arrT[i]) || double.IsNaN(arrT[i]))
                    throw new Exception();
            st.arr = arrT;
            st.id = string.Format(t.Replace("x", "{0}"), s.id);
            st.name = string.Format(t.Replace("x", "{0}"), s.name);
            st.Calculate();
            return st;
        }
        public static Sample Transform(Sample[] arrS, string expr)
        {
            Sample st = new Sample();
            Parser prs = new Parser();
            prs.SetExpr(expr);
            ParserVariable[] arrV = new ParserVariable[arrS.Length];
            for (int i = 0; i < arrS.Length; i++)
            {
                arrV[i] = new ParserVariable();
                prs.DefineVar(arrS[i].id, arrV[i]);
            }
            double[] arrT = new double[arrS[0].arr.Length];
            for (int i = 0; i < arrT.Length; i++)
            {
                for (int j = 0; j < arrV.Length; j++)
                {
                    arrV[j].Value = arrS[j].arr[i];
                }
                arrT[i] = prs.Eval();
            }
            for (int i = 0; i < arrT.Length; i++)
                if (double.IsInfinity(arrT[i]) || double.IsNaN(arrT[i]))
                    throw new Exception();
            st.arr = arrT;
            st.idHtml = st.id = expr;
            st.name = expr;
            st.Calculate();
            return st;
        }
        public static Sample Multiply(Sample s1, Sample s2)
        {
            if (s1.arr.Length != s2.arr.Length)
                throw new Exception();
            Sample s = new Sample();
            double[] arrT = new double[s1.arr.Length];
            for (int i = 0; i < arrT.Length; i++)
                arrT[i] = s1.arr[i] * s2.arr[i];
            for (int i = 0; i < arrT.Length; i++)
                if (double.IsInfinity(arrT[i]) || double.IsNaN(arrT[i]))
                    throw new Exception();
            s.idHtml = string.Format("{0}{1}", s1.idHtml, s2.idHtml);
            s.arr = arrT;
            s.id = string.Format("{0}*{1}", s1.id, s2.id);
            s.name = string.Format("{0}*{1}", s1.name, s2.name);
            s.Calculate();
            return s;
        }
        public static string[] ExcelSheets(string file)
        {
            string s = "provider = Microsoft.Jet.OLEDB.4.0;" +
                   "data source = " + file + ";" +
                   "extended properties = Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(s);
            conn.Open();
            DataTable t = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string[] arr = new string[t.Rows.Count];
            for (int i = 0; i < t.Rows.Count; i++)
                arr[i] = t.Rows[i]["TABLE_NAME"].ToString();
            return arr;
        }
        public static string[] ExcelColumns(string file, string sheet)
        {
            string s = "provider = Microsoft.Jet.OLEDB.4.0;" +
                    "data source = " + file + ";" +
                    "extended properties = Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(s);
            s = string.Format("SELECT * FROM [{0}]", sheet);
            OleDbDataAdapter da = new OleDbDataAdapter(s, conn);
            DataTable t = new DataTable();
            da.Fill(t);
            conn.Close();
            string[] arr = new string[t.Columns.Count];
            for (int i = 0; i < t.Columns.Count; i++)
                arr[i] = t.Columns[i].Caption;
            return arr;
        }
        public static Sample[] FromExcel(string file, string sheet, string[] arrColumn, string mark)
        {
            string s = "provider = Microsoft.Jet.OLEDB.4.0;" +
                    "data source = " + file + ";" +
                    "extended properties = Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(s);
            s = string.Format("SELECT * FROM [{0}]", sheet);
            OleDbDataAdapter da = new OleDbDataAdapter(s, conn);
            DataTable t = new DataTable();
            da.Fill(t);
            conn.Close();
            if (arrColumn == null)
            {
                arrColumn = new string[t.Columns.Count];
                for (int i = 0; i < t.Columns.Count; i++)
                    arrColumn[i] = t.Columns[i].Caption;
            }
            double tmp;
            List<int> lIndex = new List<int>();
            for (int i = 0; i < t.Rows.Count; i++)
            {
                int j;
                for (j = 0; j < arrColumn.Length; j++)
                    if (!double.TryParse(t.Rows[i][arrColumn[j]].ToString(), out tmp))
                        break;
                if (j == arrColumn.Length)
                    lIndex.Add(i);
            }
            Sample[] arrS = new Sample[arrColumn.Length];
            for (int i = 0; i < arrS.Length; i++)
            {
                string name = arrColumn[i];
                double[] arrValue = new double[lIndex.Count];
                for (int j = 0; j < arrValue.Length; j++)
                    arrValue[j] = double.Parse(t.Rows[lIndex[j]][name].ToString());
                arrS[i] = new Sample(name, string.Format("{0}{1}", mark, i + 1), string.Format("{0}<sub>{1}</sub>", mark, i + 1), arrValue);
            }
            return arrS;
        }
    }
    [Serializable]
    public class Regression
    {
        public double[] arrYMod;
        public double[] arrB;      // коэффициенты регрессии
        public double[,] matrC;    // матр. сист. норм. ур.
        public double[,] matrCInv; // обр. матр. C
        public double[,] matrR;    // матрица коэф. кор.
        public double[,] matrRPart;// матр. част. коэфф. кор.
        public double R;           // коэф. множ. кор.
        public double devRem;      // ост. дисп.
        public int n, p;           // кол. набл. и кол. факторов
        public double sumErr, avErr;      // сумма абс. откл. и ср. абс. откл.

        public Sample sY;
        public Sample[] arrSX;
        public Regression(Sample sY, Sample[] arrSX)
        {
            this.sY = sY;
            this.arrSX = arrSX;
            n = sY.arr.Length;
            p = arrSX.Length;
            double[][] matrX = new double[p + 1][];
            matrX[0] = new double[n];
            for (int j = 0; j < matrX[0].Length; j++)
                matrX[0][j] = 1;
            for (int i = 1; i < p + 1; i++)
                matrX[i] = (double[])arrSX[i - 1].arr.Clone();
            Matrix mX = Matrix.Create(matrX);
            mX.Transpose();
            Matrix mY = Matrix.Create(new double[][] { sY.arr });
            mY.Transpose();
            Matrix mXTr = mX.Clone();
            mXTr.Transpose();
            Matrix mB = mXTr * mX;
            matrC = mB.CopyToArray();
            mB = mB.Inverse();
            matrCInv = mB.CopyToArray();
            Matrix mXTrY = mXTr * mY;
            mB = mB * mXTrY;
            arrB = new double[mB.RowCount];
            for (int i = 0; i < mB.RowCount; i++)
                arrB[i] = mB[i, 0];

            // матр. коэф. кор.
            int smpCount = p + 1;
            List<Sample> lS = new List<Sample>(arrSX);
            lS.Insert(0, sY);
            matrR = new double[smpCount, smpCount];
            for (int i = 0; i < smpCount; i++)
                for (int j = 0; j <= i; j++)
                {
                    if (i == j)
                    {
                        matrR[i, j] = 1;
                        continue;
                    }
                    double sum = 0;
                    for (int k = 0; k < n; k++)
                        sum += lS[i][k] * lS[j][k];
                    matrR[i, j] = (sum / n - lS[i].av * lS[j].av) / (lS[i].sigma * lS[j].sigma);
                }
            for (int i = 0; i < smpCount; i++)
                for (int j = i + 1; j < smpCount; j++)
                    matrR[i, j] = matrR[j, i];

            // матр. част. коэф. кор.
            matrRPart = new double[smpCount, smpCount];
            Matrix mR = Matrix.Create(matrR);
            double minor11 = Minor(mR, 0, 0);
            for (int i = 0; i < mR.RowCount; i++)
                for (int j = 0; j < i; j++)
                    matrRPart[i, j] = Minor(mR, 1, j) /
                        Math.Sqrt(minor11 * Minor(mR, j, j));
            for (int i = 0; i < smpCount; i++)
                for (int j = i + 1; j < smpCount; j++)
                    matrRPart[i, j] = matrRPart[j, i];

            // эмп. знач. y
            arrYMod = new double[n];
            double[] arrX = new double[p];
            for (int i = 0; i < arrYMod.Length; i++)
            {
                for (int j = 0; j < arrX.Length; j++)
                    arrX[j] = arrSX[j][i];
                arrYMod[i] = RegValue(arrX);
            }

            // ост. дисп.
            devRem = 0;
            sumErr = 0;
            for (int i = 0; i < n; i++)
            {
                devRem += (sY.arr[i] - arrYMod[i]) * (sY.arr[i] - arrYMod[i]);
                sumErr += Math.Abs(sY.arr[i] - arrYMod[i]);
            }
            devRem /= n;// -p - 1;
            avErr = sumErr / n;

            // коэф. множ. кор.
            //R = Math.Sqrt(1 - devRem / sY.devStd);
            R = Math.Sqrt(1 - mR.Determinant() / minor11);
        }
        public double RegValue(double[] arrX)
        {
            double res = arrB[0];
            for (int i = 0; i < arrX.Length; i++)
                res += arrX[i] * arrB[i + 1];
            return res;
        }
        public string RegReport()
        {
            int d = 4;
            string s = string.Format("{0} = ", sY.idHtml);
            s += Math.Round(arrB[0], d).ToString();
            for (int i = 1; i < arrB.Length; i++)
            {
                if (Math.Round(arrB[i], d) >= 0)
                    s += "+";
                s += Math.Round(arrB[i], d).ToString() + arrSX[i - 1].idHtml;
            }
            s += string.Format("<br>s<sub>ост</sub><sup>2</sup> = {0:g4}<br>s<sub>ост</sub> = {1:g4}<br>|R| = {2:g4}",//<br>Сумма абс. ош.: {3:g4}<br>Ср. абс. ош.: {4:g4}<br>",
                devRem, Math.Sqrt(devRem), R, sumErr, avErr);
            return s;
        }
        public string CheckReg(double alpha)
        {
            string s = string.Format("Уровень значимости: α = {0}<br>", alpha);
            // знач. регр.
            double fishRegrTheor = StatTables.GetFishInv(n - 1, n - p - 1, alpha);
            double fishRegr = sY.devStd / devRem;
            s += "ПРОВЕРКА ЗНАЧИМОСТИ УРАВНЕНИЯ РЕГРЕССИИ<br>";
            s += string.Format("Наблюдаемое значение критерия Фишера: F<sub>набл</sub> = {0:g3}<br>" +
                "Теоретическое значение критерия Фишера: F<sub>теор</sub> = F(1-α,n-1,n-p-1) = F({1};{2};{3}) = {4:g3}<br>{5}<br>",
                fishRegr, 1 - alpha, n - 1, n - p - 1, fishRegrTheor,
                fishRegr > fishRegrTheor ? "F<sub>набл</sub> > F<sub>теор</sub> => уравнение регрессии значимо" :
                "F<sub>набл</sub> ≤ F<sub>теор</sub> => уравнение регрессии не значимо");

            // знач. коэф. ур. регр.            
            s += "ПРОВЕРКА ЗНАЧИМОСТИ КОЭФФИЦИЕНТОВ УРАВНЕНИЯ РЕГРЕССИИ<br>";
            double studBTheor = StatTables.GetStudInv(n - p - 1, alpha);
            double[] arrSigmaB = new double[p + 1], arrStudB = new double[p + 1], arrBMin = new double[p + 1], arrBMax = new double[p + 1];
            s += string.Format("Теоретическое значение критерия Стьюдента: t<sub>теор</sub> = {0:g3}<br>", studBTheor);
            for (int i = 0; i < arrSigmaB.Length; i++)
            {
                arrSigmaB[i] = Math.Sqrt(devRem * matrCInv[i, i]);
                arrStudB[i] = arrB[i] / arrSigmaB[i];
                arrBMin[i] = arrB[i] - studBTheor * arrSigmaB[i];
                arrBMax[i] = arrB[i] + studBTheor * arrSigmaB[i];
                s += string.Format("Коэффициент b<sub>{0}</sub> ({1}): наблюдаемое значение критерия Стьюдента t<sub>набл</sub> = " +
                    "{2:g3}; {3}; доверительный интервал: [{4:g3}, {5:g3}]<br>",
                    i, i > 0 ? arrSX[i - 1].name : "свободный член", arrStudB[i],
                    Math.Abs(arrStudB[i]) > studBTheor ? "|t<sub>набл</sub>| > t<sub>теор</sub> => коэффициент значим" :
                    "|t<sub>набл</sub>| ≤ t<sub>теор</sub> => коэффициент не значим", arrBMin[i], arrBMax[i]);
            }

            // знач. множ. коэф. кор.
            s += "ПРОВЕРКА ЗНАЧИМОСТИ МНОЖЕСТВЕННОГО КОЭФФИЦИЕНТА КОРРЕЛЯЦИИ<br>";
            double sigmaR = (1 - R * R) / Math.Sqrt(n - p - 1), fishR = R * R * (n - p - 1) / (1 - R * R) * p, studR = R / sigmaR;
            double studRTheor = StatTables.GetStudInv(n - p - 1, alpha);
            double fishRTheor = StatTables.GetFishInv(n - p - 1, p, alpha);
            s += string.Format("Множественный коэффициент корреляции: R = {0:g3}<br>Наблюдаемое значение критерия Стьюдента: " +
                "t<sub>набл</sub> = {1:g3}<br>Теоретическое значение критерия Стьюдента: " +
                "t<sub>теор</sub> = t(1-α;n-p-1) = t({2};{3}) = {4:g3}<br>{5}<br>" +
                "Наблюдаемое значение критерия Фишера: F<sub>набл</sub> = {6:g3}<br>" +
                "Теоретическое значение критерия Фишера: F<sub>теор</sub> = F(1-α;n-p-1;p) = F({7};{8};{9}) = {10:g3}<br>{11}<br>",
                R, studR, 1 - alpha, n - p - 1, studRTheor, Math.Abs(studR) > studRTheor ? "|t<sub>набл</sub>| > t<sub>теор</sub> => коэффициент значим" :
            "|t<sub>набл</sub>| ≤ t<sub>теор</sub> => коэффициент не значим", fishR, 1 - alpha, n - p - 1, p, fishRTheor,
             fishR > fishRTheor ? "F<sub>набл</sub> > F<sub>теор</sub> => коэффициент значим" :
            "F<sub>набл</sub> ≤ F<sub>теор</sub> => коэффициент не значим");
            return s;
        }
        public string CorrReport()
        {
            int d = 3;
            List<string> l = new List<string>();
            l.Add(sY.name);
            foreach (Sample s in arrSX)
                l.Add(s.name);
            return string.Format("Корреляционная матрица<br>{0}",//Матрица частных коэффициентов корреляции{1}",
                MatrixToHtml(matrR, d, l.ToArray()),
                 MatrixToHtml(matrRPart, d, l.ToArray()));
        }
        public string CheckCorr(double alpha)
        {
            double tTheor = StatTables.GetStudInv(n - 2, alpha);
            string s = string.Format("Уровень значимости: α = {0}<br>", alpha);
            s += string.Format("ПРОВЕРКА ЗНАЧИМОСТИ КОЭФФИЦИЕНТОВ КОРРЕЛЯЦИИ<br>Теоретическое значение критерия Стьюдента: " +
                "t<sub>теор</sub> = t(1-α,n-2) = t({0};{1}) = {2:g3}<table border = 1 cellspacing = 0>", 1 - alpha, n - 2, tTheor);
            string[,] matr = new string[p + 1, p + 1];
            for (int i = 0; i < matrR.GetLength(0); i++)
            {
                for (int j = 0; j < matrR.GetLength(1); j++)
                {
                    double sigma = (1 - matrR[i, j]) / Math.Sqrt(n - 1);
                    double t = matrR[i, j] / sigma;
                    if (Math.Abs(t) > tTheor)
                        matr[i, j] = string.Format("|t<sub>набл</sub>| = {0:g3} > t<sub>теор</sub> => значим", Math.Abs(t));
                    else
                        matr[i, j] = string.Format("|t<sub>набл</sub>| = {0:g3} ≤ t<sub>теор</sub> => не значим", Math.Abs(t));
                }
            }
            List<string> l = new List<string>();
            l.Add(sY.name);
            foreach (Sample smp in arrSX)
                l.Add(smp.name);
            s += MatrixToHtml(matr, l.ToArray());
            return s;
        }
        public double Ryx1()
        {
            return matrR[0, 1];
        }
        double Minor(Matrix m, int rowIndex, int colIndex)
        {
            Matrix mTmp = new Matrix(m.RowCount - 1, m.ColumnCount - 1);
            for (int i = 0; i < mTmp.RowCount; i++)
                for (int j = 0; j < mTmp.ColumnCount; j++)
                {
                    int i1 = i, j1 = j;
                    if (i >= rowIndex)
                        i1++;
                    if (j >= colIndex)
                        j1++;
                    mTmp[i, j] = m[i1, j1];
                }
            return mTmp.Determinant();
        }
        string MatrixToHtml(double[,] matr, int d)
        {
            string s = "<TABLE border = 1 cellspacing = 0 align = center>";
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                s += "<TR>";
                for (int j = 0; j < matr.GetLength(1); j++)
                    s += "<TD>" + Math.Round(matr[i, j], d).ToString() + "</TD>";
                s += "</TR>";
            }
            s += "</TABLE>";
            return s;
        }
        string MatrixToHtml(double[,] matr, int d, string[] arrRCHeader)
        {
            string s = "<TABLE border = 1 cellspacing = 0 align = center><TR><TD></TD>";
            for (int i = 0; i < arrRCHeader.Length; i++)
                s += "<TD>" + arrRCHeader[i] + "</TD>";
            s += "</TR>";
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                s += "<TR><TD>" + arrRCHeader[i] + "</TD>";
                for (int j = 0; j < matr.GetLength(1); j++)
                    s += "<TD>" + Math.Round(matr[i, j], d).ToString() + "</TD>";
                s += "</TR>";
            }
            s += "</TABLE>";
            return s;
        }
        string MatrixToHtml(double[,] matr, int d, string[] arrRHeader, string[] arrCHeader)
        {
            string s = "<TABLE border = 1 cellspacing = 0 align = center><TR><TD></TD>";
            for (int i = 0; i < arrCHeader.Length; i++)
                s += "<TD>" + arrCHeader[i] + "</TD>";
            s += "</TR>";
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                s += "<TR><TD>" + arrRHeader[i] + "</TD>";
                for (int j = 0; j < matr.GetLength(1); j++)
                    s += "<TD>" + Math.Round(matr[i, j], d).ToString() + "</TD>";
                s += "</TR>";
            }
            s += "</TABLE>";
            return s;
        }
        string MatrixToHtml(string[,] matr, string[] arrRCHeader)
        {
            string s = "<TABLE border = 1 cellspacing = 0 align = center><TR><TD></TD>";
            for (int i = 0; i < arrRCHeader.Length; i++)
                s += "<TD>" + arrRCHeader[i] + "</TD>";
            s += "</TR>";
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                s += "<TR><TD>" + arrRCHeader[i] + "</TD>";
                for (int j = 0; j < matr.GetLength(1); j++)
                    s += "<TD>" + matr[i, j] + "</TD>";
                s += "</TR>";
            }
            s += "</TABLE>";
            return s;
        }

        public static SortedDictionary<double, Sample> TranSamples(Sample sx, Sample sy)
        {
            string[] arrT = { "x", "x^2", "x^3", "1/x", "1/x^2", "1/x^3", "sqrt(x)", "1/sqrt(x)", "ln(x)", "e(x)" };
            SortedDictionary<double, Sample> dic = new SortedDictionary<double, Sample>();
            for (int i = 0; i < arrT.Length; i++)
            {
                Sample ts;
                Regression reg;
                try
                {
                    ts = Sample.Transform(sx, arrT[i]);
                    reg = new Regression(sy, new Sample[] { ts });
                }
                catch
                {
                    continue;
                }
                dic.Add(Math.Abs(reg.Ryx1()), ts);
            }
            return dic;
        }
        public static SortedDictionary<double, Sample> TranSamples(Sample[] arrSX, Sample sY,
            Form form, ProgressDelegate method)
        {
            string[] arrT = { "x" };//, "x^2", "x^3", "1/x", "1/x^2", "1/x^3", "sqrt(x)", "1/sqrt(x)", "ln(x)", "e(x)" };
            SortedDictionary<double, Sample> dic = new SortedDictionary<double, Sample>();
            int percent = 0;
            foreach (Sample sx1 in arrSX)
            {
                foreach (Sample sx2 in arrSX)
                {
                    foreach (string t1 in arrT)
                    {
                        foreach (string t2 in arrT)
                        {
                            Sample tsx1, tsx2, s;
                            Regression r;
                            try
                            {
                                tsx1 = Sample.Transform(sx1, t1);
                                tsx2 = Sample.Transform(sx2, t1);
                                s = Sample.Multiply(tsx1, tsx2);
                                r = new Regression(sY, new Sample[] { s });
                            }
                            catch
                            {
                                continue;
                            }
                            try
                            {
                                dic.Add(Math.Abs(r.Ryx1()), s);
                            }
                            catch { }
                        }
                    }
                }
                percent += (int)(1.0 / arrSX.Length * 100);
                form.Invoke(method, percent);
            }
            return dic;
        }
        public delegate void ProgressDelegate(int percent);
    }
    public class StatTables
    {
        static double epsilon = 0.0000001;
        static StudentsTDistribution distStud = new StudentsTDistribution();
        static FisherSnedecorDistribution distFish = new FisherSnedecorDistribution();
        static ChiSquareDistribution distChi2 = new ChiSquareDistribution();
        static public double GetStudInv(int n, double alpha)
        {
            distStud.DegreesOfFreedom = n;
            return GetX(1 - alpha, distStud);
        }
        static public double GetChi2Inv(int n, double alpha)
        {
            distChi2.DegreesOfFreedom = n;
            return GetX(1 - alpha, distChi2);
        }
        static public double GetFishInv(int n1, int n2, double alpha)
        {
            distFish.Alpha = n1;
            distFish.Beta = n2;
            return GetX(1 - alpha, distFish);
        }
        public static double GetTau(double alpha, double v)
        {
            int i, j;
            for (i = arrAlpha.Length - 1; i >= 0; i--)
                if (arrAlpha[i] >= alpha)
                    break;
            for (j = arrV.Length - 1; j >= 0; j--)
                if (arrV[j] <= v)
                    break;
            return arrTau[j, i];
        }
        static double GetX(double p, ContinuousDistribution dist)
        {
            double x, xNext = 1;
            do
            {
                x = xNext;
                xNext = x - (dist.CumulativeDistribution(x) - p) /
                    dist.ProbabilityDensity(x);
            }
            while (Math.Abs(x - xNext) > epsilon);
            return xNext;
        }
        static double[] arrAlpha = new double[]
                {
                    0.10f, 0.05f, 0.025f, 0.01f
                };
        static int[] arrV = new int[]
                {
                    3, 4, 5, 6, 7, 8, 9, 10,
                    11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                    21, 22, 23, 24, 25
                };
        static double[,] arrTau = new double[,]
                {
                    { 1.41f, 1.41f, 1.41f, 1.41f },
                    { 1.65f, 1.69f, 1.71f, 1.72f },
                    { 1.79f, 1.87f, 1.92f, 1.96f },
                    { 1.89f, 2.00f, 2.07f, 2.13f },
                    { 1.97f, 2.09f, 2.18f, 2.27f },
                    { 2.04f, 2.17f, 2.27f, 2.37f },
                    { 2.10f, 2.24f, 2.35f, 2.46f },
                    { 2.15f, 2.29f, 2.41f, 2.54f },                    
                    { 2.19f, 2.34f, 2.47f, 1.41f },
                    { 2.23f, 2.39f, 2.52f, 1.72f },
                    { 2.26f, 2.43f, 2.56f, 1.96f },
                    { 2.30f, 2.46f, 2.60f, 2.13f },                    
                    { 2.33f, 2.49f, 2.64f, 2.80f },
                    { 2.35f, 2.52f, 2.67f, 2.84f },
                    { 2.38f, 2.55f, 2.70f, 2.87f },
                    { 2.40f, 2.58f, 2.73f, 2.90f },
                    { 2.43f, 2.60f, 2.75f, 2.93f },
                    { 2.45f, 2.62f, 2.78f, 2.96f },
                    { 2.47f, 2.64f, 2.80f, 2.98f },
                    { 2.49f, 2.66f, 2.82f, 3.01f },
                    { 2.50f, 2.68f, 2.84f, 3.03f },
                    { 2.52f, 2.70f, 2.86f, 3.05f },
                    { 2.54f, 2.72f, 2.88f, 3.07f }
                };
    }
}