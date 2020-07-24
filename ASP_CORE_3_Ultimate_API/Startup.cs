using System.IO;
using AutoMapper;
using CompanyEmployee.DTO;
using CompanyEmployee.Extensions;
using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using Reposiory;
using Reposiory.DataShapping;

namespace CompanyEmployee
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureDbContext(Configuration);

            services.AddAutoMapper(typeof(Startup));

            services.AddScoped<IRepositoryManager, RepositoryManager>();

            services.AddScoped<ILoggerManager, LoggerManager>();

            services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();


            // Browser से आने वाले "Accept" header के अनुसार response भेजना। e.g. 
            services.AddControllers(opt => opt.RespectBrowserAcceptHeader = true)
                .AddXmlDataContractSerializerFormatters()
                .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //services.AddCustomMediaType();

            services.ConfigureVersioning();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureExceptionMiddleware(logger);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
