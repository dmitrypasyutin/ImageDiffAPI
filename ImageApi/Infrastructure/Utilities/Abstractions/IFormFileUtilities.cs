using System.Drawing;
using Microsoft.AspNetCore.Http;

namespace ImageDiff.Api.Infrastructure.Utilities.Abstractions
{
    public interface IFormFileUtilities
    {
        Bitmap[] ConvertFilesToImages(IFormFile[] files);

        byte[] SerializeFile(IFormFile file);
    }
}