using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace TInput
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InputForm());
            //C_EAnswer c = new C_EAnswer("Иванов", "АС", "Вопрос", "Эталон", "Ответ", new Tutor[] { new Tutor("T1", 100), new Tutor("T2", 95) });
            //c.Save("a1.xml");
            //c = new C_EAnswer("a1.xml");
        }
    }
    class C_EAnswer
    {
        private string FIO;
        public string p_FIO
        {
            get { return FIO; }
        }

        private string group;
        public string p_group
        {
            get { return group; }
        }

        private string question;
        public string p_question
        {
            get { return question; }
        }

        private string etalon_answer;
        public string p_etalon_answer
        {
            get { return etalon_answer; }
        }

        private string student_answer;
        public string p_student_answer
        {
            get { return student_answer; }
        }

        private List<Tutor> p_tutors;
        public Tutor GetTutor(int i)
        {
            return p_tutors[i];
        }
        public int GetTutorCount()
        {
            return p_tutors.Count;
        }
        public void AddTutor(Tutor t)
        {
            p_tutors.Add(t);
        }

        public C_EAnswer(string in_FIO, string in_group, string in_question, string in_etalon_answer, string in_student_answer,
            Tutor[] in_tutors)
        {
            FIO = in_FIO;
            group = in_group;
            question = in_question;
            etalon_answer = in_etalon_answer;
            student_answer = in_student_answer;
            p_tutors = new List<Tutor>(in_tutors);
        }

        public C_EAnswer(string XML_file_path)
        {
            XmlDocument answer_doc = new XmlDocument();
            answer_doc.Load(XML_file_path);
            XmlElement main_element = answer_doc.DocumentElement;
            FIO = main_element["FIO"].InnerText;
            group = main_element["group"].InnerText;
            question = main_element["question"].InnerText;
            etalon_answer = main_element["etalon_answer"].InnerText;
            student_answer = main_element["student_answer"].InnerText;
            p_tutors = new List<Tutor>();
            foreach (XmlElement e in main_element["Tutors"].ChildNodes)
            {
                if (e.Name != "Tutor")
                    throw new Exception();
                p_tutors.Add(new Tutor(e.Attributes["FIO"].Value, double.Parse(e.InnerText)));
            }
        }

        public void Save(string file_path)
        {
            XmlDocument answer_doc = new XmlDocument();
            XmlElement e = answer_doc.CreateElement("Answer_Information");
            answer_doc.AppendChild(e);
            answer_doc.PreserveWhitespace = true;
            e.AppendChild(answer_doc.CreateElement("FIO")).InnerText = p_FIO;
            e.AppendChild(answer_doc.CreateElement("group")).InnerText = p_group;
            e.AppendChild(answer_doc.CreateElement("question")).InnerText = p_question;
            e.AppendChild(answer_doc.CreateElement("etalon_answer")).InnerText = p_etalon_answer;
            e.AppendChild(answer_doc.CreateElement("student_answer")).InnerText = p_student_answer;
            e = (XmlElement)e.AppendChild(answer_doc.CreateElement("Tutors"));
            foreach (Tutor t in p_tutors)
            {
                XmlElement el = answer_doc.CreateElement("Tutor");
                el.Attributes.Append(answer_doc.CreateAttribute("FIO")).Value = t.FIO;
                e.AppendChild(el).InnerText = t.Mark.ToString();
            }
            answer_doc.Save(file_path);
        }
    }
    struct Tutor
    {
        public string FIO;
        public double Mark;
        public Tutor(string FIO,double Mark)
        {
            this.FIO = FIO;
            this.Mark = Mark;
        }
    }
}
