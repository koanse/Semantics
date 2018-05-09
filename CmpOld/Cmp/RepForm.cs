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
    public partial class RepForm : Form
    {
        public RepForm(string s)
        {
            InitializeComponent();
            webBrowser1.DocumentText = s;
        }
    }
}
