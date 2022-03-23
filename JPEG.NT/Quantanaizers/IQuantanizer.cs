namespace JPEG.NT.Quantanaizers
{
    public interface IQuantanizer<TI, TQ>
    {
        int QualityFactor { get; set; }
        
        TQ[,] Quantize(TI[,] input);
        TI[,] DeQuantize(TQ[,] input);
    }
}