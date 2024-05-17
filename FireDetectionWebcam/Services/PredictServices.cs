using System.Drawing;
using System.Threading.Tasks;

namespace FireDetectionWebcam.Services
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal static class PredictServices
    {
        public static async Task<Bitmap> PredictAsyncWithCPU(Bitmap _lastFrame,float iou, float confidence)
        {
            var image = ConvertImageTypeServices.ConvertToImageSharpImage(systemDrawingImage: _lastFrame);
            var imagePotted = await FireDetectionWithCPU.Detection.PredictAsync(image,iou,confidence);
            return (Bitmap)ConvertImageTypeServices.ConvertToSystemDrawingImage(imagePotted);
        }
    }
}
