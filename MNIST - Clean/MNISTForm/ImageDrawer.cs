using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MNISTForm
{
    public class ImageDrawer
    {

        public void DrawFrame(double[,] data, PaintEventArgs e, int scalar, bool emnist)
        {
            for (int x = 0; x < data.GetLength(0); x++)
            {
                for (int y = 0; y < data.GetLength(1); y++)
                {
                    byte c = (byte)data[x, y];

                    SolidBrush s = new SolidBrush(Color.FromArgb(255, c, c, c));
                    if (emnist) e.Graphics.FillRectangle(s, y * scalar, x * scalar, 1 * scalar, 1 * scalar);
                    else e.Graphics.FillRectangle(s, x * scalar, y * scalar, 1 * scalar, 1 * scalar);
                }
            }
        }
        
        public double[] Flattern(double[,] data, double dividier)
        {
            double[] result = new double[data.GetLength(0) * data.GetLength(1)];

            int inc = 0;
            for (int x = 0; x < data.GetLength(0); x++)
            {
                for (int y = 0; y < data.GetLength(1); y++)
                {
                    result[inc++] = data[x, y] / dividier;
                }
            }

            return result;
        }

        public double[] FlatternAlt(double[,] data, double dividier)
        {
            double[] result = new double[data.GetLength(0) * data.GetLength(1)];

            int inc = 0;
            for (int x = 0; x < data.GetLength(0); x++)
            {
                for (int y = 0; y < data.GetLength(1); y++)
                {
                    result[inc++] = data[y,x] / dividier;
                }
            }

            return result;
        }

        public double[] BlankFrame(int size)
        {
            double[] result = new double[size];

            for (int i = 0; i < size; i++) result[i] = 0;

            return result;
        }

        public double[,] BlankFrame(int sizeX, int sizeY)
        {
            double[,] result = new double[sizeX, sizeY];

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    result[x, y] = 0;
                }
            }

            return result;
        }

        public double CalculateHighest(double[] arr)
        {
            double[] sorted = new double[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                sorted[i] = arr[i];
            }
            Array.Sort(sorted);

            return Array.IndexOf(arr, sorted[sorted.Length-1]);
        }
    }
}
