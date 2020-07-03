using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MNISTForm
{
    public partial class Form1 : Form
    {

        NeuralNetwork nn = new NeuralNetwork(784, 16, 10);
        ImageDrawer id = new ImageDrawer();

        bool isTraining = true;
        bool start = false;
        bool disp = true;

        int time = 0;
        int guessing = 0;

        List<Image> testing;
        List<Image> training;
        
        Point clickLoc = new Point(0, 0);
        double[,] ps = new double[28, 28];

        int scalar = 4;

        bool thumbnail = false;

        public Form1()
        {
            InitializeComponent();

            panel1.Width = 28 * scalar;
            panel1.Height = 28 * scalar;

            button1.Enabled = false;
            timer1.Start();

            testing = MNISTReader.ReadTestData().ToList();
            training = MNISTReader.ReadTrainingData().ToList();

            ps = id.BlankFrame(28, 28);
        }

        private void TrainData(int TrainingAmount, PaintEventArgs e)
        {
            Console.WriteLine("Started Training!");

            int count = 0;
            double[] sendingData = id.BlankFrame(784);
            double[] sendingResult = id.BlankFrame(10);

            for (int index = 0; index < TrainingAmount; index++)
            {
                sendingResult = id.BlankFrame(10);

                var item = training[index];
                double[,] tData = Matrix.Transpose(Matrix.FromArray(item.Data)).data;

                if (disp) label1.Text = $"Answer: {item.Label}";
                label1.Refresh();

                if (disp) id.DrawFrame(tData, e, scalar);

                sendingData = id.Flattern(tData, 255);
                sendingResult[item.Label] = 1;

                nn.Train(Matrix.FromArray(sendingData), Matrix.FromArray(sendingResult));

                //Thread.Sleep(1000);
                if (index % 500 == 0) Console.WriteLine($"Done {index}");
                if (++count == TrainingAmount) break;
            }
            Console.WriteLine("Done Training!");
            isTraining = false;
            button1.Enabled = true;
            label3.Text = "<--- Finished!";
        }

        private void TestData(PaintEventArgs e)
        {
            if (!(time > 1)) return;
            Console.WriteLine(guessing++);

            var item = training[new Random().Next(0, 10000)];
            double[,] tData = Matrix.Transpose(Matrix.FromArray(item.Data)).data;

            double[] sendingData = id.BlankFrame(784);
            double[] sendingResult = id.BlankFrame(10);

            sendingResult = id.BlankFrame(10);

            label1.Text = $"Correct Answer: {item.Label}";
            label1.Refresh();

            id.DrawFrame(tData, e, scalar);

            sendingData = id.Flattern(tData, 255);

            double[] guesses = nn.Feedforward(sendingData);
            double guess = id.CalculateHighest(guesses);

            label2.Text = $"Guess is {guess}";

            time = 0;
            button1.Enabled = false;
        }

        private void TestData(double[,] data, PaintEventArgs e)
        {
            if (!(time > 1)) return;
            Console.WriteLine(guessing++);

            var item = data;
            double[,] tData = Matrix.Transpose(Matrix.FromArray(item)).data;

            double[] sendingData = id.BlankFrame(784);
            double[] sendingResult = id.BlankFrame(10);

            sendingResult = id.BlankFrame(10);

            label1.Text = $"Guessing";
            label1.Refresh();

            id.DrawFrame(tData, e, scalar);

            sendingData = id.Flattern(tData, 1);

            double[] guesses = nn.Feedforward(sendingData);
            double guess = id.CalculateHighest(guesses);

            label2.Text = $"Guess is {guess}";

            time = 0;
            button1.Enabled = false;
        }

        private void TestData(double[,] data)
        {
            if (!(time > 1)) return;
            Console.WriteLine(guessing++);

            var item = data;
            double[,] tData = Matrix.Transpose(Matrix.FromArray(item)).data;

            double[] sendingData = id.BlankFrame(784);
            double[] sendingResult = id.BlankFrame(10);

            sendingResult = id.BlankFrame(10);

            label1.Text = $"Guessing";
            label1.Refresh();

            sendingData = id.Flattern(tData, 255);

            double[] guesses = nn.Feedforward(sendingData);
            double guess = id.CalculateHighest(guesses);

            label2.Text = $"Guess is {guess}";

            time = 0;
            button1.Enabled = false;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (!start) return;

            if (thumbnail)
            {
                Console.WriteLine("Drawn!");
                id.DrawFrame(ps, e, scalar);
                return;
            }

            int trainingAmount = 60000;
            
            if (isTraining)
            {
                TrainData(trainingAmount, e);
            }
            else
            {
                TestData(e);
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

        bool down = false;

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            down = true;
            clickLoc = e.Location;
            panel2.Refresh();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (isTraining) return;

            int brushSize = 10;
            
            clickLoc.X = (int)(Math.Round((double)clickLoc.X / 100, 1) * 100);
            clickLoc.Y = (int)(Math.Round((double)clickLoc.Y / 100, 1) * 100);

            if (clickLoc.X / 10 < 0 || clickLoc.X / 10 > 28) return;
            if (clickLoc.Y / 10 < 0 || clickLoc.Y / 10 > 28) return;

            Rectangle pixel = new Rectangle(clickLoc, new Size(brushSize, brushSize));

            e.Graphics.FillRectangle(Brushes.Red, pixel);

            if (down)
            {
                if (ps[(clickLoc.X / 10), (clickLoc.Y / 10)+1] != 255) ps[(clickLoc.X / 10), (clickLoc.Y / 10)+1] = 125;
                if (ps[(clickLoc.X / 10), (clickLoc.Y / 10)-1] != 255) ps[(clickLoc.X / 10), (clickLoc.Y / 10)-1] = 125;
                if (ps[(clickLoc.X / 10)+1, (clickLoc.Y / 10)] != 255) ps[(clickLoc.X / 10)+1, (clickLoc.Y / 10)] = 125;
                if (ps[(clickLoc.X / 10)-1, (clickLoc.Y / 10)] != 255) ps[(clickLoc.X / 10)-1, (clickLoc.Y / 10)] = 125;

                ps[clickLoc.X / 10, clickLoc.Y / 10] = 255;
            }
            

            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    if (ps[i, j] == 255)
                    {
                        int x = i * 10;
                        int y = j * 10;

                        Rectangle p2 = new Rectangle(x, y, brushSize, brushSize);
                        e.Graphics.FillRectangle(Brushes.Black, p2);
                    }
                    if (ps[i, j] == 125)
                    {
                        int x = i * 10;
                        int y = j * 10;

                        Rectangle p2 = new Rectangle(x, y, brushSize, brushSize);
                        e.Graphics.FillRectangle(Brushes.Aqua, p2);
                    }
                }
            }

            foreach (var item in id.Flattern(ps, 1))
            {
                Console.Write(item + ",");
            }
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            clickLoc = e.Location;
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

            thumbnail = true;

            label1.Text = $"Calculating...";
            label1.Refresh();
            panel1.Refresh();

            TestData(ps);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ps = id.BlankFrame(28, 28);
            panel1.Refresh();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            thumbnail = true;
            panel1.Refresh();
        }
    }
}
