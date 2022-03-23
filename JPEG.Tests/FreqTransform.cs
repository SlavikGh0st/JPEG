using System.Collections.Generic;
using FluentAssertions;
using JPEG.NT.FreqTransformers;
using NUnit.Framework;

namespace JPEG.Tests
{
    [TestFixture]
    public class FreqTransform
    {
        public static IEnumerable<double[,]> SquareMatrix()
        {
            yield return new double[,]
            {
                {1, 2},
                {3, 4}
            };
            yield return new double[,]
            {
                {100, 200, 150, 1},
                {50, 100, 355, 2},
                {1024, 11, 86, 3},
                {25, 50, 666, 4}
            };
            yield return new double[,]
            {
                {255, 255, 255, 255, 255, 255, 255, 255},
                {255, 255, 255, 255, 255, 255, 255, 255},
                {255, 255, 255, 255, 255, 255, 255, 255},
                {255, 255, 255, 255, 255, 255, 255, 255},
                {255, 255, 255, 255, 255, 255, 255, 255},
                {255, 255, 255, 255, 255, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255}
            };
        }
        
        public static IEnumerable<double[,]> Square8x8Matrix()
        {
            yield return new double[,]
            {
                {255, 255, 255, 255, 255, 255, 255, 255},
                {255, 255, 255, 255, 255, 255, 255, 255},
                {255, 255, 255, 255, 255, 255, 255, 255},
                {255, 255, 255, 255, 255, 255, 255, 255},
                {255, 255, 255, 255, 255, 255, 255, 255},
                {255, 255, 255, 255, 255, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255}
            };
            yield return new double[,]
            {
                {1, 2, 3, 4, 5, 6, 7, 8},
                {2, 4, 6, 8, 10, 12, 14, 16},
                {3, 6, 9, 12, 15, 18, 21, 24},
                {4, 8, 12, 16, 20, 24, 28, 32},
                {5, 10, 15, 20, 25, 30, 35, 40},
                {6, 12, 18, 24, 30, 36, 42, 48},
                {7, 14, 21, 28, 35, 42, 49, 56},
                {8, 16, 24, 32, 40, 48, 56, 64}
            };
            yield return new double[,]
            {
                {100, 2, 54, 80, 29, 31, 22, 11},
                {15, 59, 0, 11, 83, 47, 83, 16},
                {3, 6, 9, 12, 15, 18, 21, 24},
                {4, 1, 12, 286, 20, 24, 28, 32},
                {5, 53, 15, 110, 25, 7, 35, 40},
                {86, 12, 18, 73, 30, 468, 42, 48},
                {2, 14, 21, 28, 51, 42, 49, 3},
                {8, 7, 468, 32, 16, 13, 22, 64}
            };
        }

        [OneTimeSetUp]
        public void SetUp()
        {
        }

        [TestCaseSource(nameof(SquareMatrix))]
        public void OriginalDCT2D_ShouldWorksCorrect(double[,] matrix)
        {
            var dct = JPEG.DCT.DCT2D(matrix);
            var idct = new double[matrix.GetLength(0), matrix.GetLength(1)];
            JPEG.DCT.IDCT2D(dct, idct);

            dct.Should().NotBeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
            idct.Should().BeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
        }

        [TestCaseSource(nameof(SquareMatrix))]
        public void DCT2D_ShouldWorksCorrect(double[,] matrix)
        {
            var freqTransformer = new DctTransformer();
            
            var dct = freqTransformer.FreqTransform2D(matrix);
            var idct = freqTransformer.IFreqTransform2D(dct);

            dct.Should().NotBeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
            idct.Should().BeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
        }
        
        [TestCaseSource(nameof(Square8x8Matrix))]
        public void DCT2D8x8_ShouldWorksCorrect(double[,] matrix)
        {
            var freqTransformer = new Dct8X8Transformer();
            
            var dct = freqTransformer.FreqTransform2D(matrix);
            var idct = freqTransformer.IFreqTransform2D(dct);

            dct.Should().NotBeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
            idct.Should().BeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
        }

        [TestCaseSource(nameof(SquareMatrix))]
        public void ParallelDCT2D_ShouldWorksCorrect(double[,] matrix)
        {
            var freqTransformer = new ParallelDctTransformer();
            
            var dct = freqTransformer.FreqTransform2D(matrix);
            var idct = freqTransformer.IFreqTransform2D(dct);

            dct.Should().NotBeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
            idct.Should().BeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
        }
        
        [TestCaseSource(nameof(Square8x8Matrix))]
        public void ParallelDCT2D8x8_ShouldWorksCorrect(double[,] matrix)
        {
            var freqTransformer = new ParallelDct8X8Transformer();
            
            var dct = freqTransformer.FreqTransform2D(matrix);
            var idct = freqTransformer.IFreqTransform2D(dct);

            dct.Should().NotBeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
            idct.Should().BeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
        }

        [TestCaseSource(nameof(SquareMatrix))]
        public void FFT_ShouldWorksCorrect(double[,] matrix)
        {
            var freqTransformer = new FftTransformer();
            
            var fft = freqTransformer.FreqTransform2D(matrix);
            var ifft = freqTransformer.IFreqTransform2D(fft);

            fft.Should().NotBeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
            ifft.Should().BeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
        }
        
        [TestCaseSource(nameof(Square8x8Matrix))]
        public void FFT8x8_ShouldWorksCorrect(double[,] matrix)
        {
            var freqTransformer = new Fft8X8Transformer();
            
            var fft = freqTransformer.FreqTransform2D(matrix);
            var ifft = freqTransformer.IFreqTransform2D(fft);

            fft.Should().NotBeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
            ifft.Should().BeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
        }
        
        [TestCaseSource(nameof(Square8x8Matrix))]
        public void RFFT8x8_ShouldWorksCorrect(double[,] matrix)
        {
            var freqTransformer = new Rfft8X8Transformer();
            
            var fft = freqTransformer.FreqTransform2D(matrix);
            var ifft = freqTransformer.IFreqTransform2D(fft);

            fft.Should().NotBeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
            ifft.Should().BeEquivalentTo(matrix, options =>
                options.Using<double>(ctx =>
                        ctx.Subject.Should().BeApproximately(ctx.Expectation, 1E-12))
                    .WhenTypeIs<double>());
        }
    }
}