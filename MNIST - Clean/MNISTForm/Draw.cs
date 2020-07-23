using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MNISTForm
{
    public partial class Draw : Form
    {
        bool down = false;
        Point clickLoc = new Point(0, 0);
        public Draw()
        {
            InitializeComponent();
            
        }
        private void button3_Click(object sender, EventArgs e)
        {
            //Guess
            Program.f.thumbnail = true;
            Program.f.SetLabel1($"Calculating...");
            Program.f.TestData(Program.f.ps);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Program.f.ps = Program.f.id.BlankFrame(28, 28);
            Program.f.RefreshPanel();
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            down = true;
            clickLoc = e.Location;
            panel2.Refresh();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            int brushSize = 10;

            clickLoc.X = (int)(Math.Round((double)clickLoc.X / 100, 1) * 100);
            clickLoc.Y = (int)(Math.Round((double)clickLoc.Y / 100, 1) * 100);

            if (clickLoc.X / 10 < 0 || clickLoc.X / 10 > 28) return;
            if (clickLoc.Y / 10 < 0 || clickLoc.Y / 10 > 28) return;

            Rectangle pixel = new Rectangle(clickLoc, new Size(brushSize, brushSize));

            e.Graphics.FillRectangle(Brushes.Red, pixel);

            if (down)
            {
                if (Program.f.ps[(clickLoc.X / 10), (clickLoc.Y / 10) + 1] != 255) Program.f.ps[(clickLoc.X / 10), (clickLoc.Y / 10) + 1] = 125;
                if (Program.f.ps[(clickLoc.X / 10), (clickLoc.Y / 10) - 1] != 255) Program.f.ps[(clickLoc.X / 10), (clickLoc.Y / 10) - 1] = 125;
                if (Program.f.ps[(clickLoc.X / 10) + 1, (clickLoc.Y / 10)] != 255) Program.f.ps[(clickLoc.X / 10) + 1, (clickLoc.Y / 10)] = 125;
                if (Program.f.ps[(clickLoc.X / 10) - 1, (clickLoc.Y / 10)] != 255) Program.f.ps[(clickLoc.X / 10) - 1, (clickLoc.Y / 10)] = 125;

                Program.f.ps[clickLoc.X / 10, clickLoc.Y / 10] = 255;
            }

            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    if (Program.f.ps[i, j] == 255)
                    {
                        int x = i * 10;
                        int y = j * 10;

                        Rectangle p2 = new Rectangle(x, y, brushSize, brushSize);
                        e.Graphics.FillRectangle(Brushes.Black, p2);
                    }
                    if (Program.f.ps[i, j] == 125)
                    {
                        int x = i * 10;
                        int y = j * 10;

                        Rectangle p2 = new Rectangle(x, y, brushSize, brushSize);
                        e.Graphics.FillRectangle(Brushes.Aqua, p2);
                    }
                }
            }

            foreach (var item in Program.f.id.Flattern(Program.f.ps, 1))
            {
                Console.Write(item + ",");
            }
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            down = false;
        }

        private void panel2_MouseMove_1(object sender, MouseEventArgs e)
        {
            clickLoc = e.Location;
            panel2.Refresh();
        }
    }
}
