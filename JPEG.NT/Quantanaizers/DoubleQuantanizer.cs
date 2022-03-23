using System;

namespace JPEG.NT.Quantanaizers
{
    public class DoubleQuantanizer : IQuantanizer<float, byte>
    {
        private int qualityFactor;
        private int[,] quantizationMatrix;

        public int QualityFactor
        {
            get => qualityFactor;
            set
            {
                if (value < 1 || value > 99)
                    throw new ArgumentException("quality must be in [1,99] interval");

                if (value == qualityFactor)
                    return;

                qualityFactor = value;
                GetQuantizationMatrix(qualityFactor);
            }
        }

        public DoubleQuantanizer(int qualityFactor = 10)
        {
            this.qualityFactor = qualityFactor;
            GetQuantizationMatrix(qualityFactor);
        }

        public byte[,] Quantize(float[,] input)
        {
            var width = input.GetLength(1);
            var height = input.GetLength(0);

            var result = new byte[height, width];
            for (var j = 0; j < height; j++)
            for (var i = 0; i < width; i++)
            {
                result[j, i] = (byte) (input[j, i] / quantizationMatrix[j, i]);
            }

            return result;
        }

        public float[,] DeQuantize(byte[,] input)
        {
            var width = input.GetLength(1);
            var height = input.GetLength(0);

            var result = new float[height, width];
            for (var j = 0; j < height; j++)
            for (var i = 0; i < width; i++)
            {
                result[j, i] =
                    ((sbyte) input[j, i]) *
                    quantizationMatrix[j, i]; //NOTE cast to sbyte not to loose negative numbers
            }

            return result;
        }

        private void GetQuantizationMatrix(int factor)
        {
            var multiplier = factor < 50 ? 5000 / factor : 200 - 2 * factor;

            quantizationMatrix = new[,]
            {
                {16, 11, 10, 16, 24, 40, 51, 61},
                {12, 12, 14, 19, 26, 58, 60, 55},
                {14, 13, 16, 24, 40, 57, 69, 56},
                {14, 17, 22, 29, 51, 87, 80, 62},
                {18, 22, 37, 56, 68, 109, 103, 77},
                {24, 35, 55, 64, 81, 104, 113, 92},
                {49, 64, 78, 87, 103, 121, 120, 101},
                {72, 92, 95, 98, 112, 100, 103, 99}
            };
            var width = quantizationMatrix.GetLength(1);
            var height = quantizationMatrix.GetLength(0);

            for (var j = 0; j < height; j++)
            for (var i = 0; i < width; i++)
            {
                quantizationMatrix[j, i] = (multiplier * quantizationMatrix[j, i] + 50) / 100;
            }
        }
    }
}