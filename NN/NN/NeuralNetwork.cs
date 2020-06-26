using System;
using System.Reflection;

namespace NN
{
    public class NeuralNetwork
    {
        private int inodes;
        private int hnodes;
        private int onodes;

        public Matrix weights_ih;
        public Matrix weights_ho;

        public Matrix bias_h;
        public Matrix bias_o;

        public double learning_rate;


        public NeuralNetwork(int _inodes, int _hnodes, int _onodes)
        {
            inodes = _inodes;
            hnodes = _hnodes;
            onodes = _onodes;

            weights_ih = new Matrix(hnodes, inodes);
            weights_ho = new Matrix(onodes, hnodes);

            weights_ih.Randomize();
            weights_ho.Randomize();

            bias_h = new Matrix(hnodes, 1);
            bias_o = new Matrix(hnodes, 1);

            learning_rate = 0.1;
        }

        public double[] Feedforward(double[,] input_array)
        {
            Matrix inputOuputs = Matrix.FromArray(input_array);

            Matrix hiddenOuputs = Matrix.Multiply(weights_ih, inputOuputs);
            hiddenOuputs = Matrix.Add(hiddenOuputs, bias_h);
            Matrix.Map(ref hiddenOuputs, Matrix.Sigmoid);

            Matrix finalOutputs = Matrix.Multiply(weights_ho, hiddenOuputs);
            finalOutputs = Matrix.Add(finalOutputs, bias_o);
            Matrix.Map(ref finalOutputs, Matrix.Sigmoid);
             
            return finalOutputs.ToArray();
        }

        public void Train(Matrix inputs, Matrix targets)
        {
            // Feedforward
            Matrix inputOuputs = inputs;

            Matrix hiddenOuputs = Matrix.Multiply(weights_ih, inputOuputs);
            hiddenOuputs = Matrix.Add(hiddenOuputs, bias_h);
            Matrix.Map(ref hiddenOuputs, Matrix.Sigmoid);

            Matrix finalOutputs = Matrix.Multiply(weights_ho, hiddenOuputs);
            finalOutputs = Matrix.Add(finalOutputs, bias_o);
            Matrix.Map(ref finalOutputs, Matrix.Sigmoid);

            //

            double[] outputs = finalOutputs.ToArray();
            double[] error = new double[outputs.Length];
            
            for (int i = 0; i < outputs.Length; i++)
            {
                error[i] = targets.data[i,0] - outputs[i];
            }

            Matrix gradients = Matrix.FromArray(outputs);
            Matrix.Map(ref gradients, flip); // desigmoid but sigmoid has already been sigmoided
            gradients = Matrix.Multiply(gradients, Matrix.FromArray(error));
            gradients = Matrix.Multiply(gradients, learning_rate);


            Matrix hiddenT = Matrix.Transpose(hiddenOuputs);
            Matrix weights_ho_deltas = Matrix.Multiply(gradients, hiddenT);

            Matrix.Add(weights_ho, weights_ho_deltas);

            Matrix who_t = Matrix.Transpose(weights_ho);
            Matrix hidden_errors = Matrix.Multiply(who_t, Matrix.FromArray(error));

            Matrix hidden_gradients = hiddenOuputs;
            Matrix.Map(ref hidden_gradients, flip);

            hidden_gradients = Matrix.Multiply(hidden_gradients, hidden_errors);
            hidden_gradients = Matrix.Multiply(hidden_gradients, learning_rate);

            Matrix inputsT = Matrix.Transpose(inputs);
            Matrix weights_ih_deltas = Matrix.Multiply(hidden_gradients, inputsT);

            Matrix.Add(weights_ih, weights_ih_deltas);

            //Working

            Console.WriteLine($"{bias_o.sizeX} by {bias_o.sizeY} add {gradients.sizeX} by {gradients.sizeY}");

            //bias_o = Matrix.Add(bias_o, gradients);
            //bias_h = Matrix.Add(bias_h, hidden_gradients);
        }

        private double flip(double i)
        {
            return i * (1 - i);
        }
    }
}
