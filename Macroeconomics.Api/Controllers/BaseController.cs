using Azure.Core;
using Azure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;


namespace Macroeconomics.Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        public string baseUrl
        {
            get
            {
                var obj = this.Request?.HttpContext?.Request;
                return $"{obj?.Scheme ?? ""}://{obj?.Host.ToString() ?? ""}{obj?.PathBase ?? ""}";
            }
        }

        //protected ILogger Log => base._logger;
        //protected void LogRequestInfo() => Log.Info($"Request recieved from {Request?.HttpContext.Connection.RemoteIpAddress.ToString() ?? "none"} at {ControllerContext?.HttpContext?.Request.Path.ToString() ?? "none"}");
        protected IActionResult MethodNotAllowed(string desciption = "")
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed, desciption);
        }
        protected IActionResult MethodNotFound(string desciption = "")
        {
            return new MethodNotFound();
        }
        protected System.Net.IPAddress? GetIPAddress()
        {
            return HttpContext?.Connection?.RemoteIpAddress ?? null;
        }

        //protected IActionResult PagedResult<T>(PagedResponse<T> res) where T : class
        //{
        //    if (res == null || Response == null)
        //    {
        //        return Ok("");
        //    }
        //    else
        //    {
        //        // total, qos
        //        Response.Headers.Add("Total", res.Total.ToString());

        //        if (res.QoS == null)
        //            Response.Headers.Add("QoS", "");
        //        else
        //            Response.Headers.Add("QoS", $"TimeTaken: {res.QoS?.TimeTaken}, Hops: {res.QoS?.Hops}");

        //        return Ok(res.Data);
        //    }
        //}
        //protected IActionResult PaginatedOk<T>(PageResponse<T> result)
        //{
        //    Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
        //    Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");

        //    return Ok(result.Items);
        //}

        protected IActionResult Created(object id)
        {
            var url = $"{this.Request?.Scheme}://{this.Request?.Host}{this.Request?.Path}/{id}";
            return Created(url, id);
        }

    }

    [DefaultStatusCode(404)]
    public class MethodNotFound : StatusCodeResult
    {
        private const int DefaultStatusCode = StatusCodes.Status404NotFound;

        /// <summary>
        /// Creates a new <see cref="Microsoft.AspNetCore.Mvc.NotFoundResult"/> instance.
        /// </summary>
        public MethodNotFound() : base(DefaultStatusCode)
        {
        }
    }
}