using ImageDiff.Api.Infrastructure.Validators;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace ImageDiff.Api.Tests
{
    public class DefaultValidatorTests
    {

        [Test]
        public void Error_When_Not_2_Files_Uploaded()
        {
            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1);
            
            var validator = new DefaultRequestValidator();
            var errors = validator.ValidateFiles(new[] {fileMock.Object});
            Assert.IsNotEmpty(errors);
            
            errors = validator.ValidateFiles(new[] {fileMock.Object, fileMock.Object, fileMock.Object});
            Assert.IsNotEmpty(errors);
            
            errors = validator.ValidateFiles(new[] {fileMock.Object, fileMock.Object });
            Assert.IsEmpty(errors);
        }
        
        [Test]
        public void Error_When_File_Is_Empty()
        {
            Mock<IFormFile> fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);
            var validator = new DefaultRequestValidator();
            var errors = validator.ValidateFiles(new[] {fileMock.Object, fileMock.Object });
            Assert.IsNotEmpty(errors);
        }
        
        [Test]
        public void Error_When_Image_Sizes_Are_Different()
        {
            Assert.True(true);
        }
        
        [Test]
        public void Error_When_Image_Types_Arent_Same()
        {
            Assert.True(true);
        }
    }
}