using Compunet.YoloV8;
using Compunet.YoloV8.Plotting;
using SixLabors.ImageSharp;

namespace FireDetectionWithCPU
{
    public static class Detection
    {
        private static YoloV8Predictor predictor = YoloV8Predictor.Create(Path.Combine(Environment.CurrentDirectory, "Assets\\model\\best.onnx"));
        public static async Task<Image> PredictAsync(Image image)
        {
            ImageSelector imageSelector = new(image);
            var result = await predictor.DetectAsync(imageSelector);
            return await result.PlotImageAsync(image);
        }
    }
}
