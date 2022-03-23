using System.Numerics;
using BenchmarkDotNet.Attributes;
using JPEG.NT.FreqTransformers;

namespace JPEG.Benchmarks
{
    [DisassemblyDiagnoser]
    public class FreqTransformBenchmark
    {
        private DctTransformer dctTransformer;
        private Dct8X8Transformer dct8X8Transformer;
        private ParallelDctTransformer parallelDctTransformer;
        private ParallelDct8X8Transformer parallelDct8X8Transformer;
        private FftTransformer fftTransformer;
        private Fft8X8Transformer fft8X8Transformer;
        private Rfft8X8Transformer rfft8X8Transformer;
        
        private float[,] fmatrix;
        private double[,] dmatrix;
        private float[,] fdct;
        private double[,] ddct;
        private Complex[,] fft;
        private float[,] rfft;

        [GlobalSetup]
        public void Setup()
        {
            dctTransformer = new DctTransformer();
            dct8X8Transformer= new Dct8X8Transformer();
            parallelDctTransformer = new ParallelDctTransformer();
            parallelDct8X8Transformer = new ParallelDct8X8Transformer();
            fftTransformer = new FftTransformer();
            fft8X8Transformer = new Fft8X8Transformer();
            rfft8X8Transformer = new Rfft8X8Transformer();
            
            fmatrix = new float[,]
            {
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255}
            };
            dmatrix = new double[,]
            {
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255},
                {255, 255, 255, 255, 225, 255, 255, 255}
            };
            fdct = parallelDctTransformer.FreqTransform2D(fmatrix);
            ddct = JPEG.DCT.DCT2D(dmatrix);
            fft = fftTransformer.FreqTransform2D(fmatrix);
            rfft = rfft8X8Transformer.FreqTransform2D(fmatrix);
        }

        //[Benchmark]
        public void OriginalDCT()
        {
            JPEG.DCT.DCT2D(dmatrix);
        }

        //[Benchmark]
        public void DCT()
        {
            dctTransformer.FreqTransform2D(fmatrix);
        }
        
        [Benchmark]
        public void DCT8x8()
        {
            dct8X8Transformer.FreqTransform2D(fmatrix);
        }

        //[Benchmark]
        public void ParallelDCT()
        {
            parallelDctTransformer.FreqTransform2D(fmatrix);
        }
        
        [Benchmark]
        public void ParallelDCT8x8()
        {
            parallelDct8X8Transformer.FreqTransform2D(fmatrix);
        }

        [Benchmark]
        public void FFT()
        {
            fftTransformer.FreqTransform2D(fmatrix);
        }
        
        [Benchmark]
        public void FFT8x8()
        {
            fft8X8Transformer.FreqTransform2D(fmatrix);
        }
        
        [Benchmark]
        public void RFFT8x8()
        {
            rfft8X8Transformer.FreqTransform2D(fmatrix);
        }
        
        //[Benchmark]
        public void OriginalIDCT()
        {
            var temp = new double[ddct.GetLength(0), ddct.GetLength(1)];
            JPEG.DCT.IDCT2D(ddct, temp);
        }
        
        //[Benchmark]
        public void IDCT()
        {
            dctTransformer.IFreqTransform2D(fdct);
        }
        
        [Benchmark]
        public void IDCT8x8()
        {
            dct8X8Transformer.IFreqTransform2D(fdct);
        }
        
        //[Benchmark]
        public void ParallelIDCT()
        {
            parallelDctTransformer.IFreqTransform2D(fdct);
        }
        
        [Benchmark]
        public void ParallelIDCT8x8()
        {
            parallelDct8X8Transformer.IFreqTransform2D(fdct);
        }
        
        [Benchmark]
        public void IFFT()
        {
            fftTransformer.IFreqTransform2D(fft);
        }
        
        [Benchmark]
        public void IFFT8x8()
        {
            fft8X8Transformer.IFreqTransform2D(fft);
        }
        
        [Benchmark]
        public void IRFFT8x8()
        {
            rfft8X8Transformer.IFreqTransform2D(rfft);
        }
    }
}