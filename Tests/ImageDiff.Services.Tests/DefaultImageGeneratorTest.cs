using System.Drawing;
using System.IO;
using ImageDiff.Services.ImageGenerators;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ImageDiff.Services.Tests
{
    public class DefaultImageGeneratorTest
    {
        
        [Test]
        public void DrawRectangles_Draws_On_Image()
        {
            var originalImage = CreateBitmap("image2.bmp");
            var originalImageData = ImageToByteArray(originalImage);

            var generator = new DefaultImageGenerator();
            var resultImageData = generator.DrawRectangles(originalImageData, "image/bmp", new Data.Rectangle(
                new Data.ImagePixel(1, 1), new Data.ImagePixel(3, 3)));

            Assert.IsFalse(AreImagesEqual(originalImageData, resultImageData));
        }

        private bool AreImagesEqual(byte[] originalImageData, byte[] resultImageData)
        {
            for (int i = 0; i < originalImageData.Length; i++)
            {
                if (originalImageData[i] != resultImageData[i])
                    return false;
            }

            return true;
        }

        private Bitmap CreateBitmap(string fileName)
        {
            return Image.FromFile(fileName) as Bitmap;
        }

        private byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
    }
}