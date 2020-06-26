using System;
namespace NN
{
    public class Matrix
    {
        public double[,] data;

        public int sizeX;
        public int sizeY;

        public Matrix(int _sizeX, int _sizeY)
        {
            data = new double[_sizeX, _sizeY];

            sizeX = _sizeX;
            sizeY = _sizeY;

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    data[x, y] = 0.0;
                }
            }
        }

        public void Randomize()
        {
            Random r = new Random();
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    data[x, y] = (double)r.Next(-100, 100) / 100;
                }
            }
        }

        public double[] ToArray()
        {
            double[] result = new double[sizeX * sizeY];
            int count = 0;
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    result[count++] = data[x, y];
                }
            }

            return result;
        }

        public void PrintData()
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    Console.Write(string.Format("{0}, ", data[i, j]));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }

        public static double Sigmoid(double value)
        {
            return (float)(1.0 / (1.0 + Math.Pow(Math.E, -value)));
        }

        public static Matrix Multiply(Matrix a, double b)
        {
            for (int x = 0; x < a.sizeX; x++)
            {
                for (int y = 0; y < a.sizeY; y++)
                {
                    a.data[x, y] *= b;
                }
            }
            return a;
        }


        public static Matrix Multiply(Matrix a, Matrix b)
        {
            Matrix result;
            if (a.sizeX == b.sizeX && a.sizeY == b.sizeY)
            {
                result = new Matrix(a.sizeX, a.sizeY);
                //Scalar
                for (int x = 0; x < a.sizeX; x++)
                {
                    for (int y = 0; y < a.sizeY; y++)
                    {
                        result.data[x, y] = a.data[x, y] * b.data[x, y];
                    }
                }
            }
            else
            {
                // Dot product
                result = new Matrix(a.sizeX, b.sizeY); //Square ofc
                                                              //Console.WriteLine($"Making {a.sizeX} by {b.sizeY}");
                for (int i = 0; i < result.sizeX; i++)
                {
                    for (int j = 0; j < b.sizeY; j++)
                    {
                        double sum = 0;
                        for (int k = 0; k < b.sizeX; k++)
                        {
                            try
                            {
                                sum += a.data[i, k] * b.data[k, j];
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error! Trying to multiple invalid matrixes... {a.sizeX} by {a.sizeY}, {a.sizeX} by {b.sizeY}");
                                return new Matrix(a.sizeX, a.sizeX);
                            }
                        }
                        result.data[i, j] = sum;
                    }
                }
            }
            return result;
        }

        public void Multiply(Matrix b)
        {
            Matrix result = new Matrix(b.sizeY, b.sizeY); //Square ofc
            //Console.WriteLine($"Making {a.sizeX} by {b.sizeY}");
            for (int i = 0; i < result.sizeX; i++)
            {
                for (int j = 0; j < b.sizeY; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < b.sizeX; k++)
                    {
                        try
                        {
                            sum += data[i, k] * b.data[k, j];
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error! Trying to multiple invalid matrixes... {data.GetLength(0)} by {data.GetLength(1)}, {b.sizeX} by {b.sizeY}");
                            
                        }
                    }
                    result.data[i, j] = sum;
                }
            }
            data = result.data;
            sizeX = result.sizeX;
            sizeY = result.sizeY;
        }

        public void Add(Matrix b)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    data[x, y] += b.data[x, y];
                }
            }
        }

        public void Subtract(Matrix b)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    data[x, y] -= b.data[x, y];
                }
            }
        }

        public static void PrintMatrix(Matrix a)
        {
            for (int i = 0; i < a.sizeX; i++)
            {
                for (int j = 0; j < a.sizeY; j++)
                {
                    Console.Write(string.Format(",{0} ", a.data[i, j]));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }

        public static void PrintMatrix(double[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                Console.Write($"{a[i]}, ");
            }
        }

        public static Matrix FromArray(double[,] array)
        {
            Matrix result = new Matrix(array.GetLength(0), array.GetLength(1));

            result.data = array;
            result.sizeX = array.GetLength(0);
            result.sizeY = array.GetLength(1);

            return result;
        }

        public static Matrix FromArray(double[] array)
        {
            Matrix result = new Matrix(array.Length, 1);

            for (int i = 0; i < array.Length; i++)
            {
                result.data[i, 0] = array[i];
            }
            result.sizeX = array.Length;
            result.sizeY = 1;

            return result;
        }

        public static Matrix Transpose(Matrix a)
        {
            Matrix result = new Matrix(a.sizeY, a.sizeX);
            for (int x = 0; x < a.sizeY; x++)
            {
                for (int y = 0; y < a.sizeX; y++)
                {
                    result.data[x, y] = a.data[y, x];
                }
            }

            return result;
        }

        public static Matrix Subtract(Matrix a, Matrix b)
        {
            if (a.sizeX != b.sizeX || a.sizeY != b.sizeY) Console.WriteLine("Attempting to add incorrect matrix's...");

            Matrix result = new Matrix(a.sizeX, a.sizeY);

            for (int x = 0; x < a.sizeX; x++)
            {
                for (int y = 0; y < a.sizeY; y++)
                {
                    result.data[x, y] = a.data[x, y] - b.data[x, y];
                }
            }

            return result;
        }

        public static Matrix Add(Matrix a, Matrix b)
        {
            if (a.sizeX != b.sizeX || a.sizeY != b.sizeY) Console.WriteLine("Attempting to add incorrect matrix's...");

            Matrix result = new Matrix(a.sizeX, a.sizeY);

            for (int x = 0; x < a.sizeX; x++)
            {
                for (int y = 0; y < a.sizeY; y++)
                {
                    result.data[x, y] = a.data[x, y] + b.data[x, y];
                }
            }

            return result;
        }

        public static void Map(ref Matrix a, Func<double, double> func)
        {
            for (int i = 0; i < a.sizeX; i++)
            {
                for (int j = 0; j < a.sizeY; j++)
                {
                    a.data[i, j] = func(a.data[i, j]);
                }
            }
        }

        public void Map(Func<double, double> func)
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    data[i, j] = func(data[i, j]);
                }
            }
        }


    }
}
