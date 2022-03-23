using System;

namespace JPEG.NT.FreqTransformers
{
    public class Dct8X8Transformer : IFreqTransformer<double>
    {
        private static readonly double[,] CosTable = new double[8, 8];
        private static readonly double CoefAlpha = 1 / Math.Sqrt(2);

        public Dct8X8Transformer()
        {
            for (var j = 0; j < 8; j++)
            for (var i = 0; i < 8; i++)
                CosTable[j, i] = Math.Cos((2 * j + 1) * i * Math.PI / 16);
        }

        public double[,] FreqTransform2D(double[,] input)
        {
            var dct = new double[8, 8];

            for (var v = 0; v < 8; v++)
            for (var u = 0; u < 8; u++)
            {
                var sum = 0d;
                for (var y = 0; y < 8; y++)
                for (var x = 0; x < 8; x++)
                {
                    sum += BasisFunction(input[y, x], u, v, x, y);
                }

                dct[v, u] = sum * Alpha(v) * Alpha(u) / 4;
            }

            return dct;
        }

        public double[,] IFreqTransform2D(double[,] dct)
        {
            var output = new double[8, 8];

            for (var y = 0; y < 8; y++)
            for (var x = 0; x < 8; x++)
            {
                var sum = 0d;
                for (var v = 0; v < 8; v++)
                for (var u = 0; u < 8; u++)
                {
                    sum += BasisFunction(dct[v, u], u, v, x, y) * Alpha(u) * Alpha(v);
                }

                output[y, x] = sum / 4;
            }

            return output;
        }

        private static double BasisFunction(double a, int u, int v, int x, int y)
        {
            var b = CosTable[x, u];
            var c = CosTable[y, v];
            return a * b * c;
        }

        private static double Alpha(int u)
        {
            if (u == 0)
                return CoefAlpha;
            return 1;
        }
    }
}