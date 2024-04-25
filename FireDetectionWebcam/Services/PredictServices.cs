using Compunet.YoloV8;
using Compunet.YoloV8.Plotting;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace FireDetectionWebcam.Services
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal static class PredictServices
    {
        private static YoloV8Predictor predictor = YoloV8Predictor.Create(Path.Combine(Environment.CurrentDirectory, "Assets\\model\\best.onnx"));
        public static async Task<Bitmap> PredictAsync(Bitmap _lastFrame)
        {
            var image = ConvertImageTypeServices.ConvertToImageSharpImage(systemDrawingImage: _lastFrame);
            ImageSelector imageSelector = new(image);
            var result = predictor.Detect(imageSelector);
            var plotted = await result.PlotImageAsync(image);
            return (Bitmap)ConvertImageTypeServices.ConvertToSystemDrawingImage(imageSharpImage: plotted);
        }
    }
}
