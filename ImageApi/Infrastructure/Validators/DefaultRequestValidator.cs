using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ImageDiff.Api.Infrastructure.Validators
{
    public class DefaultRequestValidator : IRequestValidator
    {
        public IList<string> ValidateFiles(IFormFile[] files)
        {
            List<string> errors = new List<string>();

            if (files is null || files.Length != 2)
            {
                errors.Add("There must be exactly 2 images in request");
            }
            else if (files.Any(f => f.Length == 0))
            {
                errors.Add("Incorrect image content");
            }

            return errors;
        }

        public IList<string> ValidateImages(Bitmap[] images)
        {
            if(images is null) throw new ArgumentException(nameof(images));
            
            List<string> errors = new List<string>();

            if ((images.Select(f => f.Height).Distinct().Count() > 1)
                || (images.Select(f => f.Width).Distinct().Count() > 1))
            {
                errors.Add("Image heights and widths must be the same");
            }
            

            return errors;
        }
    }
}