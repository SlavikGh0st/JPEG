using System;
using System.Threading.Tasks;

namespace JPEG.NT.FreqTransformers
{
    public class ParallelDctTransformer : IFreqTransformer<float>
    {
        private static readonly float CoefAlpha = 1 / (float) Math.Sqrt(2);

        public float[,] FreqTransform2D(float[,] input)
        {
            var height = input.GetLength(0);
            var width = input.GetLength(1);
            var dct = new float[height, width];

            Parallel.For(0, height, v =>
            {
                for (var u = 0; u < width; u++)
                {
                    var sum = 0f;
                    for (var y = 0; y < height; y++)
                    for (var x = 0; x < width; x++)
                    {
                        sum += BasisFunction(input[y, x], u, v, x, y, width, height);
                    }

                    dct[v, u] = sum * Beta(height, width) * Alpha(v) * Alpha(u);
                }
            });

            return dct;
        }

        public float[,] IFreqTransform2D(float[,] dct)
        {
            var height = dct.GetLength(0);
            var width = dct.GetLength(1);
            var output = new float[height, width];

            Parallel.For(0, height, y =>
            {
                for (var x = 0; x < width; x++)
                {
                    var sum = 0f;
                    for (var v = 0; v < height; v++)
                    for (var u = 0; u < width; u++)
                    {
                        sum += BasisFunction(dct[v, u], u, v, x, y, width, height) * Alpha(u) * Alpha(v);
                    }

                    output[y, x] = sum * Beta(height, width);
                }
            });

            return output;
        }

        private static float BasisFunction(float a, int u, int v, int x, int y, int width, int height)
        {
            var b = (float) Math.Cos(((2d * x + 1d) * u * (float) Math.PI) / (2 * width));
            var c = (float) Math.Cos(((2d * y + 1d) * v * (float) Math.PI) / (2 * height));

            return a * b * c;
        }

        private static float Alpha(int u)
        {
            if (u == 0)
                return CoefAlpha;
            return 1;
        }

        private static float Beta(int height, int width)
        {
            return 1f / width + 1f / height;
        }
    }
}