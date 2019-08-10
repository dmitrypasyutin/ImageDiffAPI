using System;

namespace ImageDiff.Data
{
    public class ResultImage
    {
        public ResultImage(int imageId, byte[] imageData, int percentsProcessed, string contentType, int imageVersion)
        {
            ImageId = imageId;
            ImageData = imageData ?? throw new ArgumentException(nameof(imageData));
            PercentsProcessed = percentsProcessed;
            ContentType = contentType ?? throw new ArgumentException(nameof(contentType));
            ImageVersion = imageVersion;
        }
        public int ImageId { get; private set; }
        public byte[] ImageData { get; private set; }
        public int PercentsProcessed { get; set; }
        public string ContentType { get; private set; }
        
        public int ImageVersion { get; set; }
    }
}