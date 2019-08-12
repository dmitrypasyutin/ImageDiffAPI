using System.IO;
using ImageDiff.Api.Infrastructure;
using ImageDiff.Api.Infrastructure.Extensions;
using ImageDiff.Api.Infrastructure.Utilities;
using ImageDiff.Api.Infrastructure.Utilities.Abstractions;
using ImageDiff.Api.Infrastructure.Validators;
using ImageDiff.CommonAbstractions;
using ImageDiff.Core;
using ImageDiff.Services;
using ImageDiff.Services.DiffObjectFinders;
using ImageDiff.Services.ImageComparers;
using ImageDiff.Services.ImageGenerators;
using ImageDiff.Services.PixelComparers;
using ImageDiff.Services.Storages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace ImageDiff.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddScoped(typeof(IPixelComparer), typeof(ARGBPixelComparer));
            services.AddScoped(typeof(IBitmapComparer), typeof(BitmapComparer));
            services.AddScoped(typeof(IDiffObjectsFinder), typeof(BreadthFirstDiffObjectsFinder));
            services.AddScoped(typeof(IStorage<,>), typeof(MemoryCacheStorage<,>));
            services.AddScoped(typeof(IResultImageStorage<>), typeof(MemoryCacheResultImageStorage<>));
            services.AddScoped(typeof(IRequestValidator), typeof(DefaultRequestValidator));
            services.AddScoped(typeof(IImageGenerator), typeof(DefaultImageGenerator));
            services.AddScoped(typeof(IFormFileUtilities), typeof(DefaultFormFileUtilities));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureExceptionHandler();
            
            app.UseMvc(routes =>
            {
                routes.MapAreaRoute(
                    name: "WebPages",
                    areaName: "WebPage",
                    template: "{action}",
                    defaults:new
                    {
                        controller = "WebPage"
                    }
                );
            });
        }
    }
}
