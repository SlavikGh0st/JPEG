using System;
using System.Numerics;
using JPEG.NT.Utilities;

namespace JPEG.NT.FreqTransformers
{
    public class FftTransformer : IFreqTransformer<Complex>
    {
        public Complex[,] FreqTransform2D(float[,] input)
        {
            var height = input.GetLength(0);
            var width = input.GetLength(1);

            var complex = new Complex[height, width];
            for (var j = 0; j < height; j++)
            for (var i = 0; i < width; i++)
                complex[j, i] = input[j, i];

            return FreqTransfrom2D(complex);
        }

        public float[,] IFreqTransform2D(Complex[,] input)
        {
            var height = input.GetLength(0);
            var width = input.GetLength(1);
            var size = width * height;

            for (var j = 0; j < height; j++)
            for (var i = 0; i < width; i++)
                input[j, i] = new Complex(input[j, i].Real, -input[j, i].Imaginary);

            var ifft = FreqTransfrom2D(input);

            var ifft_real = new float[height, width];
            for (var j = 0; j < height; j++)
            for (var i = 0; i < width; i++)
                ifft_real[j, i] = (float) ifft[j, i].Real / size;

            return ifft_real;
        }

        private static Complex[,] FreqTransfrom2D(Complex[,] input)
        {
            var height = input.GetLength(0);
            var width = input.GetLength(1);

            var fft = new Complex[height, width];
            for (var i = 0; i < width; i++)
            {
                var row = input.GetRow(i);
                var fftRow = TransformRadix2(row);
                fft.SetRow(i, fftRow);
            }

            for (var j = 0; j < height; j++)
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

            var c = -2 * (float) Math.PI / length;
            var dct = new Complex[length];
            for (var i = 0; i < length / 2; i++)
            {
                var q = Complex.FromPolarCoordinates(1, i * c) * odd[i];
                dct[i] = even[i] + q;
                dct[i + length / 2] = even[i] - q;
            }

            return dct;
        }
    }
}