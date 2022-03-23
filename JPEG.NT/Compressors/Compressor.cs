using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JPEG.NT.Common;
using JPEG.NT.CompressionCodecs;
using JPEG.NT.FreqTransformers;
using JPEG.NT.Packers;
using JPEG.NT.Quantanaizers;

namespace JPEG.NT.Compressors
{
    public class Compressor : ICompressor
    {
        private readonly IFreqTransformer<double> freqTransformer;
        private readonly IQuantanizer<double, byte> quantanizer;
        private readonly IPacker<byte> packer;

        public int CompressQuality
        {
            get => quantanizer.QualityFactor;
            set => quantanizer.QualityFactor = value;
        }

        public int DCTSize { get; set; }


        public Compressor(IFreqTransformer<double> freqTransformer,
            IQuantanizer<double, byte> quantanizer,
            IPacker<byte> packer)
        {
            DCTSize = 8;

            this.freqTransformer = freqTransformer;
            this.quantanizer = quantanizer;
            this.packer = packer;
        }


        public CompressedImage Compress(Matrix matrix)
        {
            var allQuantizedBytes = new List<byte>();

            for (var j = 0; j < matrix.Height; j += DCTSize)
            for (var i = 0; i < matrix.Width; i += DCTSize)
            {
                var subMatrix = matrix.GetSubMatrix(j, DCTSize, i, DCTSize);
                var componentSelector = new Func<Pixel, double>[] {p => p.Y, p => p.Cb, p => p.Cr};
                foreach (var channel in subMatrix.GetColorChannels(componentSelector,-128))
                {
                    var channelFreqs = freqTransformer.FreqTransform2D(channel);
                    var quantizedBytes = quantanizer.Quantize(channelFreqs);
                    var packedBytes = packer.Pack(quantizedBytes);
                    allQuantizedBytes.AddRange(packedBytes);
                }
            }

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

            using (var allQuantizedBytes =
                new MemoryStream(HuffmanCodec.Decode(image.CompressedBytes, image.DecodeTable, image.BitsCount)))
            {
                for (var j = 0; j < image.Height; j += DCTSize)
                for (var i = 0; i < image.Width; i += DCTSize)
                {
                    var channels = Enumerable.Range(0, 3).Select(x =>
                    {
                        var packedBytes = new byte[DCTSize * DCTSize];
                        allQuantizedBytes.ReadAsync(packedBytes, 0, packedBytes.Length).Wait();
                        var quantizedBytes = packer.Unpack(packedBytes);
                        var channelFreqs = quantanizer.DeQuantize(quantizedBytes);
                        return freqTransformer.IFreqTransform2D(channelFreqs);
                    }).ToArray();

                    var subMatrix = new Matrix(channels[0], channels[1], channels[2], PixelFormat.YCbCr);
                    subMatrix.ShiftMatrixValues(128);
                    result.SetSubMatrix(j, i, subMatrix);
                }
            }

            return result;
        }
    }
}