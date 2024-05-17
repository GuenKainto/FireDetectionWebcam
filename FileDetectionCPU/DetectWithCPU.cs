using Compunet.YoloV8;
using Compunet.YoloV8.Metadata;
using Compunet.YoloV8.Plotting;
using Microsoft.ML.OnnxRuntime;
using SixLabors.ImageSharp;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FireDetectionWithCPU
{
    public static class Detection
    {
        private static YoloV8Predictor predictor = YoloV8Predictor.Create(Path.Combine(Environment.CurrentDirectory, "Assets\\model\\best.onnx"));
        public static async Task<Image> PredictAsync(Image image, float? iou, float? confidence)
        {
            YoloV8Configuration config = new YoloV8Configuration();
            config.IoU = iou ?? 0.45f;
            config.Confidence = confidence ?? 0.3f;
            ImageSelector imageSelector = new(image);
            var result = await predictor.DetectAsync(imageSelector, config);
            return await result.PlotImageAsync(image);
        }
    }
}
