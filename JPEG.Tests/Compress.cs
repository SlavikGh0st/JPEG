using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using FluentAssertions;
using JPEG.NT.Common;
using JPEG.NT.Compressors;
using JPEG.NT.FreqTransformers;
using JPEG.NT.Packers;
using JPEG.NT.Quantanaizers;
using NUnit.Framework;
using PixelFormat = JPEG.NT.Common.PixelFormat;

namespace JPEG.Tests
{
    [TestFixture]
    public class Compress
    {
        private readonly string testFileName = @"Images\earth.bmp";
        private readonly string compressedFileName = "compressed";
        private Matrix matrix;
        private JPEG.Images.Matrix origMatrix;

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

            origMatrix = new JPEG.Images.Matrix(height, width);
            for (var j = 0; j < height; j++)
            for (var i = 0; i < width; i++)
            {
                var component = 255 / (i + j + 1);
                origMatrix.Pixels[j, i] =
                    new JPEG.Images.Pixel(component, component, component, Images.PixelFormat.RGB);
            }
        }

        [Test]
        public void CompressorWithDCT8x8_ShouldWorksCorrectly()
        {
            var compressor = new Compressor(
                    new Dct8X8Transformer(),
                    new DoubleQuantanizer(),
                    new ZigZag8x8Packer())
                {CompressQuality = 50};

            Action action = () => { CompressorTest(compressor, "defaultCompressDCT8x8"); };

            action.Should().NotThrow();
        }

        [Test]
        public void CompressorWithParallelDCT8x8_ShouldWorksCorrectly()
        {
            var compressor = new Compressor(
                    new ParallelDct8X8Transformer(),
                    new DoubleQuantanizer(),
                    new ZigZag8x8Packer())
                {CompressQuality = 50};

            Action action = () => { CompressorTest(compressor, "defaultCompressParallelDCT8x8"); };

            action.Should().NotThrow();
        }
        
        [Test]
        public void ParallelCompressorWithDCT8x8_ShouldWorksCorrectly()
        {
            var compressor = new ParallelCompressor(
                    new Dct8X8Transformer(),
                    new DoubleQuantanizer(),
                    new ZigZag8x8Packer())
                {CompressQuality = 50};

            Action action = () => { CompressorTest(compressor, "parallelCompressDCT8x8"); };

            action.Should().NotThrow();
        }
        
        [Test]
        public void ParallelCompressorWithParallelDCT8x8_ShouldWorksCorrectly()
        {
            var compressor = new ParallelCompressor(
                    new ParallelDct8X8Transformer(),
                    new DoubleQuantanizer(),
                    new ZigZag8x8Packer())
                {CompressQuality = 50};

            Action action = () => { CompressorTest(compressor, "parallelCompressParallelDCT8x8"); };

            action.Should().NotThrow();
        }
        

        private void CompressorTest(ICompressor compressor, string outputFileName)
        {
            var sw = new Stopwatch();

            sw.Start();
            using var fileStream = File.OpenRead(testFileName);
            using var bmp = (Bitmap) Image.FromStream(fileStream, false, false);
            var imageMatrix = (Matrix) bmp;

            sw.Stop();
            Console.WriteLine($"{bmp.Width}x{bmp.Height} - {fileStream.Length / (1024.0 * 1024):F2} MB");
            sw.Start();

            var compressedImg = compressor.Compress(imageMatrix);
            compressedImg.Save(compressedFileName);

            sw.Stop();
            Console.WriteLine($"Compression: {sw.Elapsed}");
            sw.Restart();

            var compressedImage = JPEG.NT.Common.CompressedImage.Load(compressedFileName);
            var uncompressedImage = compressor.Uncompress(compressedImage);
            var resultBmp = (Bitmap) uncompressedImage;
            resultBmp.Save(outputFileName + ".bmp", ImageFormat.Bmp);
            sw.Stop();

            Console.WriteLine($"Decompression: {sw.Elapsed}");
            Console.WriteLine($"Peak commit size: {MemoryMeter.PeakPrivateBytes() / (1024.0 * 1024):F2} MB");
            Console.WriteLine($"Peak working set: {MemoryMeter.PeakWorkingSet() / (1024.0 * 1024):F2} MB");
            Console.WriteLine(
                $@"Decompressed image path: {TestContext.CurrentContext.TestDirectory}\{outputFileName}.bmp");
        }
    }
}