using System;
using System.Windows.Forms;

namespace HiveSymulator
{
    public partial class HiveForm : Form
    {
        public Renderer renderer { get; set; }

        public HiveForm()
        {
            InitializeComponent();
        }

        private void HiveForm_MouseClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine("{0}, {1}", e.Location.X, e.Location.Y);
        }

        private void HiveForm_Paint(object sender, PaintEventArgs e)
        {
            renderer.PaintHive(e.Graphics);
        }
    }
}
