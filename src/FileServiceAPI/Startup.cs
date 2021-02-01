using FileServiceAPI.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileServiceAPI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
            services.AddControllers();

            ConfigureSwaggerServices(services);

            services.AddSingleton<InMemoryFileService>();

            services.AddSingleton<IFileMetadataService>(sp => sp.GetRequiredService<InMemoryFileService>());
            services.AddSingleton<IFileBlobService>(sp => sp.GetRequiredService<InMemoryFileService>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            ConfigureSwaggerMiddleware(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void ConfigureSwaggerMiddleware(IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(cfg =>
            {
                //cfg.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                //{
                //    swaggerDoc.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>()
                //    {
                //        new Microsoft.OpenApi.Models.OpenApiServer()
                //        {
                //            Description = "Default",
                //            Url = httpReq.GetUri().GetLeftPart(UriPartial.Authority)
                //        }
                //    };
                //});
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(cfg =>
            {
                cfg.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Program.AppName} {ThisAssembly.AssemblyInformationalVersion}");
            });
        }

        private static void ConfigureSwaggerServices(IServiceCollection services)
        {
            services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = Program.AppName,
                    Version = ThisAssembly.AssemblyInformationalVersion
                });

                IncludeXmlComments(cfg, System.Reflection.Assembly.GetExecutingAssembly());

                //cfg.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.ActionDescriptor.RouteValues["action"]}");

                // Workaround for nullable refs/enums
                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/861
                cfg.UseAllOfToExtendReferenceSchemas();
            });

            // https://github.com/domaindrivendev/Swashbuckle.AspNetCore#systemtextjson-stj-vs-newtonsoft
            //services.AddSwaggerGenNewtonsoftSupport();
        }

        private static void IncludeXmlComments(SwaggerGenOptions options, System.Reflection.Assembly assembly)
        {
            var path = System.IO.Path.GetDirectoryName(assembly.Location);
            options.IncludeXmlComments(System.IO.Path.Combine(path, $"{assembly.GetName().Name}.xml"));
        }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
