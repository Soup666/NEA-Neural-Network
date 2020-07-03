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

        NeuralNetwork nn = new NeuralNetwork(784, 16, 27);
        public bool isTraining = true;

        char[] alphabet = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
        bool start = false;
        bool disp = true;
        int time = 0;
        int guessing = 0;
        List<Image> testing;
        List<Image> training;

        public Form1()
        {
            InitializeComponent();

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

            int trainingAmount = 60000;

            float scalar = 5.0f;
            panel1.Width = 28 * int.Parse(scalar.ToString());
            panel1.Height = 28 * int.Parse(scalar.ToString());
            if (isTraining)
            {

                for (int l = 0; l < 1; l++)
                {
                    Console.WriteLine("Started Training!");
                    int count = 0;

                    double[] sendingData = new double[784];
                    double[] sendingResult = new double[27];

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

                        label1.Text = $"Answer: {alphabet[item.Label - 1]}";
                        label1.Refresh();
                        sendingResult[item.Label] = 1;
                        //Console.WriteLine(alphabet[item.Label-1]);

                        int ind = 0;
                        for (int x = 0; x < item.Data.GetLength(0); x++)
                        {
                            for (int y = 0; y < item.Data.GetLength(1); y++)
                            {
                                byte c = item.Data[x, y];

                                sendingData[ind++] = c / 255;

                                if (disp)
                                {
                                    SolidBrush s = new SolidBrush(Color.FromArgb(255, c, c, c));
                                    e.Graphics.FillRectangle(s, x * scalar, y * scalar, 1 * scalar, 1 * scalar);
                                }
                            }
                        }

                        nn.Train(Matrix.FromArray(sendingData), Matrix.FromArray(sendingResult));

                        //Thread.Sleep(1000);
                        if (index % 1000 == 0) Console.WriteLine($"Done {index}");

                        if (++count == trainingAmount) break;
                    }
                }
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
                label1.Text = $"Trying for {alphabet[Int32.Parse(item.Label.ToString())-1]}";
                label1.Refresh();

                double[] sendingData = new double[784];

                int ind = 0;
                for (int x = 0; x < item.Data.GetLength(0); x++)
                {
                    for (int y = 0; y < item.Data.GetLength(1); y++)
                    {
                        byte c = item.Data[x,y];

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
                label2.Text = $"Guess is {alphabet[winner-1]}";

                time = 0;
                button1.Enabled = false ;
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            disp = !checkBox1.Checked;
        }
    }
}
