using System;

namespace JPEG.NT.FreqTransformers
{
    public class DctTransformer : IFreqTransformer<double>
    {
        private static readonly double CoefAlpha = 1 / Math.Sqrt(2);
        
        public double[,] FreqTransform2D(double[,] input)
        {
            var height = input.GetLength(0);
            var width = input.GetLength(1);
            var dct = new double[height, width];

            for (var v = 0; v < height; v++)
            for (var u = 0; u < width; u++)
            {
                var sum = 0d;
                for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                {
                    sum += BasisFunction(input[y, x], u, v, x, y, width, height);
                }

                dct[v, u] = sum * Beta(height, width) * Alpha(v) * Alpha(u);
            }

            return dct;
        }

        public double[,] IFreqTransform2D(double[,] dct)
        {
            var height = dct.GetLength(0);
            var width = dct.GetLength(1);
            var output = new double[height, width];

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var sum = 0d;
                for (var v = 0; v < height; v++)
                for (var u = 0; u < width; u++)
                {
                    sum += BasisFunction(dct[v, u], u, v, x, y, width, height) * Alpha(u) * Alpha(v);
                }

                output[y, x] = sum * Beta(height, width);
            }

            return output;
        }
        
        private static double BasisFunction(double a, int u, int v, int x, int y, int width, int height)
        {
            var b = Math.Cos(((2 * x + 1) * u * Math.PI) / (2 * width));
            var c = Math.Cos(((2 * y + 1) * v * Math.PI) / (2 * height));

            return a * b * c;
        }

        private static double Alpha(int u)
        {
            if (u == 0)
                return CoefAlpha;
            return 1;
        }

        private static double Beta(int height, int width)
        {
            return 1d / width + 1d / height;
        }
    }
}