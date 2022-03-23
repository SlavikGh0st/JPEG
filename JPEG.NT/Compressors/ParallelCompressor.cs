using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JPEG.NT.Common;
using JPEG.NT.CompressionCodecs;
using JPEG.NT.FreqTransformers;
using JPEG.NT.Packers;
using JPEG.NT.Quantanaizers;
using JPEG.NT.Utilities;

namespace JPEG.NT.Compressors
{
    public class ParallelCompressor : ICompressor
    {
        private readonly IFreqTransformer<float> freqTransformer;
        private readonly IQuantanizer<float, byte> quantanizer;
        private readonly IPacker<byte> packer;

        public int CompressQuality
        {
            get => quantanizer.QualityFactor;
            set => quantanizer.QualityFactor = value;
        }

        public int DCTSize { get; set; }


        public ParallelCompressor(IFreqTransformer<float> freqTransformer,
            IQuantanizer<float, byte> quantanizer,
            IPacker<byte> packer)
        {
            DCTSize = 8;

            this.freqTransformer = freqTransformer;
            this.quantanizer = quantanizer;
            this.packer = packer;
        }


        public CompressedImage Compress(Matrix matrix)
        {
            var allQuantizedBytes = new byte[matrix.Height * matrix.Width * 3];

            Parallel.For(0, matrix.Height / DCTSize, j =>
            {
                for (var i = 0; i < matrix.Width; i += DCTSize)
                {
                    var subMatrix = matrix.GetSubMatrix(j * DCTSize, DCTSize, i, DCTSize);
                    var componentSelector = new Func<Pixel, float>[] {p => p.Y, p => p.Cb, p => p.Cr};
                    
                    var ch = 0;
                    foreach (var channel in subMatrix.GetColorChannels(componentSelector, -128))
                    {
                        var channelFreqs = freqTransformer.FreqTransform2D(channel);
                        var quantizedBytes = quantanizer.Quantize(channelFreqs);
                        var packedBytes = packer.Pack(quantizedBytes).ToArray();

                        SetSubVector(ref allQuantizedBytes, packedBytes,
                            DCTSize * (3 * j * matrix.Width + 3 * i + ch * DCTSize),
                            DCTSize * DCTSize);
                        ch++;
                    }
                }
            });


            long bitsCount;
            Dictionary<BitsWithLength, byte> decodeTable;
            var compressedBytes = HuffmanCodec.Encode(allQuantizedBytes, out decodeTable, out bitsCount);

            return new CompressedImage
            {
                Quality = CompressQuality, CompressedBytes = compressedBytes, BitsCount = bitsCount,
                DecodeTable = decodeTable,
                Height = matrix.Height, Width = matrix.Width
            };
        }

        public Matrix Uncompress(CompressedImage image)
        {
            var result = new Matrix(image.Height, image.Width);

            var compressedBytes = HuffmanCodec.Decode(image.CompressedBytes, image.DecodeTable, image.BitsCount);

            Parallel.For(0, image.Height / DCTSize, j =>
            {
                for (var i = 0; i < image.Width; i += DCTSize)
                {
                    var channels = Enumerable.Range(0, 3).Select(x =>
                    {
                        var packedBytes = GetSubVector(compressedBytes,
                            DCTSize * (3 * j * image.Width + 3 * i + x * DCTSize), DCTSize * DCTSize);
                        var quantizedBytes = packer.Unpack(packedBytes);
                        var channelFreqs = quantanizer.DeQuantize(quantizedBytes);
                        return freqTransformer.IFreqTransform2D(channelFreqs);
                    }).ToArray();

                    var subMatrix = new Matrix(channels[0], channels[1], channels[2], PixelFormat.YCbCr);
                    subMatrix.ShiftMatrixValues(128);
                    result.SetSubMatrix(j * DCTSize, i, subMatrix);
                }
            });

            return result;
        }

        private byte[] GetSubVector(byte[] input, int from, int count)
        {
            var result = new byte[count];
            for (var i = 0; i < count; i++)
                result[i] = input[from + i];

            return result;
        }

        private void SetSubVector(ref byte[] parent, byte[] input, int from, int count)
        {
            for (var i = 0; i < count; i++)
                parent[from + i] = input[i];
        }
    }
}