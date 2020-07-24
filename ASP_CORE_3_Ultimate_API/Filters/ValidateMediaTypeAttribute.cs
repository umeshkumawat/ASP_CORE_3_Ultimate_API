using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployee.Filters
{
    public class ValidateMediaTypeAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var hasCustomHeaderPresent = context.HttpContext.Request.Headers.ContainsKey("Accept");
            if (!hasCustomHeaderPresent)
            {
                context.Result = new BadRequestObjectResult("Accept header is missing");
                return;
            }

            var mediaType = context.HttpContext.Request.Headers["Accept"].FirstOrDefault();

            if (!MediaTypeHeaderValue.TryParse(mediaType, out var outMediaType))
            {
                context.Result = new BadRequestObjectResult($"Media type not present.Please add Accept header with the required media type.");
                return;
            }

            context.HttpContext.Items.Add("AcceptHeaderMediaType", outMediaType);
            await next();
        }
    }
}
