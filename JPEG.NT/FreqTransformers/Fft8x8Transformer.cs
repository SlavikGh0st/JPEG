using System;
using System.Numerics;
using JPEG.NT.Utilities;

namespace JPEG.NT.FreqTransformers
{
    public class Fft8X8Transformer : IFreqTransformer<Complex>
    {
        private static readonly Complex[] expTable = new Complex[7];

        public Fft8X8Transformer()
        {
            var length = new int[] {8, 4, 2};
            foreach (var l in length)
            {
                var c = -2 * Math.PI / l;
                for (var i = 0; i < l / 2; i++)
                    expTable[8 - l + i] = Complex.FromPolarCoordinates(1, i * c);
            }
        }

        public Complex[,] FreqTransform2D(double[,] input)
        {
            var complex = new Complex[8, 8];
            for (var j = 0; j < 8; j++)
            for (var i = 0; i < 8; i++)
                complex[j, i] = input[j, i];

            return FreqTransfrom2D(complex);
        }

        public double[,] IFreqTransform2D(Complex[,] input)
        {
            for (var j = 0; j < 8; j++)
            for (var i = 0; i < 8; i++)
                input[j, i] = new Complex(input[j, i].Real, -input[j, i].Imaginary);

            var ifft = FreqTransfrom2D(input);

            var ifft_real = new double[8, 8];
            for (var j = 0; j < 8; j++)
            for (var i = 0; i < 8; i++)
                ifft_real[j, i] = ifft[j, i].Real / 64;

            return ifft_real;
        }

        private static Complex[,] FreqTransfrom2D(Complex[,] input)
        {
            var fft = new Complex[8, 8];
            for (var i = 0; i < 8; i++)
            {
                var row = input.GetRow(i);
                var fftRow = TransformRadix2(row);
                fft.SetRow(i, fftRow);
            }

            for (var j = 0; j < 8; j++)
            {
                var col = fft.GetColumn(j);
                var fftCol = TransformRadix2(col);
                fft.SetColumn(j, fftCol);
            }

            return fft;
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