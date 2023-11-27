using Microsoft.OpenApi.Models;
using Macroeconomics.BLS;
using Macroeconomics.DAL;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Common.DAL;
using Common.BLS;
using Microsoft.Net.Http.Headers;

namespace Macroeconomics.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => {
                options.AddPolicy(name: "CORS_Everyone", builder =>
                {
                    builder.WithOrigins("*")
                     .WithHeaders(HeaderNames.ContentType, "x-custom-header")
                     .WithMethods("POST", "GET", "OPTIONS")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddConsulConfig(Configuration);
            services.AddMacroDal(Configuration);
            services.AddMacroBLS();
            services.AddControllers();
            services.AddMemoryCache();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("SwaggerApi", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Swagger API",
                    Description = "A simple example ASP.NET Core Web API"
                });
                c.EnableAnnotations();
            });
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new MediaTypeApiVersionReader("version"),
                    new HeaderApiVersionReader("X-Version")
                    );
                options.ReportApiVersions = true;
            });

           

            services.AddHealthChecks().AddCheck<ApiHealthCheck>("Macro Indicator MicroService Health Check", tags: new string[] { "Indicator health api" });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(
                options => options
                .SetIsOriginAllowed(x => _ = true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
            app.UseCors("CORS_Everyone");
            app.UseConsul();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.AddMacroShared(Configuration);
            app.AddCommonsShared(Configuration);
            //app.UseTestMethods();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/SwaggerApi/swagger.json", "Swagger API");
            });
            

            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        }
    }
}
