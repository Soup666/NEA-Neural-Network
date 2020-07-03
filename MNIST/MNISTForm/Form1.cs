using MNIST.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MNISTForm
{
    public partial class Form1 : Form
    {

        NeuralNetwork nn = new NeuralNetwork(784, 16, 10);
        public bool isTraining = true;

        bool start = false;
        bool disp = true;
        bool finishedTraining = false;
        int time = 0;
        int guessing = 0;
        List<Image> testing;
        List<Image> training;

        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    ps[i, j] = 0;
                }
            }

            button1.Enabled = false;

            timer1.Start();

            testing = MNISTReader.ReadTestData().ToList();
            training = MNISTReader.ReadTrainingData().ToList();

        }
        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (!start) return;

            int trainingAmount = 10000;

            float scalar = 5.0f;
            panel1.Width = 28 * int.Parse(scalar.ToString());
            panel1.Height = 28 * int.Parse(scalar.ToString());
            if (isTraining)
            {
                Console.WriteLine("Started Training!");
                int count = 0;

                double[] sendingData = new double[784];
                double[] sendingResult = new double[10];

                for (int i = 0; i < sendingResult.Length; i++)
                {
                    sendingResult[i] = 0;
                }

                for (int index = 0; index < trainingAmount; index++)
                {
                    for (int i = 0; i < sendingResult.Length; i++)
                    {
                        sendingResult[i] = 0;
                    }

                    var item = training[index];

                    //Console.WriteLine($"Answer: {item.Label}");

                    label1.Text = $"Answer: {item.Label}";
                    label1.Refresh();
                    sendingResult[item.Label] = 1;

                    int ind = 0;
                    for (int x = 0; x < item.Data.GetLength(0); x++)
                    {
                        for (int y = 0; y < item.Data.GetLength(1); y++)
                        {
                            byte c = item.Data[y, x];

                            sendingData[ind++] = c / 255;

                            if (disp)
                            {
                                SolidBrush s = new SolidBrush(Color.FromArgb(255, c, c, c));
                                e.Graphics.FillRectangle(s, x * scalar, y * scalar, 1 * scalar, 1 * scalar);
                            }
                            //Console.Write($"{c}, ");
                        }
                        //Console.WriteLine("\n");
                    }


                    for (int i = 0; i < sendingResult.Length; i++)
                    {
                        //Console.Write($"{sendingResult[i]}, ");
                    }

                    //NN
                    nn.Train(Matrix.FromArray(sendingData), Matrix.FromArray(sendingResult));
                    /*
                    Console.WriteLine((double)index / (double)trainingAmount * 100);
                    progressBar1.Value = (int)( (double)index / (double)trainingAmount) * 100;
                    progressBar1.Refresh();
                    */
                    //Thread.Sleep(1000);
                    if (index % 500 == 0)Console.WriteLine($"Done {index}");

                    if (++count == trainingAmount) break;
                }
                finishedTraining = true;
                Console.WriteLine("Done Training!");
                isTraining = false;
                button1.Enabled = true;
                label3.Text = "<--- Finished!";
            }
            else
            {
                if (!(time > 1)) return;
                Console.WriteLine(guessing++);

                var item = training[new Random().Next(0, 10000)];
                label1.Text = $"Trying for {item.Label}";
                label1.Refresh();

                double[] sendingData = new double[784];

                int ind = 0;
                for (int x = 0; x < item.Data.GetLength(0); x++)
                {
                    for (int y = 0; y < item.Data.GetLength(1); y++)
                    {
                        byte c = item.Data[y, x];

                        sendingData[ind++] = c;

                        SolidBrush s = new SolidBrush(Color.FromArgb(255, c, c, c));
                        e.Graphics.FillRectangle(s, x * scalar, y * scalar, 1 * scalar, 1 * scalar);
                        //Console.Write($"{c}, ");
                    }
                    //Console.WriteLine("\n");
                }

                //Now guessing
                double[] guesses = nn.Feedforward(sendingData);
                double hiscore = 0;
                int winner = 0;

                Console.WriteLine("======");
                for (int i = 0; i < guesses.Length; i++)
                {
                    Console.Write($"{guesses[i]}, ");
                    double guess = guesses[i];
                    if (guess > hiscore)
                    {
                        hiscore = guess;
                        winner = i;
                    }
                }
                Console.WriteLine("======");
                label2.Text = $"Guess is {winner}";

                time = 0;
                button1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Paint += new PaintEventHandler(panel1_Paint);
            panel1.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (time++ < 1) button1.Enabled = false;
            else button1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            start = true;
            panel1.Refresh();
        }

        Point clickLoc = new Point(0,0);
        double[,] ps = new double[28,28];

        bool down = false;

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            down = true;
            clickLoc = e.Location;
            panel2.Paint += new PaintEventHandler(panel2_Paint);
            panel2.Refresh();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (!finishedTraining) return;

            var p = sender as Panel;
            var g = e.Graphics;

            clickLoc.X = (int)(Math.Round((double)clickLoc.X / 100, 1) * 100);
            clickLoc.Y = (int)(Math.Round((double)clickLoc.Y / 100, 1) * 100);

            Rectangle pixel = new Rectangle(clickLoc, new Size(30, 30));
            g.FillRectangle(Brushes.Red, pixel);

            Console.WriteLine(clickLoc);

            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    if (ps[i,j] == 1)
                    {
                        Rectangle p2 = new Rectangle(i * 10, j * 10, 30,30);
                        g.FillRectangle(Brushes.Black, p2);
                    }
                }
            }

            if (down) ps[clickLoc.X / 10, clickLoc.Y / 10] = 1;

        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            clickLoc = e.Location;
            panel2.Paint += new PaintEventHandler(panel2_Paint);
            panel2.Refresh();
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            down = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            disp = !checkBox1.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Guess

            label1.Text = $"Calculating...";
            label1.Refresh();

            double[] flatterned = new double[784];


            int ind = 0;
            for (int x = 0; x < 28; x++)
            {
                for (int y = 0; y < 28; y++)
                {
                    double c = ps[x, y];

                    flatterned[ind++] = c;
                }

            }

            double[] guesses = nn.Feedforward(flatterned);
            double hiscore = 0;
            int winner = 0;

            //Console.WriteLine("======");
            for (int i = 0; i < guesses.Length; i++)
            {
                //Console.Write($"{guesses[i]}, ");
                double guess = guesses[i];
                if (guess > hiscore)
                {
                    hiscore = guess;
                    winner = i;
                }
            }
            Console.WriteLine("======");
            label2.Text = $"Guess is {winner}";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    ps[i, j] = 0;
                }
            }
        }
    }
}
