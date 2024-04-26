using System.Drawing;
using System.Threading.Tasks;

namespace FireDetectionWebcam.Services
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal static class PredictServices
    {
        public static async Task<Bitmap> PredictAsyncWithCPU(Bitmap _lastFrame)
        {
            var image = ConvertImageTypeServices.ConvertToImageSharpImage(systemDrawingImage: _lastFrame);
            var imagePotted = await FireDetectionWithCPU.Detection.PredictAsync(image);
            return (Bitmap)ConvertImageTypeServices.ConvertToSystemDrawingImage(imagePotted);
        }
        public static async Task<Bitmap> PredictAsyncWithGPU(Bitmap _lastFrame)
        {
            var image = ConvertImageTypeServices.ConvertToImageSharpImage(systemDrawingImage: _lastFrame);
            var imagePotted = await FireDetectionWithGPU.Detection.PredictAsync(image);
            return (Bitmap)ConvertImageTypeServices.ConvertToSystemDrawingImage(imagePotted);
        }
    }
}
