using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ImageDiff.Api.Infrastructure.Utilities.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ImageDiff.Api.Infrastructure.Utilities
{
    public class DefaultFormFileUtilities : IFormFileUtilities
    {
        public Bitmap[] ConvertFilesToImages(IFormFile[] files)
        {
            if(files is null) throw new ArgumentException(nameof(files));
            
            List<Bitmap> result = new List<Bitmap>();
            foreach (var file in files)
            {
                using (var stream = file.OpenReadStream())
                {
                    result.Add(Image.FromStream(stream) as Bitmap);
                }
            }

            return result.ToArray();
        }

        public byte[] SerializeFile(IFormFile file)
        {
            if(file is null) throw new ArgumentException(nameof(file));
            
            using (var stream = file.OpenReadStream())
            {
                var resultStream = new MemoryStream();
                stream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}