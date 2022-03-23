using JPEG.NT.Common;

namespace JPEG.NT.Compressors
{
    public interface ICompressor
    {
        CompressedImage Compress(Matrix matrix);
        Matrix Uncompress(CompressedImage image);
    }
}