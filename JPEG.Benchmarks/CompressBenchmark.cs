using System.Drawing;
using System.IO;
using BenchmarkDotNet.Attributes;
using JPEG.NT.Compressors;
using JPEG.NT.FreqTransformers;
using JPEG.NT.Packers;
using JPEG.NT.Quantanaizers;

namespace JPEG.Benchmarks
{
    [DisassemblyDiagnoser]
    public class CompressBenchmark
    {
        private JPEG.Images.Matrix origMatrix;
        private JPEG.NT.Common.Matrix ntMatrix;
        private JPEG.CompressedImage origCompressedImage;
        private JPEG.NT.Common.CompressedImage ntCompressedImage;
        private JPEG.NT.Common.CompressedImage rfftCompressedImage;

        private Compressor compressorDct8X8;
        private Compressor compressorPdct8X8;
        private Compressor compressorRfft8X8;
        private ParallelCompressor pcompressorDct8X8;
        private ParallelCompressor pcompressorPdct8X8;
        private ParallelCompressor pcompressorRfft8X8;
        
        [GlobalSetup]
        public void Setup()
        {
            compressorDct8X8 = new Compressor(new Dct8X8Transformer(), new DoubleQuantanizer(), new ZigZag8x8Packer()) {CompressQuality = 50};
            compressorPdct8X8 = new Compressor(new ParallelDct8X8Transformer(), new DoubleQuantanizer(), new ZigZag8x8Packer()) {CompressQuality = 50};
            compressorRfft8X8 = new Compressor(new Rfft8X8Transformer(), new DoubleQuantanizer(), new ZigZag8x8Packer()) {CompressQuality = 50};
            pcompressorDct8X8 = new ParallelCompressor(new Dct8X8Transformer(), new DoubleQuantanizer(), new ZigZag8x8Packer()) {CompressQuality = 50};
            pcompressorPdct8X8 = new ParallelCompressor(new ParallelDct8X8Transformer(), new DoubleQuantanizer(), new ZigZag8x8Packer()) {CompressQuality = 50};
            pcompressorRfft8X8 = new ParallelCompressor(new Rfft8X8Transformer(), new DoubleQuantanizer(), new ZigZag8x8Packer()) {CompressQuality = 50};
            
            using var fileStream = File.OpenRead(@"Images\sample.bmp");
            using var bmp = (Bitmap) Image.FromStream(fileStream, false, false);

            origMatrix = (JPEG.Images.Matrix) bmp;
            ntMatrix = (JPEG.NT.Common.Matrix) bmp;
            
            origCompressedImage = JPEG.Program.Compress(origMatrix);
            ntCompressedImage = pcompressorPdct8X8.Compress(ntMatrix);
            rfftCompressedImage = pcompressorRfft8X8.Compress(ntMatrix);
        }

        //[Benchmark]
        public void OriginalCompress()
        {
            JPEG.Program.Compress(origMatrix);
        }
        
        [Benchmark]
        public void CompressorWithDCT8x8()
        {
            compressorDct8X8.Compress(ntMatrix);
        }
        
        [Benchmark]
        public void CompressorWithParallelDCT8x8()
        {
            compressorPdct8X8.Compress(ntMatrix);
        }
        
        //[Benchmark]
        public void CompressorWithRFFT8x8()
        {
            compressorRfft8X8.Compress(ntMatrix);
        }
        
        [Benchmark]
        public void ParallelCompressorWithDCT8x8()
        {
            pcompressorDct8X8.Compress(ntMatrix);
        }
        
        [Benchmark]
        public void ParallelCompressorWithParallelDCT8x8()
        {
            pcompressorPdct8X8.Compress(ntMatrix);
        }
        
        //[Benchmark]
        public void ParallelCompressorWithRFFT8x8()
        {
            pcompressorRfft8X8.Compress(ntMatrix);
        }
        
        //[Benchmark]
        public void OriginalUncompress()
        {
            JPEG.Program.Uncompress(origCompressedImage);
        }
        
        [Benchmark]
        public void UncompressorWithDCT8x8()
        {
            compressorDct8X8.Uncompress(ntCompressedImage);
        }
        
        [Benchmark]
        public void UncompressorWithParallelDCT8x8()
        {
            compressorPdct8X8.Uncompress(ntCompressedImage);
        }
        
        //[Benchmark]
        public void UncompressorWithRFFT8x8()
        {
            compressorRfft8X8.Uncompress(ntCompressedImage);
        }
        
        [Benchmark]
        public void ParallelUncompressorWithDCT8x8()
        {
            pcompressorDct8X8.Uncompress(ntCompressedImage);
        }
        
        [Benchmark]
        public void ParallelUncompressorWithParallelDCT8x8()
        {
            pcompressorPdct8X8.Uncompress(ntCompressedImage);
        }
        
        //[Benchmark]
        public void ParallelUncompressorWithRFFT8x8()
        {
            pcompressorRfft8X8.Uncompress(rfftCompressedImage);
        }
    }
}