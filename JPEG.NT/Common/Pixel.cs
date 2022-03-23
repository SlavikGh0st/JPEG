namespace JPEG.NT.Common
{
    public struct Pixel
    {
        public double FirstComponent { get; set; }
        public double SecondComponent { get; set; }
        public double ThirdComponent { get; set; }
        
        public PixelFormat Format { get; }

        public double R => Format == PixelFormat.Rgb ? FirstComponent : (298.082 * FirstComponent + 408.583 * ThirdComponent) / 256.0 - 222.921;
        public double G => Format == PixelFormat.Rgb ? SecondComponent : (298.082 * FirstComponent - 100.291 * SecondComponent - 208.120 * ThirdComponent) / 256.0 + 135.576;
        public double B => Format == PixelFormat.Rgb ? ThirdComponent : (298.082 * FirstComponent + 516.412 * SecondComponent) / 256.0 - 276.836;

        public double Y => Format == PixelFormat.YCbCr ? FirstComponent : 16.0 + (65.738 * FirstComponent + 129.057 * SecondComponent + 24.064 * ThirdComponent) / 256.0;
        public double Cb => Format == PixelFormat.YCbCr ? SecondComponent : 128.0 + (-37.945 * FirstComponent - 74.494 * SecondComponent + 112.439 * ThirdComponent) / 256.0;
        public double Cr => Format == PixelFormat.YCbCr ? ThirdComponent : 128.0 + (112.439 * FirstComponent - 94.154 * SecondComponent - 18.285 * ThirdComponent) / 256.0;
        
        public Pixel(double firstComponent, double secondComponent, double thirdComponent, PixelFormat format)
        {
            FirstComponent = firstComponent;
            SecondComponent = secondComponent;
            ThirdComponent = thirdComponent;
            Format = format;
        }
    }
}