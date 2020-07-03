using System;
using System.Reflection;

namespace MNISTForm
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
            bias_o = new Matrix(onodes, 1);
            bias_h.Randomize();
            bias_o.Randomize();

            learning_rate = 0.1;
        }

        public double[] Feedforward(double[] input_array)
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
            Matrix hidden = Matrix.Multiply(weights_ih, inputs);
            hidden.Add(bias_h);
            hidden.Map(Matrix.Sigmoid);

            Matrix outputs = Matrix.Multiply(weights_ho, hidden);
            outputs.Add(bias_o);
            outputs.Map(Matrix.Sigmoid);


            Matrix output_errors = Matrix.Subtract(targets, outputs);

            Matrix gradients = outputs;
            Matrix.Map(ref gradients, flip);
            gradients = Matrix.Multiply(gradients, output_errors);
            gradients = Matrix.Multiply(gradients, learning_rate);

            Matrix hidden_T = Matrix.Transpose(hidden);
            Matrix weight_ho_deltas = Matrix.Multiply(gradients, hidden_T);

            weights_ho.Add(weight_ho_deltas);
            bias_o.Add(gradients);

            Matrix who_t = Matrix.Transpose(weights_ho);
            Matrix hidden_errors = Matrix.Multiply(who_t, output_errors);

            Matrix hidden_gradient = hidden;
            Matrix.Map(ref hidden_gradient, flip);
            hidden_gradient = Matrix.Multiply(hidden_gradient, hidden_errors);
            hidden_gradient = Matrix.Multiply(hidden_gradient, learning_rate);

            Matrix inputs_T = Matrix.Transpose(inputs);
            Matrix weights_ih_deltas = Matrix.Multiply(hidden_gradient, inputs_T);

            weights_ih.Add(weights_ih_deltas);
            bias_h.Add(hidden_gradient);

            //Console.WriteLine("Done!");
        }

        private double flip(double i)
        {
            return i * (1 - i);
        }
    }
}
