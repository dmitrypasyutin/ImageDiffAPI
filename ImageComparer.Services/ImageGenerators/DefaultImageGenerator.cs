using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using ImageDiff.CommonAbstractions;
using ImageDiff.Services.Converters;
using Rectangle = ImageDiff.Data.Rectangle;

namespace ImageDiff.Services.ImageGenerators
{
    public class DefaultImageGenerator : IImageGenerator
    {
        public byte[] DrawRectangles(byte[] imageData, string contentType, params Rectangle[] rectangles)
        {
            if(imageData is null) throw new ArgumentException(nameof(imageData));
            
            using (var originalStream = new MemoryStream(imageData))
            {
                var resultStream = new MemoryStream();
                var img = Image.FromStream(originalStream);
                {
                    using (var g = Graphics.FromImage(img))
                    {
                        using (Pen pen = new Pen(Color.Red))
                        {
                            var rectf = rectangles.Select(f => f.ToRectangleF()).ToArray();
                            g.DrawRectangles(pen, rectf);
                        }
                        g.Flush();
                    }
                    img.Save(resultStream, GetImageFormat(contentType));
                }
                return resultStream.ToArray();
            }
        }

        private ImageFormat GetImageFormat(string contentType)
        {
            switch (contentType)
            {
                case "image/bmp":
                    return ImageFormat.Bmp;
                case "image/jpg":
                case "image/jpeg":
                    return ImageFormat.Jpeg;
                default:
                    return ImageFormat.Png; //more image types could be added
            }
        }
    }
}