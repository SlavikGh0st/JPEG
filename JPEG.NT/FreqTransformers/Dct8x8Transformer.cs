using System;

namespace JPEG.NT.FreqTransformers
{
    public class Dct8X8Transformer : IFreqTransformer<float>
    {
        private static readonly float[,] CosTable = new float[8, 8];
        private static readonly float CoefAlpha = 1 / (float) Math.Sqrt(2);

        public Dct8X8Transformer()
        {
            for (var j = 0; j < 8; j++)
            for (var i = 0; i < 8; i++)
                CosTable[j, i] = (float) Math.Cos((2 * j + 1) * i * (float) Math.PI / 16);
        }

        public float[,] FreqTransform2D(float[,] input)
        {
            var dct = new float[8, 8];

            for (var v = 0; v < 8; v++)
            for (var u = 0; u < 8; u++)
            {
                var sum = 0f;
                for (var y = 0; y < 8; y++)
                for (var x = 0; x < 8; x++)
                {
                    sum += BasisFunction(input[y, x], u, v, x, y);
                }

                dct[v, u] = sum * Alpha(v) * Alpha(u) / 4;
            }

            return dct;
        }

        public float[,] IFreqTransform2D(float[,] dct)
        {
            var output = new float[8, 8];

            for (var y = 0; y < 8; y++)
            for (var x = 0; x < 8; x++)
            {
                var sum = 0f;
                for (var v = 0; v < 8; v++)
                for (var u = 0; u < 8; u++)
                {
                    sum += BasisFunction(dct[v, u], u, v, x, y) * Alpha(u) * Alpha(v);
                }

                output[y, x] = sum / 4;
            }

            return output;
        }

        private static float BasisFunction(float a, int u, int v, int x, int y)
        {
            var b = CosTable[x, u];
            var c = CosTable[y, v];
            return a * b * c;
        }

        private static float Alpha(int u)
        {
            if (u == 0)
                return CoefAlpha;
            return 1;
        }
    }
}