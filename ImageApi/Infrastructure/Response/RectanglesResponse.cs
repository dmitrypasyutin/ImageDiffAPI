using System.Net;
using ImageDiff.Data;

namespace ImageDiff.Api.Infrastructure.Response
{
    public class RectanglesResponse : BaseResponse
    {
        public RectanglesResponse()
        {
            HttpStatusCode = HttpStatusCode.OK;
        }

        public Rectangle[] Rectangles { get; set; }
    }
}