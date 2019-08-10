using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ImageDiff.Api.Infrastructure.Response
{
    public abstract class BaseResponse
    {
        public string ErrorMessage { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public ActionResult ToHttpResponse()
        {
            return new ObjectResult(this)
            {
                StatusCode = (int)HttpStatusCode
            };
        }
    }
}