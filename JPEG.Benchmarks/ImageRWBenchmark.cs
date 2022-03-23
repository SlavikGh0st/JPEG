using System.Drawing;
using System.IO;
using BenchmarkDotNet.Attributes;

namespace JPEG.Benchmarks
{
    [DisassemblyDiagnoser]
    public class ImageRWBenchmark
    {
        private Bitmap bmp;
        private JPEG.Images.Matrix origMatrix;
        private JPEG.NT.Common.Matrix ntMatrix;

        [GlobalSetup]
        public void Setup()
        {
            using var fileStream = File.OpenRead(@"Images\sample.bmp");
            bmp = (Bitmap) Image.FromStream(fileStream, false, false);

            origMatrix = (JPEG.Images.Matrix) bmp;
            ntMatrix = (JPEG.NT.Common.Matrix) bmp;
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            bmp.Dispose();
        }

        [Benchmark]
        public void OriginalRead()
        {
            var imageMatrix = (JPEG.Images.Matrix) bmp;
        }

        [Benchmark]
        public void StructMatrixRead()
        {
            var imageMatrix = JPEG.NT.Common.Matrix.ToMatrix(bmp);
        }
        
        [Benchmark]
        public void UnsafeStructMatrixRead()
        {
            var imageMatrix = (JPEG.NT.Common.Matrix) bmp;
        }
        
        [Benchmark]
        public void OriginalWrite()
        {
            var resultBmp = (Bitmap) origMatrix;
            resultBmp.Dispose();
        }
        
        [Benchmark]
        public void StructMatrixWrite()
        {
            var resultBmp = JPEG.NT.Common.Matrix.ToBitmap(ntMatrix);
            resultBmp.Dispose();
        }
        
        [Benchmark]
        public void UnsafeStructMatrixWrite()
        {
            var resultBmp = (Bitmap) ntMatrix;
            resultBmp.Dispose();
        }
    }
}