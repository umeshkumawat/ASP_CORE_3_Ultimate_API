using Contracts;
using Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployee.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDbContext (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("Default"), sqlOpt => sqlOpt.MigrationsAssembly("CompanyEmployee"));
            });
        }

        public static void ConfigureExceptionMiddleware(this IApplicationBuilder app, ILoggerManager logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async ctx =>
                {
                    var exception = ctx.Features.Get<IExceptionHandlerFeature>();

                    if (exception != null)
                    {
                        logger.LogError($"Something went wrong: {exception.Error}");

                        ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        ctx.Response.ContentType = "application/json";

                        await ctx.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            Message = "Internal Server Error"
                        }));
                    }
                });
            });
        }

        public static void AddCustomMediaType(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(opt => 
            {
                var jsonOutputFormatter = opt.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if(jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.umesh.hateoas+json");
                }

                var xmlOutputFormatter = opt.OutputFormatters.OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

                if(xmlOutputFormatter != null)
                {
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.umesh.hateoas+json");
                }
            });
        }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
        }
    }
}
