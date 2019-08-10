using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ImageDiff.Api.Infrastructure.Response;
using ImageDiff.Api.Infrastructure.Utilities.Abstractions;
using ImageDiff.Api.Infrastructure.Validators;
using ImageDiff.CommonAbstractions;
using ImageDiff.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rectangle = ImageDiff.Data.Rectangle;

namespace ImageDiff.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IDiffObjectsFinder _objectsFinder;
        private readonly IResultImageStorage<int> _imageStorage;
        private readonly IRequestValidator _requestValidator;
        private readonly IImageGenerator _imageGenerator;
        private readonly IFormFileUtilities _formFileUtilities;
        private readonly Random _imageIdGenerator = new Random();

        public ApiController(IDiffObjectsFinder objectsFinder, IResultImageStorage<int> imageStorage, IRequestValidator requestValidator,
            IImageGenerator imageGenerator, IFormFileUtilities formFileUtilities)
        {
            _objectsFinder = objectsFinder ?? throw new ArgumentException(nameof(objectsFinder));
            _imageStorage = imageStorage ?? throw new ArgumentException(nameof(imageStorage));;
            _requestValidator = requestValidator ?? throw new ArgumentException(nameof(requestValidator));;
            _imageGenerator = imageGenerator ?? throw new ArgumentException(nameof(imageGenerator));;
            _formFileUtilities = formFileUtilities ?? throw new ArgumentException(nameof(formFileUtilities));;
        }

        [HttpGet("image/{imageId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.None)]
        public IActionResult GetImage(int imageId)
        {
            try
            {
                var image = _imageStorage.Get(imageId);
                if (image == null)
                {
                    return NotFound();
                }

                return File(image.ImageData, image.ContentType);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("imageprogress/{imageId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<ImageProgressResponse> GetImageProgress(int imageId)
        {
            ImageProgressResponse response = new ImageProgressResponse();

            try
            {
                var resultImage = _imageStorage.Get(imageId);
                if (resultImage == null)
                {
                    response.HttpStatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    response.ImageId = resultImage.ImageId;
                    response.Percent = resultImage.PercentsProcessed;
                    response.ImageVersion = resultImage.ImageVersion;
                }
            }
            catch (Exception)
            {
                response = CreateInternalServerErrorResponse(response);
            }

            return response.ToHttpResponse();
        }

        [HttpPost("comparesync")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult<ImageProgressResponse> CompareImagesSync(IFormFile[] files)
        {
            ImageProgressResponse response = new ImageProgressResponse();

            try
            {
                var requestValidationErrors = _requestValidator.ValidateFiles(files);
                if (requestValidationErrors.Count > 0)
                    return CreateBadRequestResponse(response, requestValidationErrors).ToHttpResponse();
                
                var originalImages = _formFileUtilities.ConvertFilesToImages(files);

                requestValidationErrors = _requestValidator.ValidateImages(originalImages);
                if (requestValidationErrors.Count > 0)
                    return CreateBadRequestResponse(response, requestValidationErrors).ToHttpResponse();

                var foundDiffObjects = _objectsFinder.FindAllDiffObjects(originalImages[0], originalImages[1]).ToArray();
                var resultImageData = _imageGenerator.DrawRectangles(_formFileUtilities.SerializeFile(files[0]), files[0].ContentType, foundDiffObjects);

                int newImageId = GenerateImageId();

                SaveResutlImageToStorage(new ResultImage(newImageId, resultImageData, 100, files[0].ContentType, 1));
                
                response.ImageId = newImageId;
                response.Percent = 100;
            }
            catch (Exception)
            {
                response = CreateInternalServerErrorResponse(response);
            }
            return response.ToHttpResponse();
        }


        [HttpPost("compareasync")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult<ImageProgressResponse> CompareImagesAsync(IFormFile[] files)
        {
            ImageProgressResponse response = new ImageProgressResponse();

            try
            {
                var requestValidationErrors = _requestValidator.ValidateFiles(files);
                if (requestValidationErrors.Count > 0)
                    return CreateBadRequestResponse(response, requestValidationErrors).ToHttpResponse();

                var originalImages = _formFileUtilities.ConvertFilesToImages(files);

                requestValidationErrors = _requestValidator.ValidateImages(originalImages);
                if (requestValidationErrors.Count > 0)
                    return CreateBadRequestResponse(response, requestValidationErrors).ToHttpResponse();

                int newImageId = GenerateImageId();

                SaveResutlImageToStorage(new ResultImage(newImageId, _formFileUtilities.SerializeFile(files[0]), 0,
                    files[0].ContentType, 1));

                response.ImageId = newImageId;

                FindObjectsAsync(originalImages, newImageId);
            }
            catch (Exception)
            {
                response = CreateInternalServerErrorResponse(response);
            }
            return response.ToHttpResponse();
        }
        
        private async Task FindObjectsAsync(Bitmap[] images, int imageId)
        {
            if(images == null) throw new ArgumentException(nameof(images));
            
            foreach (var foundDiffObject in _objectsFinder.FindAllDiffObjects(images[0], images[1]))
            {
                await Task.Delay(3000); //artifical delay to imitate a time-consuming task
                var cachedResultImage = _imageStorage.Get(imageId);
                var updatedResultImage = _imageGenerator.DrawRectangles(cachedResultImage.ImageData, cachedResultImage.ContentType, foundDiffObject);
                int percent = CalculatePercentProcessed(images[0], foundDiffObject);

                SaveResutlImageToStorage(new ResultImage(imageId, updatedResultImage, percent, cachedResultImage.ContentType,
                    cachedResultImage.ImageVersion + 1));
            }

            _imageStorage.UpdatePercent(imageId, 100);
        }


        private void SaveResutlImageToStorage(ResultImage resultImage)
        {
            _imageStorage.Save(resultImage.ImageId, resultImage,DateTime.Now.AddHours(1));
        }
        
        private static int CalculatePercentProcessed(Bitmap image, Rectangle foundRectangle)
        {
            return (int)((double)(foundRectangle.TopLeft.X + foundRectangle.TopLeft.Y * image.Width) / (image.Height * image.Width) * 100);
        }

        private int GenerateImageId()
        {
            return _imageIdGenerator.Next(1, int.MaxValue);
        }

        private ImageProgressResponse CreateBadRequestResponse(ImageProgressResponse response, IList<string> requestValidationErrors)
        {
            if(response is null) throw new ArgumentException(nameof(response));
            
            response.ErrorMessage = string.Join(",", requestValidationErrors);
            response.HttpStatusCode = HttpStatusCode.BadRequest;
            return response;
        }

        private ImageProgressResponse CreateInternalServerErrorResponse(ImageProgressResponse response)
        {
            if(response is null) throw new ArgumentException(nameof(response));
            
            response.HttpStatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessage = "Internal server error";
            return response;
        }
    }
}
