using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using FluentAssertions;
using JPEG.NT.Common;
using NUnit.Framework;
using PixelFormat = JPEG.NT.Common.PixelFormat;

namespace JPEG.Tests
{
    [TestFixture]
    public class ImageRW
    {
        private const string FileName = @"test.bmp";
        private Matrix matrix;

        [OneTimeSetUp]
        public void SetUp()
        {
            const int width = 8;
            const int height = 8;

            matrix = new Matrix(height, width);
            for (var j = 0; j < height; j++)
            for (var i = 0; i < width; i++)
            {
                var component = 255 / (i + j + 1);
                matrix.Pixels[j, i] = new Pixel(component, component, component, PixelFormat.Rgb);
            }
        }

        [Test]
        public void Matrix_ShouldWorksCorrect()
        {
            using (var bmp = (Bitmap) matrix)
            {
                bmp.Save(FileName, ImageFormat.Bmp);
            }

            using (var fileStream = File.OpenRead(FileName))
            using (var bmp = (Bitmap) Image.FromStream(fileStream, false, false))
            {
                var actual = (Matrix) bmp;
                actual.Pixels.Should().BeEquivalentTo(matrix.Pixels);
            }
        }
    }
}