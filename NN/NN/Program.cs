using System;
using System.Reflection;

namespace NN
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The XOR Problem!");

            NeuralNetwork nn = new NeuralNetwork(2,2,1);

            Console.WriteLine("==============");

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

            while (true)
            {
                Console.WriteLine("Enter the first number: ");
                double num1 = double.Parse(Console.ReadLine());
                Console.WriteLine("Enter the second number: ");
                double num2 = double.Parse(Console.ReadLine());
                Console.WriteLine(nn.Feedforward(new double[] { num1, num2 })[0]);
                Console.WriteLine("==================");
            }
            /*
            Console.WriteLine(nn.Feedforward(new double[] { 0.0, 1.0 })[0]);
            Console.WriteLine(nn.Feedforward(new double[] { 1.0, 0.0 })[0]);
            Console.WriteLine(nn.Feedforward(new double[] { 0.0, 0.0 })[0]);
            Console.WriteLine(nn.Feedforward(new double[] { 1.0, 1.0 })[0]);*/

        }
    }
}
