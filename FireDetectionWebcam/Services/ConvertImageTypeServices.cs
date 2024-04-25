using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace FireDetectionWebcam.Services
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal static class ConvertImageTypeServices
    {
        public static SixLabors.ImageSharp.Image<Rgb24> ConvertToImageSharpImage(System.Drawing.Image systemDrawingImage)
        {
            using MemoryStream memoryStream = new MemoryStream();
            systemDrawingImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return SixLabors.ImageSharp.Image.Load<Rgb24>(memoryStream);
        }

        public static System.Drawing.Image ConvertToSystemDrawingImage(SixLabors.ImageSharp.Image imageSharpImage)
        {
            using MemoryStream memoryStream = new MemoryStream();
            imageSharpImage.Save(memoryStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
            memoryStream.Seek(0, SeekOrigin.Begin);
            return System.Drawing.Image.FromStream(memoryStream);
        }
    }
}
