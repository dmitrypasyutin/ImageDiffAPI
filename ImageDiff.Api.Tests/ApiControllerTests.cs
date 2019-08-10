using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Mime;
using ImageDiff.Api.Controllers;
using ImageDiff.Api.Infrastructure.Response;
using ImageDiff.Api.Infrastructure.Utilities.Abstractions;
using ImageDiff.Api.Infrastructure.Validators;
using ImageDiff.CommonAbstractions;
using ImageDiff.Data;
using ImageDiff.Services.Storages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Rectangle = ImageDiff.Data.Rectangle;

namespace Tests
{
    public class ApiControllerTests
    {
        private readonly Mock<IDiffObjectsFinder> _objectsFinderMock = new Mock<IDiffObjectsFinder>();
        private readonly Mock<IResultImageStorage<int>> _imageStorageMock = new Mock<IResultImageStorage<int>>();
        
        private readonly Mock<IRequestValidator> _requestValidatorSuccessMock = new Mock<IRequestValidator>();
        private readonly Mock<IRequestValidator> _requestValidatorFilesFailureMock = new Mock<IRequestValidator>();
        private readonly Mock<IRequestValidator> _requestValidatorImagesFailureMock = new Mock<IRequestValidator>();
        private readonly Mock<IRequestValidator> _requestValidatorExceptionMock = new Mock<IRequestValidator>();
        
        private readonly Mock<IImageGenerator> _imageGeneratorMock = new Mock<IImageGenerator>();
        private readonly Mock<IFormFileUtilities> _formFileUtilitiesMock = new Mock<IFormFileUtilities>();

        [SetUp]
        public void Setup()
        {
            _imageStorageMock.Setup(f => f.Get(It.Is<int>(ff => ff == 0))).Returns(
                new ResultImage(1, new byte[] {}, 100, "image/bmp", 1));
            _imageStorageMock.Setup(f => f.Get(It.Is<int>(ff => ff == 1))).Returns<ResultImage>(null);
            _imageStorageMock.Setup(f => f.Get(It.Is<int>(ff => ff == 2))).Throws(new System.Exception());

            _requestValidatorSuccessMock.Setup(f => f.ValidateFiles(It.IsAny<IFormFile[]>())).Returns(new List<string>() { });
            _requestValidatorSuccessMock.Setup(f => f.ValidateImages(It.IsAny<Bitmap[]>())).Returns(new List<string>() { });
            _requestValidatorFilesFailureMock.Setup(f => f.ValidateFiles(It.IsAny<IFormFile[]>())).Returns(new List<string>() { "error" });
            _requestValidatorImagesFailureMock.Setup(f => f.ValidateFiles(It.IsAny<IFormFile[]>())).Returns(new List<string>() { });
            _requestValidatorImagesFailureMock.Setup(f => f.ValidateImages(It.IsAny<Bitmap[]>())).Returns(new List<string>() { "error" });
            _requestValidatorExceptionMock.Setup(f => f.ValidateFiles(It.IsAny<IFormFile[]>())).Throws(new Exception());
            
            _formFileUtilitiesMock.Setup(f => f.SerializeFile(It.IsAny<IFormFile>())).Returns(new byte[] {});
            _formFileUtilitiesMock.Setup(f => f.ConvertFilesToImages(It.IsAny<IFormFile[]>())).Returns(new Bitmap[] { new Bitmap(100, 100), new Bitmap(100, 100)});

            _objectsFinderMock.Setup(f => f.FindAllDiffObjects(It.IsAny<Bitmap>(), It.IsAny<Bitmap>()))
                .Returns(new Rectangle[] {new Rectangle(new ImagePixel(1, 1), new ImagePixel(4, 4))});
        }

        [Test]
        public void GetImage_Returns_Image()
        {
            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorSuccessMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.GetImage(0);

            Assert.That(response, Is.TypeOf<FileContentResult>());
            Assert.That(((FileContentResult)response).ContentType == "image/bmp");
        }

        [Test]
        public void GetImage_Returns_NotFound()
        {
            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorSuccessMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.GetImage(1);

            Assert.That(response, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public void GetImage_Returns_InternalServerError()
        {
            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorSuccessMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.GetImage(2);

            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf<StatusCodeResult>());
            Assert.That(((StatusCodeResult)response).StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void GetImageProgress_Returns_ImageProgress()
        {
            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorSuccessMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.GetImageProgress(0);

            CheckNullTypeAndCode(response, 200);
            Assert.That(((response.Result as ObjectResult).Value as ImageProgressResponse).Percent == 100);
        }

        [Test]
        public void GetImageProgress_Returns_NotFound()
        {
            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorSuccessMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.GetImageProgress(1);
            CheckNullTypeAndCode(response, 404);
        }

        [Test]
        public void GetImageProgress_Returns_InternalServerError_When_Exception_Occurs()
        {
            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorSuccessMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.GetImageProgress(2);
            CheckNullTypeAndCode(response, 500);
        }

        [Test]
        public void CopmareImagesSync_Returns_ImageProgress()
        {
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.ContentType).Returns("image/bmp");

            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorSuccessMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.CompareImagesSync(new IFormFile[] { formFileMock.Object, formFileMock.Object });
            CheckNullTypeAndCode(response, 200);
            Assert.That(((response.Result as ObjectResult).Value as ImageProgressResponse).Percent == 100);
            Assert.That(((response.Result as ObjectResult).Value as ImageProgressResponse).ImageId != 0);
        }

        [Test]
        public void CopmareImagesSync_Returns_BadRequest_When_Files_Wrong()
        {
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.ContentType).Returns("image/bmp");

            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorFilesFailureMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.CompareImagesSync(new[] { formFileMock.Object });
            CheckNullTypeAndCode(response, 400);
            Assert.That(((response.Result as ObjectResult).Value as ImageProgressResponse).ErrorMessage.Length > 0);
        }

        [Test]
        public void CopmareImagesSync_Returns_BadRequest_When_Images_Wrong()
        {
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.ContentType).Returns("image/bmp");

            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorImagesFailureMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.CompareImagesSync(new[] { formFileMock.Object });
            CheckNullTypeAndCode(response, 400);
            Assert.That(((response.Result as ObjectResult).Value as ImageProgressResponse).ErrorMessage.Length > 0);
        }

        [Test]
        public void CopmareImagesSync_Returns_InternalServerError_When_Exception_Occurs()
        {
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.ContentType).Returns("image/bmp");

            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorExceptionMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.CompareImagesSync(new[] { formFileMock.Object });
            CheckNullTypeAndCode(response, 500);
        }

        [Test]
        public void CopmareImagesAsync_Returns_ImageProgress()
        {
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.ContentType).Returns("image/bmp");

            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorSuccessMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.CompareImagesAsync(new IFormFile[] { formFileMock.Object, formFileMock.Object });
            CheckNullTypeAndCode(response, 200);
            Assert.That(((response.Result as ObjectResult).Value as ImageProgressResponse).Percent == 0);
            Assert.That(((response.Result as ObjectResult).Value as ImageProgressResponse).ImageId != 0);
        }
        
        
        [Test]
        public void CopmareImagesAsync_Returns_BadRequest_When_Files_Wrong()
        {
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.ContentType).Returns("image/bmp");

            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorFilesFailureMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.CompareImagesAsync(new[] { formFileMock.Object });
            CheckNullTypeAndCode(response, 400);
            Assert.That(((response.Result as ObjectResult).Value as ImageProgressResponse).ErrorMessage.Length > 0);
        }
        
        [Test]
        public void CopmareImagesAsync_Returns_BadRequest_When_Images_Wrong()
        {
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.ContentType).Returns("image/bmp");

            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorImagesFailureMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.CompareImagesAsync(new[] { formFileMock.Object });
            CheckNullTypeAndCode(response, 400);
            Assert.That(((response.Result as ObjectResult).Value as ImageProgressResponse).ErrorMessage.Length > 0);
        }
        
        [Test]
        public void CopmareImagesAsync_Returns_InternalServerError_When_Exception_Occurs()
        {
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.ContentType).Returns("image/bmp");

            var controller = new ApiController(_objectsFinderMock.Object, _imageStorageMock.Object,
                _requestValidatorExceptionMock.Object, _imageGeneratorMock.Object, _formFileUtilitiesMock.Object);

            var response = controller.CompareImagesAsync(new[] { formFileMock.Object });
            CheckNullTypeAndCode(response, 500);
        }
        
        private void CheckNullTypeAndCode<T>(ActionResult<T> response, int httpCode)
        {
            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf<ActionResult<ImageProgressResponse>>());
            Assert.That(((ObjectResult)response.Result).StatusCode, Is.EqualTo(httpCode));
        }
    }
}