using System;
using System.Numerics;
using JPEG.NT.Utilities;

namespace JPEG.NT.FreqTransformers
{
    public class Rfft8X8Transformer : IFreqTransformer<float>
    {
        private static readonly Complex[] expTable = new Complex[7];
        private static readonly Complex[] expRTable = new Complex[8];

        public Rfft8X8Transformer()
        {
            var length = new[] {8, 4, 2};
            foreach (var l in length)
            {
                var c = -2 * (float) Math.PI / l;
                for (var i = 0; i < l / 2; i++)
                    expTable[8 - l + i] = Complex.FromPolarCoordinates(1, i * c);
            }

            for (var i = 0; i < 8; i++)
                expRTable[i] = Complex.Exp(new Complex(0, -i * (float) Math.PI / 16));
        }

        public float[,] FreqTransform2D(float[,] input)
        {
            var fft = new float[8, 8];
            for (var i = 0; i < 8; i++)
            {
                var row = input.GetRow(i);
                var fftRow = FreqTransform1D(row);
                fft.SetRow(i, fftRow);
            }

            for (var j = 0; j < 8; j++)
            {
                var col = fft.GetColumn(j);
                var fftCol = FreqTransform1D(col);
                fft.SetColumn(j, fftCol);
            }

            return fft;
        }

        public float[,] IFreqTransform2D(float[,] dct)
        {
            var fft = new float[8, 8];
            for (var i = 0; i < 8; i++)
            {
                var row = dct.GetRow(i);
                var fftRow = IFreqTransform1D(row);
                fft.SetRow(i, fftRow);
            }

            for (var j = 0; j < 8; j++)
            {
                var col = fft.GetColumn(j);
                var fftCol = IFreqTransform1D(col);
                fft.SetColumn(j, fftCol);
            }

            return fft;
        }

        private float[] FreqTransform1D(float[] input)
        {
            var complex = new Complex[8];
            for (var i = 0; i < 8; i++)
                complex[i] = input[i];

            var fft = TransformRadix2(complex);

            for (var i = 0; i < 8; i++)
                input[i] = (float) (fft[i] * expRTable[i]).Real;

            return input;
        }

        private float[] IFreqTransform1D(float[] input)
        {
            input[0] /= 2;

            var complex = new Complex[8];
            for (var i = 0; i < 8; i++)
                complex[i] = input[i] * expRTable[i];

            var ifft = TransformRadix2(complex);

            for (var i = 0; i < 8; i++)
                input[i] = (float) ifft[i].Real / 4;

            return input;
        }

        private static Complex[] TransformRadix2(Complex[] input)
        {
            var length = input.Length;
            if (length <= 1)
                return input;

            var even = new Complex[length / 2];
            var odd = new Complex[length / 2];
            for (var i = 0; i < length / 2; i++)
            {
                even[i] = input[i * 2];
                odd[i] = input[i * 2 + 1];
            }

            even = TransformRadix2(even);
            odd = TransformRadix2(odd);

            var dct = new Complex[length];
            for (var i = 0; i < length / 2; i++)
            {
                var q = expTable[8 - length + i] * odd[i];
                dct[i] = even[i] + q;
                dct[i + length / 2] = even[i] - q;
            }

            return dct;
        }
    }
}