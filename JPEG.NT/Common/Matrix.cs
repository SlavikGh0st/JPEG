using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace JPEG.NT.Common
{
    public struct Matrix
    {
        public readonly Pixel[,] Pixels;
        public readonly int Height;
        public readonly int Width;

        public Matrix(int height, int width, PixelFormat format = PixelFormat.Rgb)
        {
            Height = height;
            Width = width;

            Pixels = new Pixel[Height, Width];
            for (var i = 0; i < height; ++i)
            for (var j = 0; j < width; ++j)
                Pixels[i, j] = new Pixel(0, 0, 0, format);
        }

        public Matrix(float[,] firstChannel, float[,] secondChannel, float[,] thirdChannel,
            PixelFormat format = PixelFormat.Rgb)
        {
            Height = firstChannel.GetLength(0);
            Width = firstChannel.GetLength(1);

            Pixels = new Pixel[Height, Width];
            for (var j = 0; j < Height; ++j)
            for (var i = 0; i < Width; ++i)
                Pixels[j, i] = new Pixel(firstChannel[j, i], secondChannel[j, i], thirdChannel[j, i], format);
        }

        
        public static unsafe explicit operator Matrix(Bitmap bmp)
        {
            var height = bmp.Height - bmp.Height % 8;
            var width = bmp.Width - bmp.Width % 8;
            var matrix = new Matrix(height, width);

            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                bmp.PixelFormat);
            var bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
            var ptr = (byte*) bmpData.Scan0;

            Parallel.For(0, height, y =>
            {
                var currentLine = ptr + (y * bmpData.Stride);
                for (var x = 0; x < width; x++)
                {
                    matrix.Pixels[y, x] = new Pixel(
                        currentLine[bytesPerPixel * x + 2],
                        currentLine[bytesPerPixel * x + 1],
                        currentLine[bytesPerPixel * x], PixelFormat.Rgb);
                }
            });

            bmp.UnlockBits(bmpData);
            return matrix;
        }

        public static unsafe explicit operator Bitmap(Matrix matrix)
        {
            var bmp = new Bitmap(matrix.Width, matrix.Height);

            var bmpData = bmp.LockBits(new Rectangle(0, 0, matrix.Width, matrix.Height), ImageLockMode.WriteOnly,
                bmp.PixelFormat);
            var bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
            var ptr = (byte*) bmpData.Scan0;

            Parallel.For(0, matrix.Height, y =>
            {
                var currentLine = ptr + (y * bmpData.Stride);
                for (var x = 0; x < matrix.Width; x++)
                {
                    currentLine[bytesPerPixel * x] = (byte) matrix.Pixels[y, x].B;
                    currentLine[bytesPerPixel * x + 1] = (byte) matrix.Pixels[y, x].G;
                    currentLine[bytesPerPixel * x + 2] = (byte) matrix.Pixels[y, x].R;
                }
            });

            bmp.UnlockBits(bmpData);
            return bmp;
        }


        public Matrix GetSubMatrix(int rowNumber, int rowCount, int columnNumber, int columnCount)
        {
            var subMatrix = new Matrix(columnCount, rowCount);

            for (var j = 0; j < rowCount; j++)
            for (var i = 0; i < columnCount; i++)
            {
                subMatrix.Pixels[j, i] = Pixels[rowNumber + j, columnNumber + i];
            }

            return subMatrix;
        }
        
        public Matrix SetSubMatrix(int rowNumber, int columnNumber, Matrix subMatrix)
        {
            for (var j = 0; j < subMatrix.Height; j++)
            for (var i = 0; i < subMatrix.Width; i++)
            {
                Pixels[rowNumber + j, columnNumber + i] = subMatrix.Pixels[j, i];
            }

            return subMatrix;
        }
        
        public IEnumerable<float[,]> GetColorChannels(IEnumerable<Func<Pixel, float>> componentSelector, int shift = 0)
        {
            foreach (var component in componentSelector)
            {
                var result = new float[Height, Width];
                for (var j = 0; j < Height; j++)
                for (var i = 0; i < Width; i++)
                    result[j, i] = component(Pixels[j, i]) + shift;

                yield return result;
            }
        }

        public void ShiftMatrixValues(int shiftValue)
        {
            for (var j = 0; j < Height; j++)
            for (var i = 0; i < Width; i++)
            {
                Pixels[j, i].FirstComponent += shiftValue;
                Pixels[j, i].SecondComponent += shiftValue;
                Pixels[j, i].ThirdComponent += shiftValue;
            }
        }


        public static Matrix ToMatrix(Bitmap bmp)
        {
            var height = bmp.Height - bmp.Height % 8;
            var width = bmp.Width - bmp.Width % 8;
            var matrix = new Matrix(height, width);

            for (var j = 0; j < height; j++)
            for (var i = 0; i < width; i++)
            {
                var pixel = bmp.GetPixel(i, j);
                matrix.Pixels[j, i] = new Pixel(pixel.R, pixel.G, pixel.B, PixelFormat.Rgb);
            }

            return matrix;
        }

        public static Bitmap ToBitmap(Matrix matrix)
        {
            var bmp = new Bitmap(matrix.Width, matrix.Height);

            for (var j = 0; j < bmp.Height; j++)
            for (var i = 0; i < bmp.Width; i++)
            {
                var pixel = matrix.Pixels[j, i];
                bmp.SetPixel(i, j, Color.FromArgb(ToByte(pixel.R), ToByte(pixel.G), ToByte(pixel.B)));
            }

            return bmp;
        }

        private static int ToByte(double d)
        {
            var val = (int) d;
            if (val > byte.MaxValue)
                return byte.MaxValue;
            if (val < byte.MinValue)
                return byte.MinValue;
            return val;
        }
    }
}