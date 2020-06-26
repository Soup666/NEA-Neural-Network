using System;
using System.Reflection;

namespace NN
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            NeuralNetwork nn = new NeuralNetwork(2,2,1);

            Console.WriteLine("====");

            double[,] training_data =
            {
                {0,1,1}, {1,0,1}, {1,1,0}, {0,0,0}
            };

            Random r = new Random();
            for (int i = 0; i < 1000000; i++)
            {
                int x = r.Next(0, 4);
                double[] input = new double[2];
                input[0] = training_data[x, 0];
                input[1] = training_data[x, 1];


                double[] target = new double[1];
                target[0] = training_data[x, 2];


                nn.Train(Matrix.FromArray(input), Matrix.FromArray(target));
            }

            Console.WriteLine(nn.Feedforward(new double[] { 0.0, 1.0 })[0]);
            Console.WriteLine(nn.Feedforward(new double[] { 1.0, 0.0 })[0]);
            Console.WriteLine(nn.Feedforward(new double[] { 0.0, 0.0 })[0]);
            Console.WriteLine(nn.Feedforward(new double[] { 1.0, 1.0 })[0]);

            /*
            double[] inputs = { 1.0, 2.0 };
            double[] targets = { 1.0 };

            Matrix i = Matrix.FromArray(inputs);
            Matrix t = Matrix.FromArray(targets);

            nn.Train(i, t);



            
            double[,] input1 = { { -0.49, 0.85 }, { 0.11, -0.33 }, { 0.25, -0.59 } };
            Matrix m1 = Matrix.Transpose(Matrix.FromArray(input1));
            

            m1.PrintData();

            Console.WriteLine("===");

            double[,] input2 = { { 1.0, 1.0, 1.0}};
            Matrix m2 = Matrix.Transpose(Matrix.FromArray(input2));

            m2.PrintData();

            Console.WriteLine("===");

            Matrix.Multiply(m1, m2).PrintData();*/
        }
    }
}
