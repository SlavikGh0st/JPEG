namespace JPEG.NT.FreqTransformers
{
    public interface IFreqTransformer<T>
    {
        T[,] FreqTransform2D(float[,] input);
        float[,] IFreqTransform2D(T[,] dct);
    }
}