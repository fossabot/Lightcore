﻿using Lightcore.Hosting.Middleware;
using Lightcore.Kernel.Configuration;
using Lightcore.Kernel.Data;
using Lightcore.Kernel.Pipelines.Request;
using Lightcore.Kernel.Pipelines.Startup;
using Lightcore.Server;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;

namespace Lightcore.Hosting
{
    public static class StartupExtensions
    {
        public static IConfiguration BuildLightcoreConfiguration(this IApplicationEnvironment appEnv, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder(appEnv.ApplicationBasePath);

            // Add main config file
            builder.AddJsonFile("lightcore.json");

            // Add optional environment specific config file, will be merged and duplicates takes precedence
            builder.AddJsonFile($"lightcore.{env.EnvironmentName}.json", true);

            // Add environment variables, will be merged and duplicates takes precedence - Set LightcoreConfig:ServerUrl in Azure for example...
            builder.AddEnvironmentVariables();

            return builder.Build();
        }

        public static void AddLightcore(this IServiceCollection services, IConfiguration config)
        {
            // Add MVC services
            services.AddMvc();

            // Add Lightcore configuration so it can be used as a dependency IOptions<LightcoreConfig> config
            services.Configure<LightcoreConfig>(config.GetSection("LightcoreConfig"));

            // Add Lightcore services
            services.AddSingleton<IItemProvider, LightcoreApiItemProvider>();
            services.AddSingleton<StartupPipeline>();
            services.AddSingleton<RequestPipeline>();
        }

        public static IApplicationBuilder UseLightcore(this IApplicationBuilder app)
        {
            // Add pipelines that runs on each request
            app.UseMiddleware<StartupPipelineMiddleware>();
            app.UseMiddleware<RequestPipelineMiddleware>();

            // Enabled MVC
            app.UseMvc(routes =>
            {
                // Configure default route to always use the Lightcore controller and call the Render action
                routes.MapRoute("default", "{*path}", new
                {
                    controller = "Lightcore",
                    action = "Render"
                });
            });

            return app;
        }
    }
}