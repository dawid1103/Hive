using System;
using System.Windows.Forms;

namespace HiveSymulator
{
    public partial class FieldForm : Form
    {
        public Renderer renderer { get; set; }
        public FieldForm()
        {
            InitializeComponent();
        }

        private void FieldForm_MouseClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine("{0}, {1}", e.Location.X, e.Location.Y);
        }

        private void FieldForm_Paint(object sender, PaintEventArgs e)
        {
            renderer.PaintField(e.Graphics);
        }
    }
}
