using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Displays
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap bm = new Bitmap(@"C:\Alan\University\111.jpg");

            // Draw using this   
            e.Graphics.DrawImage(bm, 60, 60);

            base.OnPaint(e);
        }
    }
}
