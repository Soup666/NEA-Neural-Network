using System;
using System.Reflection;

namespace NN
{
    class Program
    {
        static void Main(string[] args)
        {

            foreach (var image in MNISTReader.ReadTrainingData())
            {
                
            }
            /*
            Console.WriteLine(nn.Feedforward(new double[] { 0.0, 1.0 })[0]);
            Console.WriteLine(nn.Feedforward(new double[] { 1.0, 0.0 })[0]);
            Console.WriteLine(nn.Feedforward(new double[] { 0.0, 0.0 })[0]);
            Console.WriteLine(nn.Feedforward(new double[] { 1.0, 1.0 })[0]);*/

        }
    }
}
