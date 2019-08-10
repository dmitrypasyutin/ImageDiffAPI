using System.Collections.Generic;
using System.Drawing;
using Microsoft.AspNetCore.Http;

namespace ImageDiff.Api.Infrastructure.Validators
{
    public interface IRequestValidator
    {
        IList<string> ValidateFiles(IFormFile[] files);

        IList<string> ValidateImages(Bitmap[] images);
    }
}