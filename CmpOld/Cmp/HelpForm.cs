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
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
            richTextBox1.LoadFile("Help.rtf");
        }
    }
}
