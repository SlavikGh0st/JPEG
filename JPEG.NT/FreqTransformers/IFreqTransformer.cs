namespace JPEG.NT.FreqTransformers
{
    public interface IFreqTransformer<T>
    {
        T[,] FreqTransform2D(double[,] input);
        double[,] IFreqTransform2D(T[,] dct);
    }
}