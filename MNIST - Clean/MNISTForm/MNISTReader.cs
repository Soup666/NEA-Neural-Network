using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MNISTForm
{
    public class Image
    {
        public byte Label { get; set; }
        public double[,] Data { get; set; }
    }

    public static class Extensions
    {
        public static int ReadBigInt32(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(Int32));
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static void ForEach<T>(this T[,] source, Action<int, int> action)
        {
            for (int w = 0; w < source.GetLength(0); w++)
            {
                for (int h = 0; h < source.GetLength(1); h++)
                {
                    action(w, h);
                }
            }
        }
    }

    public static class MNISTReader
    {
        private static string TrainImages = "";
        private static string TrainLabels = "";
        private static string TestImages = "";
        private static string TestLabels = "";
        /*
        private static string TrainImages = "mnist/train-images.idx3-ubyte";
        private static string TrainLabels = "mnist/train-labels.idx1-ubyte";
        private static string TestImages = "mnist/t10k-images.idx3-ubyte";
        private static string TestLabels = "mnist/t10k-labels.idx1-ubyte";*/

        public static void SetType(string TrainImages_, string TrainLabels_, string TestImages_, string TestLabels_)
        {
            TrainImages = TrainImages_;
            TrainLabels = TrainLabels_;
            TestImages = TestImages_;
            TestLabels = TestLabels_;
        }

        public static IEnumerable<Image> ReadTrainingData()
        {
            foreach (var item in Read(TrainImages, TrainLabels))
            {
                yield return item;
            }
        }

        public static IEnumerable<Image> ReadTestData()
        {
            foreach (var item in Read(TestImages, TestLabels))
            {
                yield return item;
            }
        }

        private static IEnumerable<Image> Read(string imagesPath, string labelsPath)
        {
            BinaryReader labels = new System.IO.BinaryReader(new FileStream(labelsPath, FileMode.Open));
            BinaryReader images = new BinaryReader(new FileStream(imagesPath, FileMode.Open));

            int magicNumber = images.ReadBigInt32();
            int numberOfImages = images.ReadBigInt32();
            int width = images.ReadBigInt32();
            int height = images.ReadBigInt32();

            int magicLabel = labels.ReadBigInt32();
            int numberOfLabels = labels.ReadBigInt32();

            for (int i = 0; i < numberOfImages; i++)
            {
                var bytes = images.ReadBytes(width * height);
                var arr = new double[height, width];

                arr.ForEach((j, k) => arr[j, k] = bytes[j * height + k]);

                yield return new Image()
                {
                    Data = arr,
                    Label = labels.ReadByte()
                };
            }
        }
    }
}
