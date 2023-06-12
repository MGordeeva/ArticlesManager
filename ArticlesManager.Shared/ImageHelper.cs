using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;

namespace ArticlesManager.Shared
{
    public static class ImageHelper
    {
        public static byte[] ReduceImageSize(byte[] imageBytes, int jpegQuality = 50)
        {
            using (var inputStream = new MemoryStream(imageBytes))
            {
                var image = Image.FromStream(inputStream);
                var jpegEncoder = ImageCodecInfo.GetImageDecoders()
                    .First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, (int)jpegQuality);
                using (var outputStream = new MemoryStream())
                {
                    image.Save(outputStream, jpegEncoder, encoderParameters);
                    return outputStream.ToArray();
                }
            }
        }
    }
}
