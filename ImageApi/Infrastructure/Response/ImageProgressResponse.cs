using System.Net;

namespace ImageDiff.Api.Infrastructure.Response
{
    public class ImageProgressResponse : BaseResponse
    {
        public int ImageId { get; set; }

        public int Percent { get; set; }

        public int ImageVersion { get; set; }
        
        public ImageProgressResponse()
        {
            HttpStatusCode = HttpStatusCode.OK;
        }
    }
}