using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace MNISTForm
{
    public partial class Form1 : Form
    {

        public NeuralNetwork nn;
        public ImageDrawer id = new ImageDrawer();

        public char[] alphabet = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        public bool isTraining = false;
        public bool start = false;
        public bool disp = true;

        public int guessing = 0;

        public List<Image> testing;
        public List<Image> training;

        public double[,] ps = new double[28, 28];

        public int scalar = 4;
        public int trainingAmount = 10000;

        public bool thumbnail = false;
        public bool emnist = true;
        public bool gqd = false;

        public void test()
        {
            Console.WriteLine("Success");
        }

        public Form1()
        {
            InitializeComponent();

            panel1.Width = 28 * scalar;
            panel1.Height = 28 * scalar;

            if (emnist) nn = new NeuralNetwork(784, 124, 26);
            else nn = new NeuralNetwork(784, 16, 10);

            MNISTReader.SetType("emnist/emnist-letters-train-images-idx3-ubyte", "emnist/emnist-letters-train-labels-idx1-ubyte", "emnist/emnist-letters-test-images-idx3-ubyte", "emnist/emnist-letters-test-labels-idx1-ubyte");

            testing = MNISTReader.ReadTestData().ToList();
            training = MNISTReader.ReadTrainingData().ToList();

            ps = id.BlankFrame(28, 28);
        }

        public void TrainData(int TrainingAmount, PaintEventArgs e)
        {
            Console.WriteLine("Started Training!");

            int count = 0;
            double[] sendingData = id.BlankFrame(784);
            double[] sendingResult;

            if (emnist) sendingResult = id.BlankFrame(26);
            else sendingResult = id.BlankFrame(10);

            for (int index = 0; index < TrainingAmount; index++)
            {

                var item = training[index];
                double[,] tData = Matrix.Transpose(Matrix.FromArray(item.Data)).data;

                if (disp && emnist) label1.Text = $"Previewing: a {alphabet[item.Label - 1]}";
                else if (disp) label1.Text = $"Previewing: a {item.Label}";
                label1.Refresh();

                if (disp) id.DrawFrame(tData, e, scalar, emnist);

                sendingData = id.Flattern(tData, 255);

                Console.WriteLine(item.Label - 1);
                Console.WriteLine(sendingResult.Length);
                if (emnist) sendingResult[item.Label-1] = 1;
                else sendingResult[item.Label] = 1;

                nn.Train(Matrix.FromArray(sendingData), Matrix.FromArray(sendingResult));

                //Thread.Sleep(1000);
                if (index % 100 == 0)
                {
                    label3.Text = $"{index} / {TrainingAmount}";
                    label3.Refresh();
                    progressBar1.Value = Int32.Parse(((double)index / (double)TrainingAmount * 100.0).ToString());
                    progressBar1.Update();
                }
                if (++count == TrainingAmount) break;
            }
            Console.WriteLine("Done Training!");
            isTraining = false;
            button1.Enabled = true;
            label3.Text = "Training Complete";
        }

        Random r = new Random();

        public void TestData(PaintEventArgs e)
        {
            if (gqd)
            {
                Console.WriteLine("Testing");
                int index = r.Next(0, 10000);
                Console.WriteLine(index);
                int type = r.Next(0, 2);
                string url;
                if (type == 0) url = "google/full_simplified_giraffe.ndjson";
                else url = "google/full_simplified_cat.ndjson";
                id.DrawFrame(GetCat(url, index), e, scalar, false);
                //DrawCat(url, index);
                double highest = id.CalculateHighest(nn.Feedforward(id.Flattern(GetCat(url, index), 255)));

                label1.Text = $"Correct Answer: { (type == 0 ? "Giraffe" : "Cat" )}";
                //label1.Refresh();

                label2.Text = $"Guess: {(highest == 0 ? "Cat" : "Giraffe")}";
                //label2.Refresh();

                return;
            }

            Console.WriteLine(guessing++);

            var item = testing[r.Next(0, 10000)];
            double[,] tData = Matrix.Transpose(Matrix.FromArray(item.Data)).data;

            double[] sendingData = id.BlankFrame(784);
            double[] sendingResult = id.BlankFrame(10);

            sendingResult = id.BlankFrame(10);

            if (emnist) label1.Text = $"Correct Answer: { alphabet[item.Label - 1]}";
            else label1.Text = $"Correct Answer: {item.Label}";
            label1.Refresh();

            id.DrawFrame(tData, e, scalar, emnist);

            sendingData = id.Flattern(tData, 255);

            double[] guesses = nn.Feedforward(sendingData);
            double guess = id.CalculateHighest(guesses);

            if (emnist) label2.Text = $"Guess: { alphabet[Int32.Parse(guess.ToString()) - 1]}";
            else label2.Text = $"Guess: {guess}";
        }

        public void TestData(double[,] data, PaintEventArgs e)
        {

            Console.WriteLine(guessing++);

            var item = data;
            double[,] tData = Matrix.Transpose(Matrix.FromArray(item)).data;

            double[] sendingData = id.BlankFrame(784);
            double[] sendingResult = id.BlankFrame(10);

            sendingResult = id.BlankFrame(10);

            label1.Text = $"Guessing";
            label1.Refresh();

            id.DrawFrame(tData, e, scalar, emnist);

            sendingData = id.Flattern(tData, 1);

            double[] guesses = nn.Feedforward(sendingData);
            double guess = id.CalculateHighest(guesses);

            label2.Text = $"Guess is {guess}";
        }

        public void TestData(double[,] data)
        {
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

        }

        public void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (!start) return;

            if (thumbnail)
            {
                Console.WriteLine("Drawn!");
                id.DrawFrame(ps, e, scalar, emnist);
                //return;
            }


            if (isTraining)
            {
                TrainData(trainingAmount, e);
            }
            else
            {
                TestData(e);
            }
        }

        public void button1_Click(object sender, EventArgs e)
        {
            panel1.Refresh();
        }

        public void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            start = true;
            panel1.Refresh();
        }

        public void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            disp = !checkBox1.Checked;
        }

        public void panel1_Click(object sender, EventArgs e)
        {
            thumbnail = true;
            panel1.Refresh();
        }

        public void DrawBtn_Click(object sender, EventArgs e)
        {
            Draw d = new Draw();
            d.Show();
        }

        public void SetLabel1(string txt)
        {
            label1.Text = txt;
            label1.Refresh();
            panel1.Refresh();
        }

        public void RefreshPanel()
        {
            panel1.Refresh();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void changeToMNISTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            emnist = false;
            MNISTReader.SetType("mnist/train-images.idx3-ubyte", "mnist/train-labels.idx1-ubyte", "mnist/t10k-images.idx3-ubyte", "mnist/t10k-labels.idx1-ubyte");
            testing = MNISTReader.ReadTestData().ToList();
            training = MNISTReader.ReadTrainingData().ToList();
        }

        private void changeToEMNISTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            emnist = false;
            trainingAmount = 30000;
            nn = new NeuralNetwork(784, 64, 26);
            MNISTReader.SetType("emnist/emnist-letters-train-images-idx3-ubyte", "emnist/emnist-letters-train-labels-idx1-ubyte", "emnist/emnist-letters-test-images-idx3-ubyte", "emnist/emnist-letters-test-labels-idx1-ubyte");
            testing = MNISTReader.ReadTestData().ToList();
            training = MNISTReader.ReadTrainingData().ToList();
        }

        private void changeToGoogleDrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            start = true;
            button2.Enabled = false;
            scalar = 2;
            nn = new NeuralNetwork(4096, 64, 2);
            for (int i = 0; i < 10000; i++)
            {
                
                if (i % 2 == 0)
                {
                    nn.Train(Matrix.FromArray(id.Flattern(GetCat("google/full_simplified_giraffe.ndjson", i), 255)), Matrix.FromArray(new double[] { 0, 1 }));
                    if (!checkBox1.Checked) DrawCat("google/full_simplified_giraffe.ndjson", i);
                }
                else
                {
                    nn.Train(Matrix.FromArray(id.Flattern(GetCat("google/full_simplified_cat.ndjson", i), 255)), Matrix.FromArray(new double[] { 1, 0 }));
                    if (!checkBox1.Checked) DrawCat("google/full_simplified_cat.ndjson", i);
                }
                label3.Text = $"{i} / 10000";
                label3.Refresh();
                progressBar1.Value = i / 10000 * 100;
            }

            label3.Text = "Complete!";

            gqd = true;
            checkBox1.Enabled = false;
            button1.Enabled = true;

            //google/full_simplified_giraffe.ndjson
            //google/full_simplified_cat.ndjson

            //Tes
            /*
            int corr = 0;

            for (int i = 0; i < 1500; i++)
            {
                int index = new Random().Next(0, 1000);
                if (i % 3 == 0)
                {
                    DrawCat("google/full_simplified_giraffe.ndjson", index);
                    double highest = id.CalculateHighest(nn.Feedforward(id.Flattern(GetCat("google/full_simplified_giraffe.ndjson", index), 255)));
                    Console.WriteLine(highest);
                    if (highest == 1)
                    {
                        corr++;
                    }
                }
                else
                {
                    DrawCat("google/full_simplified_cat.ndjson", index);
                    double highest = id.CalculateHighest(nn.Feedforward(id.Flattern(GetCat("google/full_simplified_cat.ndjson", index), 255)));
                    Console.WriteLine(highest);
                    if (highest == 0)
                    {
                        corr++;
                    }
                }
            }
            Console.WriteLine($"Got: {corr} correct! / 1500");*/
        }

        public double[,] GetCat(string url, int index)
        {
            trainingAmount = 30000;
            string line1 = File.ReadLines(url).Skip(index).Take(1).First();
            dynamic c = JsonConvert.DeserializeObject(line1);
            bool isX = true;
            List<int[]> xs = new List<int[]>();
            List<int[]> ys = new List<int[]>();
            int count = 0;
            int stroke = 0;

            foreach (var line in c.drawing.Children())
            {

                foreach (var x in line.Children())
                {
                    List<int> tmpList = new List<int>();
                    foreach (var y in x.Children())
                    {
                        tmpList.Add(Int32.Parse(y.ToString()));
                    }
                    if (isX) xs.Add(tmpList.ToArray());
                    else ys.Add(tmpList.ToArray());

                    isX = !isX;
                }
                stroke++;
                count++;
            }

            double x1;
            double y1;
            double x2;
            double y2;

            double[,] pic = id.BlankFrame(64, 64);
            // THIS
            for (int y = 0; y < stroke; y++)
            {
                for (int x = 0; x < xs[y].Length - 1; x++)
                {
                    //pic[xs[y][x], ys[y][x]] = 255;

                    x1 = xs[y][x];
                    y1 = ys[y][x];
                    x2 = xs[y][x + 1];
                    y2 = ys[y][x + 1];
                    //Console.WriteLine($"{x1},{y1} to {x2}, {y2}");

                    double m;
                    if ((x1 == x2)) m = 0;
                    else m = (y2 - y1) / (x2 - x1);

                    double C = y1 - (m * x1);
                    //Console.WriteLine($"y = {m}x + {C}");

                    for (int X = 0; X <= Math.Abs(x1 - x2); X++)
                    {
                        int offset = x1 - x2 > 0 ? -X : X;
                        int X_ = Convert.ToInt32(x1 + offset);
                        int Y_ = Convert.ToInt32((m * (X_)) + C);
                        //Console.WriteLine($"y is {Y_}");
                        try
                        {
                            //Console.WriteLine($"Writing to: {X_}, {Y_}");
                            pic[X_/4, Y_/4] = 255;

                        }
                        catch (Exception)
                        {
                            //
                        }
                    }
                    //Console.ReadLine();

                    //Console.WriteLine($"{xs[y][x]}, {ys[y][x]}");
                }
            }


            //foreach (double x in id.Flattern(pic, 255))
            //{
            //    if (x != 0) Console.WriteLine($"Found: {x}");
            //}
            //}

            return pic;
        }

        public void DrawCat(string url, int index)
        {
            trainingAmount = 30000;
            string line1 = File.ReadLines(url).Skip(index).Take(1).First();
            dynamic c = JsonConvert.DeserializeObject(line1);
            bool isX = true;
            List<int[]> xs = new List<int[]>();
            List<int[]> ys = new List<int[]>();
            int count = 0;
            int stroke = 0;


            foreach (var line in c.drawing.Children())
            {

                foreach (var x in line.Children())
                {
                    List<int> tmpList = new List<int>();
                    foreach (var y in x.Children())
                    {
                        tmpList.Add(Int32.Parse(y.ToString()));
                    }
                    if (isX) xs.Add(tmpList.ToArray());
                    else ys.Add(tmpList.ToArray());

                    isX = !isX;
                }
                stroke++;
                count++;
            }

            double x1;
            double y1;
            double x2;
            double y2;

            double[,] pic = id.BlankFrame(64, 64);
            // THIS
            for (int y = 0; y < stroke; y++)
            {
                for (int x = 0; x < xs[y].Length - 1; x++)
                {
                    //pic[xs[y][x], ys[y][x]] = 255;

                    x1 = xs[y][x];
                    y1 = ys[y][x];
                    x2 = xs[y][x + 1];
                    y2 = ys[y][x + 1];
                   // Console.WriteLine($"{x1},{y1} to {x2}, {y2}");

                    double m;
                    if ((x1 == x2)) m = 0;
                    else m = (y2 - y1) / (x2 - x1);

                    double C = y1 - (m * x1);
                    //Console.WriteLine($"y = {m}x + {C}");

                    for (int X = 0; X <= Math.Abs(x1 - x2); X++)
                    {
                        int offset = x1 - x2 > 0 ? -X : X;
                        int X_ = Convert.ToInt32(x1 + offset);
                        int Y_ = Convert.ToInt32((m * (X_)) + C);
                        //Console.WriteLine($"y is {Y_}");
                        try
                        {
                            //Console.WriteLine($"Writing to: {X_}, {Y_}");
                            pic[X_/4, Y_/4] = 255;

                        }
                        catch (Exception)
                        {
                            //
                        }
                    }
                   // Console.ReadLine();

                    //Console.WriteLine($"{xs[y][x]}, {ys[y][x]}");
                }
            }


            //foreach (double x in id.Flattern(pic, 255))
            //{
            //    if (x != 0) Console.WriteLine($"Found: {x}");
            //}
            emnist = false;
            start = true;
            ps = pic;
            thumbnail = true;
            panel1.Refresh();
        }
    }
}
