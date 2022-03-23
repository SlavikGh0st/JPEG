namespace JPEG.NT.Common
{
    public struct Pixel
    {
        public float FirstComponent { get; set; }
        public float SecondComponent { get; set; }
        public float ThirdComponent { get; set; }
        
        public PixelFormat Format { get; }

        public float R => Format == PixelFormat.Rgb ? FirstComponent : (298.082f * FirstComponent + 408.583f * ThirdComponent) / 256.0f - 222.921f;
        public float G => Format == PixelFormat.Rgb ? SecondComponent : (298.082f * FirstComponent - 100.291f * SecondComponent - 208.120f * ThirdComponent) / 256.0f + 135.576f;
        public float B => Format == PixelFormat.Rgb ? ThirdComponent : (298.082f * FirstComponent + 516.412f * SecondComponent) / 256.0f - 276.836f;

        public float Y => Format == PixelFormat.YCbCr ? FirstComponent : 16.0f + (65.738f * FirstComponent + 129.057f * SecondComponent + 24.064f * ThirdComponent) / 256.0f;
        public float Cb => Format == PixelFormat.YCbCr ? SecondComponent : 128.0f + (-37.945f * FirstComponent - 74.494f * SecondComponent + 112.439f * ThirdComponent) / 256.0f;
        public float Cr => Format == PixelFormat.YCbCr ? ThirdComponent : 128.0f + (112.439f * FirstComponent - 94.154f * SecondComponent - 18.285f * ThirdComponent) / 256.0f;
        
        public Pixel(float firstComponent, float secondComponent, float thirdComponent, PixelFormat format)
        {
            FirstComponent = firstComponent;
            SecondComponent = secondComponent;
            ThirdComponent = thirdComponent;
            Format = format;
        }
    }
}